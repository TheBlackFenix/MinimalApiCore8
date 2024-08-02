using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OutputCaching;
using WebApiNet8.DTOs.Comentarios;
using WebApiNet8.DTOs.Generos;
using WebApiNet8.Entidades;
using WebApiNet8.Repositorios;

namespace WebApiNet8.EndPoints
{
    public static class ComentariosEndpoints
    {
        private static readonly string cacheName = "comentarios-list";
        public static RouteGroupBuilder MapComentarios(this RouteGroupBuilder group)
        {

            group.MapGet("/", ObtenerComentariosPorPeliculaId)
                .CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60))
                .Tag(cacheName)
                .SetVaryByRouteValue(new string[] {"idPelicula"})); // Se Agrega cache de 60 segundos

            group.MapGet("/{id}", ObtenerComentarioPorId);

            group.MapPost("/", CrearComentario);

            group.MapPut("/{id}", ActualizarComentario);

            group.MapDelete("/{id}", EliminArComentarios);

            return group;
        }

        static async Task<Results<Ok<List<ComentarioDTO>>, NotFound>> ObtenerComentariosPorPeliculaId(int idPelicula, IRepositorioComentarios repositorioComentarios, IRepositorioPeliculas repositorioPeliculas, IMapper mapper)
        {
            if (!await repositorioPeliculas.Existe(idPelicula))
            {
                return TypedResults.NotFound();
            }

            var comentarios = await repositorioComentarios.ObtenerTodos(idPelicula);
            var comentariosDTO = mapper.Map<List<ComentarioDTO>>(comentarios);
            return TypedResults.Ok(comentariosDTO);
        }

        static async Task<Results<Ok<ComentarioDTO>, NotFound>> ObtenerComentarioPorId(int idPelicula, int id, IRepositorioComentarios repositorioComentarios, IMapper mapper)
        {
            var comentario = await repositorioComentarios.ObtenerComentarioPorId(id);
            if(comentario is null)
            {
                return TypedResults.NotFound();
            }
            
            var comnetarioDTO = mapper.Map<ComentarioDTO>(comentario);
            return TypedResults.Ok(comnetarioDTO);
        }

        static async Task<Results<Created<ComentarioDTO>,NotFound>> CrearComentario(int idPelicula, CrearComentarioDTO crearComentarioDTO, IRepositorioPeliculas repositorioPeliculas, IRepositorioComentarios repositorioComentarios, IOutputCacheStore outputCacheStore, IMapper mapper)
        {
            if(!await repositorioPeliculas.Existe(idPelicula))
            {
                return TypedResults.NotFound();
            }
            var comentario = mapper.Map<Comentario>(crearComentarioDTO);
            comentario.IdPelicula = idPelicula;
            var id = await repositorioComentarios.CrearComentario(comentario);
            await outputCacheStore.EvictByTagAsync(cacheName, default);
            var comentarioDTO = mapper.Map<ComentarioDTO>(comentario);
            comentarioDTO.IdComentario = id;
            return TypedResults.Created($"/comentarios/{id}", comentarioDTO);
        }

        static async Task<Results<NoContent, NotFound>> ActualizarComentario(int idPelicula,int id, CrearComentarioDTO crearComentarioDTO, IRepositorioComentarios repositorioComentarios, IRepositorioPeliculas repositorioPeliculas, IOutputCacheStore outputCacheStore, IMapper mapper)
        {
            if(!await repositorioPeliculas.Existe(idPelicula))
            {
                return TypedResults.NotFound();
            }
            if (!await repositorioComentarios.Existe(id))
            {
                return TypedResults.NotFound();
            }
            
            var comentario = mapper.Map<Comentario>(crearComentarioDTO);
            comentario.IdComentario = id;
            comentario.IdPelicula = idPelicula;
            await repositorioComentarios.ActualizarComentario(comentario);
            await outputCacheStore.EvictByTagAsync(cacheName, default);
            return TypedResults.NoContent();
        }

        static async Task<Results<NoContent, NotFound>> EliminArComentarios(int idPelicula, int id, IRepositorioComentarios repositorioComentarios, IOutputCacheStore outputCacheStore)
        {

            if (!await repositorioComentarios.Existe(id))
            {
                return TypedResults.NotFound();
            }
            await repositorioComentarios.EliminarComentario(id);
            await outputCacheStore.EvictByTagAsync(cacheName, default);
            return TypedResults.NoContent();
        }
    }
}
