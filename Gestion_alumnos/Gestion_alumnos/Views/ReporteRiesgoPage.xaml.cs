using Gestion_alumnos.ViewModels;

namespace Gestion_alumnos
{
    public partial class ReporteRiesgoPage : ContentPage
    {
        public ReporteRiesgoPage(ReporteRiesgoViewModel viewModel)
        {
            InitializeComponent();

            BindingContext = viewModel;
        }
    }
}