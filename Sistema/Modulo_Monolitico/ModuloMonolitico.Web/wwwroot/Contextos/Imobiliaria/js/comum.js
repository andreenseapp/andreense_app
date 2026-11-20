(function (global) {
  'use strict';

  var LIMITE_PADRAO = 8;

  function lerLista(chave) {
    try {
      var bruto = localStorage.getItem(chave);
      if (!bruto) return [];
      var lista = JSON.parse(bruto);
      return Array.isArray(lista) ? lista : [];
    } catch (e) {
      return [];
    }
  }

  function gravarLista(chave, lista) {
    localStorage.setItem(chave, JSON.stringify(lista));
  }

  function normalizarTexto(valor) {
    if (valor == null) return null;
    var t = String(valor).trim();
    return t ? t : null;
  }

  function normalizarFiltro(filtro) {
    var amenidades = Array.isArray(filtro.codigosAmenidade)
      ? filtro.codigosAmenidade
      : (Array.isArray(filtro.CodigosAmenidade) ? filtro.CodigosAmenidade : []);

    amenidades = amenidades
      .filter(function (c) { return !!normalizarTexto(c); })
      .map(function (c) { return String(c).trim(); })
      .filter(function (c, i, arr) {
        return arr.findIndex(function (x) { return x.toLowerCase() === c.toLowerCase(); }) === i;
      })
      .sort(function (a, b) { return a.toLowerCase().localeCompare(b.toLowerCase()); });

    return {
      tipoNegocio: normalizarTexto(filtro.tipoNegocio || filtro.TipoNegocio),
      tipoImovel: normalizarTexto(filtro.tipoImovel || filtro.TipoImovel),
      bairro: normalizarTexto(filtro.bairro || filtro.Bairro),
      quartosMinimo: filtro.quartosMinimo != null ? filtro.quartosMinimo : filtro.QuartosMinimo,
      banheirosMinimo: filtro.banheirosMinimo != null ? filtro.banheirosMinimo : filtro.BanheirosMinimo,
      vagasMinimo: filtro.vagasMinimo != null ? filtro.vagasMinimo : filtro.VagasMinimo,
      valorMinimo: filtro.valorMinimo != null ? filtro.valorMinimo : filtro.ValorMinimo,
      valorMaximo: filtro.valorMaximo != null ? filtro.valorMaximo : filtro.ValorMaximo,
      valorCondominioMinimo: filtro.valorCondominioMinimo != null ? filtro.valorCondominioMinimo : filtro.ValorCondominioMinimo,
      valorCondominioMaximo: filtro.valorCondominioMaximo != null ? filtro.valorCondominioMaximo : filtro.ValorCondominioMaximo,
      areaMinima: filtro.areaMinima != null ? filtro.areaMinima : filtro.AreaMinima,
      areaMaxima: filtro.areaMaxima != null ? filtro.areaMaxima : filtro.AreaMaxima,
      situacaoObra: normalizarTexto(filtro.situacaoObra || filtro.SituacaoObra),
      codigoPublico: normalizarTexto(filtro.codigoPublico || filtro.CodigoPublico),
      codigosAmenidade: amenidades,
      buscarPorPacote: !!(filtro.buscarPorPacote || filtro.BuscarPorPacote)
    };
  }

  function limparNulos(obj) {
    var out = {};
    Object.keys(obj).forEach(function (k) {
      var v = obj[k];
      if (v == null) return;
      if (k === 'buscarPorPacote' && v === false) return;
      if (k === 'codigosAmenidade' && (!Array.isArray(v) || v.length === 0)) return;
      out[k] = v;
    });
    return out;
  }

  function serializarJson(filtro) {
    return JSON.stringify(limparNulos(normalizarFiltro(filtro)));
  }

  function gerarChave(filtro) {
    var json = serializarJson(filtro);
    var hash = 2166136261;
    for (var i = 0; i < json.length; i++) {
      hash ^= json.charCodeAt(i);
      hash = Math.imul(hash, 16777619);
    }
    return (hash >>> 0).toString(16);
  }

  function rotuloTipoImovel(tipo) {
    var map = {
      apartamento: 'Apartamento',
      casa: 'Casa',
      kitnet: 'Kitnet/Studio',
      comercial: 'Comercial'
    };
    return map[String(tipo).toLowerCase()] || tipo;
  }

  function rotuloSituacao(situacao) {
    var map = {
      pronto: 'Pronto',
      em_construcao: 'Em construção',
      na_planta: 'Na planta'
    };
    return map[String(situacao).toLowerCase()] || situacao;
  }

  function formatarNumero(valor) {
    return Number(valor).toLocaleString('pt-BR');
  }

  function rotuloFaixa(unidade, minimo, maximo) {
    if (minimo != null && maximo != null) return formatarNumero(minimo) + '–' + formatarNumero(maximo) + ' ' + unidade;
    if (minimo != null) return 'de ' + formatarNumero(minimo) + ' ' + unidade;
    return 'até ' + formatarNumero(maximo) + ' ' + unidade;
  }

  function gerarRotulo(filtro) {
    var n = normalizarFiltro(filtro);
    var partes = [];
    if (n.tipoNegocio === 'aluguel') partes.push('Alugar');
    else if (n.tipoNegocio === 'venda') partes.push('Comprar');
    if (n.tipoImovel) partes.push(rotuloTipoImovel(n.tipoImovel));
    if (n.bairro) partes.push(n.bairro);
    if (n.quartosMinimo != null) partes.push(n.quartosMinimo + '+ quartos');
    if (n.banheirosMinimo != null) partes.push(n.banheirosMinimo + '+ banheiros');
    if (n.vagasMinimo != null) partes.push(n.vagasMinimo + '+ vagas');
    if (n.valorMinimo != null || n.valorMaximo != null) partes.push(rotuloFaixa('R$', n.valorMinimo, n.valorMaximo));
    if (n.areaMinima != null || n.areaMaxima != null) partes.push(rotuloFaixa('m²', n.areaMinima, n.areaMaxima));
    if (n.situacaoObra) partes.push(rotuloSituacao(n.situacaoObra));
    if (n.codigoPublico) partes.push(n.codigoPublico);
    if (n.codigosAmenidade.length) partes.push(n.codigosAmenidade.length + ' amenidade(s)');
    if (n.buscarPorPacote) partes.push('Pacote');
    if (!partes.length) return 'Busca recente';
    var rotulo = partes.join(' · ');
    return rotulo.length <= 200 ? rotulo : rotulo.slice(0, 197) + '...';
  }

  function temCriterioSalvavel(filtro) {
    var n = normalizarFiltro(filtro);
    return !!(
      n.tipoImovel ||
      n.bairro ||
      n.quartosMinimo != null ||
      n.banheirosMinimo != null ||
      n.vagasMinimo != null ||
      n.valorMinimo != null ||
      n.valorMaximo != null ||
      n.valorCondominioMinimo != null ||
      n.valorCondominioMaximo != null ||
      n.areaMinima != null ||
      n.areaMaxima != null ||
      n.situacaoObra ||
      n.codigoPublico ||
      n.codigosAmenidade.length ||
      n.buscarPorPacote ||
      n.tipoNegocio === 'aluguel'
    );
  }

  function montarQueryString(filtro) {
    var n = normalizarFiltro(filtro);
    var parts = [];
    function add(nome, valor) {
      if (valor == null || valor === '') return;
      parts.push('Filtro.' + nome + '=' + encodeURIComponent(String(valor)));
    }
    add('TipoNegocio', n.tipoNegocio);
    add('TipoImovel', n.tipoImovel);
    add('Bairro', n.bairro);
    add('QuartosMinimo', n.quartosMinimo);
    add('BanheirosMinimo', n.banheirosMinimo);
    add('VagasMinimo', n.vagasMinimo);
    add('ValorMinimo', n.valorMinimo);
    add('ValorMaximo', n.valorMaximo);
    add('ValorCondominioMinimo', n.valorCondominioMinimo);
    add('ValorCondominioMaximo', n.valorCondominioMaximo);
    add('AreaMinima', n.areaMinima);
    add('AreaMaxima', n.areaMaxima);
    add('SituacaoObra', n.situacaoObra);
    add('CodigoPublico', n.codigoPublico);
    if (n.buscarPorPacote) parts.push('Filtro.BuscarPorPacote=true');
    n.codigosAmenidade.forEach(function (c) {
      parts.push('Filtro.CodigosAmenidade=' + encodeURIComponent(c));
    });
    return parts.join('&');
  }

  function filtroDaQuery(search) {
    var params = new URLSearchParams(search || window.location.search);
    var amenidades = params.getAll('Filtro.CodigosAmenidade');
    function num(nome) {
      var v = params.get('Filtro.' + nome);
      if (v == null || v === '') return null;
      var n = Number(v);
      return isNaN(n) ? null : n;
    }
    return {
      tipoNegocio: params.get('Filtro.TipoNegocio'),
      tipoImovel: params.get('Filtro.TipoImovel'),
      bairro: params.get('Filtro.Bairro'),
      quartosMinimo: num('QuartosMinimo'),
      banheirosMinimo: num('BanheirosMinimo'),
      vagasMinimo: num('VagasMinimo'),
      valorMinimo: num('ValorMinimo'),
      valorMaximo: num('ValorMaximo'),
      valorCondominioMinimo: num('ValorCondominioMinimo'),
      valorCondominioMaximo: num('ValorCondominioMaximo'),
      areaMinima: num('AreaMinima'),
      areaMaxima: num('AreaMaxima'),
      situacaoObra: params.get('Filtro.SituacaoObra'),
      codigoPublico: params.get('Filtro.CodigoPublico'),
      codigosAmenidade: amenidades,
      buscarPorPacote: params.get('Filtro.BuscarPorPacote') === 'true'
    };
  }

  function queryTemFiltro(search) {
    var params = new URLSearchParams(search || window.location.search);
    var encontrado = false;
    params.forEach(function (_v, k) {
      if (k.toLowerCase().indexOf('filtro.') === 0) encontrado = true;
    });
    return encontrado;
  }

  function registrar(chave, filtro, limite) {
    if (!temCriterioSalvavel(filtro)) return lerLista(chave);
    var n = normalizarFiltro(filtro);
    var chaveFiltro = gerarChave(n);
    var agora = new Date().toISOString();
    var lista = lerLista(chave).filter(function (item) {
      return item && item.chaveFiltro !== chaveFiltro;
    });
    lista.unshift({
      id: chaveFiltro,
      chaveFiltro: chaveFiltro,
      filtroJson: serializarJson(n),
      rotulo: gerarRotulo(n),
      dataUltimaBusca: agora
    });
    lista = lista.slice(0, limite || LIMITE_PADRAO);
    gravarLista(chave, lista);
    return lista;
  }

  function remover(chave, chaveFiltro) {
    var lista = lerLista(chave).filter(function (item) {
      return item && item.chaveFiltro !== chaveFiltro;
    });
    gravarLista(chave, lista);
    return lista;
  }

  function limparTodas(chave) {
    gravarLista(chave, []);
    return [];
  }

  function montarHref(urlImoveis, item) {
    try {
      if (item.queryString) {
        return item.queryString ? (urlImoveis + '?' + item.queryString) : urlImoveis;
      }
      var filtro = item.filtroJson ? JSON.parse(item.filtroJson) : {};
      var qs = montarQueryString(filtro);
      return qs ? (urlImoveis + '?' + qs) : urlImoveis;
    } catch (e) {
      return urlImoveis;
    }
  }

  function obterChaveItem(item) {
    if (!item) return '';
    if (item.chaveFiltro) return String(item.chaveFiltro);
    if (item.ChaveFiltro) return String(item.ChaveFiltro);
    if (item.id) return String(item.id);
    return '';
  }

  function listaContemChaveFiltro(lista, chaveFiltro) {
    if (!chaveFiltro || !lista || !lista.length) return false;
    return lista.some(function (item) {
      return obterChaveItem(item) === chaveFiltro;
    });
  }

  function resolverChaveFiltroAtual(painel) {
    var modo = painel.getAttribute('data-modo') || '';
    if (modo === 'anonimo' && queryTemFiltro()) {
      var filtroAnonimo = filtroDaQuery();
      if (temCriterioSalvavel(filtroAnonimo)) return gerarChave(filtroAnonimo);
    }
    return painel.getAttribute('data-chave-filtro-atual') || '';
  }

  function montarListaBuscas(painel, listaEl, lista, urlImoveis) {
    if (!painel || !listaEl) return;

    var chaveAtual = resolverChaveFiltroAtual(painel);
    var rotuloAtual = painel.getAttribute('data-rotulo-atual') || '';
    var tituloAtual = painel.getAttribute('data-titulo-atual') || 'Busca atual';
    var itens = Array.isArray(lista) ? lista : [];
    var jaNaLista = listaContemChaveFiltro(itens, chaveAtual);

    listaEl.innerHTML = '';

    if (chaveAtual && rotuloAtual && !jaNaLista) {
      var span = document.createElement('span');
      span.className = 'busca-recente-card busca-recente-card--atual';
      span.title = tituloAtual;
      span.textContent = rotuloAtual;
      listaEl.appendChild(span);
    }

    itens.forEach(function (item) {
      var ehAtual = chaveAtual && obterChaveItem(item) === chaveAtual;
      var card = document.createElement('a');
      card.className = 'busca-recente-card' + (ehAtual ? ' busca-recente-card--atual' : '');
      if (ehAtual) card.title = tituloAtual;
      card.href = montarHref(urlImoveis, item);
      card.textContent = item.rotulo || item.Rotulo || 'Busca recente';
      listaEl.appendChild(card);
    });

    painel.hidden = !(itens.length || (chaveAtual && rotuloAtual && !jaNaLista));
  }

  global.ImobiliariaBuscasRecentes = {
    lerLista: lerLista,
    registrar: registrar,
    remover: remover,
    limparTodas: limparTodas,
    filtroDaQuery: filtroDaQuery,
    queryTemFiltro: queryTemFiltro,
    montarHref: montarHref,
    montarListaBuscas: montarListaBuscas,
    temCriterioSalvavel: temCriterioSalvavel,
    gerarRotulo: gerarRotulo,
    gerarChave: gerarChave,
    obterChaveItem: obterChaveItem,
    listaContemChaveFiltro: listaContemChaveFiltro
  };

  var LIMITE_VISUALIZADOS = 200;

  function lerVisualizados(chave) {
    try {
      var bruto = localStorage.getItem(chave);
      if (!bruto) return [];
      var lista = JSON.parse(bruto);
      return Array.isArray(lista) ? lista : [];
    } catch (e) {
      return [];
    }
  }

  function gravarVisualizados(chave, lista) {
    localStorage.setItem(chave, JSON.stringify(lista));
  }

  function normalizarId(anuncioId) {
    return String(anuncioId || '').trim().toLowerCase();
  }

  function registrarVisualizado(chave, anuncioId) {
    var id = normalizarId(anuncioId);
    if (!id) return lerVisualizados(chave);

    var agora = new Date().toISOString();
    var lista = lerVisualizados(chave).filter(function (item) {
      return item && normalizarId(item.anuncioId) !== id;
    });
    lista.unshift({ anuncioId: id, dataVisualizacao: agora });
    lista = lista.slice(0, LIMITE_VISUALIZADOS);
    gravarVisualizados(chave, lista);
    return lista;
  }

  function foiVisualizado(chave, anuncioId) {
    var id = normalizarId(anuncioId);
    return lerVisualizados(chave).some(function (item) {
      return item && normalizarId(item.anuncioId) === id;
    });
  }

  function obterIds(chave) {
    return lerVisualizados(chave).map(function (item) {
      return item ? normalizarId(item.anuncioId) : '';
    }).filter(Boolean);
  }

  global.ImobiliariaAnunciosVisualizados = {
    lerLista: lerVisualizados,
    registrar: registrarVisualizado,
    foiVisualizado: foiVisualizado,
    obterIds: obterIds
  };
})(window);
