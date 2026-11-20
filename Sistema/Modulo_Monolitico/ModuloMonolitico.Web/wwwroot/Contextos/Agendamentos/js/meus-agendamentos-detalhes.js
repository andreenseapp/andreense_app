(function () {
    const modal = document.getElementById('modalCancelar');
    if (!modal) return;

    function abrirModal() {
        modal.classList.add('open');
    }

    function fecharModal() {
        modal.classList.remove('open');
    }

    document.querySelectorAll('.btn-abrir-cancelar').forEach(function (botao) {
        botao.addEventListener('click', abrirModal);
    });

    document.querySelectorAll('.btn-fechar-cancelar').forEach(function (botao) {
        botao.addEventListener('click', fecharModal);
    });

    modal.addEventListener('click', function (e) {
        if (e.target === modal) fecharModal();
    });
})();
