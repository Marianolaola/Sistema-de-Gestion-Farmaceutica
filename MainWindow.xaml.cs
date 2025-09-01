using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Sistema_de_Gestión_Farmacéutica
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(string nombreUsuario)
        {
            InitializeComponent();
            this.Title = $"Gestion de Inventario - Usuario: {nombreUsuario}";
        }

        private void AbrirInventario_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Inventario Abierto");
        }

        public MainWindow() : this("Usuario por defecto")
        {

        }

        public void AbrirReporte_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Reporte Abierto");
        }

        public void CerrarSesion_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }

        private void AbrirConfiguracion_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Configuracion Abierta");
        }

        public void AcercaDe_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Gestión de Inventario\nVersión 1.0");
        }

        private List<Productos> productos = new List<Productos>();
        private int contadorId = 1;

        private void AgregarProducto_Click(object sender, RoutedEventArgs e)
        {
            string nombre = Microsoft.VisualBasic.Interaction.InputBox("Nombre del producto", "Agregar Producto", "");
            if (string.IsNullOrEmpty(nombre)) return;

            string principio_activo = Microsoft.VisualBasic.Interaction.InputBox("Principio Activo", "Agregar Producto", "");
            if (string.IsNullOrEmpty(principio_activo)) return;

            if (!int.TryParse(Microsoft.VisualBasic.Interaction.InputBox("Cantidad", "Agregar Producto", ""), out int cantidad)) return;
            if (!int.TryParse(Microsoft.VisualBasic.Interaction.InputBox("Stock_Minimo", "Agregar Producto", ""), out int stock_minimo)) return;
            if (!decimal.TryParse(Microsoft.VisualBasic.Interaction.InputBox("Precio", "Agregar Producto", ""), out decimal precio)) return;
            if (!DateTime.TryParse(Microsoft.VisualBasic.Interaction.InputBox("Fecha de Vencimiento", "Agregar Producto", ""), out DateTime fecha_vencimiento)) return;

            Productos nuevo = new Productos()
            {
                Id = contadorId++,
                Nombre = nombre,
                Principio_Activo = principio_activo,
                Cantidad = cantidad,
                Stock_Minimo = stock_minimo,
                Precio = precio,
                Fecha_Vencimiento = fecha_vencimiento
            };

            productos.Add(nuevo);
            RefrescarListado();

        }

        private void RefrescarListado()
        {
            dgProductos.ItemsSource = null;
            dgProductos.ItemsSource = productos;
        }

        private void EliminarProducto_Click(object sender, RoutedEventArgs e)
        {
            if (dgProductos.SelectedItem == null) return;
            Productos seleccionado = (Productos)dgProductos.SelectedItem;
            productos.Remove(seleccionado);
            RefrescarListado();
        }
    }
}