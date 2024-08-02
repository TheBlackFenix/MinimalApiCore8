using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using WebApiNet8.DTOs.Paginacion;
using WebApiNet8.DTOs.Peliculas;
using WebApiNet8.Entidades;
using WebApiNet8.Filtros;
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
            group.MapPost("/", Crear).AddEndpointFilter<FiltroValidaciones<CrearPeliculaDTO>>();
            group.MapPut("/{id:int}", ActualizarPorId).AddEndpointFilter<FiltroValidaciones<CrearPeliculaDTO>>();
            group.MapGet("/", ObtenerTodos).CacheOutput(c=>c.Expire(TimeSpan.FromSeconds(60)).Tag(cacheName));
            group.MapGet("/{id:int}", ObtenerPorId);
            group.MapPost("/{id:int}/asignargeneros", AsignarGeneros);
            group.MapPost("/{id:int}/asignaractores", AsignarActores);
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

        static async Task<Results<NoContent, NotFound, BadRequest<string>>> AsignarGeneros(int id, List<int> generosIds, IRepositorioPeliculas repositorioPeliculas, IRepositorioGeneros repositorio)
        {
            if (!await repositorioPeliculas.Existe(id))
            {
                return TypedResults.NotFound();
            }
            var generosExistentes = new List<int>();
            if (generosIds.Count > 0)
            {
                generosExistentes = await repositorio.Existen(generosIds);
            }
            if (generosExistentes.Count != generosIds.Count)
            {
                var generosNoExistentes = generosIds.Except(generosExistentes);
                return TypedResults.BadRequest($"Los siguientes generos no existen: {string.Join(", ", generosNoExistentes)}");
            }
            await repositorioPeliculas.AsignarGeneros(id, generosIds);
            return TypedResults.NoContent();
        }

        static async Task<Results<NotFound, NoContent, BadRequest<string>>> AsignarActores(int id, List<AsignarActorPeliculaDTO> actoresDTO, IRepositorioPeliculas repositorioPeliculas, IRepositorioActores repositorioActores, IMapper mapper)
        {
            if (!await repositorioPeliculas.Existe(id))
            {
                return TypedResults.NotFound();
            }
            var actoresExistentes = new List<int>();
            var actoresIds = actoresDTO.Select(x => x.IdActor).ToList();
            if (actoresDTO.Count > 0)
            {
                actoresExistentes = await repositorioActores.Existen(actoresIds);
            }
            if (actoresExistentes.Count != actoresIds.Count)
            {
                var actoresNoExistentes = actoresIds.Except(actoresExistentes);
                return TypedResults.BadRequest($"Los siguientes actores no existen: {string.Join(", ", actoresNoExistentes)}");
            }
            var actoresPelicula = mapper.Map<List<ActorPelicula>>(actoresDTO);
            await repositorioPeliculas.AsignarActores(id, actoresPelicula);
            return TypedResults.NoContent();
        }
    }

}
