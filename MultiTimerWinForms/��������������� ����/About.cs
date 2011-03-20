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
    public partial class About : Form
    {
        public float ProgramVersion = 1.3F;
        
        public About()
        {
            InitializeComponent();

            SystemSettings SysSet = new SystemSettings();
            if (SysSet.Lang == SystemSettings.TypeLanguage.RUSSIAN)
            {
                label2.Text = "Версия программы  " + ProgramVersion.ToString();
            }
            else if (SysSet.Lang == SystemSettings.TypeLanguage.ENGLISH)
            {
                this.Text = "About program";
                label1.Text = "2009-2010. Novatek-Electro ltd.";
                label2.Text = "Program version  " + ProgramVersion.ToString();
                label3.Text = "All rights reserved.";
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
