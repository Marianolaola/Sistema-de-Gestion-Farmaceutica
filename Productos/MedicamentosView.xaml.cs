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

namespace Sistema_de_Gestión_Farmacéutica
{
    /// <summary>
    /// Lógica de interacción para MedicamentosView.xaml
    /// </summary>
    public partial class MedicamentosView : UserControl
    {
        private Medicamento servMedicamento = new Medicamento();
        public MedicamentosView()
        {
            InitializeComponent();
            cargarMedicamento();
        }

        private void cargarMedicamento()
        {
            dgMedicamentos.ItemsSource = servMedicamento.ObtenerMedicamentos().DefaultView;
        }

        private void btnAgregar_Click(object sender, RoutedEventArgs e)
        {
            AgregarMedicamentoWindow agregarMedicamento = new AgregarMedicamentoWindow();
            bool? resultado = agregarMedicamento.ShowDialog();

            if (resultado == true)
            {
                servMedicamento.AltaMedicamento(agregarMedicamento.Nombre,
                                            agregarMedicamento.PrecioUnitario,
                                            agregarMedicamento.Presentacion,
                                            agregarMedicamento.Laboratorio,
                                            agregarMedicamento.Stock,
                                            agregarMedicamento.StockMinimo);
                cargarMedicamento();
            }

        }

        private void btnEliminar_Click(object sender, RoutedEventArgs e)
        {
            if (dgMedicamentos.SelectedItem == null)
            {
                MessageBox.Show("Debe seleccionar un medicamento para eliminar.", "Atención", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DataRowView filaSeleccionada = (DataRowView)dgMedicamentos.SelectedItem;
            int idMedicamento = Convert.ToInt32(filaSeleccionada["id_medicamento"]);

            // Confirmación antes de eliminar
            var resultado = MessageBox.Show($"¿Está seguro que desea eliminar este medicamento?",
                                                    "Confirmar eliminación", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (resultado == MessageBoxResult.Yes)
            {
               servMedicamento.BajaMedicamento(idMedicamento);
               cargarMedicamento();
               MessageBox.Show("Medicamento eliminado correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
            }
                }
            }

        }
