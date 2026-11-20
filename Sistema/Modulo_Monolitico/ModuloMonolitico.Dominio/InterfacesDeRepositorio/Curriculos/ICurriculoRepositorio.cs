using ModuloMonolitico.Dominio.Compartilhado;
using ModuloMonolitico.Dominio.Curriculos.DTOs;
using ModuloMonolitico.Dominio.Entidades.Curriculos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ModuloMonolitico.Dominio.InterfacesDeRepositorio.Curriculos;

public interface ICurriculoRepositorio
{
    Task<ResultadoPaginado<CurriculoResumoDto>> ListarComFiltrosAsync(Guid contaId, FiltroCurriculo filtro);
    Task<CurriculoDetalheDto?> ObterDetalhePublicoAsync(Guid contaId, Guid curriculoId);
    Task<CurriculoCompletoDto?> ObterCompletoDoUsuarioAsync(Guid contaId, Guid usuarioId);
    Task<Guid> CadastrarOuObterIdAsync(Guid contaId, Guid usuarioId);
    Task AtualizarDadosPrincipaisAsync(Curriculo curriculo);
    Task AlterarVisibilidadeAsync(Guid contaId, Guid usuarioId, bool visivel);

    Task<Guid> CadastrarExperienciaAsync(CurriculoExperiencia experiencia);
    Task AtualizarExperienciaAsync(CurriculoExperiencia experiencia);
    Task DeletarFisicoExperienciaAsync(Guid contaId, Guid curriculoId, Guid id);

    Task<Guid> CadastrarFormacaoAsync(CurriculoFormacao formacao);
    Task AtualizarFormacaoAsync(CurriculoFormacao formacao);
    Task DeletarFisicoFormacaoAsync(Guid contaId, Guid curriculoId, Guid id);

    Task<Guid> CadastrarCursoAsync(CurriculoCurso curso);
    Task AtualizarCursoAsync(CurriculoCurso curso);
    Task DeletarFisicoCursoAsync(Guid contaId, Guid curriculoId, Guid id);

    Task SubstituirIdiomasAsync(Guid curriculoId, Guid contaId, IEnumerable<CurriculoIdioma> idiomas);
    Task SubstituirHabilidadesAsync(Guid curriculoId, Guid contaId, IEnumerable<string> habilidades);

    Task<ExperienciaDto?> ObterExperienciaPorIdAsync(Guid contaId, Guid id);
    Task<FormacaoDto?> ObterFormacaoPorIdAsync(Guid contaId, Guid id);
    Task<CursoDto?> ObterCursoPorIdAsync(Guid contaId, Guid id);
}
