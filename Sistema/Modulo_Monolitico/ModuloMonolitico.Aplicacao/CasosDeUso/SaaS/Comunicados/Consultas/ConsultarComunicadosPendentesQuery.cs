using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

using ModuloMonolitico.Dominio.InterfacesDeRepositorio;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;

using System.Collections.Generic;

public class ComunicadoSaaSDto
{
    public Guid Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Mensagem { get; set; } = string.Empty;
    public string Tipo { get; set; } = "aviso";
    public string? ModuloAlvo { get; set; }
    public DateTime? DataExpiracao { get; set; }
    
    public List<OpcaoComunicadoSaaSDto> Opcoes { get; set; } = new();
}

public class OpcaoComunicadoSaaSDto
{
    public Guid Id { get; set; }
    public string Texto { get; set; } = string.Empty;
    public int Ordem { get; set; }
}


public class ConsultarComunicadosPendentesQuery : IRequest<ComunicadoSaaSDto?>
{
    public Guid ContaId { get; set; }
    public Guid UsuarioId { get; set; }
    public string ModuloAtual { get; set; } = string.Empty;
}

public class ConsultarComunicadosPendentesQueryHandler : IRequestHandler<ConsultarComunicadosPendentesQuery, ComunicadoSaaSDto?>
{
    private readonly IComunicadoSaaSRepositorio _repositorio;

    public ConsultarComunicadosPendentesQueryHandler(IComunicadoSaaSRepositorio repositorio)
    {
        _repositorio = repositorio;
    }

    public async Task<ComunicadoSaaSDto?> Handle(ConsultarComunicadosPendentesQuery comando, CancellationToken cancellationToken)
    {
        var entidade = await _repositorio.ObterPrimeiroPendenteParaUsuarioAsync(comando.ContaId, comando.UsuarioId, comando.ModuloAtual);
        
        if (entidade == null) return null;

        return new ComunicadoSaaSDto
        {
            Id = entidade.Id,
            Titulo = entidade.Titulo,
            Mensagem = entidade.Mensagem,
            Tipo = entidade.Tipo,
            ModuloAlvo = entidade.ModuloAlvo,
            DataExpiracao = entidade.DataExpiracao,
            Opcoes = entidade.Opcoes.Select(o => new OpcaoComunicadoSaaSDto
            {
                Id = o.Id,
                Texto = o.Texto,
                Ordem = o.Ordem
            }).ToList()
        };
    }
}


