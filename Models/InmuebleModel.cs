namespace _net_integrador.Models
{
    public class Inmueble
    {
        public int id { get; set; }
        public int id_propietario { get; set; }
        public TipoInmueble tipo { get; set; }
        public UsoInmueble uso { get; set; }
        public Estado estado { get; set; }
        public string direccion { get; set; } = string.Empty;
        public int? ambientes { get; set; }
        public double? eje_x { get; set; }
        public double? eje_y { get; set; }
        public decimal? precio { get; set; }
        public string? imagen { get; set; }
        public double? superficie { get; set; }

    }

    public enum TipoInmueble
    {
        Casa = 1,
        Departamento = 2,
        Oficina = 3,
        Local = 4

    }

    public enum UsoInmueble
    {
        Comercial = 1,
        Residencial = 2
    }

    public enum Estado
    {
        Disponible = 1,
        Suspendido = 2,
        Alquilado = 3
    }
}
