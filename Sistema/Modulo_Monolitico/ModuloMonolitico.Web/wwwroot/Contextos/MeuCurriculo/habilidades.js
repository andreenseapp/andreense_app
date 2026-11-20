(function () {
document.addEventListener('DOMContentLoaded', function () {
    let habilidades = window.habilidadesIniciais || [];
    const container = document.getElementById('tags-habilidades');
    const inputJson = document.getElementById('habilidades-json');
    const inputTexto = document.getElementById('input-habilidade');
    const btnAdicionar = document.getElementById('btn-adicionar-habilidade');
    const sugestoes = document.querySelectorAll('.sugestao-tag');

    function render() {
        if (!container) return;
        container.innerHTML = '';
        if (habilidades.length === 0) {
            container.innerHTML = '<span style="color:#9ca3af;font-size:13px;">Nenhuma habilidade adicionada.</span>';
        } else {
            habilidades.forEach((item, index) => {
                const span = document.createElement('span');
                span.className = 'tag-habilidade';
                span.innerHTML = `
                    ${item}
                    <button type="button" class="tag-remover" data-idx="${index}">×</button>
                `;
                span.querySelector('button').addEventListener('click', () => {
                    habilidades.splice(index, 1);
                    render();
                });
                container.appendChild(span);
            });
        }
        if (inputJson) {
            inputJson.value = JSON.stringify(habilidades);
        }
    }

    function adicionar(texto) {
        if (!texto) return;
        const limpo = texto.trim();
        if (limpo.length < 2) return;
        if (!habilidades.includes(limpo)) {
            habilidades.push(limpo);
            render();
        }
    }

    if (btnAdicionar && inputTexto) {
        btnAdicionar.addEventListener('click', () => {
            adicionar(inputTexto.value);
            inputTexto.value = '';
        });

        inputTexto.addEventListener('keydown', (e) => {
            if (e.key === 'Enter') {
                e.preventDefault();
                adicionar(inputTexto.value);
                inputTexto.value = '';
            }
        });
    }

    sugestoes.forEach(btn => {
        btn.addEventListener('click', () => {
            adicionar(btn.getAttribute('data-sugestao'));
        });
    });

    render();
});

})();
