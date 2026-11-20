(function () {
  'use strict';

  var raiz = document.querySelector('[data-url-alternar-destaque]');
  if (!raiz || raiz.getAttribute('data-usuario-master') !== 'true') return;

  var url = raiz.getAttribute('data-url-alternar-destaque');
  if (!url) return;

  document.querySelectorAll('[data-destaque-btn]').forEach(function (btn) {
    btn.addEventListener('click', async function (e) {
      e.preventDefault();
      e.stopPropagation();

      var anuncioId = btn.getAttribute('data-anuncio-id');
      if (!anuncioId) return;

      var estavaAtivo = btn.classList.contains('ativo');
      btn.disabled = true;
      btn.classList.toggle('ativo');

      try {
        var response = await window.requisitarApp(url, {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json',
            'Accept': 'application/json'
          },
          credentials: 'same-origin',
          body: JSON.stringify({ anuncioId: anuncioId })
        });

        if (response.status === 401 || response.status === 403) {
          btn.classList.toggle('ativo', estavaAtivo);
          if (window.NotificarErro) {
            window.NotificarErro('Sem permissão para alterar destaque.');
          }
          return;
        }

        if (!response.ok) throw new Error('erro');

        var data = await response.json();
        if (data.sucesso === false) {
          throw new Error(data.mensagem || 'erro');
        }

        var ativo = !!data.ehDestaque;
        btn.classList.toggle('ativo', ativo);
        var svg = btn.querySelector('svg');
        if (svg) svg.setAttribute('fill', ativo ? 'currentColor' : 'none');

        var card = btn.closest('.anuncio-card');
        var foto = card ? card.querySelector('.anuncio-card-foto') : null;
        var selo = foto ? foto.querySelector('[data-selo-destaque]') : null;

        if (ativo && foto && !selo) {
          var novo = document.createElement('span');
          novo.className = 'anuncio-badge badge--destaque';
          novo.setAttribute('data-selo-destaque', '');
          novo.textContent = 'Destaque';
          foto.appendChild(novo);
        } else if (!ativo && selo) {
          selo.remove();
        }

        if (window.NotificarSucesso && data.mensagem) {
          window.NotificarSucesso(data.mensagem);
        }
      } catch (err) {
        btn.classList.toggle('ativo', estavaAtivo);
        if (window.NotificarErro) {
          window.NotificarErro('Não foi possível alterar o destaque.');
        }
      } finally {
        btn.disabled = false;
      }
    });
  });
})();
