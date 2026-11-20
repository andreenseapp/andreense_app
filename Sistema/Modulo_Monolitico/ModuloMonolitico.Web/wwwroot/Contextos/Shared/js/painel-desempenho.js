(function () {
  'use strict';

  var painel = document.querySelector('[data-painel-desempenho]');
  if (!painel) return;

  var botao = painel.querySelector('[data-alternar-painel-desempenho]');
  var conteudo = painel.querySelector('.painel-desempenho-conteudo');
  if (!botao || !conteudo) return;

  botao.addEventListener('click', function () {
    var aberto = !conteudo.hidden;
    conteudo.hidden = aberto;
    botao.setAttribute('aria-expanded', aberto ? 'false' : 'true');
  });
})();
