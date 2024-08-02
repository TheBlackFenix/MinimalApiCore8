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

            CreateMap<PeliculaDTO, Pelicula>();
            CreateMap<Pelicula, PeliculaDTO>()
                .ForMember(x => x.Generos, entidad =>
                entidad.MapFrom(p =>
                    p.GenerosPeliculas.Select(gp =>
                        new GeneroDTO { IdGenero = gp.IdGenero, NombreGenero = gp.Genero.NombreGenero })))
                .ForMember(x => x.Actores, entidad =>
                entidad.MapFrom(p =>
                    p.ActoresPeliculas.Select(ap =>
                        new ActorPeliculaDTO { IdActor = ap.IdActor, Personaje = ap.Personaje, NombreActor = ap.Actor.NombreActor })));

            CreateMap<CrearPeliculaDTO, Pelicula>()
                .ForMember(x => x.Poster, opciones => opciones.Ignore());


            CreateMap<CrearComentarioDTO, Comentario>();
            CreateMap<Comentario, ComentarioDTO>().ReverseMap();

            CreateMap<AsignarActorPeliculaDTO, ActorPelicula>();

        }
    }
}
