using Gestion_alumnos.Modelos;
using Gestion_alumnos.Servicios;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Gestion_alumnos.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly DatosService datosService;
        public ObservableCollection<Estudiante> Estudiantes { get; }

        public ObservableCollection<Estudiante> EstudiantesVisibles { get; }

        // DATOS DEL FORMULARIO
        private string apellido = "";

        public string Apellido
        {
            get { return apellido; }
            set
            {
                apellido = value;
                OnPropertyChanged();
            }
        }

        private string nombre = "";

        public string Nombre
        {
            get { return nombre; }
            set
            {
                nombre = value;
                OnPropertyChanged();
            }
        }

        private string legajo = "";

        public string Legajo
        {
            get { return legajo; }
            set
            {
                legajo = value;
                OnPropertyChanged();
            }
        }

        private string? comisionSeleccionada;

        public string? ComisionSeleccionada
        {
            get { return comisionSeleccionada; }
            set
            {
                comisionSeleccionada = value;
                OnPropertyChanged();
                ActualizarNomina(value);
            }
        }

        // DATOS QUE MUESTRA LA PANTALLA
        private string totalTexto = "Seleccione una comisión";

        public string TotalTexto
        {
            get { return totalTexto; }
            set
            {
                totalTexto = value;
                OnPropertyChanged();
            }
        }

        private bool mostrarMensajeVacio;

        public bool MostrarMensajeVacio
        {
            get { return mostrarMensajeVacio; }
            set
            {
                mostrarMensajeVacio = value;
                OnPropertyChanged();
            }
        }

        // COMANDO PARA AGREGAR ESTUDIANTES
        public ICommand AgregarEstudianteCommand { get; }

        // NOTIFICACIÓN DE CAMBIOS
        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string? nombrePropiedad = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nombrePropiedad));
        }

        // CONSTRUCTOR
        public MainViewModel(DatosService datosService)
        {
            this.datosService = datosService;

            Estudiantes = datosService.Estudiantes;

            EstudiantesVisibles = new ObservableCollection<Estudiante>();

            AgregarEstudianteCommand = new Command(async () => await AgregarEstudianteAsync());

            Estudiantes.CollectionChanged += (sender, args) =>
            {
                ActualizarNomina(ComisionSeleccionada);
            };
        }

        // AGREGAR ESTUDIANTE
        private async Task AgregarEstudianteAsync()
        {
            if (string.IsNullOrWhiteSpace(Apellido))
            {
                await MostrarError("Error","El apellido no puede estar vacío.");
                return;
            }

            if (string.IsNullOrWhiteSpace(Nombre))
            {
                await MostrarError("Error","El nombre no puede estar vacío.");
                return;
            }

            if (!int.TryParse(Legajo, out int numeroLegajo)
                || numeroLegajo <= 0)
            {
                await MostrarError("Error","El legajo debe ser un número positivo válido.");
                return;
            }

            // VERIFICAR LEGAJO REPETIDO
            foreach (Estudiante estudiante in Estudiantes)
            {
                if (estudiante.Legajo == numeroLegajo)
                {
                    await MostrarError("Legajo repetido",$"El legajo {numeroLegajo} ya pertenece a " + $"{estudiante.Nombre} {estudiante.Apellido}.");
                    return;
                }
            }

            if (string.IsNullOrWhiteSpace(ComisionSeleccionada))
            {
                await MostrarError("Error","Debe seleccionar una comisión.");
                return;
            }

            // CREAR ESTUDIANTE
            Estudiante nuevoEstudiante = new Estudiante
            {
                Apellido = Apellido.Trim(),
                Nombre = Nombre.Trim(),
                Legajo = numeroLegajo,
                Comision = ComisionSeleccionada
            };

            try
            {
                await datosService.GuardarEstudianteAsync(nuevoEstudiante);
            }
            catch (Exception ex)
            {
                await MostrarError("Error de SQLite",$"No se pudo guardar el estudiante: {ex.Message}");
                return;
            }

            // ACTUALIZAR NÓMINA
            ActualizarNomina(ComisionSeleccionada);

            // LIMPIAR CAMPOS
            Apellido = "";
            Nombre = "";
            Legajo = "";
        }

        // MOSTRAR MENSAJES DE VALIDACIÓN
        private async Task MostrarError(string titulo, string mensaje)
        {
            if (Shell.Current != null)
            {
                await Shell.Current.DisplayAlertAsync(titulo, mensaje, "OK");
            }
        }

        // FILTRAR Y ORDENAR ESTUDIANTES
        public void ActualizarNomina(string? comision)
        {
            EstudiantesVisibles.Clear();

            if (string.IsNullOrWhiteSpace(comision))
            {
                TotalTexto = "Seleccione una comisión";
                MostrarMensajeVacio = false;
                return;
            }

            var filtrados = Estudiantes
                .Where(e => e.Comision == comision)
                .OrderBy(e => e.Apellido)
                .ThenBy(e => e.Nombre)
                .ToList();

            foreach (Estudiante estudiante in filtrados)
            {
                EstudiantesVisibles.Add(estudiante);
            }

            TotalTexto =
                $"Total: {EstudiantesVisibles.Count} estudiantes";

            MostrarMensajeVacio =
                EstudiantesVisibles.Count == 0;
        }
    }
}
