(function () {
let horarioSelecionado = null;

function obterInputData() {
    return document.getElementById('DataAgendamento');
}

function obterBtnSalvar() {
    return document.getElementById('btnSalvar');
}

document.getElementById('containerServico').addEventListener('seletor:change', function () {
    document.querySelectorAll('[id^="wrap_spr_"]').forEach(function (w) {
        const cid = w.id.replace('wrap_', '');
        if (window['sprLimpar_' + cid]) window['sprLimpar_' + cid]();
    });
    carregarHorarios();
});

document.getElementById('wrapSeletorCliente').addEventListener('seletor:change', function (e) {
    const cli = e.detail;
    document.getElementById('inpCliNome').value = cli ? (cli.nome || cli.nomeExibicao || '') : '';

    const telInput = document.getElementById('ClienteTelefone');
    const emailInput = document.getElementById('ClienteEmail');
    if (telInput) telInput.value = cli ? (cli.telefone || '') : '';
    if (emailInput) emailInput.value = cli ? (cli.email || '') : '';

    const cbEmail = document.getElementById('EnviaEmailConfirmacao');
    if (cbEmail && cli && typeof cli.enviaEmailAgendamento !== 'undefined')
        cbEmail.checked = cli.enviaEmailAgendamento;
});

async function carregarHorarios() {
    const servInput = document.querySelector('#containerServico input[name="ProdutoVariacaoId"]')
        || document.querySelector('input[name="ProdutoVariacaoId"]');
    const profInput = document.querySelector('input[name="PrestadorIdComposto"]');
    const dataInput = obterInputData();

    const servId = servInput ? servInput.value : '';
    const profId = profInput ? profInput.value : '';
    const data = dataInput ? dataInput.value : '';
    const container = document.getElementById('containerHorarios');
    const btnSalvar = obterBtnSalvar();

    if (!servId || !data) {
        window.definirMensagemContainer(container, 'horarios-carregando', 'Selecione o servi�o e a data para ver os hor�rios.');
        if (btnSalvar) btnSalvar.disabled = true;
        return;
    }

    window.exibirCarregamentoHorarios(container);

    try {
        const params = new URLSearchParams({ produtoVariacaoId: servId, data });
        if (profId) params.set('prestadorIdComposto', profId);
        const resp = await window.requisitarApp(`/api/agendamentos/horarios?${params}`);
        const horarios = await resp.json();

        if (!horarios.length) {
            window.definirMensagemContainer(container, 'horarios-carregando', 'Nenhum hor�rio dispon�vel para esta data.', 'var(--clr-text-muted)');
            if (btnSalvar) btnSalvar.disabled = true;
            return;
        }

        window.renderizarBotoesHorarios(container, horarios, (botao, s) => {
            botao.dataset.inicio = String(s.dataHoraInicio ?? '');
            botao.dataset.prof = String(s.profissionalId ?? '');
            selecionarHorario(botao);
        });
    } catch {
        window.definirMensagemContainer(container, 'horarios-carregando', 'Erro ao carregar hor�rios. Tente novamente.', '#ef4444');
        if (btnSalvar) btnSalvar.disabled = true;
    }
}

function selecionarHorario(botao) {
    document.querySelectorAll('.botao-horario').forEach(a => a.classList.remove('selecionado'));
    botao.classList.add('selecionado');
    horarioSelecionado = botao.dataset.inicio;
    document.getElementById('inputDataHora').value = horarioSelecionado;

    const profId = botao.dataset.prof;
    const profInput = document.querySelector('input[name="PrestadorIdComposto"]');
    if (profInput && profInput.value === '')
        profInput.value = profId;

    const btnSalvar = obterBtnSalvar();
    if (btnSalvar) btnSalvar.disabled = false;
}

window.addEventListener('DOMContentLoaded', () => {
    const btnSalvar = obterBtnSalvar();
    if (btnSalvar) btnSalvar.disabled = true;

    const dataInput = obterInputData();
    if (dataInput && dataInput.value) setTimeout(carregarHorarios, 100);
});

document.addEventListener('seletor:change', function (e) {
    const targetWrap = e.target.closest('[class*="-wrap-"]');
    if (targetWrap && (targetWrap.className.includes('spv-wrap-') || targetWrap.className.includes('spr-wrap-')))
        carregarHorarios();
});

})();
