"""纯 Python 生成消消乐美术资源（PNG，无第三方依赖）。

用法: python tools/gen_match3_art.py
输出: Client/Assets/Bundles/Atlases/*.png（YooAsset 收集，地址 = 文件名）
重新生成只需重跑本脚本，改调色/尺寸在下方 GEMS/常量里调。
"""
import math
import os
import struct
import zlib

SIZE = 128
OUT = os.path.join(os.path.dirname(os.path.dirname(os.path.abspath(__file__))),
                   "Client", "Assets", "Bundles", "Atlases")

# 颜色: (base, light, dark) —— 径向渐变 三段
GEMS = {
    "gem_red":    ((255, 86, 86),  (255, 155, 145), (168, 28, 40)),
    "gem_orange": ((255, 158, 62), (255, 205, 130), (188, 92, 18)),
    "gem_yellow": ((255, 214, 72), (255, 240, 150), (198, 148, 8)),
    "gem_green":  ((98, 220, 90),  (172, 245, 150), (26, 138, 48)),
    "gem_blue":   ((84, 158, 255), (150, 202, 255), (26, 78, 180)),
    "gem_purple": ((176, 106, 255), (216, 172, 255), (98, 38, 180)),
}


def clamp(v, lo=0.0, hi=1.0):
    return max(lo, min(hi, v))


def smooth(t):
    t = clamp(t)
    return t * t * (3 - 2 * t)


def mix(a, b, t):
    return tuple(int(a[i] + (b[i] - a[i]) * t) for i in range(3))


def write_png(path, w, h, rows):
    def chunk(typ, data):
        return (struct.pack(">I", len(data)) + typ + data
                + struct.pack(">I", zlib.crc32(typ + data) & 0xFFFFFFFF))

    raw = b"".join(b"\x00" + bytes(r) for r in rows)
    png = b"\x89PNG\r\n\x1a\n"
    png += chunk(b"IHDR", struct.pack(">IIBBBBB", w, h, 8, 6, 0, 0, 0))
    png += chunk(b"IDAT", zlib.compress(raw, 9))
    png += chunk(b"IEND", b"")
    with open(path, "wb") as f:
        f.write(png)


def draw_gem(base, light, dark):
    cx = cy = SIZE / 2
    r = SIZE * 0.455  # 半径
    hi_cx, hi_cy, hi_rx, hi_ry = cx - SIZE * 0.16, cy - SIZE * 0.18, SIZE * 0.21, SIZE * 0.14
    rows = []
    for y in range(SIZE):
        row = bytearray()
        for x in range(SIZE):
            dx, dy = x - cx, y - cy
            d = math.sqrt(dx * dx + dy * dy)
            alpha = smooth((r - d) / 2.2)  # 2px 抗锯齿边缘
            if alpha <= 0:
                row += bytes((0, 0, 0, 0))
                continue
            t = clamp(d / r)
            # 三段径向渐变: 中心亮 -> 中段基色 -> 边缘暗
            if t < 0.55:
                col = mix(light, base, smooth(t / 0.55))
            else:
                col = mix(base, dark, smooth((t - 0.55) / 0.45))
            # 高光椭圆
            hx = (x - hi_cx) / hi_rx
            hy = (y - hi_cy) / hi_ry
            hd = math.sqrt(hx * hx + hy * hy)
            if hd < 1.0:
                col = mix(col, (255, 255, 255), 0.55 * (1 - smooth(hd)))
            # 边缘暗环
            if d > r - 6:
                col = mix(col, dark, 0.45 * (d - (r - 6)) / 6)
            row += bytes((col[0], col[1], col[2], int(255 * alpha)))
        rows.append(row)
    return rows


def rounded_rect(w, h, radius, fill, border, border_w):
    rows = []
    cx, cy = w / 2, h / 2
    for y in range(h):
        row = bytearray()
        for x in range(w):
            # 圆角距离场
            qx = abs(x - cx) - (cx - radius)
            qy = abs(y - cy) - (cy - radius)
            qx, qy = max(qx, 0), max(qy, 0)
            d = math.sqrt(qx * qx + qy * qy) - radius
            if d <= 0:
                col, a = fill, fill[3]
                if d > -border_w:  # 内描边
                    col, a = border, border[3]
            else:
                a = 0
            row += bytes(col[:3] + (int(a),)) if a else bytes((0, 0, 0, 0))
        rows.append(row)
    return rows


def main():
    os.makedirs(OUT, exist_ok=True)
    for name, (base, light, dark) in GEMS.items():
        write_png(os.path.join(OUT, name + ".png"), SIZE, SIZE, draw_gem(base, light, dark))
        print("gem:", name)
    # 格子底: 半透明圆角块 + 微弱描边
    write_png(os.path.join(OUT, "cell_bg.png"), SIZE, SIZE,
              rounded_rect(SIZE, SIZE, SIZE * 0.22, (255, 255, 255, 26), (255, 255, 255, 40), 2))
    # 结算面板底
    write_png(os.path.join(OUT, "panel_bg.png"), 256, 256,
              rounded_rect(256, 256, 64, (30, 34, 52, 242), (96, 120, 180, 255), 4))
    # 按钮底
    write_png(os.path.join(OUT, "btn_bg.png"), 256, 128,
              rounded_rect(256, 128, 58, (66, 104, 220, 235), (150, 190, 255, 255), 3))
    print("tiles: cell_bg / panel_bg / btn_bg")
    print("done ->", OUT)


if __name__ == "__main__":
    main()
