namespace WebApiNet8.DTOs.Peliculas
{
    public class CrearPeliculaDTO
    {
        public string TituloPelicula { get; set; } = null!;
        public bool EnCines { get; set; }
        public DateTime FechaLanzamiento { get; set; }
        public IFormFile? Poster { get; set; } 
    }
}
