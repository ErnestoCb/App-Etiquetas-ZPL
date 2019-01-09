using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MySql.Data.MySqlClient;

namespace EtiquetasBlumar
{
    class conexion
    {
        public static MySqlConnection conn = new MySqlConnection("server = 10.60.1.220; database = Blumar; Uid = root; pwd = expled08; SslMode=none;");

        public static void probarConn()
        {
            try
            {
                conn.Open();
                MessageBox.Show("conexion");
                conn.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            finally
            {
                conn.Close();
            }
        }
    }
}
