(function () {
    function alternarCsc(modelo) {
        var campos = document.getElementById('camposCsc');
        if (!campos) return;
        campos.classList.toggle('campos-csc--hidden', modelo !== '65');
    }

    function verificarAmbiente(valor) {
        var grid = document.getElementById('Ambiente')?.closest('.grade-formulario-3');
        if (!grid) return;

        var alertas = document.querySelectorAll('.alerta-producao');
        if (valor === '1') {
            if (!alertas.length) {
                var div = document.createElement('div');
                div.className = 'alerta-producao';
                div.innerHTML = '<svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M10.29 3.86L1.82 18a2 2 0 0 0 1.71 3h16.94a2 2 0 0 0 1.71-3L13.71 3.86a2 2 0 0 0-3.42 0z"></path><line x1="12" y1="9" x2="12" y2="13"></line><line x1="12" y1="17" x2="12.01" y2="17"></line></svg><span>Ambiente de <strong>Produção</strong> ativo. As notas emitidas terão validade legal perante a Receita Federal.</span>';
                grid.before(div);
            }
        } else {
            alertas.forEach(function (a) { a.remove(); });
        }
    }

    document.addEventListener('DOMContentLoaded', function () {
        var ambiente = document.getElementById('Ambiente');
        var modelo = document.getElementById('Modelo');

        if (ambiente) {
            ambiente.addEventListener('change', function () { verificarAmbiente(this.value); });
        }
        if (modelo) {
            alternarCsc(modelo.value);
            modelo.addEventListener('change', function () { alternarCsc(this.value); });
        }
    });
})();
