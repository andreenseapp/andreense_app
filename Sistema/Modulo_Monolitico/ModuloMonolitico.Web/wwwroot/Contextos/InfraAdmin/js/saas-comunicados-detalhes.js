(function () {
document.documentElement.setAttribute('data-tema', localStorage.getItem('tema') || 'light');

document.querySelectorAll('.opcao-progress-fill[data-percent]').forEach(function (el) {
  var percent = parseInt(el.getAttribute('data-percent') || '0', 10);
  if (Number.isNaN(percent)) percent = 0;
  el.style.width = Math.min(100, Math.max(0, percent)) + '%';
});

})();
