using System;
using System.Collections.Generic;
using System.Linq;
using Gestion_alumnos.Modelos;

namespace Gestion_alumnos.Servicios
{
    public class RiesgoService
    {
        private readonly DatosService datosService;

        // UMBRALES CONFIGURABLES
        private double UmbralMedio => datosService.ConfiguracionRiesgo.UmbralMedio;

        private double UmbralAlto => datosService.ConfiguracionRiesgo.UmbralAlto;

        public RiesgoService(DatosService datosService)
        {
            this.datosService = datosService;
        }

        public List<RiesgoEstudiante> CalcularRiesgos()
        {
            List<RiesgoEstudiante> resultados = new();

            foreach (var estudiante in datosService.Estudiantes)
            {
                // CLASES DE LA COMISIÓN
                var fechasClases = datosService.Clases
                    .Where(c => c.Comision == estudiante.Comision)
                    .Select(c => c.Fecha.Date)
                    .Distinct()
                    .ToList();

                // ASISTENCIAS EFECTIVAMENTE REGISTRADAS
                var asistencias = datosService.Asistencias
                    .Where(a =>
                        a.LegajoEstudiante == estudiante.Legajo &&
                        a.Comision == estudiante.Comision &&
                        fechasClases.Contains(a.FechaClase.Date))
                    .GroupBy(a => a.FechaClase.Date)
                    .Select(g => g.First())
                    .ToList();

                int totalAsistencias = asistencias.Count;

                int inasistencias = asistencias.Count(a => !a.Presente);
                
                double porcentajeInasistencias = totalAsistencias > 0
                    ? (double)inasistencias / totalAsistencias * 100
                    : 0;

                // TRABAJOS PRÁCTICOS DE LA COMISIÓN
                var nombresTrabajos = datosService.Trabajos
                    .Where(t => t.Comision == estudiante.Comision)
                    .Select(t => t.Nombre)
                    .Distinct()
                    .ToList();

                // ENTREGAS EFECTIVAMENTE REGISTRADAS
                var entregas = datosService.Entregas
                    .Where(e =>
                        e.LegajoEstudiante == estudiante.Legajo &&
                        e.Comision == estudiante.Comision &&
                        nombresTrabajos.Contains(e.NombreTrabajo))
                    .GroupBy(e => e.NombreTrabajo)
                    .Select(g => g.First())
                    .ToList();

                int totalEntregas = entregas.Count;

                int noEntregas = entregas.Count(e => !e.Entregado);

                double porcentajeNoEntregas = totalEntregas > 0
                    ? (double)noEntregas / totalEntregas * 100
                    : 0;

                // DETERMINAR SEÑALES DISPONIBLES
                bool hayAsistencias = totalAsistencias > 0;
                bool hayEntregas = totalEntregas > 0;

                double riesgo = 0;
                string nivel = "Sin datos";

                // CALCULAR RIESGO
                if (hayAsistencias && hayEntregas)
                {
                    riesgo = porcentajeInasistencias * 0.5 + porcentajeNoEntregas * 0.5;
                }
                else if (hayAsistencias)
                {
                    riesgo = porcentajeInasistencias;
                }
                else if (hayEntregas)
                {
                    riesgo = porcentajeNoEntregas;
                }

                // CLASIFICAR RIESGO
                if (hayAsistencias || hayEntregas)
                {
                    if (riesgo >= UmbralAlto)
                        nivel = "Alto";
                    else if (riesgo >= UmbralMedio)
                        nivel = "Medio";
                    else
                        nivel = "Bajo";
                }

                // CREAR RESULTADO
                resultados.Add(new RiesgoEstudiante
                {
                    Legajo = estudiante.Legajo,

                    NombreCompleto = $"{estudiante.Apellido}, {estudiante.Nombre}",

                    Comision = estudiante.Comision,

                    PorcentajeInasistencias = porcentajeInasistencias,

                    PorcentajeNoEntregas = porcentajeNoEntregas,

                    PorcentajeRiesgo = riesgo,

                    NivelRiesgo = nivel
                });
            }

            // ORDENAR DE MAYOR A MENOR RIESGO
            return resultados
                .OrderByDescending(r => r.PorcentajeRiesgo)
                .ToList();
        }
    }
}
