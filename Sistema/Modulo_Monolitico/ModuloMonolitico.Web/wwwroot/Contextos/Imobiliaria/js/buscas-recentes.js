(function () {
  'use strict';

  var painel = document.getElementById('buscas-gestao');
  if (!painel || !window.ImobiliariaBuscasRecentes) return;

  var modo = painel.getAttribute('data-modo');
  var btnConfirmar = painel.querySelector('[data-confirmar-limpar]');
  if (btnConfirmar) {
    btnConfirmar.addEventListener('click', function (ev) {
      var msg = painel.getAttribute('data-msg-confirmar') || '';
      if (msg && !window.confirm(msg)) {
        ev.preventDefault();
      }
    });
  }

  if (modo !== 'anonimo') return;

  var api = window.ImobiliariaBuscasRecentes;
  var chave = painel.getAttribute('data-storage-key');
  var urlImoveis = painel.getAttribute('data-url-imoveis') || '/Imoveis';
  var listaEl = painel.querySelector('[data-lista-gestao]');
  var btnLimpar = painel.querySelector('[data-limpar-anonimo]');
  var msgVazia = painel.getAttribute('data-msg-vazia') || '';
  var msgExcluir = painel.getAttribute('data-msg-excluir') || 'Excluir';
  var msgConfirmar = painel.getAttribute('data-msg-confirmar') || '';

  function renderizar() {
    if (!listaEl) return;
    var lista = api.lerLista(chave);
    listaEl.innerHTML = '';

    if (!lista.length) {
      var vazio = document.createElement('p');
      vazio.className = 'buscas-gestao-vazio';
      vazio.setAttribute('data-vazio', '');
      vazio.textContent = msgVazia;
      listaEl.appendChild(vazio);
      if (btnLimpar) {
        btnLimpar.hidden = true;
        btnLimpar.disabled = true;
      }
      return;
    }

    if (btnLimpar) {
      btnLimpar.hidden = false;
      btnLimpar.disabled = false;
    }

    lista.forEach(function (item) {
      var row = document.createElement('div');
      row.className = 'busca-gestao-item';

      var link = document.createElement('a');
      link.className = 'busca-gestao-link';
      link.href = api.montarHref(urlImoveis, item);
      link.textContent = item.rotulo || 'Busca recente';

      var btn = document.createElement('button');
      btn.type = 'button';
      btn.className = 'btn-excluir-busca';
      btn.textContent = msgExcluir;
      btn.setAttribute('data-excluir-chave', item.chaveFiltro);

      row.appendChild(link);
      row.appendChild(btn);
      listaEl.appendChild(row);
    });
  }

  listaEl.addEventListener('click', function (ev) {
    var btn = ev.target.closest('[data-excluir-chave]');
    if (!btn) return;
    api.remover(chave, btn.getAttribute('data-excluir-chave'));
    renderizar();
  });

  if (btnLimpar) {
    btnLimpar.addEventListener('click', function () {
      if (msgConfirmar && !window.confirm(msgConfirmar)) return;
      api.limparTodas(chave);
      renderizar();
    });
  }

  renderizar();
})();
