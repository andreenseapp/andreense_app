import json
import os
import re
from pathlib import Path

pages_root = Path(__file__).resolve().parents[1] / "ModuloMonolitico.Web" / "Pages"
skip_dirs = {"Shared", "Components"}

CONTEXT_RENAME = {"Auth": "Autenticacao"}
# (context parts after index strip, page leaf) -> leaf name
PAGE_RENAME = {
    ("Principal",): "Hub",
}


def default_page_name(parts: list[str]) -> str:
    p = list(parts)
    if p and p[-1].lower() == "index":
        p = p[:-1]
    if not p:
        return "/"
    return "/" + "/".join(p)


def canonical_route(parts: list[str]) -> str:
    p = list(parts)
    if p and p[-1].lower() == "index":
        p = p[:-1]
    if p and p[0] in CONTEXT_RENAME:
        p[0] = CONTEXT_RENAME[p[0]]
    key = tuple(p)
    if key in PAGE_RENAME:
        if len(p) == 1:
            p = [p[0], PAGE_RENAME[key]]
        else:
            p[-1] = PAGE_RENAME[key]
    if not p:
        return "/"
    return "/Contextos/" + "/".join(p)


def read_atpage(path: Path) -> str | None:
    for line in path.read_text(encoding="utf-8").splitlines():
        line = line.strip()
        if line.startswith("@page"):
            m = re.match(r'@page\s*(?:"([^"]+)")?', line)
            return m.group(1) if m and m.group(1) else None
    return None


def to_csharp_name(route: str) -> str:
    if route == "/":
        return "Index"
    s = route.removeprefix("/Contextos/").replace("/", "_").replace("{", "").replace("}", "")
    s = re.sub(r"[^A-Za-z0-9_]", "", s)
    parts = s.split("_")
    return "".join(p[:1].upper() + p[1:] for p in parts if p)


entries = []
for dirpath, dirnames, filenames in os.walk(pages_root):
    dirnames[:] = [d for d in dirnames if d not in skip_dirs]
    for fn in filenames:
        if not fn.endswith(".cshtml") or fn.startswith("_"):
            continue
        full = Path(dirpath) / fn
        rel = full.relative_to(pages_root).as_posix()
        parts = rel.replace(".cshtml", "").split("/")
        entries.append(
            {
                "file": rel,
                "pagina_padrao": default_page_name(parts),
                "rota_canonica": canonical_route(parts),
                "atpage_atual": read_atpage(full),
            }
        )

entries.sort(key=lambda e: e["rota_canonica"])
out = pages_root.parent / "Core" / "_rotas_generated.json"
out.write_text(json.dumps(entries, indent=2, ensure_ascii=False), encoding="utf-8")
print(f"Wrote {len(entries)} entries to {out}")
