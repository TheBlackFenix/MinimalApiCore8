namespace WebApiNet8.DTOs.Paginacion
{
    public class PaginacionDTO
    {
        public int Pagina { get; set; }
        private int registrosPorPagina = 3;
        private readonly int cantidadRegistrosPorPagina = 6;

        public int RegistrosPorPagina
        {
            get => registrosPorPagina;
            set => registrosPorPagina = (value > cantidadRegistrosPorPagina) ? cantidadRegistrosPorPagina : value;
        }
    }
}
