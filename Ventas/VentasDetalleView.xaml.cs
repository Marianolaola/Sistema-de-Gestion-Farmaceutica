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
        private List<DetalleItem> detalles = new List<DetalleItem>();
        private Medicamento medicamento = new Medicamento();

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
                SqlDataAdapter da = new SqlDataAdapter("SELECT id_cliente, nombre FROM Cliente", con); 
                DataTable dt = new DataTable(); 
                da.Fill(dt); 
                cmbClientes.ItemsSource = dt.DefaultView; 
            }
        }

        private void CargarMedicamentos()
        {
            try
            {
                DataTable dt = medicamento.ObtenerMedicamentos();
                cmbMedicamentos.ItemsSource = dt.DefaultView;
                cmbMedicamentos.DisplayMemberPath = "nombre_comercial";
                cmbMedicamentos.SelectedValuePath = "id_medicamento";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar medicamentos: " + ex.Message);
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
            double precio = Convert.ToDouble(((DataRowView)cmbMedicamentos.SelectedItem)["precio_unitario"]);
            int cantidad = Convert.ToInt32(txtCantidad.Text);
            double subtotal = cantidad * precio;

            detalles.Add(new DetalleItem { IdMedicamento = idMed, Nombre = nombre, Cantidad = cantidad, Precio = precio, Subtotal = subtotal });

            dgDetalle.ItemsSource = null;
            dgDetalle.ItemsSource = detalles;

            txtTotal.Text = detalles.Sum(x => x.Subtotal).ToString("F2");
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
                detalles.Clear();
                dgDetalle.ItemsSource = null;
                txtTotal.Text = "";
            }
        }
    }

    public class DetalleItem
    {
        public int IdMedicamento { get; set; }
        public string Nombre { get; set; }
        public int Cantidad { get; set; }
        public double Precio { get; set; }
        public double Subtotal { get; set; }
    }
}

