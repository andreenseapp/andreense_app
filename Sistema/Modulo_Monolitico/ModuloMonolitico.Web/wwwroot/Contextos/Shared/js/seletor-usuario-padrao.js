window.inicializarSeletorUsuario = function (CID, PAPEL, LABEL) {
    let debounceTimer = null;
    let dados = [];
    let indiceAtivo = -1;
    let modoAtivo   = 'contatos';

    const el = id => document.getElementById(id);

    function seuAbrirModal_local() {
        el('overlay_' + CID).classList.add('aberto');
        const inp = el('seu_q_' + CID);
        if (inp) { inp.value = ''; setTimeout(() => inp.focus(), 80); }
        seuBuscar_local('');
        document.addEventListener('keydown', seuOuvinteTeclaEsc_local);
    }

    function seuFecharModal_local() {
        el('overlay_' + CID).classList.remove('aberto');
        document.removeEventListener('keydown', seuOuvinteTeclaEsc_local);
        indiceAtivo = -1;
    }

    function seuOuvinteTeclaEsc_local(e) {
        if (e.key === 'Escape') seuFecharModal_local();
    }

    function seuModo_local(modo) {
        modoAtivo = modo;
        const pillC = el('pill_contatos_' + CID);
        const pillV = el('pill_vinculos_' + CID);
        if (modo === 'contatos') {
            pillC?.classList.add('active');
            pillV?.classList.remove('active');
        } else {
            pillV?.classList.add('active');
            pillC?.classList.remove('active');
        }
        const termo = el('seu_q_' + CID)?.value || '';
        seuBuscar_local(termo);
    }

    function seuAtrasoBusca_local(termo) {
        clearTimeout(debounceTimer);
        debounceTimer = setTimeout(() => seuBuscar_local(termo), 280);
    }

    async function seuBuscar_local(termo) {
        const icon = el('seu_icon_' + CID);
        const spin = el('seu_spin_' + CID);
        if (icon) icon.style.display = 'none';
        if (spin) spin.style.display = 'block';

        try {
            const params = new URLSearchParams({ papel: PAPEL, modo: modoAtivo });
            if (termo?.trim()) params.set('q', termo.trim());
            const resp = await window.requisitarApp('/Api/SeletorUsuario?' + params);
            if (!resp.ok) {
                const errData = await resp.json().catch(() => null);
                if (errData && errData.erro) throw new Error(errData.erro);
                throw new Error('Erro ' + resp.status);
            }
            dados = await resp.json();
            indiceAtivo = -1;
            seuRenderizar_local();
        } catch (err) {
            const list = el('seu_list_' + CID);
            if (list) {
                list.replaceChildren();
                const div = document.createElement('div');
                div.className = 'seu-empty';
                div.style.color = '#d93025';
                div.textContent = '?? ' + (err.message || 'Erro ao buscar. Tente novamente.');
                list.appendChild(div);
            }
        } finally {
            if (icon) icon.style.display = 'block';
            if (spin) spin.style.display = 'none';
        }
    }

    function seuRenderizar_local() {
        const list  = el('seu_list_' + CID);
        const count = el('seu_count_' + CID);
        if (!list) return;

        if (count) count.textContent = dados.length === 0 ? 'Nenhum resultado' : `${dados.length} encontrado(s)`;

        const inicial = nome => (nome || '?')[0].toUpperCase();
        const escaparHtml = s => String(s).replace(/&/g,'&amp;').replace(/</g,'&lt;').replace(/>/g,'&gt;').replace(/"/g,'&quot;');

        if (dados.length === 0) {
            const msg = modoAtivo === 'vinculos'
                ? 'Nenhum v�nculo encontrado. <a href="@ModuloRotas.Base.Vinculos" style="color:var(--clr-primary)">Gerenciar v�nculos</a>'
                : 'Nenhum resultado encontrado.';
            list.innerHTML = `<div class="seu-empty">${msg}</div>`;
            return;
        }

        list.innerHTML = dados.map((item, i) => {
            const isVinculo = item.fonte === 'vinculo';
            const isPJ = item.tipoPessoa === 'juridica';
            const avatarClass = isVinculo ? 'vl' : (isPJ ? 'pj' : 'pf');
            const meta = [item.documento, item.email].filter(Boolean).join(' • ');
            const badge = isVinculo ? `<span class="seu-badge vinculo">Vínculo</span>` : '';

            return `<div class="seu-item" data-idx="${i}" data-action="selecionar">
                <div class="seu-item-avatar ${avatarClass}">${escaparHtml(inicial(item.nome))}</div>
                <div class="seu-item-info">
                    <div class="seu-item-nome">${escaparHtml(item.nome || '')}</div>
                    ${meta ? `<div class="seu-item-meta">${escaparHtml(meta)}</div>` : ''}
                </div>
                ${badge}
            </div>`;
        }).join('');
    }

    function seuNavegarTeclado_local(e) {
        if (e.key === 'ArrowDown') { e.preventDefault(); seuMoverIndice_local(1); }
        else if (e.key === 'ArrowUp')  { e.preventDefault(); seuMoverIndice_local(-1); }
        else if (e.key === 'Enter')    { e.preventDefault(); if (indiceAtivo >= 0) seuSelecionarIndice_local(indiceAtivo); }
    }

    function seuDefinirIndice_local(i) { indiceAtivo = i; seuDestacar_local(); };

    function seuMoverIndice_local(dir) {
        if (dados.length === 0) return;
        indiceAtivo = Math.max(0, Math.min(dados.length - 1, indiceAtivo + dir));
        seuDestacar_local();
        const list = el('seu_list_' + CID);
        if (list) {
            const row = list.querySelector(`[data-idx="${indiceAtivo}"]`);
            row?.scrollIntoView({ block: 'nearest' });
        }
    }

    function seuDestacar_local() {
        const list = el('seu_list_' + CID);
        if (!list) return;
        list.querySelectorAll('.seu-item').forEach((item, i) => {
            item.classList.toggle('ativo', i === indiceAtivo);
        });
    }

    function seuSelecionarIndice_local(i) {
        if (i < 0 || i >= dados.length) return;
        const item = dados[i];
        seuAplicar_local(item);
    }

    function seuAplicar_local(item) {
        el('val_' + CID).value = item ? (item.id || '') : '';

        const nomeEl   = el('nome_' + CID);
        const avatarEl = el('seu_avatar_' + CID);
        const iconFieldEl = el('seu_icon_field_' + CID);

        if (nomeEl) {
            nomeEl.textContent = item ? item.nome : `Selecionar ${LABEL.toLowerCase()}...`;
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
            const field = el('wrap_' + CID)?.querySelector('.seu-field');
            const btnBus = field?.querySelector('.seu-btn');
            if (field && btnBus) {
                const cl = document.createElement('button');
                cl.type      = 'button';
                cl.id        = 'clear_' + CID;
                cl.className = 'seu-clear';
                cl.title     = 'Remover seleção';
                cl.dataset.action = 'limpar';
                cl.innerHTML = `<svg width="13" height="13" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5"><line x1="18" y1="6" x2="6" y2="18"/><line x1="6" y1="6" x2="18" y2="18"/></svg>`;
                field.insertBefore(cl, btnBus);
            }
        } else if (!item && existingClear) {
            existingClear.remove();
        }

        el('wrap_' + CID)?.dispatchEvent(new CustomEvent('seletor:change', {
            detail: item,
            bubbles: true
        }));

        seuFecharModal_local();
    }

    function seuLimpar_local() {
        seuAplicar_local(null);
    }

    const wrapEl = el('wrap_' + CID);
    if (wrapEl) {
        wrapEl.addEventListener('click', (e) => {
            const botao = e.target.closest('[data-action]');
            if (!botao) return;
            const action = botao.dataset.action;
            if (action === 'abrir') {
                e.stopPropagation();
                seuAbrirModal_local();
            } else if (action === 'limpar') {
                e.stopPropagation();
                seuLimpar_local();
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
                    seuFecharModal_local();
                } else if (action === 'modo') {
                    seuModo_local(botao.dataset.modo);
                } else if (action === 'selecionar') {
                    seuSelecionarIndice_local(parseInt(botao.dataset.idx));
                } else if (action === 'fechar-overlay') {
                    if (e.target === overlayEl) seuFecharModal_local();
                }
            } else if (e.target === overlayEl) {
                seuFecharModal_local();
            }
        });

        overlayEl.addEventListener('mouseover', (e) => {
            const item = e.target.closest('.seu-item');
            if (item && item.dataset.idx) {
                seuDefinirIndice_local(parseInt(item.dataset.idx));
            }
        });

        const inputBusca = el('seu_q_' + CID);
        if (inputBusca) {
            inputBusca.addEventListener('input', (e) => seuAtrasoBusca_local(e.target.value));
            inputBusca.addEventListener('keydown', (e) => seuNavegarTeclado_local(e));
        }
    }
};

document.addEventListener('DOMContentLoaded', () => {
    document.querySelectorAll('.seu-wrap-component').forEach(el => {
        if (!el.dataset.initSeu) {
            el.dataset.initSeu = 'true';
            window.inicializarSeletorUsuario(el.dataset.cid, el.dataset.papel, el.dataset.label);
        }
    });
});
