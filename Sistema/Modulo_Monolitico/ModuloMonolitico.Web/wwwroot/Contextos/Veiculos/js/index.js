(function () {
// index.js — Vitrine pública de veículos
(function () {
    'use strict';

    document.addEventListener('DOMContentLoaded', function () {
        const portal = document.getElementById('form-filtros');
        if (!portal) return;

        const usuarioLogado = portal.dataset.usuarioLogado === 'true';
        const urlFavoritar = portal.dataset.urlFavoritar;
        const urlLogin = portal.dataset.urlLogin;
        const urlAlternarDestaque = portal.dataset.urlAlternarDestaque;
        const usuarioMaster = portal.dataset.usuarioMaster === 'true';

        document.querySelectorAll('.btn-favoritar').forEach(function (btn) {
            btn.addEventListener('click', async function () {
                if (!usuarioLogado) {
                    window.location.href = urlLogin;
                    return;
                }
                const id = btn.dataset.id;
                try {
                    const resp = await window.requisitarApp(urlFavoritar, {
                        method: 'POST',
                        headers: { 'Content-Type': 'application/json', 'RequestVerificationToken': obterToken() },
                        body: JSON.stringify({ anuncioVeiculoId: id })
                    });
                    if (resp.ok) {
                        btn.classList.toggle('favoritado');
                    }
                } catch (e) {
                    console.error('Erro ao favoritar', e);
                }
            });
        });

        if (usuarioMaster) {
            document.querySelectorAll('.btn-destaque').forEach(function (btn) {
                btn.addEventListener('click', async function () {
                    const id = btn.dataset.id;
                    try {
                        const resp = await window.requisitarApp(urlAlternarDestaque, {
                            method: 'POST',
                            headers: { 'Content-Type': 'application/json', 'RequestVerificationToken': obterToken() },
                            body: JSON.stringify({ anuncioVeiculoId: id })
                        });
                        if (resp.ok) location.reload();
                    } catch (e) {
                        console.error('Erro ao alternar destaque', e);
                    }
                });
            });
        }
    });

    function obterToken() {
        const el = document.querySelector('input[name="__RequestVerificationToken"]');
        return el ? el.value : '';
    }
})();

})();
