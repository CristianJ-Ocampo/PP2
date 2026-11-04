using Gestion_alumnos.ViewModels;

namespace Gestion_alumnos
{
    public partial class CalendarioPage : ContentPage
    {
        public CalendarioPage(CalendarioViewModel vm)
        {
            InitializeComponent();

            BindingContext = vm;
        }
    }
}