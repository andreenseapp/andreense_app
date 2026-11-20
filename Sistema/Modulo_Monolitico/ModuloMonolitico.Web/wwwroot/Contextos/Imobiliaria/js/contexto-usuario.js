(function () {
  'use strict';

  var raiz = document.getElementById('form-filtros');
  if (!raiz || raiz.getAttribute('data-usuario-logado') !== 'true') return;

  var urlContexto = raiz.getAttribute('data-url-contexto-usuario');
  if (!urlContexto) return;

  function aplicarFavoritos(ids) {
    if (!Array.isArray(ids)) return;
    var conjunto = new Set(ids.map(function (id) { return String(id).toLowerCase(); }));
    document.querySelectorAll('[data-favoritar-btn]').forEach(function (btn) {
      var imovelId = String(btn.getAttribute('data-imovel-id') || '').toLowerCase();
      if (conjunto.has(imovelId)) btn.classList.add('ativo');
    });
  }

  function aplicarVisualizados(ids) {
    if (!Array.isArray(ids)) return;

    ids.forEach(function (id) {
      var card = document.querySelector('.anuncio-card[data-anuncio-id="' + id + '"]');
      if (!card || card.classList.contains('anuncio-card--visualizado')) return;

      card.classList.add('anuncio-card--visualizado');
      var foto = card.querySelector('.anuncio-card-foto');
      if (!foto || foto.querySelector('.badge--visualizado')) return;

      var selo = document.createElement('span');
      selo.className = 'anuncio-badge badge--visualizado';
      selo.textContent = 'Visto';
      foto.appendChild(selo);
    });
  }

  function renderizarBuscas(buscas) {
    var painel = document.getElementById('buscas-recentes');
    var listaEl = document.getElementById('buscas-recentes-lista');
    var api = window.ImobiliariaBuscasRecentes;
    if (!painel || !listaEl || !api || !api.montarListaBuscas) return;

    var urlImoveis = painel.getAttribute('data-url-imoveis') || '/Imoveis';
    api.montarListaBuscas(painel, listaEl, buscas, urlImoveis);
  }

  window.requisitarApp(urlContexto, {
    method: 'GET',
    headers: { 'Accept': 'application/json' },
    credentials: 'same-origin'
  })
    .then(function (response) {
      if (!response.ok) throw new Error('contexto_usuario');
      return response.json();
    })
    .then(function (payload) {
      if (!payload || payload.sucesso !== true || !payload.dados) return;
      aplicarFavoritos(payload.dados.favoritosIds);
      aplicarVisualizados(payload.dados.visualizadosIds);
      renderizarBuscas(payload.dados.buscasRecentes);
    })
    .catch(function () { /* silencioso */ });
})();
