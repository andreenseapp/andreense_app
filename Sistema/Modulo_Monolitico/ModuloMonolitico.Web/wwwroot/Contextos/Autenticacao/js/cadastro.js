(function () {
  'use strict';

  function abrirModal(id) {
    var modal = document.getElementById(id);
    if (modal) {
      modal.classList.add('esta-aberto');
      modal.setAttribute('aria-hidden', 'false');
    }
  }

  function fecharModal(id) {
    var modal = document.getElementById(id);
    if (modal) {
      modal.classList.remove('esta-aberto');
      modal.setAttribute('aria-hidden', 'true');
    }
  }

  function validarFormularioCadastro(formulario) {
    var valido = true;
    var campos = [
      { id: 'Formulario_Nome', valFor: 'Formulario.Nome', msg: 'Informe o nome.' },
      { id: 'Formulario_Email', valFor: 'Formulario.Email', msg: 'Informe o e-mail.' },
      { id: 'Formulario_Senha', valFor: 'Formulario.Senha', msg: 'Informe a senha.' },
      { id: 'Formulario_ConfirmarSenha', valFor: 'Formulario.ConfirmarSenha', msg: 'Confirme a senha.' }
    ];

    campos.forEach(function (campo) {
      var input = document.getElementById(campo.id);
      var msgEl = document.querySelector('[data-valmsg-for="' + campo.valFor + '"]');

      if (!input || !input.value.trim()) {
        if (msgEl) msgEl.textContent = campo.msg;
        valido = false;
      } else if (msgEl) {
        msgEl.textContent = '';
      }
    });

    var termos = document.getElementById('Formulario_AceitouTermos');
    var termosMsg = document.querySelector('[data-valmsg-for="Formulario.AceitouTermos"]');
    if (termos && !termos.checked) {
      if (termosMsg) termosMsg.textContent = 'Você deve ler e aceitar os termos de uso para continuar.';
      valido = false;
    } else if (termosMsg) {
      termosMsg.textContent = '';
    }

    return valido;
  }

  function configurarModaisDocumentos() {
    document.querySelectorAll('[data-abrir-modal-doc]').forEach(function (link) {
      link.addEventListener('click', function (e) {
        e.preventDefault();
        e.stopPropagation();
        abrirModal(link.getAttribute('data-abrir-modal-doc'));
      });
    });

    document.querySelectorAll('[data-fechar-modal-doc]').forEach(function (botao) {
      botao.addEventListener('click', function () {
        fecharModal(botao.getAttribute('data-fechar-modal-doc'));
      });
    });
  }

  function configurarEnvioCadastroAjax() {
    var form = document.getElementById('form-cadastro');
    if (!form || !window.RequisicaoAplicacao) return;

    window.RequisicaoAplicacao.vincularFormulario(form, {
      botao: '#btn-cadastro',
      mensagemErroPadrao: 'Não foi possível criar a conta. Tente novamente.',
      tituloErro: 'Não foi possível cadastrar',
      validar: function () {
        return validarFormularioCadastro(form);
      }
    });
  }

  configurarModaisDocumentos();
  configurarEnvioCadastroAjax();
})();
