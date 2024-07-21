using AutoMapper;
using WebApiNet8.DTOs;
using WebApiNet8.Entidades;

namespace WebApiNet8.Utilidades
{
    public class AutoMapperProfiles: Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<CrearGeneroDTO, Genero>();
            CreateMap<GeneroDTO, Genero>().ReverseMap();
        }
    }
}
