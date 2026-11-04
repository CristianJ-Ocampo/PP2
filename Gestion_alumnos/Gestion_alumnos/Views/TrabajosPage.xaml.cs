
using Gestion_alumnos.ViewModels;

namespace Gestion_alumnos
{
    public partial class TrabajosPage : ContentPage
    {
        public TrabajosPage(CalendarioViewModel viewModel)
        {
            InitializeComponent();

            BindingContext = viewModel;
        }
    }
}