using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using WebApiNet8.DTOs.Actores;
using WebApiNet8.DTOs.Paginacion;
using WebApiNet8.Entidades;
using WebApiNet8.Repositorios;
using WebApiNet8.Servicios;

namespace WebApiNet8.EndPoints
{
    public static class ActoresEndpoints
    {
        private static readonly string contenedor = "fotosactores";
        private static readonly string cacheName = "Actores-list";
        public static RouteGroupBuilder MapActores(this RouteGroupBuilder group)
        {

            group.MapGet("/", ObtenerActores).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag(cacheName)); // Se Agrega cache de 60 segundos

            group.MapGet("/filtrado/{nombreActor}", ObtenerActoresFiltrados);

            group.MapGet("/{id}", ObtenerActoresPorId);

            group.MapPost("/", CrearActor);

            group.MapPut("/{id}", ActualizarActor);

            group.MapDelete("/{id}", EliminarActor);

            return group;
        }

        static async Task<Ok<List<ActorDTO>>> ObtenerActores(IRepositorioActores repositorioActores, IMapper mapper, int pagina = 1, int registrosPorPagina = 3)
        {
            var paginacion = new PaginacionDTO() { Pagina = pagina, RegistrosPorPagina = registrosPorPagina };
            var Actores = await repositorioActores.ObtenerTodos(paginacion);
            var ActoresDto = mapper.Map<List<ActorDTO>>(Actores);
            return TypedResults.Ok(ActoresDto);
        }

        static async Task<Results<Ok<ActorDTO>, NotFound>> ObtenerActoresPorId(int id, IRepositorioActores repositorioActores, IMapper mapper)
        {
            var actor = await repositorioActores.ObtenerPorId(id);
            if (actor is null)
            {
                return TypedResults.NotFound();
            }
            var actorDTO = mapper.Map<ActorDTO>(actor);
            return TypedResults.Ok(actorDTO);
        }

        static async Task<Results<Ok<List<ActorDTO>>, NotFound>> ObtenerActoresFiltrados(string nombreActor, IRepositorioActores repositorioActores, IMapper mapper)
        {
            var actor = await repositorioActores.ObtenerFiltrados(nombreActor);
            if (actor is null || actor.Count == 0  )
            {
                return TypedResults.NotFound();
            }
            var actorDTO = mapper.Map<List<ActorDTO>>(actor);
            return TypedResults.Ok(actorDTO);
        }

        static async Task<Created<ActorDTO>> CrearActor([FromForm]CrearActorDTO crearActorDTO, IRepositorioActores repositorioActores, IOutputCacheStore outputCacheStore, IMapper mapper, IAlmacenarArchivos almacenarArchivos)
        {
            var actor = mapper.Map<Actor>(crearActorDTO);
            if (crearActorDTO.Foto is not null)
            {
                var url = await almacenarArchivos.Almacenar(contenedor, crearActorDTO.Foto);
                actor.Foto = url;
            }
            var id = await repositorioActores.CrearActor(actor);
            await outputCacheStore.EvictByTagAsync(cacheName, default);
            var actorDTO = mapper.Map<ActorDTO>(actor);
            actorDTO.IdActor = id;
            return TypedResults.Created($"/Actores/{id}", actorDTO);
        }

        static async Task<Results<NoContent, NotFound>> ActualizarActor(int id, [FromForm] CrearActorDTO crearActorDTO, IRepositorioActores repositorioActores, IOutputCacheStore outputCacheStore, IMapper mapper, IAlmacenarArchivos almacenarArchivos)
        {
            var actorExiste = await repositorioActores.ObtenerPorId(id);
            if (actorExiste is null)
            {
                return TypedResults.NotFound();
            }
            else
            {
                var actor = mapper.Map<Actor>(crearActorDTO);
                if (crearActorDTO.Foto is not null )
                {
                    actor.Foto = await almacenarArchivos.Editar(actorExiste.Foto,contenedor, crearActorDTO.Foto);
                }
                actor.IdActor = id;
                await repositorioActores.ActualizarActor(actor);
                await outputCacheStore.EvictByTagAsync(cacheName, default);
                return TypedResults.NoContent();
            }
        }

        static async Task<Results<NoContent, NotFound>> EliminarActor(int id, IRepositorioActores repositorioActores, IOutputCacheStore outputCacheStore, IAlmacenarArchivos almacenarArchivos)
        {
            var existe = await repositorioActores.Existe(id);
            if (!existe)
            {
                return TypedResults.NotFound();
            }
            else
            {
                await repositorioActores.EliminarActor(id);
                //await almacenarArchivos.Borrar(contenedor, id.ToString());
                await outputCacheStore.EvictByTagAsync(cacheName, default);
                return TypedResults.NoContent();
            }
        }
    }
}
