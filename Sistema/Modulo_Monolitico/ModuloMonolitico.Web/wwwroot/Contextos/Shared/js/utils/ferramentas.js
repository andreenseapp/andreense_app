(function () {
    'use strict';

    window.AppUtils = window.AppUtils || {};

    window.AppUtils.Ferramentas = {
        /**
         * Remove todas as letras e caracteres especiais, deixando apenas números
         */
        apenasNumeros: function (valor) {
            if (!valor) return '';
            return String(valor).replace(/\D/g, '');
        },

        /**
         * Formata um valor numérico longo para notação abreviada (ex: 1.5M, 300K)
         */
        formatarValorCurto: function (valor) {
            if (valor == null) return '';
            var num = typeof valor === 'string' ? parseFloat(valor.replace(',', '.')) : parseFloat(valor);
            if (isNaN(num)) return '';

            if (num >= 1000000) {
                return (num / 1000000).toFixed(1).replace(/\.0$/, '') + 'M';
            }
            if (num >= 1000) {
                return (num / 1000).toFixed(1).replace(/\.0$/, '') + 'K';
            }
            return num.toString();
        },

        /**
         * Abre um modal genérico pelo ID
         */
        abrirModal: function (idModal) {
            var modal = document.getElementById(idModal);
            if (modal) {
                modal.classList.add('aberto');
                modal.setAttribute('aria-hidden', 'false');
            }
        },

        /**
         * Fecha um modal genérico pelo ID
         */
        fecharModal: function (idModal) {
            var modal = document.getElementById(idModal);
            if (modal) {
                modal.classList.remove('aberto');
                modal.setAttribute('aria-hidden', 'true');
            }
        }
    };
})();
