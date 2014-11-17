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
using System.Windows.Media.Animation;
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
using System.Collections.Specialized;



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
        bool isFTPConnesso = false;
        


        public MainWindow()
        {
            InitializeComponent();
            dispatcher = Dispatcher.CurrentDispatcher;
            serverList.Add(new ServerWindow("192.168.1.132", 1601, "c"));
            lServers.ItemsSource = serverList;
            Clipboard.Clear();
      }

        private void aggiungi_button_Click(object sender, RoutedEventArgs e)
        {
            AddServer add = new AddServer();
            add.Show();
            add.rif = this;
        }

        /*****************************/
        //metodi del dispatcher
        public void SetClip(StringCollection s)
        {
            Action action = () =>
            {
                //Console.WriteLine("Sto settando clipboard");
                Clipboard.SetFileDropList(s);
                MessageBox.Show("Clipboard copiata!");
                
            };

            dispatcher.BeginInvoke(action);
        }

        public void SetClipText(string text)
        {
            Action action = () =>
            {
                Clipboard.SetText(text);
                MessageBox.Show("Clipboard copiata!");
            };

            dispatcher.Invoke(action);
        }


        public void Inizio(ServerWindow s, bool set) {

            Action action = () =>
            {
                if(set==true){
                    s.label_receiving.Content = "Sending ...";
                }
                else{
                    s.label_receiving.Content = "Receiving ...";
                }
                s.e1.Fill = Brushes.Orange;
                s.e1.Opacity = 1.0;

                s.e2.Fill = Brushes.Black;
                s.e2.Opacity = 0.9;

                s.e3.Fill = Brushes.Black;
                s.e3.Opacity = 0.8;

                s.e4.Fill = Brushes.Black;
                s.e4.Opacity = 0.7;

                s.e5.Fill = Brushes.Black;
                s.e5.Opacity = 0.6;

                s.e6.Fill = Brushes.Black;
                s.e6.Opacity = 0.5;
                
                s.e7.Fill = Brushes.Black;
                s.e7.Opacity = 0.4;

                s.e8.Fill = Brushes.Black;
                s.e8.Opacity = 0.3;

                s.e9.Fill = Brushes.Black;
                s.e9.Opacity = 0.2;

                s.e10.Fill = Brushes.Black;
                s.e10.Opacity = 0.1;
           };


            dispatcher.Invoke(action);
        }


        public void Fine(ServerWindow s)
        {

            Action action = () =>
            {
                s.e1.Fill = Brushes.LightBlue;
                s.e2.Fill = Brushes.LightBlue;
                s.e3.Fill = Brushes.LightBlue;
                s.e4.Fill = Brushes.LightBlue;
                s.e5.Fill = Brushes.LightBlue;
                s.e6.Fill = Brushes.LightBlue;
                s.e7.Fill = Brushes.LightBlue;
                s.e8.Fill = Brushes.LightBlue;
                s.e9.Fill = Brushes.LightBlue;
                s.e10.Fill = Brushes.LightBlue;
                s.label_receiving.Content = "";

            };


            dispatcher.Invoke(action);
        }

        public void DisplayErrorMessage() {

            Action action = () => { MessageBox.Show("Errore copia clipboard"); };
            dispatcher.Invoke(action);
        }

        public void EmptyClipboard()
        {

            Action action = () => { MessageBox.Show("Clipboard vuota!"); };
            dispatcher.Invoke(action);
        }

        public bool ShowOptions()
        {
            object var=true;
            Action action = () => {
               MessageBoxResult res = MessageBox.Show("Vuoi interrompere trasferimento?", "Copia clipboard in corso!", MessageBoxButton.YesNo);
               if (res == MessageBoxResult.Yes)
               {
                   var = true;
               }
               else
               {
                   var = false;
               }
            };

            dispatcher.Invoke(action);
            return (bool)var;
        }
        public void Change_Focus(ServerWindow window)
        {
            Action action = () =>
            {
                controlla_button.IsEnabled = true;
                DisconnettiButton.IsEnabled = true;
                CancellaButton.IsEnabled = true;
                loading_label.Content = "";

                if (window.Connesso == true)
                {
                    this.DisconnettiButton.Visibility = Visibility.Visible;
                }

                int n = serverList.IndexOf(window);
                window.boss = true;
                Console.WriteLine(n);
                Button b = new Button();
                b.Background = Brushes.Yellow;
                b.Width = 20;
                b.Height = 20;
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
        /****************************************/

        private void CancellaButton_Click(object sender, RoutedEventArgs e)
        {
            cancella_premuto = true;
            if (lServers.SelectedItem == null)
            {
                MessageBox.Show("Seleziona un server");
            }
            else
            {
                if (((ServerWindow)lServers.SelectedItem).Connesso == true)
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
            else
            {
                controlla_button.IsEnabled = false;
                CancellaButton.IsEnabled = false;
                DisconnettiButton.IsEnabled = false;
                loading_label.Content = "Controllo in corso...";
                (lServers.SelectedItem as ServerWindow).rif = this;
                //Fine(lServers.SelectedItem as ServerWindow);
                //eventuale connessione mouse+tastiera+client ftp
                Thread workerThread1 = new Thread((lServers.SelectedItem as ServerWindow).Controlla);
                workerThread1.Start();
            }
        }

        private void lServers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cancella_premuto == false)
            {
                if (((ServerWindow)lServers.SelectedItem).Connesso == false)
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
                if (((ServerWindow)lServers.SelectedItem).Connesso == true)
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

        public void restoreVisibility()
        {
            Action action = new Action(() =>
            {
                DisconnettiButton.Visibility = Visibility.Collapsed;
                loading_label.Content = "";
            });
            dispatcher.Invoke(action);
        }
    }
}