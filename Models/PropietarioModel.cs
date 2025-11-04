namespace _net_integrador.Models
{
    public class Propietario
    {
        public int id { get; set; }
        public string nombre { get; set; } = string.Empty;
        public string apellido { get; set; } = string.Empty;
        public string dni { get; set; } = string.Empty;
        public string email { get; set; } = string.Empty;
        public string telefono { get; set; } = string.Empty;
        public string? clave { get; set; }
        public int estado { get; set; } = 1;

        public Propietario() { }

        public Propietario(string nombre, string apellido, string dni, string email, string telefono, string clave)
        {
            this.nombre = nombre;
            this.apellido = apellido;
            this.dni = dni;
            this.email = email;
            this.telefono = telefono;
            this.clave = clave;
            this.estado = 1;
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
