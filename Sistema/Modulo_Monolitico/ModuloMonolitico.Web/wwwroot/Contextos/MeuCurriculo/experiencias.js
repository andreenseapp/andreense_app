(function () {
document.addEventListener('DOMContentLoaded', function () {
    const checkEmAndamento = document.querySelector('[data-em-andamento]');
    const grupoDataFim = document.getElementById('grupo-data-fim');

    function alternarDataFim() {
        if (!checkEmAndamento || !grupoDataFim) return;
        if (checkEmAndamento.checked) {
            grupoDataFim.style.display = 'none';
        } else {
            grupoDataFim.style.display = 'flex';
        }
    }

    if (checkEmAndamento) {
        checkEmAndamento.addEventListener('change', alternarDataFim);
        alternarDataFim();
    }
});

})();
