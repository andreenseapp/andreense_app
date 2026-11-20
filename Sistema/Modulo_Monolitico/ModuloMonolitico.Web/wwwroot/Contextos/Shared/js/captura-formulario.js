document.addEventListener('DOMContentLoaded', () => {
    const formularios = document.querySelectorAll('form[method="post"]:not([data-sem-instantaneo])');

    formularios.forEach(formulario => {
        const idInstantaneo = formulario.getAttribute('data-instantaneo-id') || `instantaneo_${window.location.pathname.replace(/\//g, '_')}`;
        const chaveArmazenamento = `andreense_form_${idInstantaneo}`;

        const limparInstantaneo = formulario.getAttribute('data-limpar-instantaneo')
            || document.body.getAttribute('data-limpar-instantaneo');
        if (limparInstantaneo && limparInstantaneo === idInstantaneo) {
            localStorage.removeItem(chaveArmazenamento);
            return;
        }

        const instantaneoSalvo = localStorage.getItem(chaveArmazenamento);
        if (instantaneoSalvo) {
            try {
                const dados = JSON.parse(instantaneoSalvo);
                if (Object.keys(dados).length > 0) {
                    mostrarAlertaRecuperacao(formulario, dados, chaveArmazenamento);
                }
            } catch (erro) {
                console.error("Erro ao ler instantaneo do formulario", erro);
            }
        }

        let atraso = null;
        formulario.addEventListener('input', () => {
            clearTimeout(atraso);
            atraso = setTimeout(() => {
                salvarInstantaneoFormulario(formulario, chaveArmazenamento);
            }, 2000);
        });
        formulario.addEventListener('change', () => {
            salvarInstantaneoFormulario(formulario, chaveArmazenamento);
        });

        formulario.addEventListener('submit', () => {
        });
    });

    function salvarInstantaneoFormulario(formulario, chave) {
        const dadosFormulario = new FormData(formulario);
        const dadosObj = {};

        dadosFormulario.forEach((valor, nome) => {
            if (nome !== '__RequestVerificationToken' && !nome.toLowerCase().includes('senha') && !nome.toLowerCase().includes('password')) {
                if (dadosObj[nome] !== undefined) {
                    if (!Array.isArray(dadosObj[nome])) {
                        dadosObj[nome] = [dadosObj[nome]];
                    }
                    dadosObj[nome].push(valor);
                } else {
                    dadosObj[nome] = valor;
                }
            }
        });

        localStorage.setItem(chave, JSON.stringify(dadosObj));
    }

    function mostrarAlertaRecuperacao(formulario, dados, chave) {
        const alertaDiv = document.createElement('div');
        alertaDiv.className = 'alerta alerta-aviso d-flex flex-column flex-md-row align-items-start align-items-md-center justify-content-between gap-3 mb-4 shadow-sm p-3';
        alertaDiv.style.borderRadius = 'var(--radius-lg)';
        alertaDiv.innerHTML = `
            <div class="d-flex align-items-start gap-3">
                <div class="mt-1 flex-shrink-0 text-warning d-flex align-items-center justify-content-center" style="background: rgba(245, 158, 11, 0.15); padding: 8px; border-radius: 8px;">
                    <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round"><path d="M3 12a9 9 0 1 0 9-9 9.75 9.75 0 0 0-6.74 2.74L3 8"/><path d="M3 3v5h5"/></svg>
                </div>
                <div>
                    <strong style="font-weight: 600; color: var(--text-primary, #1e293b); font-size: 0.95rem;">Rascunho Encontrado!</strong><br/>
                    <span class="small" style="color: var(--text-secondary, #64748b);">Parece que você preencheu dados não salvos anteriormente. Deseja recuperá-los?</span>
                </div>
            </div>
            <div class="d-flex gap-2 w-100 w-md-auto justify-content-end align-self-stretch align-self-md-center">
                <button type="button" class="botao btn-sm btn-outline-secondary flex-fill flex-md-initial px-3 py-1.5" id="botao-descartar-instantaneo" style="font-size: 0.85rem; font-weight: 500; border-radius: 6px;">Descartar</button>
                <button type="button" class="botao btn-sm btn-warning flex-fill flex-md-initial px-3 py-1.5" id="botao-restaurar-instantaneo" style="font-size: 0.85rem; font-weight: 500; border-radius: 6px; background-color: var(--bs-warning, #f59e0b); border-color: var(--bs-warning, #f59e0b); color: #fff;">Recuperar Dados</button>
            </div>
        `;

        formulario.insertBefore(alertaDiv, formulario.firstChild);

        document.getElementById('botao-descartar-instantaneo').addEventListener('click', () => {
            localStorage.removeItem(chave);
            alertaDiv.remove();
        });

        document.getElementById('botao-restaurar-instantaneo').addEventListener('click', () => {
            restaurarDadosFormulario(formulario, dados);
            alertaDiv.remove();
        });
    }

    function restaurarDadosFormulario(formulario, dados) {
        Object.keys(dados).forEach(nome => {
            const campos = formulario.querySelectorAll(`[name="${nome}"]`);
            if (campos.length === 0) return;

            const valor = dados[nome];

            campos.forEach(campo => {
                if (campo.type === 'radio') {
                    campo.checked = (campo.value === valor);
                } else if (campo.type === 'checkbox') {
                    if (Array.isArray(valor)) {
                        campo.checked = valor.includes(campo.value);
                    } else {
                        campo.checked = (campo.value === valor || valor === 'true' || valor === 'on');
                    }
                } else if (campo.tagName === 'SELECT') {
                    campo.value = valor;
                } else {
                    campo.value = valor;
                }

                campo.dispatchEvent(new Event('change', { bubbles: true }));
            });
        });
    }
});
