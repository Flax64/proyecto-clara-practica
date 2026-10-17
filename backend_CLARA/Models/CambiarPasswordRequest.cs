namespace backend_CLARA.Models
{
    public class CambiarPasswordRequest
    {
        public string PasswordActual { get; set; } = string.Empty;
        public string NuevaPassword { get; set; } = string.Empty;
    }
}
