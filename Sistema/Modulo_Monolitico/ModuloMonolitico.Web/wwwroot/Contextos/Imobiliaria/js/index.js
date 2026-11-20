(function () {
  'use strict';

  var lateral = document.getElementById('filtros-lateral');
  var overlay = document.querySelector('[data-fechar-filtros].imob-filtros-overlay');
  var btnAbrir = document.querySelector('[data-abrir-filtros]');
  var btnsFechar = document.querySelectorAll('[data-fechar-filtros]');

  function abrirFiltros() {
    if (!lateral) return;
    lateral.classList.add('is-open');
    if (overlay) overlay.hidden = false;
    if (btnAbrir) btnAbrir.setAttribute('aria-expanded', 'true');
    document.body.style.overflow = 'hidden';
  }

  function fecharFiltros() {
    if (!lateral) return;
    lateral.classList.remove('is-open');
    if (overlay) overlay.hidden = true;
    if (btnAbrir) btnAbrir.setAttribute('aria-expanded', 'false');
    document.body.style.overflow = '';
  }

  if (btnAbrir) {
    btnAbrir.addEventListener('click', abrirFiltros);
  }

  btnsFechar.forEach(function (btn) {
    btn.addEventListener('click', fecharFiltros);
  });

  var painel = document.getElementById('buscas-recentes');
  if (!painel || !window.ImobiliariaBuscasRecentes) return;

  var api = window.ImobiliariaBuscasRecentes;
  var modo = painel.getAttribute('data-modo');
  var chave = painel.getAttribute('data-storage-key');
  var urlImoveis = painel.getAttribute('data-url-imoveis') || '/Imoveis';
  var urlRegistrar = painel.getAttribute('data-url-registrar-busca');
  var limite = parseInt(painel.getAttribute('data-limite') || '8', 10);
  var listaEl = painel.querySelector('[data-lista-buscas]');

  function renderizar(lista) {
    api.montarListaBuscas(painel, listaEl, lista, urlImoveis);
  }

  if (modo === 'anonimo') {
    if (api.queryTemFiltro()) {
      var filtro = api.filtroDaQuery();
      renderizar(api.registrar(chave, filtro, limite));
    } else {
      renderizar(api.lerLista(chave));
    }
    return;
  }

  if (modo === 'logado' && urlRegistrar && api.queryTemFiltro()) {
    var filtroLogado = api.filtroDaQuery();
    if (api.temCriterioSalvavel(filtroLogado)) {
      window.requisitarApp(urlRegistrar, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          'Accept': 'application/json'
        },
        credentials: 'same-origin',
        body: JSON.stringify({ filtro: filtroLogado })
      }).catch(function () { /* silencioso */ });
    }
  }
})();
