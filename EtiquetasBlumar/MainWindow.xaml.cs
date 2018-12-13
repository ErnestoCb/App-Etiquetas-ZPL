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
using System.Windows.Navigation;
using System.Windows.Shapes;
using iTextSharp;
using iTextSharp.text.pdf;

namespace EtiquetasBlumar
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Barcode128 code128 = new Barcode128();
        Random random = new Random();
        int randomNumber = -1;
        string nf;


        private MainWindow instance;
        public MainWindow()
        {
            InitializeComponent();
            instance = this;
        }
    }
}
