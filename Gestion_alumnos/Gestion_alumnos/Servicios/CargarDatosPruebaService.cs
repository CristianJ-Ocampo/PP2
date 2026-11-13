
using Gestion_alumnos.Modelos;

namespace Gestion_alumnos.Servicios
{
    public class CargaDatosPruebaService
    {
        private readonly DatosService datosService;

        public CargaDatosPruebaService(DatosService datosService)
        {
            this.datosService = datosService;
        }

        public async Task GenerarDatosAsync()
        {
            var estudiantes = new List<Estudiante>();
            var clases = new List<Clase>();
            var trabajos = new List<TrabajoPractico>();
            var asistencias = new List<Asistencia>();
            var entregas = new List<Entrega>();

            string[] nombres =
            {
                "Juan", "María", "Pedro", "Lucía", "Martín",
                "Sofía", "Lucas", "Valentina", "Tomás", "Camila",
                "Mateo", "Julieta", "Nicolás", "Martina", "Facundo"
            };

            string[] apellidos =
            {
                "Gómez", "Pérez", "Rodríguez", "Fernández", "López",
                "Martínez", "García", "Sánchez", "Romero", "Torres",
                "Díaz", "Ruiz", "Herrera", "Castro", "Medina",
                "Flores", "Acosta", "Rojas", "Silva", "Molina",
                "Vega", "Suárez", "Ortiz", "Navarro", "Ríos",
                "Cabrera", "Morales", "Ponce", "Aguirre", "Benítez"
            };

            int legajo = 10001;

            foreach (string comision in ComisionesService.Todas)
            {
                for (int i = 0; i < 30; i++)
                {
                    estudiantes.Add(new Estudiante
                    {
                        Legajo = legajo++,
                        Nombre = nombres[i % nombres.Length],
                        Apellido = apellidos[i],
                        Comision = comision
                    });
                }

                // 1°B: estudiantes sin datos académicos
                if (comision == "1°B")
                    continue;

                // CREAR 10 CLASES POR COMISIÓN
                for (int i = 0; i < 10; i++)
                {
                    clases.Add(new Clase
                    {
                        Comision = comision,
                        Fecha = new DateTime(2026, 8, 3).AddDays(i * 7)
                    });
                }

                // CREAR 5 TRABAJOS PRÁCTICOS POR COMISIÓN
                for (int i = 0; i < 5; i++)
                {
                    trabajos.Add(new TrabajoPractico
                    {
                        Comision = comision,
                        Nombre = $"Trabajo práctico {i + 1}",
                        FechaEntrega = new DateTime(2026, 9, 4).AddDays(i * 7)
                    });
                }

                var alumnosComision = estudiantes
                    .Where(e => e.Comision == comision).ToList();

                var clasesComision = clases .Where(c => c.Comision == comision) .ToList();

                var trabajosComision = trabajos .Where(t => t.Comision == comision) .ToList();

                for (int i = 0; i < alumnosComision.Count; i++)
                {
                    var alumno = alumnosComision[i];

                    int faltas;
                    int trabajosSinEntregar;

                    if (comision == "1°A")
                    {
                        // EXACTAMENTE 7 ESTUDIANTES EN RIESGO ALTO
                        faltas = i < 4 ? 4 : i < 7 ? 6 : 1;
                        trabajosSinEntregar = i < 4 ? 2 : i < 7 ? 3 : 0;
                    }
                    else
                    {
                        // DIFERENTES SITUACIONES ACADÉMICAS
                        switch (i % 5)
                        {
                            case 0:
                                faltas = 0;
                                trabajosSinEntregar = 0;
                                break;

                            case 1:
                                faltas = 2;
                                trabajosSinEntregar = 1;
                                break;

                            case 2:
                                faltas = 4;
                                trabajosSinEntregar = 2;
                                break;

                            case 3:
                                faltas = 6;
                                trabajosSinEntregar = 3;
                                break;

                            default:
                                faltas = 1;
                                trabajosSinEntregar = 0;
                                break;
                        }
                    }

                    // REGISTRAR ASISTENCIAS
                    for (int j = 0; j < clasesComision.Count; j++)
                    {
                        asistencias.Add(new Asistencia
                        {
                            LegajoEstudiante = alumno.Legajo,
                            Comision = comision,
                            FechaClase = clasesComision[j].Fecha,
                            Presente = j >= faltas
                        });
                    }

                    // REGISTRAR ENTREGAS
                    for (int j = 0; j < trabajosComision.Count; j++)
                    {
                        entregas.Add(new Entrega
                        {
                            LegajoEstudiante = alumno.Legajo,
                            Comision = comision,
                            NombreTrabajo = trabajosComision[j].Nombre,
                            Entregado = j >= trabajosSinEntregar
                        });
                    }
                }
            }

            // GUARDAR EN SQLITE Y ACTUALIZAR COLECCIONES
            await datosService.CargarDatosPruebaAsync(estudiantes,clases,trabajos,asistencias,entregas);
        }
    }
}