
function inicializarTema() {
  const botaoAlternar = document.getElementById('alternar-tema');
  if (!botaoAlternar) return;

  const temaAtual = localStorage.getItem('tema') || 'light';
  document.documentElement.setAttribute('data-tema', temaAtual);

  botaoAlternar.addEventListener('click', () => {
    const ehEscuro = document.documentElement.getAttribute('data-tema') === 'dark';
    const novoTema = ehEscuro ? 'light' : 'dark';
    document.documentElement.setAttribute('data-tema', novoTema);
    localStorage.setItem('tema', novoTema);
  });
}

inicializarTema();

window._appSessaoExpiradaEmAndamento = false;

const _APP_MSG_SESSAO_EXPIRADA = 'Sua sessão expirou. Faça login novamente.';
const _APP_URL_LOGIN_EXPIRADO = '/Contextos/Autenticacao/Login?exp=1';

function _appCriarErroSessao(mensagem) {
  const erro = new Error(mensagem || _APP_MSG_SESSAO_EXPIRADA);
  erro.codigo = 'SESSAO_EXPIRADA';
  return erro;
}

window.tratarSessaoExpirada = function(mensagem) {
  const msg = mensagem || _APP_MSG_SESSAO_EXPIRADA;
  const irParaLogin = () => { window.location.href = _APP_URL_LOGIN_EXPIRADO; };

  if (!window._appSessaoExpiradaEmAndamento) {
    window._appSessaoExpiradaEmAndamento = true;
    if (typeof window.exibirModalObrigatorio === 'function') {
      window.exibirModalObrigatorio('Sessão expirada', msg, 'warning', irParaLogin);
    } else {
      irParaLogin();
    }
  }

  throw _appCriarErroSessao(msg);
};

window.alerta = function(mensagem, tipo) {
  if (typeof window.exibirToast === 'function') {
    window.exibirToast(mensagem, tipo || 'error');
    return;
  }
  if (typeof window.exibirModal === 'function') {
    window.exibirModal('Aviso', String(mensagem ?? ''), tipo || 'error');
  }
};

window.requisitarApp = async function(url, options = {}) {
  options.headers = options.headers || {};
  options.headers['X-Requested-With'] = 'XMLHttpRequest';

  try {
    const response = await fetch(url, options);

    if (response.status === 401 || response.status === 403) {
      let mensagem = _APP_MSG_SESSAO_EXPIRADA;
      try {
        const data = await response.clone().json();
        mensagem = data.mensagem || data.erro || mensagem;
      } catch (_) {}
      window.tratarSessaoExpirada(mensagem);
    }

    return response;
  } catch (err) {
    if (err && err.codigo === 'SESSAO_EXPIRADA') throw err;
    if (err && err.message && err.message.includes('Sessão expirada')) throw err;
    console.error(`Erro fatal no requisitarApp para ${url}:`, err);
    throw err;
  }
};

function inicializarApp(container = document) {
  window.exibirCarregamento = function(text = "Carregando...", type = "loading") {
    const overlay = document.getElementById('carregamento-global-sobreposicao');
    const elTexto = document.getElementById('carregamento-global-texto');
    const containerIcone = document.getElementById('carregamento-global-icone');
    
    if (overlay) {
      if (elTexto) elTexto.innerText = text;
      
      if (containerIcone) {
        let iconHtml = '';
        switch(type) {
          case 'sql':
            iconHtml = `
              <div class="carregamento-sql-pulso">
                <svg width="64" height="64" viewBox="0 0 24 24" fill="none" stroke="var(--clr-primary, #3b82f6)" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round" class="pulse-svg">
                  <ellipse cx="12" cy="5" rx="9" ry="3"></ellipse>
                  <path d="M3 5v14c0 1.66 4 3 9 3s9-1.34 9-3V5"></path>
                  <path d="M3 12c0 1.66 4 3 9 3s9-1.34 9-3"></path>
                </svg>
              </div>`;
            break;
          case 'layout':
            iconHtml = `
              <div class="esqueleto-layout">
                <div class="barra-esqueleto title"></div>
                <div class="barra-esqueleto body1"></div>
                <div class="barra-esqueleto body2"></div>
              </div>`;
            break;
          case 'processing':
            iconHtml = `
              <div class="engrenagens-processando">
                <svg width="48" height="48" viewBox="0 0 24 24" fill="none" stroke="var(--clr-primary, #10b981)" stroke-width="1.5" class="engrenagem-giro">
                  <circle cx="12" cy="12" r="3"></circle>
                  <path d="M19.4 15a1.65 1.65 0 0 0 .33 1.82l.06.06a2 2 0 1 1-2.83 2.83l-.06-.06a1.65 1.65 0 0 0-1.82-.33 1.65 1.65 0 0 0-1 1.51V21a2 2 0 0 1-4 0v-.09A1.65 1.65 0 0 0 9 19.4a1.65 1.65 0 0 0-1.82.33l-.06.06a2 2 0 1 1-2.83-2.83l.06-.06a1.65 1.65 0 0 0 .33-1.82 1.65 1.65 0 0 0-1.51-1H3a2 2 0 0 1 0-4h.09A1.65 1.65 0 0 0 4.6 9a1.65 1.65 0 0 0-.33-1.82l-.06-.06a2 2 0 1 1 2.83-2.83l.06.06a1.65 1.65 0 0 0 1.82.33H9a1.65 1.65 0 0 0 1-1.51V3a2 2 0 0 1 4 0v.09a1.65 1.65 0 0 0 1 1.51 1.65 1.65 0 0 0 1.82-.33l.06-.06a2 2 0 1 1 2.83 2.83l-.06.06a1.65 1.65 0 0 0-.33 1.82V9a1.65 1.65 0 0 0 1.51 1H21a2 2 0 0 1 0 4h-.09a1.65 1.65 0 0 0-1.51 1z"></path>
                </svg>
                <svg width="32" height="32" viewBox="0 0 24 24" fill="none" stroke="var(--clr-accent, #fbbc04)" stroke-width="1.5" class="cog-spin-rev" style="margin-left: -10px; margin-top: 20px;">
                  <circle cx="12" cy="12" r="3"></circle>
                  <path d="M19.4 15a1.65 1.65 0 0 0 .33 1.82l.06.06a2 2 0 1 1-2.83 2.83l-.06-.06a1.65 1.65 0 0 0-1.82-.33 1.65 1.65 0 0 0-1 1.51V21a2 2 0 0 1-4 0v-.09A1.65 1.65 0 0 0 9 19.4a1.65 1.65 0 0 0-1.82.33l-.06.06a2 2 0 1 1-2.83-2.83l.06-.06a1.65 1.65 0 0 0 .33-1.82 1.65 1.65 0 0 0-1.51-1H3a2 2 0 0 1 0-4h.09A1.65 1.65 0 0 0 4.6 9a1.65 1.65 0 0 0-.33-1.82l-.06-.06a2 2 0 1 1 2.83-2.83l.06.06a1.65 1.65 0 0 0 1.82.33H9a1.65 1.65 0 0 0 1-1.51V3a2 2 0 0 1 4 0v.09a1.65 1.65 0 0 0 1 1.51 1.65 1.65 0 0 0 1.82-.33l.06-.06a2 2 0 1 1 2.83 2.83l-.06.06a1.65 1.65 0 0 0-.33 1.82V9a1.65 1.65 0 0 0 1.51 1H21a2 2 0 0 1 0 4h-.09a1.65 1.65 0 0 0-1.51 1z"></path>
                </svg>
              </div>`;
            break;
          case 'calculation':
            iconHtml = `
              <div class="carregamento-calculo">
                <div class="calc-box">
                  <span class="calc-symbol font-heading">+</span>
                  <span class="calc-symbol font-heading">-</span>
                  <span class="calc-symbol font-heading">?</span>
                  <span class="calc-symbol font-heading">?</span>
                </div>
              </div>`;
            break;
          default:
            iconHtml = `<div class="carregamento-spinner"></div>`;
        }
        containerIcone.innerHTML = iconHtml;
      }
      
      overlay.classList.add('active');
    }
  };

  window.ocultarCarregamento = function() {
    const overlay = document.getElementById('carregamento-global-sobreposicao');
    if (overlay) overlay.classList.remove('active');
  };

  container.querySelectorAll('form:not([data-inicializado])').forEach(form => {
    form.setAttribute('data-inicializado', 'true');
    form.addEventListener('submit', (e) => {
      if (form.hasAttribute('data-sem-carregamento')) return;

      const botao = form.querySelector('button[type=submit]');
      if (botao) {
        botao.disabled = true;
        botao.setAttribute('aria-busy', 'true');
      }
      
      window.exibirCarregamento("Processando...");
    });
    form.addEventListener('htmx:afterRequest', () => {
      window.ocultarCarregamento();
      const botao = form.querySelector('button[type=submit]');
      if (botao) {
        botao.disabled = false;
        botao.removeAttribute('aria-busy');
      }
    });
  });

  inicializarValidacaoAntecipada(container);


  container.querySelectorAll('[data-alternar-senha]:not([data-inicializado])').forEach(botao => {
    botao.setAttribute('data-inicializado', 'true');
    botao.addEventListener('click', () => {
      const idAlvo = botao.getAttribute('data-alternar-senha');
      const input = document.getElementById(idAlvo);
      if (!input) return;
      const ehSenha = input.type === 'password';
      input.type = ehSenha ? 'text' : 'password';
      botao.querySelector('.icon-eye-open')?.classList.toggle('hidden', !ehSenha);
      botao.querySelector('.icon-eye-closed')?.classList.toggle('hidden', ehSenha);
    });
  });

  container.querySelectorAll('input[type=password]').forEach(input => {
    const containerForca = document.getElementById(`strength-container-${input.id}`);
    if (!containerForca) return;

    input.addEventListener('focus', () => {
      containerForca.style.display = 'block';
    });

    const checkIcon = `<svg width="12" height="12" viewBox="0 0 24 24" fill="none" stroke="#81c995" stroke-width="3"><polyline points="20 6 9 17 4 12"></polyline></svg>`;
    const circleIcon = `<svg width="12" height="12" viewBox="0 0 24 24" fill="none" stroke="var(--clr-text-subtle)" stroke-width="3"><circle cx="12" cy="12" r="10"></circle></svg>`;

    const atualizarRequisito = (reqEl, meetsRequirement) => {
      if (!reqEl) return;
      const containerIcone = reqEl.querySelector('.req-icon');
      if (containerIcone) {
        containerIcone.innerHTML = meetsRequirement ? checkIcon : circleIcon;
      }
      if (meetsRequirement) {
        reqEl.style.color = 'var(--clr-text)';
      } else {
        reqEl.style.color = 'var(--clr-text-muted)';
      }
    };

    input.addEventListener('input', () => {
      const val = input.value;
      
      const atingeTamanho = val.length >= 8;
      const atingeMaiuscula = /[A-Z]/.test(val);
      const atingeMinuscula = /[a-z]/.test(val);
      const atingeNumero = /\d/.test(val);
      const atingeEspecial = /[@$!%*?&]/.test(val);

      atualizarRequisito(containerForca.querySelector(`#req-length-${input.id}`), atingeTamanho);
      atualizarRequisito(containerForca.querySelector(`#req-upper-${input.id}`), atingeMaiuscula);
      atualizarRequisito(containerForca.querySelector(`#req-lower-${input.id}`), atingeMinuscula);
      atualizarRequisito(containerForca.querySelector(`#req-number-${input.id}`), atingeNumero);
      atualizarRequisito(containerForca.querySelector(`#req-special-${input.id}`), atingeEspecial);

      let score = 0;
      if (atingeTamanho) score++;
      if (atingeMaiuscula) score++;
      if (atingeMinuscula) score++;
      if (atingeNumero) score++;
      if (atingeEspecial) score++;

      const barraProgresso = containerForca.querySelector(`#strength-bar-${input.id}`);
      const textoProgresso = containerForca.querySelector(`#strength-text-${input.id}`);

      if (barraProgresso && textoProgresso) {
        let pct = (score / 5) * 100;
        barraProgresso.style.width = `${pct}%`;

        if (val.length === 0) {
          barraProgresso.style.width = '0%';
          barraProgresso.style.backgroundColor = '#ea4335';
          textoProgresso.innerText = 'Fraca';
          textoProgresso.style.color = '#ea4335';
        } else if (score <= 2) {
          barraProgresso.style.backgroundColor = '#ea4335'; // vermelho
          textoProgresso.innerText = 'Fraca';
          textoProgresso.style.color = '#ea4335';
        } else if (score <= 4) {
          barraProgresso.style.backgroundColor = '#fbbc04'; // amarelo/laranja
          textoProgresso.innerText = 'Média';
          textoProgresso.style.color = '#fbbc04';
        } else {
          barraProgresso.style.backgroundColor = '#81c995'; // verde
          textoProgresso.innerText = 'Forte';
          textoProgresso.style.color = '#81c995';
        }
      }
    });
  });

  if (window.AppUtils && window.AppUtils.Mascaras) {
    window.AppUtils.Mascaras.bindAll(container);
  } else {
    console.warn("AppUtils.Mascaras não encontrado. Lembre-se de importar mascaras.js antes de app.js.");
  }

  container.querySelectorAll('[data-mascara=cep]:not([data-cep-bound])').forEach(input => {
    input.setAttribute('data-cep-bound', 'true');

    const _appNormalizarTextoCep = (valor) =>
      String(valor || '')
        .trim()
        .toLowerCase()
        .normalize('NFD')
        .replace(/[\u0300-\u036f]/g, '');

    const _appExibirAvisoCep = (mensagem) => {
      const grupo = input.closest('.campo-grupo');
      if (!grupo) {
        if (mensagem && typeof window.exibirToast === 'function') {
          window.exibirToast(mensagem, 'info');
        }
        return;
      }
      let span = grupo.querySelector('[data-cep-aviso]');
      if (!mensagem) {
        if (span) span.remove();
        return;
      }
      if (!span) {
        span = document.createElement('span');
        span.setAttribute('data-cep-aviso', 'true');
        span.className = 'campo-mensagem-ajuda';
        grupo.appendChild(span);
      }
      span.textContent = mensagem;
    };

    const buscarEndereco = async (forcar = false) => {
      const cepStr = input.value.replace(/\D/g, '');
      if (cepStr.length !== 8) return;

      const cepConsultado = input.getAttribute('data-cep-consultado') || '';
      if (!forcar && cepStr === cepConsultado) return;

      const separator = input.id.includes('_') ? '_' : '-';
      const parts = input.id.split(separator);
      const prefix = parts.slice(0, parts.length - 1).join(separator);

      const obterElemento = (suffix) => {
        return document.getElementById(`${prefix}${separator}${suffix.charAt(0).toUpperCase() + suffix.slice(1)}`) ||
               document.getElementById(`${prefix}${separator}${suffix.toLowerCase()}`);
      };

      const elLogradouro = obterElemento('logradouro');
      const elBairro     = obterElemento('bairro');
      const elCidade     = obterElemento('cidade');
      const elUf         = obterElemento('uf');
      const elNumero     = obterElemento('numero');

      if (!elLogradouro && !elBairro && !elCidade) return;

      const sobrescreverTudo = cepStr !== cepConsultado;
      input.classList.add('carregando-cep');
      input.style.opacity = '0.7';
      input.style.cursor = 'wait';

      try {
        const response = await window.requisitarApp(`/Api/Cep?cep=${cepStr}`);
        if (!response.ok) {
          _appExibirErroCampo(input, 'Não foi possível consultar o CEP. Tente novamente.');
          return;
        }

        const data = await response.json();

        if (data.encontrado) {
          const diferencas = [];
          const aplicarCampo = (el, valorApi, rotulo) => {
            if (!el) return;
            const novo = valorApi || '';
            const atual = (el.value || '').trim();
            if (sobrescreverTudo || !atual) {
              el.value = novo;
              return;
            }
            if (novo && _appNormalizarTextoCep(atual) !== _appNormalizarTextoCep(novo)) {
              diferencas.push(rotulo);
            }
          };

          aplicarCampo(elLogradouro, data.logradouro, 'logradouro');
          aplicarCampo(elBairro, data.bairro, 'bairro');
          aplicarCampo(elCidade, data.cidade, 'cidade');
          aplicarCampo(elUf, data.uf, 'UF');

          input.setAttribute('data-cep-consultado', cepStr);
          _appExibirErroCampo(input, '');

          if (diferencas.length > 0) {
            const lista = diferencas.length === 1
              ? diferencas[0]
              : diferencas.slice(0, -1).join(', ') + ' e ' + diferencas[diferencas.length - 1];
            _appExibirAvisoCep(`Consulta de CEP retornou ${lista} diferente do informado.`);
          } else {
            _appExibirAvisoCep('');
            if (elNumero) elNumero.focus();
          }
        } else {
          _appExibirAvisoCep('');
          _appExibirErroCampo(input, 'CEP não encontrado.');
        }
      } catch (err) {
        if (err && err.codigo === 'SESSAO_EXPIRADA') return;
        console.error('Erro ao buscar CEP:', err);
        _appExibirErroCampo(input, 'Não foi possível consultar o CEP. Tente novamente.');
      } finally {
        input.classList.remove('carregando-cep');
        input.style.opacity = '1';
        input.style.cursor = 'default';
      }
    };

    const dispararBusca = (e) => {
      const currentVal = e.target.value.replace(/\D/g, '');
      if (currentVal.length === 8) {
        buscarEndereco(false);
      }
    };
    input.addEventListener('focusout', dispararBusca);
    input.addEventListener('input', dispararBusca);

    const parentWrapper = input.closest('.input-icon-wrapper');
    if (parentWrapper) {
      const btnBuscar = parentWrapper.querySelector('.botao-buscar-cep');
      if (btnBuscar) {
        btnBuscar.addEventListener('click', (e) => {
          e.preventDefault();
          buscarEndereco(true);
        });
      }
    }
  });

  container.querySelectorAll('.papel-alternar:not([data-inicializado])').forEach(toggle => {
    toggle.setAttribute('data-inicializado', 'true');
    const atualizarCamposExtras = () => {
      const card = toggle.closest('.papel-card');
      const extra = card?.querySelector('.papel-extra');
      if (!extra) return;
      extra.style.display = toggle.checked ? 'block' : 'none';
    };
    toggle.addEventListener('change', atualizarCamposExtras);
    atualizarCamposExtras(); // estado inicial
  });

  container.querySelectorAll('[data-selecao-tabela]:not([data-selecao-bound])').forEach(table => {
    table.setAttribute('data-selecao-bound', 'true');

    const botaoAlternar = document.querySelector('[data-selecao-toggle]');
    const painel = document.querySelector('[data-selecao-painel]');
    const contador = document.querySelector('[data-selecao-contador]');
    const chkMestre = table.querySelector('[data-selecao-mestre]');
    
    if (!botaoAlternar) return;

    botaoAlternar.addEventListener('click', () => {
      table.classList.toggle('selecao-ativa');
      botaoAlternar.classList.toggle('ativo');
      botaoAlternar.classList.toggle('active');
      
      if (!table.classList.contains('selecao-ativa')) {
        if (chkMestre) chkMestre.checked = false;
        table.querySelectorAll('[data-selecao-item]').forEach(chk => chk.checked = false);
        if (painel) {
          painel.classList.remove('ativo');
          painel.classList.remove('active');
        }
      }
    });

    if (chkMestre) {
      chkMestre.addEventListener('change', () => {
        const items = table.querySelectorAll('[data-selecao-item]');
        items.forEach(chk => {
          chk.checked = chkMestre.checked;
        });
        atualizarPainel();
      });
    }

    table.querySelectorAll('[data-selecao-item]').forEach(chk => {
      chk.addEventListener('change', () => {
        atualizarPainel();
      });
    });

    function atualizarPainel() {
      const items = table.querySelectorAll('[data-selecao-item]');
      const selecionados = Array.from(items).filter(chk => chk.checked).length;

      if (contador) contador.innerText = selecionados;

      if (chkMestre) {
        chkMestre.checked = items.length > 0 && selecionados === items.length;
      }

      if (painel) {
        if (selecionados > 0) {
          painel.classList.add('active');
          painel.classList.add('ativo');
        } else {
          painel.classList.remove('active');
          painel.classList.remove('ativo');
        }
      }
    }
  });

  container.querySelectorAll('.aba-nav:not([data-scroll-bound])').forEach(nav => {
    nav.setAttribute('data-scroll-bound', 'true');
    
    const wrapper = document.createElement('div');
    wrapper.className = 'aba-nav-envoltorio';
    
    nav.parentNode.insertBefore(wrapper, nav);
    wrapper.appendChild(nav);

    const botaoEsquerda = document.createElement('button');
    botaoEsquerda.className = 'aba-nav-seta tab-nav-arrow-left';
    botaoEsquerda.innerHTML = '<svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5"><polyline points="15 18 9 12 15 6"></polyline></svg>';
    botaoEsquerda.type = 'button';
    
    const botaoDireita = document.createElement('button');
    botaoDireita.className = 'aba-nav-seta tab-nav-arrow-right';
    botaoDireita.innerHTML = '<svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5"><polyline points="9 18 15 12 9 6"></polyline></svg>';
    botaoDireita.type = 'button';

    wrapper.appendChild(botaoEsquerda);
    wrapper.appendChild(botaoDireita);

    const atualizarSetas = () => {
      if (nav.scrollWidth <= nav.clientWidth + 2) {
        botaoEsquerda.classList.remove('visible');
        botaoDireita.classList.remove('visible');
        return;
      }
      
      if (nav.scrollEsquerda > 5) {
        botaoEsquerda.classList.add('visible');
      } else {
        botaoEsquerda.classList.remove('visible');
      }

      if (Math.ceil(nav.scrollEsquerda + nav.clientWidth) >= nav.scrollWidth - 5) {
        botaoDireita.classList.remove('visible');
      } else {
        botaoDireita.classList.add('visible');
      }
    };

    botaoEsquerda.addEventListener('click', () => { nav.scrollBy({ left: -200, behavior: 'smooth' }); });
    botaoDireita.addEventListener('click', () => { nav.scrollBy({ left: 200, behavior: 'smooth' }); });

    nav.addEventListener('scroll', atualizarSetas);
    window.addEventListener('resize', atualizarSetas);
    
    const observer = new MutationObserver(atualizarSetas);
    observer.observe(nav, { childList: true, subtree: true, attributes: true });

    setTimeout(atualizarSetas, 100);

    nav.addEventListener('wheel', (e) => {
      if (e.deltaY !== 0) {
        e.preventDefault();
        nav.scrollEsquerda += e.deltaY * 1.5;
      }
    }, { passive: false });

    let pressionado = false;
    let inicioX;
    let scrollEsquerda;

    nav.addEventListener('mousedown', (e) => {
      pressionado = true;
      nav.style.cursor = 'grabbing';
      inicioX = e.pageX - nav.offsetLeft;
      scrollEsquerda = nav.scrollEsquerda;
    });

    nav.addEventListener('mouseleave', () => {
      pressionado = false;
      nav.style.cursor = 'default';
    });

    nav.addEventListener('mouseup', () => {
      pressionado = false;
      nav.style.cursor = 'default';
    });

    nav.addEventListener('mousemove', (e) => {
      if (!pressionado) return;
      e.preventDefault();
      const x = e.pageX - nav.offsetLeft;
      const walk = (x - inicioX) * 2;
      nav.scrollEsquerda = scrollEsquerda - walk;
    });
  });
}

function _appAnexarSvgEstatico(parent, svgHtml) {
  const wrap = document.createElement('div');
  wrap.innerHTML = svgHtml;
  if (wrap.firstElementChild) parent.appendChild(wrap.firstElementChild);
}

window.escaparHtml = function(s) {
  return String(s ?? '')
    .replace(/&/g, '&amp;')
    .replace(/</g, '&lt;')
    .replace(/>/g, '&gt;')
    .replace(/"/g, '&quot;')
    .replace(/'/g, '&#39;');
};

window.classeCssSegura = function(s) {
  return String(s ?? '').replace(/[^a-zA-Z0-9_-]/g, '');
};

window.corCssSegura = function(s) {
  const v = String(s ?? '').trim();
  if (/^#[0-9a-fA-F]{3,8}$/.test(v)) return v;
  if (/^rgba?\([^)]+\)$/.test(v)) return v;
  if (/^var\(--[\w-]+\)$/.test(v)) return v;
  return '#6366f1';
};

window.definirMensagemContainer = function(container, nomeClasse, texto, cor) {
  if (!container) return;
  container.className = nomeClasse || '';
  container.replaceChildren();
  const span = document.createElement('span');
  if (cor) span.style.color = cor;
  span.textContent = texto;
  container.appendChild(span);
};

window.exibirCarregamentoHorarios = function(container, mensagem) {
  if (!container) return;
  container.className = 'horarios-carregando';
  container.replaceChildren();
  const wrap = document.createElement('div');
  wrap.style.display = 'flex';
  wrap.style.alignItems = 'center';
  wrap.style.gap = '0.5rem';
  _appAnexarSvgEstatico(wrap, '<svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" style="animation:spin 1s linear infinite"><path d="M21 12a9 9 0 1 1-9-9"/></svg>');
  wrap.appendChild(document.createTextNode(mensagem || ' Carregando horários...'));
  container.appendChild(wrap);
};

window.renderizarBotoesHorarios = function(container, horarios, aoSelecionar) {
  if (!container) return;
  container.className = 'grade-horarios';
  container.replaceChildren();
  (horarios || []).forEach(s => {
    const botao = document.createElement('button');
    botao.type = 'button';
    botao.className = 'botao-horario';
    botao.textContent = String(s.horaFormatada ?? '');
    botao.addEventListener('click', () => aoSelecionar(botao, s));
    container.appendChild(botao);
  });
};

function _appIconeSvgToast(type) {
  if (type === 'success') {
    return '<svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="#81c995" stroke-width="3" stroke-linecap="round" stroke-linejoin="round"><path d="M22 11.08V12a10 10 0 1 1-5.93-9.14"></path><polyline points="22 4 12 14.01 9 11.01"></polyline></svg>';
  }
  if (type === 'error') {
    return '<svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="#f28b82" stroke-width="3" stroke-linecap="round" stroke-linejoin="round"><circle cx="12" cy="12" r="10"></circle><line x1="15" y1="9" x2="9" y2="15"></line><line x1="9" y1="9" x2="15" y2="15"></line></svg>';
  }
  return '<svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="var(--clr-primary)" stroke-width="3" stroke-linecap="round" stroke-linejoin="round"><circle cx="12" cy="12" r="10"></circle><line x1="12" y1="16" x2="12" y2="12"></line><line x1="12" y1="8" x2="12.01" y2="8"></line></svg>';
}

function _appIconeSvgModal(type) {
  if (type === 'success') {
    return '<svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="#81c995" stroke-width="3" stroke-linecap="round" stroke-linejoin="round"><path d="M22 11.08V12a10 10 0 1 1-5.93-9.14"></path><polyline points="22 4 12 14.01 9 11.01"></polyline></svg>';
  }
  if (type === 'error') {
    return '<svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="#f28b82" stroke-width="3" stroke-linecap="round" stroke-linejoin="round"><circle cx="12" cy="12" r="10"></circle><line x1="15" y1="9" x2="9" y2="15"></line><line x1="9" y1="9" x2="15" y2="15"></line></svg>';
  }
  if (type === 'danger') {
    return '<svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="#ea4335" stroke-width="3" stroke-linecap="round" stroke-linejoin="round"><path d="M10.29 3.86L1.82 18a2 2 0 0 0 1.71 3h16.94a2 2 0 0 0 1.71-3L13.71 3.86a2 2 0 0 0-3.42 0z"></path><line x1="12" y1="9" x2="12" y2="13"></line><line x1="12" y1="17" x2="12.01" y2="17"></line></svg>';
  }
  if (type === 'warning') {
    return '<svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="#fbbc04" stroke-width="3" stroke-linecap="round" stroke-linejoin="round"><path d="M10.29 3.86L1.82 18a2 2 0 0 0 1.71 3h16.94a2 2 0 0 0 1.71-3L13.71 3.86a2 2 0 0 0-3.42 0z"></path><line x1="12" y1="9" x2="12" y2="13"></line><line x1="12" y1="17" x2="12.01" y2="17"></line></svg>';
  }
  return '<svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="var(--clr-primary)" stroke-width="3" stroke-linecap="round" stroke-linejoin="round"><circle cx="12" cy="12" r="10"></circle><line x1="12" y1="16" x2="12" y2="12"></line><line x1="12" y1="8" x2="12.01" y2="8"></line></svg>';
}

function _appCorDestaqueModal(type) {
  if (type === 'success') return '#81c995';
  if (type === 'error') return '#f28b82';
  if (type === 'danger') return '#ea4335';
  if (type === 'warning') return '#fbbc04';
  return 'var(--clr-primary, #8ab4f8)';
}

function _appFecharToast(toast) {
  toast.style.transform = 'translateX(120%)';
  toast.style.opacity = '0';
  setTimeout(() => toast.remove(), 300);
}

window.exibirToast = function(mensagem, type = 'success') {
  let container = document.getElementById('toast-app-container');
  if (!container) {
    container = document.createElement('div');
    container.id = 'toast-app-container';
    container.style.cssText = 'position: fixed; top: 2rem; right: 2rem; z-index: 10000; display: flex; flex-direction: column; gap: 0.75rem; pointer-events: none; max-width: 360px; width: calc(100% - 4rem);';
    document.body.appendChild(container);
  }

  const toast = document.createElement('div');
  toast.className = `toast-app toast-app-${type}`;
  toast.style.cssText = 'pointer-events: auto; display: flex; align-items: center; gap: 0.75rem; padding: 1rem 1.25rem; background: var(--glass-bg); border: 1px solid var(--glass-border); border-radius: var(--radius-md); box-shadow: var(--glass-shadow-lg); backdrop-filter: blur(20px); transform: translateX(120%); opacity: 0; transition: transform 0.3s cubic-bezier(0.34, 1.56, 0.64, 1), opacity 0.3s ease;';

  if (type === 'success') toast.style.borderLeft = '4px solid #81c995';
  else if (type === 'error') toast.style.borderLeft = '4px solid #f28b82';
  else toast.style.borderLeft = '4px solid var(--clr-primary)';

  const iconWrap = document.createElement('div');
  iconWrap.style.cssText = 'display:flex; align-items:center; justify-content:center; flex-shrink:0;';
  _appAnexarSvgEstatico(iconWrap, _appIconeSvgToast(type));

  const msgEl = document.createElement('div');
  msgEl.style.cssText = 'flex-grow:1; font-family:var(--font-body); font-size:0.875rem; font-weight:500; color:var(--clr-text); line-height:1.4;';
  msgEl.textContent = String(mensagem ?? '');

  const botaoFechar = document.createElement('button');
  botaoFechar.type = 'button';
  botaoFechar.style.cssText = 'background:none; border:none; color:var(--clr-text-subtle); cursor:pointer; display:flex; align-items:center; justify-content:center; padding:0.2rem; margin-left:0.5rem; transition:color 0.2s;';
  botaoFechar.addEventListener('click', () => _appFecharToast(toast));
  _appAnexarSvgEstatico(botaoFechar, '<svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round"><line x1="18" y1="6" x2="6" y2="18"></line><line x1="6" y1="6" x2="18" y2="18"></line></svg>');

  toast.appendChild(iconWrap);
  toast.appendChild(msgEl);
  toast.appendChild(botaoFechar);
  container.appendChild(toast);

  setTimeout(() => {
    toast.style.transform = 'translateX(0)';
    toast.style.opacity = '1';
  }, 50);

  setTimeout(() => {
    if (toast.parentElement) _appFecharToast(toast);
  }, 4000);
};

window.exibirModal = function(titulo, mensagem, type = 'success', aoConfirmar = null) {
  const tiposValidos = ['success', 'error', 'info'];

  if (typeof mensagem === 'string' && tiposValidos.includes(mensagem.toLowerCase()) && (typeof type === 'undefined' || type === 'success' || typeof type === 'function')) {
    if (typeof type === 'function') {
      aoConfirmar = type;
    }
    type = mensagem.toLowerCase();
    mensagem = titulo;
    titulo = type === 'error' ? 'Erro' : (type === 'success' ? 'Sucesso' : 'Aviso');
  }
  else if (typeof mensagem === 'undefined' || mensagem === null) {
    mensagem = titulo;
    titulo = type === 'error' ? 'Erro' : (type === 'success' ? 'Sucesso' : 'Aviso');
  }

  const modalAnterior = document.getElementById('app-dynamic-modal-overlay');
  if (modalAnterior) modalAnterior.remove();

  const corDestaque = _appCorDestaqueModal(type);

  const overlay = document.createElement('div');
  overlay.id = 'app-dynamic-modal-overlay';
  overlay.style.cssText = 'position: fixed; top: 0; left: 0; width: 100vw; height: 100vh; background: rgba(0,0,0,0.55); backdrop-filter: blur(12px); z-index: 11000; display: flex; align-items: center; justify-content: center; padding: 1.5rem; opacity: 0; transition: opacity 0.25s ease;';

  const card = document.createElement('div');
  card.style.cssText = 'width: 100%; max-width: 480px; background: var(--glass-bg, rgba(30, 30, 30, 0.75)); backdrop-filter: blur(25px); border: 1px solid var(--glass-border, rgba(255, 255, 255, 0.1)); border-radius: var(--radius-lg, 12px); box-shadow: var(--glass-shadow-lg, 0 8px 32px 0 rgba(0, 0, 0, 0.3)); overflow: hidden; transform: scale(0.9); opacity: 0; transition: transform 0.3s cubic-bezier(0.34, 1.56, 0.64, 1), opacity 0.3s ease;';

  const header = document.createElement('div');
  header.style.cssText = 'display: flex; align-items: center; gap: 0.75rem; padding: 1.25rem 1.75rem; border-bottom: 1px solid var(--glass-border, rgba(255, 255, 255, 0.1));';
  const iconWrap = document.createElement('div');
  iconWrap.style.cssText = 'display: flex; align-items: center; justify-content: center; flex-shrink: 0;';
  _appAnexarSvgEstatico(iconWrap, _appIconeSvgModal(type));
  const tituloEl = document.createElement('span');
  tituloEl.style.cssText = 'font-family: var(--font-heading, inherit); font-size: 1.1rem; font-weight: 600; color: var(--clr-text, #ffffff);';
  tituloEl.textContent = String(titulo ?? '');
  header.appendChild(iconWrap);
  header.appendChild(tituloEl);

  const body = document.createElement('div');
  body.style.cssText = 'padding: 1.75rem; font-family: var(--font-body, inherit); font-size: 0.9375rem; color: var(--clr-text-muted, #cccccc); line-height: 1.5; border-bottom: 1px solid var(--glass-border, rgba(255, 255, 255, 0.1));';
  body.textContent = String(mensagem ?? '');

  const footer = document.createElement('div');
  footer.style.cssText = 'display: flex; justify-content: flex-end; padding: 1rem 1.75rem; background: rgba(0,0,0,0.15);';
  const botaoConfirmar = document.createElement('button');
  botaoConfirmar.id = 'app-dynamic-modal-confirm';
  botaoConfirmar.type = 'button';
  botaoConfirmar.textContent = 'Ok';
  botaoConfirmar.style.cssText = `background: ${corDestaque}; border: none; border-radius: var(--radius-sm, 4px); color: #121212; padding: 0.5rem 1.75rem; font-family: var(--font-heading, inherit); font-size: 0.875rem; font-weight: 600; cursor: pointer; transition: opacity 0.2s; outline: none; box-shadow: 0 2px 8px rgba(0,0,0,0.2);`;

  footer.appendChild(botaoConfirmar);
  card.appendChild(header);
  card.appendChild(body);
  card.appendChild(footer);
  overlay.appendChild(card);
  document.body.appendChild(overlay);

  const fecharModal = () => {
    overlay.style.opacity = '0';
    card.style.transform = 'scale(0.9)';
    card.style.opacity = '0';
    setTimeout(() => {
      overlay.remove();
      if (typeof aoConfirmar === 'function') aoConfirmar();
    }, 250);
  };

  botaoConfirmar.addEventListener('click', fecharModal);
  overlay.addEventListener('click', (e) => { if (e.target === overlay) fecharModal(); });

  const ouvinteTecla = (e) => {
    if (e.key === 'Escape') {
      document.removeEventListener('keydown', ouvinteTecla);
      fecharModal();
    }
  };
  document.addEventListener('keydown', ouvinteTecla);

  setTimeout(() => {
    overlay.style.opacity = '1';
    card.style.transform = 'scale(1)';
    card.style.opacity = '1';
  }, 20);
};

window.exibirModalConfirmacao = function(titulo, mensagem, type = 'warning', aoConfirmar = null) {
  const modalAnterior = document.getElementById('app-dynamic-modal-overlay');
  if (modalAnterior) modalAnterior.remove();

  const corDestaque = _appCorDestaqueModal(type);

  const overlay = document.createElement('div');
  overlay.id = 'app-dynamic-modal-overlay';
  overlay.style.cssText = 'position: fixed; top: 0; left: 0; width: 100vw; height: 100vh; background: rgba(0,0,0,0.55); backdrop-filter: blur(12px); z-index: 11000; display: flex; align-items: center; justify-content: center; padding: 1.5rem; opacity: 0; transition: opacity 0.25s ease;';

  const card = document.createElement('div');
  card.style.cssText = 'width: 100%; max-width: 440px; background: var(--glass-bg, rgba(30, 30, 30, 0.75)); backdrop-filter: blur(25px); border: 1px solid var(--glass-border, rgba(255, 255, 255, 0.1)); border-radius: var(--radius-lg, 12px); box-shadow: var(--glass-shadow-lg, 0 8px 32px 0 rgba(0, 0, 0, 0.3)); overflow: hidden; transform: scale(0.9); opacity: 0; transition: transform 0.3s cubic-bezier(0.34, 1.56, 0.64, 1), opacity 0.3s ease;';

  const header = document.createElement('div');
  header.style.cssText = 'display: flex; align-items: center; gap: 0.75rem; padding: 1.25rem 1.75rem; border-bottom: 1px solid var(--glass-border, rgba(255, 255, 255, 0.1));';
  const iconWrap = document.createElement('div');
  iconWrap.style.cssText = 'display: flex; align-items: center; justify-content: center; flex-shrink: 0;';
  _appAnexarSvgEstatico(iconWrap, _appIconeSvgModal(type));
  const tituloEl = document.createElement('span');
  tituloEl.style.cssText = 'font-family: var(--font-heading, inherit); font-size: 1.1rem; font-weight: 600; color: var(--clr-text, #ffffff);';
  tituloEl.textContent = String(titulo ?? '');
  header.appendChild(iconWrap);
  header.appendChild(tituloEl);

  const body = document.createElement('div');
  body.style.cssText = 'padding: 1.75rem; font-family: var(--font-body, inherit); font-size: 0.9375rem; color: var(--clr-text-muted, #cccccc); line-height: 1.5; border-bottom: 1px solid var(--glass-border, rgba(255, 255, 255, 0.1));';
  body.textContent = String(mensagem ?? '');

  const footer = document.createElement('div');
  footer.style.cssText = 'display: flex; justify-content: flex-end; gap: 1rem; padding: 1rem 1.75rem; background: rgba(0,0,0,0.15);';

  const botaoCancelar = document.createElement('button');
  botaoCancelar.id = 'app-dynamic-modal-cancel';
  botaoCancelar.type = 'button';
  botaoCancelar.textContent = 'Cancelar';
  botaoCancelar.style.cssText = 'background: transparent; border: 1px solid var(--glass-border); border-radius: var(--radius-sm, 4px); color: var(--clr-text-muted); padding: 0.5rem 1.2rem; font-family: var(--font-heading, inherit); font-size: 0.875rem; font-weight: 600; cursor: pointer; transition: all 0.2s; outline: none;';

  const botaoConfirmar = document.createElement('button');
  botaoConfirmar.id = 'app-dynamic-modal-confirm';
  botaoConfirmar.type = 'button';
  botaoConfirmar.textContent = 'Confirmar';
  botaoConfirmar.style.cssText = `background: ${corDestaque}; border: none; border-radius: var(--radius-sm, 4px); color: #fff; padding: 0.5rem 1.2rem; font-family: var(--font-heading, inherit); font-size: 0.875rem; font-weight: 600; cursor: pointer; transition: opacity 0.2s; outline: none; box-shadow: 0 2px 8px rgba(0,0,0,0.2);`;

  footer.appendChild(botaoCancelar);
  footer.appendChild(botaoConfirmar);
  card.appendChild(header);
  card.appendChild(body);
  card.appendChild(footer);
  overlay.appendChild(card);
  document.body.appendChild(overlay);

  const fecharModal = () => {
    overlay.style.opacity = '0';
    card.style.transform = 'scale(0.9)';
    card.style.opacity = '0';
    setTimeout(() => overlay.remove(), 250);
  };

  botaoCancelar.addEventListener('click', fecharModal);
  botaoConfirmar.addEventListener('click', () => {
    fecharModal();
    if (typeof aoConfirmar === 'function') aoConfirmar();
  });

  const ouvinteTecla = (e) => {
    if (e.key === 'Escape') {
      document.removeEventListener('keydown', ouvinteTecla);
      fecharModal();
    }
  };
  document.addEventListener('keydown', ouvinteTecla);

  setTimeout(() => {
    overlay.style.opacity = '1';
    card.style.transform = 'scale(1)';
    card.style.opacity = '1';
  }, 20);
};

window.exibirModalObrigatorio = function(titulo, mensagem, tipo, aoConfirmar) {
  tipo = tipo || 'warning';

  const modalAnterior = document.getElementById('app-modal-obrigatorio-overlay');
  if (modalAnterior) modalAnterior.remove();

  const corDestaque = _appCorDestaqueModal(tipo);

  const overlay = document.createElement('div');
  overlay.id = 'app-modal-obrigatorio-overlay';
  overlay.style.cssText = 'position:fixed;top:0;left:0;width:100vw;height:100vh;background:rgba(0,0,0,0.7);backdrop-filter:blur(14px);z-index:12000;display:flex;align-items:center;justify-content:center;padding:1.5rem;opacity:0;transition:opacity 0.25s ease;';

  const card = document.createElement('div');
  card.style.cssText = 'width:100%;max-width:480px;background:var(--glass-bg);backdrop-filter:blur(25px);border:1px solid var(--glass-border);border-top:3px solid ' + corDestaque + ';border-radius:var(--radius-lg);box-shadow:var(--glass-shadow-lg);overflow:hidden;transform:scale(0.9);opacity:0;transition:transform 0.3s cubic-bezier(0.34,1.56,0.64,1),opacity 0.3s ease;';

  const header = document.createElement('div');
  header.style.cssText = 'display:flex;align-items:center;gap:0.75rem;padding:1.25rem 1.75rem;border-bottom:1px solid var(--glass-border);';
  const iconWrap = document.createElement('div');
  iconWrap.style.cssText = 'display:flex;align-items:center;justify-content:center;flex-shrink:0;';
  _appAnexarSvgEstatico(iconWrap, _appIconeSvgModal(tipo));
  const tituloEl = document.createElement('span');
  tituloEl.style.cssText = 'font-family:var(--font-heading,inherit);font-size:1.1rem;font-weight:600;color:var(--clr-text);';
  tituloEl.textContent = String(titulo ?? '');
  header.appendChild(iconWrap);
  header.appendChild(tituloEl);

  const body = document.createElement('div');
  body.style.cssText = 'padding:1.75rem;font-family:var(--font-body,inherit);font-size:0.9375rem;color:var(--clr-text-muted);line-height:1.6;border-bottom:1px solid var(--glass-border);';
  body.textContent = String(mensagem ?? '');

  const footer = document.createElement('div');
  footer.style.cssText = 'display:flex;justify-content:flex-end;padding:1rem 1.75rem;background:rgba(0,0,0,0.1);';
  const botaoEntendido = document.createElement('button');
  botaoEntendido.id = 'app-modal-obrigatorio-confirm';
  botaoEntendido.type = 'button';
  botaoEntendido.textContent = 'Entendido';
  botaoEntendido.style.cssText = 'background:' + corDestaque + ';border:none;border-radius:var(--radius-sm);color:#121212;padding:0.55rem 2rem;font-family:var(--font-heading,inherit);font-size:0.875rem;font-weight:700;cursor:pointer;transition:opacity 0.2s;outline:none;box-shadow:0 2px 8px rgba(0,0,0,0.2);';

  footer.appendChild(botaoEntendido);
  card.appendChild(header);
  card.appendChild(body);
  card.appendChild(footer);
  overlay.appendChild(card);
  document.body.appendChild(overlay);

  const fecharModal = () => {
    overlay.style.opacity = '0';
    card.style.transform = 'scale(0.9)';
    card.style.opacity = '0';
    setTimeout(() => {
      overlay.remove();
      if (typeof aoConfirmar === 'function') aoConfirmar();
    }, 250);
  };

  botaoEntendido.addEventListener('click', fecharModal);

  setTimeout(() => {
    overlay.style.opacity = '1';
    card.style.transform = 'scale(1)';
    card.style.opacity = '1';
    botaoEntendido.focus();
  }, 20);
};

function _appExibirErroCampo(input, mensagem) {
  const grupo = input.closest('.campo-grupo');
  if (!grupo) {
    if (mensagem && typeof window.exibirToast === 'function') {
      window.exibirToast(mensagem, 'error');
    }
    return;
  }

  let span = grupo.querySelector('[data-valmsg-for]');
  if (!span) {
    span = document.createElement('span');
    span.className = 'campo-mensagem-erro';
    span.setAttribute('data-valmsg-for', input.name || input.id || '');
    span.setAttribute('data-valmsg-replace', 'true');
    grupo.appendChild(span);
  }

  if (mensagem) {
    span.innerHTML = '<svg class="campo-mensagem-erro-icone" width="12" height="12" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round"><circle cx="12" cy="12" r="10"></circle><line x1="12" y1="8" x2="12" y2="12"></line><line x1="12" y1="16" x2="12.01" y2="16"></line></svg>' + window.escaparHtml(mensagem);
    span.className = 'campo-mensagem-erro';
    grupo.classList.add('campo-grupo--erro');
    input.classList.add('esta-invalido');
  } else {
    span.innerHTML = '';
    span.className = 'campo-mensagem-erro';
    grupo.classList.remove('campo-grupo--erro');
    input.classList.remove('esta-invalido');
  }
}

function _appValidarCampo(input) {
  const valor = input.value.trim();
  const obrigatorio = input.hasAttribute('required');
  const tipo = (input.type || '').toLowerCase();
  const minLen = parseInt(input.getAttribute('minlength') || '0', 10);
  const maxLen = parseInt(input.getAttribute('maxlength') || '0', 10);
  const pattern = input.getAttribute('pattern');

  if (obrigatorio && valor === '') {
    _appExibirErroCampo(input, 'Este campo é obrigatório.');
    return false;
  }

  if (valor !== '' && tipo === 'email') {
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (!emailRegex.test(valor)) {
      _appExibirErroCampo(input, 'Informe um e-mail válido.');
      return false;
    }
  }

  if (valor !== '' && minLen > 0 && valor.length < minLen) {
    _appExibirErroCampo(input, `Mínimo de ${minLen} caracteres.`);
    return false;
  }

  if (valor !== '' && maxLen > 0 && valor.length > maxLen) {
    _appExibirErroCampo(input, `Máximo de ${maxLen} caracteres.`);
    return false;
  }

  if (valor !== '' && pattern) {
    try {
      const re = new RegExp(pattern);
      if (!re.test(valor)) {
        const title = input.getAttribute('title') || 'Formato inválido.';
        _appExibirErroCampo(input, title);
        return false;
      }
    } catch (e) {}
  }

  _appExibirErroCampo(input, '');
  return true;
}

function inicializarValidacaoAntecipada(container) {
  container.querySelectorAll('[data-campo-validavel]:not([data-blur-bound])').forEach(input => {
    if (input.closest('[data-sem-validacao-blur]')) return;
    input.setAttribute('data-blur-bound', 'true');
    input.addEventListener('blur', () => _appValidarCampo(input));
    input.addEventListener('input', () => {
      if (input.classList.contains('esta-invalido')) _appValidarCampo(input);
    });
  });
}


window.RequisicaoAplicacao = (function () {
  const PADRAO_ERRO = 'Não foi possível concluir a operação. Tente novamente.';
  const PADRAO_CONEXAO = 'Erro de conexão. Tente novamente.';

  function extrairMensagem(data, fallback) {
    if (!data) return fallback || PADRAO_ERRO;
    return data.mensagem || data.message || data.erro || fallback || PADRAO_ERRO;
  }

  function exibirMensagem(mensagem, opcoes) {
    const opts = opcoes || {};
    const tipo = opts.tipo || 'error';
    const modo = opts.modo || 'toast';
    const titulo = opts.titulo || (tipo === 'error' ? 'Erro' : 'Sucesso');

    if (modo === 'modal' && typeof window.exibirModal === 'function') {
      window.exibirModal(titulo, mensagem, tipo);
      return;
    }
    if (typeof window.exibirToast === 'function') {
      window.exibirToast(mensagem, tipo);
      return;
    }
    if (typeof window.exibirModal === 'function') {
      window.exibirModal(titulo, mensagem, tipo);
      return;
    }
    alerta(mensagem);
  }

  function resolverElemento(ref, contexto) {
    if (!ref) return null;
    if (typeof ref === 'string') return (contexto || document).querySelector(ref);
    return ref;
  }

  function definirEstadoBotao(botao, carregando) {
    if (!botao) return;
    botao.disabled = carregando;
    botao.setAttribute('aria-busy', carregando ? 'true' : 'false');
  }

  function montarFormData(form) {
    const formData = new FormData(form);
    const tokenInput = form.querySelector('input[name="__RequestVerificationToken"]');
    if (tokenInput && !formData.has('__RequestVerificationToken')) {
      formData.append('__RequestVerificationToken', tokenInput.value);
    }
    return formData;
  }

  function processarRespostaJson(resultado, opcoes) {
    const data = resultado.data;
    if (data && data.sucesso) {
      if (typeof opcoes.onSucesso === 'function') {
        opcoes.onSucesso(data, resultado);
        return;
      }
      window.location.href = data.urlRedirecionamento || opcoes.redirecionamentoPadrao || '/';
      return;
    }

    const msg = extrairMensagem(data, opcoes.mensagemErroPadrao);
    if (typeof opcoes.onErro === 'function') {
      opcoes.onErro(msg, resultado);
      return;
    }
    exibirMensagem(msg, { tipo: 'error', titulo: opcoes.tituloErro, modo: opcoes.modoErro });
  }

  function enviarFormulario(form, opcoes) {
    const opts = opcoes || {};
    const formEl = resolverElemento(form);
    if (!formEl) return Promise.reject(new Error('Formulário não encontrado.'));

    const botao = resolverElemento(opts.botao, formEl) || formEl.querySelector('[type="submit"]');
    const url = opts.url || formEl.action || window.location.pathname;

    definirEstadoBotao(botao, true);
    if (typeof opts.antesEnviar === 'function') opts.antesEnviar(formEl);

    return window.requisitarApp(url, {
      method: opts.metodo || 'POST',
      headers: Object.assign({ 'X-Requested-With': 'XMLHttpRequest' }, opts.headers || {}),
      body: montarFormData(formEl),
      credentials: opts.credentials || 'same-origin'
    })
      .then((response) => response.json()
        .then((data) => ({ ok: response.ok, status: response.status, data }))
        .catch(() => ({ ok: response.ok, status: response.status, data: null })))
      .then((resultado) => {
        processarRespostaJson(resultado, opts);
        return resultado;
      })
      .catch((err) => {
        if (err && err.codigo === 'SESSAO_EXPIRADA') throw err;
        const msg = opts.mensagemConexaoPadrao || PADRAO_CONEXAO;
        if (typeof opts.onErro === 'function') opts.onErro(msg, null);
        else exibirMensagem(msg, { tipo: 'error', titulo: opts.tituloErro, modo: opts.modoErro });
        throw err;
      })
      .finally(() => {
        definirEstadoBotao(botao, false);
        if (typeof opts.depoisEnviar === 'function') opts.depoisEnviar(formEl);
      });
  }

  function enviarJson(url, payload, opcoes) {
    const opts = opcoes || {};
    return window.requisitarApp(url, {
      method: opts.metodo || 'POST',
      headers: Object.assign({
        'Content-Type': 'application/json',
        'X-Requested-With': 'XMLHttpRequest'
      }, opts.headers || {}),
      body: JSON.stringify(payload),
      credentials: opts.credentials || 'same-origin'
    })
      .then((response) => response.json()
        .then((data) => ({ ok: response.ok, status: response.status, data }))
        .catch(() => ({ ok: response.ok, status: response.status, data: null })))
      .then((resultado) => {
        processarRespostaJson(resultado, opts);
        return resultado;
      })
      .catch((err) => {
        if (err && err.codigo === 'SESSAO_EXPIRADA') throw err;
        const msg = opts.mensagemConexaoPadrao || PADRAO_CONEXAO;
        if (typeof opts.onErro === 'function') opts.onErro(msg, null);
        else exibirMensagem(msg, { tipo: 'error', titulo: opts.tituloErro, modo: opts.modoErro });
        throw err;
      });
  }

  function vincularFormulario(form, opcoes) {
    const opts = opcoes || {};
    const formEl = resolverElemento(form);
    if (!formEl) return;

    formEl.addEventListener('submit', (e) => {
      if (typeof opts.validar === 'function' && opts.validar(formEl, e) === false) {
        e.preventDefault();
        return;
      }
      e.preventDefault();
      enviarFormulario(formEl, opts);
    });
  }

  function vincularFormulariosAutomaticos() {
    document.querySelectorAll('form[data-app-ajax-form]').forEach((form) => {
      if (form.dataset.appAjaxBound === 'true') return;
      form.dataset.appAjaxBound = 'true';
      vincularFormulario(form, {
        botao: form.getAttribute('data-app-ajax-botao') || undefined,
        mensagemErroPadrao: form.getAttribute('data-app-ajax-erro') || undefined,
        tituloErro: form.getAttribute('data-app-ajax-titulo-erro') || undefined,
        modoErro: form.getAttribute('data-app-ajax-modo-erro') || 'toast'
      });
    });
  }

  return {
    extrairMensagem,
    exibirMensagem,
    enviarFormulario,
    enviarJson,
    vincularFormulario,
    vincularFormulariosAutomaticos
  };
})();

window.extrairMensagemResposta = window.RequisicaoAplicacao.extrairMensagem;
window.exibirMensagemUsuario = window.RequisicaoAplicacao.exibirMensagem;

function vincularConfirmacaoData(container) {
  container.querySelectorAll('[data-confirmacao]:not([data-confirmacao-vinculado])').forEach(el => {
    el.setAttribute('data-confirmacao-vinculado', 'true');
    el.addEventListener('click', (e) => {
      e.preventDefault();
      
      const mensagem = el.getAttribute('data-confirmacao') || 'Tem certeza que deseja realizar esta ação?';
      const titulo = el.getAttribute('data-confirmacao-titulo') || 'Confirmação';
      let tipo = el.getAttribute('data-confirmacao-tipo') || 'warning';
      if (tipo === 'perigo' || tipo === 'danger') tipo = 'error';
      if (tipo === 'aviso') tipo = 'warning';

      window.exibirModalConfirmacao(titulo, mensagem, tipo, () => {
        if (el.tagName === 'BUTTON' && el.type === 'submit') {
          const form = el.closest('form');
          if (form) form.submit();
        } 
        else if (el.tagName === 'A') {
          window.location.href = el.href;
        }
        else if (el.hasAttribute('hx-post') || el.hasAttribute('hx-get') || el.hasAttribute('hx-delete')) {
          htmx.trigger(el, 'confirmed');
        }
      });
    });
  });
}

document.addEventListener('DOMContentLoaded', () => {
  inicializarApp(document);
  vincularConfirmacaoData(document);
});
document.addEventListener('htmx:load', (e) => {
  inicializarApp(e.detail.elt);
  vincularConfirmacaoData(e.detail.elt);
});

(function() {
  let ultimaAtividadeEm = Date.now();
  let ultimaRenovacaoEm = Date.now();
  const RENEWAL_INTERVAL = 10 * 60 * 1000; // 10 minutos
  
  const atualizarAtividade = () => { ultimaAtividadeEm = Date.now(); };
  document.addEventListener('mousemove', atualizarAtividade, { passive: true });
  document.addEventListener('keydown', atualizarAtividade, { passive: true });
  document.addEventListener('click', atualizarAtividade, { passive: true });
  document.addEventListener('touchstart', atualizarAtividade, { passive: true });

  setInterval(() => {
    const now = Date.now();
    if (now - ultimaAtividadeEm < RENEWAL_INTERVAL && now - ultimaRenovacaoEm >= RENEWAL_INTERVAL) {
      const p = window.location.pathname.toLowerCase();
      if (p.includes('/contextos/autenticacao/login') || p.includes('/contextos/autenticacao/cadastro') || p.includes('/contextos/autenticacao/sair') || p.includes('/autenticacao/login') || p.includes('/autenticacao/cadastro') || p.includes('/autenticacao/sair')) return;
      
      window.requisitarApp('/Contextos/Autenticacao/RenovarToken', {
        method: 'POST'
      }).then(r => {
        if (r.ok) ultimaRenovacaoEm = Date.now();
      }).catch(err => {
        if (err && err.codigo === 'SESSAO_EXPIRADA') return;
        console.error('Erro ao renovar sessão:', err);
      });
    }
  }, 60 * 1000); // Verifica a cada 1 minuto
})();

document.addEventListener('DOMContentLoaded', () => {
  if (window.RequisicaoAplicacao) {
    window.RequisicaoAplicacao.vincularFormulariosAutomaticos();
  }

  document.querySelectorAll('.mensagens-servidor').forEach(el => {
    const msgSucesso = el.getAttribute('data-sucesso');
    const msgErro = el.getAttribute('data-erro');

    if (window.RequisicaoAplicacao) {
      if (msgSucesso) window.RequisicaoAplicacao.exibirMensagem(msgSucesso, { tipo: 'success' });
      if (msgErro) window.RequisicaoAplicacao.exibirMensagem(msgErro, { tipo: 'error' });
      return;
    }

    if (typeof window.exibirModal === 'function' || typeof window.exibirToast === 'function') {
      const exibirMensagemFn = window.exibirModal || window.exibirToast;
      if (msgSucesso) exibirMensagemFn(msgSucesso, 'success');
      if (msgErro) exibirMensagemFn(msgErro, 'error');
    }
  });
});

