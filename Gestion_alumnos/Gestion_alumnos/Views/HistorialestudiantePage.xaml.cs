using Gestion_alumnos.Modelos;
using Gestion_alumnos.Servicios;

namespace Gestion_alumnos
{
    public partial class HistorialEstudiantePage : ContentPage
    {
        private readonly DatosService datosService;
        private readonly RiesgoService riesgoService;
        private readonly int legajo;

        public HistorialEstudiantePage(DatosService datosService,RiesgoService riesgoService,int legajo)
        {
            InitializeComponent();

            this.datosService = datosService;
            this.riesgoService = riesgoService;
            this.legajo = legajo;

            CargarHistorial();
            CargarIntervenciones();
        }

        private void CargarHistorial()
        {
            var estudiante = datosService.Estudiantes
                .FirstOrDefault(e => e.Legajo == legajo);

            if (estudiante == null)
            {
                NombreEstudianteLabel.Text =
                    "Estudiante no encontrado.";
                return;
            }

            // DATOS PERSONALES
            NombreEstudianteLabel.Text =
                $"Estudiante: {estudiante.Apellido}, {estudiante.Nombre}";

            LegajoEstudianteLabel.Text =
                $"Legajo: {estudiante.Legajo}";

            ComisionEstudianteLabel.Text =
                $"Comisión: {estudiante.Comision}";

            // HISTORIAL DE ASISTENCIAS
            ContenedorAsistencias.Children.Clear();

            var clases = datosService.Clases
                .Where(c => c.Comision == estudiante.Comision)
                .OrderBy(c => c.Fecha)
                .ToList();

            if (clases.Count == 0)
            {
                ContenedorAsistencias.Children.Add(
                    new Label
                    {
                        Text = "No hay clases registradas."
                    });
            }

            foreach (var clase in clases)
            {
                var asistencia = datosService.Asistencias
                    .FirstOrDefault(a =>
                        a.LegajoEstudiante == legajo &&
                        a.Comision == estudiante.Comision &&
                        a.FechaClase.Date == clase.Fecha.Date);

                string estado = asistencia == null
                    ? "Sin registrar"
                    : asistencia.Presente
                        ? "Presente"
                        : "Ausente";

                ContenedorAsistencias.Children.Add(
                    new Label
                    {
                        Text = $"{clase.Fecha:dd/MM/yyyy} - {estado}",
                        FontSize = 16,
                        TextColor = estado == "Ausente"
                            ? Colors.Red
                            : estado == "Presente"
                                ? Colors.Green
                                : Colors.Gray
                    });
            }

            // HISTORIAL DE TRABAJOS PRÁCTICOS
            ContenedorTrabajos.Children.Clear();

            var trabajos = datosService.Trabajos
                .Where(t => t.Comision == estudiante.Comision)
                .OrderBy(t => t.FechaEntrega)
                .ToList();

            if (trabajos.Count == 0)
            {
                ContenedorTrabajos.Children.Add(
                    new Label
                    {
                        Text = "No hay trabajos prácticos registrados."
                    });
            }

            foreach (var trabajo in trabajos)
            {
                var entrega = datosService.Entregas
                    .FirstOrDefault(e =>
                        e.LegajoEstudiante == legajo &&
                        e.Comision == estudiante.Comision &&
                        e.NombreTrabajo == trabajo.Nombre);

                string estado = entrega == null
                    ? "Sin registrar"
                    : entrega.Entregado
                        ? "Entregado"
                        : "No entregado";

                ContenedorTrabajos.Children.Add(
                    new Label
                    {
                        Text =
                            $"{trabajo.Nombre} - " +
                            $"{trabajo.FechaEntrega:dd/MM/yyyy} - " +
                            estado,
                        FontSize = 16,
                        TextColor = estado == "No entregado"
                            ? Colors.Red
                            : estado == "Entregado"
                                ? Colors.Green
                                : Colors.Gray
                    });
            }

            // RIESGO ACADÉMICO
            var riesgo = riesgoService.CalcularRiesgos()
                .FirstOrDefault(r => r.Legajo == legajo);

            if (riesgo != null)
            {
                RiesgoEstudianteLabel.Text =
                    $"Nivel: {riesgo.NivelRiesgo}\n" +
                    $"Riesgo total: {riesgo.PorcentajeRiesgo:F1}%\n" +
                    $"Inasistencias: {riesgo.PorcentajeInasistencias:F1}%\n" +
                    $"Trabajos no entregados: {riesgo.PorcentajeNoEntregas:F1}%";

                // COLOR SEGÚN NIVEL DE RIESGO
                switch (riesgo.NivelRiesgo?.ToString().Trim().ToLowerInvariant())
                {
                    case "bajo":
                        RiesgoBorder.BackgroundColor = Color.FromArgb("#DCFCE7");
                        RiesgoBorder.Stroke = Color.FromArgb("#86EFAC");
                        RiesgoEstudianteLabel.TextColor = Color.FromArgb("#166534");
                        break;

                    case "medio":
                        RiesgoBorder.BackgroundColor = Color.FromArgb("#FEF3C7");
                        RiesgoBorder.Stroke = Color.FromArgb("#FCD34D");
                        RiesgoEstudianteLabel.TextColor = Color.FromArgb("#92400E");
                        break;

                    case "alto":
                        RiesgoBorder.BackgroundColor = Color.FromArgb("#FEE2E2");
                        RiesgoBorder.Stroke = Color.FromArgb("#FCA5A5");
                        RiesgoEstudianteLabel.TextColor = Color.FromArgb("#991B1B");
                        break;

                    default:
                        RiesgoBorder.BackgroundColor = Color.FromArgb("#EEF3F9");
                        RiesgoBorder.Stroke = Color.FromArgb("#DEE6EF");
                        RiesgoEstudianteLabel.TextColor = Color.FromArgb("#334155");
                        break;
                }
            }
            else
            {
                RiesgoEstudianteLabel.Text = "Sin información de riesgo.";

                RiesgoBorder.BackgroundColor = Color.FromArgb("#EEF3F9");
                RiesgoBorder.Stroke = Color.FromArgb("#DEE6EF");
                RiesgoEstudianteLabel.TextColor = Color.FromArgb("#334155");
            }
        }

        private async void Volver_Clicked(
    object sender, EventArgs e)
        {
            await Shell.Current.Navigation.PopAsync();
        }

        private async void GuardarIntervencion_Clicked(object sender, EventArgs e)
        {
            // OBTENER DATOS DEL FORMULARIO
            DateTime fecha = FechaIntervencionPicker.Date ?? DateTime.Today;

            string medio =
                MedioContactoPicker.SelectedItem?.ToString() ?? "";

            string motivo =
                MotivoIntervencionEditor.Text?.Trim() ?? "";

            string resultado =
                ResultadoIntervencionEditor.Text?.Trim() ?? "";

            // VALIDAR CAMPOS
            if (string.IsNullOrWhiteSpace(medio) ||
                string.IsNullOrWhiteSpace(motivo) ||
                string.IsNullOrWhiteSpace(resultado))
            {
                await DisplayAlertAsync(
                    "Datos incompletos",
                    "Completá todos los campos de la intervención.",
                    "Aceptar");

                return;
            }

            // CREAR INTERVENCIÓN
            var nuevaIntervencion = new Intervencion
            {
                LegajoEstudiante = legajo,
                Fecha = fecha,
                MedioContacto = medio,
                Motivo = motivo,
                Resultado = resultado
            };

            try
            {
                // GUARDAR EN SQLITE
                await datosService.GuardarIntervencionAsync(
                    nuevaIntervencion);

                // LIMPIAR FORMULARIO
                MedioContactoPicker.SelectedIndex = -1;
                MotivoIntervencionEditor.Text = "";
                ResultadoIntervencionEditor.Text = "";

                // ACTUALIZAR HISTORIAL
                CargarIntervenciones();

                await DisplayAlertAsync(
                    "Intervención registrada",
                    "El contacto fue guardado correctamente.",
                    "Aceptar");
            }
            catch (Exception ex)
            {
                await DisplayAlertAsync(
                    "Error",
                    $"No se pudo guardar la intervención: {ex.Message}",
                    "Aceptar");
            }
        }
        private void CargarIntervenciones()
        {
            ContenedorIntervenciones.Children.Clear();

            var intervenciones = datosService.Intervenciones
                .Where(i => i.LegajoEstudiante == legajo)
                .OrderByDescending(i => i.Fecha)
                .ThenByDescending(i => i.Id)
                .ToList();

            if (intervenciones.Count == 0)
            {
                ContenedorIntervenciones.Children.Add(
                    new Label
                    {
                        Text = "No hay intervenciones registradas.",
                        TextColor = Colors.Gray
                    });

                return;
            }

            foreach (var intervencion in intervenciones)
            {
                var contenido = new VerticalStackLayout
                {
                    Spacing = 6
                };

                contenido.Children.Add(new Label
                {
                    Text = $"Fecha: {intervencion.Fecha:dd/MM/yyyy}",
                    FontAttributes = FontAttributes.Bold
                });

                contenido.Children.Add(new Label
                {
                    Text = $"Medio: {intervencion.MedioContacto}"
                });

                contenido.Children.Add(new Label
                {
                    Text = $"Motivo: {intervencion.Motivo}"
                });

                contenido.Children.Add(new Label
                {
                    Text = $"Resultado: {intervencion.Resultado}"
                });

                var tarjeta = new Border
                {
                    Stroke = Colors.Gray,
                    StrokeThickness = 1,
                    Padding = new Thickness(12),
                    Content = contenido
                };

                ContenedorIntervenciones.Children.Add(tarjeta);
            }
        }
    }
}