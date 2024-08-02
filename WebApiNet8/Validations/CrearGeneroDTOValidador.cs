using FluentValidation;
using WebApiNet8.DTOs.Generos;

namespace WebApiNet8.Validations
{
    public class CrearGeneroDTOValidador : AbstractValidator<CrearGeneroDTO>
    {
        public CrearGeneroDTOValidador()
        {
            RuleFor(x => x.NombreGenero).NotNull().NotEmpty().WithMessage("El campo {PropertyName} es Requerido")
                                        .MaximumLength(50).WithMessage("El campo {PropertyName} no puede tener más de {MaxLength} caracteres")
                                        .MinimumLength(3).WithMessage("El campo {PropertyName} debe tener al menos {MinLength} caracteres");

        }
    }
}
