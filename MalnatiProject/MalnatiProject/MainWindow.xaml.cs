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
using System.Windows.Threading;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.ComponentModel;
using System.Collections.ObjectModel;
using WindowsInput;



namespace MalnatiProject
{
    /// <summary>
    /// Logica di interazione per MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
        public ObservableCollection<ServerWindow> serverList = new ObservableCollection<ServerWindow>();
        private Dispatcher dispatcher;
        bool cancella_premuto = false;
        
        

        public MainWindow() 
        {
            InitializeComponent();
            dispatcher = Dispatcher.CurrentDispatcher;
            serverList.Add(new ServerWindow("192.168.1.135", 1601, "ciao"));
            lServers.ItemsSource=serverList;
            Clipboard.SetData(DataFormats.Text, "Ciao");
            

        }

        private void aggiungi_button_Click(object sender, RoutedEventArgs e)
        {

            AddServer add = new AddServer();
            add.Show();
            add.rif = this;

        }


       public void Change_Focus(ServerWindow window)
        {
            Action action = () =>
            {
                controlla_button.IsEnabled = true;
                DisconnettiButton.IsEnabled = true;
                CancellaButton.IsEnabled = true;
                loading_label.Content = "";

                if (window.isConnesso() == true)
                {
                    this.DisconnettiButton.Visibility = Visibility.Visible;
                }

                int n = serverList.IndexOf(window);
                window.boss = true;
                Console.WriteLine(n);
                Button b= new Button();
                b.Background= Brushes.Yellow;
                b.Width =20;
                b.Height =20;
                b.BorderBrush = Brushes.White;
                b.VerticalAlignment = VerticalAlignment.Top;
                b.Content = "M";
                b.BorderBrush = Brushes.Black;


                double x = 0;
                double y = 0;

                //for (int i =0; i < serverList.Count() && i!=n; i++)
                //{
                //    Button b1 = new Button();
                //    b1.Background = Brushes.White;
                //    b1.Width = 20;
                //    b1.Height = 20;
                //    b1.BorderBrush = Brushes.White;
                //    b1.VerticalAlignment = VerticalAlignment.Top;
                //    b1.Margin = new Thickness(x, y +(20*i), 0, 0); 
                
                //    master.Children.Add(b1);
                //}


                master.Children.Clear();
                master.Children.Add(b);
                b.Margin = new Thickness(x, y + (20 * n), 0, 0); 
                window.Show();

            };

            dispatcher.BeginInvoke(action);
        }

        

        public void Enable_Buttons()
        {
            Action action = () =>
            {
                controlla_button.IsEnabled = true;
                CancellaButton.IsEnabled = true;
                DisconnettiButton.IsEnabled = true;

                loading_label.Content = "";
            };

            dispatcher.BeginInvoke(action);
        }

        private void CancellaButton_Click(object sender, RoutedEventArgs e)
        {
            cancella_premuto = true;
            if (lServers.SelectedItem == null)
            {
                MessageBox.Show("Seleziona un server");
            }
            else
            {
                if (((ServerWindow)lServers.SelectedItem).isConnesso() == true)
                {
                    if (((ServerWindow)lServers.SelectedItem).boss == true)
                    {
                        //int n = serverList.IndexOf((ServerWindow)lServers.SelectedItem);
                        //Console.WriteLine(n);
                        //Console.WriteLine("Entrato");
                            master.Children.RemoveAt(0);
                    }
                    loading_label.Content = "Cancellazione server in corso...";
                    ((ServerWindow)lServers.SelectedItem).Disconnetti();
                }
                serverList.Remove(lServers.SelectedItem as ServerWindow);
            
            
                cancella_premuto = false;
            }
        }

      

        private void Controlla_Button_Click(object sender, RoutedEventArgs e)
        {
            if (lServers.SelectedItem == null)
                {
                    MessageBox.Show("Seleziona un server");
                }
           else{
                        controlla_button.IsEnabled = false;
                        CancellaButton.IsEnabled = false;
                        DisconnettiButton.IsEnabled = false;
                        loading_label.Content = "Controllo in corso...";
                        (lServers.SelectedItem as ServerWindow).rif = this;
                        Thread workerThread1 = new Thread((lServers.SelectedItem as ServerWindow).Controlla);
                        workerThread1.Start();
                    
                    
                }
           
        }

        private void lServers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cancella_premuto == false)
            {
                if (((ServerWindow)lServers.SelectedItem).isConnesso() == false)
                {
                    DisconnettiButton.Visibility = Visibility.Collapsed; 
                }
                else
                {
                    DisconnettiButton.Visibility = Visibility.Visible;

                }
            }
        }

        private void Disconnetti_Click(object sender, RoutedEventArgs e)
        {
            if (lServers.SelectedItem == null)
            {
                MessageBox.Show("Seleziona un server");
            }
            else
            {
                if (((ServerWindow)lServers.SelectedItem).isConnesso() == true)
                {

                    if (((ServerWindow)lServers.SelectedItem).boss == true)
                    {
                        //int n = serverList.IndexOf((ServerWindow)lServers.SelectedItem);
                        //Console.WriteLine(n);
                        //Console.WriteLine("Entrato");
                        master.Children.RemoveAt(0);
                    }
                    loading_label.Content = "Disconnessione in corso...";
                    ((ServerWindow)lServers.SelectedItem).Disconnetti();
                  
                }
                
            }
        }

    }
}