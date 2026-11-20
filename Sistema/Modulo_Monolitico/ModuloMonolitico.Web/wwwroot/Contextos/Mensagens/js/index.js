(function () {
  const messagesArea = document.getElementById('messages-area');
  if (messagesArea) {
    messagesArea.scrollTop = messagesArea.scrollHeight;
  }

  const composeInput = document.getElementById('compose-input');
  if (composeInput) {
    composeInput.addEventListener('input', () => {
      composeInput.style.height = 'auto';
      composeInput.style.height = Math.min(composeInput.scrollHeight, 150) + 'px';
    });

    composeInput.addEventListener('keydown', (e) => {
      if ((e.ctrlKey || e.metaKey) && e.key === 'Enter') {
        e.preventDefault();
        document.getElementById('form-compose').submit();
      }
    });
  }

  const inputBusca = document.getElementById('busca-topicos');
  if (inputBusca) {
    inputBusca.addEventListener('input', () => {
      const q = inputBusca.value.toLowerCase();
      document.querySelectorAll('.topico-item').forEach(item => {
        const nome = item.querySelector('.topico-nome')?.textContent.toLowerCase() ?? '';
        const preview = item.querySelector('.topico-preview')?.textContent.toLowerCase() ?? '';
        item.style.display = (nome.includes(q) || preview.includes(q)) ? '' : 'none';
      });
    });
  }


})();
