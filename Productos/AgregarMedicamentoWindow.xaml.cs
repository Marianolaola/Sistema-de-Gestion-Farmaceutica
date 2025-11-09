using Sistema_de_Gestión_Farmacéutica.Productos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
        public Medicamento medicamentoCreado { get; private set; }

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

            // probablemente debería cambiar esto
            char letra = txtNombre.Text[0];
            for (int i=0; i < txtNombre.Text.Length; i++)
            {
                if (!(char.IsLetter(letra)))
                {
                    MessageBox.Show("El campo de nombre solo admite letras", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                } else
                {
                    letra = txtNombre.Text[i];
                }
            }

            if (!Regex.IsMatch(txtStock.Text, @"^\d+$")) 
            {
                MessageBox.Show("El campo de stock solo admite enteros", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!Regex.IsMatch(txtStockMinimo.Text, @"^\d+$"))
            {
                MessageBox.Show("El campo de stock minimo solo admite enteros", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            medicamentoCreado = new Medicamento
            {
                nombre_comercial = txtNombre.Text,
                precio_unitario = float.Parse(txtPrecioUnitario.Text),
                presentacion = txtPresentacion.Text,
                laboratorio = txtLaboratorio.Text,
                stock = int.Parse(txtStock.Text),
                stock_minimo = int.Parse(txtStockMinimo.Text)
            };

            this.DialogResult = true;
            this.Close();
        }

        private void Cancelar_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
        private void SoloLetras_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!e.Text.All(c => char.IsLetter(c) || c == ' '))
            {
                MessageBox.Show("Este campo solo admite letras", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                e.Handled = true;
            }
        }

        private void SoloNumeros_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!e.Text.All(c => char.IsDigit(c)))
            {
                MessageBox.Show("Este campo solo admite numeros", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                e.Handled = true;
            }
        }

        private void Decimales_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!e.Text.All(c => char.IsDigit(c) || c == ',' || c == '.'))
            {
                MessageBox.Show("Este campo solo admite numeros", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                e.Handled = true;
            }
        }
    }
}
