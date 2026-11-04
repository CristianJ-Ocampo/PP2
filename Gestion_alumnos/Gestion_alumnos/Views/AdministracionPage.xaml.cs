
using Gestion_alumnos.Servicios;
using System.Globalization;

namespace Gestion_alumnos
{
    public partial class AdministracionPage : ContentPage
    {
        private readonly DatosService datosService;

        public AdministracionPage(DatosService datosService)
        {
            InitializeComponent();

            this.datosService = datosService;
        }

        // CARGAR LOS UMBRALES ACTUALES
        protected override void OnAppearing()
        {
            base.OnAppearing();

            var configuracion = datosService.ConfiguracionRiesgo;

            UmbralMedioEntry.Text =
                configuracion.UmbralMedio.ToString(
                    CultureInfo.CurrentCulture);

            UmbralAltoEntry.Text =
                configuracion.UmbralAlto.ToString(
                    CultureInfo.CurrentCulture);
        }

        // GUARDAR CONFIGURACIÓN DE RIESGO
        private async void GuardarConfiguracion_Clicked(
            object sender, EventArgs e)
        {
            // VALIDAR LOS VALORES INGRESADOS
            bool medioValido = double.TryParse(
                UmbralMedioEntry.Text,
                NumberStyles.Float,
                CultureInfo.CurrentCulture,
                out double umbralMedio);

            bool altoValido = double.TryParse(
                UmbralAltoEntry.Text,
                NumberStyles.Float,
                CultureInfo.CurrentCulture,
                out double umbralAlto);

            if (!medioValido || !altoValido)
            {
                await DisplayAlertAsync(
                    "Valores incorrectos",
                    "Ingresá porcentajes numéricos válidos.",
                    "Aceptar");

                return;
            }

            try
            {
                // GUARDAR EN SQLITE
                await datosService.GuardarConfiguracionRiesgoAsync(
                    umbralMedio,
                    umbralAlto);

                await DisplayAlertAsync(
                    "Configuración guardada",
                    "Los umbrales de riesgo se actualizaron correctamente.",
                    "Aceptar");
            }
            catch (ArgumentException ex)
            {
                await DisplayAlertAsync(
                    "Valores incorrectos",
                    ex.Message,
                    "Aceptar");
            }
            catch (Exception ex)
            {
                await DisplayAlertAsync(
                    "Error",
                    $"No se pudo guardar la configuración: {ex.Message}",
                    "Aceptar");
            }
        }

        // ELIMINAR TODOS LOS DATOS DE PRUEBA
        private async void LimpiarDatos_Clicked(
            object sender, EventArgs e)
        {
            bool confirmar = await DisplayAlertAsync(
                "Confirmar eliminación",
                "¿Estás seguro de que querés eliminar TODOS los datos? Esta acción no se puede deshacer.",
                "Eliminar",
                "Cancelar");

            if (!confirmar)
                return;

            try
            {
                await datosService.LimpiarDatosAsync();

                await DisplayAlertAsync(
                    "Limpieza completada",
                    "Todos los datos de prueba fueron eliminados.",
                    "Aceptar");
            }
            catch (Exception ex)
            {
                await DisplayAlertAsync(
                    "Error",
                    $"No se pudieron eliminar los datos: {ex.Message}",
                    "Aceptar");
            }
        }
    }
}