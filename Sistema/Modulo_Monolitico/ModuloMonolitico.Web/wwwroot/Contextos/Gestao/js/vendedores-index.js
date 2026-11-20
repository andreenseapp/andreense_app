(function () {
  document.addEventListener('DOMContentLoaded', function () {
    var form = document.getElementById('formularioFiltros');
    var inputBusca = document.querySelector('input[name="Filtros.Busca"]');
    var debounceTimer = null;

    if (inputBusca && form) {
      inputBusca.addEventListener('input', function () {
        clearTimeout(debounceTimer);
        debounceTimer = setTimeout(function () {
          form.submit();
        }, 600);
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
