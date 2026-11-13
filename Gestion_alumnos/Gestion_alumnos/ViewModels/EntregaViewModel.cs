using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Gestion_alumnos.Modelos;
using Gestion_alumnos.Servicios;

namespace Gestion_alumnos.ViewModels
{
    public class EntregaViewModel : INotifyPropertyChanged
    {
        private readonly DatosService datosService;

        // LISTA CENTRALIZADA DE COMISIONES
        public IReadOnlyList<string> Comisiones => ComisionesService.Todas;

        public ObservableCollection<TrabajoPractico> TrabajosDisponibles { get; } = new();

        public ObservableCollection<EstudianteEntrega> EstudiantesEntrega { get; } = new();

        private string? comisionSeleccionada;

        public string? ComisionSeleccionada
        {
            get => comisionSeleccionada;
            set
            {
                if (comisionSeleccionada == value)
                    return;

                comisionSeleccionada = value;
                OnPropertyChanged();
                ActualizarTrabajos();
            }
        }

        private TrabajoPractico? trabajoSeleccionado;

        public TrabajoPractico? TrabajoSeleccionado
        {
            get => trabajoSeleccionado;
            set
            {
                if (trabajoSeleccionado == value)
                    return;

                trabajoSeleccionado = value;
                OnPropertyChanged();
                ActualizarEstudiantes();
            }
        }

        private string mensajeEstado = "Seleccioná una comisión y un trabajo práctico.";

        public string MensajeEstado
        {
            get => mensajeEstado;
            set
            {
                mensajeEstado = value;
                OnPropertyChanged();
            }
        }

        private bool puedeGuardar;

        public bool PuedeGuardar
        {
            get => puedeGuardar;
            set
            {
                puedeGuardar = value;
                OnPropertyChanged();
            }
        }

        public ICommand GuardarEntregasCommand { get; }

        public EntregaViewModel(DatosService datosService)
        {
            this.datosService = datosService;

            GuardarEntregasCommand = new Command(async () => await GuardarEntregasAsync());
        }

        private void ActualizarTrabajos()
        {
            TrabajoSeleccionado = null;
            TrabajosDisponibles.Clear();
            EstudiantesEntrega.Clear();
            PuedeGuardar = false;

            if (string.IsNullOrWhiteSpace(ComisionSeleccionada))
            {
                MensajeEstado = "Seleccioná una comisión.";
                return;
            }

            var trabajos = datosService.Trabajos
                .Where(t => t.Comision == ComisionSeleccionada)
                .OrderBy(t => t.FechaEntrega);

            foreach (var trabajo in trabajos)
            {
                TrabajosDisponibles.Add(trabajo);
            }

            MensajeEstado = TrabajosDisponibles.Count == 0
                ? "No hay trabajos prácticos cargados para esta comisión."
                : "Seleccioná un trabajo práctico.";
        }

        private void ActualizarEstudiantes()
        {
            EstudiantesEntrega.Clear();
            PuedeGuardar = false;

            if (TrabajoSeleccionado == null)
            {
                MensajeEstado = "Seleccioná un trabajo práctico.";
                return;
            }

            var estudiantes = datosService.Estudiantes
                .Where(e => e.Comision == TrabajoSeleccionado.Comision)
                .OrderBy(e => e.Apellido)
                .ThenBy(e => e.Nombre);

            foreach (var estudiante in estudiantes)
            {
                var entregaExistente = datosService.Entregas.FirstOrDefault(e =>
                    e.LegajoEstudiante == estudiante.Legajo &&
                    e.Comision == TrabajoSeleccionado.Comision &&
                    e.NombreTrabajo == TrabajoSeleccionado.Nombre);

                EstudiantesEntrega.Add(new EstudianteEntrega
                {
                    Legajo = estudiante.Legajo,
                    NombreCompleto = $"{estudiante.Apellido}, {estudiante.Nombre}",
                    Entregado = entregaExistente?.Entregado ?? false
                });
            }

            PuedeGuardar = EstudiantesEntrega.Count > 0;

            MensajeEstado = PuedeGuardar
                ? "Marcá los estudiantes que entregaron el trabajo."
                : "No hay estudiantes cargados en esta comisión.";
        }

        private async Task GuardarEntregasAsync()
        {
            if (TrabajoSeleccionado == null || !PuedeGuardar)
                return;

            PuedeGuardar = false;

            try
            {
                foreach (var estudiante in EstudiantesEntrega)
                {
                    var entregaExistente = datosService.Entregas.FirstOrDefault(e =>
                        e.LegajoEstudiante == estudiante.Legajo &&
                        e.Comision == TrabajoSeleccionado.Comision &&
                        e.NombreTrabajo == TrabajoSeleccionado.Nombre);

                    if (entregaExistente != null)
                    {
                        // ACTUALIZAR ENTREGA EXISTENTE
                        entregaExistente.Entregado = estudiante.Entregado;

                        await datosService.GuardarEntregaAsync(entregaExistente);
                    }
                    else
                    {
                        // CREAR NUEVA ENTREGA
                        var nuevaEntrega = new Entrega
                        {
                            LegajoEstudiante = estudiante.Legajo,
                            NombreTrabajo = TrabajoSeleccionado.Nombre,
                            Comision = TrabajoSeleccionado.Comision,
                            Entregado = estudiante.Entregado
                        };

                        await datosService.GuardarEntregaAsync(nuevaEntrega);
                    }
                }

                MensajeEstado = "Entregas guardadas correctamente.";

                await Shell.Current.DisplayAlertAsync("Registro de entregas","Las entregas se guardaron correctamente.","Aceptar");
            }
            catch (Exception ex)
            {
                MensajeEstado = "Error al guardar las entregas.";

                await Shell.Current.DisplayAlertAsync("Error",$"No se pudieron guardar las entregas: {ex.Message}","Aceptar");
            }
            finally
            {
                PuedeGuardar = EstudiantesEntrega.Count > 0;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string? nombre = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nombre));
        }
    }
}
