using Gestion_alumnos.Servicios;

namespace Gestion_alumnos
{
    public partial class App : Application
    {
        private readonly AppShell appShell;
        private readonly DatosService datosService;

        public App(AppShell appShell, DatosService datosService)
        {
            InitializeComponent();

            this.appShell = appShell;
            this.datosService = datosService;
           
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            Window ventana = new Window(appShell);

            ventana.Created += async (sender, args) =>
            {
                try
                {
                    // CARGAR LOS DATOS EXISTENTES
                    await datosService.CargarDatosAsync();
                    
                }
                catch (Exception ex)
                {
                    await Shell.Current.DisplayAlertAsync("Error de base de datos",ex.Message,"Aceptar");
                }
            };

            return ventana;
        }
    }
}