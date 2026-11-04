using SQLite;

namespace Gestion_alumnos.Modelos
{
    public class Clase
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public DateTime Fecha { get; set; }

        public string Comision { get; set; } = string.Empty;
    }
}