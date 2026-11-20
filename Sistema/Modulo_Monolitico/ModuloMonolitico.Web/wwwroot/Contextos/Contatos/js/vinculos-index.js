(function () {
document.querySelectorAll('[data-revogar-id]').forEach(function (botao) {
    botao.addEventListener('click', function () {
        var vinculoId = botao.getAttribute('data-revogar-id');
        var nome = botao.getAttribute('data-revogar-nome') || 'este vínculo';
        document.getElementById('campo-revogacao-id').value = vinculoId;
        document.getElementById('modal-revogacao-texto').innerHTML =
            'Deseja revogar o vínculo com <strong>' + nome + '</strong>? Esta ação não pode ser desfeita automaticamente.';
        document.getElementById('modal-revogacao').classList.add('exibido');
    });
});

document.getElementById('botao-cancelar-revogacao')?.addEventListener('click', function () {
    document.getElementById('modal-revogacao').classList.remove('exibido');
});

document.getElementById('modal-revogacao')?.addEventListener('click', function (e) {
    if (e.target === this) this.classList.remove('exibido');
});

})();
