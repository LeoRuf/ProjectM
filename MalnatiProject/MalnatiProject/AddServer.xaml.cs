using System;
using System.Collections;
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
using System.Net;

namespace MalnatiProject
{
    /// <summary>
    /// Interaction logic for AddServer.xaml
    /// </summary>
    /// 

    public partial class AddServer : Window
    {
        public AddServer()
        {
            InitializeComponent();
        }

        public MainWindow rif;

        private void aggiungi_button_Click(object sender, RoutedEventArgs e)
        {
            int flag = 0;

            string myIpString = this.ip_text_box.Text;
            IPAddress ipAddress = null;
            bool isValidPorta=true;
            bool isValidPassword = true;



            Int16 num;
            if (this.porta_text_box.Text.Trim().Length == 0 || !Int16.TryParse(this.porta_text_box.Text.Trim(), out num))
            {

                MessageBox.Show("Porta non valida o mancante");
                isValidPorta = false;


            }

            if (this.password_text_box.Password.Trim().Length == 0)
            {

                MessageBox.Show("Password mancante");
                isValidPassword = false;


            }


            bool isValidIp = IPAddress.TryParse(myIpString, out ipAddress);

            if (isValidIp == false)
            {
                MessageBox.Show("IP non valido o mancante");
            }
            
            if (isValidIp == true && isValidPassword==true && isValidPorta==true)
            {
                ServerWindow server = new ServerWindow(this.ip_text_box.Text, Convert.ToInt16(this.porta_text_box.Text), this.password_text_box.Password);
            
                foreach (ServerWindow se in rif.serverList)
                {
                    if (se.ip == this.ip_text_box.Text && se.porta == Convert.ToInt16(this.porta_text_box.Text)) { flag = 1; }
                }

                if (flag == 0)
                {
                    rif.serverList.Add(server);
                    //MessageBox.Show("Server aggiunto alla lista");
                    this.Close();
                }
                else
                {

                    MessageBox.Show("Server già presente in lista");
                }
            }
            

        }



    }
}
