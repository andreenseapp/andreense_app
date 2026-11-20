(function () {
function gerarUuid() {
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
        var r = Math.random() * 16 | 0, v = c === 'x' ? r : (r & 0x3 | 0x8);
        return v.toString(16);
    });
}

function adicionarEndereco() {
    const lista = document.getElementById('enderecos-lista');
    if (!lista) return;

    const items = lista.querySelectorAll('.endereco-item');
    let nextIndex = 0;
    if (items.length > 0) {
        const lastItem = items[items.length - 1];
        nextIndex = parseInt(lastItem.getAttribute('data-index'), 10) + 1;
    }

    const newId = gerarUuid();
    const template = document.getElementById('endereco-template');
    if (!template) return;

    const html = template.innerHTML.replace(/\{0\}/g, nextIndex).replace(/\{1\}/g, newId);
    lista.insertAdjacentHTML('beforeend', html);

    if (typeof aplicarMascaras === 'function') {
        aplicarMascaras();
    }
}

function removerEndereco(botao) {
    const item = botao.closest('.endereco-item');
    if (item) item.remove();
}

document.addEventListener('DOMContentLoaded', () => {
    document.getElementById('btn-adicionar-endereco')?.addEventListener('click', adicionarEndereco);

    document.getElementById('enderecos-lista')?.addEventListener('click', (e) => {
        const botao = e.target.closest('[data-acao="remover-endereco"]');
        if (botao) removerEndereco(botao);
    });
});

})();
