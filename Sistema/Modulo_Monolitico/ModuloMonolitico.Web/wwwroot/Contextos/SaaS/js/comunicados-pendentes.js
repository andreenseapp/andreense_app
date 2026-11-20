(function () {
    'use strict';

    function inicializarModalComunicado(container) {
        if (!container || container.dataset.bound === 'true') return;
        container.dataset.bound = 'true';

        var form = container.querySelector('.saas-comunicado-form');
        if (!form) return;

        form.addEventListener('submit', function (e) {
            e.preventDefault();

            var submitter = e.submitter;
            var formData = new FormData(form);
            if (submitter && submitter.name) {
                formData.set(submitter.name, submitter.value);
            }

            var buttons = form.querySelectorAll('button[type="submit"]');
            buttons.forEach(function (botao) { botao.disabled = true; });

            window.requisitarApp(form.action, {
                method: 'POST',
                body: formData,
                headers: { 'X-Requested-With': 'XMLHttpRequest' },
                credentials: 'same-origin'
            })
                .then(function (response) {
                    if (response.status === 204) {
                        container.remove();
                        return;
                    }
                    buttons.forEach(function (botao) { botao.disabled = false; });
                })
                .catch(function () {
                    buttons.forEach(function (botao) { botao.disabled = false; });
                });
        });
    }

    function carregarComunicadosPendentes(loader) {
        if (!loader) return;

        var url = loader.getAttribute('data-url');
        if (!url) return;

        window.requisitarApp(url, {
            headers: { 'X-Requested-With': 'XMLHttpRequest' },
            credentials: 'same-origin'
        })
            .then(function (response) {
                if (response.status === 204) {
                    loader.remove();
                    return null;
                }
                if (!response.ok) {
                    loader.remove();
                    return null;
                }
                return response.text();
            })
            .then(function (html) {
                if (!html) return;
                loader.outerHTML = html;
                inicializarModalComunicado(document.getElementById('saas-modal-container'));
            })
            .catch(function () {
                loader.remove();
            });
    }

    document.addEventListener('DOMContentLoaded', function () {
        setTimeout(function () {
            carregarComunicadosPendentes(document.getElementById('saas-comunicados-loader'));
        }, 1000);
    });
})();
