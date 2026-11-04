using SQLite;

namespace Gestion_alumnos.Modelos
{
    public class Asistencia
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public int LegajoEstudiante { get; set; }

        public DateTime FechaClase { get; set; }

        public string Comision { get; set; } = string.Empty;

        public bool Presente { get; set; }
    }
}
