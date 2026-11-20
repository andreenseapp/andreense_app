(function () {
  'use strict';

  document.documentElement.setAttribute('data-tema', localStorage.getItem('tema') || 'light');

  document.addEventListener('DOMContentLoaded', function () {
    document.querySelectorAll('.btn-marcar-todos').forEach(function (botao) {
      botao.addEventListener('click', function () {
        var mod = botao.dataset.modulo;
        var checks = document.querySelectorAll('input[type=checkbox][data-modulo="' + mod + '"]');
        var allChecked = Array.from(checks).every(function (c) { return c.checked; });

        checks.forEach(function (c) {
          c.checked = !allChecked;
          var wrap = c.closest('.recurso-check');
          if (wrap) wrap.classList.toggle('checked', !allChecked);
        });

        botao.textContent = allChecked ? 'marcar todos' : 'desmarcar todos';
      });
    });

    document.querySelectorAll('.recurso-check input[type=checkbox]').forEach(function (cb) {
      cb.addEventListener('change', function () {
        var wrap = cb.closest('.recurso-check');
        if (wrap) wrap.classList.toggle('checked', cb.checked);
      });
    });
  });
})();
