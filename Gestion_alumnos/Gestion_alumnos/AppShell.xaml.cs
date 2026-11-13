
namespace Gestion_alumnos
{
    public partial class AppShell : Shell
    {
        public AppShell(
            MainPage estudiantesPage,
            CalendarioPage calendarioPage,
            TrabajosPage trabajosPage,
            AsistenciaPage asistenciaPage,
            EntregaPage entregaPage,
            PanelPage panelPage,
            ReporteRiesgoPage reporteRiesgoPage,
            AdministracionPage administracionPage)
        {
            InitializeComponent();


            Routing.RegisterRoute("HistorialEstudiante",typeof(HistorialEstudiantePage));
            EstudiantesTab.Content = estudiantesPage;
            CalendarioTab.Content = calendarioPage;
            TrabajosTab.Content = trabajosPage;
            AsistenciaTab.Content = asistenciaPage;
            EntregasTab.Content = entregaPage;
            PanelTab.Content = panelPage;
            ReporteRiesgoTab.Content = reporteRiesgoPage;
            AdministracionTab.Content = administracionPage;
        }
    }
}