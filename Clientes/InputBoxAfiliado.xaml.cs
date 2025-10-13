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

namespace Sistema_de_Gestión_Farmacéutica.Clientes
{
    /// <summary>
    /// Lógica de interacción para ImputBoxAfiliado.xaml
    /// </summary>
    public partial class InputBoxAfiliado : Window
    {
        public string respuesta { get; private set; }
        public InputBoxAfiliado(string prompt, string titulo)
        {
            InitializeComponent();
            this.Title = titulo;
            lblPrompt.Text = prompt;
        }

        private void Aceptar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtInput.Text))
            {
                MessageBox.Show("Este campo no puede estar vacío", "Dato Requerido", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            respuesta = txtInput.Text;
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
