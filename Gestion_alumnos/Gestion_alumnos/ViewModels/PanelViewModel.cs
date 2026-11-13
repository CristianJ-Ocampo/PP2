
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Gestion_alumnos.Modelos;
using Gestion_alumnos.Servicios;

namespace Gestion_alumnos.ViewModels
{
    public class PanelViewModel : INotifyPropertyChanged
    {
        private readonly RiesgoService riesgoService;

        private readonly DatosService datosService;

        // LISTA CENTRALIZADA DE COMISIONES
        public IReadOnlyList<string> Comisiones => ComisionesService.Todas;

        public ObservableCollection<RiesgoEstudiante> EstudiantesRiesgo
        { get; } = new();

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
                ActualizarPanel();
            }
        }

        private string mensajeEstado = "Seleccioná una comisión.";

        public string MensajeEstado
        {
            get => mensajeEstado;
            set
            {
                mensajeEstado = value;
                OnPropertyChanged();
            }
        }

        public ICommand ActualizarPanelCommand { get; }

        public PanelViewModel(
    RiesgoService riesgoService,
    DatosService datosService)
        {
            this.riesgoService = riesgoService;
            this.datosService = datosService;

            ActualizarPanelCommand = new Command(ActualizarPanel);
        }

        private void ActualizarPanel()
        {
            EstudiantesRiesgo.Clear();

            if (string.IsNullOrWhiteSpace(ComisionSeleccionada))
            {
                MensajeEstado = "Seleccioná una comisión.";
                return;
            }

            var resultados = riesgoService.CalcularRiesgos()
                .Where(r => r.Comision == ComisionSeleccionada)
                .OrderByDescending(r => r.PorcentajeRiesgo);

            foreach (var resultado in resultados)
            {
                // BUSCAR INTERVENCIONES DEL ESTUDIANTE
                var intervenciones = datosService.Intervenciones
                    .Where(i => i.LegajoEstudiante == resultado.Legajo)
                    .OrderByDescending(i => i.Fecha)
                    .ThenByDescending(i => i.Id)
                    .ToList();

                if (intervenciones.Count > 0)
                {
                    // EL ESTUDIANTE FUE CONTACTADO
                    var ultima = intervenciones.First();

                    resultado.FueContactado = true;
                    resultado.EstadoContacto = "Contactado";

                    resultado.UltimoContacto =
                        $"Último contacto: {ultima.Fecha:dd/MM/yyyy}";

                    resultado.CantidadIntervenciones =
                        $"Intervenciones registradas: {intervenciones.Count}";
                }
                else
                {
                    // EL ESTUDIANTE NO TIENE INTERVENCIONES
                    resultado.FueContactado = false;
                    resultado.EstadoContacto =
                        "Sin intervenciones registradas";

                    resultado.UltimoContacto = "";

                    resultado.CantidadIntervenciones = "";
                }

                EstudiantesRiesgo.Add(resultado);
            }

            MensajeEstado = EstudiantesRiesgo.Count == 0
                ? "No hay estudiantes cargados en esta comisión."
                : $"Estudiantes encontrados: {EstudiantesRiesgo.Count}";
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged(
            [CallerMemberName] string? nombre = null)
        {
            PropertyChanged?.Invoke(
                this, new PropertyChangedEventArgs(nombre));
        }
    }
}
