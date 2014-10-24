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


namespace MalnatiProject
{
    /// <summary>
    /// Logica di interazione per MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
        public ObservableCollection<ServerWindow> serverList = new ObservableCollection<ServerWindow>();
        private Dispatcher dispatcher;

        public MainWindow() //maon window
        {
            InitializeComponent();
            dispatcher = Dispatcher.CurrentDispatcher;
            serverList.Add(new ServerWindow("192.168.1.136", 1601, "ciao"));
            lServers.ItemsSource=serverList;

        }



       // public ArrayList list_servers = new ArrayList(); //arrrayList comment

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
                loading_label.Content = "";

                if (window.isConnesso() == true)
                {
                    controlla_button.Content = "Disconnetti";
                    controlla_button.Background= Brushes.Yellow;
                }
                
                
                
                
                window.Show();

            };

            dispatcher.BeginInvoke(action);
        }

    

        public void Enable_ControllaButton()
        {
            Action action = () =>
            {
                controlla_button.IsEnabled = true;
                loading_label.Content = "";
                controlla_button.Content = "Disconetti";
            };

            dispatcher.BeginInvoke(action);
        }

        private void CancellaButton_Click(object sender, RoutedEventArgs e)
        {
            if (lServers.SelectedItem == null)
            {
                MessageBox.Show("Seleziona un server");
            }
            else
            {
                if (((ServerWindow)lServers.SelectedItem).isConnesso() == true)
                    loading_label.Content = "Cancellazione server in corso...";
                ((ServerWindow)lServers.SelectedItem).Disconnetti();
           
                serverList.Remove(lServers.SelectedItem as ServerWindow);
            }
        }

      

        private void Controlla_Button_Click(object sender, RoutedEventArgs e)
        {

           

                if (lServers.SelectedItem == null)
                {
                    MessageBox.Show("Seleziona un server");
                }
                else
                {
                    if (((ServerWindow)lServers.SelectedItem).isConnesso() == false)
                    {
                        controlla_button.IsEnabled = false;
                        loading_label.Content = "Controllo in corso...";
                        (lServers.SelectedItem as ServerWindow).rif = this;
                        Thread workerThread1 = new Thread((lServers.SelectedItem as ServerWindow).Controlla);
                        workerThread1.Start();
                    }
                    else
                    {

                        loading_label.Content = "Disconnessione in corso...";
                        ((ServerWindow)lServers.SelectedItem).Disconnetti();

                       
                    }
                }
           
        }

        private void lServers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((ServerWindow)lServers.SelectedItem).isConnesso() == false)
            {
                controlla_button.Content = "Controlla";
                controlla_button.Background = Brushes.LimeGreen;
            }
            else{
                controlla_button.Content = "Disconnetti";
                controlla_button.Background = Brushes.Yellow;
                ((ServerWindow)lServers.SelectedItem).Controlla();
            
            }
        }





     

    



    }
}