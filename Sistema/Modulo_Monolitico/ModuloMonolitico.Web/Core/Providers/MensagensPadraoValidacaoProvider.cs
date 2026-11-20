using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using System.ComponentModel.DataAnnotations;

namespace ModuloMonolitico.Web.Core.Providers;

public class MensagensPadraoValidacaoProvider : IValidationMetadataProvider
{
    public void CreateValidationMetadata(ValidationMetadataProviderContext context)
    {
        foreach (var attribute in context.ValidationMetadata.ValidatorMetadata)
        {
            if (attribute is RequiredAttribute required)
            {
                if (required.ErrorMessage == null && required.ErrorMessageResourceName == null)
                    required.ErrorMessage = "O campo {0} é obrigatório.";
            }
            else if (attribute is StringLengthAttribute stringLength)
            {
                if (stringLength.ErrorMessage == null && stringLength.ErrorMessageResourceName == null)
                {
                    if (stringLength.MinimumLength > 0)
                        stringLength.ErrorMessage = "O campo {0} deve ter entre {2} e {1} caracteres.";
                    else
                        stringLength.ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.";
                }
            }
            else if (attribute is MaxLengthAttribute maxLength)
            {
                if (maxLength.ErrorMessage == null && maxLength.ErrorMessageResourceName == null)
                    maxLength.ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.";
            }
            else if (attribute is MinLengthAttribute minLength)
            {
                if (minLength.ErrorMessage == null && minLength.ErrorMessageResourceName == null)
                    minLength.ErrorMessage = "O campo {0} deve ter no mínimo {1} caracteres.";
            }
            else if (attribute is EmailAddressAttribute email)
            {
                if (email.ErrorMessage == null && email.ErrorMessageResourceName == null)
                    email.ErrorMessage = "O campo {0} não é um e-mail válido.";
            }
            else if (attribute is CompareAttribute compare)
            {
                if (compare.ErrorMessage == null && compare.ErrorMessageResourceName == null)
                    compare.ErrorMessage = "Os campos {0} e {1} não conferem.";
            }
            else if (attribute is RegularExpressionAttribute regex)
            {
                if (regex.ErrorMessage == null && regex.ErrorMessageResourceName == null)
                    regex.ErrorMessage = "O formato do campo {0} é inválido.";
            }
            else if (attribute is RangeAttribute range)
            {
                if (range.ErrorMessage == null && range.ErrorMessageResourceName == null)
                    range.ErrorMessage = "O campo {0} deve estar entre {1} e {2}.";
            }
            else if (attribute is PhoneAttribute phone)
            {
                if (phone.ErrorMessage == null && phone.ErrorMessageResourceName == null)
                    phone.ErrorMessage = "O campo {0} não é um telefone válido.";
            }
            else if (attribute is UrlAttribute url)
            {
                if (url.ErrorMessage == null && url.ErrorMessageResourceName == null)
                    url.ErrorMessage = "O campo {0} não é uma URL válida.";
            }
            else if (attribute is CreditCardAttribute creditCard)
            {
                if (creditCard.ErrorMessage == null && creditCard.ErrorMessageResourceName == null)
                    creditCard.ErrorMessage = "O campo {0} não é um cartão válido.";
            }
        }
    }
}
