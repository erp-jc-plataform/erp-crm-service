using System;
using System.Collections.Generic;

namespace ErpCrmService.Application.Models
{
    /// <summary>
    /// Representa un resultado paginado de una consulta
    /// </summary>
    /// <typeparam name="T">Tipo de los elementos de la lista</typeparam>
    public class PagedResult<T>
    {
        /// <summary>
        /// Lista de elementos de la página actual
        /// </summary>
        public List<T> Items { get; set; }

        /// <summary>
        /// Número de página actual (base 1)
        /// </summary>
        public int PageNumber { get; set; }

        /// <summary>
        /// Tamaño de página
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Total de elementos en todas las páginas
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// Total de páginas disponibles
        /// </summary>
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);

        /// <summary>
        /// Indica si existe una página anterior
        /// </summary>
        public bool HasPrevious => PageNumber > 1;

        /// <summary>
        /// Indica si existe una página siguiente
        /// </summary>
        public bool HasNext => PageNumber < TotalPages;

        /// <summary>
        /// Número de la primera página
        /// </summary>
        public int FirstPage => 1;

        /// <summary>
        /// Número de la última página
        /// </summary>
        public int LastPage => TotalPages;

        /// <summary>
        /// Número de elementos en la página actual
        /// </summary>
        public int CurrentPageSize => Items?.Count ?? 0;

        /// <summary>
        /// Constructor por defecto
        /// </summary>
        public PagedResult()
        {
            Items = new List<T>();
        }

        /// <summary>
        /// Constructor con datos
        /// </summary>
        public PagedResult(List<T> items, int count, int pageNumber, int pageSize)
        {
            Items = items;
            TotalCount = count;
            PageNumber = pageNumber;
            PageSize = pageSize;
        }
    }

    /// <summary>
    /// Parámetros de paginación para las consultas
    /// </summary>
    public class PaginationParameters
    {
        private const int MaxPageSize = 100;
        private int _pageSize = 20;

        /// <summary>
        /// Número de página (base 1)
        /// </summary>
        public int PageNumber { get; set; } = 1;

        /// <summary>
        /// Tamaño de página (máximo 100)
        /// </summary>
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
        }

        /// <summary>
        /// Campo para ordenar
        /// </summary>
        public string OrderBy { get; set; }

        /// <summary>
        /// Dirección del orden (asc, desc)
        /// </summary>
        public string OrderDirection { get; set; } = "asc";
    }
}
