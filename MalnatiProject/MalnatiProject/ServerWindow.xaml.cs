﻿using System;
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
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.ComponentModel;
using System.Text.RegularExpressions;
//using Windows.Input;

namespace MalnatiProject
{
    public partial class ServerWindow : Window
    {
        public String ip; //del server
        public Int16 porta;  //del server
        public String password;
        public Socket socket;
        byte[] rec = new byte[64];

        public MainWindow rif;
        public static ManualResetEvent allDone = new ManualResetEvent(false);


        bool connesso = false;
        public bool boss = false;

        FtpClient ftpClient;

        /*****************************/
        //proprieta'
        public String Address
        {
            get { return "       " + ip + "                         " + Convert.ToString(porta); }
            set { }
        }

        public bool Connesso
        {
            get { return connesso; }
            set { connesso = value; }
        }
        /****************************/

        public ServerWindow(String ip, Int16 porta, String password)
        {
            InitializeComponent();

            this.ip = ip;
            this.porta = porta;
            this.password = password;
            ftpClient = new FtpClient(porta, IPAddress.Parse(ip));

        }

        public String ToString()
        {
            return this.ip + "    " + this.porta;

        }

        public void Connetti()
        {
            try
            {
                //connession client tastiera + mouse
                IPEndPoint lep = new IPEndPoint(IPAddress.Parse(ip), Convert.ToInt16(porta));
                Console.WriteLine("Sto per fare la connect");
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                socket.Connect(lep);

                Console.WriteLine("Socket connected to {0}",
                    socket.RemoteEndPoint.ToString());

                //controllo password
                socket.Send(Encoding.UTF8.GetBytes(password));
                socket.ReceiveTimeout = 50000;
                socket.Receive(rec);
                string pwdAns = Encoding.UTF8.GetString(rec);
                // if (true) { }
                if (Encoding.UTF8.GetString(rec).Trim('\0').Equals(password))
                {
                    Console.WriteLine("Password corretta");
                }
                else
                {
                    MessageBox.Show("Password errata");
                    rif.Enable_Buttons();
                    Disconnetti();
                    return;
                }

                IPEndPoint remoteEndPoint = (IPEndPoint)socket.RemoteEndPoint;
                //aspetto che il server mi confermi che e' in ascolto sulla porta 21
                socket.Receive(rec);
                if (Encoding.UTF8.GetString(rec).Trim('\0').Equals("FTPlistening"))
                {
                    if ((ftpClient.Connetti(remoteEndPoint.Address) == true))
                    {
                        Console.WriteLine("Connected to {0}:{1}", remoteEndPoint.Address, remoteEndPoint.Port);
                        socket.Send(Encoding.UTF8.GetBytes("ready"));


                        //per la cirlce progress bar
                        //Timeline.DesiredFrameRateProperty.OverrideMetadata(typeof(Timeline), new FrameworkPropertyMetadata { DefaultValue = 5 });

                        connesso = true;
                        ftpClient.setRif(rif, this);
                        rif.Change_Focus(this);
                    }
                    else
                    {
                        Console.WriteLine("Non e' stato possibile impostare la connessione al server");
                        Disconnetti();
                        return;
                    }
                }
                //provo ora ad instanziare il client ftp
            }
            catch (ArgumentNullException ane)
            {
                //Unica eccezione, non tre diverse
                Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
                MessageBox.Show("Impossibile connettersi...");
                Disconnetti();
                rif.Enable_Buttons();
                return;
            }
            catch (SocketException se)
            {
                if (se.ErrorCode == 10060)
                {
                    Console.WriteLine("SocketException : {0}", se.ToString());
                    MessageBox.Show("Timeout scaduto...");
                    Disconnetti();
                    rif.Enable_Buttons();
                    return;
                }
                else
                {
                    Console.WriteLine("SocketException : {0}", se.ToString());
                    MessageBox.Show("Impossibile connettersi...");
                    Disconnetti();
                    rif.Enable_Buttons();
                    return;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Unexpected exception : {0}", e.ToString());
                MessageBox.Show("Impossibile connettersi...");
                Disconnetti();
                rif.Enable_Buttons();
                return;
            }

            //connesso = true;
            //rif.Change_Focus(this);
        }

        public void Controlla()
        {
            if (connesso == true && socket != null)
            {
                rif.Change_Focus(this);
            }
            else
            {
                rif.Fine(this);
                Connetti();
            }
        }

        public void Disconnetti()
        {
            connesso = false;
            ftpClient.Disconnetti();
            if (socket != null)
                //if(socket.Connected == true)
                //    socket.Close();
                socket.Close();
            rif.restoreVisibility();
        }

        private void Grid_MouseUp(object sender, MouseButtonEventArgs e)
        {

            byte[] string_send = Encoding.UTF8.GetBytes("U");
            Console.WriteLine("You clicked me at " + e.GetPosition(this).ToString());
            //s.Send(string_send);
            try
            {
                socket.BeginSend(string_send, 0, string_send.Length, SocketFlags.None, BeginSendCallback, socket);
            }
            catch (SocketException)
            {
                MessageBox.Show("Connessione caduta");
                this.Hide();
                this.Disconnetti();
                rif.master.Children.Clear();
            }

        }

        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            socket.NoDelay = true;
            double x = e.GetPosition(this).X / System.Windows.SystemParameters.PrimaryScreenWidth;
            double y = e.GetPosition(this).Y / System.Windows.SystemParameters.PrimaryScreenHeight;

            byte[] string_send = Encoding.UTF8.GetBytes(x + ";" + y + "?");
            //Console.WriteLine("You moved me at " + e.GetPosition(this).ToString());
            try
            {
                //socket.Send(string_send);
                //socket.SendBufferSize = 1024;
                socket.BeginSend(string_send, 0, string_send.Length, SocketFlags.None, BeginSendCallback, socket);
            }
            catch (SocketException)
            {
                MessageBox.Show("Connessione caduta");
                this.Hide();
                this.Disconnetti();
                rif.master.Children.Clear();

            }

        }

        public static void BeginSendCallback(IAsyncResult ar) { }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            byte[] string_send = Encoding.UTF8.GetBytes("D");
            Console.WriteLine("You clicked me at " + e.GetPosition(this).ToString());
            //s.Send(string_send);
            try
            {
                socket.BeginSend(string_send, 0, string_send.Length, SocketFlags.None, BeginSendCallback, socket);
            }
            catch (SocketException)
            {
                MessageBox.Show("Connessione caduta");
                this.Hide();
                this.Disconnetti();
                rif.master.Children.Clear();

            }

        }

        private void Grid_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            byte[] string_send = Encoding.UTF8.GetBytes("R");
            Console.WriteLine("You clicked me at " + e.GetPosition(this).ToString());
            //s.Send(string_send);
            try
            {
                socket.BeginSend(string_send, 0, string_send.Length, SocketFlags.None, BeginSendCallback, socket);
            }
            catch (SocketException)
            {
                MessageBox.Show("Connessione caduta");
                this.Hide();
                this.Disconnetti();
                rif.master.Children.Clear();

            }

        }


        private void grid_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
            {
                byte[] string_send = Encoding.UTF8.GetBytes("W");
                Console.WriteLine(e.Delta.ToString());
                socket.BeginSend(string_send, 0, string_send.Length, SocketFlags.None, BeginSendCallback, socket);
            }
            else
            {

                byte[] string_send = Encoding.UTF8.GetBytes("P");
                Console.WriteLine(e.Delta.ToString());
                socket.BeginSend(string_send, 0, string_send.Length, SocketFlags.None, BeginSendCallback, socket);

            }
            return;
        }

        private void Grid_KeyDown(object sender, KeyEventArgs e)
        {
            socket.NoDelay = true;

            bool isD = false;
            bool isS = false;
            bool isL = false;
            bool isK = false;

            if (e.KeyboardDevice.IsKeyDown(Key.LeftAlt))
            {
                e.Handled = true;
            }
            if (e.KeyboardDevice.IsKeyDown(Key.LeftCtrl) || e.KeyboardDevice.IsKeyDown(Key.RightCtrl))
            {
                byte[] string_send;
                //switch (e.Key)
                //{

                //  case Key.D:
                string d = e.Key.ToString();
                Console.WriteLine(d);

                if (d.Equals(rif.ritornaClient))
                {
                    Console.WriteLine(d);
                    if (e.KeyboardDevice.IsKeyDown(Key.LeftCtrl))
                        this.Grid_KeyUp(this, new KeyEventArgs(Keyboard.PrimaryDevice, Keyboard.PrimaryDevice.ActiveSource, 0, Key.LeftCtrl));
                    else if (e.KeyboardDevice.IsKeyDown(Key.RightCtrl))
                        this.Grid_KeyUp(this, new KeyEventArgs(Keyboard.PrimaryDevice, Keyboard.PrimaryDevice.ActiveSource, 0, Key.RightCtrl));

                    this.Grid_KeyUp(this, new KeyEventArgs(Keyboard.PrimaryDevice, Keyboard.PrimaryDevice.ActiveSource, 0, Key.D));

                    this.Hide();
                    string_send = Encoding.UTF8.GetBytes("_XI_");
                    socket.BeginSend(string_send, 0, string_send.Length, SocketFlags.None, BeginSendCallback, socket);
                    rif.Show(); //Serve?
                    isD = true;
                    //break;
                }
                // case Key.S:
                Console.WriteLine(d);
                Console.WriteLine(rif.cambiaFocus);

                if (d.Equals(rif.cambiaFocus))
                {

                    Console.WriteLine(d);
                    if (e.KeyboardDevice.IsKeyDown(Key.LeftCtrl))
                        this.Grid_KeyUp(this, new KeyEventArgs(Keyboard.PrimaryDevice, Keyboard.PrimaryDevice.ActiveSource, 0, Key.LeftCtrl));
                    else if (e.KeyboardDevice.IsKeyDown(Key.RightCtrl))
                        this.Grid_KeyUp(this, new KeyEventArgs(Keyboard.PrimaryDevice, Keyboard.PrimaryDevice.ActiveSource, 0, Key.RightCtrl));
                    this.Grid_KeyUp(this, new KeyEventArgs(Keyboard.PrimaryDevice, Keyboard.PrimaryDevice.ActiveSource, 0, Key.S));

                    foreach (ServerWindow ser in rif.serverList)
                    {
                        if (rif.serverList.IndexOf(this) < rif.serverList.IndexOf(ser) && ser.socket != null && ser.Connesso == true)
                        {
                            string_send = Encoding.UTF8.GetBytes("_XI_");
                            socket.BeginSend(string_send, 0, string_send.Length, SocketFlags.None, BeginSendCallback, socket);
                            rif.Change_Focus(ser);
                            boss = false;
                            break;
                        }
                    }
                    string_send = Encoding.UTF8.GetBytes("_XI_");
                    socket.BeginSend(string_send, 0, string_send.Length, SocketFlags.None, BeginSendCallback, socket);
                    rif.Show();
                    this.Hide();
                    isS = true;
                    //break;
                }
                //case Key.L:
                if (d.Equals(rif.copiaClipboard))
                {
                    Console.WriteLine(d);
                    Thread retrieveThread = new Thread(ftpClient.Retrieve);
                    retrieveThread.Start();
                    isL = true;
                    if (e.KeyboardDevice.IsKeyDown(Key.LeftCtrl))
                        this.Grid_KeyUp(this, new KeyEventArgs(Keyboard.PrimaryDevice, Keyboard.PrimaryDevice.ActiveSource, 0, Key.LeftCtrl));
                    else if (e.KeyboardDevice.IsKeyDown(Key.RightCtrl))
                        this.Grid_KeyUp(this, new KeyEventArgs(Keyboard.PrimaryDevice, Keyboard.PrimaryDevice.ActiveSource, 0, Key.RightCtrl));
                    //  break;
                }

                //case Key.K:
                if (d.Equals(rif.copiaClipboardServer))
                {
                    Thread copyThread = new Thread(ftpClient.copyToServer);
                    copyThread.Start();
                    isK = true;
                    if (e.KeyboardDevice.IsKeyDown(Key.LeftCtrl))
                        this.Grid_KeyUp(this, new KeyEventArgs(Keyboard.PrimaryDevice, Keyboard.PrimaryDevice.ActiveSource, 0, Key.LeftCtrl));
                    else if (e.KeyboardDevice.IsKeyDown(Key.RightCtrl))
                        this.Grid_KeyUp(this, new KeyEventArgs(Keyboard.PrimaryDevice, Keyboard.PrimaryDevice.ActiveSource, 0, Key.RightCtrl));

                    // break;
                }
                //default:
                //  Console.WriteLine("Default case");
                //break;


            }


            //if (e.KeyboardDevice.IsKeyDown(Key.LeftAlt) && e.KeyboardDevice.IsKeyDown(Key.F))
            //{

            //    switch (e.Key)
            //    {

            //        case Key.F4:
            //            Console.WriteLine("Ho premuto alt+F4");
            //            if (e.KeyboardDevice.IsKeyDown(Key.LeftAlt))
            //                this.Grid_KeyUp(this, new KeyEventArgs(Keyboard.PrimaryDevice, Keyboard.PrimaryDevice.ActiveSource, 0, Key.LeftAlt));
            //            rif.Close();

            //            break;
            //    }


            //}

            try
            {
                if (isD == false && isS == false && isL == false && isK == false)
                {
                    if (e.Key.ToString() == "System")
                    {
                        Console.WriteLine("Down: " + e.SystemKey.ToString());
                        int valore = Convert.ToInt32(KeyInterop.VirtualKeyFromKey(e.SystemKey).ToString());
                        Console.WriteLine("Down: " + valore.ToString() + "\n");
                        byte[] string_send = Encoding.UTF8.GetBytes("_X" + valore.ToString() + "_");
                        socket.BeginSend(string_send, 0, string_send.Length, SocketFlags.None, BeginSendCallback, socket);
                    }
                    else
                    {
                        Console.WriteLine("Down: " + e.Key.ToString());
                        //Console.WriteLine("Down: " + Key.D);

                        int valore = Convert.ToInt32(KeyInterop.VirtualKeyFromKey(e.Key).ToString());
                        Console.WriteLine("Down: " + valore.ToString() + "\n");
                        byte[] string_send = Encoding.UTF8.GetBytes("_X" + valore.ToString() + "_");
                        socket.BeginSend(string_send, 0, string_send.Length, SocketFlags.None, BeginSendCallback, socket);
                    }
                }


            }
            catch (SocketException)
            {
                MessageBox.Show("Connessione caduta");
                this.Hide();
                this.Disconnetti();
                rif.master.Children.Clear();

            }


        }

        private void Grid_KeyUp(object sender, KeyEventArgs e)
        {
            socket.NoDelay = true;
            try
            {
                if (e.Key.ToString() == "System")
                {
                    Console.WriteLine("Up: " + e.SystemKey.ToString());
                    int valore = Convert.ToInt32(KeyInterop.VirtualKeyFromKey(e.SystemKey).ToString());
                    Console.WriteLine("Up: " + valore.ToString() + "\n");
                    byte[] string_send = Encoding.UTF8.GetBytes("_Y" + valore.ToString() + "_");
                    socket.BeginSend(string_send, 0, string_send.Length, SocketFlags.None, BeginSendCallback, socket);

                }
                else
                {
                    Console.WriteLine("Up: " + e.Key.ToString());
                    int valore = Convert.ToInt32(KeyInterop.VirtualKeyFromKey(e.Key).ToString());
                    Console.WriteLine("Up: " + valore.ToString() + "\n");
                    byte[] string_send = Encoding.UTF8.GetBytes("_Y" + valore.ToString() + "_");
                    socket.BeginSend(string_send, 0, string_send.Length, SocketFlags.None, BeginSendCallback, socket);

                }
            }
            catch (SocketException)
            {
                MessageBox.Show("Connessione caduta");
                this.Hide();
                this.Disconnetti();
                rif.master.Children.Clear();
            }
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            grid.Focus();
        }


        public void inCorso(bool set)
        {
            rif.Inizio(this, set);
            return;
        }

        public void fineTrasferimento()
        {
            rif.Fine(this);
            return;
        }


    }
}
