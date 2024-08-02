using WebApiNet8.DTOs.Paginacion;
using WebApiNet8.Entidades;

namespace WebApiNet8.Repositorios
{
    public interface IRepositorioPeliculas
    {
        Task ActualizarPelicula(Pelicula pelicula);
        Task AsignarActores(int id, List<ActorPelicula> actores);
        Task AsignarGeneros(int id, List<int> generosIds);
        Task<int> CrearPelicula(Pelicula pelicula);
        Task EliminarPelicula(int id);
        Task<bool> Existe(int id);
        Task<Pelicula?> ObtenerPeliculaPorId(int id);
        Task<List<Pelicula>> ObtenerTodos(PaginacionDTO paginacion);
    }
}