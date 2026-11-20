(function () {
  'use strict';

  var api = window.ImobiliariaAnunciosVisualizados;
  if (!api) return;

  var container = document.querySelector('.imob-detalhe-container') ||
    document.getElementById('form-filtros');
  if (!container) return;

  var chave = container.getAttribute('data-storage-visualizados');
  var usuarioLogado = container.getAttribute('data-usuario-logado') === 'true';
  var anuncioId = container.getAttribute('data-anuncio-id');

  function aplicarSeloNoCard(card) {
    if (!card || card.classList.contains('anuncio-card--visualizado')) return;

    card.classList.add('anuncio-card--visualizado');
    var foto = card.querySelector('.anuncio-card-foto');
    if (!foto || foto.querySelector('.badge--visualizado')) return;

    var selo = document.createElement('span');
    selo.className = 'anuncio-badge badge--visualizado';
    selo.textContent = 'Visto';
    foto.appendChild(selo);
  }

  function marcarCardsNaListagem() {
    if (!chave) return;
    document.querySelectorAll('.anuncio-card[data-anuncio-id]').forEach(function (card) {
      var id = card.getAttribute('data-anuncio-id');
      if (api.foiVisualizado(chave, id)) aplicarSeloNoCard(card);
    });
  }

  function registrarNaPaginaDetalhe() {
    if (!anuncioId || !chave) return;

    var jaVisto = api.foiVisualizado(chave, anuncioId);
    api.registrar(chave, anuncioId);

    if (!usuarioLogado && jaVisto) {
      var badges = document.querySelector('.detalhe-badges');
      if (badges && !badges.querySelector('.badge--visualizado')) {
        var selo = document.createElement('span');
        selo.className = 'anuncio-badge badge--visualizado detalhe-selo-visualizado';
        selo.textContent = 'Visto';
        badges.appendChild(selo);
      }

      if (!document.querySelector('.detalhe-aviso-visualizado')) {
        var titulo = document.querySelector('.detalhe-titulo');
        if (titulo) {
          var p = document.createElement('p');
          p.className = 'detalhe-aviso-visualizado';
          p.textContent = 'Você já visualizou este imóvel.';
          titulo.insertAdjacentElement('afterend', p);
        }
      }
    }
  }

  registrarNaPaginaDetalhe();

  if (!usuarioLogado) {
    marcarCardsNaListagem();
  }
})();
