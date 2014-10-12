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
using System.Text.RegularExpressions;
using System.Windows;

namespace MalnatiProject
{
    partial class ServerWindow : Window
    {

        public String ip;
        public Int16 porta;
        public String password;
        public Socket socket;
        byte[] rec= new byte[64];
        public MainWindow rif;
        public static ManualResetEvent allDone = new ManualResetEvent(false);
        bool connesso = false;


        public String Address
        {
            get {return     "      " + ip + "             " + Convert.ToString(porta);} 
            set{}
        }



        public ServerWindow(String ip, Int16 porta, String password)
        {
            InitializeComponent();
            this.ip = ip;
            this.porta = porta;
            this.password = password;


        }


        public String ToString()
        {
            return this.ip + "    " + this.porta; 

        }

        public void Connetti()
        {
                try
                {
                    IPEndPoint lep = new IPEndPoint(IPAddress.Parse(ip), Convert.ToInt16(porta));
                    Console.WriteLine("Sto per fare la connect");
                    socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                   

                    socket.Connect(lep);
                    
                    Console.WriteLine("Socket connected to {0}",
                        socket.RemoteEndPoint.ToString());

                    socket.Send(Encoding.UTF8.GetBytes(password));

                    socket.Receive(rec);

                    if (Encoding.UTF8.GetString(rec).Trim('\0').Equals(password)) { }
                    else
                    {
                        MessageBox.Show("Password errata");
                        return;
                    }

                    connesso = true;

                    

                    IPEndPoint remoteEndPoint = (IPEndPoint)socket.RemoteEndPoint;
                    Console.WriteLine("Connected to {0}:{1}", remoteEndPoint.Address, remoteEndPoint.Port);


                }
                catch (ArgumentNullException ane)
                {
                    //Unica eccezione, non tre diverse
                    Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
                    MessageBox.Show("Impossibile connettersi...");
                    rif.Enable_ConnettiButton();
                    return;

                }
                catch (SocketException se)
                {
                    Console.WriteLine("SocketException : {0}", se.ToString());
                    MessageBox.Show("Impossibile connettersi...");
                    rif.Enable_ConnettiButton();
                    return;
                }
                catch (Exception e)
                {
                    Console.WriteLine("Unexpected exception : {0}", e.ToString());
                    MessageBox.Show("Impossibile connettersi...");
                    rif.Enable_ConnettiButton();
                    return;

                }

                rif.Change_Focus(this);

        }

        //public static void ConnectCallback1(IAsyncResult ar)
        //{
        //    Console.WriteLine("4.5");

        //    Console.WriteLine("5");
        //    Socket s = (Socket)ar.AsyncState;
        //    Console.WriteLine("6");

        //    s.EndConnect(ar);
        //    Console.WriteLine("7");
        //    allDone.Set();


        //}

        private void Grid_MouseUp(object sender, MouseButtonEventArgs e)
        {

            byte[] string_send = Encoding.UTF8.GetBytes("U");
            Console.WriteLine("You clicked me at " + e.GetPosition(this).ToString());
            //s.Send(string_send);
            socket.BeginSend(string_send, 0, string_send.Length, SocketFlags.None, BeginSendCallback, socket);
        }

        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            double x = e.GetPosition(this).X /System.Windows.SystemParameters.PrimaryScreenWidth;
            double y = e.GetPosition(this).Y / System.Windows.SystemParameters.PrimaryScreenHeight;

            byte[] string_send = Encoding.UTF8.GetBytes(x+";"+y+ "?");
            Console.WriteLine("You moved me at " + e.GetPosition(this).ToString());
            //socket.Send(string_send);
            socket.BeginSend(string_send, 0, string_send.Length, SocketFlags.None, BeginSendCallback, socket);
   
            
        }

        public static void BeginSendCallback(IAsyncResult ar) { }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            byte[] string_send = Encoding.UTF8.GetBytes("D");
            Console.WriteLine("You clicked me at " + e.GetPosition(this).ToString());
            //s.Send(string_send);
            socket.BeginSend(string_send, 0, string_send.Length, SocketFlags.None, BeginSendCallback, socket);
        }

        private void Grid_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            byte[] string_send = Encoding.UTF8.GetBytes("R");
            Console.WriteLine("You clicked me at " + e.GetPosition(this).ToString());
            //s.Send(string_send);
            socket.BeginSend(string_send, 0, string_send.Length, SocketFlags.None, BeginSendCallback, socket);
        }



    }
}
