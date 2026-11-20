    async function abrirHistoricoCadastral(tabela, registroId) {
        if (!tabela || !registroId) return;

        if (typeof window.exibirCarregamento === 'function') {
            window.exibirCarregamento("Carregando histórico de auditoria...");
        }

        try {
            const response = await window.requisitarApp(`/api/auditoria?tabela=${tabela}&registroId=${registroId}`);
            
            if (response.status === 403) {
                if (typeof window.ocultarCarregamento === 'function') window.ocultarCarregamento();
                if (typeof window.exibirModal === 'function') {
                    window.exibirModal("Acesso Restrito", "Apenas o usuário master (andreense.app@gmail.com) tem permissão de auditoria.", "error");
                } else {
                    alerta("Apenas o usuário master tem permissão de auditoria.");
                }
                return;
            }

            if (!response.ok) {
                throw new Error("Erro de resposta.");
            }

            const logs = await response.json();
            if (typeof window.ocultarCarregamento === 'function') window.ocultarCarregamento();

            exibirModalAuditoria(tabela, registroId, logs);
        } catch (error) {
            if (typeof window.ocultarCarregamento === 'function') window.ocultarCarregamento();
            if (typeof window.exibirModal === 'function') {
                window.exibirModal("Erro de Auditoria", "Não foi possível recuperar o histórico de auditoria deste registro.", "error");
            } else {
                alerta("Erro ao buscar histórico.");
            }
        }
    }

    function exibirModalAuditoria(tabela, registroId, logs) {
        const modalAntigo = document.getElementById('auditoria-modal-container');
        if (modalAntigo) modalAntigo.remove();

        const backdrop = document.createElement('div');
        backdrop.id = 'auditoria-modal-container';
        backdrop.className = 'auditoria-backdrop';

        let htmlTimeline = '';
        if (!logs || logs.length === 0) {
            htmlTimeline = '<div style="text-align: center; color: var(--clr-text-muted); padding: 3rem 0; font-size: 0.9rem;">Nenhuma alteração registrada para este cadastro no novo sistema de auditoria.</div>';
        } else {
            htmlTimeline = '<div class="auditoria-timeline">';
            logs.forEach((log, index) => {
                const acaoClass = log.acao.toLowerCase();
                const data = new Date(log.dataHora);
                const dataStr = data.toLocaleDateString('pt-BR') + ' às ' + data.toLocaleTimeString('pt-BR', { hour: '2-digit', minute: '2-digit', second: '2-digit' });
                
                let detalhesFormatados = '';
                if (log.detalhes) {
                    try {
                        const analisado = JSON.parse(log.detalhes);
                        detalhesFormatados = JSON.stringify(analisado, null, 2);
                    } catch {
                        detalhesFormatados = log.detalhes;
                    }
                }

                htmlTimeline += `
                    <div class="auditoria-item">
                        <div class="auditoria-badge ${acaoClass}"></div>
                        <div class="auditoria-content">
                            <div class="auditoria-meta">
                                <span class="auditoria-user">${log.usuarioEmail}</span>
                                <span class="auditoria-time">${dataStr}</span>
                            </div>
                            <div class="auditoria-desc">
                                Efetuou a operação de <strong>${acaoClass.toUpperCase()}</strong>.
                            </div>
                            ${log.detalhes ? `
                                <button type="button" class="auditoria-action-btn" onclick="alternarAuditoriaDetalhe('${index}')">
                                    <svg width="12" height="12" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" style="display:inline; vertical-align:-1px;">
                                        <polyline points="6 9 12 15 18 9"></polyline>
                                    </svg>
                                    Visualizar Dados de Entrada
                                </button>
                                <pre id="auditoria-detalhe-${index}" class="auditoria-details">${detalhesFormatados}</pre>
                            ` : ''}
                        </div>
                    </div>
                `;
            });
            htmlTimeline += '</div>';
        }

        backdrop.innerHTML = `
            <div class="auditoria-modal">
                <div class="auditoria-header">
                    <div class="auditoria-title">
                        <h3>Histórico de Auditoria</h3>
                        <span>Tabela: ${tabela} | ID: ${registroId}</span>
                    </div>
                    <button type="button" class="auditoria-close" onclick="fecharModalAuditoria()">&times;</button>
                </div>
                <div class="auditoria-body">
                    ${htmlTimeline}
                </div>
            </div>
        `;

        document.body.appendChild(backdrop);

        setTimeout(() => {
            backdrop.classList.add('active');
        }, 10);

        backdrop.addEventListener('click', (e) => {
            if (e.target === backdrop) fecharModalAuditoria();
        });
    }

    function fecharModalAuditoria() {
        const backdrop = document.getElementById('auditoria-modal-container');
        if (backdrop) {
            backdrop.classList.remove('active');
            setTimeout(() => {
                backdrop.remove();
            }, 300);
        }
    }

    window.alternarAuditoriaDetalhe = function(index) {
        const el = document.getElementById(`auditoria-detalhe-${index}`);
        if (el) {
            const visivel = el.style.display === 'block';
            el.style.display = visivel ? 'none' : 'block';
        }
    }

    document.addEventListener('DOMContentLoaded', function () {
        document.querySelectorAll('[data-audit-log]').forEach(function (botao) {
            botao.addEventListener('click', function () {
                abrirHistoricoCadastral(
                    botao.getAttribute('data-audit-tabela'),
                    botao.getAttribute('data-audit-registro-id'));
            });
        });
    });