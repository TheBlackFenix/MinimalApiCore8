using Microsoft.AspNetCore.Identity;

namespace WebApiNet8.Repositorios
{
    public interface IRepositorioUsuarios
    {
        Task<IdentityUser> BuscarUsuarioPorEmail(string normalizedEmail);
        Task<string> Crear(IdentityUser usuario);
    }
}