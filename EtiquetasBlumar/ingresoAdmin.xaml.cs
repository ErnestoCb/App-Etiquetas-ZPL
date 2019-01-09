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

namespace EtiquetasBlumar
{
    /// <summary>
    /// Lógica de interacción para ingresoAdmin.xaml
    /// </summary>
    public partial class ingresoAdmin : Window
    {
        public ingresoAdmin()
        {
            InitializeComponent();

            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //best login de la vida

            if(txtUser.Text == "blumar" && txtPwd.Password.ToString() == "blumar")
            {
                centros ventanaCentros = new centros();
                ventanaCentros.Visibility = Visibility.Visible;
                this.Close();
            }
            else
            {
                MessageBox.Show("Contraseña incorrecta... Contactese con su administrador.");
            }
            
        }
    }
}
