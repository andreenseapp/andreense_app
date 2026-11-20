window.inicializarSeletorProdutoVariacao = function (CID, APENAS_SRVS, LABEL) {
    let debounceTimer   = null;
    let dados           = [];
    let indiceAtivo        = -1;

    const el = id => document.getElementById(id);

    async function spvResolverCodigo_local() {
        const cod = el('cod_' + CID)?.value?.trim();
        if (!cod) return;
        try {
            const params = new URLSearchParams({ apenasServicos: APENAS_SRVS, q: cod });
            const resp = await window.requisitarApp('/Api/SeletorProdutoVariacao?' + params);
            if (!resp.ok) return;
            const lista = await resp.json();
            if (lista.length === 1) { spvSelecionar_local(lista[0]); return; }
            spvAbrirModal_local(cod);
        } catch {}
    }

    function spvAbrirModal_local(termoPre) {
        el('overlay_' + CID).classList.add('aberto');
        const inp = el('spv_q_' + CID);
        if (inp) {
            inp.value = typeof termoPre === 'string' ? termoPre : '';
            setTimeout(() => inp.focus(), 80);
        }
        spvBuscar_local(typeof termoPre === 'string' ? termoPre : '');
        document.addEventListener('keydown', spvOuvinteTeclaEsc_local);
    }

    function spvFecharModal_local() {
        el('overlay_' + CID).classList.remove('aberto');
        document.removeEventListener('keydown', spvOuvinteTeclaEsc_local);
        indiceAtivo = -1;
    }

    function spvOuvinteTeclaEsc_local(e) {
        if (e.key === 'Escape') spvFecharModal_local();
    }

    function spvAtrasoBusca_local(termo) {
        clearTimeout(debounceTimer);
        debounceTimer = setTimeout(() => spvBuscar_local(termo), 280);
    }

    async function spvBuscar_local(termo) {
        const icon  = el('spv_icon_' + CID);
        const spin  = el('spv_spin_' + CID);
        if (icon) icon.style.display = 'none';
        if (spin) spin.style.display = 'block';

        try {
            const params = new URLSearchParams({ apenasServicos: APENAS_SRVS });
            if (termo?.trim()) params.set('q', termo.trim());
            const resp = await window.requisitarApp('/Api/SeletorProdutoVariacao?' + params);
            if (!resp.ok) throw new Error();
            dados = await resp.json();
            indiceAtivo = -1;
            spvRenderizar_local();
        } catch {
            el('spv_body_' + CID).innerHTML = `<div class="spv-empty">?? Erro ao buscar. Tente novamente.</div>`;
        } finally {
            if (icon) icon.style.display = 'block';
            if (spin) spin.style.display = 'none';
        }
    }

    function spvRenderizar_local() {
        const body  = el('spv_body_' + CID);
        const count = el('spv_count_' + CID);
        if (!body) return;

        if (count) count.textContent = dados.length === 0 ? 'Nenhum resultado' : `${dados.length} resultado(s)`;

        if (dados.length === 0) {
            body.innerHTML = `<div class="spv-empty">Nenhum item encontrado.</div>`;
            return;
        }

        const fmt = new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' });

        body.innerHTML = `
        <table class="spv-table">
            <thead>
                <tr>
                    <th style="width:110px">Cdigo</th>
                    <th>Descrio / Variao</th>
                    <th style="width:80px">Tipo</th>
                    <th style="width:100px;text-align:right">Preo</th>
                </tr>
            </thead>
            <tbody id="spv_tbody_${CID}">
                ${dados.map((item, i) => {
                    const isServ    = item.tipoProduto === 'servico';
                    const badgeCls  = isServ ? 'servico' : 'produto';
                    const badgeTxt  = isServ ? 'Servio' : 'Produto';
                    const partes    = (item.nomeExibicao || '').split(' • ');
                    const nomeProd  = escaparHtml(partes[0] || '');
                    const nomeVar   = partes[1] ? ` <span style="color:var(--clr-text-muted);font-size:0.8rem">• ${escaparHtml(partes[1])}</span>` : '';
                    return `<tr data-idx="${i}" data-action="selecionar">
                        <td class="mono muted">${escaparHtml(item.codigoSku || '-')}</td>
                        <td>${nomeProd}${nomeVar}</td>
                        <td><span class="spv-badge ${badgeCls}">${badgeTxt}</span></td>
                        <td style="text-align:right">${fmt.format(item.precoVenda)}</td>
                    </tr>`;
                }).join('')}
            </tbody>
        </table>`;
    }

    function spvNavegarTeclado_local(e) {
        if (e.key === 'ArrowDown') { e.preventDefault(); spvMoverIndice_local(1); }
        else if (e.key === 'ArrowUp') { e.preventDefault(); spvMoverIndice_local(-1); }
        else if (e.key === 'Enter')  { e.preventDefault(); if (indiceAtivo >= 0) spvSelecionarIndice_local(indiceAtivo); }
    }

    function spvDefinirIndice_local(i) {
        indiceAtivo = i;
        spvDestacar_local();
    }

    function spvMoverIndice_local(dir) {
        if (dados.length === 0) return;
        indiceAtivo = Math.max(0, Math.min(dados.length - 1, indiceAtivo + dir));
        spvDestacar_local();
        const tbody = el('spv_tbody_' + CID);
        if (tbody) {
            const row = tbody.querySelector(`tr[data-idx="${indiceAtivo}"]`);
            row?.scrollIntoView({ block: 'nearest' });
        }
    }

    function spvDestacar_local() {
        const tbody = el('spv_tbody_' + CID);
        if (!tbody) return;
        tbody.querySelectorAll('tr').forEach((tr, i) => {
            tr.classList.toggle('ativo', i === indiceAtivo);
        });
    }

    function spvSelecionarIndice_local(i) {
        if (i < 0 || i >= dados.length) return;
        spvSelecionar_local(dados[i]);
    }

    function spvSelecionar_local(item) {
        el('val_' + CID).value = item.id || '';
        const codEl = el('cod_' + CID);
        if (codEl) codEl.value = item.codigoSku || '';

        const nomeEl = el('nome_' + CID);
        if (nomeEl) {
            const partes = (item.nomeExibicao || '').split(' ? ');
            nomeEl.textContent = partes[0] || item.nomeExibicao || '';
            nomeEl.classList.add('selecionado');
        }

        const iconEl = el('icon_' + CID);
        if (iconEl) {
            iconEl.classList.remove('servico', 'produto');
            iconEl.classList.add(item.tipoProduto === 'servico' ? 'servico' : 'produto');
        }

        if (!el('clear_' + CID)) {
            const field  = el('wrap_' + CID)?.querySelector('.spv-field');
            const btnBus = field?.querySelector('.spv-btn');
            if (field && btnBus) {
                const cl     = document.createElement('button');
                cl.type      = 'button';
                cl.id        = 'clear_' + CID;
                cl.className = 'spv-clear';
                cl.title     = 'Remover';
                cl.dataset.action = 'limpar';
                cl.innerHTML = `<svg width="13" height="13" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5"><line x1="18" y1="6" x2="6" y2="18"/><line x1="6" y1="6" x2="18" y2="18"/></svg>`;
                field.insertBefore(cl, btnBus);
            }
        }

        el('wrap_' + CID)?.dispatchEvent(new CustomEvent('seletor:change', { detail: item, bubbles: true }));
        spvFecharModal_local();
    }

    function spvLimpar_local() {
        el('val_' + CID).value = '';
        const codEl = el('cod_' + CID);
        if (codEl) codEl.value = '';
        const nomeEl = el('nome_' + CID);
        if (nomeEl) {
            nomeEl.textContent = 'Selecionar...';
            nomeEl.classList.remove('selecionado');
        }
        const iconEl = el('icon_' + CID);
        if (iconEl) iconEl.classList.remove('servico', 'produto');
        el('clear_' + CID)?.remove();
        el('wrap_' + CID)?.dispatchEvent(new CustomEvent('seletor:change', { detail: null, bubbles: true }));
    }

    function escaparHtml(s) {
        return String(s).replace(/&/g,'&amp;').replace(/</g,'&lt;').replace(/>/g,'&gt;').replace(/"/g,'&quot;');
    }

    const wrapEl = el('wrap_' + CID);
    if (wrapEl) {
        wrapEl.addEventListener('click', (e) => {
            const botao = e.target.closest('[data-action]');
            if (!botao) return;
            const action = botao.dataset.action;
            if (action === 'abrir') {
                e.stopPropagation();
                spvAbrirModal_local();
            } else if (action === 'limpar') {
                e.stopPropagation();
                spvLimpar_local();
            }
        });
    }

    const overlayEl = el('overlay_' + CID);
    if (overlayEl) {
        overlayEl.addEventListener('click', (e) => {
            const botao = e.target.closest('[data-action]');
            if (botao) {
                const action = botao.dataset.action;
                if (action === 'fechar') {
                    spvFecharModal_local();
                } else if (action === 'selecionar') {
                    spvSelecionarIndice_local(parseInt(botao.dataset.idx));
                } else if (action === 'fechar-overlay') {
                    if (e.target === overlayEl) spvFecharModal_local();
                }
            } else if (e.target === overlayEl) {
                spvFecharModal_local();
            }
        });

        overlayEl.addEventListener('mouseover', (e) => {
            const item = e.target.closest('tr[data-idx]');
            if (item && item.dataset.idx) {
                spvDefinirIndice_local(parseInt(item.dataset.idx));
            }
        });

        const inputBusca = el('spv_q_' + CID);
        if (inputBusca) {
            inputBusca.addEventListener('input', (e) => spvAtrasoBusca_local(e.target.value));
            inputBusca.addEventListener('keydown', (e) => spvNavegarTeclado_local(e));
        }
    }
};

document.addEventListener('DOMContentLoaded', () => {
    document.querySelectorAll('.spv-wrap-component').forEach(el => {
        if (!el.dataset.initSpv) {
            el.dataset.initSpv = 'true';
            window.inicializarSeletorProdutoVariacao(el.dataset.cid, el.dataset.apenasSrvs === 'true', el.dataset.label);
        }
    });
});
