using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _net_integrador.Models
{
    public class Propietario
    {
        [Key] 
        public int id { get; set; }

        [Required] 
        [StringLength(100)]
        public string nombre { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string apellido { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string dni { get; set; } = string.Empty;

        [Required] 
        [StringLength(100)] 
        [EmailAddress]
        public string email { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string telefono { get; set; } = string.Empty;

        [StringLength(255)]
        public string? clave { get; set; }

        [Required]
        public int estado { get; set; } = 1; 

        public enum EstadoPropietario
        {
            Activo = 1,
            Inactivo = 0
        }

        public Propietario() { }

        public Propietario(string nombre, string apellido, string dni, string email, string telefono, string clave)
        {
            this.nombre = nombre;
            this.apellido = apellido;
            this.dni = dni;
            this.email = email;
            this.telefono = telefono;
            this.clave = clave;
            this.estado = (int)EstadoPropietario.Activo;
        }

        public Propietario(int id, string nombre, string apellido, string dni, string email, string telefono, string clave, int estado)
        {
            this.id = id;
            this.nombre = nombre;
            this.apellido = apellido;
            this.dni = dni;
            this.email = email;
            this.telefono = telefono;
            this.clave = clave;
            this.estado = estado;
        }

        public override string ToString()
        {
            return $"{apellido}, {nombre} ({email})";
        }
    }
}
