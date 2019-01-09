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
using System.Xml;

namespace EtiquetasBlumar
{
    /// <summary>
    /// Lógica de interacción para centros.xaml
    /// </summary>
    public partial class centros : Window
    {
        //XmlDOCUMENT
        XmlDocument doc = new XmlDocument();
        //XmlDocument
        public centros()
        {
            InitializeComponent();
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;

            //XmlDOCUMENT Y CENTROS
            doc.Load(System.IO.Directory.GetCurrentDirectory() + @"\centrosBlumar.xml");
            XmlNodeList nodoCentros = doc.GetElementsByTagName("centros");
            foreach (XmlNode node in nodoCentros[0].ChildNodes)
            {
                ComboBoxItem cbxitem = new ComboBoxItem();

                cbxitem.Tag = node.Attributes["id"].Value;
                cbxitem.Content = node.InnerText;

                comboAdminCentro.Items.Add(cbxitem);
            }

            XmlNodeList nodoValor = doc.GetElementsByTagName("seleccion");
            foreach (XmlNode node in nodoValor[0].FirstChild)
            {
                int valorCentro = Convert.ToInt32(node.InnerText);
                comboAdminCentro.SelectedIndex = valorCentro-1;
            }

            //XmlDocument Y CENTROS
        }

        private void SaveCentro_Click(object sender, RoutedEventArgs e)
        {
            String valorcito = comboAdminCentro.SelectedValue.ToString();

            doc.Load(System.IO.Directory.GetCurrentDirectory() + @"\centrosBlumar.xml");
            //XmlNode valorInicio = doc.FirstChild;

            foreach (XmlElement element in doc.SelectNodes("//seleccion"))
            {
                foreach (XmlElement element1 in element)
                {
                    XmlNode newValue = doc.CreateElement("idCentro");
                    newValue.InnerText = valorcito;
                    element.ReplaceChild(newValue, element1);

                    doc.Save(System.IO.Directory.GetCurrentDirectory() + @"\centrosBlumar.xml");
                }
            }
            MessageBox.Show("Centro guardado");
            //        if (element.SelectSingleNode("//Title").InnerText == "Alarm1")
        }
    }
}
