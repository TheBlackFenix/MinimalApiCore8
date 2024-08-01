using WebApiNet8.DTOs.Paginacion;
using WebApiNet8.Entidades;

namespace WebApiNet8.Repositorios
{
    public interface IRepositorioPeliculas
    {
        Task ActualizarPelicula(Pelicula pelicula);
        Task<int> CrearPelicula(Pelicula pelicula);
        Task EliminarPelicula(int id);
        Task<bool> Existe(int id);
        Task<Pelicula?> ObtenerPeliculaPorId(int id);
        Task<List<Pelicula>> ObtenerTodos(PaginacionDTO paginacion);
    }
}