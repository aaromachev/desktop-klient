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

        public void open(object obj)
        {
            Application.Run(new Settings());    
        }

    }
}
