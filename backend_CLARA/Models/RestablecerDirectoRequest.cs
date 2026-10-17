namespace backend_CLARA.Models
{
    public class RestablecerDirectoRequest
    {
        public string Token { get; set; } = string.Empty; // El identificador mágico
        public string NuevaPassword { get; set; } = string.Empty;
    }
}
