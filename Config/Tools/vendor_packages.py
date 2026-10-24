"""递归 vendored GameFrameX / VContainer 包源码为 embedded packages。

用法: python vendor_packages.py
- 从 manifest 直接依赖出发，克隆每个包（优先按版本 tag，失败退 main），解析其 package.json
  依赖并递归（跳过 com.unity.* 官方包与已存在目录）
- Tests/ -> Tests~/（避免 Unity 导入测试程序集），删除悬空 Tests.meta
"""
import json
import shutil
import subprocess
import tempfile
from pathlib import Path

DEST = Path(r"D:\GameFrameY\Client\Packages")
GFX = "https://github.com/GameFrameX/{name}.git"

# manifest 中的直接依赖（com.gameframex.unity.entry 已移除：GameApp 门面未使用，砍掉可显著缩小闭包）
DIRECT = {
    "com.gameframex.unity": "3.0.1",
    "com.gameframex.unity.asset": "5.0.0",
    "com.gameframex.unity.cysharp.unitask": "2.6.2",
    "com.gameframex.unity.event": "2.0.0",
    "com.gameframex.unity.mono": "2.0.0",
    "com.gameframex.unity.network": "3.0.0",
    "com.gameframex.unity.procedure": "2.0.0",
    "com.gameframex.unity.scene": "3.0.2",
    "com.gameframex.unity.ui": "3.0.1",
    "com.gameframex.unity.ui.ugui": "2.7.2",
    "jp.hadashikick.vcontainer": "1.19.0",
}

EXTRA_URL = {
    "jp.hadashikick.vcontainer": "https://github.com/hadashiA/VContainer.git",
}

results = {}
failures = []
todo = [(n, v, EXTRA_URL.get(n, GFX.format(name=n))) for n, v in DIRECT.items()]

while todo:
    nxt = []
    for name, ver, url in todo:
        dest = DEST / name
        if dest.exists():
            print(f"SKIP {name} (already vendored)")
            continue

        tmp = Path(tempfile.mkdtemp(prefix="vendor_"))
        tag_used = True
        r = subprocess.run(
            ["git", "clone", "--depth", "1", "--branch", ver, url, str(tmp)],
            capture_output=True, text=True)
        if r.returncode != 0:
            tag_used = False
            r = subprocess.run(
                ["git", "clone", "--depth", "1", url, str(tmp)],
                capture_output=True, text=True)
        if r.returncode != 0:
            print(f"FAIL {name}@{ver}: {r.stderr.strip().splitlines()[-1] if r.stderr.strip() else '?'}")
            failures.append((name, ver))
            shutil.rmtree(tmp, ignore_errors=True)
            continue

        pkg_dir = tmp
        if not (pkg_dir / "package.json").exists():
            found = None
            for p in tmp.rglob("package.json"):
                try:
                    data = json.loads(p.read_text(encoding="utf-8-sig"))
                except Exception:
                    continue
                if data.get("name") == name:
                    found = p.parent
                    break
            if found is None:
                print(f"FAIL {name}: repo has no package.json with this name")
                failures.append((name, ver))
                shutil.rmtree(tmp, ignore_errors=True)
                continue
            pkg_dir = found

        shutil.rmtree(tmp / ".git", ignore_errors=True)
        shutil.copytree(pkg_dir, dest)
        shutil.rmtree(tmp, ignore_errors=True)

        data = json.loads((dest / "package.json").read_text(encoding="utf-8-sig"))
        results[name] = {"version": data.get("version"), "requested": ver, "tag": tag_used}
        print(f"OK {name}: vendored {data.get('version')} (requested {ver}, by_tag={tag_used})")

        for dep, dver in (data.get("dependencies") or {}).items():
            if dep.startswith(("com.unity.", "com.gameframey.", "com.tuyoogame.")):
                continue
            if (DEST / dep).exists():
                continue
            nxt.append((dep, dver, EXTRA_URL.get(dep, GFX.format(name=dep))))
    todo = nxt

# Tests -> Tests~（vendoring 常规处理）
renamed = []
for d in DEST.iterdir():
    tests = d / "Tests"
    if tests.is_dir():
        tests.rename(d / "Tests~")
        meta = d / "Tests.meta"
        if meta.exists():
            meta.unlink()
        renamed.append(d.name)

print("\n=== vendored ===")
print(json.dumps(results, indent=1, ensure_ascii=False))
print("tests_hidden:", renamed)
print("failures:", failures)
