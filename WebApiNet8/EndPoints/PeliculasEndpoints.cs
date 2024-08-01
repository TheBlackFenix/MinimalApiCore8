using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using WebApiNet8.DTOs.Paginacion;
using WebApiNet8.DTOs.Peliculas;
using WebApiNet8.Entidades;
using WebApiNet8.Repositorios;
using WebApiNet8.Servicios;

namespace WebApiNet8.EndPoints
{
    public static class PeliculasEndpoints
    {
        private static readonly string contenedor = "peliculas";
        private static readonly string cacheName = "Peliculas-list";
        public static RouteGroupBuilder MapPeliculas(this RouteGroupBuilder group)
        {
            group.MapPost("/", Crear);
            group.MapGet("/", ObtenerTodos).CacheOutput(c=>c.Expire(TimeSpan.FromSeconds(60)).Tag(cacheName));
            group.MapGet("/{id:int}", ObtenerPorId);
            group.MapPut("/{id:int}", ActualizarPorId);

            return group;
        }

        static async Task<Ok<List<PeliculaDTO>>> ObtenerTodos(IRepositorioPeliculas repositorio, IMapper mapper, int pagina = 1, int registrosPorPagina = 3)
        {
            var paginacion = new PaginacionDTO() { Pagina = pagina, RegistrosPorPagina = registrosPorPagina };
            var peliculasPaginadas = await repositorio.ObtenerTodos(paginacion);
            var peliculasDTO = mapper.Map<List<PeliculaDTO>>(peliculasPaginadas);
            return TypedResults.Ok(peliculasDTO);
        }

        static async Task<Results<Ok<PeliculaDTO>, NotFound>> ObtenerPorId(int id, IRepositorioPeliculas repositorio, IMapper mapper)
        {
            var pelicula = await repositorio.ObtenerPeliculaPorId(id);
            if (pelicula is null)
            {
                return TypedResults.NotFound();
            }
            var peliculaDTO = mapper.Map<PeliculaDTO>(pelicula);
            return TypedResults.Ok(peliculaDTO);
        }

        static async Task<Created<PeliculaDTO>> Crear([FromForm] CrearPeliculaDTO crearPeliculaDTO, IRepositorioPeliculas repositorioPeliculas, IAlmacenarArchivos almacenarArchivos, IOutputCacheStore outputCacheStore, IMapper mapper)
        {
            var pelicula = mapper.Map<Pelicula>(crearPeliculaDTO);
            if(crearPeliculaDTO.Poster is not null)
            {
               pelicula.Poster = await almacenarArchivos.Almacenar(contenedor, crearPeliculaDTO.Poster );
            }
            var idPelicula = await repositorioPeliculas.CrearPelicula(pelicula);
            await outputCacheStore.EvictByTagAsync(cacheName, default);
            var peliculaDTO = mapper.Map<PeliculaDTO>(pelicula);
            return TypedResults.Created($"/peliculas/{idPelicula}", peliculaDTO);
        }

        static async Task<Results<NoContent, NotFound>> ActualizarPorId(int id, [FromForm] CrearPeliculaDTO crearPeliculaDTO, IRepositorioPeliculas repositorio, IAlmacenarArchivos almacenarArchivos, IOutputCacheStore outputCacheStore, IMapper mapper)
        {
            var peliculaDB = await repositorio.ObtenerPeliculaPorId(id);
            if (peliculaDB is null)
            {
                return TypedResults.NotFound();
            }

            var pelicula = mapper.Map<Pelicula>(crearPeliculaDTO);
            pelicula.IdPelicula = id;
            pelicula.Poster = peliculaDB.Poster;

            if (crearPeliculaDTO.Poster is not null)
            {
                pelicula.Poster = await almacenarArchivos.Editar(pelicula.Poster, contenedor, crearPeliculaDTO.Poster);
            }

            await repositorio.ActualizarPelicula(pelicula);
            await outputCacheStore.EvictByTagAsync(cacheName, default);
            return TypedResults.NoContent();
        }

        static async Task<Results<NotFound, NoContent>> BorrarPorId(int id, IRepositorioPeliculas repositorio, IAlmacenarArchivos almacenarArchivos, IOutputCacheStore outputCacheStore)
        {
            var peliculaDB = await repositorio.ObtenerPeliculaPorId(id);
            if(peliculaDB is null)
            {
                return TypedResults.NotFound();
            }
            await repositorio.EliminarPelicula(id);
            await outputCacheStore.EvictByTagAsync(cacheName, default);
            //if (peliculaDB.Poster is not null)
            //{
            //    await almacenarArchivos.Borrar(peliculaDB.Poster, contenedor);
            //}
            return TypedResults.NoContent();
        }

    }

}
