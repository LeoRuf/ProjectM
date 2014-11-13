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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Collections.Specialized;
using Ionic.Zip;


namespace MalnatiProject
{
    class FtpClient
    {
        private NetworkStream ns;
        private StreamWriter streamWriter;
        private StreamReader streamReader;
        private TcpClient tcpClient;
        private IPEndPoint ipEndPoint;
        private IPAddress ipLocalAddress;
        //private IPAddress ipAddress;
        //private IPAddress ipAddressClient;
        //private NetworkStream dataNs;
        //private StreamWriter dataSw;
        //private StreamReader dataSr;
        private Int16 port = 0;
        string hostname;
        private bool connesso;
        bool isDir = false;

        public MainWindow rif;

        public bool Connesso
        {
            get { return connesso; }
            set { connesso = value; }
        }

        public FtpClient(Int16 port, IPAddress localAddress)
        {
            this.port = ++port;
            //hostname = Dns.GetHostName();
            //IPHostEntry ipEntry = Dns.GetHostEntry(hostname);
            //IPAddress[] addresses = ipEntry.AddressList;

            //Console.WriteLine("Computer Host Name = " + hostname);

            //for (int i = 0; i < addresses.Length; i++)
            //{
            //    Console.WriteLine("IP Address n.{0} = {1} ", i, addresses[i].ToString());
            //    if (addresses[i].ToString().Length <= 16)
            //    {
            //        ipLocalAddress = addresses[i];
            //    }
            //}
            ipLocalAddress = localAddress;
        }

        public void setRif(MainWindow main)
        {
            rif = main;
        }

        public bool Connetti(IPAddress ipRemoteAddress)
        {
            try
            {
                ipEndPoint = new IPEndPoint(ipRemoteAddress, 21);
                tcpClient = new TcpClient();

                tcpClient.Connect(ipEndPoint);

                Console.WriteLine("connessione impostata con il server: " + tcpClient.Client.LocalEndPoint.ToString());
                ns = tcpClient.GetStream();
                streamWriter = new StreamWriter(ns);
                streamReader = new StreamReader(ns);
                string answer;

                if (ns.CanRead)
                {
                    answer = streamReader.ReadLine();
                    Console.WriteLine("This is the server answer: " + answer + "\n");
                    porta();
                    return true;
                }
                else
                {
                    Console.WriteLine("Non e' possibile comunicare con il server\n");
                    Disconnetti();
                    return false;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Disconnetti();
                return false;
            }
        }

        public void Disconnetti()
        {

            if (tcpClient != null && tcpClient.Connected == true)
            {
                if (streamWriter != null && ns.CanWrite && ns.CanRead)
                {
                    quit();
                    string answer;

                }
                tcpClient.Close();
            }
        }

        //private void Retrieve_button_Click(object sender, RoutedEventArgs e)
        //{

        //IPAddress[] localIPs = Dns.GetHostAddresses(Dns.GetHostName());
        //foreach (IPAddress addr in localIPs)
        //{
        //    if (addr.AddressFamily == AddressFamily.InterNetwork)
        //    {
        //        Console.WriteLine(addr);
        //    }
        //}


        //    if (ns.CanWrite)
        //    {
        //        //string string_command = Command_box.Text;
        //        //string[] stringhe = string_command.Split(' ');

        //        //if (stringhe[0] == "porta")
        //        //{
        //        //    string stringa = this.IPLocal_box.Text;
        //        //    string[] address = stringa.Split('.');

        //        //    byte[] bAddr = new byte[4];
        //        //    bAddr[0] = Convert.ToByte(address[0]);
        //        //    bAddr[1] = Convert.ToByte(address[1]);
        //        //    bAddr[2] = Convert.ToByte(address[2]);
        //        //    bAddr[3] = Convert.ToByte(address[3]);
        //        //    ipAddressClient = new IPAddress(bAddr);

        //        //    port = Convert.ToInt16(stringhe[1]);
        //        //    streamWriter.WriteLine(string_command);
        //        //    streamWriter.Flush();
        //        //    string answer = streamReader.ReadLine();
        //        //    MessageBox.Show("Il server ha risposto: " + answer);
        //        //    MessageBox.Show("Connessione dati sulla porta " + port);
        //        //}

        //        //if (stringhe[0] == "retr")
        //        //{
        //        //    if (port != 0)
        //        //        Retrieve(ipAddressClient, port, streamWriter, streamReader);
        //        //}
        //        ////Command_box.Text = String.Empty;

        //    }
        //    else
        //        Console.WriteLine("Non e' possibile scrivere sullo stream\n");

        //}

        public void Retrieve()
        {
            char[] string_line = new char[4096];
            byte[] buffer = new byte[4096];
            int total = 0;
            int count = 0;
            bool ricevuto = false;
            isDir = false;
            TcpListener dataListener = null;
            TcpClient dataConnection = null;
            FileStream file = null;
            string filePath=null;
            try
            {
                dataListener = new TcpListener(IPAddress.Any, port);
                dataListener.Start();

                streamWriter.WriteLine("retr");
                streamWriter.Flush();

                dataConnection = dataListener.AcceptTcpClient();
                dataListener.Stop();

                Console.WriteLine("Connessione dati stabilita! " + dataConnection.Client.LocalEndPoint.ToString());
                NetworkStream dataNetworkStream = dataConnection.GetStream();
                StreamReader dataStreamR = new StreamReader(dataNetworkStream);
                StreamWriter dataStreamW = new StreamWriter(dataNetworkStream);

                //string answer = dataStreamR.ReadLine();
                //string answer2 = dataStreamR.ReadLine();
                string fileName = dataStreamR.ReadLine();
               string[] fileNameArray = fileName.Split(';');
               string[] fileNameArray1=null;
                
                if(fileNameArray.Length==2){
                    isDir = true;
                    fileName = fileNameArray[1];
                    fileNameArray1= fileName.Split('.');

                }
                //string fileExtension = fileNameArray[1];

                //if (!(fileExtension.Equals("txt")==true || fileExtension.Equals("bmp")==true))
                //    return;

                if (dataNetworkStream.CanWrite)
                {
                    dataStreamW.WriteLine("go");
                    dataStreamW.Flush();
                }

                filePath = "C:\\temp\\" + fileName;
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
                 file = File.Create(filePath);

                //if(fileExtension.Equals("txt")==true)
                //{

                using (BinaryReader bReader = new BinaryReader(dataNetworkStream))
                {
                    using (BinaryWriter bWriter = new BinaryWriter(file))
                    {
                        while ((count = bReader.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            bWriter.Write(buffer, 0, count);
                            total += count;
                        }
                    }
                }
                file.Close();
                dataStreamR.Close();
                Console.WriteLine("File ricevuto!");
                ricevuto = true;
                
                //}

                dataConnection.Close();
                StringCollection s = new StringCollection();
                if (isDir) {
                    using (ZipFile zip = ZipFile.Read(filePath))
                    {
                        Directory.CreateDirectory("C:\\temp\\" +fileNameArray1[0]);   
                        foreach (ZipEntry e in zip)
                        {
                            e.Extract("C:\\temp\\"+fileNameArray1[0]);

                        }
                    }
                    filePath = "C:\\temp\\"+fileNameArray1[0];
                }
                s.Add(filePath);
                rif.SetClip(s);
            }
            catch (Exception)
            {
                file.Close();
                if (ricevuto == false)
                    
                    File.Delete(filePath);
                    
                if (dataListener != null)
                    dataListener.Stop();
                if (dataConnection != null)
                    if (dataConnection.Connected)
                        dataConnection.Close();
                //Disconnetti(); //meglio di no, un problema sulla connessione dati non deve implicare un problema sul canale di ctrl
            }
        }

        public void porta()
        {
            try
            {
                streamWriter.WriteLine("porta " + port);
                streamWriter.Flush();
                string answer = streamReader.ReadLine();
                Console.WriteLine("Il server ha risposto: " + answer);
                Console.WriteLine("Connessione dati sulla porta " + port);
            }
            catch (Exception)
            {
                Disconnetti();
            }

        }

        public void quit()
        {
            streamWriter.WriteLine("quit");
            streamWriter.Flush();
        }
    }
}
