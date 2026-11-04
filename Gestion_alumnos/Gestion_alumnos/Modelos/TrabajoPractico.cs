using SQLite;

namespace Gestion_alumnos.Modelos
{
    public class TrabajoPractico
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Nombre { get; set; } = string.Empty;

        public DateTime FechaEntrega { get; set; }

        public string Comision { get; set; } = string.Empty;
    }
}
