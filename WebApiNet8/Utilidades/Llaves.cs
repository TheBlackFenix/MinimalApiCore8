using Microsoft.IdentityModel.Tokens;

namespace WebApiNet8.Utilidades
{
    public static class Llaves
    {
        public const string IssuerPropio = "BlackFenixIssuer";
        private const string seccionLlaves = "Authentication:Schemes:Bearer:SigningKeys";
        private const string SeccionLlaves_Emisor = "Issuer";
        private const string SeccionLlaves_valor = "Value";

        public static IEnumerable<SecurityKey> ObtenerLlave(IConfiguration configuration)
            => ObtenerLlave(configuration, IssuerPropio);

        public static IEnumerable<SecurityKey> ObtenerLlave(IConfiguration configuration, string issuer)
        {
            var signInKey = configuration.GetSection(seccionLlaves)
                .GetChildren()
                .SingleOrDefault(llave => llave[SeccionLlaves_Emisor] == issuer);
            if (signInKey is not null && signInKey[SeccionLlaves_valor] is string valorLlave)
            {
                yield return new SymmetricSecurityKey(Convert.FromBase64String(valorLlave));
            }
        }

        public static IEnumerable<SecurityKey> ObtenerLlaves(IConfiguration configuration)
        {
            var signInKeys = configuration.GetSection(seccionLlaves)
                .GetChildren();
            foreach (var signInKey in signInKeys)
            {
                if (signInKey[SeccionLlaves_valor] is string valorLlave)
                {
                    yield return new SymmetricSecurityKey(Convert.FromBase64String(valorLlave));
                }
            }
        }
    }
}
