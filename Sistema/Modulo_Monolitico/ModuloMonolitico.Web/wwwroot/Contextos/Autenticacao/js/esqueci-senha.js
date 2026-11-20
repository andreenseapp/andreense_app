(function () {
  'use strict';

  function validarFormularioEsqueci(form) {
    var emailInput = document.getElementById('Email');
    var emailMsg = document.querySelector('[data-valmsg-for="Email"]');

    if (!emailInput || !emailInput.value.trim()) {
      if (emailMsg) emailMsg.textContent = 'Informe o e-mail.';
      return false;
    }

    if (emailMsg) emailMsg.textContent = '';
    return true;
  }

  function configurarEnvioEsqueciAjax() {
    var form = document.getElementById('form-esqueci');
    if (!form || !window.RequisicaoAplicacao) return;

    window.RequisicaoAplicacao.vincularFormulario(form, {
      botao: '#btn-enviar',
      mensagemErroPadrao: 'Não foi possível enviar o código. Tente novamente.',
      tituloErro: 'Recuperação de senha',
      validar: function () {
        return validarFormularioEsqueci(form);
      }
    });
  }

  configurarEnvioEsqueciAjax();
})();
