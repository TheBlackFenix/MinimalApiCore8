namespace WebApiNet8.DTOs.Actores
{
    public class CrearActorDTO
    {
        public string NombreActor { get; set; } = null!;
        public DateTime FechaNacimiento { get; set; }
        public IFormFile? Foto { get; set; }
    }
}
