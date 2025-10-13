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
using System.Windows.Shapes;

namespace Sistema_de_Gestión_Farmacéutica.Clientes
{
    /// <summary>
    /// Lógica de interacción para GestionarObrasSociales.xaml
    /// </summary>
    public partial class GestionarObrasSociales : Window
    {
        private ClienteRepositorio clienteRepo = new ClienteRepositorio();
        private int _idCliente;
        private DataTable dtAsociados, dtTodas;

        public GestionarObrasSociales(int idCliente, string nombreCliente)
        {
            //Inicializacion
            InitializeComponent();
            _idCliente = idCliente;
            lblNombreCliente.Text = $"Cliente: {nombreCliente}";
            CargarDatosIniciales();
        }

        private void CargarDatosIniciales()
        {
            //pedimos los datos a la DB
            dtAsociados = clienteRepo.ObtenerObrasSocialesDelCliente(_idCliente);
            dtTodas = clienteRepo.ObtenerObrasSociales();

            lvAsociadas.ItemsSource = dtAsociados.DefaultView;
            ActualizarListaDisponible();
        }

        private void ActualizarListaDisponible()
        {
            //Filta aquellas obras que estan asociadas
            var idAsociadas = dtAsociados.AsEnumerable().Select(r => r.Field<int>("id_obra_social"));
            //Filta aquellas obras que no estan asociadas
            var disponibles = dtTodas.AsEnumerable().Where(r => !idAsociadas.Contains(r.Field<int>("id_obra_social")));

            if (disponibles.Any())
            {
                lvDisponibles.ItemsSource = disponibles.CopyToDataTable().DefaultView;
            }
            else
            {
                lvDisponibles.ItemsSource = null;
            }

            lvDisponibles.DisplayMemberPath = "nombre";
            lvDisponibles.SelectedValuePath = "id_obra_social";
        }



        private void btnAgregar_Click(object sender, RoutedEventArgs e)
        {
            if(lvDisponibles.SelectedItem == null)
            {
                MessageBox.Show("Debe seleccionar una obra social para agregarla", "Selección Requerida", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            DataRowView fila = (DataRowView)lvDisponibles.SelectedItem;
            int idOS = (int)fila["id_obra_social"];
            string nombreOS = fila["nombre"].ToString();

            //Se llama la view para pedir el número de afiliado
            InputBoxAfiliado inputBox = new InputBoxAfiliado($"Ingrese el N° de afiliado para {nombreOS}:", "N° de Afiliado");

            if(inputBox.ShowDialog() == true)
            {
                string nroAfiliado = inputBox.respuesta;

                //Creamos nuevas listas
                DataRow nuevaFila = dtAsociados.NewRow();
                nuevaFila["id_obra_social"] = idOS;
                nuevaFila["nombre"] = nombreOS;
                nuevaFila["nro_afiliado"] = nroAfiliado;
                dtAsociados.Rows.Add(nuevaFila);
                //Recargamos la lista de obras sociales
                ActualizarListaDisponible();
            }

        }

        private void btnQuitar_Click(object sender, RoutedEventArgs e)
        {
            if (lvAsociadas.SelectedItem == null)
            {
                MessageBox.Show("Debe seleccionar una obra social para eliminarla", "Selección Requerida", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
           
            //Se convierte la lista a Data para manipularla
            DataRowView filaRemovida = (DataRowView)lvAsociadas.SelectedItem;
            //Se elimina la fila seleccionada
            dtAsociados.Rows.Remove(filaRemovida.Row);

            //Refrescamos las obras sociales asociadas
            ActualizarListaDisponible();

        }

        private void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                clienteRepo.GuardarAsociacionesCliente(_idCliente, dtAsociados);
                MessageBox.Show("Los cambios se han guardado con éxito", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                this.DialogResult = true;
                this.Close();

            }catch (Exception ex)
            {
                MessageBox.Show($"Ocurrió un error al guardar: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
