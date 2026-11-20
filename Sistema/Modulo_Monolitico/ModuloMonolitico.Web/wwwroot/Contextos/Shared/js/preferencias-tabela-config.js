(function () {
    'use strict';
    var elemento = document.getElementById('layout-modulo-config');
    if (!elemento) return;

    var preferencias = elemento.getAttribute('data-preferencias-tabela');
    if (preferencias) window.PreferenciasTabelaJson = preferencias;

    var identificador = elemento.getAttribute('data-tela-identificador');
    if (identificador) window.IdentificadorTela = identificador;
})();
