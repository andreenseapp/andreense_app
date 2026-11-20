(function () {
  document.addEventListener('DOMContentLoaded', function () {
    var sel = document.getElementById('seletorEmpresa');
    if (!sel) return;

    sel.addEventListener('change', function () {
      var base = sel.getAttribute('data-url-listagem') || window.location.pathname;
      var sep = base.indexOf('?') >= 0 ? '&' : '?';
      window.location.href = base + sep + 'empresaId=' + encodeURIComponent(sel.value);
    });
  });
})();
