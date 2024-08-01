namespace WebApiNet8.DTOs.Comentarios
{
    public class ComentarioDTO
    {
        public int IdComentario { get; set; }
        public string Cuerpo { get; set; } = null!;
        public int IdPelicula { get; set; }
    }
}
