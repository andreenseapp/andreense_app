(function () {
document.addEventListener('DOMContentLoaded', function () {
    var filtrosComAutoSubmit = document.querySelectorAll('[data-auto-submit]');
    filtrosComAutoSubmit.forEach(function (el) {
        el.addEventListener('change', function () {
            document.getElementById('form-filtros').submit();
        });
    });
});

})();
