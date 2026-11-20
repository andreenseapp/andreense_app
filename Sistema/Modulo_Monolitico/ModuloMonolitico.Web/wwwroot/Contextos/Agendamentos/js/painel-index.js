(function () {
const rotas = JSON.parse(document.getElementById('dadosRotas').textContent);
const agendamentos = JSON.parse(document.getElementById('dadosAgendamentos').textContent);
const configs = JSON.parse(document.getElementById('dadosConfiguracoes').textContent);

const diasAtivos = {};
for (let i = 0; i < 7; i++) diasAtivos[i] = false;
for (const c of configs) diasAtivos[c.diaSemana] = c.ativo;

let diaSelecionado = null;

function montarUrlNovo(dataStr) {
    const params = new URLSearchParams({ data: dataStr });
    if (rotas.prestadorIdComposto) params.set('prestadorIdComposto', rotas.prestadorIdComposto);
    return `${rotas.novo}?${params}`;
}

function criarCartaoAgendamento(a) {
    const link = document.createElement('a');
    link.href = `${rotas.detalhes}?id=${encodeURIComponent(a.id)}`;
    link.className = 'agend-cartao';
    link.style.setProperty('--cartao-cor', window.corCssSegura(a.cor));

    const header = document.createElement('div');
    header.className = 'agend-cartao-cabecalho';

    const timeDiv = document.createElement('div');
    timeDiv.className = 'agend-cartao-hora';
    timeDiv.textContent = `${a.hora} – ${a.horaFim}`;

    const badge = document.createElement('span');
    badge.className = `etiqueta ${window.classeCssSegura(a.classeCssSituacao)}`;
    badge.textContent = a.situacao;

    header.appendChild(timeDiv);
    header.appendChild(badge);

    const clientDiv = document.createElement('div');
    clientDiv.className = 'agend-cartao-cliente';
    clientDiv.textContent = a.cliente;

    const serviceDiv = document.createElement('div');
    serviceDiv.className = 'agend-cartao-servico';
    serviceDiv.textContent = `${a.servico} · ${a.profissional}`;

    link.appendChild(header);
    link.appendChild(clientDiv);
    link.appendChild(serviceDiv);
    return link;
}

function anexarCards(parent, agendsNoDia) {
    const frag = document.createDocumentFragment();
    agendsNoDia.forEach(a => frag.appendChild(criarCartaoAgendamento(a)));
    parent.appendChild(frag);
}

function renderizarEstadoVazio(conteudo) {
    conteudo.replaceChildren();
    const wrap = document.createElement('div');
    wrap.className = 'dia-vazio-wrap';
    const p1 = document.createElement('p');
    p1.className = 'dia-vazio-titulo';
    p1.textContent = 'Agenda Livre';
    const p2 = document.createElement('p');
    p2.className = 'dia-vazio-texto';
    p2.textContent = 'Nenhum agendamento para esta data.';
    wrap.appendChild(p1);
    wrap.appendChild(p2);
    conteudo.appendChild(wrap);
}

function renderizarDiaFechado(conteudo, agendsNoDia) {
    conteudo.replaceChildren();
    const alerta = document.createElement('div');
    alerta.className = 'dia-fechado-alerta';
    alerta.textContent = 'Empresa fechada neste dia.';
    conteudo.appendChild(alerta);
    if (agendsNoDia.length > 0) {
        const titulo = document.createElement('div');
        titulo.className = 'agends-previos-title';
        titulo.textContent = 'Agendamentos Prévios:';
        conteudo.appendChild(titulo);
        anexarCards(conteudo, agendsNoDia);
    }
}

function selecionarDia(dataStr) {
    if (!dataStr) return;
    diaSelecionado = dataStr;

    document.querySelectorAll('.cal-celula').forEach(c => c.classList.remove('dia-selecionado'));
    const cellClicada = document.querySelector(`.cal-celula[data-dia="${dataStr}"]`);
    if (cellClicada) cellClicada.classList.add('dia-selecionado');

    const titulo = document.getElementById('tituloPainelDia');
    const subtitulo = document.getElementById('subtituloPainelDia');
    const conteudo = document.getElementById('conteudoPainelDia');
    const actions = document.getElementById('acoesPainelDia');
    const btnNovo = document.getElementById('btnNovoAgendamento');

    const [y, m, d] = dataStr.split('-');
    const dataObj = new Date(y, m - 1, d);
    titulo.textContent = dataObj.toLocaleDateString('pt-BR', { weekday: 'long', day: 'numeric', month: 'long' });

    const diaSemana = dataObj.getDay();
    const ativo = diasAtivos[diaSemana];
    const agendsNoDia = agendamentos.filter(a => a.dia === dataStr).sort((a, a) => a.hora.localeCompare(a.hora));

    subtitulo.textContent = agendsNoDia.length === 1 ? '1 Agendamento' : `${agendsNoDia.length} Agendamentos`;

    if (ativo) {
        btnNovo.href = montarUrlNovo(dataStr);
        actions.classList.add('visivel');
    } else {
        actions.classList.remove('visivel');
    }

    if (!ativo) {
        subtitulo.textContent = 'Dia Fechado';
        renderizarDiaFechado(conteudo, agendsNoDia);
        return;
    }

    if (agendsNoDia.length === 0) {
        renderizarEstadoVazio(conteudo);
        return;
    }

    conteudo.replaceChildren();
    anexarCards(conteudo, agendsNoDia);
}

document.querySelectorAll('.cal-celula[data-selecionavel]').forEach(cell => {
    cell.addEventListener('click', () => selecionarDia(cell.dataset.dia));
});

document.querySelectorAll('.cal-stop-propagation').forEach(el => {
    el.addEventListener('click', e => e.stopPropagation());
});

const filtroPrestador = document.querySelector('#formFiltroPrestador select');
if (filtroPrestador) {
    filtroPrestador.addEventListener('change', () => {
        document.getElementById('formFiltroPrestador').submit();
    });
}

const hojeObj = new Date();
const hojeStr = `${hojeObj.getFullYear()}-${String(hojeObj.getMonth() + 1).padStart(2, '0')}-${String(hojeObj.getDate()).padStart(2, '0')}`;
window.addEventListener('DOMContentLoaded', () => {
    setTimeout(() => selecionarDia(hojeStr), 50);
});

})();
// Modal de Novo Agendamento
document.addEventListener('click', function(e) {
    // Se o usuario clicou no botao de Abrir
    const btnAbrir = e.target.closest('[data-abrir-modal="novo"]');
    if (btnAbrir) {
        e.preventDefault();
        const modal = document.getElementById('modalNovoAgendamento');
        if (modal) modal.classList.add('ativo');
    }

    // Se o usuario clicou em Fechar
    const btnFechar = e.target.closest('[data-fechar-modal]');
    if (btnFechar) {
        e.preventDefault();
        const modal = document.getElementById('modalNovoAgendamento');
        if (modal) modal.classList.remove('ativo');
    }

    // Se clicou no fundo escuro fora
    const modalBackground = document.getElementById('modalNovoAgendamento');
    if (e.target === modalBackground) {
        modalBackground.classList.remove('ativo');
    }
});