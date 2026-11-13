using System.Collections.Generic;

namespace Gestion_alumnos.Servicios
{
    public static class ComisionesService
    {
        public static IReadOnlyList<string> Todas { get; } = new List<string>
            {
                "1°A", "1°B", "1°C",
                "2°A", "2°B", "2°C",
                "3°A", "3°B", "3°C",
                "4°A", "4°B", "4°C",
                "5°A", "5°B", "5°C"
            }.AsReadOnly();
    }
}