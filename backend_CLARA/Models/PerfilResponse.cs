namespace backend_CLARA.Models
{
    public class PerfilResponse
    {
        public string Nombre { get; set; } = string.Empty; 
        public string ApellidoP { get; set; } = string.Empty; 
        public string ApellidoM { get; set; } = string.Empty; 
        public string Telefono { get; set; } = string.Empty; 
        public DateTime FechaNacimiento { get; set; }
        public string Correo { get; set; } = string.Empty; 
        public string Genero { get; set; } = string.Empty; 
        public string Username { get; set; } = string.Empty; 
    }
}
