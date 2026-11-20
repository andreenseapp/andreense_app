(function () {
  'use strict';

  var etapaAtual = 1;
  var totalEtapas = 5;

  var etapas = document.querySelectorAll('.assistente-etapa');
  var steps = document.querySelectorAll('.assistente-passo');
  var btnPrev = document.getElementById('btn-anterior');
  var btnNext = document.getElementById('btn-proximo');
  var btnSubmit = document.getElementById('btn-enviar');

  function mostrarEtapa(n) {
    etapas.forEach(function (e, i) {
      e.classList.toggle('assistente-etapa--ativa', i + 1 === n);
    });
    steps.forEach(function (s, i) {
      s.classList.remove('assistente-passo--ativo', 'assistente-passo--completo');
      if (i + 1 === n) s.classList.add('assistente-passo--ativo');
      if (i + 1 < n) s.classList.add('assistente-passo--completo');
    });
    if (btnPrev) btnPrev.hidden = n === 1;
    if (btnNext) btnNext.hidden = n === totalEtapas;
    if (btnSubmit) btnSubmit.hidden = n !== totalEtapas;
    if (n === 5 && typeof gerarSugestaoTitulo === 'function') {
      gerarSugestaoTitulo();
    }
  }

  if (btnNext) {
    btnNext.addEventListener('click', function () {
      if (etapaAtual < totalEtapas) {
        etapaAtual++;
        mostrarEtapa(etapaAtual);
        window.scrollTo({ top: 0, behavior: 'smooth' });
      }
    });
  }

  if (btnPrev) {
    btnPrev.addEventListener('click', function () {
      if (etapaAtual > 1) {
        etapaAtual--;
        mostrarEtapa(etapaAtual);
        window.scrollTo({ top: 0, behavior: 'smooth' });
      }
    });
  }

  steps.forEach(function(step) {
    step.addEventListener('click', function() {
      var stepTarget = parseInt(this.getAttribute('data-step'));
      if (stepTarget && stepTarget !== etapaAtual) {
        etapaAtual = stepTarget;
        mostrarEtapa(etapaAtual);
        window.scrollTo({ top: 0, behavior: 'smooth' });
      }
    });
  });

  document.querySelectorAll('input[type="number"][max], input[type="number"][min]').forEach(function (inp) {
    inp.addEventListener('input', function () {
      var val = parseFloat(this.value);
      if (isNaN(val)) return;
      var min = parseFloat(this.getAttribute('min'));
      var max = parseFloat(this.getAttribute('max'));
      if (!isNaN(max) && val > max) this.value = max;
      if (!isNaN(min) && val < min) this.value = min;
    });
  });

  var inpValorPrincipal = document.querySelector('[name="ValorPrincipal"]');
  var inpCondominio     = document.querySelector('[name="ValorCondominio"]');
  var inpIptu           = document.querySelector('[name="ValorIptu"]');
  var previewPacote     = document.getElementById('previa-pacote');
  var pacoteTotalValor  = document.getElementById('pacote-total-valor');

  function atualizarPacote() {
    var principal  = parseFloat(inpValorPrincipal?.value) || 0;
    var condominio = parseFloat(inpCondominio?.value)     || 0;
    var iptu       = parseFloat(inpIptu?.value)           || 0;

    var tipoNegocio = document.querySelector('[name="TipoNegocio"]:checked')?.value;
    if (previewPacote) {
      previewPacote.hidden = tipoNegocio !== 'aluguel' || (condominio === 0 && iptu === 0);
    }
    if (pacoteTotalValor) {
      var total = tipoNegocio === 'aluguel' ? (principal + condominio + iptu) : principal;
      pacoteTotalValor.textContent = 'R$ ' + total.toLocaleString('pt-BR');
    }
  }

  [inpValorPrincipal, inpCondominio, inpIptu].forEach(function (inp) {
    inp?.addEventListener('input', atualizarPacote);
  });

  document.querySelectorAll('[name="TipoNegocio"]').forEach(function (r) {
    r.addEventListener('change', function () {
      var labelValor = document.getElementById('label-valor-principal');
      if (this.value === 'aluguel') {
        if (labelValor) labelValor.textContent = 'Valor do Aluguel (R$/mês) *';
      } else {
        if (labelValor) labelValor.textContent = 'Valor de Venda *';
      }
      atualizarPacote();
    });
  });

  var inpComplemento = document.getElementById('campo-complemento');
  var boxBotoesApto = document.getElementById('botoes-padrao-apto');
  var textoDicaCompl = document.getElementById('complemento-texto-dica');

  function atualizarModoComplemento() {
    var tipoImovel = document.querySelector('[name="TipoImovel"]:checked')?.value;
    if (tipoImovel === 'apartamento') {
      if (inpComplemento) inpComplemento.placeholder = 'Ex: Apto 101 Bloco A (ou Bl 01)';
      if (boxBotoesApto) boxBotoesApto.style.display = 'flex';
      if (textoDicaCompl) textoDicaCompl.textContent = 'ℹ️ Não é obrigatório. Dica para apto:';
    } else {
      if (inpComplemento) inpComplemento.placeholder = 'Ex: Casa 2, Fundos, Sala 101';
      if (boxBotoesApto) boxBotoesApto.style.display = 'none';
      if (textoDicaCompl) textoDicaCompl.textContent = 'ℹ️ Não é obrigatório informar.';
    }
  }

  document.querySelectorAll('[name="TipoImovel"]').forEach(function (r) {
    r.addEventListener('change', atualizarModoComplemento);
  });

  document.querySelectorAll('.btn-pill-complemento').forEach(function (botao) {
    botao.addEventListener('click', function () {
      if (!inpComplemento) return;
      var valAdd = this.getAttribute('data-add') || '';
      var valAtual = inpComplemento.value;
      if (valAtual && !valAtual.endsWith(' ')) {
        inpComplemento.value = valAtual + ' ' + valAdd;
      } else {
        inpComplemento.value = valAtual + valAdd;
      }
      inpComplemento.focus();
    });
  });

  var inpTitulo = document.querySelector('[name="Titulo"]');
  var tituloEditadoManualmente = false;
  var sugestaoAtualText = '';

  function formatarValorCurto(val) {
    if (!val || val <= 0) return '';
    if (val >= 1000000) {
      var milhoes = (val / 1000000).toLocaleString('pt-BR', { maximumFractionDigits: 1 });
      return milhoes + (val >= 2000000 ? ' milhões' : ' milhão');
    }
    if (val >= 1000) {
      var milhares = (val / 1000).toLocaleString('pt-BR', { maximumFractionDigits: 1 });
      return milhares + ' mil';
    }
    return 'R$ ' + val.toLocaleString('pt-BR');
  }

  function gerarSugestaoTitulo() {
    var tipoImovelVal = document.querySelector('[name="TipoImovel"]:checked')?.value || '';
    var quartosVal = parseInt(document.querySelector('[name="Quartos"]')?.value) || 0;
    var valorVal = parseFloat(document.querySelector('[name="ValorPrincipal"]')?.value) || 0;

    var partes = [];
    var mapaTipos = {
      'apartamento': 'Apartamento',
      'casa': 'Casa',
      'kitnet': 'Kitnet/Studio',
      'comercial': 'Imóvel Comercial'
    };
    partes.push(mapaTipos[tipoImovelVal] || 'Imóvel');

    if (quartosVal === 1) {
      partes.push('1 quarto');
    } else if (quartosVal > 1) {
      partes.push(quartosVal + ' quartos');
    }

    var valorFormatado = formatarValorCurto(valorVal);
    if (valorFormatado) {
      partes.push(valorFormatado);
    }

    sugestaoAtualText = partes.join(' ');

    if (inpTitulo && (!tituloEditadoManualmente || !inpTitulo.value.trim())) {
      inpTitulo.value = sugestaoAtualText;
    }

    var linkSugestao = document.getElementById('btn-usar-sugestao');
    var boxSugestao = document.getElementById('sugestao-titulo-box');
    if (linkSugestao && sugestaoAtualText) {
      linkSugestao.textContent = sugestaoAtualText;
      if (boxSugestao) boxSugestao.style.display = 'flex';
    } else if (boxSugestao) {
      boxSugestao.style.display = 'none';
    }
  }

  if (inpTitulo) {
    inpTitulo.addEventListener('input', function () {
      if (this.value.trim() !== '' && this.value !== sugestaoAtualText) {
        tituloEditadoManualmente = true;
      } else if (this.value.trim() === '') {
        tituloEditadoManualmente = false;
        gerarSugestaoTitulo();
      }
    });
  }

  var linkUsarSugestao = document.getElementById('btn-usar-sugestao');
  if (linkUsarSugestao && inpTitulo) {
    linkUsarSugestao.addEventListener('click', function (e) {
      e.preventDefault();
      inpTitulo.value = sugestaoAtualText;
      tituloEditadoManualmente = false;
      inpTitulo.focus();
    });
  }

  document.querySelectorAll('[name="TipoImovel"], [name="Quartos"], [name="ValorPrincipal"]').forEach(function (el) {
    el?.addEventListener('input', gerarSugestaoTitulo);
    el?.addEventListener('change', gerarSugestaoTitulo);
  });

  var uploadArea = document.getElementById('foto-envio-area');
  var inputFotos = document.getElementById('input-fotos');
  var previewGrid = document.getElementById('foto-previa-grade');
  var placeholder = document.getElementById('foto-placeholder');

  var arquivosSelecionados = [];

  if (uploadArea && inputFotos) {
    uploadArea.addEventListener('click', function (e) {
      if (!e.target.classList.contains('foto-previa-remover')) {
        inputFotos.click();
      }
    });

    uploadArea.addEventListener('dragover', function (e) {
      e.preventDefault();
      uploadArea.classList.add('arrastar-sobre');
    });
    uploadArea.addEventListener('dragleave', function () {
      uploadArea.classList.remove('arrastar-sobre');
    });
    uploadArea.addEventListener('drop', function (e) {
      e.preventDefault();
      uploadArea.classList.remove('arrastar-sobre');
      adicionarArquivos(e.dataTransfer.files);
    });

    inputFotos.addEventListener('change', function () {
      adicionarArquivos(this.files);
    });
  }

  var inputFotosRemovidas = document.getElementById('campo-fotos-removidas');
  document.querySelectorAll('.btn-remover-foto-existente').forEach(function(botao) {
    botao.addEventListener('click', function(ev) {
      ev.stopPropagation(); // Evita acionar o click do uploadArea (que abre o seletor)
      var item = botao.closest('.foto-existente-item');
      if (item) {
        var id = botao.getAttribute('data-id');
        if (id && inputFotosRemovidas) {
          var ids = inputFotosRemovidas.value ? inputFotosRemovidas.value.split(',') : [];
          ids.push(id);
          inputFotosRemovidas.value = ids.join(',');
        }
        item.remove(); // Remove do DOM

        if (typeof atualizarUIFotos === 'function') {
           atualizarUIFotos();
        }
      }
    });
  });

  function adicionarArquivos(files) {
    var maxFotos = parseInt(uploadArea?.dataset.max) || 15;
    Array.from(files).forEach(function (file) {
      if (arquivosSelecionados.length >= maxFotos) return;
      if (!file.type.match(/image\/(jpeg|png|webp)/)) return;
      arquivosSelecionados.push(file);
      renderizarPreview(file, arquivosSelecionados.length - 1);
    });
    sincronizarInput();
    atualizarUIFotos();
  }

  function renderizarPreview(file, idx) {
    var reader = new FileReader();
    reader.onload = function (e) {
      var item = document.createElement('div');
      item.className = 'foto-previa-item';
      item.dataset.idx = idx;

      var img = document.createElement('img');
      img.src = e.target.result;
      img.alt = 'Preview';

      var botao = document.createElement('button');
      botao.type = 'button';
      botao.className = 'foto-previa-remover';
      botao.textContent = '×';
      botao.addEventListener('click', function (ev) {
        ev.stopPropagation();
        removerFoto(idx);
      });

      item.appendChild(img);
      item.appendChild(botao);
      if (previewGrid) previewGrid.appendChild(item);
    };
    reader.readAsDataURL(file);
  }

  function removerFoto(idx) {
    arquivosSelecionados.splice(idx, 1);
    if (previewGrid) {
      var novos = previewGrid.querySelectorAll('.foto-previa-item:not(.foto-existente-item)');
      novos.forEach(function(n) { n.remove(); });
    }
    arquivosSelecionados.forEach(function (f, i) { renderizarPreview(f, i); });
    sincronizarInput();
    atualizarUIFotos();
  }

  function atualizarUIFotos() {
    var itensExistentes = document.querySelectorAll('.foto-existente-item').length;
    var totalFotos = itensExistentes + arquivosSelecionados.length;
    var maxFotos = parseInt(uploadArea?.dataset.max) || 15;
    var btnAdd = document.getElementById('btn-adicionar-mais-fotos');
    
    if (placeholder) {
      placeholder.style.display = totalFotos === 0 ? '' : 'none';
    }
    if (btnAdd) {
      btnAdd.style.display = (totalFotos > 0 && totalFotos < maxFotos) ? 'flex' : 'none';
    }
  }

  var btnAddMais = document.getElementById('btn-adicionar-mais-fotos');
  if (btnAddMais && inputFotos) {
    btnAddMais.addEventListener('click', function(e) {
      e.stopPropagation();
      inputFotos.click();
    });
  }

  function sincronizarInput() {
    if (!inputFotos) return;
    var dt = new DataTransfer();
    arquivosSelecionados.forEach(function (f) { dt.items.add(f); });
    inputFotos.files = dt.files;
  }

  var formNovoAnuncio = document.getElementById('form-novo-anuncio');
  if (formNovoAnuncio) {
    formNovoAnuncio.addEventListener('submit', function () {
      sincronizarInput();
    });
  }

  if (typeof gerarSugestaoTitulo === 'function') {
    gerarSugestaoTitulo();
  }
  if (typeof atualizarModoComplemento === 'function') {
    atualizarModoComplemento();
  }
  mostrarEtapa(etapaAtual);
})();
