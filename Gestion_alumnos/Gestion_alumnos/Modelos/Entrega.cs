using SQLite;

namespace Gestion_alumnos.Modelos
{
    public class Entrega
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public int LegajoEstudiante { get; set; }

        public string NombreTrabajo { get; set; } = string.Empty;

        public string Comision { get; set; } = string.Empty;

        public bool Entregado { get; set; }
    }
}
