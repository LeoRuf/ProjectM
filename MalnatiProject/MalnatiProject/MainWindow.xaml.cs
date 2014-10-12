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

        public MainWindow()
        {
            InitializeComponent();
            dispatcher = Dispatcher.CurrentDispatcher;
            serverList.Add(new ServerWindow("192.168.1.140", 1601, "volley"));
            //serverList.Add(new ServerWindow("192.168.1.22", 6001, "ciao1"));
            lServers.ItemsSource=serverList;

        }



       // public ArrayList list_servers = new ArrayList(); //arrrayList comment

        private void aggiungi_button_Click(object sender, RoutedEventArgs e)
        {

            AddServer add = new AddServer();
            add.Show();
            add.rif = this;

        }




        private void Window_ContentRendered(object sender, EventArgs e)
        {/*
            BackgroundWorker worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.DoWork += worker_DoWork;
            worker.ProgressChanged += worker_ProgressChanged;

            worker.RunWorkerAsync();*/
        }

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {/*
            for (int i = 0; i < 100; i++)
            {
                (sender as BackgroundWorker).ReportProgress(i);
                Thread.Sleep(100);
            }*/
        }

        void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {/*
            pbStatus.Value = e.ProgressPercentage;
           */
        }

        private void ConnettiButton_Click(object sender, RoutedEventArgs e)
        {
            if (lServers.SelectedItem == null)
            {
                MessageBox.Show("Seleziona un server");
            }
            else
            {
                ConnettiButton.IsEnabled = false;
                loading_label.Content = "Connessione in corso...";
                (lServers.SelectedItem as ServerWindow).rif = this;
                Thread workerThread = new Thread((lServers.SelectedItem as ServerWindow).Connetti);
                workerThread.Start();

                
            }  
        }

        public void Change_Focus(ServerWindow window)
        {
            Action action = () =>
            {
                ConnettiButton.IsEnabled = true;
                loading_label.Content = "";
                window.Show();
            };

            dispatcher.BeginInvoke(action);
        }

        public void Enable_ConnettiButton()
        {
            Action action = () =>
            {
                ConnettiButton.IsEnabled = true;
                loading_label.Content = "";
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
                serverList.Remove(lServers.SelectedItem as ServerWindow);
            }
        }

    



    }
}