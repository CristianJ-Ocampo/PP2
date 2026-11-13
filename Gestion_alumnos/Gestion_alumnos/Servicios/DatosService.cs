using Gestion_alumnos.Modelos;
using System.Collections.ObjectModel;

namespace Gestion_alumnos.Servicios
{
    public class DatosService
    {
        private readonly BaseDatosService baseDatosService;

        public bool HayEstudiantes => Estudiantes.Count > 0;

        // COLECCIONES
        public ObservableCollection<Estudiante> Estudiantes { get; }
            = new();

        public ObservableCollection<Clase> Clases { get; }
            = new();

        public ObservableCollection<TrabajoPractico> Trabajos { get; }
            = new();

        public ObservableCollection<Asistencia> Asistencias { get; }
            = new();

        public ObservableCollection<Entrega> Entregas { get; }
            = new();
        public ObservableCollection<Intervencion> Intervenciones { get; }
            = new();
        // CONFIGURACIÓN DE RIESGO
        public ConfiguracionRiesgo ConfiguracionRiesgo { get; private set; }
            = new ConfiguracionRiesgo();

        // CONSTRUCTOR
        public DatosService(BaseDatosService baseDatosService)
        {
            this.baseDatosService = baseDatosService;
        }

        // CARGAR TODOS LOS DATOS DESDE SQLITE
        public async Task CargarDatosAsync()
        {
            await baseDatosService.InicializarAsync();

            // ESTUDIANTES
            var estudiantesGuardados = await baseDatosService.ObtenerEstudiantesAsync();

            Estudiantes.Clear();

            foreach (var estudiante in estudiantesGuardados)
            {
                Estudiantes.Add(estudiante);
            }

            // CLASES
            var clasesGuardadas = await baseDatosService.ObtenerClasesAsync();

            Clases.Clear();

            foreach (var clase in clasesGuardadas)
            {
                Clases.Add(clase);
            }

            // TRABAJOS PRÁCTICOS
            var trabajosGuardados = await baseDatosService.ObtenerTrabajosAsync();

            Trabajos.Clear();

            foreach (var trabajo in trabajosGuardados)
            {
                Trabajos.Add(trabajo);
            }

            // ASISTENCIAS
            var asistenciasGuardadas = await baseDatosService.ObtenerAsistenciasAsync();

            Asistencias.Clear();

            foreach (var asistencia in asistenciasGuardadas)
            {
                Asistencias.Add(asistencia);
            }

            // ENTREGAS
            var entregasGuardadas = await baseDatosService.ObtenerEntregasAsync();

            Entregas.Clear();

            foreach (var entrega in entregasGuardadas)
            {
                Entregas.Add(entrega);
            }
            // INTERVENCIONES
            var intervencionesGuardadas = await baseDatosService.ObtenerIntervencionesAsync();

            Intervenciones.Clear();

            foreach (var intervencion in intervencionesGuardadas)
            {
                Intervenciones.Add(intervencion);
            }

            // CARGAR CONFIGURACIÓN DE RIESGO
            ConfiguracionRiesgo = await baseDatosService.ObtenerConfiguracionRiesgoAsync();
        }

        // GUARDAR ESTUDIANTE
        public async Task GuardarEstudianteAsync(Estudiante estudiante)
        {
            await baseDatosService.GuardarEstudianteAsync(estudiante);

            Estudiantes.Add(estudiante);
        }

        // GUARDAR CLASE
        public async Task GuardarClaseAsync(Clase clase)
        {
            await baseDatosService.GuardarClaseAsync(clase);

            Clases.Add(clase);
        }

        // GUARDAR TRABAJO PRÁCTICO
        public async Task GuardarTrabajoAsync(TrabajoPractico trabajo)
        {
            await baseDatosService.GuardarTrabajoAsync(trabajo);

            Trabajos.Add(trabajo);
        }

        // GUARDAR O ACTUALIZAR ASISTENCIA
        public async Task GuardarAsistenciaAsync(Asistencia asistencia)
        {
            await baseDatosService.GuardarAsistenciaAsync(asistencia);

            if (!Asistencias.Contains(asistencia))
            {
                Asistencias.Add(asistencia);
            }
        }

        // GUARDAR O ACTUALIZAR ENTREGA
        public async Task GuardarEntregaAsync(Entrega entrega)
        {
            await baseDatosService.GuardarEntregaAsync(entrega);

            if (!Entregas.Contains(entrega))
            {
                Entregas.Add(entrega);
            }
        }
        // GUARDAR INTERVENCIÓN
        public async Task GuardarIntervencionAsync(Intervencion intervencion)
        {
            await baseDatosService.GuardarIntervencionAsync(intervencion);

            Intervenciones.Add(intervencion);
        }

        public async Task LimpiarDatosAsync()
        {
            // Eliminar registros de SQLite
            await baseDatosService.LimpiarDatosAsync();

            // Vaciar las colecciones de la aplicación
            Intervenciones.Clear();
            Asistencias.Clear();
            Entregas.Clear();
            Trabajos.Clear();
            Clases.Clear();
            Estudiantes.Clear();
        }

        // GUARDAR CONFIGURACIÓN DE RIESGO
        public async Task GuardarConfiguracionRiesgoAsync(double umbralMedio,double umbralAlto)
        {
            // VALIDAR LOS PORCENTAJES
            if (double.IsNaN(umbralMedio) ||
                double.IsNaN(umbralAlto) ||
                double.IsInfinity(umbralMedio) ||
                double.IsInfinity(umbralAlto) ||
                umbralMedio < 0 ||
                umbralMedio > 100 ||
                umbralAlto < 0 ||
                umbralAlto > 100 ||
                umbralMedio >= umbralAlto)
            {
                throw new ArgumentException("Los umbrales deben estar entre 0 y 100, " + "y el umbral medio debe ser menor que el alto.");
            }

            // CREAR CONFIGURACIÓN
            var nuevaConfiguracion = new ConfiguracionRiesgo
            {
                Id = 1,
                UmbralMedio = umbralMedio,
                UmbralAlto = umbralAlto
            };

            // GUARDAR EN SQLITE
            await baseDatosService.GuardarConfiguracionRiesgoAsync(nuevaConfiguracion);

            // ACTUALIZAR LA CONFIGURACIÓN EN MEMORIA
            ConfiguracionRiesgo = nuevaConfiguracion;
        }

        // CARGAR DATOS FICTICIOS EN SQLITE
        public async Task CargarDatosPruebaAsync(
            List<Estudiante> estudiantes,
            List<Clase> clases,
            List<TrabajoPractico> trabajos,
            List<Asistencia> asistencias,
            List<Entrega> entregas)
        {
            // GUARDAR TODOS LOS REGISTROS EN UNA TRANSACCIÓN
            await baseDatosService.CargarDatosPruebaAsync(
                estudiantes,
                clases,
                trabajos,
                asistencias,
                entregas);

            // ACTUALIZAR LAS COLECCIONES DE LA APLICACIÓN
            await CargarDatosAsync();
        }
    }
}
