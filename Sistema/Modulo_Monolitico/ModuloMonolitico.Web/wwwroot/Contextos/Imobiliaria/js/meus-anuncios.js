(function () {
  'use strict';

  document.querySelectorAll('[data-alternar-menu]').forEach(function (botao) {
    botao.addEventListener('click', function (e) {
      e.stopPropagation();
      var menu = botao.closest('.situacao-menu').querySelector('.menu-suspenso');
      menu.hidden = !menu.hidden;
    });
  });

  document.addEventListener('click', function () {
    document.querySelectorAll('.menu-suspenso').forEach(function (menu) {
      menu.hidden = true;
    });
  });

  var formStatus = document.getElementById('form-situacao');
  var hiddenAnuncioId = document.getElementById('hidden-anuncio-id');
  var hiddenNovaSituacao = document.getElementById('hidden-nova-situacao');

  document.querySelectorAll('.menu-suspenso-item[data-situacao]').forEach(function (item) {
    item.addEventListener('click', function () {
      var anuncioId = this.dataset.anuncioId;
      var situacao = this.dataset.situacao;

      if (!confirm('Confirmar alteração de status para "' + situacao + '"?')) return;

      hiddenAnuncioId.value = anuncioId;
      hiddenNovaSituacao.value = situacao;
      formStatus.submit();
    });
  });
})();
