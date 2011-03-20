using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
//using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MultiTimerWinForms
{
    public partial class ConnectionDialog1 : Form
    {
        public ConnectionDialog1()
        {
            InitializeComponent();

            Init();     // начальные задания параметров
        }

        public int ProcBar1
        {
            set
            {
                this.progressBar1.Value = value;
            }
            get
            {
                return this.progressBar1.Value;
            }
        }

        // инициализация окна соединения по USB
        private void Init()
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();            
        }
    }
}
