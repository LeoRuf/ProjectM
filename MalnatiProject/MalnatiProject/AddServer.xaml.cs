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
using System.Text.RegularExpressions;

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
            bool isValidIp = true;

            string pattern = @"^([1-9]|[1-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|25[0-5])(\.([0-9]|[1-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|25[0-5])){3}$";
            Regex check = new Regex(pattern);
            if (myIpString == null)
            {
                isValidIp = false;
            }
            else
            {
                isValidIp = check.IsMatch(myIpString, 0);
            }

            //isValidIp = IPAddress.TryParse(myIpString, out ipAddress);

            Int16 num;
            if (isValidIp == false)
            {
                MessageBox.Show("IP non valido o mancante");
            } else if (this.porta_text_box.Text.Trim().Length == 0 || !Int16.TryParse(this.porta_text_box.Text.Trim(), out num))
            {

                MessageBox.Show("Porta non valida o mancante");
                

            } else if (this.password_text_box.Password.Trim().Length == 0)
            {

                MessageBox.Show("Password mancante");


            } else {
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
