(function () {
// detalhes.js — Galeria da página de detalhe do veículo
(function () {
    'use strict';

    document.addEventListener('DOMContentLoaded', function () {
        const fotoPrincipal = document.getElementById('foto-principal');
        if (!fotoPrincipal) return;

        document.querySelectorAll('.galeria-miniatura').forEach(function (miniatura) {
            miniatura.addEventListener('click', function () {
                fotoPrincipal.src = miniatura.src;
                document.querySelectorAll('.galeria-miniatura').forEach(m => m.classList.remove('ativa'));
                miniatura.classList.add('ativa');
            });
        });

        const primeiraMiniatura = document.querySelector('.galeria-miniatura');
        if (primeiraMiniatura) primeiraMiniatura.classList.add('ativa');
    });
})();

})();
