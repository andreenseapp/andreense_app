namespace ModuloMonolitico.Web.Extensions;

public static class AvatarIniciais
{
    public static string DeNomeOuEmail(string? nome, string? email, string fallback = "U")
    {
        var nomeTrim = nome?.Trim();
        if (!string.IsNullOrEmpty(nomeTrim) && !nomeTrim.Equals("?", StringComparison.Ordinal))
        {
            var partes = nomeTrim.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (partes.Length >= 2)
                return $"{char.ToUpperInvariant(partes[0][0])}{char.ToUpperInvariant(partes[1][0])}";
            if (partes[0].Length > 0)
                return char.ToUpperInvariant(partes[0][0]).ToString();
        }

        var emailTrim = email?.Trim();
        if (!string.IsNullOrEmpty(emailTrim))
        {
            var arroba = emailTrim.IndexOf('@');
            var local = arroba > 0 ? emailTrim[..arroba] : emailTrim;
            if (local.Length > 0)
                return char.ToUpperInvariant(local[0]).ToString();
        }

        return fallback;
    }
}
