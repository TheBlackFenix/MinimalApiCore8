
using AutoMapper;
using WebApiNet8.Repositorios;

namespace WebApiNet8.Filtros
{
    public class FiltroEjemplo : IEndpointFilter
    {
        public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
        {
            //var param1 = (IRepositorioGeneros)context.Arguments[0]!;
            //var param2 = (int)context.Arguments[1]!;
            //var param3 = (IMapper)context.Arguments[2]!;
            var paramRepositorioGeneros = context.Arguments.OfType<IRepositorioGeneros>().FirstOrDefault();
            var paramNumero = context.Arguments.OfType<int>().FirstOrDefault();
            var paramMapper = context.Arguments.OfType<IMapper>().FirstOrDefault();
            var result = await next(context);
            return result;
        }
    }
}
