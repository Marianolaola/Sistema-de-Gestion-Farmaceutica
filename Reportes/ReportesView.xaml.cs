using Sistema_de_Gestión_Farmacéutica.Productos;
using Sistema_de_Gestión_Farmacéutica.Usuarios;
using Sistema_de_Gestión_Farmacéutica.Ventas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Microsoft.Data.SqlClient;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data;

namespace Sistema_de_Gestión_Farmacéutica.Reportes
{
    public partial class ReportesView : UserControl
    {
        private Factura factura = new Factura();
        private UsuarioRepositorio usuarioRepo = new UsuarioRepositorio();
        private MedicamentoRepositorio medicamentoRepo = new MedicamentoRepositorio();
        private VentaDetalleRepositorio ventaRepo = new VentaDetalleRepositorio();
        private string connectionString = "Server=localhost\\SQLEXPRESS; Database=SistemaFarmaceutico; Trusted_Connection=True; TrustServerCertificate=True;";
        public ReportesView()
        {
            InitializeComponent();
            CargarFiltros();
        }


        private void CargarFiltros()
        {
            cmbVendedor.ItemsSource = usuarioRepo.ObtenerVendedores().DefaultView;
            cmbProducto.ItemsSource = medicamentoRepo.ObtenerMedicamentosActivos().DefaultView;
        }


        private void btnFiltrar_Click(object sender, RoutedEventArgs e)
        {
            DateTime? desde = dpDesde.SelectedDate;
            DateTime? hasta = dpHasta.SelectedDate;
            int? idVendedor = cmbVendedor.SelectedValue as int?;
            int? idProducto = cmbProducto.SelectedValue as int?;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                string query = @"
                                SELECT DISTINCT v.id_venta , v.fecha_venta, u.nombre + ' ' + u.apellido AS nombre_vendedor,
                                       c.nombre + ' ' + c.apellido AS nombre_cliente,
                                       v.total
                                FROM venta v
                                INNER JOIN Usuario u ON v.id_usuario = u.id_usuario
                                INNER JOIN Cliente c ON v.id_cliente = c.id_cliente
                                INNER JOIN Detalle_Venta dv ON v.id_venta = dv.id_venta
                                INNER JOIN Medicamento m ON dv.id_medicamento = m.id_medicamento
                                WHERE (v.fecha_venta BETWEEN @desde AND @hasta)
                                AND (@idVendedor IS NULL OR v.id_usuario = @idVendedor)
                                AND (@idProducto IS NULL OR m.id_medicamento = @idProducto)
                                ORDER BY v.fecha_venta DESC";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@desde", (object)desde ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@hasta", (object)hasta ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@idVendedor", (object)idVendedor ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@idProducto", (object)idProducto ?? DBNull.Value);


                DataTable dt = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                dgVentas.ItemsSource = dt.DefaultView;
            }
        }

        private void dgVentas_LoadingRowDetails(object sender, DataGridRowDetailsEventArgs e)
        {
            DataRowView filaVenta = e.Row.Item as DataRowView;
            if (filaVenta == null) return;

            int idVenta = Convert.ToInt32(filaVenta["id_venta"]);

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                string query = @"SELECT m.nombre_comercial AS nombre_medicamento, 
                                        dv.cantidad, dv.subtotal / dv.cantidad AS precio_unitario, dv.subtotal
                                 FROM Detalle_Venta dv
                                 INNER JOIN Medicamento m ON dv.id_medicamento = m.id_medicamento
                                 WHERE dv.id_venta = @idVenta";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@idVenta", idVenta);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dtDetalles = new DataTable();
                da.Fill(dtDetalles);

                // Busca el datagrid interno del RowDetails y le asigna los detalles
                DataGrid dgDetalles = e.DetailsElement.FindName("dgDetalles") as DataGrid;
                if (dgDetalles != null)
                {
                    dgDetalles.ItemsSource = dtDetalles.DefaultView;
                }
            }
        }

        private void dgVentas_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var row = (DataGridRow)dgVentas.ItemContainerGenerator.ContainerFromItem(dgVentas.SelectedItem);
            if (row != null)
            {
                row.DetailsVisibility = row.DetailsVisibility == Visibility.Visible
                    ? Visibility.Collapsed
                    : Visibility.Visible;
            }
        }

        private void btnImprimirReporte_Click(object sender, RoutedEventArgs e)
        {
            if (dgVentas.ItemsSource is DataView view)
            {
                DateTime? desde = dpDesde.SelectedDate;
                DateTime? hasta = dpHasta.SelectedDate;
                string vendedor = cmbVendedor.Text;
                string producto = cmbProducto.Text;

                string filtros = "";
                if (desde.HasValue && hasta.HasValue)
                    filtros += $"Fechas: {desde:dd/MM/yyyy} - {hasta:dd/MM/yyyy}";
                if (!string.IsNullOrEmpty(vendedor))
                    filtros += $" | Vendedor: {vendedor}";
                if (!string.IsNullOrEmpty(producto))
                    filtros += $" | Producto: {producto}";
                if (string.IsNullOrEmpty(filtros))
                    filtros = "Sin filtros aplicados";

                DataTable dt = view.ToTable();
                factura.GenerarReportePDF(dt, ventaRepo.ObtenerDetallesPorVenta, filtros);
            } else
            {
                MessageBox.Show("No hay datos para imprimir el reporte");
            }
        }
    }
}
