/* ============================================================
   GameFrameY 模块说明系列 — 共享脚本
   依赖：无（离线可用，file:// 直接打开）
   ============================================================ */
"use strict";

/* ---------------- 系列页面清单（侧边导航的数据源） ---------------- */
var SERIES = [
  { file: "index.html",    num: "00", short: "总览",     accent: "#6366f1", title: "总览 · 架构地图" },
  { file: "core.html",     num: "01", short: "核心层",   accent: "#0ea5e9", title: "GameFrameY.Core · 模块管理与服务发现" },
  { file: "gameframex.html", num: "02", short: "组件底座", accent: "#f59e0b", title: "GameFrameX · 组件底座" },
  { file: "asset.html",    num: "03", short: "资源",     accent: "#10b981", title: "Asset & YooAsset · 资源管理" },
  { file: "ui-scene.html", num: "04", short: "UI/场景",  accent: "#ec4899", title: "UI 与场景" },
  { file: "network.html",  num: "05", short: "网络",     accent: "#8b5cf6", title: "Network · 网络与协议" },
  { file: "foundation.html", num: "06", short: "异步/DI", accent: "#14b8a6", title: "UniTask & VContainer · 异步与 DI 底座" },
  { file: "config.html",   num: "07", short: "配置",     accent: "#f97316", title: "Luban · 配置管线" },
  { file: "server.html",   num: "08", short: "服务端",   accent: "#64748b", title: "GameFrameX.Server · 服务端概览" }
];

/* ---------------- 小工具 ---------------- */
function $(sel, root) { return (root || document).querySelector(sel); }
function $$(sel, root) { return Array.prototype.slice.call((root || document).querySelectorAll(sel)); }
function esc(s) {
  return String(s).replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;").replace(/"/g, "&quot;");
}
function el(tag, attrs, children) {
  var node = document.createElement(tag);
  if (attrs) {
    Object.keys(attrs).forEach(function (k) {
      if (k === "class") node.className = attrs[k];
      else if (k === "text") node.textContent = attrs[k];
      else if (k === "html") node.innerHTML = attrs[k];
      else if (k.indexOf("on") === 0) node.addEventListener(k.slice(2), attrs[k]);
      else node.setAttribute(k, attrs[k]);
    });
  }
  (children || []).forEach(function (c) { if (c != null) node.appendChild(c); });
  return node;
}
/* 演示日志：demoLog(el, 'info', '...') */
function dlog(box, kind, msg) {
  var line = el("div", { class: "log-line l-" + (kind || "info") });
  var t = new Date();
  var ts = ("0" + t.getMinutes()).slice(-2) + ":" + ("0" + t.getSeconds()).slice(-2) + "." + ("00" + t.getMilliseconds()).slice(-3);
  line.textContent = "[" + ts + "] " + msg;
  box.appendChild(line);
  box.scrollTop = box.scrollHeight;
}

/* ---------------- 主题 ---------------- */
(function initTheme() {
  var saved = null;
  try { saved = localStorage.getItem("gfy-theme"); } catch (e) { /* file:// 某些环境禁用 storage */ }
  var theme = saved || (window.matchMedia && window.matchMedia("(prefers-color-scheme: dark)").matches ? "dark" : "light");
  document.documentElement.setAttribute("data-theme", theme);
})();

function toggleTheme() {
  var cur = document.documentElement.getAttribute("data-theme") === "dark" ? "light" : "dark";
  document.documentElement.setAttribute("data-theme", cur);
  try { localStorage.setItem("gfy-theme", cur); } catch (e) { }
  var btn = $("#theme-btn");
  if (btn) btn.textContent = cur === "dark" ? "☀" : "🌙";
}

/* ---------------- 代码高亮（轻量 tokenizer） ---------------- */
var HL = (function () {
  var KW = {
    cs: "public private protected internal static readonly sealed abstract virtual override partial class struct interface enum void new return async await using namespace var null true false this base where get set params const is as in out ref try catch finally throw if else for foreach while switch case default break continue yield operator implicit explicit explicit record delegate event",
    bash: "if then else fi for in do done while case esac function return set local export echo cd exit",
    bat: "echo set setlocal endlocal if exist neq equ goto call exit rem",
    json: "true false null",
    yaml: "true false null yes no on off",
    proto: "message service rpc returns syntax option package import enum repeated optional oneof map",
    text: ""
  };
  function rules(lang) {
    var kw = KW[lang] || "";
    var list = [];
    if (lang === "cs" || lang === "proto") {
      list.push({ re: /\/\/[^\n]*/, cls: "c-com" });
      list.push({ re: /\/\*[\s\S]*?\*\//, cls: "c-com" });
      list.push({ re: /@?"(?:[^"\\]|\\[\s\S])*"?/, cls: "c-str" });
      list.push({ re: /'(?:[^'\\]|\\[\s\S])*'?/, cls: "c-str" });
      list.push({ re: /\[[A-Za-z][\w.]*(?:\([^\]]*?\))?[^\]]*\]/, cls: "c-attr" });
      if (kw) list.push({ re: new RegExp("\\b(" + kw.trim().split(/\s+/).join("|") + ")\\b"), cls: "c-kw" });
      list.push({ re: /\b[A-Z][A-Za-z0-9_]*\b/, cls: "c-type" });
      list.push({ re: /\b\d[\d_]*(?:\.\d+)?[fdulLmM]?\b/, cls: "c-num" });
    } else if (lang === "json") {
      list.push({ re: /"(?:[^"\\]|\\[\s\S])*"(?=\s*:)/, cls: "c-key" });
      list.push({ re: /"(?:[^"\\]|\\[\s\S])*"/, cls: "c-str" });
      list.push({ re: /\b(?:true|false|null)\b/, cls: "c-kw" });
      list.push({ re: /-?\b\d+(?:\.\d+)?(?:[eE][+-]?\d+)?\b/, cls: "c-num" });
    } else if (lang === "xml") {
      list.push({ re: /<!--[\s\S]*?-->/, cls: "c-com" });
      list.push({ re: /<\/?[A-Za-z_][\w.:-]*/, cls: "c-tag" });
      list.push({ re: /"(?:[^"]*)"/, cls: "c-str" });
      list.push({ re: /[\w.:-]+(?==")/, cls: "c-key" });
      list.push({ re: /[?>/]/, cls: "c-tag" });
    } else if (lang === "yaml") {
      list.push({ re: /#[^\n]*/, cls: "c-com" });
      list.push({ re: /"(?:[^"\\]|\\[\s\S])*"?/, cls: "c-str" });
      list.push({ re: /^[\w.$-]+(?=\s*:)/m, cls: "c-key" });
      list.push({ re: /\b(?:true|false|null|yes|no|on|off)\b/, cls: "c-kw" });
      list.push({ re: /\b\d+(?:\.\d+)?\b/, cls: "c-num" });
    } else if (lang === "bash" || lang === "bat" || lang === "sh") {
      list.push({ re: lang === "bat" ? /(?:^|\s)(?:rem|::)[^\n]*/ : /#[^\n]*/, cls: "c-com" });
      list.push({ re: /"(?:[^"\\]|\\[\s\S])*"?/, cls: "c-str" });
      list.push({ re: /'(?:[^'\\]|\\[\s\S])*'?/, cls: "c-str" });
      if (kw) list.push({ re: new RegExp("\\b(" + kw.trim().split(/\s+/).join("|") + ")\\b"), cls: "c-kw" });
      list.push({ re: /(?:^|\s)--?[\w-]+/, cls: "c-flag" });
      list.push({ re: /\$\{?[\w@#?*]+\}?|%[^%]+%/, cls: "c-num" });
    } else {
      list.push({ re: /#[^\n]*/, cls: "c-com" });
      list.push({ re: /"(?:[^"\\]|\\[\s\S])*"?/, cls: "c-str" });
      list.push({ re: /\b\d+(?:\.\d+)?\b/, cls: "c-num" });
    }
    return list;
  }
  function highlight(code, lang) {
    var rs = rules(lang).map(function (r) { return { re: new RegExp(r.re.source, r.re.flags.replace("m", "") + "y"), cls: r.cls }; });
    var out = "";
    var i = 0, n = code.length;
    while (i < n) {
      var matched = false;
      for (var k = 0; k < rs.length; k++) {
        var r = rs[k];
        r.re.lastIndex = i;
        var m = r.re.exec(code);
        if (m && m.index === i && m[0].length > 0) {
          out += '<span class="' + r.cls + '">' + esc(m[0]) + "</span>";
          i += m[0].length;
          matched = true;
          break;
        }
      }
      if (!matched) {
        var ch = code.slice(i, i + 1);
        out += esc(ch);
        i += 1;
      }
    }
    return out;
  }
  return { highlight: highlight };
})();

function enhanceCode() {
  $$("pre.codeblock").forEach(function (pre) {
    var code = pre.querySelector("code");
    if (!code || code.getAttribute("data-hl")) return;
    code.setAttribute("data-hl", "1");
    var lang = (code.getAttribute("data-lang") || "text").toLowerCase();
    code.innerHTML = HL.highlight(code.textContent, lang);
    var btn = el("button", { class: "copy-btn", text: "复制" });
    btn.addEventListener("click", function () {
      var text = code.textContent;
      function done() { btn.textContent = "已复制 ✓"; btn.classList.add("done"); setTimeout(function () { btn.textContent = "复制"; btn.classList.remove("done"); }, 1600); }
      if (navigator.clipboard && navigator.clipboard.writeText) {
        navigator.clipboard.writeText(text).then(done, function () { legacyCopy(text); done(); });
      } else { legacyCopy(text); done(); }
    });
    pre.appendChild(btn);
  });
}
function legacyCopy(text) {
  var ta = el("textarea", { style: "position:fixed;opacity:0;left:-999px" });
  ta.value = text;
  document.body.appendChild(ta);
  ta.select();
  try { document.execCommand("copy"); } catch (e) { }
  document.body.removeChild(ta);
}

/* ---------------- Tabs ---------------- */
function initTabs() {
  $$(".tabs").forEach(function (tabs) {
    var btns = $$(".tab-btn", tabs);
    var panels = $$(".tab-panel", tabs);
    btns.forEach(function (btn, idx) {
      btn.addEventListener("click", function () {
        btns.forEach(function (b) { b.classList.remove("active"); });
        panels.forEach(function (p) { p.classList.remove("active"); });
        btn.classList.add("active");
        if (panels[idx]) panels[idx].classList.add("active");
      });
    });
    if (!btns.some(function (b) { return b.classList.contains("active"); }) && btns.length) btns[0].classList.add("active");
    if (btns.length && !panels.some(function (p) { return p.classList.contains("active"); }) && panels.length) panels[0].classList.add("active");
  });
}

/* ---------------- 侧边栏 ---------------- */
function buildSidebar() {
  var sb = $("#sidebar");
  if (!sb) return;
  var current = document.body.getAttribute("data-page") || location.pathname.split("/").pop() || "index.html";
  var brand = el("a", { class: "sidebar-brand", href: "index.html" }, [
    el("span", { class: "brand-logo", text: "Y" }),
    el("span", {}, [
      el("div", { class: "brand-title", text: "GameFrameY" }),
      el("div", { class: "brand-sub", text: "模块说明系列 · v0.1" })
    ])
  ]);
  var page = SERIES.filter(function (p) { return p.file === current; })[0] || SERIES[0];
  document.documentElement.style.setProperty("--accent", page.accent);

  var searchInput = el("input", { type: "text", placeholder: "搜索本页内容…", id: "nav-search" });
  var searchBox = el("div", { class: "sidebar-search" }, [
    el("span", { class: "search-icon", text: "🔍" }),
    searchInput,
    el("kbd", { text: "/" })
  ]);

  var nav = el("div", { class: "sidebar-nav", id: "sidebar-nav" });
  var sectionLabel = el("div", { class: "nav-section-label", text: "系列章节" });
  var pageList = el("div", {});
  SERIES.forEach(function (p) {
    var a = el("a", { class: "nav-page" + (p.file === current ? " active" : ""), href: p.file, title: p.title }, [
      el("span", { class: "num", text: p.num }),
      el("span", { text: p.short })
    ]);
    pageList.appendChild(a);
  });

  var tocLabel = el("div", { class: "nav-section-label", text: "本页目录" });
  var toc = el("div", { class: "nav-toc", id: "page-toc" });
  $$("h2[id]").forEach(function (h) {
    toc.appendChild(el("a", { href: "#" + h.id, text: h.textContent.replace(/^\s*\d+(\.\d+)?\s*/, "") }));
  });

  nav.appendChild(sectionLabel);
  nav.appendChild(pageList);
  if (toc.children.length) { nav.appendChild(tocLabel); nav.appendChild(toc); }
  nav.appendChild(el("div", { class: "search-empty nav-hidden", id: "search-empty", text: "没有匹配的章节" }));

  var themeBtn = el("button", { class: "icon-btn", id: "theme-btn", title: "切换明暗主题", onclick: toggleTheme,
    text: document.documentElement.getAttribute("data-theme") === "dark" ? "☀" : "🌙" });
  var foot = el("div", { class: "sidebar-foot" }, [
    el("div", { class: "meta", html: "源码：<a href='https://github.com/'>D:\\GameFrameY</a><br>Unity 2022.3 · .NET 10" }),
    themeBtn
  ]);

  sb.appendChild(brand);
  sb.appendChild(searchBox);
  sb.appendChild(nav);
  sb.appendChild(foot);

  /* 搜索过滤（章节名 + 本页目录） */
  searchInput.addEventListener("input", function () {
    var q = searchInput.value.trim().toLowerCase();
    var any = false;
    $$(".nav-page", pageList).forEach(function (a, i) {
      var hit = !q || SERIES[i].title.toLowerCase().indexOf(q) >= 0 || SERIES[i].short.toLowerCase().indexOf(q) >= 0;
      a.classList.toggle("nav-hidden", !hit);
      if (hit) any = true;
    });
    $$("#page-toc a").forEach(function (a) {
      var hit = !q || a.textContent.toLowerCase().indexOf(q) >= 0;
      a.classList.toggle("nav-hidden", !hit);
      if (hit) any = true;
    });
    tocLabel.classList.toggle("nav-hidden", !!q);
    $("#search-empty").classList.toggle("nav-hidden", any || !q);
  });
  document.addEventListener("keydown", function (e) {
    if (e.key === "/" && document.activeElement !== searchInput) {
      e.preventDefault();
      searchInput.focus();
    }
  });

  /* 本页目录的滚动高亮 */
  if (toc.children.length && "IntersectionObserver" in window) {
    var links = $$("#page-toc a");
    var map = {};
    links.forEach(function (a) { map[a.getAttribute("href").slice(1)] = a; });
    var obs = new IntersectionObserver(function (entries) {
      entries.forEach(function (en) {
        if (en.isIntersecting) {
          links.forEach(function (a) { a.classList.remove("active"); });
          var a = map[en.target.id];
          if (a) a.classList.add("active");
        }
      });
    }, { rootMargin: "-10% 0px -80% 0px" });
    $$("h2[id]").forEach(function (h) { obs.observe(h); });
  }

  /* 移动端菜单 */
  var fab = el("button", { class: "icon-btn menu-fab", text: "☰", title: "目录" });
  fab.addEventListener("click", function () { sb.classList.toggle("open"); });
  document.body.appendChild(fab);
  document.addEventListener("click", function (e) {
    if (sb.classList.contains("open") && !sb.contains(e.target) && e.target !== fab) sb.classList.remove("open");
  });
}

/* ---------------- 页脚导航 ---------------- */
function buildPageNav() {
  var holder = $(".pagenav");
  if (!holder) return;
  var current = document.body.getAttribute("data-page") || "index.html";
  var idx = SERIES.findIndex(function (p) { return p.file === current; });
  if (idx < 0) return;
  if (idx > 0) {
    var p = SERIES[idx - 1];
    holder.appendChild(el("a", { href: p.file }, [
      el("span", { class: "pn-label", text: "← 上一章" }),
      el("span", { class: "pn-title", text: p.num + " " + p.title.split("·")[0].trim() })
    ]));
  }
  if (idx < SERIES.length - 1) {
    var n = SERIES[idx + 1];
    holder.appendChild(el("a", { class: "next", href: n.file }, [
      el("span", { class: "pn-label", text: "下一章 →" }),
      el("span", { class: "pn-title", text: n.num + " " + n.title.split("·")[0].trim() })
    ]));
  }
}

document.addEventListener("DOMContentLoaded", function () {
  buildSidebar();
  enhanceCode();
  initTabs();
  buildPageNav();
});
