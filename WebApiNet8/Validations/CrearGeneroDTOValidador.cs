using FluentValidation;
using WebApiNet8.DTOs.Generos;
using WebApiNet8.Repositorios;

namespace WebApiNet8.Validations
{
    public class CrearGeneroDTOValidador : AbstractValidator<CrearGeneroDTO>
    {
        public CrearGeneroDTOValidador(IRepositorioGeneros repositorioGeneros, IHttpContextAccessor contextAccessor)
        {
            var valor = contextAccessor.HttpContext.Request.RouteValues["id"];
            var idGenero = 0;
            if (valor is string valorString)
            {
                int.TryParse(valorString, out idGenero);
            }
            RuleFor(x => x.NombreGenero).NotNull().NotEmpty().WithMessage(Utilidades.CampoRequerido)
                                        .MaximumLength(50).WithMessage(Utilidades.CampoLongitudMaxima)
                                        .MinimumLength(3).WithMessage(Utilidades.CampoLongitudMinima)
                                        .MustAsync(async(nombreGenero,_)=>
                                        {
                                            var existe = await repositorioGeneros.Existe(idGenero: idGenero, nombreGenero);
                                            return !existe;
                                        }).WithMessage(g => $"Ya existe un género con el nombre {g.NombreGenero}");

        }
    }
}
