using SQLite;

namespace Gestion_alumnos.Modelos
{
    public class ConfiguracionRiesgo
    {
        [PrimaryKey]
        public int Id { get; set; } = 1;

        public double UmbralMedio { get; set; } = 20;

        public double UmbralAlto { get; set; } = 40;
    }
}