namespace WebApiNet8.Entidades
{
    public class Actor
    {
        public int IdActor { get; set; }
        public string NombreActor { get; set; } = null!;
        public DateTime? FechaNacimiento { get; set; }
        public string? Foto { get; set; }
        
    }
}
