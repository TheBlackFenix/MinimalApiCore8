namespace WebApiNet8.Entidades
{
    public class Comentario
    {
        public int IdComentario { get; set; }
        public string Cuerpo { get; set; } = null!;
        public int IdPelicula { get; set; }
    }
}
