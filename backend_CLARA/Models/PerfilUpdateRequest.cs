namespace backend_CLARA.Models
{
    public class PerfilUpdateRequest
    {
        public string Nombre { get; set; } = string.Empty;
        public string ApellidoP { get; set; } = string.Empty; 
        public string ApellidoM { get; set; } = string.Empty; 
        public string Telefono { get; set; } = string.Empty; 
        public DateTime FechaNacimiento { get; set; }
        public int IdGenero { get; set; }
        public string Username { get; set; } = string.Empty; 
    }
}
