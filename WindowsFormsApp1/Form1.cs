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

    public partial class Form1 : Form
    {
        private delegate void printer(string data);
        private delegate void cleaner();
        printer Printer;
        cleaner Cleaner;
        private Socket _serverSocket;
        private Thread _clientThread;
        private string _serverHost = "localhost";
        private const int _serverPort = 9933;
        private volatile string[] response = new string[] { "0", "0" };
        private const string IS_RENTABLE_CODE = "Введенные данные показывают хороший результат рентабельности";
        private const string NO_RENTABLE_CODE = "Введенные данные показывают плохой результат рентабельности";
        private const string APPROXIMATE_INCOME = "Приблизительный доход = ";

        public class XPacket
        {
            public static XPacket Parse(byte[] packet)
            {
                if (packet.Length < 7)
                {
                    return null;
                }

                if (packet[0] != 0xAF ||
                    packet[1] != 0xAA ||
                    packet[2] != 0xAF)
                {
                    return null;
                }

                /*  var mIndex = packet.Length - 1;

                   if (packet.[mIndex - 1] != 0xFF ||
                       packet[mIndex] != 0x00)
                   {
                       return null;
                   }*/
                var type = packet[3];
                var subtype = packet[4];
                var xpacket = Create(type, subtype);

                var fields = packet.Skip(5).ToArray();

                if (fields.Length == 2)
                {
                    return xpacket;
                }
                var id = fields[0];
                var size = fields[1];

                var contents = size != 0 ?
                fields.Skip(2).Take(size).ToArray() : null;

                xpacket.Field = (new XPacketField
                {
                    FieldID = id,
                    FieldSize = size,
                    Contents = contents
                });

                return xpacket;

            }

            private XPacket() { }
            public static XPacket Create(byte type, byte subtype)
            {
                return new XPacket
                {
                    PacketType = type,
                    PacketSubtype = subtype
                };
            }
            public byte PacketType { get; private set; }
            public byte PacketSubtype { get; private set; }
            public XPacketField Field { get; set; } = new XPacketField();


            public byte[] ToPacket(byte[] data)
            {
                var packet = new MemoryStream();

                packet.Write(
                new byte[] { 0xAF, 0xAA, 0xAF, PacketType, PacketSubtype }, 0, 5);
                var field = Field;
                field.FieldID = 0;

                field.Contents = data;
                field.FieldSize = Convert.ToByte(field.Contents.Length);

                packet.Write(new[] { field.FieldID, field.FieldSize }, 0, 2);
                packet.Write(field.Contents, 0, field.Contents.Length);

                packet.Write(new byte[] { 0xFF, 0x00 }, 0, 2);

                return packet.ToArray();
            }
        }

        public class XPacketField
        {
            public byte FieldID { get; set; }
            public byte FieldSize { get; set; }
            public byte[] Contents { get; set; }
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
        public static class ThreadHelperClass
        {
            delegate void EnabledCallback(Form f, Control ctrl, string data);
            /// <summary>
            /// Set text property of various controls
            /// </summary>
            /// <param name="form">The calling form</param>
            /// <param name="ctrl"></param>
            /// <param name="data"></param>

            public static void Enabled(Form form, Control ctrl, string data)
            {
                // InvokeRequired required compares the thread ID of the 
                // calling thread to the thread ID of the creating thread. 
                // If these threads are different, it returns true. 
                if (ctrl.InvokeRequired)
                {
                    EnabledCallback d = new EnabledCallback(Enabled);
                    form.Invoke(d, new object[] { form, ctrl, data });
                }
                else
                {
                    ctrl.Text = data;
                }
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
            catch { print("Сервер недоступен!"); }
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
        private void send(string data)
        {
            var packet = XPacket.Create(1, 0);
            //experimental
            byte[] buffer = Encoding.UTF8.GetBytes(data);
            var packetbytes = packet.ToPacket(buffer);
            int bytesSent = _serverSocket.Send(packetbytes);
        }
        private void sendMessage()
        {
            try
            {
                string data = textBox1.Text + "%" + textBox2.Text + "%" + textBox3.Text + "%" + textBox4.Text + "%" + textBox5.Text + "%" + textBox6.Text + "%" + textBox7.Text;
                if (string.IsNullOrEmpty(data)) return;
                send("#newmsg&" + data);
                //    chat_msg.Text = string.Empty;
            }
            catch { MessageBox.Show("Ошибка при отправке сообщения!"); }
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



        Thread TH;
        public Form1()
        {
            InitializeComponent();
        }

        private void SaveAssToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //samole
            //Файл/Сохранить как
        }

        private void SaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Файл/Сохранить

        }
        private void CopytoolStripMenuItem1_Click(object sender, EventArgs e)
        {
            //Правка/Копировать


        }

        private void PastetoolStripMenuItem2_Click(object sender, EventArgs e)
        {
            //Правка/Вставить

        }

        private void FonttoolStripMenuItem4_Click_1(object sender, EventArgs e)
        {
            //Вид/темная тема
            this.BackColor = System.Drawing.Color.Gray;

        }

        private void SettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Настройки
            //this.Close();
            TH = new Thread(open);
            TH.SetApartmentState(ApartmentState.STA);
            TH.Start();
        }

        public void open(object obj)
        {
            Application.Run(new Settings());    
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            //Вид/светлая тема
            this.BackColor = System.Drawing.Color.White;
          //  this.BackColor = System.Windows.Forms.MenuStrip.DefaultBackColor;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
            textBox5.Text = "";
            textBox6.Text = "";
            textBox7.Text = "";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //РАСЧИТАТЬ  письку

            sendMessage();

        }

        private void button3_Click(object sender, EventArgs e)
        {
            _serverHost = "192.168.1.66";
            fun_connect();
            if (_serverSocket.Connected)
            {
                _clientThread = new Thread(listner);
                _clientThread.IsBackground = true;
                _clientThread.Start();
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            var culture = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            culture.NumberFormat.NumberDecimalSeparator = ".";
            if (float.Parse(response[0],culture) != 0.00000000)
            {
                if (float.Parse(response[0],culture) > 0.5)
                {
                    label8.Text = IS_RENTABLE_CODE;
                }
                else
                {
                    label8.Text = NO_RENTABLE_CODE;
                }
            }
            label9.Text = APPROXIMATE_INCOME + response[1];
        }
    }
}
