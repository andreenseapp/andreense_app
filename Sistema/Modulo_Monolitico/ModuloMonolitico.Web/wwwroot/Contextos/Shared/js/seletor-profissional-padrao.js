window.inicializarSeletorProfissional = function (CID) {
    let debounceTimer = null;
    let dados = [];
    let indiceAtivo = -1;

    const el = id => document.getElementById(id);

    function sprAbrirModal_local() {
        el('overlay_' + CID).classList.add('aberto');
        const inp = el('spr_q_' + CID);
        if (inp) { inp.value = ''; setTimeout(() => inp.focus(), 80); }
        sprBuscar_local('');
        document.addEventListener('keydown', sprOuvinteTeclaEsc_local);
    }

    function sprFecharModal_local() {
        el('overlay_' + CID).classList.remove('aberto');
        document.removeEventListener('keydown', sprOuvinteTeclaEsc_local);
        indiceAtivo = -1;
    }

    function sprOuvinteTeclaEsc_local(e) {
        if (e.key === 'Escape') sprFecharModal_local();
    }

    function sprAtrasoBusca_local(termo) {
        clearTimeout(debounceTimer);
        debounceTimer = setTimeout(() => sprBuscar_local(termo), 280);
    }

    function sprObterProdutoVariacaoId_local() {
        const input = document.querySelector('#containerServico input[name="ProdutoVariacaoId"]')
                   || document.querySelector('input[name="ProdutoVariacaoId"]');
        const val = input?.value?.trim();
        if (!val) return null;
        if (!/^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$/i.test(val)) return null;
        if (val === '00000000-0000-0000-0000-000000000000') return null;
        return val;
    }

    async function sprBuscar_local(termo) {
        const icon = el('spr_icon_' + CID);
        const spin = el('spr_spin_' + CID);
        if (icon) icon.style.display = 'none';
        if (spin) spin.style.display = 'block';

        try {
            const params = new URLSearchParams();
            if (termo?.trim()) params.set('q', termo.trim());

            const pvId = sprObterProdutoVariacaoId_local();
            if (pvId) params.set('produtoVariacaoId', pvId);

            const resp = await window.requisitarApp('/Api/SeletorProfissional?' + params);
            const contentType = resp.headers.get('content-type') || '';
            if (!resp.ok || !contentType.includes('application/json')) throw new Error('resposta_invalida');
            const json = await resp.json();
            if (json?.erro) {
                const errEl = document.createElement('div');
                errEl.className = `spr-empty`;
                errEl.textContent = '?? ' + json.erro;
                el('spr_list_' + CID).innerHTML = '';
                el('spr_list_' + CID).appendChild(errEl);
                return;
            }
            dados = Array.isArray(json) ? json : [];
            indiceAtivo = -1;
            sprRenderizar_local(pvId);
        } catch {
            el('spr_list_' + CID).innerHTML = `<div class="spr-empty">?? Erro ao buscar profissionais. Verifique se o workspace est� selecionado.</div>`;
        } finally {
            if (icon) icon.style.display = 'block';
            if (spin) spin.style.display = 'none';
        }
    }

    function sprRenderizar_local(produtoVariacaoId) {
        const list  = el('spr_list_' + CID);
        const count = el('spr_count_' + CID);
        if (!list) return;

        if (count) count.textContent = dados.length === 0 ? 'Nenhum resultado' : `${dados.length} profissional(is)`;

        const inicial = nome => (nome || '?')[0].toUpperCase();
        const escaparHtml = s => String(s).replace(/&/g,'&amp;').replace(/</g,'&lt;').replace(/>/g,'&gt;').replace(/"/g,'&quot;');

        let html = `
            <div class="spr-item-todos" data-action="selecionar-todos">
                <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M17 21v-2a4 4 0 0 0-4-4H5a4 4 0 0 0-4 4v2"/><circle cx="9" cy="7" r="4"/><path d="M23 21v-2a4 4 0 0 0-3-3.87"/><path d="M16 3.13a4 4 0 0 1 0 7.75"/></svg>
                Todos disponíveis
            </div>`;

        if (dados.length === 0) {
            const msg = produtoVariacaoId
                ? 'Nenhum profissional disponível para este serviço.'
                : 'Selecione um serviço para ver os profissionais disponíveis.';
            html += `<div class="spr-empty">${msg}</div>`;
        } else {
            html += dados.map((item, i) => {
                const tipoClass = item.tipo === 'usuario' ? 'usuario' : 'contato';
                const tipoLabel = item.tipo === 'usuario' ? 'Usuário' : 'Funcionário';
                return `<div class="spr-item" data-idx="${i}" data-action="selecionar">
                    <div class="spr-item-avatar ${tipoClass}">${escaparHtml(inicial(item.nome))}</div>
                    <div class="spr-item-info">
                        <div class="spr-item-nome">${escaparHtml(item.nome)}</div>
                        ${item.email ? `<div class="spr-item-tipo">${escaparHtml(item.email)}</div>` : ''}
                    </div>
                    <span class="spr-badge ${tipoClass}">${tipoLabel}</span>
                </div>`;
            }).join('');
        }

        list.innerHTML = html;
    }

    function sprNavegarTeclado_local(e) {
        if (e.key === 'ArrowDown') { e.preventDefault(); sprMoverIndice_local(1); }
        else if (e.key === 'ArrowUp')  { e.preventDefault(); sprMoverIndice_local(-1); }
        else if (e.key === 'Enter')    { e.preventDefault(); if (indiceAtivo >= 0) sprSelecionarIndice_local(indiceAtivo); else sprSelecionarTodos_local(); }
    }

    function sprDefinirIndice_local(i) { indiceAtivo = i; sprDestacar_local(); }

    function sprMoverIndice_local(dir) {
        if (dados.length === 0) return;
        indiceAtivo = Math.max(0, Math.min(dados.length - 1, indiceAtivo + dir));
        sprDestacar_local();
        const list = el('spr_list_' + CID);
        if (list) {
            const row = list.querySelector(`[data-idx="${indiceAtivo}"]`);
            row?.scrollIntoView({ block: 'nearest' });
        }
    }

    function sprDestacar_local() {
        const list = el('spr_list_' + CID);
        if (!list) return;
        list.querySelectorAll('.spr-item').forEach((item, i) => {
            item.classList.toggle('ativo', i === indiceAtivo);
        });
    }

    function sprSelecionarIndice_local(i) {
        if (i < 0 || i >= dados.length) return;
        const item = dados[i];
        const idRaw = item.tipo === 'usuario' ? `U|${item.id}` : `C|${item.id}`;
        sprAplicar_local(idRaw, item.nome, item.tipo, item);
    }

    function sprSelecionarTodos_local() {
        sprAplicar_local('', 'Todos disponíveis...', null, null);
    }

    function sprAplicar_local(idRaw, nome, tipo, item) {
        el('val_' + CID).value = idRaw;

        const nomeEl      = el('nome_' + CID);
        const avatarEl    = el('spr_avatar_' + CID);
        const iconFieldEl = el('spr_icon_field_' + CID);

        if (nomeEl) {
            nomeEl.textContent = item ? item.nome : 'Selecionar profissional...';
            if (item) {
                nomeEl.classList.add('preenchido');
                nomeEl.classList.remove('vazio');
            } else {
                nomeEl.classList.add('vazio');
                nomeEl.classList.remove('preenchido');
            }
        }

        if (avatarEl && iconFieldEl) {
            if (item && item.nome) {
                avatarEl.textContent = item.nome.charAt(0).toUpperCase();
                avatarEl.style.display = 'flex';
                iconFieldEl.style.display = 'none';
            } else {
                avatarEl.style.display = 'none';
                avatarEl.textContent = '';
                iconFieldEl.style.display = 'flex';
            }
        }

        const existingClear = el('clear_' + CID);
        if (item && !existingClear) {
            const field = el('wrap_' + CID)?.querySelector('.spr-field');
            const btnBus = field?.querySelector('.spr-btn');
            if (field && btnBus) {
                const cl = document.createElement('button');
                cl.type      = 'button';
                cl.id        = 'clear_' + CID;
                cl.className = 'spr-clear';
                cl.title     = 'Remover seleção';
                cl.dataset.action = 'limpar';
                cl.innerHTML = `<svg width="13" height="13" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5"><line x1="18" y1="6" x2="6" y2="18"/><line x1="6" y1="6" x2="18" y2="18"/></svg>`;
                field.insertBefore(cl, btnBus);
            }
        } else if (!idRaw && existingClear) {
            existingClear.remove();
        }

        el('wrap_' + CID)?.dispatchEvent(new CustomEvent('seletor:profissional', {
            detail: { idRaw, nome, tipo },
            bubbles: true
        }));

        sprFecharModal_local();
    }

    function sprLimpar_local() {
        sprAplicar_local('', 'Todos disponíveis...', null);
    }

    const wrapEl = el('wrap_' + CID);
    if (wrapEl) {
        wrapEl.addEventListener('click', (e) => {
            const botao = e.target.closest('[data-action]');
            if (!botao) return;
            const action = botao.dataset.action;
            if (action === 'abrir') {
                e.stopPropagation();
                sprAbrirModal_local();
            } else if (action === 'limpar') {
                e.stopPropagation();
                sprLimpar_local();
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
                    sprFecharModal_local();
                } else if (action === 'selecionar-todos') {
                    sprSelecionarTodos_local();
                } else if (action === 'selecionar') {
                    sprSelecionarIndice_local(parseInt(botao.dataset.idx));
                } else if (action === 'fechar-overlay') {
                    if (e.target === overlayEl) sprFecharModal_local();
                }
            } else if (e.target === overlayEl) {
                sprFecharModal_local();
            }
        });

        overlayEl.addEventListener('mouseover', (e) => {
            const item = e.target.closest('.spr-item');
            if (item && item.dataset.idx) {
                sprDefinirIndice_local(parseInt(item.dataset.idx));
            }
        });

        const inputBusca = el('spr_q_' + CID);
        if (inputBusca) {
            inputBusca.addEventListener('input', (e) => sprAtrasoBusca_local(e.target.value));
            inputBusca.addEventListener('keydown', (e) => sprNavegarTeclado_local(e));
        }
    }
};

document.addEventListener('DOMContentLoaded', () => {
    document.querySelectorAll('.spr-wrap-component').forEach(el => {
        if (!el.dataset.initSpr) {
            el.dataset.initSpr = 'true';
            window.inicializarSeletorProfissional(el.dataset.cid);
        }
    });
});
