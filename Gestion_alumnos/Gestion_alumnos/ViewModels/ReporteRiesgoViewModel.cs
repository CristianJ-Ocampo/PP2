
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Gestion_alumnos.Servicios;

namespace Gestion_alumnos.ViewModels
{
    public class ReporteRiesgoViewModel : INotifyPropertyChanged
    {
        private readonly DatosService datosService;
        private readonly RiesgoService riesgoService;

        public IReadOnlyList<string> Comisiones =>
            ComisionesService.Todas;

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
                ActualizarReporte();
            }
        }

        private int totalEstudiantes;

        public int TotalEstudiantes
        {
            get => totalEstudiantes;
            set
            {
                totalEstudiantes = value;
                OnPropertyChanged();
            }
        }

        private int estudiantesEnRiesgo;

        public int EstudiantesEnRiesgo
        {
            get => estudiantesEnRiesgo;
            set
            {
                estudiantesEnRiesgo = value;
                OnPropertyChanged();
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

        public ICommand ActualizarReporteCommand { get; }

        public ReporteRiesgoViewModel(
            DatosService datosService,
            RiesgoService riesgoService)
        {
            this.datosService = datosService;
            this.riesgoService = riesgoService;

            ActualizarReporteCommand = new Command(ActualizarReporte);
        }

        private void ActualizarReporte()
        {
            TotalEstudiantes = 0;
            EstudiantesEnRiesgo = 0;

            if (string.IsNullOrWhiteSpace(ComisionSeleccionada))
            {
                MensajeEstado = "Seleccioná una comisión.";
                return;
            }

            TotalEstudiantes = datosService.Estudiantes.Count(e =>
                e.Comision == ComisionSeleccionada);

            if (TotalEstudiantes == 0)
            {
                MensajeEstado =
                    "No hay estudiantes cargados en esta comisión.";
                return;
            }

            // REUTILIZAR EL CÁLCULO EXISTENTE
            var resultados = riesgoService.CalcularRiesgos()
                .Where(r => r.Comision == ComisionSeleccionada)
                .ToList();

            // COMPROBAR SI EXISTEN DATOS ACADÉMICOS
            bool hayDatos = resultados.Any(r =>
                r.NivelRiesgo != "Sin datos");

            if (!hayDatos)
            {
                MensajeEstado =
                    "No hay datos para calcular el riesgo.";
                return;
            }

            // CONTAR ESTUDIANTES EN RIESGO MEDIO O ALTO
            EstudiantesEnRiesgo = resultados.Count(r =>
                r.NivelRiesgo == "Medio" ||
                r.NivelRiesgo == "Alto");

            MensajeEstado =
                $"{EstudiantesEnRiesgo} de {TotalEstudiantes} " +
                "estudiantes en riesgo.";
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