(function () {
// form-anuncio.js — Preview de fotos no formulário de novo anúncio
(function () {
    'use strict';

    document.addEventListener('DOMContentLoaded', function () {
        const inputFotos = document.getElementById('input-fotos');
        const preview = document.getElementById('fotos-preview');
        if (!inputFotos || !preview) return;

        inputFotos.addEventListener('change', function () {
            preview.innerHTML = '';
            Array.from(inputFotos.files).forEach(function (file) {
                if (!file.type.startsWith('image/')) return;
                const reader = new FileReader();
                reader.onload = function (e) {
                    const img = document.createElement('img');
                    img.src = e.target.result;
                    img.alt = file.name;
                    preview.appendChild(img);
                };
                reader.readAsDataURL(file);
            });
        });
    });
})();

})();
