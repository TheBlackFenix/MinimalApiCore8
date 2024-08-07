﻿using WebApiNet8.DTOs.Paginacion;
using WebApiNet8.Entidades;

namespace WebApiNet8.Repositorios
{
    public interface IRepositorioActores
    {
        Task ActualizarActor(Actor actor);
        Task<int> CrearActor(Actor actor);
        Task EliminarActor(int id);
        Task<Actor> ObtenerPorId(int id);
        Task<bool> Existe(int id);
        Task<List<Actor>> ObtenerTodos(PaginacionDTO paginacion);
        Task<List<Actor>> ObtenerFiltrados(string nombreActor);
        Task<List<int>> Existen(List<int> ids);
    }
}