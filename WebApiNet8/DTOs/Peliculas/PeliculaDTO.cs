using WebApiNet8.DTOs.Actores;
using WebApiNet8.DTOs.Comentarios;
using WebApiNet8.DTOs.Generos;
using WebApiNet8.Entidades;

namespace WebApiNet8.DTOs.Peliculas
{
    public class PeliculaDTO
    {
        public int IdPelicula { get; set; }
        public string TituloPelicula { get; set; } = null!;
        public bool EnCines { get; set; }
        public DateTime FechaLanzamiento { get; set; }
        public string Poster { get; set; } = null!;
        public List<ComentarioDTO> Comentarios { get; set; } = new List<ComentarioDTO>();
        public List<GeneroDTO> Generos { get; set; } = new List<GeneroDTO>();
        public List<ActorPeliculaDTO> Actores { get; set; } = new List<ActorPeliculaDTO>();
    }
}
