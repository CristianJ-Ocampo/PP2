using Gestion_alumnos.ViewModels;

namespace Gestion_alumnos
{
    public partial class MainPage : ContentPage
    {
        public MainPage(MainViewModel vm)
        {
            InitializeComponent();

            BindingContext = vm;
        }
    }
}