(function () {
    'use strict';

    document.addEventListener('DOMContentLoaded', function () {
        document.querySelectorAll('.chip-ordenacao[data-ordenar]').forEach(function (botao) {
            botao.addEventListener('click', function () {
                if (typeof ordenarTabela === 'function') {
                    ordenarTabela(botao.getAttribute('data-ordenar'));
                }
            });
        });
    });
})();
