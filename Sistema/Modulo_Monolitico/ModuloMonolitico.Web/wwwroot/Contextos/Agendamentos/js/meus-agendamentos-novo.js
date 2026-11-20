(function () {
let idServicoSelecionado = null;
let duracaoSelecionada = 60;
let horarioSelecionado = null;

function obterBtn(id) {
    return document.getElementById(id);
}

function selecionarEmpresa(id, nome, el) {
    document.querySelectorAll('#gridEmpresas .cartao-selecao').forEach(c => c.classList.remove('selecionado'));
    el.classList.add('selecionado');
    document.getElementById('inputEmpresaId').value = id;
    document.getElementById('inputEmpresaNome').value = nome;
    const botao = obterBtn('btnP1');
    if (botao) botao.disabled = false;
}

function selecionarServico(id, nome, dur, el) {
    document.querySelectorAll('.cartao-selecao-servico').forEach(c => c.classList.remove('selecionado'));
    el.classList.add('selecionado');
    idServicoSelecionado = id;
    duracaoSelecionada = dur;
    document.getElementById('inputTsId').value = id;
    document.getElementById('inputTsNome').value = nome;
    carregarHorarios();
}

async function carregarHorarios() {
    if (!idServicoSelecionado) return;

    const dataInput = document.getElementById('inputData');
    const data = dataInput ? dataInput.value : '';
    const selProf = document.getElementById('selProf');
    const profId = selProf ? selProf.value : '';
    const ctr = document.getElementById('containerHorarios');
    const formEl = document.getElementById('formPasso2');
    const empresaId = formEl ? formEl.dataset.empresaId : '';

    if (!data || !ctr) return;

    if (selProf) {
        document.getElementById('inputProfRaw').value = profId;
        if (profId) {
            const opt = selProf.options[selProf.selectedIndex];
            document.getElementById('inputProfNome').value = opt ? opt.text : '';
        }
    }

    window.exibirCarregamentoHorarios(ctr);

    const params = new URLSearchParams({ produtoVariacaoId: idServicoSelecionado, data });
    if (profId) params.set('prestadorIdComposto', profId);

    try {
        const r = await window.requisitarApp(`/api/agendamentos/horarios/publico/${empresaId}?${params}`);
        const horarios = await r.json();

        if (!horarios.length) {
            window.definirMensagemContainer(ctr, 'horarios-carregando', 'Nenhum horário disponível.', 'var(--clr-text-muted)');
            const botao = obterBtn('btnP2');
            if (botao) botao.disabled = true;
            return;
        }

        window.renderizarBotoesHorarios(ctr, horarios, (botao, s) => {
            botao.dataset.ini = String(s.dataHoraInicio ?? '');
            botao.dataset.fim = String(s.dataHoraFim ?? '');
            botao.dataset.pid = String(s.profissionalId ?? '');
            botao.dataset.pnome = String(s.nomeProfissional ?? '');
            selecionarHorario(botao);
        });
    } catch {
        window.definirMensagemContainer(ctr, 'horarios-carregando', 'Erro ao carregar.', '#ef4444');
        const botao = obterBtn('btnP2');
        if (botao) botao.disabled = true;
    }
}

function selecionarHorario(botao) {
    document.querySelectorAll('.botao-horario').forEach(a => a.classList.remove('sel'));
    botao.classList.add('sel');
    horarioSelecionado = botao.dataset.ini;

    document.getElementById('inputDHI').value = botao.dataset.ini;
    document.getElementById('inputDHF').value = botao.dataset.fim;
    document.getElementById('inputProfRaw').value = botao.dataset.pid;
    document.getElementById('inputProfNome').value = botao.dataset.pnome;

    const sel = document.getElementById('selProf');
    if (sel && !sel.value) {
        for (const o of sel.options) {
            if (o.value === botao.dataset.pid) {
                o.selected = true;
                break;
            }
        }
    }

    const btnP2 = obterBtn('btnP2');
    if (btnP2) btnP2.disabled = false;
}

window.addEventListener('DOMContentLoaded', () => {
    const btnP1 = obterBtn('btnP1');
    const btnP2 = obterBtn('btnP2');
    if (btnP1) btnP1.disabled = true;
    if (btnP2) btnP2.disabled = true;

    document.querySelectorAll('.cartao-selecao-empresa').forEach(card => {
        card.addEventListener('click', () => {
            selecionarEmpresa(card.dataset.empresaId, card.dataset.empresaNome, card);
        });
    });

    document.querySelectorAll('.cartao-selecao-servico').forEach(card => {
        card.addEventListener('click', () => {
            selecionarServico(
                card.dataset.servicoId,
                card.dataset.servicoNome,
                parseInt(card.dataset.duracao, 10),
                card
            );
        });
    });

    const selProf = document.getElementById('selProf');
    if (selProf) {
        selProf.addEventListener('change', () => {
            document.getElementById('inputProfRaw').value = selProf.value;
            const opt = selProf.options[selProf.selectedIndex];
            document.getElementById('inputProfNome').value = opt && selProf.value ? opt.text : '';
            carregarHorarios();
        });
    }

    const inputData = document.getElementById('inputData');
    if (inputData) {
        inputData.addEventListener('change', carregarHorarios);
    }

    const btnHoje = document.getElementById('btnHoje');
    if (btnHoje && inputData) {
        btnHoje.addEventListener('click', () => {
            const d = new Date();
            inputData.value = `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, '0')}-${String(d.getDate()).padStart(2, '0')}`;
            carregarHorarios();
        });
    }

    const formPasso2 = document.getElementById('formPasso2');
    if (formPasso2) {
        formPasso2.addEventListener('submit', () => {
            const sel = document.getElementById('selProf');
            if (sel && sel.value && !document.getElementById('inputProfRaw').value) {
                document.getElementById('inputProfRaw').value = sel.value;
                const opt = sel.options[sel.selectedIndex];
                document.getElementById('inputProfNome').value = opt ? opt.text : '';
            }
        });
    }
});

})();
