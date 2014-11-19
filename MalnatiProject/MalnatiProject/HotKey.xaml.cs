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

namespace MalnatiProject
{
    /// <summary>
    /// Interaction logic for HotKey.xaml
    /// </summary>
    public partial class HotKey : Window
    {

      
        MainWindow rif;
       
        public HotKey(MainWindow m)
        {
            InitializeComponent();
             rif = m;
             t1.Text =rif.cambiaFocus;
             t2.Text = rif.ritornaClient;
             t3.Text = rif.copiaClipboard;
             t4.Text = rif.copiaClipboardServer;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            if (t1.Text.Length == 1 && t1.Text != "C" && t1.Text != "V" && t1.Text != "Z" && t1.Text != "X" && ((t1.Text[0] >= 0x30 && t1.Text[0] <= 0x39) || (t1.Text[0] >= 0x61 && t1.Text[0] <= 0x7A) || (t1.Text[0] >= 0x41 && t1.Text[0] <= 0x5A)))
            {
                rif.cambiaFocus = t1.Text.ToUpperInvariant();

            }
            else
            {
                MessageBox.Show("Carattere non valido per CambiaFocus!");
                return;
            }

            if (t2.Text.Length == 1 && t2.Text != "C" && t2.Text != "V" && t2.Text != "Z" && t2.Text != "X" && ((t2.Text[0] >= 0x30 && t2.Text[0] <= 0x39) || (t2.Text[0] >=0x61 && t2.Text[0] <= 0x7A) || (t2.Text[0] >= 0x41 && t2.Text[0] <= 0x5A)))
            {
                rif.ritornaClient = t2.Text.ToUpperInvariant();
            }
            else
            {
                MessageBox.Show("Carattere non valido per TornaAlClient!");
                return;
            }

            if (t3.Text.Length == 1 && t3.Text != "C" && t3.Text != "V" && t3.Text != "Z" && t3.Text != "X" && ((t3.Text[0] >= 0x30 && t3.Text[0] <= 0x39) || (t3.Text[0] >= 0x61 && t3.Text[0] <= 0x7A) || (t3.Text[0] >= 0x41 && t3.Text[0] <= 0x5A)))
            {
                rif.copiaClipboard = t3.Text.ToUpperInvariant();
            }
            else
            {
                MessageBox.Show("Carattere non valido per CopiaClipboard!");
                return;
            }

            if (t4.Text.Length == 1 && t4.Text != "C" && t4.Text != "V" && t4.Text != "Z" && t4.Text != "X" && ((t4.Text[0] >= 0x30 && t4.Text[0] <= 0x39) || (t4.Text[0] >= 0x61 && t4.Text[0] <= 0x7A) || (t4.Text[0] >= 0x41 && t4.Text[0] <= 0x5A)))
            {
                rif.copiaClipboardServer = t4.Text.ToUpperInvariant();
            }
            else
            {
                MessageBox.Show("Carattere non valido per CopiaClipboardServer!");
                return;
            }

            MessageBox.Show("Hotkeys settate");

        }

      
    }
}
