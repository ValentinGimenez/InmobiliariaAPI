using System.ComponentModel.DataAnnotations;

namespace _net_integrador.Models
{
    public class Inquilino
    {
        public int id { get; set; }

        [Required(ErrorMessage = "Ingrese un Nombre")]
        [RegularExpression(@"^[\p{L}]+$", ErrorMessage = "El nombre no puede contener números ni caracteres especiales")]
        [StringLength(100)]
        public string nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ingrese un Apellido")]
        [RegularExpression(@"^[\p{L}]+$", ErrorMessage = "El apellido no puede contener números ni caracteres especiales")]
        [StringLength(100)]
        public string apellido { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ingrese un DNI")]
        [RegularExpression(@"^\d{8}$", ErrorMessage = "El DNI no es válido")]
        [StringLength(20)]
        public string dni { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ingrese un Email")]
        [EmailAddress(ErrorMessage = "El email no es válido")]
        [StringLength(100)] 
        public string email { get; set; } = string.Empty;

        [Required(ErrorMessage = "El teléfono es obligatorio")]
        [RegularExpression(@"^(\d{8,12})?$", ErrorMessage = "El teléfono no es válido")]
        [StringLength(20)]
        public string telefono { get; set; } = string.Empty;

        public int estado { get; set; } 
        
        public string? imagen { get; set; } 

        public string? NombreCompleto => $"{nombre} {apellido}";

        public enum EstadoInquilino
        {
            Activo = 1,
            Inactivo = 0
        }
    }
}
