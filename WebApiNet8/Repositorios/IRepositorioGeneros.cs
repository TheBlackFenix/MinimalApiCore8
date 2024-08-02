using WebApiNet8.Entidades;

namespace WebApiNet8.Repositorios
{
    public interface IRepositorioGeneros
    {
        Task<int> CrearGenero(Genero genero);
        Task<Genero?> ObtenerGeneroPorId(int id);
        Task<List<Genero>> ObtenerGeneros();
        Task<bool> Existe(int id);
        Task ActualizarGenero(Genero genero);
        Task EliminarGenero(int id);
        Task<List<int>> Existen(List<int> ids);
        Task<bool> Existe(int idGenero, string nombreGenero);
    }
}