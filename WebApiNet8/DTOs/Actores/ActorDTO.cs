namespace WebApiNet8.DTOs.Actores
{
    public class ActorDTO
    {
        public int IdActor { get; set; }
        public string NombreActor { get; set; } = null!;
        public DateTime FechaNacimiento { get; set; }
        public string? Foto { get; set; }
    }
}
