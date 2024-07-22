using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OutputCaching;
using WebApiNet8.DTOs.Generos;
using WebApiNet8.Entidades;
using WebApiNet8.Repositorios;

namespace WebApiNet8.EndPoints
{
    public static class GenerosEndpoints
    {
        public static RouteGroupBuilder MapGeneros(this RouteGroupBuilder group)
        {

            group.MapGet("/", ObtenerGeneros).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("generos-list")); // Se Agrega cache de 60 segundos

            group.MapGet("/{id}", ObtenerGenerosPorId);

            group.MapPost("/", CrearGenero);

            group.MapPut("/{id}", ActualizarGenero);

            group.MapDelete("/{id}", EliminarGenero);

            return group;
        }

        static async Task<Ok<List<GeneroDTO>>> ObtenerGeneros(IRepositorioGeneros repositorioGeneros, IMapper mapper)
        {
            var generos = await repositorioGeneros.ObtenerGeneros();
            var generosDto = mapper.Map<List<GeneroDTO>>(generos);
            return TypedResults.Ok(generosDto);
        }

        static async Task<Results<Ok<GeneroDTO>, NotFound>> ObtenerGenerosPorId(int id, IRepositorioGeneros repositorioGeneros, IMapper mapper)
        {
            var genero = await repositorioGeneros.ObtenerGeneroPorId(id);
            if (genero is null)
            {
                return TypedResults.NotFound();
            }
            var generoDTO = mapper.Map<GeneroDTO>(genero);
            return TypedResults.Ok(generoDTO);
        }

        static async Task<Created<GeneroDTO>> CrearGenero(CrearGeneroDTO crearGeneroDTO, IRepositorioGeneros repositorioGeneros, IOutputCacheStore outputCacheStore, IMapper mapper)
        {
            var genero = mapper.Map<Genero>(crearGeneroDTO);
            var id = await repositorioGeneros.CrearGenero(genero);
            await outputCacheStore.EvictByTagAsync("generos-list", default);
            var generoDTO = mapper.Map<GeneroDTO>(genero);
            generoDTO.IdGenero = id;
            return TypedResults.Created($"/generos/{id}", generoDTO);
        }

        static async Task<Results<NoContent, NotFound>> ActualizarGenero(int id, CrearGeneroDTO crearGeneroDTO, IRepositorioGeneros repositorioGeneros, IOutputCacheStore outputCacheStore, IMapper mapper)
        {
            var existe = await repositorioGeneros.Existe(id);
            if (!existe)
            {
                return TypedResults.NotFound();
            }
            else
            {
                var genero = mapper.Map<Genero>(crearGeneroDTO);
                genero.IdGenero = id;
                await repositorioGeneros.ActualizarGenero(genero);
                await outputCacheStore.EvictByTagAsync("generos-list", default);
                return TypedResults.NoContent();
            }
        }

        static async Task<Results<NoContent, NotFound>> EliminarGenero(int id, IRepositorioGeneros repositorioGeneros, IOutputCacheStore outputCacheStore)
        {
            var existe = await repositorioGeneros.Existe(id);
            if (!existe)
            {
                return TypedResults.NotFound();
            }
            else
            {
                await repositorioGeneros.EliminarGenero(id);
                await outputCacheStore.EvictByTagAsync("generos-list", default);
                return TypedResults.NoContent();
            }
        }
    }

}
