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

        private void btnEstadisticas_Click(object sender, RoutedEventArgs e)
        {
            DateTime? desde = dpDesde.SelectedDate;
            DateTime? hasta = dpHasta.SelectedDate;

            string condicionFecha = "";
            bool filtrarPorFecha = desde.HasValue && hasta.HasValue;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                //  Vendedor con más ventas 
                string queryVendedor = @"
            SELECT TOP 1 u.nombre + ' ' + u.apellido AS nombre_vendedor, COUNT(v.id_venta) AS cantidad_ventas
            FROM Venta v
            INNER JOIN Usuario u ON v.id_usuario = u.id_usuario
            /**CONDICION_FECHA**/
            GROUP BY u.nombre, u.apellido
            ORDER BY cantidad_ventas DESC";

                // Producto más vendido
                string queryProducto = @"
            SELECT TOP 1 m.nombre_comercial AS nombre_producto, SUM(dv.cantidad) AS total_vendido
            FROM Detalle_Venta dv
            INNER JOIN Medicamento m ON dv.id_medicamento = m.id_medicamento
            INNER JOIN Venta v ON dv.id_venta = v.id_venta
            /**CONDICION_FECHA**/
            GROUP BY m.nombre_comercial
            ORDER BY total_vendido DESC";

                if (filtrarPorFecha)
                {
                    condicionFecha = "WHERE v.fecha_venta BETWEEN @desde AND @hasta";
                    queryVendedor = queryVendedor.Replace("/**CONDICION_FECHA**/", condicionFecha);
                    queryProducto = queryProducto.Replace("/**CONDICION_FECHA**/", condicionFecha);
                }
                else
                {
                    queryVendedor = queryVendedor.Replace("/**CONDICION_FECHA**/", "");
                    queryProducto = queryProducto.Replace("/**CONDICION_FECHA**/", "");
                }

                SqlCommand cmdVendedor = new SqlCommand(queryVendedor, con);
                SqlCommand cmdProducto = new SqlCommand(queryProducto, con);

                if (filtrarPorFecha)
                {
                    cmdVendedor.Parameters.AddWithValue("@desde", desde.Value);
                    cmdVendedor.Parameters.AddWithValue("@hasta", hasta.Value);
                    cmdProducto.Parameters.AddWithValue("@desde", desde.Value);
                    cmdProducto.Parameters.AddWithValue("@hasta", hasta.Value);
                }

                string mejorVendedor = "Sin datos";
                string productoMasVendido = "Sin datos";

                using (SqlDataReader dr = cmdVendedor.ExecuteReader())
                {
                    if (dr.Read())
                        mejorVendedor = $"{dr["nombre_vendedor"]} ({dr["cantidad_ventas"]} ventas)";
                }

                using (SqlDataReader dr = cmdProducto.ExecuteReader())
                {
                    if (dr.Read())
                        productoMasVendido = $"{dr["nombre_producto"]} ({dr["total_vendido"]} unidades)";
                }

                // Mostrar resultados
                MessageBox.Show(
                    $"Estadísticas{(filtrarPorFecha ? $" entre {desde:dd/MM/yyyy} y {hasta:dd/MM/yyyy}" : "")}\n\n" +
                    $"Vendedor con más ventas: {mejorVendedor}\n" +
                    $"Producto más vendido: {productoMasVendido}",
                    "Estadísticas de Ventas",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );
            }
        }
    }
}
