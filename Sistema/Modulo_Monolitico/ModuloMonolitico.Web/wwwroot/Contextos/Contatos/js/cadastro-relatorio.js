(function () {
  document.addEventListener('DOMContentLoaded', function () {
    var form = document.getElementById('formularioFiltros');
    var inputBusca = document.querySelector('input[name="FiltroNome"]');
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
