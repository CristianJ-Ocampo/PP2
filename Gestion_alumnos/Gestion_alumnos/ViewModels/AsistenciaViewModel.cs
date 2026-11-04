using Gestion_alumnos.Modelos;
using Gestion_alumnos.Servicios;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Gestion_alumnos.ViewModels
{
    public class AsistenciaViewModel : INotifyPropertyChanged
    {
        private readonly DatosService datosService;

        // LISTAS PARA MOSTRAR EN PANTALLA
        public ObservableCollection<Clase> ClasesDisponibles { get; }

        public ObservableCollection<EstudianteAsistencia> EstudiantesAsistencia { get; }

        // COMISIÓN SELECCIONADA
        private string? comisionSeleccionada;

        public string? ComisionSeleccionada
        {
            get { return comisionSeleccionada; }
            set
            {
                comisionSeleccionada = value;
                OnPropertyChanged();
                ActualizarClases();
            }
        }

        // CLASE SELECCIONADA
        private Clase? claseSeleccionada;

        public Clase? ClaseSeleccionada
        {
            get { return claseSeleccionada; }
            set
            {
                claseSeleccionada = value;
                OnPropertyChanged();
                ActualizarEstudiantes();
            }
        }

        // MENSAJE DE LA PANTALLA
        private string mensajeEstado =
            "Seleccione una comisión y una clase para registrar asistencia.";

        public string MensajeEstado
        {
            get { return mensajeEstado; }
            set
            {
                mensajeEstado = value;
                OnPropertyChanged();
            }
        }

        // HABILITAR BOTÓN GUARDAR
        private bool puedeGuardar;

        public bool PuedeGuardar
        {
            get { return puedeGuardar; }
            set
            {
                puedeGuardar = value;
                OnPropertyChanged();
            }
        }

        // COMANDO
        public ICommand GuardarAsistenciaCommand { get; }

        // NOTIFICACIÓN DE CAMBIOS
        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged(
            [CallerMemberName] string? nombrePropiedad = null)
        {
            PropertyChanged?.Invoke(
                this,
                new PropertyChangedEventArgs(nombrePropiedad));
        }

        // CONSTRUCTOR
        public AsistenciaViewModel(DatosService datosService)
        {
            this.datosService = datosService;

            ClasesDisponibles = new ObservableCollection<Clase>();

            EstudiantesAsistencia =
                new ObservableCollection<EstudianteAsistencia>();

            GuardarAsistenciaCommand =
                new Command(async () => await GuardarAsistenciaAsync());

            // ACTUALIZAR LAS CLASES SI SE CARGAN DESDE SQLITE
            datosService.Clases.CollectionChanged += (sender, args) =>
            {
                ActualizarClases();
            };
        }

        // CAMBIAR COMISIÓN
        private void ActualizarClases()
        {
            ClaseSeleccionada = null;

            ClasesDisponibles.Clear();
            EstudiantesAsistencia.Clear();

            PuedeGuardar = false;

            if (string.IsNullOrWhiteSpace(ComisionSeleccionada))
            {
                MensajeEstado =
                    "Seleccione una comisión y una clase para registrar asistencia.";
                return;
            }

            var clasesFiltradas = datosService.Clases
                .Where(c => c.Comision == ComisionSeleccionada)
                .OrderBy(c => c.Fecha)
                .ToList();

            foreach (Clase clase in clasesFiltradas)
            {
                ClasesDisponibles.Add(clase);
            }

            MensajeEstado = ClasesDisponibles.Count == 0
                ? "No hay clases registradas para esta comisión."
                : "Seleccione una clase.";
        }

        // CAMBIAR CLASE
        private void ActualizarEstudiantes()
        {
            EstudiantesAsistencia.Clear();

            PuedeGuardar = false;

            if (ClaseSeleccionada == null)
            {
                return;
            }

            var estudiantes = datosService.Estudiantes
                .Where(e => e.Comision == ClaseSeleccionada.Comision)
                .OrderBy(e => e.Apellido)
                .ThenBy(e => e.Nombre)
                .ToList();

            foreach (Estudiante estudiante in estudiantes)
            {
                Asistencia? asistenciaGuardada =
                    datosService.Asistencias.FirstOrDefault(a =>
                        a.LegajoEstudiante == estudiante.Legajo &&
                        a.Comision == ClaseSeleccionada.Comision &&
                        a.FechaClase.Date == ClaseSeleccionada.Fecha.Date);

                EstudianteAsistencia registro = new EstudianteAsistencia
                {
                    Legajo = estudiante.Legajo,

                    NombreCompleto =
                        estudiante.Apellido + ", " + estudiante.Nombre,

                    Presente = asistenciaGuardada?.Presente ?? false
                };

                EstudiantesAsistencia.Add(registro);
            }

            MensajeEstado = estudiantes.Count == 0
                ? "No hay estudiantes en esta comisión."
                : "Marque los estudiantes presentes.";

            PuedeGuardar = estudiantes.Count > 0;
        }

        // GUARDAR O CORREGIR ASISTENCIA EN SQLITE
        private async Task GuardarAsistenciaAsync()
        {
            if (ClaseSeleccionada == null)
            {
                await MostrarMensaje(
                    "Error",
                    "Debe seleccionar una clase.");
                return;
            }

            // EVITAR QUE SE GUARDE DOS VECES AL MISMO TIEMPO
            if (!PuedeGuardar)
            {
                return;
            }

            PuedeGuardar = false;

            try
            {
                foreach (EstudianteAsistencia estudiante in EstudiantesAsistencia)
                {
                    Asistencia? asistenciaExistente =
                        datosService.Asistencias.FirstOrDefault(a =>
                            a.LegajoEstudiante == estudiante.Legajo &&
                            a.Comision == ClaseSeleccionada.Comision &&
                            a.FechaClase.Date == ClaseSeleccionada.Fecha.Date);

                    if (asistenciaExistente != null)
                    {
                        // CORREGIR REGISTRO EXISTENTE
                        asistenciaExistente.Presente = estudiante.Presente;

                        await datosService.GuardarAsistenciaAsync(
                            asistenciaExistente);
                    }
                    else
                    {
                        // CREAR NUEVO REGISTRO
                        Asistencia nuevaAsistencia = new Asistencia
                        {
                            LegajoEstudiante = estudiante.Legajo,
                            FechaClase = ClaseSeleccionada.Fecha,
                            Comision = ClaseSeleccionada.Comision,
                            Presente = estudiante.Presente
                        };

                        await datosService.GuardarAsistenciaAsync(
                            nuevaAsistencia);
                    }
                }

                await MostrarMensaje(
                    "Asistencia registrada",
                    "Los datos se guardaron correctamente en SQLite.");
            }
            catch (Exception ex)
            {
                await MostrarMensaje(
                    "Error de SQLite",
                    $"No se pudieron guardar las asistencias: {ex.Message}");
            }
            finally
            {
                PuedeGuardar = ClaseSeleccionada != null &&
                              EstudiantesAsistencia.Count > 0;
            }
        }

        // MOSTRAR MENSAJES
        private async Task MostrarMensaje(string titulo, string mensaje)
        {
            if (Shell.Current != null)
            {
                await Shell.Current.DisplayAlertAsync(
                    titulo, mensaje, "OK");
            }
        }
    }
}
