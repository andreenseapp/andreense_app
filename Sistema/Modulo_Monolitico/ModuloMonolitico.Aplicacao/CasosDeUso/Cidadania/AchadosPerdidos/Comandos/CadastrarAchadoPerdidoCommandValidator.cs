using FluentValidation;
using ModuloMonolitico.Dominio.Configuracoes;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;

public class CadastrarAchadoPerdidoCommandValidator : AbstractValidator<CadastrarAchadoPerdidoCommand>
{
    public CadastrarAchadoPerdidoCommandValidator()
    {
        RuleFor(x => x.Tipo)
            .NotEmpty().WithMessage(MensagensSistema.Cidadania.TipoObrigatorio)
            .Must(t => t == "achado" || t == "perdido").WithMessage(MensagensSistema.Cidadania.TipoInvalido);

        RuleFor(x => x.Titulo)
            .NotEmpty().WithMessage(MensagensSistema.Cidadania.TituloObrigatorio)
            .MaximumLength(100).WithMessage(MensagensSistema.Cidadania.TituloTamanhoMaximo);

        RuleFor(x => x.Descricao)
            .MaximumLength(1000).WithMessage(MensagensSistema.Cidadania.DescricaoTamanhoMaximo);
            
        RuleFor(x => x.ContatoInfo)
            .MaximumLength(200).WithMessage(MensagensSistema.Cidadania.ContatoTamanhoMaximo);
            
        RuleFor(x => x.LocalVisto)
            .MaximumLength(200).WithMessage(MensagensSistema.Cidadania.LocalVistoTamanhoMaximo);
    }
}
