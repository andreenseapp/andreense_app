(function () {
document.documentElement.setAttribute('data-tema', localStorage.getItem('tema') || 'light');

(function () {
  const modalDeConvite = document.getElementById('modal-convite');
  const modalDeEdicao = document.getElementById('modal-editar');
  const inputUsuarioEditar = document.getElementById('editarUsuarioId');
  const selectGrupoEditar = document.getElementById('NovoGrupoId');

  function abrirModal(modal) {
    if (!modal) return;
    modal.classList.add('aberto');
  }

  function fecharModal(modal) {
    if (!modal) return;
    modal.classList.remove('aberto');
  }

  document.querySelectorAll('.botao-abrir-convite').forEach(function (botao) {
    botao.addEventListener('click', function () {
      abrirModal(modalDeConvite);
    });
  });

  document.querySelectorAll('.botao-fechar-convite').forEach(function (botao) {
    botao.addEventListener('click', function () {
      fecharModal(modalDeConvite);
    });
  });

  document.querySelectorAll('.botao-fechar-editar').forEach(function (botao) {
    botao.addEventListener('click', function () {
      fecharModal(modalDeEdicao);
    });
  });

  document.querySelectorAll('.botao-abrir-editar').forEach(function (botao) {
    botao.addEventListener('click', function () {
      if (inputUsuarioEditar) {
        inputUsuarioEditar.value = botao.getAttribute('data-usuario-id') || '';
      }

      const perfil = botao.getAttribute('data-perfil') || '';
      if (selectGrupoEditar) {
        Array.from(selectGrupoEditar.options).forEach(function (opt) {
          opt.selected = opt.text === perfil;
        });
      }

      abrirModal(modalDeEdicao);
    });
  });

  if (modalDeConvite) {
    modalDeConvite.addEventListener('click', function (e) {
      if (e.target === modalDeConvite) fecharModal(modalDeConvite);
    });
  }

  if (modalDeEdicao) {
    modalDeEdicao.addEventListener('click', function (e) {
      if (e.target === modalDeEdicao) fecharModal(modalDeEdicao);
    });
  }
})();

})();
