using WebApiNet8.Entidades;

namespace WebApiNet8.Repositorios
{
    public interface IRepositorioComentarios
    {
        Task ActualizarComentario(Comentario comentario);
        Task<int> CrearComentario(Comentario comentario);
        Task EliminarComentario(int id);
        Task<bool> Existe(int id);
        Task<Comentario?> ObtenerComentarioPorId(int id);
        Task<List<Comentario>> ObtenerTodos(int id);
    }
}