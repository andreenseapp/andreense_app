(function () {
document.addEventListener('DOMContentLoaded', function () {
    const checkPcd = document.getElementById('check-pcd');
    const painelPcd = document.getElementById('painel-pcd');
    if (checkPcd && painelPcd) {
        checkPcd.addEventListener('change', function () {
            if (checkPcd.checked) {
                painelPcd.classList.remove('hidden');
            } else {
                painelPcd.classList.add('hidden');
            }
        });
    }

    const cnhChips = document.querySelectorAll('.cnh-chip');
    const inputCnh = document.getElementById('input-cnh-categorias');

    function atualizarInputCnh() {
        const selecionadas = [];
        cnhChips.forEach(chip => {
            const check = chip.querySelector('input');
            if (check && check.checked) {
                selecionadas.push(check.value);
                chip.classList.add('ativo');
            } else {
                chip.classList.remove('ativo');
            }
        });
        if (inputCnh) {
            inputCnh.value = selecionadas.join(',');
        }
    }

    cnhChips.forEach(chip => {
        chip.addEventListener('click', function (e) {
            e.preventDefault();
            const check = chip.querySelector('input');
            if (check) {
                check.checked = !check.checked;
                atualizarInputCnh();
            }
        });
    });

    if (inputCnh && inputCnh.value) {
        const cats = inputCnh.value.split(',');
        cnhChips.forEach(chip => {
            const check = chip.querySelector('input');
            if (check && cats.includes(check.value)) {
                check.checked = true;
            }
        });
        atualizarInputCnh();
    }
});

})();
