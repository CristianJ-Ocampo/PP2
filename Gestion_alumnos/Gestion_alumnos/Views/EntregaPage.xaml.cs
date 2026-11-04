using Gestion_alumnos.ViewModels;

namespace Gestion_alumnos
{
    public partial class EntregaPage : ContentPage
    {
        public EntregaPage(EntregaViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }
    }
}