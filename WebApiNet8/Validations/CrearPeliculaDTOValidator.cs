using FluentValidation;
using WebApiNet8.DTOs.Peliculas;

namespace WebApiNet8.Validations
{
    public class CrearPeliculaDTOValidator : AbstractValidator<CrearPeliculaDTO>
    {
        public CrearPeliculaDTOValidator()
        {
            RuleFor(x => x.TituloPelicula)
                .NotEmpty().WithMessage(Utilidades.CampoRequerido)
                .MaximumLength(200).WithMessage(Utilidades.CampoLongitudMaxima);

            RuleFor(x => x.EnCines)
                .NotNull().WithMessage(Utilidades.CampoRequerido);

            RuleFor(x => x.FechaLanzamiento)
                .NotNull().WithMessage(Utilidades.CampoRequerido);

            
        }
    }
}
