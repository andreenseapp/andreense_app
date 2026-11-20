(function () {
    'use strict';

    function alternarAba(tab) {
        document.querySelectorAll('.form-tab-btn').forEach(function (botao) {
            var isActive = botao.getAttribute('data-aba') === tab;
            botao.classList.toggle('active', isActive);
            botao.setAttribute('aria-selected', isActive ? 'true' : 'false');
        });

        document.querySelectorAll('.form-section-fields').forEach(function (sec) {
            sec.classList.remove('active');
        });

        var tabEl = document.getElementById('tab-' + tab);
        if (tabEl) tabEl.classList.add('active');
    }

    function formatarCep(value) {
        var digits = value.replace(/\D/g, '').slice(0, 8);
        if (digits.length > 5) return digits.slice(0, 5) + '-' + digits.slice(5);
        return digits;
    }

    function vincularPrevia(inputId, previewId, transformar) {
        var input = document.getElementById(inputId);
        var preview = document.getElementById(previewId);
        if (!input || !preview) return;

        function atualizar() {
            preview.textContent = transformar ? transformar(input.value) : input.value;
        }

        input.addEventListener('input', atualizar);
        atualizar();
    }

    function inicializarFallbackLogoCorreios() {
        var img = document.getElementById('correios-img-logo');
        var svg = document.getElementById('correios-svg-logo');
        if (!img || !svg) return;

        img.addEventListener('error', function () {
            img.hidden = true;
            svg.hidden = false;
        });

        if (img.complete && img.naturalWidth === 0) {
            img.hidden = true;
            svg.hidden = false;
        }
    }

    document.addEventListener('DOMContentLoaded', function () {
        document.querySelectorAll('[data-aba]').forEach(function (botao) {
            botao.addEventListener('click', function () {
                alternarAba(botao.getAttribute('data-aba'));
            });
        });

        document.querySelectorAll('[data-action="imprimir"]').forEach(function (botao) {
            botao.addEventListener('click', function () {
                window.print();
            });
        });

        vincularPrevia('dest-nome', 'preview-dest-nome');
        vincularPrevia('dest-endereco', 'preview-dest-endereco');
        vincularPrevia('dest-cidade', 'preview-dest-cidade');
        vincularPrevia('rem-nome', 'preview-rem-nome');
        vincularPrevia('rem-endereco', 'preview-rem-endereco');
        vincularPrevia('rem-cidade', 'preview-rem-cidade');

        var destCep = document.getElementById('dest-cep');
        var remCep = document.getElementById('rem-cep');
        var previewDestCep = document.getElementById('preview-dest-cep-text');
        var previewRemCep = document.getElementById('preview-rem-cep');

        if (destCep) {
            destCep.addEventListener('input', function () {
                this.value = formatarCep(this.value);
                if (previewDestCep) previewDestCep.textContent = 'CEP ' + this.value;
            });
        }

        if (remCep) {
            remCep.addEventListener('input', function () {
                this.value = formatarCep(this.value);
                if (previewRemCep) previewRemCep.textContent = 'CEP ' + this.value;
            });
        }

        inicializarFallbackLogoCorreios();
    });
})();
