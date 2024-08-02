using FluentValidation;
using WebApiNet8.DTOs.Comentarios;

namespace WebApiNet8.Validations
{
    public class CrearComentarioDTOValidator :AbstractValidator<CrearComentarioDTO>
    {
        public CrearComentarioDTOValidator()
        {
            RuleFor(x => x.Cuerpo)
                .NotEmpty().WithMessage(Utilidades.CampoRequerido)
                .MaximumLength(500).WithMessage(Utilidades.CampoLongitudMaxima);
        }
    }
}
