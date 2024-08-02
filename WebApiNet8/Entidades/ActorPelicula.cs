namespace WebApiNet8.Entidades
{
    public class ActorPelicula
    {
        public int IdActor { get; set; }
        public Actor Actor { get; set; } = null!;
        public int IdPelicula { get; set; }
        public Pelicula Pelicula { get; set; } = null!;
        public int Orden { get; set; }
        public string Personaje { get; set; } = null!;

    }
}
