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
        private Int16 port = 0; //porta usata per ascoltare in caso di connessione dati ftp, uguale per server e client
        string hostname;
        private bool connesso;
        bool isDir = false;
        static bool set = false;
        TcpClient dataConnection;

        public static ServerWindow rif_ser;
        public MainWindow rif;

        TcpClient _dataServer;
        IPEndPoint _dataEndpoint;
        string _transferType;
        private string fileName;
        private string fileExtension;


        public bool Connesso
        {
            get { return connesso; }
            set { connesso = value; }
        }

        public FtpClient(Int16 port, IPAddress localAddress)
        {
            this.port = ++port; //porta su cui si mette in ascolto il client per permettere connessione attiva a server
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

        public void setRif(MainWindow main, ServerWindow se)
        {
            rif = main;
            rif_ser = se;
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

            if (tcpClient != null)
            {
                if (tcpClient.Connected == true)
                {
                    if (streamWriter != null && ns.CanWrite && ns.CanRead)
                    {
                        //quit();
                    }
                    //tcpClient.Close();
                }
                tcpClient.Close();
            }
            if (_dataServer != null)
                //if (_dataServer.Connected == true)
                //    _dataServer.Close();
                _dataServer.Close();
        }

        public void Retrieve()
        {
            char[] stringBuffer = new char[4096];
            byte[] buffer = new byte[4096];
            int total = 0;
            int count = 0;
            FileStream file=null;
            TcpListener dataListener = null;
            string filePath = null;
            bool ricevuto = false;
            bool isText;
            isDir = false;
            bool yes = false;
            try
            {
                
                if (dataConnection != null)
                {
                    if (dataConnection.Connected)
                    {
                        bool answer = rif.ShowOptions();
                        if (answer == true)
                        {
                            yes = true;
                            rif_ser.fineTrasferimento();
                            throw new Exception();

                        }
                        return;
                    }
                }

            dataConnection = null;
             
            
               dataListener = new TcpListener(IPAddress.Any, port);
                dataListener.Start();

                streamWriter.WriteLine("retr");
                streamWriter.Flush();

                dataConnection = dataListener.AcceptTcpClient();
                dataListener.Stop();

                //Console.WriteLine("Connessione dati stabilita! " + dataConnection.Client.LocalEndPoint.ToString());
                NetworkStream dataNetworkStream = dataConnection.GetStream();
                StreamReader dataStreamR = new StreamReader(dataNetworkStream);
                StreamWriter dataStreamW = new StreamWriter(dataNetworkStream);

                string fileName = dataStreamR.ReadLine(); //legge il messaggio mandato dal server
                string text = null;
              
                string[] sizeArray = fileName.Split('!');
                long size = Convert.ToInt64(sizeArray[0]);
                fileName = sizeArray[1];

                if (fileName.Equals("Text"))
                {
                    //File.Open("tmpStringFile.txt", FileMode.Create);
                    isText = true;
                    if (dataNetworkStream.CanWrite)
                    {
                        dataStreamW.WriteLine("go");
                        dataStreamW.Flush();
                    }
                    using (StreamReader reader = new StreamReader(dataNetworkStream))
                    {
                        //using (StreamWriter writer = new StreamWriter(File.Open("tmpStringFile.txt", FileMode.Create)))
                        //{
                        //    while ((count = reader.Read(stringBuffer, 0, stringBuffer.Length)) > 0)
                        //    {
                        //        writer.Write(stringBuffer, 0, count);
                        //        total += count;
                        //    }
                        //}
                        text = reader.ReadToEnd();

                        if (text.Length != size) //plain text corrotto
                            throw new Exception();
                    }

                }
                else
                {
                    isText = false;
                    string[] fileNameArray = fileName.Split(';'); //; inviato solo nel caso di directory
                    string[] fileNameArray1 = null; //utilizzato per tirare fuori il nome della directory (in ricez ho nomeDir.zip)

                    if (fileNameArray.Length == 2)
                    {
                        isDir = true;
                        fileName = fileNameArray[1]; //stacco dal fileName ricevuto il "dir;"
                        fileNameArray1 = fileName.Split('.');
                    }

                    if (dataNetworkStream.CanWrite)
                    {
                        dataStreamW.WriteLine("go");
                        dataStreamW.Flush();
                    }

                    filePath = "C:\\temp\\" + fileName; //path al file di destinazione in temp (file o cartella comp)
                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                    }
                    file = File.Create(filePath);

                    //fase di lettura indipendente dal tipo di file (i direttori sono letti e memorizzati come files compressi)
                    using (BinaryReader bReader = new BinaryReader(dataNetworkStream))
                    {
                        using (BinaryWriter bWriter = new BinaryWriter(file))
                        {
                            set = false;
                            rif_ser.inCorso(set);
                            while ((count = bReader.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                bWriter.Write(buffer, 0, count);
                                total += count;
                            }
                        }
                    }
                    rif_ser.fineTrasferimento();
                    file.Close();
                    FileInfo fInfo = new FileInfo(filePath);
                    if (fInfo.Length != size)
                        throw new Exception(); //file corrotto


                    if (isDir)
                    {
                        using (ZipFile zip = ZipFile.Read(filePath))
                        {
                            if (Directory.Exists("C:\\temp\\" + fileNameArray1[0])) {
                                DeleteDirectory("C:\\temp\\" + fileNameArray1[0]);
                            }
                            Directory.CreateDirectory("C:\\temp\\" + fileNameArray1[0]);
                            foreach (ZipEntry e in zip)
                            {
                                e.Extract("C:\\temp\\" + fileNameArray1[0]);
                            }
                        }
                        if (File.Exists(filePath)) //elimino il file compresso ricevuto, ora ho la cartella decompressa
                            File.Delete(filePath);
                        filePath = "C:\\temp\\" + fileNameArray1[0]; //imposto il nome del path alla cartella decompressa
                    }
                    //Console.WriteLine("File ricevuto!");    
                    ricevuto = true;
                }
                dataStreamR.Close();
                dataConnection.Close(); //connessione dati abbattuta dopo ogni trasferimento

                if (isText)
                    rif.SetClipText(text);
                else
                {
                    StringCollection s = new StringCollection();
                    s.Add(filePath);
                    rif.SetClip(s);
                }
            }
            catch (Exception)
            {
                if (file != null)
                    file.Close();
                if (ricevuto == false && yes==true)
                { //per il plain text non devo fare niente, al rientro dalla funzione la stringa corrotta e' eliminata senza essere copiata nella clipboard
                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                        rif.DisplayErrorMessage();
                    }
                }
                if (dataListener != null)
                    dataListener.Stop();
                if (dataConnection != null)
                    if (dataConnection.Connected)
                        dataConnection.Close();
                //Disconnetti(); //meglio di no, un problema sulla connessione dati non deve implicare un problema sul canale di ctrl
            }
        }


        public static void DeleteDirectory(string target_dir)
        {
            string[] files = Directory.GetFiles(target_dir);
            string[] dirs = Directory.GetDirectories(target_dir);

            foreach (string file in files)
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }

            foreach (string dir in dirs)
            {
                DeleteDirectory(dir);
            }

            Directory.Delete(target_dir, false);
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
            if (ns.CanWrite)
            {
                streamWriter.WriteLine("quit");
                streamWriter.Flush();
            }
        }

        /*******************************************/
        //metodi server-like

        //questo metodo tenta solo di connettersi al server per potergli in seguito trasferire la clipboard
        public void copyToServer()
        {
            try
            {

            if (_dataServer != null) {
                if (_dataServer.Connected) {
                    bool answer1= rif.ShowOptions();
                    if (answer1 == true) {
                        rif_ser.fineTrasferimento();
                        throw new Exception();
                    }
                    return;
                }
            }
            //mando comando al server
            streamWriter.WriteLine("copy");
            streamWriter.Flush();

            //aspetto che il server sia pronto, ovvero che sia in listening
            string answer = streamReader.ReadLine();

            _dataEndpoint = ipEndPoint; //l'endPoint e' lo stesso server
            _dataEndpoint.Port = port; //gli cambio pero' la porta=porta connessione "mouse+keyboard" + 1

            //instauro la connessione in modo attivo
           
                string pathname = "";
                _dataServer = new TcpClient(); //creo la connessione 
                _dataServer.BeginConnect(_dataEndpoint.Address, _dataEndpoint.Port, DoCopyToServer, pathname);
                return;
            }
            catch (Exception)
            {
                if (_dataServer != null)
                    if (_dataServer.Connected)
                        _dataServer.Close();
                return;
            }
        }

        private void DoCopyToServer(IAsyncResult result)
        {
            try
            {
                
                _dataServer.EndConnect(result); //modalita' attiva

                _transferType = "T"; //ipotizzo di trasferire un plain text, in caso negativo lo cambio
                
                object plainText = null;
                object stringObject = null; // Used to store the return value
                var thread = new Thread(

                  () =>
                  {
                      IDataObject dataObject = Clipboard.GetDataObject();
                      if (Clipboard.ContainsFileDropList())
                      {
                          plainText = false;
                          StringCollection strColl = Clipboard.GetFileDropList();
                          StringEnumerator myEnumerator = strColl.GetEnumerator();
                          while (myEnumerator.MoveNext())
                          {
                              stringObject = myEnumerator.Current.ToString();
                          }
                      }
                      else
                      {
                              plainText = true;
                              stringObject = Clipboard.GetText();
                      }
                  });
                thread.SetApartmentState(ApartmentState.STA);
                thread.Start();
                thread.Join();

                long size;
                string path = null;
                string message = null;
                string textClipboard = null;
                
                if ((bool)plainText)
                {
                    //message = DoRetrievePlainText((string)stringObject);
                    textClipboard = (string)stringObject;
                    size = textClipboard.Length;
                    message = size + "!Text";
                }
                else
                {
                    path = (string)stringObject;
                    message = DoRetrieveFileOrDir(ref path);
                    FileInfo fInfo = new FileInfo(path);
                    size = fInfo.Length;
                }

                using (NetworkStream dataStream = _dataServer.GetStream())
                {
                    //stream per le comunicaz preliminari di controllo
                    //non mandate sul canale di controllo perche' verrebbero intercettate dal thread
                    //che esegue il dispatching dei comandi al server

                    StreamReader dataCtrlStreamR = new StreamReader(dataStream);
                    StreamWriter dataCtrlStreamW = new StreamWriter(dataStream);

                    dataCtrlStreamW.WriteLine(message);
                    dataCtrlStreamW.Flush();

                    //go
                    string answer = dataCtrlStreamR.ReadLine();

                    if (_transferType.Equals("T"))
                    {
                        CopyStreamPlainText(textClipboard, dataStream);
                    }
                    else
                    {
                        using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
                        {
                            CopyStream(fs, dataStream, 4096);
                            //se ho inviato una cartella (compressa) devo eliminare lo zip creato
                            if (message.StartsWith("dir;"))
                            {
                                if (File.Exists(path))
                                    File.Delete(path);
                            }
                        }
                    }

                    dataCtrlStreamR.Close();
                    dataCtrlStreamW.Close();
                    dataStream.Close();
                }
                _dataServer.Close();
            }
            catch (Exception)
            {
                if (_dataServer != null)
                    if (_dataServer.Connected)
                        _dataServer.Close();
            }
        }

        private string DoRetrieveFileOrDir(ref string path)
        {
            try
            {
                _transferType = "F";
                FileInfo fInfo = new FileInfo(path);
                fileName = fInfo.Name;
                string[] extensionArray = fileName.Split('.');
                if (extensionArray.Length == 1) //caso di direttorio
                {
                    //modifico path con il nuovo path al direttorio compresso
                    path = CompressDir(path);
                    FileInfo modifiedfInfo = new FileInfo(path); //in questo caso dobbiamo inviare qualcosa di diverso dal contenuto della clipboard
                    fileName = modifiedfInfo.Name; //ritocco fileName con il nome della cartella compressa contenente il direttorio copiato
                    long size = modifiedfInfo.Length;
                    return size + "!dir;" + fileName; //devo informare il client che sto inviando un direttorio sotto forma di file compresso
                }
                else // caso di file
                {
                    fileExtension = extensionArray[1];
                    long size = fInfo.Length;
                    return size + "!" + fileName;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private string CompressDir(string path)
        {
            ZipFile zip = new ZipFile();
            zip.AddDirectory(path);
            string fileNameZip = path + ".zip";
            zip.Save(string.Format(fileNameZip));
            return fileNameZip;
        }

        private static long CopyStream(Stream input, Stream output, int bufferSize)
        {
            //il metodo permette di copiare qualsiasi tipo di file, lavorando in binario
            //il client sa riconoscere il tipo di file corretto grazie al messaggio inviatogli dal server con il nome del file
            byte[] buffer = new byte[bufferSize];
            int count = 0;
            long total = 0;
            try
            {
                //provo ad associare lo stream input (filestream) ad un binary reader
                using (BinaryReader bReader = new BinaryReader(input))
                {
                    //ed lo stream di output (network stream) ad un binary writer
                    using (BinaryWriter bWriter = new BinaryWriter(output))
                    {
                        set = true;
                        rif_ser.inCorso(set);
                        while ((count = bReader.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            bWriter.Write(buffer, 0, count);
                            total += count;
                        }
                    }
                }
                
                rif_ser.fineTrasferimento();
            }
            catch (Exception)
            {
                throw;
            }
            return total;
        }

        private static long CopyStreamPlainText(string input, Stream output)
        {
            //byte[] bufferString;
            //char[] bufferChar;
            //int count = 0;
            long total = 0;

            try
            {
                //bufferString = Encoding.UTF8.GetBytes(input);
                //using (MemoryStream ms = new MemoryStream(bufferString))
                //{
                //    using (StreamReader streamReader = new StreamReader(ms))
                //    {
                //        using(StreamWriter streamWriter = new StreamWriter(output))
                //        {
                //            while ((count = streamReader.Read(bufferChar, 0, bufferChar.Length)) > 0)
                //            {
                //                streamWriter.Write()
                //            }
                //        }
                //    }
                //}
                using (StreamWriter streamWriter = new StreamWriter(output))
                    streamWriter.Write(input);

                return total;
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
