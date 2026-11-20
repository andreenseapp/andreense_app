(function () {
    'use strict';

    document.addEventListener('DOMContentLoaded', function () {
        var input = document.querySelector('.validar-email-otp input[name="Codigo"]');
        if (!input) return;

        input.addEventListener('input', function () {
            this.value = this.value.replace(/\D/g, '').slice(0, 6);
        });

        input.addEventListener('paste', function (e) {
            e.preventDefault();
            var pasted = (e.clipboardData || window.clipboardData).getData('text');
            this.value = pasted.replace(/\D/g, '').slice(0, 6);
        });
    });
})();
