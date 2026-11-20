(function () {
  document.addEventListener('DOMContentLoaded', function () {
    var form = document.getElementById('formFiltros');
    var inputBusca = document.querySelector('input[name="Filtros.Busca"]');
    var temporizadorAtraso = null;

    if (inputBusca && form) {
      inputBusca.addEventListener('input', function () {
        clearTimeout(temporizadorAtraso);
        temporizadorAtraso = setTimeout(function () {
          form.submit();
        }, 600);
      });
    }
  });
})();
