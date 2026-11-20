function ordenarTabela(campo, opcoes) {
    const opts = Object.assign({
        paramCampo: 'OrdenarPor',
        paramDirecao: 'OrdenarDirecao',
        paramPagina: 'PaginaAtual',
        direcaoPadrao: 'desc'
    }, opcoes || {});

    const parametrosUrl = new URLSearchParams(window.location.search);
    const campoAtual = parametrosUrl.get(opts.paramCampo) || '';
    const direcaoAtual = parametrosUrl.get(opts.paramDirecao) || opts.direcaoPadrao;

    let proximaDirecao = 'asc';
    if (campoAtual === campo) {
        proximaDirecao = direcaoAtual === 'asc' ? 'desc' : 'asc';
    }

    parametrosUrl.set(opts.paramCampo, campo);
    parametrosUrl.set(opts.paramDirecao, proximaDirecao);
    parametrosUrl.set(opts.paramPagina, '1');

    window.location.search = parametrosUrl.toString();
}
