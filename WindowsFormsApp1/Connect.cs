using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Runtime.InteropServices;
using System.IO;
using System.Globalization;

namespace WindowsFormsApp1
{
    public partial class Connect : Form
    {
        private delegate void printer(string data);
        private delegate void cleaner();
        printer Printer;
        cleaner Cleaner;
        private Socket _serverSocket;
        private Thread _clientThread;
        private string _serverHost;
        // private string _serverHost = "localhost";
        private const int _serverPort = 9933;
        private volatile string[] response = new string[] { "0", "0" };
        public Connect()
        {
            InitializeComponent();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
        
        private void listner()
        {
            while (_serverSocket.Connected)
            {
                try
                {
                    byte[] buffer = new byte[8196];
                    int bytesRec = _serverSocket.Receive(buffer);
                    string data = Encoding.UTF8.GetString(buffer, 0, bytesRec);
                    if (data.Contains("#newresponse"))
                    {

                        response = data.Split('&')[1].Split('%');
                        continue;
                    }
                    if (data.Contains("#updatelog"))
                    {
                        UpdateLog(data);
                        continue;
                    }

                }
                catch { print("Connection lost"); /*fixer();*/ }

            }
        }

        private void clearLog()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(Cleaner);
                return;
            }
            //   chatBox.Clear();
        }

        private void UpdateLog(string data)
        {
            //#updatelog&userName~data|username~data
            clearLog();
            string[] messages = data.Split('&')[1].Split('|');
            int countMessages = messages.Length;
            if (countMessages <= 0) return;
            for (int i = 0; i < countMessages; i++)
            {
                try
                {
                    if (string.IsNullOrEmpty(messages[i])) continue;
                    print(String.Format("[{0}]:{1}.", messages[i].Split('~')[0], messages[i].Split('~')[1]));
                }
                catch { continue; }
            }
        }


        private void print(string msg)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(Printer, msg);
                return;
            }

        }

        private void fun_connect()
        {
            try
            {
                // IPHostEntry ipHost = Dns.GetHostEntry(_serverHost);
                //IPAddress ipAddress = ipHost.AddressList[0];
                IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse(_serverHost), _serverPort);
                _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                _serverSocket.Connect(ipEndPoint);
            }
            catch {
                label1.Text = "Соединение установлено";
               // label1.ForeColor = Color.Green;
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            _serverHost = textBox1.Text;
            //string _serverHost = textBox1.Text;
            fun_connect();
            label1.ForeColor = Color.Green;
            if (_serverSocket.Connected)
            {
                _clientThread = new Thread(listner);
                _clientThread.IsBackground = true;
                _clientThread.Start();
            }
        }

    }
}
