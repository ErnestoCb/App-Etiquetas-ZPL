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
using Gma.QrCodeNet.Encoding;
using Gma.QrCodeNet.Encoding.Windows.Render;
using System.IO;
using System.Drawing.Printing;
using System.Net;
using System.Runtime.Serialization.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Configuration;
using System.Xml;

namespace EtiquetasBlumar
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml 
    /// </summary>
    public partial class MainWindow : Window
    {
        //XmlDOCUMENT
        XmlDocument doc = new XmlDocument();
        //XmlDocument

        private MainWindow instance;
        public MainWindow()
        {
            InitializeComponent();
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            instance = this;

            fechaPicker.SelectedDate = DateTime.Today;
            txtCorrelativo.Text = cantEtiquetas.Value.ToString();

            //XmlDOCUMENT Y CENTROS
            doc.Load(System.IO.Directory.GetCurrentDirectory() + @"\centrosBlumar.xml");
            XmlNodeList nodoCentros = doc.GetElementsByTagName("centros");
            foreach(XmlNode node in nodoCentros[0].ChildNodes)
            {
                ComboBoxItem cbxitem = new ComboBoxItem();

                cbxitem.Tag = node.Attributes["id"].Value;
                cbxitem.Content = node.InnerText;

                comboCentro.Items.Add(cbxitem);
            }

            llenarCentro();

            //XmlDocument Y CENTROS

            //Conexion con SAP-------------------------------------
            WebClient client = new WebClient();
            string url = ConfigurationManager.AppSettings["JsonMateriales"];
            string json = client.DownloadString(url);
            
            dynamic asj = JsonConvert.DeserializeObject(json);

            JArray a = JArray.Parse(asj.LT_DETALLE.ToString());

            foreach (dynamic item in a)
            {
                ComboBoxItem cbxitem = new ComboBoxItem();

                cbxitem.Content = item.MAKTX;
                cbxitem.Tag = item.MATNR;

                cbxMaterial.Items.Add(cbxitem);
            }
            //Conexion con SAP-------------------------------------

        }

        public Boolean llenarCampos()
        {
            BlumarWS.rfcNetSoapClient cliente = new BlumarWS.rfcNetSoapClient();

            BlumarWS.request_ZMOV_10000 imp = new BlumarWS.request_ZMOV_10000();

            imp.CHARG = txtLote.Text;
            imp.MATNR = cbxMaterial.SelectedValue.ToString();

            BlumarWS.responce_ZMOV_10000 respuesta = new BlumarWS.responce_ZMOV_10000();

            respuesta = cliente.ZMOV_10000(imp);

            try
            {
                var envase = Array.FindIndex(respuesta.CHAR_OF_BATCH, row => row.ATNAM == "ZENVHU");
                cbxEnvase.Text = respuesta.CHAR_OF_BATCH[envase].ATWTB;
                var porcionsita = Array.FindIndex(respuesta.CHAR_OF_BATCH, row => row.ATNAM == "ZTVNMPPO");
                txtPorcion.Text = respuesta.CHAR_OF_BATCH[porcionsita].ATWTB;
                
                testeo.Content = respuesta.CHAR_OF_BATCH[envase].ATWTB;

                return true;
            }
            catch (Exception)
            {
                MessageBox.Show("No se encuentra el Lote");
                cbxMaterial.SelectedValue = 0;
                return false;
            }

            
            
        }

        public int crearCodQr()
        {
            
            if (!validaCampos("codigoQr"))
            {
                return 0;
            }

            int correlativo = Convert.ToInt16(cantEtiquetas.Value.ToString());

            String codigoQr = "{" +
                "\"lt\":\"" + txtLote.Text + "\"," +
                "\"mt\":\"" + cbxMaterial.SelectedValue + "\"," +
                "\"pr\":\"" + txtPorcion.Text + "\"," +
                "\"cr\":\"" + txtCorrelativo.Text + "\"," +
                "\"f\":\"" + fechaPicker.SelectedDate.Value.ToString("dd-MM-yyyy") + "\"," +
                "\"ev\":\"" + cbxEnvase.SelectedValue + "\"," +
                //"\"almacen\":\"" + cbxAlmacen.Text + "\"," +
                "\"jl\":\"" + lblJuliano.Content.ToString() + "\"," +
                "\"an\":\"" + lblAño.Content.ToString() + "\"," +
                "\"rz\": 0," +
                "\"ct\":\"" + cantToneladas.Value.ToString() + "\"" +
                "}";



            QrEncoder encoder = new QrEncoder(ErrorCorrectionLevel.M);
            QrCode qrCode;
            encoder.TryEncode(codigoQr, out qrCode);
            WriteableBitmapRenderer wRenderer = new WriteableBitmapRenderer(new FixedModuleSize(2, QuietZoneModules.Two), Colors.Black, Colors.White);
            WriteableBitmap wBitmap = new WriteableBitmap(104, 104, 170, 170, PixelFormats.Gray8, null);
            wRenderer.Draw(wBitmap, qrCode.Matrix);

            imgQrCode.Source = wBitmap;
            
            return 0;
        }

        private bool validaCampos(String cod)
        {
            if (cod == "codigoQr")
            {
                if(
                    cbxMaterial.SelectedIndex+1 != 0 &&
                    txtLote.Text != "" &&
                    txtPorcion.Text != "" &&
                    cbxEnvase.SelectedIndex+1 != 0 
                    //cbxAlmacen.SelectedIndex+1 != 0
                    )
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            else if (cod == "imprimir")
            {
                if(
                    cbxMaterial.SelectedIndex + 1 != 0 &&
                    txtLote.Text != "" &&
                    txtPorcion.Text != "" &&
                    cbxEnvase.SelectedIndex + 1 != 0 
                    //cbxAlmacen.SelectedIndex + 1 != 0
                    )
                {
                    return true;
                }
                else
                {
                    MessageBox.Show("Ingrese todo los datos solicitados para Imprimir");
                }
            }
            return false;
        }

        public string print(int correlativoEtiq)
        {
            //application startuppath ejemplo en wpf
            string etiqueta = System.IO.Directory.GetCurrentDirectory() + @"\zpl\etiqueta.zpl";
            string tempoEtiqueta = System.IO.Directory.GetCurrentDirectory() + @"\zpl\tempoEtiqueta.zpl";
            var lines = File.ReadAllLines(etiqueta);
            string salida = "";

            foreach(var line in lines)
            {
                salida += line;
            }
            crearCodQr();

            String qrcode = "{" +
                "\"lt\":\"" + txtLote.Text + "\"," +
                "\"mt\":\"" + cbxMaterial.SelectedValue + "\"," +
                "\"pr\":\"" + txtPorcion.Text + "\"," +
                "\"cr\":\"" + correlativoEtiq + "\"," +
                "\"f\":\"" + fechaPicker.SelectedDate.Value.ToString("dd-MM-yyyy") + "\"," +
                "\"ev\":\"" + cbxEnvase.SelectedValue + "\"," +
                //"\"almacen\":\"" + cbxAlmacen.Text + "\"," +
                "\"jl\":\"" + lblJuliano.Content.ToString() + "\"," +
                "\"an\":\"" + lblAño.Content.ToString() + "\"," +
                "\"rz\": 0," +
                "\"ct\":\"" + cantToneladas.Value.ToString() + "\"" +
                "}";

            //string qrcode = "" + txtLote.Text + "-" + txtPorcion.Text + "-" + correlativoEtiq.ToString() + "-" + lblJuliano.Content + "-" + lblAño.Content + "-" + (cbxMaterial.SelectedIndex + 1).ToString() + "-" + (cbxEnvase.SelectedIndex + 1).ToString() + "-" + (cbxAlmacen.SelectedIndex + 1).ToString() + "";

            salida = salida.Replace("[lote]", txtLote.Text);
            salida = salida.Replace("[correlativo]", correlativoEtiq.ToString().PadLeft(3, '0'));
            salida = salida.Replace("[porcion]", txtPorcion.Text.PadLeft(2, '0'));
            salida = salida.Replace("[juliana]", lblJuliano.Content.ToString());
            salida = salida.Replace("[anno]", lblAño.Content.ToString());
            salida = salida.Replace("[qrCode]", qrcode);

            try
            {
                if (File.Exists(tempoEtiqueta))
                {
                    File.Delete(tempoEtiqueta);
                }

                using(FileStream fs = File.Create(tempoEtiqueta))
                {
                    if(salida != null)
                    {
                        Byte[] info = new UTF8Encoding(true).GetBytes(salida);
                        fs.Write(info, 0, info.Length);
                        fs.Dispose();
                        fs.Close();
                    }
                    else
                    {
                        Console.WriteLine("Debe ingresar ZPL.");
                    }
                }

                PrintDialog dlgSettings = new PrintDialog();
                PrinterSettings ps = new PrinterSettings();

                RawPrinterHelper.SendFileToPrinter(ps.PrinterName, tempoEtiqueta);                

            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            return salida;
        }

        private void BtnImprimir_Click(object sender, RoutedEventArgs e)
        {
            txtPorcion.Text = txtPorcion.Text.PadLeft(2, '0');
            txtCorrelativo.Text = txtCorrelativo.Text.PadLeft(3, '0');
            if (!validaCampos("imprimir"))
            {
                return;
            }

            int cant = Convert.ToInt16(cantEtiquetas.Value.ToString());

            for(int i=1; i <= cant; i++)
            {
                print(i);
            }
        }

        private void FechaPicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            txtPorcion.Text = txtPorcion.Text.PadLeft(2, '0');
            lblAño.Content = fechaPicker.SelectedDate.Value.ToString("yy");
            lblJuliano.Content = Convert.ToDateTime(fechaPicker.SelectedDate.Value).DayOfYear.ToString().PadLeft(3, '0');
            txtCorrelativo.Text = txtCorrelativo.Text.PadLeft(3, '0');
            crearCodQr();
        }

        private void TxtLote_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtPorcion.Text = txtPorcion.Text.PadLeft(2, '0');
            lblLote.Content = txtLote.Text;
            txtCorrelativo.Text = txtCorrelativo.Text.PadLeft(3, '0');
            crearCodQr();
        }

        private void CbxEnvase_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            txtPorcion.Text = txtPorcion.Text.PadLeft(2, '0');
            txtCorrelativo.Text = txtCorrelativo.Text.PadLeft(3, '0');
            crearCodQr();
        }

        private void CbxMaterial_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            txtPorcion.Text = txtPorcion.Text.PadLeft(2, '0');
            txtCorrelativo.Text = txtCorrelativo.Text.PadLeft(3, '0');

            if(txtLote.Text != "" && cbxMaterial.SelectedIndex != 0)
            {
                llenarCampos();
            }

            
            crearCodQr();

        }

        private void TxtPorcion_TextChanged(object sender, TextChangedEventArgs e)
        {
            lblPorcion.Content = txtPorcion.Text;
            txtCorrelativo.Text = txtCorrelativo.Text.PadLeft(3, '0');
            crearCodQr();
        }

        private void CantEtiquetas_ValueChanged(object sender, RoutedPropertyChangedEventArgs<decimal> e)
        {
            txtPorcion.Text = txtPorcion.Text.PadLeft(2, '0');
            txtCorrelativo.Text = txtCorrelativo.Text.PadLeft(3, '0');
            lblCorrelativo.Content = cantEtiquetas.Value.ToString().PadLeft(3, '0');
            txtCorrelativo.Text = cantEtiquetas.Value.ToString().PadLeft(3, '0') ;
            crearCodQr();
        }

        private void TxtLote_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtLote.Text != "" && cbxMaterial.SelectedValue != null && cbxMaterial.SelectedIndex != 0)
            {
                llenarCampos();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            conexion.probarConn();
        }

        private void BtnAdmin_Click(object sender, RoutedEventArgs e)
        {
            ingresoAdmin ventanaAdmin = new ingresoAdmin();
            ventanaAdmin.Visibility = Visibility.Visible;
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            llenarCentro();
        }

        public void llenarCentro()
        {
            doc.Load(System.IO.Directory.GetCurrentDirectory() + @"\centrosBlumar.xml");

            XmlNodeList nodoValor = doc.GetElementsByTagName("seleccion");
            foreach (XmlNode node in nodoValor[0].FirstChild)
            {
                int valorCentro = Convert.ToInt32(node.InnerText);
                comboCentro.SelectedIndex = valorCentro - 1;
            }
        }
    }
}
