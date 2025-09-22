using System;
using System.Collections.Generic;
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
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Sistema_de_Gestión_Farmacéutica
{
    /// <summary>
    /// Lógica de interacción para AgregarMedicamentoWindow.xaml
    /// </summary>
    public partial class AgregarMedicamentoWindow : Window
    {
        public string Nombre { get; private set; }
        public float PrecioUnitario { get; private set; }
        public string Presentacion { get; private set; }
        public string Laboratorio { get; private set; }
        public int Stock { get; private set; }
        public int StockMinimo { get; private set; }

        public AgregarMedicamentoWindow()
        {
            InitializeComponent();
        }

        private void Aceptar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text) ||
                string.IsNullOrWhiteSpace(txtPrecioUnitario.Text) ||
                string.IsNullOrWhiteSpace(txtPresentacion.Text) ||
                string.IsNullOrWhiteSpace(txtLaboratorio.Text) ||
                string.IsNullOrWhiteSpace(txtStock.Text) ||
                string.IsNullOrWhiteSpace(txtStockMinimo.Text)) 
            {
                MessageBox.Show("Todos los campos son obligatorios.");
                return;
            }

            Nombre = txtNombre.Text;
            PrecioUnitario = float.Parse(txtPrecioUnitario.Text);
            Presentacion = txtPresentacion.Text;
            Laboratorio = txtLaboratorio.Text;
            Stock = int.Parse(txtStock.Text);
            StockMinimo = int.Parse(txtStockMinimo.Text);
            //Rol = (cmbRol.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? string.Empty;

            this.DialogResult = true;
            this.Close();
        }

        private void Cancelar_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
        private void SoloLetras_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            if (!e.Text.All(c => char.IsLetter(c) || c == ' '))
            {
                MessageBox.Show("Solo se permiten letras", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                e.Handled = true;
            }
        }
    }
}
