using MediatR;
using Microsoft.AspNetCore.Mvc;
using ModuloMonolitico.Aplicacao.CasosDeUso;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Web.Pages.Shared;
using Microsoft.Extensions.Logging;

namespace ModuloMonolitico.Web.Pages.InfraAdmin.Documentos
{
    public class IndexModel : AppBasePageModel
    {
        private readonly IMediator _mediator;
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(IMediator mediator, ILogger<IndexModel> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        protected override bool ExigeEmpresaAtiva => false;

        public IEnumerable<DocumentoLegal> Documentos { get; private set; } = [];

        [TempData] public string? MensagemSucesso { get; set; }
        [TempData] public string? MensagemErro { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (!UsuarioEhMaster)
            {
                _logger.LogWarning("Acesso negado à listagem de documentos legais para o usuário '{Email}'. Não é Master.", UsuarioEmail);
                return Forbid();
            }

            try
            {
                Documentos = await _mediator.Send(new ListarDocumentosLegaisQuery());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro crítico ao carregar documentos legais para o usuário '{Email}'.", UsuarioEmail);
                MensagemErro = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.InfraAdmin.Documentos.ErroCarregarLista;
            }
            
            return Page();
        }
    }
}
