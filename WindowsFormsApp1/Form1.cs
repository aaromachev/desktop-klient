using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Net.Sockets;

namespace WindowsFormsApp1
{

    public partial class Form1 : Form
    {
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
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //РАСЧИТАТЬ  письку



        }
    }
}
