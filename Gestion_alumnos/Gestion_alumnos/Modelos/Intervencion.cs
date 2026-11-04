
using SQLite;

namespace Gestion_alumnos.Modelos
{
    public class Intervencion
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public int LegajoEstudiante { get; set; }

        public DateTime Fecha { get; set; }

        public string MedioContacto { get; set; } = "";

        public string Motivo { get; set; } = "";

        public string Resultado { get; set; } = "";
    }
}