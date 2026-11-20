(function () {
    'use strict';

    function alternarModal(modal, exibir) {
        if (!modal) return;

        if (exibir) {
            modal.classList.add('open');
            document.body.style.overflow = 'hidden';
        } else {
            modal.classList.remove('open');
            document.body.style.overflow = '';
        }
    }

    document.addEventListener('DOMContentLoaded', function () {
        document.querySelectorAll('[data-modal-open]').forEach(function (botao) {
            botao.addEventListener('click', function () {
                var id = botao.getAttribute('data-modal-open');
                var modal = id ? document.getElementById(id) : document.querySelector('[data-modal]');
                alternarModal(modal, true);
            });
        });

        document.querySelectorAll('[data-modal-close]').forEach(function (botao) {
            botao.addEventListener('click', function () {
                var modal = botao.closest('[data-modal]') || botao.closest('.overlay');
                alternarModal(modal, false);
            });
        });

        document.querySelectorAll('[data-modal]').forEach(function (modal) {
            modal.addEventListener('click', function (e) {
                if (e.target === modal) alternarModal(modal, false);
            });
        });

        document.addEventListener('keydown', function (e) {
            if (e.key !== 'Escape') return;
            document.querySelectorAll('.overlay.open').forEach(function (modal) {
                alternarModal(modal, false);
            });
        });

        var formPublicar = document.getElementById('form-publicar');
        if (formPublicar) {
            formPublicar.addEventListener('submit', function (e) {
                e.preventDefault();
                var btnSubmit = formPublicar.querySelector('button[type="submit"]');
                var txtOriginal = btnSubmit.innerHTML;
                btnSubmit.innerHTML = 'Publicando...';
                btnSubmit.disabled = true;

                var formData = new FormData(formPublicar);
                var action = formPublicar.getAttribute('action'); // The form action (which is the current URL + ?handler=Publicar)
                
                
                window.requisitarApp(action || window.location.href + '?handler=Publicar', {
                    method: 'POST',
                    body: formData
                })
                .then(function (response) {
                    if (response.ok) {
                        return response.text().then(function (html) {
                            var grid = document.getElementById('achados-grid');
                            if (grid) {
                                grid.insertAdjacentHTML('afterbegin', html);
                            }
                            alternarModal(document.getElementById('publishModal'), false);
                            formPublicar.reset();
                            if (window.exibirModal) window.exibirModal('Sucesso', 'Publicação realizada com sucesso!', 'success');
                        });
                    } else {
                        return response.json().then(function (json) {
                            if (window.exibirModal) window.exibirModal('Erro', json.mensagem || 'Erro ao publicar.', 'error');
                            else alerta(json.mensagem || 'Erro ao publicar.');
                        });
                    }
                })
                .catch(function (error) {
                    if (window.exibirModal) window.exibirModal('Erro', 'Erro de conexão.', 'error');
                })
                .finally(function () {
                    btnSubmit.innerHTML = txtOriginal;
                    btnSubmit.disabled = false;
                });
            });
        }
    });
})();
