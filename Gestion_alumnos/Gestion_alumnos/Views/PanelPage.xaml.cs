
using Gestion_alumnos.ViewModels;
using Gestion_alumnos.Servicios;

namespace Gestion_alumnos
{
    public partial class PanelPage : ContentPage
    {
        private readonly DatosService datosService;
        private readonly RiesgoService riesgoService;

        public PanelPage(
            PanelViewModel vm,
            DatosService datosService,
            RiesgoService riesgoService)
        {
            InitializeComponent();

            BindingContext = vm;

            this.datosService = datosService;
            this.riesgoService = riesgoService;
        }

        private async void VerHistorial_Clicked(
            object sender, EventArgs e)
        {
            if (sender is not Button boton ||
                boton.CommandParameter is not int legajo)
            {
                return;
            }

            var pagina = new HistorialEstudiantePage(
                datosService,
                riesgoService,
                legajo);

            await Shell.Current.Navigation.PushAsync(pagina);
        }
    }
}