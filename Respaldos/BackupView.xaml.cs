using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Printing;
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

namespace Sistema_de_Gestión_Farmacéutica.Respaldos
{
    /// <summary>
    /// Lógica de interacción para BackupView.xaml
    /// </summary>
    public partial class BackupView : UserControl
    {
        private string connectionString = "Server=localhost\\SQLEXPRESS; Database=SistemaFarmaceutico; Trusted_Connection=True; TrustServerCertificate=True;";

        public BackupView()
        {
            InitializeComponent();
        }

        private void btnRuta_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFolderDialog()
            {
                Title = "Seleccione la carpeta",
                RootDirectory = Environment.SpecialFolder.MyComputer.ToString()
            };

            if (dialog.ShowDialog() == true)
            {
                string rutaSeleccionada = dialog.FolderName;
                txtRutaBackup.Text = rutaSeleccionada;
            }
            
        }

        private void btnCrearBackup_Click(object sender, RoutedEventArgs e)
        {
            string rutaBackup = txtRutaBackup.Text;
            const string NOMBRE_DB_A_RESPALDAR = "SistemaFarmaceutico";

            if (string.IsNullOrEmpty(rutaBackup))
            {
                MessageBox.Show("Seleccione una ruta de destino para el Backup", "Error de ruta", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!Directory.Exists(rutaBackup))
            {
                MessageBox.Show("La carpeta de destino no existe. Por favor, verifique la ruta", "Error de ruta", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // crea la base de datos con el nombre asignado incluyendo hora de creación
                Backup servicio = new Backup(connectionString, rutaBackup);
                servicio.BackupDatabase(NOMBRE_DB_A_RESPALDAR);

                MessageBox.Show($"Copia de seguridad de '{NOMBRE_DB_A_RESPALDAR}' completada con éxito.",
                                 "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);

                txtRutaBackup.Text = string.Empty;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al crear la copia de seguridad. Verifique permisos y ruta:\n{ex.Message}", "Error de Backup", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
