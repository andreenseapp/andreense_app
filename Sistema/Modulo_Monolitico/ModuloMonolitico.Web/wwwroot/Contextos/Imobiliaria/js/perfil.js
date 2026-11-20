(function () {
  'use strict';

  var formulario = document.getElementById('form-perfil-anunciante');
  if (!formulario) return;

  var campoLogo = document.getElementById('Perfil_UrlFotoLogo');
  var previa = document.getElementById('perfil-logo-previa');
  if (!campoLogo || !previa) return;

  function atualizarPrevia() {
    var url = (campoLogo.value || '').trim();
    previa.innerHTML = '';

    if (!url) {
      previa.innerHTML = '<svg width="36" height="36" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5" aria-hidden="true"><path d="M20 21v-2a4 4 0 0 0-4-4H8a4 4 0 0 0-4 4v2"></path><circle cx="12" cy="7" r="4"></circle></svg>';
      return;
    }

    var imagem = document.createElement('img');
    imagem.alt = 'Prévia do logo';
    imagem.loading = 'lazy';
    imagem.src = url;
    imagem.addEventListener('error', function () {
      previa.innerHTML = '<svg width="36" height="36" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5" aria-hidden="true"><path d="M20 21v-2a4 4 0 0 0-4-4H8a4 4 0 0 0-4 4v2"></path><circle cx="12" cy="7" r="4"></circle></svg>';
    });
    previa.appendChild(imagem);
  }

  campoLogo.addEventListener('input', atualizarPrevia);
  campoLogo.addEventListener('change', atualizarPrevia);
  atualizarPrevia();
})();
