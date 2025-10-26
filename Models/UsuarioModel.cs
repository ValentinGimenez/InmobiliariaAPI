using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _net_integrador.Models
{
    [Table("usuario")]
    public class Usuario
    {
        public int id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio."), StringLength(100)]
        public string nombre { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "El apellido es obligatorio."), StringLength(100)]
        public string apellido { get; set; } = string.Empty;

        [Required(ErrorMessage = "El DNI es obligatorio."), StringLength(20)]
        public string dni { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "El email es obligatorio."), EmailAddress(ErrorMessage = "El email no es válido."), StringLength(100)]
        public string email { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria."), DataType(DataType.Password), StringLength(255)]
        public string password { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "El rol es obligatorio."), StringLength(20), Column(TypeName = "enum('Admin', 'Empleado')")]
        public string rol { get; set; } = "Empleado";

        [Required(ErrorMessage = "El estado es obligatorio.")]
        public int estado { get; set; }
        
        [StringLength(255)]
        public string avatar { get; set; } = string.Empty;
    }
}