namespace WebApiNet8.Entidades
{
    public class LogError
    {
        public Guid IdLog { get; set; }
        public string MensajeError { get; set; } = null!;
        public string? StackTrace { get; set; }
        public DateTime Fecha { get; set; }
    }
}
