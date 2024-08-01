using AutoMapper;
using WebApiNet8.DTOs.Actores;
using WebApiNet8.DTOs.Comentarios;
using WebApiNet8.DTOs.Generos;
using WebApiNet8.DTOs.Peliculas;
using WebApiNet8.Entidades;

namespace WebApiNet8.Utilidades
{
    public class AutoMapperProfiles: Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<CrearGeneroDTO, Genero>();
            CreateMap<GeneroDTO, Genero>().ReverseMap();

            CreateMap<CrearActorDTO, Actor>()
                .ForMember(x => x.Foto, opciones => opciones.Ignore());
            CreateMap<Actor, ActorDTO>().ReverseMap();

            CreateMap<CrearPeliculaDTO, Pelicula>()
                .ForMember(x => x.Poster, opciones => opciones.Ignore());
            CreateMap<Pelicula, PeliculaDTO>().ReverseMap();

            CreateMap<CrearComentarioDTO, Comentario>();
            CreateMap<Comentario, ComentarioDTO>().ReverseMap();


        }
    }
}
