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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.ComponentModel;
namespace MalnatiProject
{
    /// <summary>
    /// Interaction logic for ElencoServers.xaml
    /// </summary>
    public partial class ElencoServers : Window
    {
        public ElencoServers()
        {
            InitializeComponent();
        }

        bool connesso = false;
        ArrayList copy = new ArrayList();

        public void PassArgument(ArrayList list)
        {
            copy = list;

            GridView g = new GridView();
            foreach (ServerWindow se in list)
            {
                Button b;

                b = new Button();

                b.Width = 90;
                b.Height = 20;
                b.Content = "Disconnetti";
                b.Background = Brushes.Orange;

                list_box.Items.Add(se.ToString());
                //list_box.Items.Add();



            }

        }





        private void connetti_button_Click(object sender, RoutedEventArgs e)
        {
            if (list_box.SelectedItem == null)
            {
                MessageBox.Show("Seleziona un server");
            }
            else
            {
                String stringa = list_box.SelectedItem.ToString();
                string[] strArr = stringa.Split(' ');
                //MessageBox.Show(strArr[0] + " " + strArr[4]);

                foreach (ServerWindow ser in copy)
                {
                    if (ser.ip == strArr[0] && ser.porta == Convert.ToInt16(strArr[4]))
                    {

                        ser.Connetti();
                    }

                }




            }//chiudo else

        }

        private void cancella_button_Click(object sender, RoutedEventArgs e)
        {
            if (list_box.SelectedItem == null)
            {
                MessageBox.Show("Seleziona un server");
            }
            else
            {
                copy.RemoveAt(list_box.SelectedIndex);
                list_box.Items.Clear();
                foreach (ServerWindow se in copy)
                {
                    list_box.Items.Add(se.ToString());
                }

            }

        }



    }
}
