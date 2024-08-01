namespace WebApiNet8.DTOs.Peliculas
{
    public class PeliculaDTO
    {
        public int IdPelicula { get; set; }
        public string TituloPelicula { get; set; } = null!;
        public bool EnCines { get; set; }
        public DateTime FechaLanzamiento { get; set; }
        public string Poster { get; set; } = null!;
    }
}
