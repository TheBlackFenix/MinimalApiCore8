namespace WebApiNet8.Entidades
{
    public class GeneroPelicula
    {
        public int IdPelicula { get; set; }
        public int IdGenero { get; set; }
        public Genero Genero { get; set; } = null!;
        public Pelicula Pelicula { get; set; } = null!;
    }
}
