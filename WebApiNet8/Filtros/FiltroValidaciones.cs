
using FluentValidation;
using WebApiNet8.DTOs.Generos;

namespace WebApiNet8.Filtros
{
    public class FiltroValidaciones<T> : IEndpointFilter
    {
        public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
        {
            var validador = context.HttpContext.RequestServices.GetService<IValidator<T>>();
            if (validador is null)
            {
                return await next(context);
            }

            var entiidad = context.Arguments.OfType<T>().FirstOrDefault();
            if (entiidad is null)
            {
                return TypedResults.Problem("No se encontró la entidad a validar.");
            }
            var resultadoValidacion = await validador.ValidateAsync(entiidad);
            if (!resultadoValidacion.IsValid)
            {
                return TypedResults.ValidationProblem(resultadoValidacion.ToDictionary());
            }

            return await next(context);
        }
    }
}
