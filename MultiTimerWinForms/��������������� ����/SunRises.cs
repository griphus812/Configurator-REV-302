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
    public partial class SunRises : Form
    {
        public TimerClass ListEvents = new TimerClass();        // хранится сгенерированный список событий
        private int CountItemsInListEvents = 0;
        private DateTime CalculateTime;
        //private DateTime UserOffset;
        //private bool DirectUserOffset;          // if "true", then need add to calculate time else UserOffset, if "false" then substract
        private bool ContactCondition;
        private double day;
        private double month;
        private double year;
        private double latitude;    // широта
        private double longitude;  // долгота
        private double localOffset;     // местный сдвиг времени с учетом летнего времени
        private double zenith;
        private bool WhatDesired;       // если true, то необходимо расчитать восход солнца, если false, то закат

        private SystemSettings SysSet = new SystemSettings();
        
        
        public SunRises()
        {
            InitializeComponent();


            if (SysSet.Lang == SystemSettings.TypeLanguage.ENGLISH)
            {
                this.Text = "Sunrises and Sunsets";
                groupBox2.Text = "Location geographic coordinates";
                groupBox4.Text = "Latitude:";
                label6.Text = "Degrees:";
                label5.Text = "Minutes:";
                label4.Text = "Seconds:";
                groupBox3.Text = "Longitude:";
                label1.Text = "Degrees:";
                label2.Text = "Minutes:";
                label3.Text = "Seconds:";
                comboBox2.Items[0] = "Northern latitude";
                comboBox2.Items[1] = "Southern latitude";
                comboBox1.Items[0] = "West longitude";
                comboBox1.Items[1] = "East longitude";
                groupBox5.Text = "Difference with Greenwich";
                checkBox4.Text = "Recognize summer time conversion";
                groupBox1.Text = "List events Sunrise/Sunset setup:";
                label13.Text = "List type:";
                comboBox4.Items[0] = "Sunrises";
                comboBox4.Items[1] = "Sunsets";
                label19.Text = "Day/Night boundary:";
                comboBox8.Items[0] = "Official Sun's zenith";
                comboBox8.Items[1] = "Civil Sun's zenith";
                comboBox8.Items[2] = "Nautical Sun's zenith";
                comboBox8.Items[3] = "Astronomical Sun's zenith";
                label8.Text = "First day:";
                label9.Text = "Last day:";
                label10.Text = "Time shift:";
                comboBox3.Items[0] = "befor Sunrise/Sunset";
                comboBox3.Items[1] = "after Sunrise/Sunset";
                label11.Text = "on";
                label12.Text = "Contacts after event occurrence:";
                comboBox5.Items[0] = "Close";
                comboBox5.Items[1] = "Open";
                checkBox3.Text = "Preliminary erase the existing list";
                button1.Text = "Create array";
                button2.Text = "Cancel";
            }            
            
            Init();
        }

        private void Init()
        {
            this.comboBox1.SelectedIndex = 0;
            this.comboBox2.SelectedIndex = 0;
            this.comboBox3.SelectedIndex = 0;
            this.comboBox4.SelectedIndex = 0;
            this.comboBox5.SelectedIndex = 0;            
            this.comboBox8.SelectedIndex = 0;            
        }

        // создание списка событий
        private void button1_Click(object sender, EventArgs e)
        {
            // данные для обнаружения совпадений
            TimerClass WorkListEvents = new TimerClass();
            foreach (TimerClass TC in ListEvents)        // сохранение списка для некоторых случаев
                WorkListEvents.Add(new TimerClass(TC.DateAndTime, TC.Condition));
            bool WasOverwrite = false;      // указывает произошла ли перезапись данных в списке
            int CountElemBefore = 0;       // временно хранит число элементов в списке перед добавлением нового
                        
            
            
            // подготовка общих данных одинаковых для всего списка                        
            latitude = (double)numericUpDown6.Value + ((double)numericUpDown5.Value / 60) + ((double)numericUpDown4.Value / 3600);                       
            if (comboBox2.SelectedIndex == 1)
                latitude = -latitude;

            longitude = (double)numericUpDown1.Value + ((double)numericUpDown2.Value / 60) + ((double)numericUpDown3.Value / 3600);
            if (comboBox1.SelectedIndex == 1)
                longitude = -longitude;

            localOffset = (double)numericUpDown7.Value;

            if( comboBox4.SelectedIndex == 0 )
                WhatDesired = true;
            else
                WhatDesired = false;

            switch( comboBox8.SelectedIndex)
            {
                case 0:
                    zenith = 90.83333;
                    break;
                case 1:
                    zenith = 96;
                    break;
                case 2:
                    zenith = 102;
                    break;
                case 3:
                    zenith = 108;
                    break;
                default:
                    zenith = 90.83333;
                    break;
            }

            if (comboBox5.SelectedIndex == 0)
                ContactCondition = true;
            else
                ContactCondition = false;

            if(checkBox3.Checked == true)
            {
                // необходимо предварительно очистить списко для сохранения расчитанных событий
                ListEvents.Clear();
                CountItemsInListEvents = -1;        // пока ни одного элемента не существует
            }
       
            if( DateTime.Compare(dateTimePicker1.Value, dateTimePicker2.Value) > 0 )
            {
                // первый день больше последнего - ошибка, позволить исправить
                if (SysSet.Lang == SystemSettings.TypeLanguage.RUSSIAN)
                {
                    MessageBox.Show("Последние сутки в списке предшествуют первым!\nРасчет невозможен.",
                                                "Внимание!", MessageBoxButtons.OK,
                                                MessageBoxIcon.Exclamation);
                }
                else if( SysSet.Lang == SystemSettings.TypeLanguage.ENGLISH )
                {
                    MessageBox.Show("Last day in list precede first day!\nCalculation is not possible.",
                                                "Attention!", MessageBoxButtons.OK,
                                                MessageBoxIcon.Exclamation);
                }
            }
            else if ((latitude > 90) || (latitude < -90) || (longitude > 180) || (longitude < -180))
            {
                // неправильные координаты
                if (SysSet.Lang == SystemSettings.TypeLanguage.RUSSIAN)
                {
                    MessageBox.Show("Недопустимые координаты местности!\nРасчет невозможен.",
                                            "Внимание!", MessageBoxButtons.OK,
                                            MessageBoxIcon.Exclamation);
                }
                else if (SysSet.Lang == SystemSettings.TypeLanguage.ENGLISH)
                {
                    MessageBox.Show("Illegal location geographic coordinates!\nCalculation is not possible.",
                                            "Attention!", MessageBoxButtons.OK,
                                            MessageBoxIcon.Exclamation);
                }
            }
            else
            {
                CalculateTime = dateTimePicker1.Value.AddDays(-1);
                while (CalculateTime != dateTimePicker2.Value)
                {
                    CalculateTime = CalculateTime.AddDays(1);

                    day = CalculateTime.Day;
                    month = CalculateTime.Month;
                    year = CalculateTime.Year;

                    // сам расчет
                    //1. first calculate the day of the year
                    double N1 = Math.Floor(275 * month / 9);
                    double N2 = Math.Floor((month + 9) / 12);
                    double N3 = (1 + Math.Floor((year - 4 * Math.Floor(year / 4) + 2) / 3));
                    double N = N1 - (N2 * N3) + day - 30;

                    //2. convert the longitude to hour value and calculate an approximate time
                    double lngHour = longitude / 15;
                    double t;
                    if (WhatDesired == true)
                        t = N + ((6 - lngHour) / 24);
                    else
                        t = N + ((18 - lngHour) / 24);

                    //3. calculate the Sun's mean anomaly
                    double M = (0.9856 * t) - 3.289;

                    //4. calculate the Sun's true longitude
                    double L = M + (1.916 * Math.Sin(M * Math.PI / 180)) + (0.020 * Math.Sin(2 * M * Math.PI / 180)) + 282.634;
                    while (L >= 360)
                        L -= 360;
                    while (L < 0)
                        L += 360;

                    //5a. calculate the Sun's right ascension
                    double RA = Math.Atan(0.91764 * Math.Tan(L * Math.PI / 180)) * 180 / Math.PI;
                    while (RA >= 360)
                        RA -= 360;
                    while (RA < 0)
                        RA += 360;

                    //5b. right ascension value needs to be in the same quadrant as L
                    double Lquadrant = Math.Floor(L / 90) * 90;
                    double RAquadrant = Math.Floor(RA / 90) * 90;
                    RA = RA + (Lquadrant - RAquadrant);

                    //5c. right ascension value needs to be converted into hours
                    RA = RA / 15;

                    //6. calculate the Sun's declination
                    double sinDec = 0.39782 * Math.Sin(L * Math.PI / 180);
                    double cosDec = Math.Cos(Math.Asin(sinDec));

                    //7a. calculate the Sun's local hour angle
                    double cosH = (Math.Cos(zenith * Math.PI / 180) - (sinDec * Math.Sin(latitude * Math.PI / 180))) / (cosDec * Math.Cos(latitude * Math.PI / 180));
                    if ((cosH > 1) || (cosH < -1))
                    {
                        // солнце в этот день никогда не взойдет и не опуститься
                        continue;
                    }

                    //7b. finish calculating H and convert into hours
                    double H;
                    if (WhatDesired == true)
                        H = 360 - (Math.Acos(cosH) * 180 / Math.PI);
                    else
                        H = Math.Acos(cosH) * 180 / Math.PI;
                    H = H / 15;

                    //8. calculate local mean time of rising/setting
                    double T = H + RA - (0.06571 * t) - 6.622;

                    //9. adjust back to UTC
                    double UT = T - lngHour;
                    while (UT >= 24)
                        UT -= 24;
                    while (UT < 0)
                        UT += 24;

                    //10. convert UT value to local time zone of latitude/longitude
                    double localT = UT + localOffset;
                    while (localT >= 24)
                        localT -= 24;
                    while (localT < 0)
                        localT += 24;
                    double hour = Math.Floor(localT);
                    double minute = (localT - hour) * 60;
                    double second = (minute - Math.Floor(minute)) * 60;
                    DateTime d1 = new DateTime(CalculateTime.Year, CalculateTime.Month, CalculateTime.Day, (int)hour, (int)minute, (int)second);
                    if (checkBox4.Checked == true)
                    {
                        // если необходимо учесть летнее время
                        // проверка того, что указанная дата попадает в зону летнего времени
                        if (CheckDayOnSummerTime(d1) == true)
                        {
                            d1 = d1.AddHours(1);
                        }
                    }

                    int userOffsetSecond = dateTimePicker5.Value.Hour * 3600 + dateTimePicker5.Value.Minute * 60 + dateTimePicker5.Value.Second;
                    if (comboBox3.SelectedIndex == 0)
                    {
                        d1 = d1.AddSeconds(-userOffsetSecond);
                    }
                    else
                    {
                        d1 = d1.AddSeconds(userOffsetSecond);
                    }
                    CountItemsInListEvents++;       // следующий

                    CountElemBefore = ListEvents.Count;
                    ListEvents.AddSmart(new TimerClass(new DateTime(1996, d1.Month, d1.Day, d1.Hour, d1.Minute, d1.Second), ContactCondition), 2);
                    if (CountElemBefore == ListEvents.Count)
                        WasOverwrite = true;        // произошла запись элемента поверх старого


                    //MessageBox.Show(((int)hour).ToString() + ":" + ((int)minute).ToString() + ":" + ((int)second).ToString());

                }

                // тестовый вывод результатов
                //string str = "";
                //foreach (TimerClass Event in ListEvents)
                //    str = str + Event.DateAndTime.ToString() + "\n";                
                //MessageBox.Show(str);

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
        }

        private bool CheckDayOnSummerTime(DateTime d1)
        {
            if ((d1.Month > 3) && (d1.Month < 10))
            {
                return true;
            }
            else if (d1.Month == 3)
            {
                if (d1.DayOfWeek == DayOfWeek.Sunday)
                {
                    if (d1.AddDays(7).Month != 3)
                    {
                        if (d1.Hour >= 3)
                            return true;
                        else
                            return false;
                    }
                    else
                        return false;
                }
                else
                {
                    while (d1.Month == 3)
                    {                        
                        if (d1.DayOfWeek == DayOfWeek.Sunday)
                            return false;
                        d1 = d1.AddDays(1);
                    }
                    return true;
                }
            }
            else if (d1.Month == 10)
            {
                if (d1.DayOfWeek == DayOfWeek.Sunday)
                {
                    if (d1.AddDays(7).Month != 10)
                    {
                        if (d1.Hour <= 2)
                            return true;
                        else
                            return false;
                    }
                    else
                        return true;
                }
                else
                {
                    while (d1.Month == 10)
                    {                        
                        if (d1.DayOfWeek == DayOfWeek.Sunday)
                            return true;
                        d1 = d1.AddDays(1);
                    }
                    return false;
                }
            }
            return false;
        }

        // создание списка событий восходов
        private void CreateSunRises()
        {
               
        }

        // создание списков событий закатов
        private void CreateSunSets()
        {

        }



        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }        
    }
}
