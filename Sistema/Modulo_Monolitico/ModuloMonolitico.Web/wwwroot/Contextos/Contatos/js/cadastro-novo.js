(function () {
  function ativarAba(tipo) {
    document.querySelectorAll('.botao-aba[data-aba]').forEach(function (botao) {
      var ativo = botao.getAttribute('data-aba') === tipo;
      botao.classList.toggle('ativo', ativo);
      botao.setAttribute('aria-selected', ativo ? 'true' : 'false');
    });

    document.querySelectorAll('.painel-aba').forEach(function (panel) {
      panel.classList.toggle('ativo', panel.id === 'painel-' + tipo);
    });

    if (tipo === 'fisica' || tipo === 'juridica') {
      var tipoInput = document.getElementById('tipo-Contato');
      if (tipoInput) tipoInput.value = tipo;
    }
  }

  function sincronizarAbasPapeis() {
    var form = document.getElementById('formulario-contato');
    var papelForcado = form ? (form.getAttribute('data-papel-forcado') || '') : '';
    var papeis = ['cliente', 'fornecedor', 'transportador', 'lead', 'prospect', 'funcionario'];

    papeis.forEach(function (papel) {
      var checkbox = document.getElementById('papel-' + papel);
      var tab = document.getElementById('tab-' + papel);
      if (!tab) return;

      var visivel = false;
      if (papelForcado) {
        visivel = papel === papelForcado;
      } else if (checkbox) {
        visivel = checkbox.checked;
      }

      tab.classList.toggle('is-visible', visivel);

      if (!visivel && tab.classList.contains('ativo')) {
        ativarAba('papeis');
      }
    });
  }

  document.addEventListener('DOMContentLoaded', function () {
    document.querySelectorAll('.botao-aba[data-aba]').forEach(function (botao) {
      botao.addEventListener('click', function () {
        ativarAba(botao.getAttribute('data-aba'));
      });
    });

    document.querySelectorAll('.papel-alternar').forEach(function (chk) {
      chk.addEventListener('change', sincronizarAbasPapeis);
    });

    sincronizarAbasPapeis();
  });
})();
