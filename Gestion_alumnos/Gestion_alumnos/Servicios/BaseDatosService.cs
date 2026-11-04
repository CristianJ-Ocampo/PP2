using SQLite;
using Gestion_alumnos.Modelos;

namespace Gestion_alumnos.Servicios
{
    public class BaseDatosService
    {
        private readonly SQLiteAsyncConnection conexion;

        public BaseDatosService()
        {
            string rutaBaseDatos = Path.Combine(FileSystem.AppDataDirectory,"gestion_alumnos.db3");

            System.Diagnostics.Debug.WriteLine($"RUTA DE LA BASE DE DATOS: {rutaBaseDatos}");

            conexion = new SQLiteAsyncConnection(rutaBaseDatos);
        }

        public async Task InicializarAsync()
        {
            await conexion.CreateTableAsync<Estudiante>();
            await conexion.CreateTableAsync<Clase>();
            await conexion.CreateTableAsync<TrabajoPractico>();
            await conexion.CreateTableAsync<Asistencia>();
            await conexion.CreateTableAsync<Entrega>();
            await conexion.CreateTableAsync<ConfiguracionRiesgo>();
            await conexion.CreateTableAsync<Intervencion>();
        }

        public async Task<int> GuardarEstudianteAsync(Estudiante estudiante)
        {
            return await conexion.InsertAsync(estudiante);
        }

        // OBTENER ESTUDIANTES
        public async Task<List<Estudiante>> ObtenerEstudiantesAsync()
        {
            return await conexion.Table<Estudiante>().ToListAsync();
        }


        // GUARDAR CLASE
        public async Task<int> GuardarClaseAsync(Clase clase)
        {
            return await conexion.InsertAsync(clase);
        }

        // OBTENER CLASES
        public async Task<List<Clase>> ObtenerClasesAsync()
        {
            return await conexion.Table<Clase>().ToListAsync();
        }

        // GUARDAR TRABAJO PRÁCTICO
        public async Task<int> GuardarTrabajoAsync(TrabajoPractico trabajo)
        {
            return await conexion.InsertAsync(trabajo);
        }

        // OBTENER TRABAJOS PRÁCTICOS
        public async Task<List<TrabajoPractico>> ObtenerTrabajosAsync()
        {
            return await conexion.Table<TrabajoPractico>().ToListAsync();

        }

        // GUARDAR O ACTUALIZAR ASISTENCIA
        public async Task<int> GuardarAsistenciaAsync(Asistencia asistencia)
        {
            if (asistencia.Id == 0)
                return await conexion.InsertAsync(asistencia);

            return await conexion.UpdateAsync(asistencia);
        }

        // OBTENER ASISTENCIAS
        public async Task<List<Asistencia>> ObtenerAsistenciasAsync()
        {
            return await conexion.Table<Asistencia>().ToListAsync();
        }

        // GUARDAR O ACTUALIZAR ENTREGA
        public async Task<int> GuardarEntregaAsync(Entrega entrega)
        {
            if (entrega.Id == 0)
            {
                return await conexion.InsertAsync(entrega);
            }

            return await conexion.UpdateAsync(entrega);
        }

        // OBTENER ENTREGAS
        public async Task<List<Entrega>> ObtenerEntregasAsync()
        {
            return await conexion.Table<Entrega>().ToListAsync();
        }

        public async Task LimpiarDatosAsync()
        {
            // Primero eliminamos los registros relacionados
            await conexion.DeleteAllAsync<Asistencia>();
            await conexion.DeleteAllAsync<Entrega>();
            await conexion.DeleteAllAsync<Intervencion>();

            // Después eliminamos los datos principales
            await conexion.DeleteAllAsync<TrabajoPractico>();
            await conexion.DeleteAllAsync<Clase>();
            await conexion.DeleteAllAsync<Estudiante>();
        }

        public async Task<ConfiguracionRiesgo> ObtenerConfiguracionRiesgoAsync()
        {
            var configuracion = await conexion
                .Table<ConfiguracionRiesgo>()
                .FirstOrDefaultAsync();

            if (configuracion == null)
            {
                configuracion = new ConfiguracionRiesgo
                {
                    Id = 1,
                    UmbralMedio = 20,
                    UmbralAlto = 40
                };

                await conexion.InsertAsync(configuracion);
            }

            return configuracion;
        }
        // GUARDAR O ACTUALIZAR UMBRALES DE RIESGO
        public async Task GuardarConfiguracionRiesgoAsync(ConfiguracionRiesgo configuracion)
        {
            await conexion.InsertOrReplaceAsync(configuracion);
        }

        // GUARDAR INTERVENCIÓN
        public async Task<int> GuardarIntervencionAsync(Intervencion intervencion)
        {
            return await conexion.InsertAsync(intervencion);
        }

        // OBTENER INTERVENCIONES
        public async Task<List<Intervencion>> ObtenerIntervencionesAsync()
        {
            return await conexion.Table<Intervencion>()
                .ToListAsync();
        }

    }
}
