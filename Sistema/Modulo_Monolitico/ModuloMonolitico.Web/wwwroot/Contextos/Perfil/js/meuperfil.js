(function () {
function iniciarEnvioFoto() {
  const form = document.getElementById('formulario-envio-foto');
  const input = document.getElementById('input-foto');
  if (input.files && input.files[0]) {
    if (typeof window.exibirCarregamento === 'function') {
      window.exibirCarregamento('Enviando foto para a nuvem...');
    }
    const container = document.getElementById('avatar-container');
    if (container) {
      container.classList.add('enviando');
    }
    form.submit();
  }
}

function abrirModalSenha() {
  const modal = document.getElementById('modal-senha');
  if (!modal) return;
  modal.classList.add('modal-senha--aberto');
  if (typeof inicializarApp === 'function') {
    inicializarApp(modal);
  }
}

function fecharModalSenha() {
  const modal = document.getElementById('modal-senha');
  if (!modal) return;
  modal.classList.remove('modal-senha--aberto');
}

document.addEventListener('DOMContentLoaded', () => {
  const avatarContainer = document.getElementById('avatar-container');
  const inputFoto = document.getElementById('input-foto');
  const btnAbrirSenha = document.getElementById('botao-abrir-senha');
  const btnFecharSenha = document.querySelectorAll('.botao-fechar-senha');

  if (avatarContainer && inputFoto) {
    avatarContainer.addEventListener('click', () => inputFoto.click());
  }

  if (inputFoto) {
    inputFoto.addEventListener('change', iniciarEnvioFoto);
  }

  if (btnAbrirSenha) {
    btnAbrirSenha.addEventListener('click', abrirModalSenha);
  }

  btnFecharSenha.forEach((botao) => {
    botao.addEventListener('click', fecharModalSenha);
  });

  const carreiraCard = document.getElementById('carreira');
  if (!carreiraCard) return;

  carreiraCard.addEventListener('submit', async (e) => {
    const form = e.target;
    if (form.method.toLowerCase() !== 'post') return;

    e.preventDefault();

    if (typeof window.exibirCarregamento === 'function') {
      window.exibirCarregamento('Processando...');
    }

    try {
      const response = await window.requisitarApp(form.action || window.location.href, {
        method: 'POST',
        body: new FormData(form)
      });

      if (response.ok) {
        const html = await response.text();
        const parser = new DOMParser();
        const doc = parser.parseFromString(html, 'text/html');
        const newCarreiraCard = doc.getElementById('carreira');

        if (newCarreiraCard) {
          carreiraCard.innerHTML = newCarreiraCard.innerHTML;

          if (typeof inicializarApp === 'function') {
            inicializarApp(carreiraCard);
          }

          const input = carreiraCard.querySelector('[name="NovaProfissao"]');
          if (input) {
            input.focus();
          }
        }

        const scripts = doc.querySelectorAll('script');
        scripts.forEach((script) => {
          if (script.textContent.includes('window.exibirModal')) {
            const newScript = document.createElement('script');
            newScript.textContent = script.textContent;
            document.body.appendChild(newScript);
            setTimeout(() => newScript.remove(), 100);
          }
        });
      } else if (typeof window.exibirModal === 'function') {
        window.exibirModal('Erro', 'Erro ao processar a requisi��o no servidor.', 'error');
      }
    } catch (err) {
      console.error('Erro AJAX:', err);
      if (typeof window.exibirModal === 'function') {
        window.exibirModal('Erro', 'Erro de conex�o com o servidor.', 'error');
      }
    } finally {
      if (typeof window.ocultarCarregamento === 'function') {
        window.ocultarCarregamento();
      }
    }
  });
});

})();
