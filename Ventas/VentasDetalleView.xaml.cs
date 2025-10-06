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
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                SqlDataAdapter da = new SqlDataAdapter("SELECT id_medicamento, nombre_comercial, precio_unitario FROM Medicamento", con);
                DataTable dt = new DataTable();
                da.Fill(dt);
                cmbMedicamentos.ItemsSource = dt.DefaultView;
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
            int idUsuario = SesionActual.Usuario.id_usuario; // 👈 tu usuario logueado
            MessageBox.Show(idUsuario.ToString());

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                SqlTransaction tx = con.BeginTransaction();

                try
                {
                    // Insertar venta
                    SqlCommand cmdVenta = new SqlCommand(
                        "INSERT INTO Venta (fecha_venta, id_cliente, id_usuario) OUTPUT INSERTED.id_venta VALUES (@fecha, @id_cliente, @id_usuario)", con, tx);
                    cmdVenta.Parameters.AddWithValue("@fecha", DateTime.Now);
                    cmdVenta.Parameters.AddWithValue("@id_cliente", idCliente);
                    cmdVenta.Parameters.AddWithValue("@id_usuario", idUsuario);

                    int idVenta = (int)cmdVenta.ExecuteScalar();

                    // Insertar detalle
                    foreach (var d in detalles)
                    {
                        SqlCommand cmdDet = new SqlCommand(
                            "INSERT INTO Detalle_Venta (id_venta, id_medicamento, cantidad, subtotal) VALUES (@idv, @idm, @cant, @sub)", con, tx);
                        cmdDet.Parameters.AddWithValue("@idv", idVenta);
                        cmdDet.Parameters.AddWithValue("@idm", d.IdMedicamento);
                        cmdDet.Parameters.AddWithValue("@cant", d.Cantidad);
                        cmdDet.Parameters.AddWithValue("@sub", d.Subtotal);
                        cmdDet.ExecuteNonQuery();
                    }

                    tx.Commit();
                    MessageBox.Show("Venta registrada correctamente.");

                    detalles.Clear();
                    dgDetalle.ItemsSource = null;
                    txtTotal.Text = "";
                }
                catch (Exception ex)
                {
                    tx.Rollback();
                    MessageBox.Show("Error al registrar venta: " + ex.Message);
                }
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

