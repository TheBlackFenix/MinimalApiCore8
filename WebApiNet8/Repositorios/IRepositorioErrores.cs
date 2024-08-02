using WebApiNet8.Entidades;

namespace WebApiNet8.Repositorios
{
    public interface IRepositorioErrores
    {
        Task CrearLogError(LogError error);
    }
}