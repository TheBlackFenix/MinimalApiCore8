using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OutputCaching;
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

            group.MapPut("/", ActualizarGenero);

            group.MapDelete("/{id}", EliminarGenero);

            return group;
        }

        static async Task<Ok<List<Genero>>> ObtenerGeneros(IRepositorioGeneros repositorioGeneros)
        {
            var generos = await repositorioGeneros.ObtenerGeneros();
            return TypedResults.Ok(generos);
        }

        static async Task<Results<Ok<Genero>, NotFound>> ObtenerGenerosPorId(int id, IRepositorioGeneros repositorioGeneros)
        {
            var genero = await repositorioGeneros.ObtenerGeneroPorId(id);
            if (genero is null)
            {
                return TypedResults.NotFound();
            }
            return TypedResults.Ok(genero);
        }

        static async Task<Created<Genero>> CrearGenero(Genero genero, IRepositorioGeneros repositorioGeneros, IOutputCacheStore outputCacheStore)
        {
            var id = await repositorioGeneros.CrearGenero(genero);
            await outputCacheStore.EvictByTagAsync("generos-list", default);
            return TypedResults.Created($"/generos/{id}", genero);
        }

        static async Task<Results<NoContent, NotFound>> ActualizarGenero(Genero genero, IRepositorioGeneros repositorioGeneros, IOutputCacheStore outputCacheStore)
        {
            var existe = await repositorioGeneros.Existe(genero.IdGenero);
            if (!existe)
            {
                return TypedResults.NotFound();
            }
            else
            {
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
