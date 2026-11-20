(function () {
    function alternarLinha(chk) {
        const dia = chk.dataset.dia;
        const row = document.getElementById('row_' + dia);
        if (!row) return;
        if (chk.checked) {
            row.classList.remove('is-disabled');
        } else {
            row.classList.add('is-disabled');
        }
    }

    document.querySelectorAll('.horario-toggle').forEach(function (chk) {
        chk.addEventListener('change', function () {
            alternarLinha(chk);
        });
    });

    const filtroProf = document.querySelector('#formFiltroProf select');
    if (filtroProf) {
        filtroProf.addEventListener('change', function () {
            document.getElementById('formFiltroProf').submit();
        });
    }

    const formProf = document.getElementById('formProf');
    document.querySelectorAll('.btn-editar-prof').forEach(function (botao) {
        botao.addEventListener('click', function () {
            document.getElementById('profUsuarioId').value = botao.dataset.usuarioId || '';
            document.getElementById('profContatoId').value = botao.dataset.contatoId || '';
            document.getElementById('profNomeTitulo').textContent = '- ' + (botao.dataset.nome || '');

            const ids = botao.dataset.servicos ? botao.dataset.servicos.split(',') : [];
            document.querySelectorAll('.prof-servico-check').forEach(function (chk) {
                chk.checked = ids.includes(chk.value);
            });

            formProf.classList.add('aberto');
            formProf.scrollIntoView({ behavior: 'smooth', block: 'center' });
        });
    });

    document.querySelectorAll('.btn-fechar-prof').forEach(function (botao) {
        botao.addEventListener('click', function () {
            formProf.classList.remove('aberto');
        });
    });

    const formBloqueio = document.getElementById('formBloqueio');
    document.querySelectorAll('.btn-abrir-bloqueio').forEach(function (botao) {
        botao.addEventListener('click', function () {
            formBloqueio.classList.add('aberto');
            formBloqueio.scrollIntoView({ behavior: 'smooth' });
        });
    });

    document.querySelectorAll('.btn-fechar-bloqueio').forEach(function (botao) {
        botao.addEventListener('click', function () {
            formBloqueio.classList.remove('aberto');
        });
    });
})();
