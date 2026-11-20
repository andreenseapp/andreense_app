(function () {
  var produtos = {};

  function carregarProdutos() {
    document.querySelectorAll('#seletorProduto option[data-variacoes]').forEach(function (opt) {
      if (opt.value) {
        produtos[opt.value] = JSON.parse(opt.dataset.variacoes);
      }
    });
  }

  function atualizarVariacoes(produtoId) {
    var sel = document.getElementById('Formulario_ProdutoVariacaoId');
    if (!sel) return;

    sel.replaceChildren();
    var placeholder = document.createElement('option');
    placeholder.value = '';
    placeholder.textContent = 'Selecione a variação';
    sel.appendChild(placeholder);

    var vars = produtos[produtoId] || [];
    vars.forEach(function (v) {
      var opt = document.createElement('option');
      opt.value = v.id;
      opt.textContent = v.desc;
      sel.appendChild(opt);
    });

    if (vars.length === 1) {
      sel.value = vars[0].id;
    }
  }

  function recarregarLocais(empresaId) {
    var form = document.getElementById('formularioEntrada');
    var base = form ? form.getAttribute('data-entrada-url') : window.location.pathname;
    window.location.href = base + '?empresaId=' + encodeURIComponent(empresaId);
  }

  function definirDataHoje(targetId) {
    var el = document.getElementById(targetId);
    if (!el) return;
    var d = new Date();
    el.value = d.getFullYear() + '-'
      + String(d.getMonth() + 1).padStart(2, '0') + '-'
      + String(d.getDate()).padStart(2, '0');
  }

  document.addEventListener('DOMContentLoaded', function () {
    carregarProdutos();

    var seletorProduto = document.getElementById('seletorProduto');
    if (seletorProduto) {
      seletorProduto.addEventListener('change', function () {
        atualizarVariacoes(seletorProduto.value);
      });
    }

    var seletorEmpresa = document.getElementById('Formulario_EmpresaId');
    if (seletorEmpresa) {
      seletorEmpresa.addEventListener('change', function () {
        recarregarLocais(seletorEmpresa.value);
      });
    }

    document.querySelectorAll('[data-data-hoje]').forEach(function (botao) {
      botao.addEventListener('click', function () {
        definirDataHoje(botao.getAttribute('data-data-hoje'));
      });
    });

    var formularioEntrada = document.getElementById('formularioEntrada');
    var variacaoAlvo = formularioEntrada ? formularioEntrada.getAttribute('data-variacao-id') : null;
    if (variacaoAlvo && variacaoAlvo !== '00000000-0000-0000-0000-000000000000') {
      var pIds = Object.keys(produtos);
      for (var i = 0; i < pIds.length; i++) {
        var pId = pIds[i];
        if (produtos[pId].some(function (v) { return v.id === variacaoAlvo; })) {
          if (seletorProduto) seletorProduto.value = pId;
          atualizarVariacoes(pId);
          setTimeout(function () {
            var selVar = document.getElementById('Formulario_ProdutoVariacaoId');
            if (selVar) selVar.value = variacaoAlvo;
          }, 50);
          break;
        }
      }
    }
  });
})();
