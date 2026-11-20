(function () {
  'use strict';

  function validarFormularioRedefinir(form) {
    var valid = true;
    var campos = [
      { id: 'NovaSenha', msg: 'Informe a nova senha.' },
      { id: 'ConfirmarNovaSenha', msg: 'Confirme a nova senha.' }
    ];

    campos.forEach(function (campo) {
      var input = document.getElementById(campo.id);
      var msgEl = document.querySelector('[data-valmsg-for="' + campo.id + '"]');

      if (!input || !input.value.trim()) {
        if (msgEl) msgEl.textContent = campo.msg;
        valid = false;
      } else if (msgEl) {
        msgEl.textContent = '';
      }
    });

    return valid;
  }

  function configurarEnvioRedefinirAjax() {
    var form = document.getElementById('form-redefinir');
    if (!form || !window.RequisicaoAplicacao) return;

    window.RequisicaoAplicacao.vincularFormulario(form, {
      botao: '#btn-salvar',
      mensagemErroPadrao: 'Não foi possível redefinir a senha. Tente novamente.',
      tituloErro: 'Redefinição de senha',
      validar: function () {
        return validarFormularioRedefinir(form);
      }
    });
  }

  function iniciarContagemRegressiva(botao, textoOriginal) {
    var segundos = 60;
    botao.disabled = true;
    botao.textContent = 'Aguarde ' + segundos + 's';

    var intervalo = setInterval(function () {
      segundos--;
      if (segundos <= 0) {
        clearInterval(intervalo);
        botao.disabled = false;
        botao.textContent = textoOriginal;
      } else {
        botao.textContent = 'Aguarde ' + segundos + 's';
      }
    }, 1000);
  }

  function configurarReenvioCodigo() {
    var btnReenviar = document.getElementById('btn-reenviar');
    var emailInput = document.getElementById('Email');
    if (!btnReenviar || !emailInput) return;

    btnReenviar.addEventListener('click', function () {
      var email = emailInput.value.trim();
      if (!email) {
        if (window.exibirMensagemUsuario) {
          window.exibirMensagemUsuario('E-mail não informado.', { tipo: 'error' });
        } else {
          alerta('E-mail não informado.');
        }
        return;
      }

      btnReenviar.disabled = true;
      var textoOriginal = btnReenviar.textContent;
      btnReenviar.textContent = 'Enviando...';

      var tokenInput = document.querySelector('input[name="__RequestVerificationToken"]');
      var headers = {
        'X-Requested-With': 'XMLHttpRequest'
      };
      
      var formData = new FormData();
      formData.append('Email', email);
      if (tokenInput) {
        formData.append('__RequestVerificationToken', tokenInput.value);
      }

      window.requisitarApp('?handler=ReenviarCodigo', {
        method: 'POST',
        headers: headers,
        body: formData
      })
      .then(function (response) {
        return response.json().then(function (data) {
          return { ok: response.ok, data: data };
        });
      })
      .then(function (res) {
        if (res.ok && res.data && res.data.sucesso) {
          if (window.exibirMensagemUsuario) {
            window.exibirMensagemUsuario(res.data.mensagem || 'Código reenviado com sucesso!', { tipo: 'success' });
          } else {
            alerta('Código reenviado com sucesso!');
          }
          iniciarContagemRegressiva(btnReenviar, textoOriginal);
        } else {
          var msg = (res.data && res.data.mensagem) || 'Não foi possível reenviar o código.';
          if (window.exibirMensagemUsuario) {
            window.exibirMensagemUsuario(msg, { tipo: 'error' });
          } else {
            alerta(msg);
          }
          btnReenviar.disabled = false;
          btnReenviar.textContent = textoOriginal;
        }
      })
      .catch(function () {
        if (window.exibirMensagemUsuario) {
          window.exibirMensagemUsuario('Erro de conexão. Tente novamente.', { tipo: 'error' });
        } else {
          alerta('Erro de conexão. Tente novamente.');
        }
        btnReenviar.disabled = false;
        btnReenviar.textContent = textoOriginal;
      });
    });
  }

  function configurarValidacaoEtapaCodigo() {
    var btnValidar = document.getElementById('btn-validar-codigo');
    var emailInput = document.getElementById('Email');
    var codigoInput = document.getElementById('Codigo');
    var etapaCodigo = document.getElementById('etapa-codigo');
    var etapaSenha = document.getElementById('etapa-senha');
    var titleEl = document.getElementById('titulo-autenticacao');
    var subtitleEl = document.getElementById('subtitulo-autenticacao');

    if (!btnValidar || !emailInput || !codigoInput || !etapaCodigo || !etapaSenha) return;

    btnValidar.addEventListener('click', function () {
      var email = emailInput.value.trim();
      var codigo = codigoInput.value.trim();
      var msgEl = document.querySelector('[data-valmsg-for="Codigo"]');

      if (!codigo) {
        if (msgEl) msgEl.textContent = 'Informe o código.';
        return;
      } else if (msgEl) {
        msgEl.textContent = '';
      }

      btnValidar.disabled = true;
      var textSpan = btnValidar.querySelector('.botao-texto');
      var spinner = btnValidar.querySelector('.htmx-indicator');
      var textoOriginal = textSpan ? textSpan.textContent : 'Verificar Código';

      if (textSpan) textSpan.textContent = 'Verificando...';
      if (spinner) spinner.style.display = 'inline-block';

      var tokenInput = document.querySelector('input[name="__RequestVerificationToken"]');
      var formData = new FormData();
      formData.append('Email', email);
      formData.append('Codigo', codigo);
      if (tokenInput) {
        formData.append('__RequestVerificationToken', tokenInput.value);
      }

      window.requisitarApp('?handler=ValidarCodigoPrevio', {
        method: 'POST',
        headers: { 'X-Requested-With': 'XMLHttpRequest' },
        body: formData
      })
      .then(function (response) {
        return response.json().then(function (data) {
          return { ok: response.ok, data: data };
        });
      })
      .then(function (res) {
        if (res.ok && res.data && res.data.sucesso) {
          etapaCodigo.classList.add('hidden');
          etapaSenha.classList.remove('hidden');
          etapaSenha.classList.add('entrada-fade-ativa');

          if (titleEl) titleEl.textContent = 'Criar Nova Senha';
          if (subtitleEl) subtitleEl.textContent = 'Escolha uma senha forte para proteger seu acesso.';

          var inputSenha = document.getElementById('NovaSenha');
          if (inputSenha) inputSenha.focus();
        } else {
          var msg = (res.data && res.data.mensagem) || 'Código inválido ou expirado.';
          if (window.exibirMensagemUsuario) {
            window.exibirMensagemUsuario(msg, { tipo: 'error' });
          } else {
            alerta(msg);
          }
          if (msgEl) msgEl.textContent = msg;
        }
      })
      .catch(function () {
        if (window.exibirMensagemUsuario) {
          window.exibirMensagemUsuario('Erro de conexão. Tente novamente.', { tipo: 'error' });
        } else {
          alerta('Erro de conexão. Tente novamente.');
        }
      })
      .finally(function () {
        btnValidar.disabled = false;
        if (textSpan) textSpan.textContent = textoOriginal;
        if (spinner) spinner.style.display = 'none';
      });
    });
  }

  configurarEnvioRedefinirAjax();
  configurarReenvioCodigo();
  configurarValidacaoEtapaCodigo();
})();
