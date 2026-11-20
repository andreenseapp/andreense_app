(function () {
  'use strict';

  var tema = localStorage.getItem('tema') || 'light';
  document.documentElement.setAttribute('data-tema', tema);
})();
