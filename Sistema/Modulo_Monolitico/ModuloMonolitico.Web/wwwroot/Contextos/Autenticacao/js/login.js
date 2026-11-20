(function () {
  'use strict';

  function validarFormularioLogin(formulario) {
    var valido = true;
    var campoEmail = document.getElementById('Formulario_Email');
    var campoSenha = document.getElementById('Formulario_Senha');

    if (!campoEmail || !campoEmail.value.trim()) {
      valido = false;
    }

    if (!campoSenha || !campoSenha.value.trim()) {
      valido = false;
    }

    if (!valido && typeof window.exibirToast === 'function') {
      window.exibirToast('Informe e-mail e senha.', 'warning');
    }

    return valido;
  }

  function configurarEnvioLoginAjax() {
    var formulario = document.getElementById('form-login');
    if (!formulario || !window.RequisicaoAplicacao) return;

    window.RequisicaoAplicacao.vincularFormulario(formulario, {
      botao: '#btn-login',
      mensagemErroPadrao: 'Credenciais inválidas. Verifique seu e-mail e senha.',
      tituloErro: 'Não foi possível entrar',
      validar: function () {
        return validarFormularioLogin(formulario);
      }
    });
  }

  function enviarEntradaGoogle(tokenIdentidade) {
    var rotaLoginGoogle = document.body.dataset.rotaLoginGoogle || '/Contextos/Autenticacao/LoginGoogle';
    if (!window.RequisicaoAplicacao) return;

    window.RequisicaoAplicacao.enviarJson(rotaLoginGoogle, { idToken: tokenIdentidade }, {
      mensagemErroPadrao: 'Não foi possível entrar com Google.',
      tituloErro: 'Não foi possível entrar'
    });
  }

  function inicializarEntradaGoogle() {
    var container = document.getElementById('btn-google-login-container');
    if (!container) return;

    var idCliente = container.dataset.googleIdCliente;
    if (!idCliente || typeof google === 'undefined' || !google.accounts) return;

    google.accounts.id.initialize({
      client_id: idCliente,
      callback: function (resposta) {
        if (resposta && resposta.credential) {
          enviarEntradaGoogle(resposta.credential);
        }
      }
    });

    google.accounts.id.renderButton(container, {
      theme: 'outline',
      size: 'large',
      text: 'signin_with',
      width: '300'
    });
  }

  function configurarLinkEsqueciSenha() {
    var link = document.getElementById('link-esqueci-senha');
    var campoEmail = document.getElementById('Formulario_Email');
    var rotaEsqueciSenha = document.body.dataset.rotaEsqueciSenha || '/Contextos/Autenticacao/EsqueciSenha';
    if (!link) return;

    link.addEventListener('click', function () {
      var email = campoEmail ? campoEmail.value.trim() : '';
      if (email) {
        link.href = rotaEsqueciSenha + '?email=' + encodeURIComponent(email);
      }
    });
  }

  function inicializarLogin() {
    configurarEnvioLoginAjax();
    configurarLinkEsqueciSenha();

    if (document.getElementById('btn-google-login-container')) {
      window.addEventListener('load', inicializarEntradaGoogle);
    }

    var chkManter = document.getElementById('Formulario_ManterConectado');
    if (chkManter) {
      var valorSalvo = localStorage.getItem('manterConectado');
      if (valorSalvo !== null) {
        chkManter.checked = valorSalvo === 'true';
      }
      chkManter.addEventListener('change', function () {
        localStorage.setItem('manterConectado', this.checked);
      });
    }
  }

  inicializarLogin();
})();
