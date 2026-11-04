using Gestion_alumnos.Modelos;
using Gestion_alumnos.Servicios;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Gestion_alumnos.ViewModels
{
    public class CalendarioViewModel : INotifyPropertyChanged
    {
        private readonly DatosService datosService;

        // COLECCIONES
        public ObservableCollection<Clase> Clases { get; }
        public ObservableCollection<TrabajoPractico> Trabajos { get; }

        public ObservableCollection<Clase> ClasesVisibles { get; }
        public ObservableCollection<TrabajoPractico> TrabajosVisibles { get; }

        // COMISIÓN SELECCIONADA
        private string? comisionSeleccionada;

        public string? ComisionSeleccionada
        {
            get { return comisionSeleccionada; }
            set
            {
                comisionSeleccionada = value;
                OnPropertyChanged();
                ActualizarCalendario();
            }
        }

        // FECHA DE CLASE
        private DateTime fechaClase = DateTime.Today;

        public DateTime FechaClase
        {
            get { return fechaClase; }
            set
            {
                fechaClase = value;
                OnPropertyChanged();
            }
        }

        // NOMBRE DEL TRABAJO
        private string nombreTrabajo = "";

        public string NombreTrabajo
        {
            get { return nombreTrabajo; }
            set
            {
                nombreTrabajo = value;
                OnPropertyChanged();
            }
        }

        // FECHA DE ENTREGA
        private DateTime fechaEntrega = DateTime.Today;

        public DateTime FechaEntrega
        {
            get { return fechaEntrega; }
            set
            {
                fechaEntrega = value;
                OnPropertyChanged();
            }
        }

        // MENSAJES DE LISTAS VACÍAS
        private bool mostrarClasesVacias;

        public bool MostrarClasesVacias
        {
            get { return mostrarClasesVacias; }
            set
            {
                mostrarClasesVacias = value;
                OnPropertyChanged();
            }
        }

        private bool mostrarTrabajosVacios;

        public bool MostrarTrabajosVacios
        {
            get { return mostrarTrabajosVacios; }
            set
            {
                mostrarTrabajosVacios = value;
                OnPropertyChanged();
            }
        }

        // COMANDOS
        public ICommand AgregarClaseCommand { get; }
        public ICommand AgregarTrabajoCommand { get; }

        // NOTIFICACIÓN DE CAMBIOS
        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged(
            [CallerMemberName] string? nombrePropiedad = null)
        {
            PropertyChanged?.Invoke(
                this,
                new PropertyChangedEventArgs(nombrePropiedad));
        }

        // CONSTRUCTOR
        public CalendarioViewModel(DatosService datosService)
        {
            this.datosService = datosService;

            Clases = datosService.Clases;
            Trabajos = datosService.Trabajos;

            ClasesVisibles = new ObservableCollection<Clase>();
            TrabajosVisibles = new ObservableCollection<TrabajoPractico>();

            AgregarClaseCommand =
                new Command(async () => await AgregarClaseAsync());

            AgregarTrabajoCommand =
                new Command(async () => await AgregarTrabajoAsync());

            // ACTUALIZAR LAS LISTAS CUANDO CAMBIAN LOS DATOS
            Clases.CollectionChanged += (sender, args) =>
            {
                ActualizarCalendario();
            };

            Trabajos.CollectionChanged += (sender, args) =>
            {
                ActualizarCalendario();
            };
        }

        // AGREGAR CLASE
        private async Task AgregarClaseAsync()
        {
            if (string.IsNullOrWhiteSpace(ComisionSeleccionada))
            {
                await MostrarMensaje("Error","Debe seleccionar una comisión.");
                return;
            }

            foreach (Clase clase in Clases)
            {
                if (clase.Fecha.Date == FechaClase.Date &&
                    clase.Comision == ComisionSeleccionada)
                {
                    await MostrarMensaje("Clase repetida","Ya existe una clase en esa fecha para esta comisión.");
                    return;
                }
            }

            Clase nuevaClase = new Clase
            {
                Fecha = FechaClase,
                Comision = ComisionSeleccionada
            };

            // GUARDAR EN SQLITE
            try
            {
                await datosService.GuardarClaseAsync(nuevaClase);
            }
            catch (Exception ex)
            {
                await MostrarMensaje("Error de SQLite",$"No se pudo guardar la clase: {ex.Message}");
                return;
            }

            ActualizarCalendario();

            await MostrarMensaje("Clase registrada","La clase se agregó correctamente.");
        }

        // AGREGAR TRABAJO PRÁCTICO
        private async Task AgregarTrabajoAsync()
        {
            if (string.IsNullOrWhiteSpace(ComisionSeleccionada))
            {
                await MostrarMensaje("Error","Debe seleccionar una comisión.");
                return;
            }

            if (string.IsNullOrWhiteSpace(NombreTrabajo))
            {
                await MostrarMensaje("Error","Debe ingresar el nombre del trabajo práctico.");
                return;
            }

            string nombre = NombreTrabajo.Trim();

            foreach (TrabajoPractico trabajo in Trabajos)
            {
                if (trabajo.Nombre.Equals(nombre,StringComparison.OrdinalIgnoreCase) && trabajo.Comision == ComisionSeleccionada)
                {
                    await MostrarMensaje("Trabajo repetido","Ya existe un trabajo con ese nombre en esta comisión.");
                    return;
                }
            }

            TrabajoPractico nuevoTrabajo = new TrabajoPractico
            {
                Nombre = nombre,
                FechaEntrega = FechaEntrega,
                Comision = ComisionSeleccionada
            };

            // GUARDAR EN SQLITE
            try
            {
                await datosService.GuardarTrabajoAsync(nuevoTrabajo);
            }
            catch (Exception ex)
            {
                await MostrarMensaje("Error de SQLite",$"No se pudo guardar el trabajo: {ex.Message}");
                return;
            }

            ActualizarCalendario();

            NombreTrabajo = "";

            await MostrarMensaje("Trabajo registrado","El trabajo práctico se agregó correctamente.");
        }

        // FILTRAR POR COMISIÓN
        private void ActualizarCalendario()
        {
            ClasesVisibles.Clear();
            TrabajosVisibles.Clear();

            if (string.IsNullOrWhiteSpace(ComisionSeleccionada))
            {
                MostrarClasesVacias = false;
                MostrarTrabajosVacios = false;
                return;
            }

            var clasesFiltradas = Clases
                .Where(c => c.Comision == ComisionSeleccionada)
                .OrderBy(c => c.Fecha);

            foreach (Clase clase in clasesFiltradas)
            {
                ClasesVisibles.Add(clase);
            }

            var trabajosFiltrados = Trabajos
                .Where(t => t.Comision == ComisionSeleccionada)
                .OrderBy(t => t.FechaEntrega);

            foreach (TrabajoPractico trabajo in trabajosFiltrados)
            {
                TrabajosVisibles.Add(trabajo);
            }

            MostrarClasesVacias = ClasesVisibles.Count == 0;
            MostrarTrabajosVacios = TrabajosVisibles.Count == 0;
        }

        // MOSTRAR MENSAJES
        private async Task MostrarMensaje(string titulo, string mensaje)
        {
            if (Shell.Current != null)
            {
                await Shell.Current.DisplayAlertAsync(titulo, mensaje, "OK");
            }
        }
    }
}
