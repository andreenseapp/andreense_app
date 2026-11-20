(function () {
  function alternarAba(idAba, botao) {
    document.querySelectorAll('.conteudo-aba').forEach(function (el) {
      el.classList.remove('active');
    });
    document.querySelectorAll('.botao-aba').forEach(function (el) {
      el.classList.remove('active');
    });
    document.getElementById(idAba).classList.add('active');
    botao.classList.add('active');
  }

  function incluirLinhaVariacao() {
    var tbody = document.getElementById('corpo-variacoes');
    var qtdLinhas = tbody.querySelectorAll('tr').length;

    var tr = document.createElement('tr');
    tr.className = 'linha-variacao';

    var tdId = document.createElement('td');
    tdId.className = 'coluna-oculta';
    var idOculto = document.createElement('input');
    idOculto.type = 'hidden';
    idOculto.name = 'Comando.Variacoes[' + qtdLinhas + '].Id';
    idOculto.value = '00000000-0000-0000-0000-000000000000';
    idOculto.className = 'variacao-id';
    tdId.appendChild(idOculto);

    var tdDesc = document.createElement('td');
    var inpDescricao = document.createElement('input');
    inpDescricao.type = 'text';
    inpDescricao.name = 'Comando.Variacoes[' + qtdLinhas + '].Descricao';
    inpDescricao.required = true;
    inpDescricao.className = 'variacao-desc';
    inpDescricao.placeholder = 'Ex: Vermelho G';
    tdDesc.appendChild(inpDescricao);

    var tdCodigoSku = document.createElement('td');
    var inpCodigoSku = document.createElement('input');
    inpCodigoSku.type = 'text';
    inpCodigoSku.name = 'Comando.Variacoes[' + qtdLinhas + '].CodigoSku';
    inpCodigoSku.className = 'variacao-sku';
    tdCodigoSku.appendChild(inpCodigoSku);

    var tdBarras = document.createElement('td');
    var inpBarras = document.createElement('input');
    inpBarras.type = 'text';
    inpBarras.name = 'Comando.Variacoes[' + qtdLinhas + '].CodigoBarras';
    inpBarras.className = 'variacao-barras';
    tdBarras.appendChild(inpBarras);

    var tdPreco = document.createElement('td');
    var inpPreco = document.createElement('input');
    inpPreco.type = 'number';
    inpPreco.step = '0.01';
    inpPreco.name = 'Comando.Variacoes[' + qtdLinhas + '].PrecoVenda';
    inpPreco.value = '0';
    inpPreco.className = 'variacao-preco';
    tdPreco.appendChild(inpPreco);

    var tdCusto = document.createElement('td');
    var inpCusto = document.createElement('input');
    inpCusto.type = 'number';
    inpCusto.step = '0.01';
    inpCusto.name = 'Comando.Variacoes[' + qtdLinhas + '].Custo';
    inpCusto.value = '0';
    inpCusto.className = 'variacao-custo';
    tdCusto.appendChild(inpCusto);

    var tdAcao = document.createElement('td');
    var botaoRemover = document.createElement('button');
    botaoRemover.type = 'button';
    botaoRemover.className = 'botao-remover';
    botaoRemover.setAttribute('data-acao', 'remover-linha');
    botaoRemover.title = 'Remover Variaçao';
    botaoRemover.innerHTML = '<svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><line x1="18" y1="6" x2="6" y2="18"></line><line x1="6" y1="6" x2="18" y2="18"></line></svg>';
    tdAcao.appendChild(botaoRemover);

    tr.append(tdId, tdDesc, tdCodigoSku, tdBarras, tdPreco, tdCusto, tdAcao);
    tbody.appendChild(tr);
    renumerarLinhas();
  }

  function removerLinha(botao) {
    var tbody = document.getElementById('corpo-variacoes');
    if (tbody.querySelectorAll('tr').length <= 1) {
      alerta('O produto precisa ter pelo menos uma varia��o.');
      return;
    }
    botao.closest('tr').remove();
    renumerarLinhas();
  }

  function renumerarLinhas() {
    var rows = document.querySelectorAll('#corpo-variacoes tr.linha-variacao');
    rows.forEach(function (row, index) {
      row.querySelector('.variacao-id').name = 'Comando.Variacoes[' + index + '].Id';
      row.querySelector('.variacao-desc').name = 'Comando.Variacoes[' + index + '].Descricao';
      row.querySelector('.variacao-sku').name = 'Comando.Variacoes[' + index + '].CodigoSku';
      row.querySelector('.variacao-barras').name = 'Comando.Variacoes[' + index + '].CodigoBarras';
      row.querySelector('.variacao-preco').name = 'Comando.Variacoes[' + index + '].PrecoVenda';
      row.querySelector('.variacao-custo').name = 'Comando.Variacoes[' + index + '].Custo';
    });
  }

  function escaparHtml(text) {
    var div = document.createElement('div');
    div.textContent = text;
    return div.innerHTML;
  }

  function fecharModalCadastroRapido() {
    var modal = document.getElementById('cadastro-rapido-modal');
    if (modal) modal.remove();
  }

  function obterTokenAntiforgery() {
    var tokenInput = document.querySelector('input[name="__RequestVerificationToken"]');
    return tokenInput ? tokenInput.value : '';
  }

  function notificarSucesso(titulo, mensagem) {
    if (window.exibirModal) {
      window.exibirModal(titulo, mensagem, 'success');
    } else {
      alerta(mensagem);
    }
  }

  async function cadastrarRapido(url, formData, idSelecao, aoSucesso) {
    var response = await window.requisitarApp(url, { method: 'POST', body: formData });
    var payload;

    try {
      payload = await response.json();
    } catch (_) {
      throw new Error('Erro ao salvar registro.');
    }

    if (!response.ok || !payload.sucesso) {
      throw new Error(payload.mensagem || 'Erro ao salvar registro.');
    }

    aoSucesso(payload);
    fecharModalCadastroRapido();
  }

  function abrirCadastroRapido(tituloCampo, tituloModal, url, idSelecao) {
    fecharModalCadastroRapido();

    var modal = document.createElement('div');
    modal.id = 'cadastro-rapido-modal';
    modal.className = 'cadastro-rapido-modal';

    modal.innerHTML =
      '<div class="cadastro-rapido-caixa">' +
        '<h3 class="cadastro-rapido-titulo">Adicionar ' + escaparHtml(tituloCampo) + '</h3>' +
        '<p class="cadastro-rapido-dica">Cadastre um(a) novo(a) ' + escaparHtml(tituloCampo.toLowerCase()) + ' para selecionar instantaneamente.</p>' +
        '<div class="cadastro-rapido-campo">' +
          '<label for="cadastro-rapido-nome">Nome / Descri��o *</label>' +
          '<input type="text" id="cadastro-rapido-nome" placeholder="Ex: ' + (tituloCampo === 'Marca' ? 'Nike' : 'Eletr�nicos') + '" required />' +
        '</div>' +
        '<div class="cadastro-rapido-acoes">' +
          '<button type="button" class="botao-rapido-cancelar" data-acao="fechar-cadastro-rapido">Cancelar</button>' +
          '<button type="button" class="botao-rapido-salvar" id="botao-cadastro-rapido-salvar">Salvar</button>' +
        '</div>' +
      '</div>';

    document.body.appendChild(modal);

    var nomeInput = document.getElementById('cadastro-rapido-nome');
    nomeInput.focus();

    nomeInput.addEventListener('keydown', function (e) {
      if (e.key === 'Enter') {
        e.preventDefault();
        document.getElementById('botao-cadastro-rapido-salvar').click();
      }
    });

    document.getElementById('botao-cadastro-rapido-salvar').addEventListener('click', async function () {
      var nome = nomeInput.value.trim();
      if (!nome) {
        alerta('Por favor, informe o nome.');
        return;
      }

      var botaoSalvar = this;
      botaoSalvar.disabled = true;
      botaoSalvar.textContent = 'Salvando...';

      try {
        var formData = new FormData();
        formData.append('nome', nome);
        formData.append('__RequestVerificationToken', obterTokenAntiforgery());

        await cadastrarRapido(url, formData, idSelecao, function (res) {
          var elSelecao = document.getElementById(idSelecao);
          elSelecao.add(new Option(res.nome, res.nome, true, true));
          notificarSucesso('Sucesso', tituloCampo + ' "' + res.nome + '" criada(o) com sucesso!');
        });
      } catch (err) {
        alerta(err.message || 'Erro inesperado.');
      } finally {
        botaoSalvar.disabled = false;
        botaoSalvar.textContent = 'Salvar';
      }
    });
  }

  function abrirCadastroRapidoUnidade() {
    fecharModalCadastroRapido();

    var modal = document.createElement('div');
    modal.id = 'cadastro-rapido-modal';
    modal.className = 'cadastro-rapido-modal';

    modal.innerHTML =
      '<div class="cadastro-rapido-caixa">' +
        '<h3 class="cadastro-rapido-titulo">Adicionar Unidade de Medida</h3>' +
        '<p class="cadastro-rapido-dica">Cadastre um novo c�digo de U.M. e sua respectiva descri��o.</p>' +
        '<div class="cadastro-rapido-campo cadastro-rapido-campo--maiusculo">' +
          '<label for="unidade-rapida-sigla">Sigla (U.M.) *</label>' +
          '<input type="text" id="unidade-rapida-sigla" placeholder="Ex: LT, DZ, CJ" required maxlength="6" />' +
        '</div>' +
        '<div class="cadastro-rapido-campo">' +
          '<label for="unidade-rapida-nome">Nome / Descri��o *</label>' +
          '<input type="text" id="unidade-rapida-nome" placeholder="Ex: Litro, D�zia, Conjunto" required />' +
        '</div>' +
        '<div class="cadastro-rapido-acoes">' +
          '<button type="button" class="botao-rapido-cancelar" data-acao="fechar-cadastro-rapido">Cancelar</button>' +
          '<button type="button" class="botao-rapido-salvar" id="botao-unidade-rapida-cadastrar">Salvar</button>' +
        '</div>' +
      '</div>';

    document.body.appendChild(modal);

    var siglaInput = document.getElementById('unidade-rapida-sigla');
    siglaInput.focus();

    siglaInput.addEventListener('input', function () {
      this.value = this.value.toUpperCase().replace(/[^A-Z0-9]/g, '');
    });

    document.getElementById('unidade-rapida-nome').addEventListener('keydown', function (e) {
      if (e.key === 'Enter') {
        e.preventDefault();
        document.getElementById('botao-unidade-rapida-cadastrar').click();
      }
    });

    document.getElementById('botao-unidade-rapida-cadastrar').addEventListener('click', async function () {
      var sigla = siglaInput.value.trim();
      var nome = document.getElementById('unidade-rapida-nome').value.trim();

      if (!sigla) {
        alerta('Por favor, informe a sigla.');
        return;
      }
      if (!nome) {
        alerta('Por favor, informe a descri��o/nome.');
        return;
      }

      var botaoSalvar = this;
      botaoSalvar.disabled = true;
      botaoSalvar.textContent = 'Salvando...';

      try {
        var formData = new FormData();
        formData.append('sigla', sigla);
        formData.append('nome', nome);
        formData.append('__RequestVerificationToken', obterTokenAntiforgery());

        await cadastrarRapido('?handler=CadastrarUnidade', formData, 'selecao-unidade-medida', function (res) {
          var elSelecao = document.getElementById('selecao-unidade-medida');
          elSelecao.add(new Option(res.nome + ' (' + res.sigla + ')', res.sigla, true, true));
          notificarSucesso('Sucesso', 'Unidade "' + res.nome + ' (' + res.sigla + ')" criada com sucesso!');
        });
      } catch (err) {
        alerta(err.message || 'Erro inesperado.');
      } finally {
        botaoSalvar.disabled = false;
        botaoSalvar.textContent = 'Salvar';
      }
    });
  }

  document.addEventListener('DOMContentLoaded', function () {
    var form = document.getElementById('formulario-produto');
    if (form) {
      form.addEventListener('submit', function () {
        renumerarLinhas();
      });
    }

    document.querySelectorAll('.botao-aba[data-aba]').forEach(function (botao) {
      botao.addEventListener('click', function () {
        alternarAba(botao.getAttribute('data-aba'), botao);
      });
    });

    document.body.addEventListener('click', function (e) {
      var target = e.target.closest('[data-acao]');
      if (!target) return;

      var action = target.getAttribute('data-acao');

      if (action === 'adicionar-variacao') {
        incluirLinhaVariacao();
      } else if (action === 'remover-linha') {
        removerLinha(target);
      } else if (action === 'fechar-cadastro-rapido') {
        fecharModalCadastroRapido();
      } else if (action === 'cadastro-rapido-unidade') {
        abrirCadastroRapidoUnidade();
      } else if (action === 'cadastro-rapido') {
        abrirCadastroRapido(
          target.getAttribute('data-field'),
          target.getAttribute('data-title'),
          target.getAttribute('data-handler'),
          target.getAttribute('data-select')
        );
      }
    });

    document.body.addEventListener('click', function (e) {
      if (e.target.id === 'cadastro-rapido-modal') {
        fecharModalCadastroRapido();
      }
    });
  });
})();
