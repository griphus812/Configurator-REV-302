using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace MultiTimerWinForms
{
    public partial class ArrayEvents : Form
    {
        public TimerClass ListEvents = new TimerClass();        // хранится сгенерированный список событий
        public int TypeOfList;

        SystemSettings SysSet = new SystemSettings();

        public ArrayEvents()
        {
            InitializeComponent();
                        
            if (SysSet.Lang == SystemSettings.TypeLanguage.ENGLISH)
            {
                this.Text = "Events array wizard";
                groupBox1.Text = "First event setup";
                label12.Text = "Contacts:";
                comboBox5.Items[0] = "Close";
                comboBox5.Items[1] = "Open";
                groupBox2.Text = "Array:";
                label4.Text = "Events general quantity:";
                label2.Text = "Step:";
                label3.Text = "days";
                label1.Text = "Contacts:";
                comboBox1.Items[0] = "like the first event";
                comboBox1.Items[1] = "alternate";
                checkBox3.Text = "Preliminary erase the existing list";
                button1.Text = "Create array";
                button2.Text = "Cancel";
            }              
            Init();
        }

        private void Init()
        {
            comboBox5.SelectedIndex = 0;
            comboBox1.SelectedIndex = 0;
        }

        public void ChangeShowElements(int NewTypeList)
        {
            TypeOfList = NewTypeList;

            dateTimePicker1.Visible = true;
            numericUpDown1.Visible = true;
            label3.Visible = true;

            dateTimePicker5.Visible = true;
            label12.Visible = true;
            comboBox5.Visible = true;
            dateTimePicker2.Visible = true;
            label1.Visible = true;
            comboBox1.Visible = true;


            if (SysSet.Lang == SystemSettings.TypeLanguage.RUSSIAN)
            {
                this.Text = "Создание массива событий";
                groupBox1.Text = "Настройка первого события:";
                label4.Text = "Общее кол-во событий:";
            }
            else if (SysSet.Lang == SystemSettings.TypeLanguage.ENGLISH)
            {
                this.Text = "Events array wizard";
                groupBox1.Text = "First event setup:";
                label4.Text = "Events general quantity:";
            }

            switch (NewTypeList)
            {
                case 0:
                    // праздники
                    dateTimePicker5.Visible = false;
                    label12.Visible = false;
                    comboBox5.Visible = false;
                    dateTimePicker2.Visible = false;
                    label1.Visible = false;
                    comboBox1.Visible = false;

                    dateTimePicker5.Value = new DateTime(1996, 1, 1, 0, 0, 0);
                    dateTimePicker2.Value = new DateTime(1996, 1, 1, 0, 0, 0);

                    if (SysSet.Lang == SystemSettings.TypeLanguage.RUSSIAN)
                    {
                        this.Text = "Создание массива праздников";
                        groupBox1.Text = "Настройка первого праздника:";
                        label4.Text = "Общее кол-во праздников:";
                    }
                    else if (SysSet.Lang == SystemSettings.TypeLanguage.ENGLISH)
                    {
                        this.Text = "Holidays array wizard";
                        groupBox1.Text = "First holiday setup:";
                        label4.Text = "Holidays general quantity:";
                    }

                    numericUpDown1.Maximum = 365;
                    break;
                case 1:
                    // исключ. соб.
                    dateTimePicker1.Visible = false;
                    numericUpDown1.Visible = false;
                    label3.Visible = false;

                    numericUpDown1.Value = 0;
                    break;
                case 2:
                    // год. соб.
                    numericUpDown1.Maximum = 365;
                    break;
                case 3:
                    // мес. соб.
                    numericUpDown1.Maximum = 30;
                    break;
                case 4:
                    // нед. соб.
                    numericUpDown1.Maximum = 6;
                    break;
                case 5:
                    // сут. соб.
                    dateTimePicker1.Visible = false;
                    numericUpDown1.Visible = false;
                    label3.Visible = false;

                    numericUpDown1.Value = 0;
                    break;
            }
        }

        // создание массива
        private void button1_Click_1(object sender, EventArgs e)
        {
            TimerClass WorkListEvents = new TimerClass();
            foreach(TimerClass TC in ListEvents)        // сохранение списка для некоторых случаев
                WorkListEvents.Add(new TimerClass(TC.DateAndTime, TC.Condition));
            bool WasOverwrite = false;      // указывает произошла ли перезапись данных в списке
            int CountElemBefore = 0;       // временно хранит число элементов в списке перед добавлением нового
            
            if (checkBox3.Checked == true)
            {
                // необходимо предварительно очистить список для сохранения расчитанных событий
                ListEvents.Clear();                
            }

            bool ContactCondition;
            if( comboBox5.SelectedIndex == 0 )
                ContactCondition = true;
            else
                ContactCondition = false;

            CountElemBefore = ListEvents.Count;
            DateTime d1 = new DateTime(dateTimePicker1.Value.Year, dateTimePicker1.Value.Month, dateTimePicker1.Value.Day, dateTimePicker5.Value.Hour, dateTimePicker5.Value.Minute, dateTimePicker5.Value.Second);
            if(TypeOfList == 4) // если нед. соб.
                ListEvents.AddSmart(new TimerClass(new DateTime(1996, 1, ConvertDayOfWeekInInt(d1.DayOfWeek), d1.Hour, d1.Minute, d1.Second), ContactCondition), TypeOfList);            
            else
                ListEvents.AddSmart(new TimerClass(new DateTime(1996, d1.Month, d1.Day, d1.Hour, d1.Minute, d1.Second), ContactCondition), TypeOfList);
            if (CountElemBefore == ListEvents.Count)
                WasOverwrite = true;        // произошла запись элемента поверх старого
            
            for (int i = 1; i < (int)numericUpDown2.Value; i++)
            {
                if (comboBox1.SelectedIndex == 0)
                {
                }
                else if (comboBox1.SelectedIndex == 1)
                {
                    ContactCondition = !ContactCondition;
                }

                d1 = d1.AddDays((int)numericUpDown1.Value);
                d1 = d1.AddHours(dateTimePicker2.Value.Hour);
                d1 = d1.AddMinutes(dateTimePicker2.Value.Minute);
                d1 = d1.AddSeconds(dateTimePicker2.Value.Second);

                CountElemBefore = ListEvents.Count;
                if (TypeOfList == 4) // если нед. соб.
                    ListEvents.AddSmart(new TimerClass(new DateTime(1996, 1, ConvertDayOfWeekInInt(d1.DayOfWeek), d1.Hour, d1.Minute, d1.Second), ContactCondition), TypeOfList);
                else
                    ListEvents.AddSmart(new TimerClass(new DateTime(1996, d1.Month, d1.Day, d1.Hour, d1.Minute, d1.Second), ContactCondition), TypeOfList);
                if (CountElemBefore == ListEvents.Count)
                    WasOverwrite = true;        // произошла запись элемента поверх старого
            }

            if ((checkBox3.Checked != true) && (WasOverwrite == true))
            {
                if (SysSet.Lang == SystemSettings.TypeLanguage.RUSSIAN)
                {
                    if (MessageBox.Show("Некоторые новые события списка совпадают по времени со старыми.\n\nЗаменить старые события новыми?",
                                            "Внимание!", MessageBoxButtons.OKCancel,
                                            MessageBoxIcon.Question) == DialogResult.Cancel)
                    {
                        ListEvents = WorkListEvents;
                    }
                    else
                    {
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                }
                else if (SysSet.Lang == SystemSettings.TypeLanguage.ENGLISH)
                {
                    if (MessageBox.Show("Some new elements of list concured with old.\n\nReplace old elements by new?",
                                            "Attention!", MessageBoxButtons.OKCancel,
                                            MessageBoxIcon.Question) == DialogResult.Cancel)
                    {
                        ListEvents = WorkListEvents;
                    }
                    else
                    {
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                }
            }
            else
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }            
        }

        private int ConvertDayOfWeekInInt(DayOfWeek inday)
        {
            switch (inday)
            {
                case DayOfWeek.Monday: return 1;
                case DayOfWeek.Tuesday: return 2;
                case DayOfWeek.Wednesday: return 3;
                case DayOfWeek.Thursday: return 4;
                case DayOfWeek.Friday: return 5;
                case DayOfWeek.Saturday: return 6;
                case DayOfWeek.Sunday: return 7;
            }
            return 1;
        }
    }
}
