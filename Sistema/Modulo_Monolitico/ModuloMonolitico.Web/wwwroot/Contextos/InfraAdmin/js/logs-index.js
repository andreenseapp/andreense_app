(function () {
  'use strict';

  document.addEventListener('DOMContentLoaded', function () {
    var btnAtualizar = document.getElementById('btn-atualizar-logs');
    if (btnAtualizar) {
      btnAtualizar.addEventListener('click', function () {
        window.location.reload();
      });
    }

    var buscaRapida = document.getElementById('TermoPesquisa');
    var linhas = document.querySelectorAll('.log-linha');

    if (!buscaRapida || linhas.length === 0)
      return;

    var form = buscaRapida.closest('form');
    var debounceTimer;

    buscaRapida.addEventListener('input', function () {
      clearTimeout(debounceTimer);
      debounceTimer = setTimeout(function () {
        var termo = buscaRapida.value.trim().toLowerCase();

        linhas.forEach(function (linha) {
          if (!termo) {
            linha.classList.remove('oculta');
            return;
          }

          var texto = linha.textContent.toLowerCase();
          linha.classList.toggle('oculta', !texto.includes(termo));
        });
      }, 180);
    });

    if (form) {
      form.addEventListener('submit', function () {
        linhas.forEach(function (linha) {
          linha.classList.remove('oculta');
        });
      });
    }
  });
})();
