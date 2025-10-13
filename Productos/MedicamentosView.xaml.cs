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

namespace Sistema_de_Gestión_Farmacéutica.Productos
{
    /// <summary>
    /// Lógica de interacción para MedicamentosView.xaml
    /// </summary>
    public partial class MedicamentosView : UserControl
    {
        private MedicamentoRepositorio medicamentoRepo = new MedicamentoRepositorio();
        private int opcion = 0;
        public MedicamentosView()
        {
            InitializeComponent();
            CargarMedicamentos();
        }
        
        private void CargarMedicamentos()
        {
            if (opcion == 0)
            {
                dgMedicamentos.ItemsSource = medicamentoRepo.ObtenerMedicamentosActivos().DefaultView;
            } else
            {
                dgMedicamentos.ItemsSource = medicamentoRepo.ObtenerMedicamentosInactivos().DefaultView;
            }
     
        }

        private void btnAgregar_Click(object sender, RoutedEventArgs e)
        {
            AgregarMedicamentoWindow agregarMedicamento = new AgregarMedicamentoWindow();
            bool? resultado = agregarMedicamento.ShowDialog();

            if (resultado == true)
            {
                medicamentoRepo.AltaMedicamento(agregarMedicamento.medicamentoCreado);
                CargarMedicamentos();
            }

        }

        private void btnCambiarGrid_Click(object sender, RoutedEventArgs e)
        {
            // por cada click cambia el título y el texto de los botones, y vuelve a cargar el grid con el valor activado correspondiente
            if (opcion == 0)
            {
                opcion = 1;
                Titulo.Text = "Medicamentos Inactivos";
                btnCambiarGrid.Content = "Mostrar Activos";
                btnActivarOEliminar.Content = "🗑 Activar Medicamento";
                dgMedicamentos.RowBackground = Brushes.LightGray;
                CargarMedicamentos();
            } else
            {
                opcion = 0;
                Titulo.Text = "Medicamentos Activos";
                btnCambiarGrid.Content = "Mostrar Eliminados";
                btnActivarOEliminar.Content = "🗑 Eliminar Medicamento";
                dgMedicamentos.RowBackground = Brushes.White;
                CargarMedicamentos();
            }
        }
        
       private void btnActivarOEliminar_Click(object sender, RoutedEventArgs e)
       {
           if (dgMedicamentos.SelectedItem == null)
           {
               MessageBox.Show("Debe seleccionar un medicamento.", "Atención", MessageBoxButton.OK, MessageBoxImage.Warning);
               return;
           }

           DataRowView filaSeleccionada = (DataRowView)dgMedicamentos.SelectedItem;
           int idMedicamento = Convert.ToInt32(filaSeleccionada["id_medicamento"]);

            if (opcion == 0)
            {
                var resultado = MessageBox.Show($"¿Está seguro que desea eliminar este medicamento?", "Confirmar eliminación", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (resultado == MessageBoxResult.Yes)
                {
                    medicamentoRepo.BajaMedicamento(idMedicamento);
                    CargarMedicamentos();
                    MessageBox.Show("Medicamento eliminado correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            } else
            {
                var resultado = MessageBox.Show($"¿Está seguro que desea activar este medicamento?", "Confirmar activación", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (resultado == MessageBoxResult.Yes)
                {
                    medicamentoRepo.ActivarMedicamento(idMedicamento);
                    CargarMedicamentos();
                    MessageBox.Show("Medicamento activado correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }
        private void btnEditar_Click(object sender, RoutedEventArgs e)
        {
            if (dgMedicamentos.SelectedItem == null)
            {
                MessageBox.Show("Seleccione un medicamento para editar.", "Atención", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DataRowView filaSeleccionada = (DataRowView)dgMedicamentos.SelectedItem;

            Medicamento medicamentoAEditar = new Medicamento
            {
                id_medicamento = Convert.ToInt32(filaSeleccionada["id_medicamento"]),
                nombre_comercial = filaSeleccionada["nombre_comercial"].ToString(),
                precio_unitario = Convert.ToSingle(filaSeleccionada["precio_unitario"]),
                presentacion = filaSeleccionada["presentacion"].ToString(),
                laboratorio = filaSeleccionada["laboratorio"].ToString(),
                stock = Convert.ToInt32(filaSeleccionada["stock"]),
                stock_minimo = Convert.ToInt32(filaSeleccionada["stock_minimo"])
            };

            EditarMedicamento ventana = new EditarMedicamento(medicamentoAEditar);
            bool? resultado = ventana.ShowDialog();

            if (resultado == true)
            {
                medicamentoRepo.EditarMedicamento(medicamentoAEditar);
                CargarMedicamentos();
                MessageBox.Show("Medicamento editado correctamente", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
            }

        }
    } 
}
