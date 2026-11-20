(function () {
document.addEventListener('DOMContentLoaded', () => {
    const raiz = document.querySelector('[data-url-favoritar]');
    const urlFavoritar = raiz?.getAttribute('data-url-favoritar') || '/Imoveis/Acoes/Favoritar';
    const urlLogin = raiz?.getAttribute('data-url-login') || '/Contextos/Autenticacao/Login';
    const botoesFavoritar = document.querySelectorAll('[data-favoritar-btn]');

    botoesFavoritar.forEach(btn => {
        btn.addEventListener('click', async (e) => {
            e.preventDefault();
            e.stopPropagation();

            const imovelId = btn.getAttribute('data-imovel-id');
            if (!imovelId) return;

            const estavaFavoritado = btn.classList.contains('ativo');
            btn.classList.toggle('ativo');
            btn.disabled = true;

            try {
                const response = await window.requisitarApp(urlFavoritar, {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                        'Accept': 'application/json'
                    },
                    credentials: 'same-origin',
                    body: JSON.stringify({ imovelId: imovelId })
                });

                if (response.status === 401) {
                    btn.classList.toggle('ativo', estavaFavoritado);
                    const returnUrl = encodeURIComponent(window.location.pathname + window.location.search);
                    window.location.href = `${urlLogin}?returnUrl=${returnUrl}`;
                    return;
                }

                if (!response.ok) {
                    throw new Error('Erro ao favoritar');
                }

                const data = await response.json();
                if (data.sucesso === false) {
                    throw new Error(data.mensagem || 'Erro ao favoritar');
                }

                if (data.favoritado) {
                    btn.classList.add('ativo');
                } else {
                    btn.classList.remove('ativo');
                    var paginaFavoritos = document.querySelector('[data-pagina-favoritos]');
                    if (paginaFavoritos) {
                        var card = btn.closest('.anuncio-card');
                        if (card) {
                            card.remove();
                            var grid = document.getElementById('favoritos-grid');
                            if (grid && !grid.querySelector('.anuncio-card')) {
                                var vazio = document.getElementById('favoritos-vazio');
                                if (vazio) {
                                    vazio.hidden = false;
                                } else {
                                    var msg = paginaFavoritos.getAttribute('data-msg-vazio') || '';
                                    var explorar = paginaFavoritos.getAttribute('data-msg-explorar') || '';
                                    var urlImoveis = paginaFavoritos.getAttribute('data-url-imoveis') || '/Imoveis';
                                    var bloco = document.createElement('div');
                                    bloco.className = 'favoritos-gestao-vazio';
                                    bloco.id = 'favoritos-vazio';
                                    bloco.innerHTML = '<p>' + msg + '</p><a class="favoritos-gestao-explorar" href="' + urlImoveis + '">' + explorar + '</a>';
                                    grid.replaceWith(bloco);
                                }
                            }
                        }
                    }
                }
            } catch (err) {
                console.error(err);
                btn.classList.toggle('ativo', estavaFavoritado);

                if (window.NotificarErro) {
                    window.NotificarErro('Não foi possível atualizar o favorito.');
                }
            } finally {
                btn.disabled = false;
            }
        });
    });
});

})();
