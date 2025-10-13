using Sistema_de_Gestión_Farmacéutica.Usuarios;
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

namespace Sistema_de_Gestión_Farmacéutica.Productos
{
    /// <summary>
    /// Lógica de interacción para EditarMedicamento.xaml
    /// </summary>
    public partial class EditarMedicamento : Window
    {
        public Medicamento medicamentoEditado { get; set; }
        public EditarMedicamento(Medicamento p_medicamento)
        {
            InitializeComponent();
            Inicializar(p_medicamento);
        }

        private void Inicializar(Medicamento p_medicamento)
        {
            txtNombre.Text = p_medicamento.nombre_comercial;
            txtPrecioUnitario.Text = p_medicamento.precio_unitario.ToString();
            txtPresentacion.Text = p_medicamento.presentacion;
            txtLaboratorio.Text = p_medicamento.laboratorio;
            txtStock.Text = p_medicamento.stock.ToString();
            txtStockMinimo.Text = p_medicamento.stock_minimo.ToString();
            medicamentoEditado = p_medicamento;
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

            char letra = txtNombre.Text[0];
            for (int i = 0; i < txtNombre.Text.Length; i++)
            {
                if (!(char.IsLetter(letra)))
                {
                    MessageBox.Show("El campo de nombre solo admite letras", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                else
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


            medicamentoEditado.nombre_comercial = txtNombre.Text;
            medicamentoEditado.precio_unitario = float.Parse(txtPrecioUnitario.Text);
            medicamentoEditado.presentacion = txtPresentacion.Text;
            medicamentoEditado.laboratorio = txtLaboratorio.Text;
            medicamentoEditado.stock = int.Parse(txtStock.Text);
            medicamentoEditado.stock_minimo = int.Parse(txtStockMinimo.Text);
            
            this.DialogResult = true;
            this.Close();
        }

        private void Cancelar_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
