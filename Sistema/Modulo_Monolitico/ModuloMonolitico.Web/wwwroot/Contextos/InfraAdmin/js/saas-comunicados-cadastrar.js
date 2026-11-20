(function () {
document.documentElement.setAttribute('data-tema', localStorage.getItem('tema') || 'light');

(function () {
  const tipoSelect = document.getElementById('Tipo');
  const opcoesWrapper = document.getElementById('enquete-opcoes-wrapper');
  const opcoesList = document.getElementById('opcoes-list');
  const btnAddOpcao = document.querySelector('.btn-nova-opcao');

  if (!tipoSelect || !opcoesWrapper || !opcoesList) return;

  function atualizarVisibilidadeEnquete() {
    const isEnquete = tipoSelect.value === 'enquete';
    opcoesWrapper.classList.toggle('is-visible', isEnquete);
    opcoesList.querySelectorAll('input[name="OpcoesTextos"]').forEach(function (input) {
      input.required = isEnquete;
    });
  }

  function atualizarBotoesRemover() {
    const itens = opcoesList.querySelectorAll('.opcao-item');
    const desabilitar = itens.length <= 2;
    itens.forEach(function (item) {
      const botao = item.querySelector('.btn-remover-opcao');
      if (botao) botao.disabled = desabilitar;
    });
  }

  function criarOpcaoItem() {
    const item = document.createElement('div');
    item.className = 'opcao-item';
    item.innerHTML =
      '<input type="text" name="OpcoesTextos" class="campo-controle opcao-input" placeholder="Texto da opção..." />' +
      '<button type="button" class="btn-deletar-opcao btn-remover-opcao" title="Remover opção">' +
      '<svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><polyline points="3 6 5 6 21 6"></polyline><path d="M19 6v14a2 2 0 0 1-2 2H7a2 2 0 0 1-2-2V6m3 0V4a2 2 0 0 1 2-2h4a2 2 0 0 1 2 2v2"></path></svg>' +
      '</button>';
    return item;
  }

  opcoesList.addEventListener('click', function (e) {
    const botao = e.target.closest('.btn-remover-opcao');
    if (!botao || botao.disabled) return;
    botao.closest('.opcao-item')?.remove();
    atualizarBotoesRemover();
    atualizarVisibilidadeEnquete();
  });

  btnAddOpcao?.addEventListener('click', function () {
    opcoesList.appendChild(criarOpcaoItem());
    atualizarBotoesRemover();
    atualizarVisibilidadeEnquete();
  });

  tipoSelect.addEventListener('change', atualizarVisibilidadeEnquete);
  atualizarBotoesRemover();
  atualizarVisibilidadeEnquete();
})();

})();
