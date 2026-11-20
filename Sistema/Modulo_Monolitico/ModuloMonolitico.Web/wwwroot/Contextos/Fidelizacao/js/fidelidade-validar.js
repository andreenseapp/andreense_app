(function () {
document.querySelectorAll('.botao-imprimir').forEach(function (botao) {
    botao.addEventListener('click', function () {
        window.print();
    });
});

})();
