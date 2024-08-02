using FluentValidation;
using WebApiNet8.DTOs.Actores;

namespace WebApiNet8.Validations
{
    public class CrearActorDTOValidator : AbstractValidator<CrearActorDTO>
    {
        public CrearActorDTOValidator()
        {
            RuleFor(x => x.NombreActor).NotNull().NotEmpty().WithMessage(Utilidades.CampoRequerido)
                                    .MaximumLength(200).WithMessage(Utilidades.CampoLongitudMaxima)
                                    .MinimumLength(3).WithMessage(Utilidades.CampoLongitudMinima);
            var fechaMinima = new DateTime(1890, 1, 1);
            RuleFor(x => x.FechaNacimiento).NotNull().NotEmpty().WithMessage(Utilidades.CampoRequerido)
                                    .GreaterThan(fechaMinima).WithMessage(Utilidades.MayorQue);

                
        }
    }
}
