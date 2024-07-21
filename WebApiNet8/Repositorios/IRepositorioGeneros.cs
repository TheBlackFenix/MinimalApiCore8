using WebApiNet8.Entidades;

namespace WebApiNet8.Repositorios
{
    public interface IRepositorioGeneros
    {
        Task<int> CrearGenero(Genero genero);
    }
}