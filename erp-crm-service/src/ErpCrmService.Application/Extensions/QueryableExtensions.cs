using System;
using System.Linq;
using System.Collections.Generic;
using ErpCrmService.Application.Models;

namespace ErpCrmService.Application.Extensions
{
    /// <summary>
    /// Métodos de extensión para paginación de IQueryable
    /// </summary>
    public static class QueryableExtensions
    {
        /// <summary>
        /// Convierte un IQueryable en un resultado paginado
        /// </summary>
        /// <typeparam name="T">Tipo de entidad</typeparam>
        /// <param name="source">Query source</param>
        /// <param name="pageNumber">Número de página (base 1)</param>
        /// <param name="pageSize">Tamaño de página</param>
        /// <returns>Resultado paginado</returns>
        public static PagedResult<T> ToPagedResult<T>(this IQueryable<T> source, int pageNumber, int pageSize)
        {
            if (pageNumber < 1)
                pageNumber = 1;

            if (pageSize < 1)
                pageSize = 20;

            var count = source.Count();
            var items = source
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PagedResult<T>(items, count, pageNumber, pageSize);
        }

        /// <summary>
        /// Convierte un IQueryable en un resultado paginado usando PaginationParameters
        /// </summary>
        /// <typeparam name="T">Tipo de entidad</typeparam>
        /// <param name="source">Query source</param>
        /// <param name="parameters">Parámetros de paginación</param>
        /// <returns>Resultado paginado</returns>
        public static PagedResult<T> ToPagedResult<T>(this IQueryable<T> source, PaginationParameters parameters)
        {
            return source.ToPagedResult(parameters.PageNumber, parameters.PageSize);
        }

        /// <summary>
        /// Aplica paginación a un IQueryable sin materializar la consulta
        /// </summary>
        /// <typeparam name="T">Tipo de entidad</typeparam>
        /// <param name="source">Query source</param>
        /// <param name="pageNumber">Número de página</param>
        /// <param name="pageSize">Tamaño de página</param>
        /// <returns>IQueryable paginado</returns>
        public static IQueryable<T> ApplyPaging<T>(this IQueryable<T> source, int pageNumber, int pageSize)
        {
            if (pageNumber < 1)
                pageNumber = 1;

            if (pageSize < 1)
                pageSize = 20;

            return source
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);
        }
    }
}
