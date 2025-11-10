using Sistema_de_Gestión_Farmacéutica.Productos;
using Sistema_de_Gestión_Farmacéutica.Sesion;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Sistema_de_Gestión_Farmacéutica.Ventas
{
    /// <summary>
    /// Lógica de interacción para VentasDetalleView.xaml
    /// </summary>
    public partial class VentasDetalleView : UserControl
    {
        private string connectionString = "Server=localhost\\SQLEXPRESS; Database=SistemaFarmaceutico; Trusted_Connection=True; TrustServerCertificate=True;";
        private MedicamentoRepositorio medicamento = new MedicamentoRepositorio();
        private Dictionary<int, int> stockTemporal = new Dictionary<int, int>();
        private List<DetalleItem> detalles = new List<DetalleItem>();
        Factura factura = new Factura();

        public VentasDetalleView()
        {
            InitializeComponent();
            CargarClientes();
            CargarMedicamentos();
        }


        private void CargarClientes()
        {
            using (SqlConnection con = new SqlConnection(connectionString)) 
            { 
                con.Open(); 
                SqlDataAdapter da = new SqlDataAdapter("SELECT id_cliente, nombre, apellido, dni FROM Cliente", con); 
                DataTable dt = new DataTable(); 
                da.Fill(dt); 
                cmbClientes.ItemsSource = dt.DefaultView;
                cmbClientes.DisplayMemberPath = "nombre";
                cmbClientes.SelectedValuePath = "id_cliente";
            }
        }


        private void CargarMedicamentos()
        {
            try
            {
                DataTable dt = medicamento.ObtenerMedicamentosActivos();
                cmbMedicamentos.ItemsSource = dt.DefaultView;
                cmbMedicamentos.DisplayMemberPath = "nombre_comercial";
                cmbMedicamentos.SelectedValuePath = "id_medicamento";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar medicamentos: " + ex.Message);
            }
        }


        private void cmbClientes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbClientes.SelectedItem is DataRowView fila)
            {
                txtNombreCliente.Text = fila["nombre"].ToString() + " " + fila["apellido"].ToString();
                txtDniCliente.Text = fila["dni"].ToString();
            }
            else
            {
                txtNombreCliente.Clear();
                txtDniCliente.Clear();
            }
        }


        private void cmbMedicamentos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbMedicamentos.SelectedValue != null)
            {
                var fila = (DataRowView)cmbMedicamentos.SelectedItem;
                txtNombreMedicamento.Text = fila["nombre_comercial"].ToString();
                txtDescripcionMedicamento.Text = fila["presentacion"].ToString();
                txtPrecio.Text = fila["precio_unitario"].ToString();

                int idMedicamento = Convert.ToInt32(cmbMedicamentos.SelectedValue);
                int stock = medicamento.ObtenerStock(idMedicamento);

                if (stockTemporal.ContainsKey(idMedicamento))
                {
                    stock = stockTemporal[idMedicamento];
                }
                else
                {
                    stockTemporal[idMedicamento] = stock;
                }
                txtStock.Text = stock.ToString();
            }
        }


        private void btnAgregar_Click(object sender, RoutedEventArgs e)
        {
            if (cmbMedicamentos.SelectedValue == null || string.IsNullOrWhiteSpace(txtCantidad.Text))
            {
                MessageBox.Show("Seleccione un medicamento y cantidad válida.");
                return;
            }

            int idMed = Convert.ToInt32(cmbMedicamentos.SelectedValue);
            string nombre = ((DataRowView)cmbMedicamentos.SelectedItem)["nombre_comercial"].ToString();
            string presentacion = ((DataRowView)cmbMedicamentos.SelectedItem)["presentacion"].ToString();
            decimal precio = Convert.ToDecimal(((DataRowView)cmbMedicamentos.SelectedItem)["precio_unitario"]);
            int cantidad = Convert.ToInt32(txtCantidad.Text);
            decimal subtotal = cantidad * precio;

            int stockActual = medicamento.ObtenerStock(idMed);

            if (cantidad > stockActual)
            {
                MessageBox.Show($"No hay stock suficiente para el medicamento.\n" +
                                $"Disponible: {stockActual}, solicitado: {cantidad}",
                                $"Stock insuficiente", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int cantidadNueva = Convert.ToInt32(txtCantidad.Text);

            // buscamos si el medicamento ya está en la lista
            var existente = detalles.FirstOrDefault(x => x.IdMedicamento == idMed);
            if (existente != null)
            {
                // si ya existe se suma la cantidad seleccionada a la ya existente y se calcula el subtotal
                existente.Cantidad += cantidadNueva;
                existente.Subtotal = existente.Cantidad * existente.Precio;
            }
            else
            {
                // si no existe agrega un nuevo detalle
                detalles.Add(new DetalleItem
                {
                    IdMedicamento = idMed,
                    Nombre = nombre,
                    Presentacion = presentacion,
                    Cantidad = cantidadNueva,
                    Precio = precio,
                    Subtotal = cantidadNueva * precio
                });

                //Actualizar stock temporal
                stockTemporal[idMed] -= cantidad;
                txtStock.Text = stockTemporal[idMed].ToString();
            }

            dgDetalle.ItemsSource = null;
            dgDetalle.ItemsSource = detalles;

            txtTotal.Text = detalles.Sum(x => x.Subtotal).ToString("F2");

            txtNombreMedicamento.Clear();
            txtDescripcionMedicamento.Clear();
            txtPrecio.Clear();
            txtCantidad.Clear();
            txtStock.Clear();

            cmbMedicamentos.SelectedIndex = -1;
        }


        private void btnQuitar_Click(object sender, RoutedEventArgs e)
        {
            var boton = sender as Button;
            var detalle = boton?.DataContext as DetalleItem;

            if (detalle != null)
            {
                // consultar si se desea quitar el producto del detalle
                if (MessageBox.Show($"¿Desea quitar {detalle.Nombre} del detalle?", "Confirmar", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    // devolver la cantidad del detalle al stock
                    if (stockTemporal.ContainsKey(detalle.IdMedicamento))
                    {
                        stockTemporal[detalle.IdMedicamento] += detalle.Cantidad;
                    }

                    // refrescar datagrid
                    detalles.Remove(detalle);
                    dgDetalle.ItemsSource = null;
                    dgDetalle.ItemsSource = detalles;

                    // recalcular total
                    txtTotal.Text = detalles.Sum(x => x.Subtotal).ToString("F2");
                }
            }
        }


        private void btnConfirmar_Click(object sender, RoutedEventArgs e)
        {
            if (cmbClientes.SelectedValue == null || detalles.Count == 0)
            {
                MessageBox.Show("Seleccione un cliente y agregue al menos un medicamento.");
                return;
            }

            int idCliente = Convert.ToInt32(cmbClientes.SelectedValue);
            int idUsuario = SesionActual.Usuario.id_usuario; // id de la sesión actual

            VentaDetalleRepositorio repo = new VentaDetalleRepositorio();
            string mensaje;
            bool exito = repo.RegistrarVenta(idCliente, idUsuario, detalles, out mensaje);

            MessageBox.Show(mensaje);

            if (exito)
            {
                // descuenta el stock
                foreach (var item in detalles)
                {
                    medicamento.DescontarStock(item.IdMedicamento, item.Cantidad);
                }

                // pregunta si desea generar una factura
                var resultado = MessageBox.Show("Desea generar una factura de la venta?", "Generar Factura", MessageBoxButton.YesNo);
                if (resultado == MessageBoxResult.Yes)
                {
                    string nombreCliente = ((DataRowView)cmbClientes.SelectedItem)["nombre"].ToString();
                    DateTime fechaVenta = DateTime.Now;

                    factura.GenerarFacturaPDF(nombreCliente, fechaVenta, detalles);
                }

                // refresca el datagrid
                CargarMedicamentos();
                detalles.Clear();
                dgDetalle.ItemsSource = null;
                txtTotal.Text = "";
            }

        }

        private void btnLimpiar_Click(object sender, RoutedEventArgs e)
        {
            if (detalles == null || detalles.Count == 0)
                return;

            if (MessageBox.Show("¿Desea quitar todos los medicamentos del detalle?",
                                "Confirmar acción",
                                MessageBoxButton.YesNo,
                                MessageBoxImage.Question) == MessageBoxResult.Yes)
            {

                // devolver stock de cada medicamento
                foreach (var item in detalles)
                {
                    if (stockTemporal.ContainsKey(item.IdMedicamento))
                    {
                        stockTemporal[item.IdMedicamento] += item.Cantidad;
                    }
                }

                // limpiar el detalle
                detalles.Clear();

                // refrescar grilla (si es List)
                dgDetalle.ItemsSource = null;
                dgDetalle.ItemsSource = detalles;

                // recalcular total
                txtTotal.Text = "0.00";
            }
        }
    }

    public class DetalleItem
    {
        public int IdMedicamento { get; set; }
        public string Nombre { get; set; }
        public string Presentacion { get; set; }
        public int Cantidad { get; set; }
        public decimal Precio { get; set; }
        public decimal Subtotal { get; set; }
    }
}

