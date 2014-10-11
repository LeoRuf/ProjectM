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
        public static ManualResetEvent allDone = new ManualResetEvent(false);
        bool connesso = false;


        public String Address
        {
            get {return ip + " " + Convert.ToString(porta);} 
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
            return this.ip + "    " + this.porta; //tostring

        }

        public void Connetti()
        {
            try
            {
                // Connects to the server and receives the dummy byte array, then closes the socket.
                IPEndPoint lep = new IPEndPoint(IPAddress.Parse(ip), Convert.ToInt16(porta));
                Console.WriteLine("STo per fare la connect");
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                socket.BeginConnect(lep, new AsyncCallback(ConnectCallback1), socket);
                allDone.WaitOne();
                socket.Send(Encoding.UTF8.GetBytes(password));
                
                socket.Receive(rec);
                
                if (Encoding.UTF8.GetString(rec).Trim('\0').Equals(password)) {}
                else {
                    MessageBox.Show("Password errata");
                    return;}
                connesso = true;

                Console.WriteLine("STo per ricevere");

                IPEndPoint remoteEndPoint = (IPEndPoint)socket.RemoteEndPoint;
                Console.WriteLine("Connected to {0}:{1}", remoteEndPoint.Address, remoteEndPoint.Port);
                this.Show();

                //byte[] buffer = new byte[4];
                //Console.WriteLine((s.Receive(buffer) == buffer.Length) ? "Received message" : "Incorrect message");
                //Console.WriteLine(Encoding.UTF8.GetString(buffer));
                //s.Send(Encoding.UTF8.GetBytes("Che bello funziona"));


            }
            catch (Exception ex)
            {
                // An exception occured: should be a SocketException due to a timeout if we chose to bind to an address.
                Console.WriteLine("ERROR: " + ex.ToString());
            }




        }

        public static void ConnectCallback1(IAsyncResult ar)
        {
            allDone.Set();
            Socket s = (Socket)ar.AsyncState;
            s.EndConnect(ar);
        }

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
