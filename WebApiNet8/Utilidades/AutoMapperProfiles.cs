using AutoMapper;
using WebApiNet8.DTOs.Actores;
using WebApiNet8.DTOs.Generos;
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
        }
    }
}
