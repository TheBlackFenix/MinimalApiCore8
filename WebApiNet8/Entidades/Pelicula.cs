namespace WebApiNet8.Entidades
{
    public class Pelicula
    {
        public int IdPelicula { get; set; }
        public string TituloPelicula { get; set; } = null!;
        public bool EnCines { get; set; }
        public DateTime FechaLanzamiento { get; set; }
        public string Poster { get; set; } = null!;
        public List<Comentario> Comentarios { get; set; } = new List<Comentario>();
        public List<GeneroPelicula> GenerosPeliculas { get; set; } = new List<GeneroPelicula>();
        public List<ActorPelicula> ActoresPeliculas { get; set; } = new List<ActorPelicula>();
    }

}
