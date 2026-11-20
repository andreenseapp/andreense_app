(function () {
document.addEventListener('DOMContentLoaded', function () {
    let idiomas = window.idiomasIniciais || [];
    const container = document.getElementById('lista-idiomas');
    const inputJson = document.getElementById('idiomas-json');
    const selIdioma = document.getElementById('sel-idioma');
    const selNivel = document.getElementById('sel-nivel');
    const btnAdicionar = document.getElementById('btn-adicionar-idioma');

    function render() {
        if (!container) return;
        container.innerHTML = '';
        idiomas.forEach((item, index) => {
            const div = document.createElement('div');
            div.className = 'idioma-item-row';
            div.innerHTML = `
                <div class="idioma-item-info">
                    <strong>${item.Idioma}</strong>
                    <span>(${item.Nivel})</span>
                </div>
                <button type="button" class="btn btn-danger-ghost btn-sm" data-idx="${index}">Remover</button>
            `;
            div.querySelector('button').addEventListener('click', () => {
                idiomas.splice(index, 1);
                render();
            });
            container.appendChild(div);
        });
        if (inputJson) {
            inputJson.value = JSON.stringify(idiomas);
        }
    }

    if (btnAdicionar) {
        btnAdicionar.addEventListener('click', () => {
            const idioma = selIdioma ? selIdioma.value : '';
            const nivel = selNivel ? selNivel.value : 'intermediario';
            if (!idioma) return;

            if (!idiomas.some(i => i.Idioma === idioma)) {
                idiomas.push({ Idioma: idioma, Nivel: nivel });
                render();
            }
            if (selIdioma) selIdioma.value = '';
        });
    }

    render();
});

})();
