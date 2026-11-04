
using Gestion_alumnos.ViewModels;

namespace Gestion_alumnos
{
    public partial class AsistenciaPage : ContentPage
    {
        public AsistenciaPage(AsistenciaViewModel vm)
        {
            InitializeComponent();

            BindingContext = vm;
        }
    }
}