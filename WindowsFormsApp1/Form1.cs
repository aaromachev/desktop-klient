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

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        Thread TH;
        public Form1()
        {
            InitializeComponent();
        }

        private void сохранитьКакToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            //Файл/Сохранить как
        }

        private void сохранитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Файл/Сохранить

        }
        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            //Правка/Копировать
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            //Правка/Вставить
        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            //Правка/Вырезать
        }

        private void toolStripMenuItem4_Click_1(object sender, EventArgs e)
        {
            //Вид/Шрифт
        }

        private void настройкиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Настройки
            //this.Close();
            TH = new Thread(open);
            TH.SetApartmentState(ApartmentState.STA);
            TH.Start();
        }
        private void toolStripComboBox1_Click(object sender, EventArgs e)
        {
            //Прогноз/Создать
        }

        private void toolStripMenuItem4_Click(object sender, EventArgs e)
        {
            //Прогноз/История
        }

        private void toolStripComboBox1_Click_1(object sender, EventArgs e)
        {
            //Прогноз/Поделиться
        }

        public void open(object obj)
        {
            Application.Run(new Settings());    
        }
    }
}
