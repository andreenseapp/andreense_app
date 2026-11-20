#!/usr/bin/env python3
"""Gera Manifesto_Mapa_Contextos_Pastas.html — matriz de pastas por camada."""
from __future__ import annotations

import os
import re
from datetime import datetime
from pathlib import Path

ROOT = Path(r"C:\Projeto\Projeto")
MONO = ROOT / "Sistema" / "Modulo_Monolitico"
OUT = ROOT / "Documentação" / "Manifestos" / "Manifesto_Mapa_Contextos_Pastas.html"

LAYER_PATHS = {
    "web_pages": MONO / "ModuloMonolitico.Web" / "Pages",
    "web_contextos": MONO / "ModuloMonolitico.Web" / "wwwroot" / "Contextos",
    "app": MONO / "ModuloMonolitico.Aplicacao" / "CasosDeUso",
    "dom_repo": MONO / "ModuloMonolitico.Dominio" / "InterfacesDeRepositorio",
    "dom_ent": MONO / "ModuloMonolitico.Dominio" / "Entidades",
    "infra_repo": MONO / "ModuloMonolitico.Infraestrutura" / "AcessoADados" / "Repositorios",
    "infra_di": MONO / "ModuloMonolitico.Infraestrutura" / "InjecaoDependencia" / "Contextos",
}

# Contexto canônico (rota /Contextos/X) → nomes físicos por camada
# Chaves ordenadas alfabeticamente na saída
CONTEXT_MAP: dict[str, dict[str, list[str] | str]] = {
    "Agendamentos": {
        "rota": "/Contextos/Agendamentos",
        "web": ["Agendamentos"], "app": ["Agendamentos"], "dom": ["Agendamentos"],
        "infra": ["Agendamentos"], "di": "AgendamentosSetup",
    },
    "Api": {
        "rota": "/Contextos/Api",
        "web": ["Api"], "app": [], "dom": [], "infra": [], "di": None,
        "nota": "Endpoints auxiliares (seletores, CEP)",
    },
    "AreaServicos": {
        "rota": "/Contextos/AreaServicos",
        "web": ["AreaServicos"], "app": [], "dom": [], "infra": [], "di": "ServicosSetup",
    },
    "Autenticacao": {
        "rota": "/Contextos/Autenticacao",
        "web": ["Auth"], "app": ["Auth"], "dom": ["Auth"], "infra": ["Autenticacao"],
        "di": "AutenticacaoSetup",
        "nota": "Pages=Auth · Infra=Autenticacao",
    },
    "Auth (Grupos)": {
        "rota": "/Contextos/Auth/Grupos",
        "web": ["Auth/Grupos"], "app": ["Auth/Grupos"], "dom": ["Auth"], "infra": ["Autenticacao"],
        "di": "AutenticacaoSetup",
    },
    "Base": {
        "rota": "/Contextos/Base",
        "web": ["Base"], "app": [], "dom": ["Base"], "infra": ["Base"], "di": None,
        "nota": "Entidades transversais (Conta, Auditoria SaaS)",
    },
    "Cidadania": {
        "rota": "/Contextos/Cidadania",
        "web": ["Cidadania", "Cidade"], "app": [], "dom": ["Cidadania"], "infra": ["Cidadania"],
        "di": "CidadaniaSetup",
    },
    "Contatos": {
        "rota": "/Contextos/Contatos",
        "web": ["Contatos"], "app": ["Contatos"], "dom": ["Contatos"], "infra": ["Contatos"],
        "di": "ContatosSetup",
    },
    "Empresas": {
        "rota": "/Contextos/Empresas",
        "web": ["Empresas"], "app": ["Empresas"], "dom": ["Empresas"], "infra": ["Empresas"],
        "di": "EmpresasSetup",
    },
    "ERP": {
        "rota": "/Contextos/ERP",
        "web": ["ERP"], "app": ["ERP"], "dom": ["ERP"], "infra": ["Erp"],
        "di": "ErpSetup",
        "nota": "Infra pasta Erp (case divergente)",
    },
    "Estoque": {
        "rota": "/Contextos/Estoque",
        "web": ["Estoque", "ERP/Estoque", "ERP/UnidadesMedida"],
        "app": ["Estoque"], "dom": ["Estoque"], "infra": ["Estoque", "Erp"],
        "di": "EstoqueSetup",
        "nota": "ProdutoRepositorio em Infra/Estoque; Categoria/Marca em Erp",
    },
    "Fidelizacao": {
        "rota": "/Contextos/Fidelizacao",
        "web": ["Fidelizacao"], "app": ["Fidelizacao"], "dom": ["Fidelizacao"],
        "infra": ["Fidelizacao"], "di": "FidelizacaoSetup",
    },
    "Financeiro": {
        "rota": "/Contextos/Financeiro",
        "web": ["Financeiro"], "app": [], "dom": [], "infra": [], "di": "FinanceiroSetup",
    },
    "Fiscal": {
        "rota": "/Contextos/Fiscal",
        "web": ["Fiscal"], "app": [], "dom": ["Fiscal"], "infra": ["Fiscal"],
        "di": "FiscalSetup",
    },
    "Geral": {
        "rota": "/Contextos/Geral",
        "web": ["Geral"], "app": ["Geral"], "dom": ["Gerais"], "infra": ["Geral"],
        "di": "GeralSetup",
        "nota": "Domínio usa Gerais (plural)",
    },
    "Gestao": {
        "rota": "/Contextos/Gestao",
        "web": ["Gestao"], "app": ["Gestao"], "dom": ["Gestao"], "infra": ["Gestao"],
        "di": "GestaoSetup",
        "nota": "DocumentoRepositorio em Infra/Gestao; IDocumento em Dom/InfraAdmin",
    },
    "HelpDesk": {
        "rota": "/Contextos/HelpDesk",
        "web": ["HelpDesk"], "app": [], "dom": [], "infra": [], "di": "HelpDeskSetup",
    },
    "InfraAdmin": {
        "rota": "/Contextos/InfraAdmin",
        "web": ["InfraAdmin"], "app": ["InfraAdmin"], "dom": ["InfraAdmin"],
        "infra": ["Gestao"], "di": None,
        "nota": "DocumentoRepositorio em Infra/Gestao; sem *Setup dedicado",
    },
    "Logistica": {
        "rota": "/Contextos/Logistica",
        "web": ["Logistica"], "app": [], "dom": [], "infra": [], "di": "LogisticaSetup",
    },
    "Mensagens": {
        "rota": "/Contextos/Mensagens",
        "web": ["Mensagens"], "app": [], "dom": ["Gerais"], "infra": ["Geral"],
        "di": "GeralSetup",
        "nota": "MensagensRepositorio em Infra/Geral",
    },
    "Notificacoes": {
        "rota": "/Contextos/Notificacoes",
        "web": ["Notificacoes"], "app": [], "dom": ["Gerais"], "infra": ["Geral"],
        "di": "GeralSetup",
    },
    "Perfil": {
        "rota": "/Contextos/Perfil",
        "web": ["Perfil"], "app": [], "dom": [], "infra": [], "di": None,
    },
    "Precos": {
        "rota": "/Contextos/Precos",
        "web": ["Precos", "ERP/ContextosPreco"], "app": ["Precos"], "dom": ["Precos"],
        "infra": ["Precos"], "di": "PrecosSetup",
    },
    "Principal": {
        "rota": "/Contextos/Principal",
        "web": ["Principal"], "app": [], "dom": [], "infra": [], "di": None,
    },
    "SaaS": {
        "rota": "/Contextos/SaaS",
        "web": ["SaaS", "InfraAdmin/SaaS"], "app": ["SaaS"], "dom": ["SaaS"],
        "infra": ["SaaS"], "di": "SaaSSetup",
    },
    "Servicos": {
        "rota": "(Agendamentos/Prestadores)",
        "web": [], "app": [], "dom": ["Servicos"], "infra": [], "di": "ServicosSetup",
        "nota": "Entidade Prestador; sem pasta Web dedicada",
    },
    "Varejo": {
        "rota": "/Contextos/Varejo",
        "web": ["Varejo"], "app": [], "dom": [], "infra": [], "di": "VarejoSetup",
    },
}

SKIP_WEB = {"Shared", "Error"}
SKIP_INFRA = {"Base"}


def dirs_at(base: Path) -> set[str]:
    if not base.is_dir():
        return set()
    return {p.name for p in base.iterdir() if p.is_dir()}


def exists_rel(base: Path, rel: str) -> bool:
    return (base / rel.replace("/", os.sep)).is_dir()


def count_cs(base: Path, rel: str) -> int:
    target = base / rel.replace("/", os.sep)
    if not target.is_dir():
        return 0
    return sum(1 for _ in target.rglob("*.cs"))


def count_assets_in_pages(web_paths: list[str]) -> tuple[int, int]:
    css = js = 0
    pages = LAYER_PATHS["web_pages"]
    for rel in web_paths:
        folder = pages / rel.replace("/", os.sep)
        if folder.is_dir():
            css += sum(1 for _ in folder.rglob("*.css"))
            js += sum(1 for _ in folder.rglob("*.js"))
    return css, js


def count_assets_contextos(canonical: str) -> tuple[int, int]:
    folder = LAYER_PATHS["web_contextos"] / canonical
    if not folder.is_dir():
        return 0, 0
    css = sum(1 for _ in folder.rglob("*.css"))
    js = sum(1 for _ in folder.rglob("*.js"))
    return css, js


def di_exists(setup: str | None) -> bool:
    if not setup:
        return False
    return (LAYER_PATHS["infra_di"] / setup).is_file()


def status_cell(ok: bool, detail: str = "", warn: bool = False) -> str:
    if ok and warn:
        return f'<span class="warn">⚠ {detail}</span>' if detail else '<span class="warn">⚠</span>'
    if ok:
        return f'<span class="ok">✔ {detail}</span>' if detail else '<span class="ok">✔</span>'
    return '<span class="miss">✘</span>'


def layer_detail(base: Path, paths: list[str]) -> tuple[bool, str, bool]:
    """Returns (exists, label, has_naming_warning)."""
    if not paths:
        return False, "", False
    found = [p for p in paths if exists_rel(base, p)]
    if not found:
        return False, ", ".join(paths), False
    warn = len(found) < len(paths) or (len(set(paths)) > 1 and found[0] != paths[0])
    label = found[0] if len(found) == 1 else f"{found[0]} (+{len(found)-1})"
    return True, label, warn


def detect_gaps(ctx: str, cfg: dict, layers: dict) -> list[str]:
    gaps = []
    web_ok = layers["web_ok"]
    app_ok = layers["app_ok"]
    dom_ok = layers["dom_repo_ok"]
    infra_ok = layers["infra_ok"]
    di_ok = layers["di_ok"]
    asset_ok = layers["asset_ok"]

    ui_only = {"Api", "Base", "Financeiro", "HelpDesk", "Logistica", "Perfil", "Principal", "Varejo", "AreaServicos"}
    infra_only_di = {"Financeiro", "HelpDesk", "Logistica", "Servicos", "Varejo", "AreaServicos"}

    if web_ok and not app_ok and ctx not in ui_only:
        gaps.append("Sem CasosDeUso")
    if web_ok and not dom_ok and cfg.get("dom"):
        gaps.append("Sem IRepositorio/Entidade")
    if web_ok and not infra_ok and cfg.get("infra"):
        gaps.append("Sem Repositorio Infra")
    if (web_ok or app_ok) and cfg.get("di") and not di_ok:
        gaps.append("Sem DI Setup")
    if web_ok and not asset_ok:
        gaps.append("Sem wwwroot/Contextos")
    if app_ok and not web_ok and cfg.get("web"):
        gaps.append("App sem Pages")
    if infra_ok and not dom_ok:
        gaps.append("Infra sem Dominio")
    if di_ok and ctx in infra_only_di and not web_ok:
        gaps.append("DI Setup sem UI")
    return gaps


def orphan_list(title: str, all_dirs: set[str], mapped: set[str], skip: set[str]) -> str:
    orphans = sorted(all_dirs - mapped - skip)
    if not orphans:
        return ""
    items = "".join(f"<li><code>{o}</code></li>" for o in orphans)
    return f'<div class="alert-card"><div class="alert-title">⚠ {title}</div><ul class="orphan-list">{items}</ul></div>'


def build() -> str:
    web_all = dirs_at(LAYER_PATHS["web_pages"])
    app_all = dirs_at(LAYER_PATHS["app"])
    dom_repo_all = dirs_at(LAYER_PATHS["dom_repo"])
    dom_ent_all = dirs_at(LAYER_PATHS["dom_ent"])
    infra_all = dirs_at(LAYER_PATHS["infra_repo"])
    ctx_assets_all = dirs_at(LAYER_PATHS["web_contextos"])
    di_all = {p.stem.replace("Setup", "") for p in LAYER_PATHS["infra_di"].glob("*Setup.cs")} if LAYER_PATHS["infra_di"].is_dir() else set()

    rows_html = ""
    full_stack = 0
    with_gaps = 0
    naming_issues = 0

    mapped_web: set[str] = set()
    mapped_app: set[str] = set()
    mapped_dom: set[str] = set()
    mapped_infra: set[str] = set()

    for ctx, cfg in sorted(CONTEXT_MAP.items()):
        w_paths: list[str] = cfg.get("web", [])
        a_paths: list[str] = cfg.get("app", [])
        d_paths: list[str] = cfg.get("dom", [])
        i_paths: list[str] = cfg.get("infra", [])

        for p in w_paths:
            mapped_web.add(p.split("/")[0])
        for p in a_paths:
            mapped_app.add(p.split("/")[0])
        for p in d_paths:
            mapped_dom.add(p)
        for p in i_paths:
            mapped_infra.add(p)

        web_ok, web_lbl, web_warn = layer_detail(LAYER_PATHS["web_pages"], w_paths)
        app_ok = any(exists_rel(LAYER_PATHS["app"], p) for p in a_paths) if a_paths else False
        app_count = sum(count_cs(LAYER_PATHS["app"], p) for p in a_paths)
        dom_repo_ok = any(exists_rel(LAYER_PATHS["dom_repo"], p) for p in d_paths) if d_paths else False
        dom_ent_ok = any(exists_rel(LAYER_PATHS["dom_ent"], p) for p in d_paths) if d_paths else False
        infra_ok = any(exists_rel(LAYER_PATHS["infra_repo"], p) for p in i_paths) if i_paths else False
        di_ok = di_exists(cfg.get("di"))
        di_lbl = cfg.get("di") or "—"

        canonical_asset = ctx.split(" ")[0]
        asset_folder_ok = canonical_asset in ctx_assets_all
        css_pages, js_pages = count_assets_in_pages(w_paths)
        css_ctx, js_ctx = count_assets_contextos(canonical_asset)
        asset_ok = asset_folder_ok
        asset_detail = f"ctx:{css_ctx}c/{js_ctx}j · pg:{css_pages}c/{js_pages}j"

        layers = dict(web_ok=web_ok, app_ok=app_ok, dom_repo_ok=dom_repo_ok,
                      infra_ok=infra_ok, di_ok=di_ok, asset_ok=asset_folder_ok)
        gaps = detect_gaps(ctx, cfg, layers)
        if gaps:
            with_gaps += 1
        if web_ok and app_ok and dom_repo_ok and infra_ok:
            full_stack += 1

        nota = cfg.get("nota", "")
        rota = cfg.get("rota", "")
        if web_warn or (w_paths and web_lbl and w_paths[0] != web_lbl.split(" ")[0]):
            naming_issues += 1

        gap_html = ", ".join(gaps) if gaps else "—"
        row_class = "gap-warn" if gaps else ""
        nota_html = f'<br><small class="muted">{nota}</small>' if nota else ""

        rows_html += f"""          <tr class="{row_class}">
            <td><strong>{ctx}</strong>{nota_html}<br><small class="muted">{rota}</small><br><small class="layer-path">Web: {', '.join(w_paths) or '—'}</small></td>
            <td>{status_cell(web_ok, web_lbl, web_warn)}</td>
            <td>{status_cell(app_ok, f'{app_count} arq' if app_ok else '')}</td>
            <td>{status_cell(dom_repo_ok, 'Repo')} {status_cell(dom_ent_ok, 'Ent')}</td>
            <td>{status_cell(infra_ok, ', '.join(i_paths) if infra_ok else '')} {status_cell(di_ok, di_lbl.replace('Setup','') if di_ok else '')}</td>
            <td>{status_cell(asset_folder_ok, asset_detail)}</td>
            <td>{gap_html}</td>
          </tr>
"""

    total = len(CONTEXT_MAP)
    now = datetime.now().strftime("%d/%m/%Y %H:%M")

    orphans = (
        orphan_list("Web/Pages — pastas top-level fora do mapa", web_all, mapped_web, SKIP_WEB)
        + orphan_list("Aplicacao/CasosDeUso — pastas órfãs", app_all, mapped_app, set())
        + orphan_list("Dominio/InterfacesDeRepositorio — pastas órfãs", dom_repo_all, mapped_dom, set())
        + orphan_list("Infraestrutura/Repositorios — pastas órfãs", infra_all, mapped_infra, SKIP_INFRA)
    )

    di_orphans = sorted(di_all - {c.get("di", "").replace("Setup", "") for c in CONTEXT_MAP.values() if c.get("di")})
    if di_orphans:
        items = "".join(f"<li><code>{d}Setup.cs</code></li>" for d in di_orphans)
        orphans += f'<div class="alert-card warn-card"><div class="alert-title">ℹ DI Setups sem contexto mapeado na matriz</div><ul class="orphan-list">{items}</ul></div>'

    return f"""<!DOCTYPE html>
<html lang="pt-BR">
<head>
  <meta charset="UTF-8">
  <meta name="viewport" content="width=device-width, initial-scale=1.0">
  <title>Mapa de Contextos e Pastas — Monolito</title>
  <style>
    @import url('https://fonts.googleapis.com/css2?family=Outfit:wght@300;400;600;700&display=swap');
    body {{ background:#090d16; color:#e2e8f0; font-family:'Outfit',sans-serif; margin:0; padding:40px 20px; display:flex; justify-content:center; }}
    .developer-dashboard {{ background:#0f172a; padding:32px; border-radius:16px; border:1px solid #1e293b; max-width:1440px; width:100%; box-shadow:0 20px 40px rgba(0,0,0,.4); }}
    .dashboard-header {{ display:flex; justify-content:space-between; align-items:center; border-bottom:2px solid #1e293b; padding-bottom:24px; margin-bottom:24px; flex-wrap:wrap; gap:16px; }}
    .header-info h2 {{ margin:0 0 6px; font-size:28px; font-weight:700; background:linear-gradient(135deg,#38bdf8,#818cf8); -webkit-background-clip:text; -webkit-text-fill-color:transparent; }}
    .header-info p {{ margin:0; font-size:14px; color:#94a3b8; }}
    .stats-container {{ display:flex; gap:16px; flex-wrap:wrap; }}
    .stat-badge {{ background:rgba(30,41,59,.7); border:1px solid #334155; padding:10px 20px; border-radius:12px; text-align:center; min-width:90px; }}
    .stat-val {{ display:block; font-size:24px; font-weight:700; color:#38bdf8; }}
    .stat-lbl {{ font-size:11px; color:#64748b; text-transform:uppercase; letter-spacing:.5px; }}
    .legend-row {{ display:flex; flex-wrap:wrap; gap:16px; margin-bottom:24px; background:rgba(15,23,42,.6); padding:16px; border-radius:10px; border:1px solid rgba(255,255,255,.02); font-size:13px; color:#cbd5e1; }}
    .ok {{ color:#34d399; font-weight:600; }}
    .miss {{ color:#f87171; font-weight:700; }}
    .warn {{ color:#fbbf24; font-weight:600; }}
    .muted {{ color:#64748b; font-size:11px; }}
    .gap-warn {{ background:rgba(245,158,11,.04); }}
    .hub-section {{ background:rgba(30,41,59,.4); border:1px solid #1e293b; border-radius:12px; margin-bottom:24px; overflow:hidden; }}
    .hub-header {{ background:rgba(30,41,59,.8); padding:16px 20px; border-bottom:1px solid #1e293b; }}
    .hub-title {{ font-size:18px; font-weight:600; }}
    .table-container {{ padding:16px; overflow-x:auto; }}
    .dashboard-table {{ width:100%; border-collapse:collapse; font-size:13px; }}
    .dashboard-table th {{ text-align:left; padding:12px 14px; color:#64748b; font-weight:600; border-bottom:1px solid #1e293b; text-transform:uppercase; font-size:10px; letter-spacing:.5px; white-space:nowrap; }}
    .dashboard-table td {{ padding:12px 14px; border-bottom:1px solid rgba(30,41,59,.5); vertical-align:top; }}
    .dashboard-table tr:hover {{ background:rgba(255,255,255,.02); }}
    .alert-card {{ background:rgba(239,68,68,.08); border:1px solid rgba(239,68,68,.2); border-radius:12px; padding:16px 20px; margin-bottom:16px; }}
    .warn-card {{ background:rgba(245,158,11,.08); border-color:rgba(245,158,11,.2); }}
    .alert-title {{ font-weight:600; color:#f87171; margin-bottom:8px; }}
    .warn-card .alert-title {{ color:#fbbf24; }}
    .orphan-list {{ margin:0; padding-left:20px; color:#fca5a5; font-size:13px; }}
    .warn-card .orphan-list {{ color:#fcd34d; }}
    .orphan-list code {{ background:rgba(0,0,0,.3); padding:2px 6px; border-radius:4px; }}
    .note {{ font-size:12px; color:#94a3b8; margin-top:24px; line-height:1.7; }}
    .layer-path {{ font-family:Consolas,monospace; font-size:10px; color:#64748b; }}
  </style>
</head>
<body>
<div class="developer-dashboard">
  <div class="dashboard-header">
    <div class="header-info">
      <h2>Mapa de Contextos × Pastas</h2>
      <p>Domínio → Infraestrutura → Aplicação → Web — detecta pastas ausentes, fora do contexto ou com nomenclatura divergente</p>
      <p class="layer-path">Gerado em {now} · ModuloMonolitico · Script: Scripts_Auxiliares/gerar_mapa_contextos_pastas.py</p>
    </div>
    <div class="stats-container">
      <div class="stat-badge"><span class="stat-val">{total}</span><span class="stat-lbl">Contextos</span></div>
      <div class="stat-badge"><span class="stat-val">{full_stack}</span><span class="stat-lbl">Stack completa</span></div>
      <div class="stat-badge"><span class="stat-val">{with_gaps}</span><span class="stat-lbl">Com lacunas</span></div>
      <div class="stat-badge"><span class="stat-val">{len(ctx_assets_all)}</span><span class="stat-lbl">wwwroot/Contextos</span></div>
    </div>
  </div>

  <div class="legend-row">
    <span><span class="ok">✔</span> Presente</span>
    <span><span class="miss">✘</span> Ausente</span>
    <span><span class="warn">⚠</span> Nome divergente entre camadas</span>
    <span>Assets: <code>wwwroot/Contextos/{{ctx}}</code> · co-localizados em <code>Pages/**/*.cshtml.css|js</code></span>
  </div>

  <div class="hub-section">
    <div class="hub-header"><div class="hub-title">📁 Matriz por Bounded Context</div></div>
    <div class="table-container">
      <table class="dashboard-table">
        <thead>
          <tr>
            <th>Contexto / Rota</th>
            <th>Web Pages</th>
            <th>Aplicação</th>
            <th>Domínio</th>
            <th>Infraestrutura</th>
            <th>Assets</th>
            <th>Lacunas</th>
          </tr>
        </thead>
        <tbody>
{rows_html}
        </tbody>
      </table>
    </div>
  </div>

  <div class="hub-section">
    <div class="hub-header"><div class="hub-title">🔍 Pastas órfãs e DI sem par</div></div>
    <div class="table-container">
      {orphans or '<p class="muted">Nenhuma pasta órfã detectada.</p>'}
    </div>
  </div>

  <div class="hub-section">
    <div class="hub-header"><div class="hub-title">📐 Caminhos físicos por camada</div></div>
    <div class="table-container">
      <table class="dashboard-table">
        <thead><tr><th>Camada</th><th>Padrão de pasta</th></tr></thead>
        <tbody>
          <tr><td>Domínio — Interfaces</td><td class="layer-path">ModuloMonolitico.Dominio/InterfacesDeRepositorio/{{Contexto}}/</td></tr>
          <tr><td>Domínio — Entidades</td><td class="layer-path">ModuloMonolitico.Dominio/Entidades/{{Contexto}}/</td></tr>
          <tr><td>Infra — Repositories</td><td class="layer-path">ModuloMonolitico.Infraestrutura/AcessoADados/Repositorios/{{Contexto}}/</td></tr>
          <tr><td>Infra — DI</td><td class="layer-path">ModuloMonolitico.Infraestrutura/InjecaoDependencia/Contextos/{{Contexto}}Setup.cs</td></tr>
          <tr><td>Aplicação</td><td class="layer-path">ModuloMonolitico.Aplicacao/CasosDeUso/{{Contexto}}/Comandos|Consultas/</td></tr>
          <tr><td>Web — Pages</td><td class="layer-path">ModuloMonolitico.Web/Pages/{{Contexto}}/</td></tr>
          <tr><td>Web — Assets (alvo)</td><td class="layer-path">ModuloMonolitico.Web/wwwroot/Contextos/{{Contexto}}/css|js/</td></tr>
        </tbody>
      </table>
    </div>
  </div>

  <p class="note">
    <strong>Principais inconsistências de nomenclatura:</strong><br>
    • <code>Auth</code> (Web/App/Dom) ↔ <code>Autenticacao</code> (Infra) — rota canônica <code>/Contextos/Autenticacao</code><br>
    • <code>ERP</code> (Dom/App/Web) ↔ <code>Erp</code> (Infra) — Categoria/Marca/UnidadeMedida em Erp; Produto em Estoque<br>
    • <code>Geral</code> (Web/App/Infra) ↔ <code>Gerais</code> (Dom) — Mensagens/Notificações/Auditoria<br>
    • <code>InfraAdmin</code> — repositório de documentos em <code>Infra/Gestao/DocumentoRepositorio</code><br>
    • <code>wwwroot/Contextos/</code> — pasta alvo do manifesto ainda <strong>não existe</strong>; assets hoje em Pages co-localizados e wwwroot/css|js globais<br>
    • DI setups existentes sem UI: Financeiro, HelpDesk, Logistica, Servicos, Varejo<br>
    • Rotas canônicas: <code>ModuloMonolitico.Web/Core/ModuloRotas.Paginas.cs</code>
  </p>
</div>
</body>
</html>
"""


def main() -> None:
    html = build()
    OUT.parent.mkdir(parents=True, exist_ok=True)
    OUT.write_text(html, encoding="utf-8")
    print(f"OK: {OUT} ({len(html)} bytes)")


if __name__ == "__main__":
    main()
