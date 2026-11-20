(function () {
document.documentElement.setAttribute('data-tema', localStorage.getItem('tema') || 'light');

function abrirModalPlano(contaId, contaNome, valorAtual) {
  document.getElementById('modal-conta-id').value = contaId;
  document.getElementById('modal-conta-nome').textContent = contaNome;
  document.getElementById('modal-valor').value = valorAtual;
  document.getElementById('modal-plano').classList.add('open');
}

document.querySelectorAll('.btn-abrir-plano').forEach(function (botao) {
  botao.addEventListener('click', function () {
    abrirModalPlano(
      botao.dataset.contaId,
      botao.dataset.contaNome,
      botao.dataset.valor || '0'
    );
  });
});

const modalPlano = document.getElementById('modal-plano');
modalPlano.addEventListener('click', function (e) {
  if (e.target === this) this.classList.remove('open');
});

document.getElementById('modal-plano-cancelar')?.addEventListener('click', function () {
  modalPlano.classList.remove('open');
});
})();
