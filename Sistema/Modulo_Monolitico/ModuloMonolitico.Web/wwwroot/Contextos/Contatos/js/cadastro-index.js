(function () {
  document.addEventListener('DOMContentLoaded', function () {
    var form = document.getElementById('formularioFiltros');
    var inputBusca = document.querySelector('input[name="Filtros.Busca"]');
    var temporizadorAtraso = null;

    if (inputBusca && form) {
      if (inputBusca.value.length > 0) {
        setTimeout(function () {
          inputBusca.selectionStart = inputBusca.selectionEnd = inputBusca.value.length;
          inputBusca.focus();
        }, 10);
      }

      inputBusca.addEventListener('input', function () {
        clearTimeout(temporizadorAtraso);
        temporizadorAtraso = setTimeout(function () {
          form.submit();
        }, 600);
      });
    }

    if (form) {
      form.querySelectorAll('[data-envio-automatico="true"]').forEach(function (el) {
        el.addEventListener('change', function () {
          form.submit();
        });
      });
    }

    document.querySelectorAll('.app-table th.ordenavel[data-ordenar]').forEach(function (th) {
      th.addEventListener('click', function () {
        if (typeof ordenarTabela === 'function') {
          ordenarTabela(th.getAttribute('data-ordenar'));
        }
      });
    });
  });
})();
