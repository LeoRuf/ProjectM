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
        public MainWindow rif;

        public FtpClient(Int16 port) {
            this.port = port++;
            hostname = Dns.GetHostName();
            IPHostEntry ipEntry = Dns.GetHostEntry(hostname);
            IPAddress[] addresses = ipEntry.AddressList;

            Console.WriteLine("Computer Host Name = " + hostname);

            for (int i = 0; i < addresses.Length; i++)
            {
                Console.WriteLine("IP Address n.{0} = {1} ", i, addresses[i].ToString());
                if (addresses[i].ToString().Length <= 16)
                {
                    ipLocalAddress = addresses[i];
                }
            }
        
        
        }


        public void Connetti(IPAddress ipRemoteAddress)
        {
            //string stringa = this.IP_box.Text;
            //string[] address = stringa.Split('.');

            //byte[] bAddr = new byte[4];
            //bAddr[0] = Convert.ToByte(address[0]);
            //bAddr[1] = Convert.ToByte(address[1]);
            //bAddr[2] = Convert.ToByte(address[2]);
            //bAddr[3] = Convert.ToByte(address[3]);

            //ipAddress = new IPAddress(bAddr);
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
                }
                else
                   Console.WriteLine("Non e' possibile comunicare con il server\n");

            }
            catch(Exception e) {
                Console.WriteLine(e.Message);
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
            int total = 0;
            int count = 0;
            TcpListener dataListener = new TcpListener(ipLocalAddress, port);
            dataListener.Start();
            //MessageBox.Show("In ascolto sulla porta " + port + "\ndall'IP " + ip.ToString());
            //str.WriteLine("retr ciao");
            streamWriter.WriteLine("retr");
            streamWriter.Flush();

            //Console.WriteLine("attendo file");

            TcpClient dataConnection = dataListener.AcceptTcpClient();
            //dataListener.Stop();
            //string answer = strreader.ReadLine();
            //MessageBox.Show(answer);
            Console.WriteLine("Connessione dati stabilita! " + dataConnection.Client.LocalEndPoint.ToString());
            NetworkStream dataNetworkStream = dataConnection.GetStream();
            StreamReader dataStreamR = new StreamReader(dataNetworkStream);
            StreamWriter dataStreamW = new StreamWriter(dataNetworkStream);

            //dataStreamW.WriteLine("Connesso!");
            //dataStreamW.Flush();
            //string answer2 = dataStreamR.ReadLine();
            //MessageBox.Show(answer2);
            //FileStream file = new FileStream("prova.txt", FileMode.Open, FileAccess.Write);

            //if (File.Exists("C:\\Users\\Riccardo\\Documents\\Visual Studio 2013\\Projects\\FTPClientClipboard\\FTPClientClipboard\\bin\\Debug\\prova.txt")) ;
            //{
            //    File.Delete("C:\\Users\\Riccardo\\Documents\\Visual Studio 2013\\Projects\\FTPClientClipboard\\FTPClientClipboard\\bin\\Debug\\prova.txt");
            //}
            if (File.Exists("C:\\fileTemporanei\\prova.txt"))
            {
                File.Delete("C:\\fileTemporanei\\prova.txt");
            }
            FileStream file = File.Create("C:\\fileTemporanei\\prova.txt");
            StreamWriter fileStream = new StreamWriter(file);


            while ((count = dataStreamR.Read(string_line, 0, string_line.Length)) > 0)
            {

                fileStream.Write(string_line, 0, count);
                total += count;
                //Console.WriteLine(string_line);
            }
            fileStream.Close();
            dataStreamR.Close();
            Console.WriteLine("File ricevuto!");
            /************************/
            //clipboard
            //Clipboard.SetData(DataFormats.Text, (Object)milan);
            
            StringCollection s = new StringCollection();
                s.Add("C:\\fileTemporanei\\prova.txt");
            
            rif.SetClip(s);
            

            /************************/
        }


        public void porta() {

                //string stringa = ip;
                //string[] address = stringa.Split('.');

                //byte[] bAddr = new byte[4];
                //bAddr[0] = Convert.ToByte(address[0]);
                //bAddr[1] = Convert.ToByte(address[1]);
                //bAddr[2] = Convert.ToByte(address[2]);
                //bAddr[3] = Convert.ToByte(address[3]);
                //ipAddressClient = new IPAddress(bAddr);

               // port = Convert.ToInt16(stringhe[1]);
                streamWriter.WriteLine("porta " + port);
                streamWriter.Flush();
                string answer = streamReader.ReadLine();
                Console.WriteLine("Il server ha risposto: " + answer);
                Console.WriteLine("Connessione dati sulla porta " + port);
        }
    }
}
