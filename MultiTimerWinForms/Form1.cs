using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
//using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UsbLibrary;
using System.Diagnostics;

namespace MultiTimerWinForms
{

    public partial class Form1 : Form
    {
        
        // системные переменные
        public bool FactoryProgramType = false;          // указывает включить код для производственной отладки устройства        
        //public bool FactoryProgramType = true;
        public int LangGlobal = 1;      // глобальный выбор языка для всей программы
        // 0 - Russian
        // 1 - English
        

        // основной код
        
        public Form1()
        {
            InitializeComponent();

            SystemSettings SysSet = new SystemSettings();
            switch (SysSet.Lang)
            {
                case SystemSettings.TypeLanguage.RUSSIAN:
                    LangGlobal = 0;
                    break;
                case SystemSettings.TypeLanguage.ENGLISH:
                    LangGlobal = 1;
                    break;
            }

            Init();     // инициализация при запуске программы                        
        }

        //private int CountByteData;      // подсчет текущего принимаемого или передаваемого байта

        //private TreeNode mySelectedNode;            // текущий выделенный элемент
        private int CtrlProgsMax = 8;               // общее кол-во управляющих программ
        
        int ProgSelected = 0;           // текущая активная программ для редактирования
        int ChannelSelected = 0;        // текущий выбранный канал для редактирования
        //private int tmp1;             
        string PathOfFile;      // содержит путь к последнему загруженному или сохраненному файлу

        // подписи пунктов меню
        private string StRelVrem = "Реле времени ";
        private string StRelVremOptions = "Общие настройки реле времени ";
        private string StExceptions = "Исключения ";
        private string StHolidays = "Праздники ";
        private string StWeekEnds = "Выходные дни";
        private string StListEventExceptions = "Список исключительных событий ";
        private string StListEventYear = "События годового реле ";
        private string StListEventMonth = "События месячного реле ";
        private string StListEventWeek = "События недельного реле ";
        private string StListEventDay = "События суточного реле ";
        private string StImpulseOptions = "Настройки импульсного реле";
        private string StSimpleOptions = "Настройки простого реле";
        private string StVoltRelayOptions = "Реле напряжения ";
        private string StPhotoRelayOptions = "Фотореле ";
        private string StDeviceOptions = "Общие настройки устройства";
        private string StTimeCorrect = "Установка времени";
        private string StVoltBrightCorrect = "Коррекция напряжения и освещенности";

        private string[] StDaysOfWeek = {"Понедельник","Вторник","Среда","Четверг","Пятница","Суббота","Воскресенье",};

        //====== переменные подписей для русской и английской версий ================================
        private string stMon = "Monday";
        private string stTue = "Tuesday";
        private string stWed = "Wednesday";
        private string stThu = "Thursday";
        private string stFri = "Friday";
        private string stSat = "Saturday";
        private string stSun = "Sunday";

        private string[] stInfo = {"Информация", "Information"};
        private string[] stAbsent = {"отсутствует", "is absent"};
        private string[] stChannel1 = {"Kaнал К1", "Channel К1"};
        private string[] stChannel2 = { "Канал К2", "Channel K2" };
        private string[] stProgram = {"Программа П", "Program P"};
        private string[] stUSBnoCon = {"USB: Связь с реле отсутствует", "USB: No connection"};
        private string[] stEvents_t1 = { " событий)", " events)" };
        private string[] stInTreeOff = { "(откл)", "(OFF)" };
        private string[] stInTreeYear = { "(год)", "(year)" };
        private string[] stInTreeMonth = { "(мес)", "(month)" };
        private string[] stInTreeWeek = { "(нед)", "(week)" };
        private string[] stInTreeDay = { "(сут)", "(day)" };
        private string[] stInTreePulse = { "(имп)", "(pulse)" };
        private string[] stInTreeSimple = { "(простое)", "(simple)" };        
        private string[] stInTreeON = { "(вкл)", "(ON)" };
        private string[] stInTreeChannelN = { "Канал К", "Channel K" };
        private string[] stInTreeOffFull = {" (отключить)", " (OFF)"};
        private string[] stInTreeP = { "П", "P" };

        private string[] stVoltAndPhoto = {"Напряжение и освещенность", "Voltage and Illumination"};
        private string[] stTimeSetting = { "Установка времени", "Time Setting" };
        private string[] stGeneralSettings = { "Общие настройки устройства", "General Settings" };
        private string[] stPhotoRelay = { "Фото реле", "Photorelay" };
        private string[] stRV = { "Реле напряжения", "Voltage Relay" };
        private string[] stSimpleRT = {"Простое реле времени", "Simple Time Relay"};
        private string[] stPulseRT = { "Импульсное реле времени", "Pulse Time Relay" };
        private string[] stEventsDailyRT = { "События суточного реле времени", "Daily Time Relay Events" };
        private string[] stEventsWeeklyRT = { "События недельного реле времени", "Weekly Time Relay Events" };
        private string[] stEventsMonthlyRT = { "События месячного реле времени", "Monthly Time Relay Events" };
        private string[] stEventsYearlyRT = { "События годового реле времени", "Yearly Time Relay Events" };
        private string[] stExceptionEventsRT = { "Исключительные события", "Exception Events" };
        private string[] stDaysOffRT = { "Выходные дни", "Days Off" };
        private string[] stHolidaysRT = { "Праздники", "Holidays" };
        private string[] stExceptionsRT = { "Исключения", "Exceptions" };
        private string[] stRT = { "Реле времени", "Time Relay" };
        private string[] stGeneralOptionsRT = { "Общие настройки реле времени", "Time Relay General Settings" };
        private string[] stChooseModeRT = { "Выберите режим работы реле времени программы П", "Choose operation mode of time relay P" };
        private string[] stChooseProgram = { "Выберите программу для управления контактами канала К", "Choose program for managment contacts of channel K" };
        private string[] stClose = { "Включить", "Close" };
        private string[] stOpen = { "Отключить", "Open" };
        //private string[] stNovatek = { "Novatek-Electro - Конфигуратор реле РЭВ-302", "Novatek-Electro - Configurator relay REV-302" };
        private string[] stNovatek = { "Конфигуратор реле РЭВ-302", "Configurator relay REV-302" };
        private string[] stNoAccessToDisk = { "Нет доступа к диску. Файл не сохранен.", "Not access to disk. The file has not saved" };
        private string[] stAttention = { "Внимание!", "Attention!" };
        private string[] stNotGoogFile = { "Файл не содежит информацию\nо настройках или устарел.", "The file does not have information\nabout setting or obsolete." };
        private string[] stFileNotFind = { "Файл не найден", "The file has not found." };
        private string[] stFileReset = { "Все текущие настройки будут сброшены.", "All setting will be reset!" };
        private string[] stSettingSuccessRead = { "Настройки РЭВ-302 успешно считаны.", "Settings has been read successfully" };
        private string[] stReceivingData = { "Прием данных", "Receiving data" };
        private string[] stTransferData = { "Передача данных", "Transfer data" };
        private string[] stSettingSuccessTransfer = { "Настройки успешно записаны в РЭВ-302.\n\nДля выхода из меню настроек\nи перевода реле в рабочее\nсостояние нажмите кнопку \"Влево\"\nна лицевой панели РЭВ-302.", 
                                                      "Settings has been transfer successfully into REV-302.\n\nFor exit from Setting Menu\nand conversion relay in operation\nstate press button \"Left\"\n on REV-302 face panel." };
        private string[] stOverMemory1 = { "  В памяти подключенного реле \nможно сохранить не больше", 
                                           "In the memory of connected relay may be saved only" };
        private string[] stOverMemory2 = { " событий.\nПрограмма содержит ", 
                                           " events.\nProgram contain " };
        private string[] stOverMemory3 = { " событий.\nПопробуйте отредактировать \nсписки и повторить запись.", 
                                           " events.\nTry to reduct lists and repeat settings transfer" };
        private string[] stSaveSettings = { "Сохранение настроек в памяти реле", "Save settings in relay memory" };
        private string[] stVersionDeviceNotSupport_1 = { " Версия программы устройства v", " Program version of device v" };
        //private string[] stVersionDeviceNotSupport_2 = { " не поддеживается. \nУстановите последнюю версию программного обеспечения \nс сайта: www.novatek-electro.com.", 
        //                                                 " does not suppoted. \nInstall last version of software \n from: www.novatek-electro.com" };
        private string[] stVersionDeviceNotSupport_2 = { " не поддеживается. \nУстановите последнюю версию программного обеспечения.", 
                                                         " does not suppoted. \nInstall last version of software." };
        private string[] stVersionDevice = { " Версия программы устройства: v", " Program version of device: v" };
        private string[] stREV302 = { "РЭВ-302", "REV-302" };
        private string[] stNewTimeSend = { " В реле успешно записаны новые \nзначения времени и даты.", "New time and date have been written\nin relay successfully." };
        private string[] stSetTime = { "Установка времени", "Time setting" };
        private string[] stUSBconnectIs = { "USB: Установлена связь с реле", "USB: Connection with relay established" };
        private string[] stDeviceNotConnect = { "Устройство не подключено. \n Проверь питание и целостность кабеля USB. \n Затем повторите попытку считать данные.",
                                                "Connection with device is not existing.\n Check power and cable integrity USB. \n Аfter try read data again." };
        private string[] stUSBconnectionNotIs = { "USB: Связь с реле отсутствует", "USB: Connection is not existing" };
        private string[] stConnectError = { "Произошел сбой связи с реле. \n Проверьте надежность подключения и \n повторите попытку.",
                                            "Connection has malfunctioned. \n Check cable solidity and try attempt again." };
        private string[] stCopy1 = { "   Внимание! Настройки программы П{0} будут \nзаменены настройками программы П{1}.",
                                     "   Attention! Settings of program P{0} will be replaced by settings of program P{1}" };
        private string[] stCopy2 = { "Копирование настроек управляющих программ", 
                                     "Copy settings of programs control" };
        private string[] stCopy3 = { "   Все настройки программы П{0} были успешно \nскопированы в программу П{1}.", 
                                     "   All settings of program P{0} have been copied successfully in program P{1}." };
        private string[] stClearLists1 = {" Вы действительно хотите очистить все списки Программы П{0}?",
                                          " Do you really wich to clear all program P{0} lists?"};
        private string[] stClearLists2 = {"Очистка списков",
                                          "Clearing lists"};
        private string[] stSaveSettingsInFile = {"Сохранить настройки в файл?",
                                                 "Save settings in file?"};
        private string[] stExitProgram = {"Выход из программы", "Exit"};
        private string[] stResetProgramSettings1 = {" Вы действительно хотите сбросить все настройки Программы П{0}?",
                                          " Do you really wish to reset all settings of program P{0}?"};
        private string[] stResetProgramSettings2 = {"Сброс настроек",
                                                    "Reset settings"};


        //label4.Text = stInTreeP[LangGlobal] + ProgSelected.ToString() + ":  " + stEventsDailyRT[LangGlobal];

        //======================================              
        
        private string StMessageUSB;        // сообщение выводится в нижней строке окна

        private DeviceOptionsClass DeviceOptions = new DeviceOptionsClass();   // создание объекта класса для хранения и обмена данными с устройством
        private CtrlProgramOptionsClass CtrlProgramsOptions = new CtrlProgramOptionsClass();     // определение коллекции настроек управляющих программ
        ConnectionClass Connection = new ConnectionClass();     // создание глобального объекта обработки приема и передачи данных
        ConnectionDialog1 dlgConnect = new ConnectionDialog1();     // диалоговое окно приема-передачи данных
        SunRises dlgSunRise = new SunRises();                       // диалогое окно восхода/заката Солнца
        private ArrayEvents dlgArEv = new ArrayEvents();

        private CtrlProgramOptionsClass BackCtrlProgramsOptions = new CtrlProgramOptionsClass();        // коллекция для отмены последних изменений в настройках программ
        
        private CtrlProgramOptionsClass InRelayCtrlProgramsOptions = new CtrlProgramOptionsClass();       // последние настройки сохраненные или считанные из устройства
        private DeviceOptionsClass InRelayDeviceOptions = new DeviceOptionsClass();     // последние настройки сохраненные или считанные из устройства
                
        
        // организация последовательности передачи данных
        private enum USB_GetSequenceEnum
        {
            NO,                 // никакие запросы не инициализированы, или по умолчанию
            GET_OPTIONS,         // требование выслать настройки
            SEND_OPTIONS            // пересылка найстроек с проверкой версии программы
        };
        private USB_GetSequenceEnum USB_GetSequence = USB_GetSequenceEnum.NO;
        private int USB_GetSeqStep = 0;                 // шаг выполнения последовательности приема

        private bool USB_Connect = false;           // указывает подключено ли устройство к USB

        private DateTime DeviceTimeMonitor;                 // ослеживается текущее время устройства
        private bool DeviceTimeMonitorExist = false;        // указывает на то, что время устройства было считано        
        private DateTime SychTime;          // используется для синхронизации времени устройства и компьютера        
        private bool WriteNewTimeInDevice;  // синхронизировано записать текущее время ПК в устройство
        private bool SychShowTime;          // синхронизировать отображаемое время с временем ПК


        private void LanguageInit()
        {
            if(LangGlobal == 1)
            {            
                // перевод статических элементов
                this.Text = stNovatek[LangGlobal];
                radioButton15.Text = "OFF";
                radioButton1.Text = "Program P1";
                radioButton2.Text = "Program P2";
                radioButton3.Text = "Program P3";
                radioButton4.Text = "Program P4";
                radioButton5.Text = "Program P5";
                radioButton6.Text = "Program P6";
                radioButton7.Text = "Program P7";
                radioButton8.Text = "Program P8";
                groupBox2.Text = "Choose operation mode of time relay";
                radioButton16.Text = "OFF";
                radioButton9.Text = "Yearly timer";
                radioButton10.Text = "Monthly timer";
                radioButton11.Text = "Weekly timer";
                radioButton12.Text = "Daily timer";
                radioButton13.Text = "Pulse timer";
                radioButton14.Text = "Simple timer";
                groupBox3.Text = "Additional settings";
                checkBox1.Text = "Recognize days off";
                checkBox2.Text = "Recognize holidays";
                checkBox3.Text = "Repeat cycle";
                richTextBox1.Text = "    To control channel contacts, in the channel K1 or K2 setup menu select one of 8 programs according to which the contacts will be switched. Then go to settings of the program selected and activate the time relay, voltage relay and/or photo relay.\n\n    Use this window to copy other programs settings into current program.";
                groupBox40.Text = "Choose program for copy it settings:";
                button39.Text = "Copy into current program";
                button42.Text = "Clear lists";
                button49.Text = "Reset settings";
                groupBox4.Text = "Choose days off in week:";
                checkBox4.Text = stMon;
                checkBox5.Text = stTue;
                checkBox6.Text = stWed;
                checkBox7.Text = stThu;
                checkBox8.Text = stFri;
                checkBox9.Text = stSat;
                checkBox10.Text = stSun;
                groupBox5.Text = "List of daily events:";
                listView1.Columns[1].Text = "Day";
                listView1.Columns[2].Text = "Month";
                button2.Text = "Add";
                button3.Text = "Delete selected";
                button4.Text = "Clear list";
                button34.Text = "Array wizard";
                button43.Text = "Undo last action";
                
                groupBox6.Text = "List of exception events:";
                listView2.Columns[1].Text = "Time";
                listView2.Columns[2].Text = "Contacts";
                label6.Text = "Hours";
                label7.Text = "Minutes";
                label8.Text = "Seconds";
                groupBox7.Text = "Condition channel contacts:";
                radioButton17.Text = "Open";
                radioButton18.Text = "Close";
                button7.Text = "Add";
                button6.Text = "Delete selected";
                button5.Text = "Clear list";
                button36.Text = "Array wizard";
                button44.Text = "Undo last action";
                groupBox8.Text = "Events list of yearly time relay:";
                listView3.Columns[1].Text = "Day";
                listView3.Columns[2].Text = "Month";
                listView3.Columns[3].Text = "Time";
                listView3.Columns[4].Text = "Contacts";
                label15.Text = "Hours";
                label14.Text = "Minutes";
                label13.Text = "Seconds";
                groupBox9.Text = "Condition channel contacts:";
                radioButton20.Text = "Open";
                radioButton19.Text = "Close";
                button10.Text = "Add";
                button9.Text = "Delete selected";
                button8.Text = "Clear list";
                button24.Text = "Array wizard";
                button45.Text = "Undo last action";
                button23.Text = "List of Sunrise/Sunset wizard";
                groupBox10.Text = "Events list of monthly time relay:";
                listView4.Columns[1].Text = "Day of Month";
                listView4.Columns[2].Text = "Time";
                listView4.Columns[3].Text = "Contacts";
                label21.Text = "Day of month:";
                label20.Text = "Hours";
                label19.Text = "Minutes";
                label18.Text = "Seconds";
                groupBox11.Text = "Condition channel contacts:";
                radioButton22.Text = "Open";
                radioButton21.Text = "Close";
                button13.Text = "Add";
                button12.Text = "Delete selected";
                button11.Text = "Clear list";
                button25.Text = "Array wizard";
                button46.Text = "Undo last action";

                groupBox12.Text = "Events list of weekly time relay:";
                listView5.Columns[1].Text = "Day of week";
                listView5.Columns[2].Text = "Time";
                listView5.Columns[3].Text = "Contacts";
                label22.Text = "Day of week:";
                comboBox1.Items[0] = stMon;
                comboBox1.Items[1] = stTue;
                comboBox1.Items[2] = stWed;
                comboBox1.Items[3] = stThu;
                comboBox1.Items[4] = stFri;
                comboBox1.Items[5] = stSat;
                comboBox1.Items[6] = stSun;
                label27.Text = "Hours";
                label26.Text = "Minutes";
                label25.Text = "Seconds";
                groupBox13.Text = "Condition channel contacts:";
                radioButton24.Text = "Open";
                radioButton23.Text = "Close";
                button16.Text = "Add";
                button15.Text = "Delete selected";
                button14.Text = "Clear list";
                button37.Text = "Array wizard";
                button47.Text = "Undo last action";

                groupBox14.Text = "Events list of daily time relay:";
                listView6.Columns[1].Text = "Time";
                listView6.Columns[2].Text = "Contacts";                                
                label32.Text = "Hours";
                label31.Text = "Minutes";
                label30.Text = "Seconds";
                groupBox15.Text = "Condition channel contacts:";
                radioButton26.Text = "Open";
                radioButton25.Text = "Close";
                button19.Text = "Add";
                button18.Text = "Delete selected";
                button17.Text = "Clear list";
                button38.Text = "Array wizard";
                button48.Text = "Undo last action";

                groupBox16.Text = "Pulse time relay settings:";
                groupBox17.Text = "Preliminary Delay:";
                groupBox18.Text = "Contacts close duration:";
                groupBox19.Text = "Contacts open duration:";
                label33.Text = "Minutes";
                label38.Text = "Minutes";
                label41.Text = "Minutes";
                label35.Text = "Seconds";
                label36.Text = "Seconds";
                label39.Text = "Seconds";

                groupBox20.Text = "Simple time relay settings:";
                groupBox23.Text = "Delay before contacts close:";
                label50.Text = "Minutes";
                label48.Text = "Seconds";

                groupBox21.Text = "Voltage relay settings:";
                groupBox22.Text = "Operation Mode:";
                groupBox26.Text = "Bottom Threshold Open Delay\n(U < Umin):";
                groupBox36.Text = "Operating range bottom threshold:";
                groupBox27.Text = "Reclosing Delay\n(Umin < U < Umax):";
                groupBox25.Text = "Operating range upper threshold:";
                groupBox28.Text = "Upper Threshold Open Delay\n(U > Umax):";
                radioButton27.Text = "OFF";
                radioButton33.Text = "ON";
                label51.Text = "Minutes";
                label46.Text = "Seconds";
                label72.Text = "Umin, V:";
                label71.Text = "Positive\nhysteresis, V:";
                label54.Text = "Minutes";
                label52.Text = "Seconds";
                label45.Text = "Umax, V:";
                label44.Text = "Negative\nhysteresis, V:";
                label57.Text = "Minutes";
                label55.Text = "Seconds";

                groupBox29.Text = "Photorelay settings:";
                groupBox35.Text = "Operation mode:";
                groupBox24.Text = "Illumination threshold:";
                groupBox32.Text = "Delay if L < Lthr:";
                groupBox31.Text = "Delay if L > Lthr:";
                groupBox33.Text = "Contacts Position if L < Lthr:";
                groupBox30.Text = "Contacts Position if L > Lthr:";
                radioButton28.Text = "OFF";
                radioButton29.Text = "ON";
                label42.Text = "Lthr, lux:";
                label43.Text = "Positive\nhysteresis, lux:";
                label66.Text = "Minutes";
                label64.Text = "Seconds";
                label63.Text = "Minutes";
                label61.Text = "Seconds";
                radioButton34.Text = "Open";
                radioButton31.Text = "Close";
                radioButton35.Text = "Time relay repeat";
                radioButton32.Text = "Open till\nnext event";
                radioButton30.Text = "Close till\nnext event";
                radioButton40.Text = "Open";
                radioButton39.Text = "Close";
                radioButton38.Text = "Time relay repeat";
                radioButton37.Text = "Open till\nnext event";
                radioButton36.Text = "Close till\nnext event";

                groupBox42.Text = "Device general settings:";
                groupBox43.Text = "Common time delay between\nthe moment of device energizing\nand channel operation start:";
                label78.Text = "Minutes";
                label76.Text = "Seconds";
                checkBox12.Text = "Recognize summer time conversion";

                groupBox34.Text = "Time setting:";
                groupBox38.Text = "REV-302 current time:";
                groupBox37.Text = "System time:";
                groupBox39.Text = "Manual time setup:";
                button22.Text = "Reload";
                button20.Text = "Apply";
                button21.Text = "Apply";
                label58.Text = "Synchronize REV-302\ntime with system:";
                label59.Text = "Load in REV-302:";

                richTextBox2.Text = "    To close time relay, select one of the relay operation modes from the \"General time menu settings\". For fine setting of each mode, move to the corresponding menu items where the event lists are edited in case of calendar modes or time parameters for the pulse or common relay are set.\n\n    Days-off and holidays with special event lists are set up within the \"Exceptions\" group.\n\n    Use \"create array\" buttons to edit event lists.\n    For the yearly events list there is a possibility to calculate the moments of Sunrise and Sunset for a set time range with a time shift before or after the event and the required contacts position.";
                richTextBox3.Text = "    Certain days selected from the weekly (days-off) or the yearly (holidays) lists can be marked as exceptional. For such days a separate list of events is made up. To allow executing a separate event list on days-off and holidays, check corresponding checkboxes in the \"Time Relay General Settings\" item.";
                
                label95.Text = "Attention! Last changes are not kept in REV-302";

                FileToolStripMenuItem.Text = "File";
                FileToolStripMenuItem.Text = "File";
                relayToolStripMenuItem.Text = "Relay";
                HelpToolStripMenuItem.Text = "Help";
                createToolStripMenuItem.Text = "New";
                openToolStripMenuItem.Text = "Open";
                saveToolStripMenuItem.Text = "Save";
                saveAsToolStripMenuItem.Text = "Save as...";
                exitToolStripMenuItem.Text = "Exit";
                readSettingsToolStripMenuItem.Text = "Read settings";
                sendSettingsToolStripMenuItem.Text = "Write settings";
                versionProgToolStripMenuItem.Text = "Relay program version";
                contentsHelpToolStripMenuItem.Text = "Content";
                aboutProgramToolStripMenuItem.Text = "About program";

                createToolStripButton.Text = "New";
                openToolStripButton.Text = "Open";
                saveToolStripButton.Text = "Save";
                helpToolStripButton.Text = "Help";

                // переводы меню дерева 
                StRelVrem = "Time Relay ";
                StRelVremOptions = "Time relay general settings";
                StExceptions = "Exceptions ";
                StHolidays = "Holidays ";
                StWeekEnds = "Days off";
                StListEventExceptions = "List of exceptional events ";
                StListEventYear = "Yearly relay events ";
                StListEventMonth = "Monthly relay events ";
                StListEventWeek = "Weekly relay events ";
                StListEventDay = "Daily relay events ";
                StImpulseOptions = "Pulse relay options";
                StSimpleOptions = "Simple relay options";
                StVoltRelayOptions = "Voltage relay ";
                StPhotoRelayOptions = "Photorelay ";
                StDeviceOptions = "Device general settings";
                StTimeCorrect = "Time setting";
                StVoltBrightCorrect = "Voltage and illuminate correction";

                StDaysOfWeek[0] = stMon;
                StDaysOfWeek[1] = stTue;
                StDaysOfWeek[2] = stWed;
                StDaysOfWeek[3] = stThu;
                StDaysOfWeek[4] = stFri;
                StDaysOfWeek[5] = stSat;
                StDaysOfWeek[6] = stSun;
            }
        }
        
        private void Init()
        {            
            int k = 0;

            LanguageInit();     // языковые предустановки при включении            
            
            // предустановки в окне установок времени
            //tabPage1.BackColor = Color.DarkSeaGreen;
            //tabPage1_ChannelMode = Color.DarkSeaGreen;
            //tabPage10_Day = Color.DarkSeaGreen;
            //tabPage11_ImpulseRelay = Color.DarkSeaGreen;
            //tabPage12_SimpleRelay = Color.DarkSeaGreen;
            //tabPage14_RV = Color.DarkSeaGreen;
            //tabPage14_RV = Color.DarkSeaGreen;
            //tabPage15_RF = Color.DarkSeaGreen;
            //tabPage16_Options = Color.DarkSeaGreen;
            //tabPage2_Except = Color.DarkSeaGreen;
            //tabPage2_RTcom = Color.DarkSeaGreen;
            //tabPage2_RtOptions = Color.DarkSeaGreen;
            //tabPage2_Time = Color.DarkSeaGreen;
            //tabPage3_Empty = Color.DarkSeaGreen;
            //tabPage3_VoltBright = Color.DarkSeaGreen;
            //tabPage4_DayOffs = Color.DarkSeaGreen;
            //tabPage5_Holidays = Color.DarkSeaGreen;
            //tabPage6_ExceptEvents = Color.DarkSeaGreen;
            //tabPage7_Year = Color.DarkSeaGreen;
            //tabPage8_Month = Color.DarkSeaGreen;
            //tabPage9_Week = Color.DarkSeaGreen;            
            
            label60.Text = stInfo[LangGlobal];        // инициализация монитора текущего времени устройства
            label67.Text = stAbsent[LangGlobal];
            label68.Text = " "; 
            label69.Text = " ";
            SychShowTime = true;
            SychTime = DateTime.Now;        // запомнить текущее время            
            timer3.Enabled = true;      // запустить выдержку для синхронизации
            
            
            treeView1.Nodes.Clear();
            treeView1.BeginUpdate();
            treeView1.Nodes.Add(stChannel1[LangGlobal]);        // [0]
            k++;
            treeView1.Nodes.Add(stChannel2[LangGlobal]);        // [1]
            k++;            
            for( int i = k ; i < (k + CtrlProgsMax); i++)      // перебор следующих узлов в соот. их номером в массиве
            {
                treeView1.Nodes.Add(stProgram[LangGlobal] + (i - k + 1).ToString());                            //[i]
                //treeView1.Nodes.Add(string.Format("Программа П{0}", i - k + 1));                            //[i]
                treeView1.Nodes[i].Nodes.Add(StRelVrem);                                           //[i][0]
                treeView1.Nodes[i].Nodes[0].Nodes.Add(StRelVremOptions);                      //[i][0][0]
                treeView1.Nodes[i].Nodes[0].Nodes.Add(StExceptions);                                        //[i][0][1]
                treeView1.Nodes[i].Nodes[0].Nodes[1].Nodes.Add(StWeekEnds);                                    //[i][0][1][0]   
                treeView1.Nodes[i].Nodes[0].Nodes[1].Nodes.Add(StHolidays);                                     //[i][0][1][1]
                treeView1.Nodes[i].Nodes[0].Nodes[1].Nodes.Add(StListEventExceptions);                //[i][0][1][0]
                treeView1.Nodes[i].Nodes[0].Nodes.Add(StListEventYear);                            //[i][0][2]
                treeView1.Nodes[i].Nodes[0].Nodes.Add(StListEventMonth);                           //[i][0][3]
                treeView1.Nodes[i].Nodes[0].Nodes.Add(StListEventWeek);                          //[i][0][4]
                treeView1.Nodes[i].Nodes[0].Nodes.Add(StListEventDay);                           //[i][0][5]
                treeView1.Nodes[i].Nodes[0].Nodes.Add(StImpulseOptions);                        //[i][0][6]
                treeView1.Nodes[i].Nodes[0].Nodes.Add(StSimpleOptions);                           //[i][0][7]
                treeView1.Nodes[i].Nodes.Add(StVoltRelayOptions);                                        //[i][1]
                treeView1.Nodes[i].Nodes.Add(StPhotoRelayOptions);                                              //[i][2]                    
            }
            treeView1.Nodes.Add(StDeviceOptions);        
            treeView1.Nodes.Add(StTimeCorrect);
            if (FactoryProgramType == true)
                treeView1.Nodes.Add(StVoltBrightCorrect);       // только для промышленной программы

            treeView1.SelectedNode = treeView1.Nodes[0];
            treeView1.EndUpdate();      // закончить редактировать дерево настроек 

            CreatCollectionForCtrlPrograms();       // наполнение коллекции для 8-ми программ управления
            BackCtrlProgramsOptions.Add(new CtrlProgramOptionsClass());     // создание одного элемента коллекции для возврата последнего действия
       
            // передача ссылок на объекты настроек в объект обработки соединения USB
            Connection.LetLinksOnOptions(DeviceOptions, CtrlProgramsOptions);

            //StMessageUSB = "USB: Связь с реле отсутствует";
            StMessageUSB = stUSBnoCon[LangGlobal];
            label81.Text = StMessageUSB;
            

            // ОТЛАДОЧНЫЙ КОД! УДАЛИТЬ!
            //for (int i = 1; i <= 2000; i++)
            //{
            //    CtrlProgramsOptions.ListEventsYear.Add()
            //}

            ReCalculateEventsInLists();
            ReDrawCorrespondenceProgAndChannel();
        }

        // метод обновления информации о кол-вах событий в каждом пункте
        private void ReCalculateEventsInLists()
        {            
            for (int pr = 1; pr <= 8; pr++)
            {
                ReCalculateEventsInLists(pr);                
            }
        }

        // метод обновления информации о кол-вах событий в рамках одной из управляющих программ
        private void ReCalculateEventsInLists(int pr)
        {
            if (pr == 0)
                pr = 1;
            
            int i = pr + 1;         // узел

            int EventsInProgram = CtrlProgramsOptions[pr].ListHolidays.Count + CtrlProgramsOptions[pr].ListEventsException.Count + CtrlProgramsOptions[pr].ListEventsYear.Count + CtrlProgramsOptions[pr].ListEventsMonth.Count + CtrlProgramsOptions[pr].ListEventsWeek.Count + CtrlProgramsOptions[pr].ListEventsDay.Count;
            treeView1.Nodes[i].Text = stProgram[LangGlobal] + (pr).ToString() + " (" + EventsInProgram.ToString() + stEvents_t1[LangGlobal];                            //[i]                

            string str1;
            switch (CtrlProgramsOptions[pr].RelayTimeMode)
            {
                case CtrlProgramOptionsClass.RelayTimeModeType.R_T_M_OFF:
                    str1 = stInTreeOff[LangGlobal];
                    break;
                case CtrlProgramOptionsClass.RelayTimeModeType.R_T_M_YEAR:
                    str1 = stInTreeYear[LangGlobal];
                    break;
                case CtrlProgramOptionsClass.RelayTimeModeType.R_T_M_MONTH:
                    str1 = stInTreeMonth[LangGlobal];
                    break;
                case CtrlProgramOptionsClass.RelayTimeModeType.R_T_M_WEEK:
                    str1 = stInTreeWeek[LangGlobal];
                    break;
                case CtrlProgramOptionsClass.RelayTimeModeType.R_T_M_DAY:
                    str1 = stInTreeDay[LangGlobal];
                    break;
                case CtrlProgramOptionsClass.RelayTimeModeType.R_T_M_PULSE:
                    str1 = stInTreePulse[LangGlobal];
                    break;
                case CtrlProgramOptionsClass.RelayTimeModeType.R_T_M_SIMPLE:
                    str1 = stInTreeSimple[LangGlobal];
                    break;
                default :
                    str1 = "";
                    break;
            }            
            treeView1.Nodes[i].Nodes[0].Text = StRelVrem + str1;                                           //[i][0]

            treeView1.Nodes[i].Nodes[0].Nodes[1].Text = StExceptions + PlaceInScobe(CtrlProgramsOptions[pr].ListHolidays.Count + CtrlProgramsOptions[pr].ListEventsException.Count);                                        //[i][0][1]
            treeView1.Nodes[i].Nodes[0].Nodes[1].Nodes[1].Text = StHolidays + PlaceInScobe(CtrlProgramsOptions[pr].ListHolidays.Count);                                     //[i][0][1][1]
            treeView1.Nodes[i].Nodes[0].Nodes[1].Nodes[2].Text = StListEventExceptions + PlaceInScobe(CtrlProgramsOptions[pr].ListEventsException.Count);                //[i][0][1][0]
            treeView1.Nodes[i].Nodes[0].Nodes[2].Text = StListEventYear + PlaceInScobe(CtrlProgramsOptions[pr].ListEventsYear.Count);                            //[i][0][2]
            treeView1.Nodes[i].Nodes[0].Nodes[3].Text = StListEventMonth + PlaceInScobe(CtrlProgramsOptions[pr].ListEventsMonth.Count);
            treeView1.Nodes[i].Nodes[0].Nodes[4].Text = StListEventWeek + PlaceInScobe(CtrlProgramsOptions[pr].ListEventsWeek.Count);
            treeView1.Nodes[i].Nodes[0].Nodes[5].Text = StListEventDay + PlaceInScobe(CtrlProgramsOptions[pr].ListEventsDay.Count);
                        
            if(CtrlProgramsOptions[pr].RV_OnOff == true )
                str1 = stInTreeON[LangGlobal];
            else
                str1 = stInTreeOff[LangGlobal];            
            treeView1.Nodes[i].Nodes[1].Text = StVoltRelayOptions + str1;                                        //[i][1]
                        
            if(CtrlProgramsOptions[pr].RF_OnOff == true )
                str1 = stInTreeON[LangGlobal];
            else
                str1 = stInTreeOff[LangGlobal];
            treeView1.Nodes[i].Nodes[2].Text = StPhotoRelayOptions + str1;                                              //[i][2]
        }

        // добавляет в дереве меню какая программа управления соот. каналу
        private void ReDrawCorrespondenceProgAndChannel()
        {
            string str1;

            for (int i = 0; i <= 1; i++)
            {
                if (DeviceOptions.Channel_CtrlProg[i+1] == 0)
                    //str1 = string.Format("Канал К{0} (отключен)", i+1);
                    str1 = stInTreeChannelN[LangGlobal] + (i+1).ToString() + stInTreeOffFull[LangGlobal];
                else
                    //str1 = string.Format("Канал К{0} (П{1})", i+1, DeviceOptions.Channel_CtrlProg[i+1]);
                    str1 = stInTreeChannelN[LangGlobal] + (i+1).ToString() + " (" + stInTreeP[LangGlobal] + DeviceOptions.Channel_CtrlProg[i + 1].ToString() + ")";

                treeView1.Nodes[i].Text = str1;
            }

            //treeView1.Nodes[0].Text = string.Format("Канал К1 (П{0})", DeviceOptions.Channel_CtrlProg[0]);
            //treeView1.Nodes[1].Text = string.Format("Канал К2 (П{0})", DeviceOptions.Channel_CtrlProg[1]);
        }

        private string PlaceInScobe(int In)     // помещает полученное число в скобки в текстовом виде
        {
            return "(" + In.ToString() + ")";
        }


        private void CreatCollectionForCtrlPrograms()
        {
            // наполнение коллекции настроек управляющих программ            
            for (int i = 0; i <= CtrlProgsMax + 1; i++)
            {
                CtrlProgramsOptions.Add(new CtrlProgramOptionsClass());      // добавить 8-ь программ, (нулевая не считается)
            }
        }

        // метод события выбора нового пункта меню
        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            TreeNode SaveNode;
            TreeNode SelectedNode;

            SelectedNode = e.Node;
            
            
            if( e.Node.GetType() == typeof(TreeNode))
            {
                if (e.Node.Parent != null)
                {
                    // проследить до корневого узла для определения программы
                    SaveNode = e.Node;
                    while (SaveNode.Parent != null)
                    {
                        SaveNode = SaveNode.Parent;
                    }
                    ProgSelected = SaveNode.Index - 1;      // номер выбранной программы
                    if (ProgSelected > 8)
                    {
                        ProgSelected = 0;
                    }
                                       
                    // определить текущий пункт и вызвать соот. метод для перерисовки окна
                    InVisibleAllPanels();
                        
                        //==========
                    //string strtmp1 = e.Node.Text.Split('(')[0];

                    //MessageBox.Show(strtmp1,
                    //                    "Служебная информация", MessageBoxButtons.OK,
                    //                    MessageBoxIcon.Asterisk);                    
                        //==========
                    string TextOfNode = e.Node.Text.Split('(')[0];
                    if (TextOfNode == StRelVrem)
                    {
                        ShowWin_RT();
                    }
                    else if (TextOfNode == StRelVremOptions)
                    {
                        ShowWin_RTOptions();
                    }
                    else if (TextOfNode == StExceptions)
                    {
                        ShowWin_Exceptions();
                    }
                    else if (TextOfNode == StHolidays)
                    {
                        ShowWin_Holidays();
                    }
                    else if (TextOfNode == StWeekEnds)
                    {
                        ShowWin_WeekEnds();
                    }
                    else if (TextOfNode == StListEventExceptions)
                    {
                        ShowWin_ListOfExcept();
                    }
                    else if (TextOfNode == StListEventYear)
                    {
                        ShowWin_ListOfYearEvents();
                    }
                    else if (TextOfNode == StListEventMonth)
                    {
                        ShowWin_ListOfMonthEvents(); 
                    }
                    else if (TextOfNode == StListEventWeek)
                    {
                        ShowWin_ListOfWeekEvents();
                    }
                    else if (TextOfNode == StListEventDay)
                    {
                        ShowWin_ListOfDayEvents();
                    }
                    else if (TextOfNode == StImpulseOptions)
                    {
                        ShowWin_ImpulseOptions();
                    }
                    else if (TextOfNode == StSimpleOptions)
                    {
                        ShowWin_SimpleOptions();
                    }
                    else if (TextOfNode == StVoltRelayOptions)
                    {
                        ShowWin_RVOptions();
                    }
                    else if (TextOfNode == StPhotoRelayOptions)
                    {
                        ShowWin_RFOptions();
                    }                    
                }
                else if (e.Node.Parent == null)
                {
                    // если это сразу корневой узел, то выяснить номер узла от начала
                    InVisibleAllPanels();       // выключить панель для вывода другой
                    if (e.Node.Index == 0)
                    {
                        // выбран К1   
                        ChannelSelected = 1;
                        
                        ShowWin_Channel();
                    }
                    else if (e.Node.Index == 1)
                    {
                        // выбран К2
                        ChannelSelected = 2;

                        ShowWin_Channel();
                    }
                    else if (e.Node.Index >= 2 && e.Node.Index < 2 + CtrlProgsMax)
                    {
                        // определение номера программы
                        ProgSelected = e.Node.Index - 1;      // номер выбранной программы

                        ShowWin_Empty();
                    }
                    else if (e.Node.Index == 2 + CtrlProgsMax)
                    {
                        ShowWin_CommonDeviceOptions();
                    }
                    else if (e.Node.Index == 3 + CtrlProgsMax)
                    {
                        ShowWin_TimeCorrect();   
                    }
                    else if (e.Node.Index == 4 + CtrlProgsMax)
                    {
                        ShowWin_VoltBrightCorrect();
                    }
                }
                label1.Text = e.Node.Text;
                label2.Text = string.Format("Номер корневого узла: {0}.", e.Node.Index);
                label3.Text = string.Format("Номер программы №{0}", ProgSelected);
            }

            //treeView1.SelectedNode = e.Node; 
            treeView1.Focus();
            //e.Node.Text = e.Node.Text.ToUpper(); 
           
            // скрыть все кнопки отвечающие за отмену действий со списками
            UnVisibledBackButtons();
        }

        

        
        //=================== методы вывода соот. окна с одновременным обновлением в нем информации
        private void ShowWin_VoltBrightCorrect()
        {
            label4.Text = stVoltAndPhoto[LangGlobal];   // string.Format("Напряжение и освещенность", ProgSelected);

            tabControl1.SelectedTab = tabPage3_VoltBright;
            //UpdateAllFormElements();
        }       
        
        private void ShowWin_TimeCorrect()
        {
            label4.Text = stTimeSetting[LangGlobal];    // string.Format("Установка времени", ProgSelected);


            tabControl1.SelectedTab = tabPage2_Time;                        
        }
        
        private void ShowWin_CommonDeviceOptions()
        {
            label4.Text = stGeneralSettings[LangGlobal];        // string.Format("Общие настройки устройства", ProgSelected);
            numericUpDown42.Value = DeviceOptions.CommonDelay.Minute;
            numericUpDown41.Value = DeviceOptions.CommonDelay.Second;
            checkBox12.Checked = DeviceOptions.DST_OnOff;

            tabControl1.SelectedTab = tabPage16_Options; // пустое окно
            //UpdateAllFormElements();            
        }

        private void ShowWin_Empty()
        {
            comboBox2.Items.Clear();
            for (int i = 1; i <= 8; i++)
            {
                if(i != ProgSelected )
                    comboBox2.Items.Add(stProgram[LangGlobal] + i.ToString()); // string.Format("Программа П{0}", i));
            }
            comboBox2.SelectedIndex = 0;

            label4.Text = string.Format(stProgram[LangGlobal] + ProgSelected.ToString());      //"Программа П{0}", ProgSelected);
            tabControl1.SelectedTab = tabPage3_Empty; // пустое окно
        }

        private void ShowWin_RFOptions()
        {
            tabControl1.SelectedTab = tabPage15_RF; // пустое окно
            label4.Text = stInTreeP[LangGlobal] + ProgSelected.ToString() + ":  " + stPhotoRelay[LangGlobal];         //   "П{0}:  Фотореле", ProgSelected);
            radioButton28.Checked = !CtrlProgramsOptions[ProgSelected].RF_OnOff;
            radioButton29.Checked = CtrlProgramsOptions[ProgSelected].RF_OnOff;
            numericUpDown23.Value = CtrlProgramsOptions[ProgSelected].RF_Lpor;
            numericUpDown24.Value = CtrlProgramsOptions[ProgSelected].RF_Lporhyst;
            numericUpDown40.Value = CtrlProgramsOptions[ProgSelected].RF_DelayLmin.Minute;
            numericUpDown39.Value = CtrlProgramsOptions[ProgSelected].RF_DelayLmin.Second;
            numericUpDown38.Value = CtrlProgramsOptions[ProgSelected].RF_DelayLmax.Minute;
            numericUpDown37.Value = CtrlProgramsOptions[ProgSelected].RF_DelayLmax.Second;
            switch (CtrlProgramsOptions[ProgSelected].RF_Condition_Lmin)
            {
                case 0:
                    radioButton34.Checked = true;
                    break;
                case 1:
                    radioButton31.Checked = true;
                    break;
                case 2:
                    radioButton35.Checked = true;
                    break;
                case 3:
                    radioButton32.Checked = true;
                    break;
                case 4:
                    radioButton30.Checked = true;
                    break;
            }
            switch (CtrlProgramsOptions[ProgSelected].RF_Condition_Lmax)
            {
                case 0:
                    radioButton40.Checked = true;
                    break;
                case 1:
                    radioButton39.Checked = true;
                    break;
                case 2:
                    radioButton38.Checked = true;
                    break;
                case 3:
                    radioButton37.Checked = true;
                    break;
                case 4:
                    radioButton36.Checked = true;
                    break;
            }
            //UpdateAllFormElements();            
        }

        private void ShowWin_RVOptions()
        {
            //label4.Text = string.Format("П{0}:  Реле напряжения", ProgSelected);
            label4.Text = stInTreeP[LangGlobal] + ProgSelected.ToString() + ":  " + stRV[LangGlobal];    // string.Format("П{0}:  Реле напряжения", ProgSelected);
            tabControl1.SelectedTab = tabPage14_RV;
            //UpdateAllFormElements();
            // элементы реле напряжения
            radioButton27.Checked = !CtrlProgramsOptions[ProgSelected].RV_OnOff;
            radioButton33.Checked = CtrlProgramsOptions[ProgSelected].RV_OnOff;
            numericUpDown46.Value = (decimal)CtrlProgramsOptions[ProgSelected].RV_Umin;
            numericUpDown45.Value = (decimal)CtrlProgramsOptions[ProgSelected].RV_Uminhyst;
            numericUpDown26.Value = (decimal)CtrlProgramsOptions[ProgSelected].RV_Umax;
            numericUpDown25.Value = (decimal)CtrlProgramsOptions[ProgSelected].RV_Umaxhyst;
            numericUpDown30.Value = (decimal)CtrlProgramsOptions[ProgSelected].RV_DelayUmin.Minute;
            numericUpDown29.Value = (decimal)CtrlProgramsOptions[ProgSelected].RV_DelayUmin.Second;
            numericUpDown32.Value = (decimal)CtrlProgramsOptions[ProgSelected].RV_DelayUnorm.Minute;
            numericUpDown31.Value = (decimal)CtrlProgramsOptions[ProgSelected].RV_DelayUnorm.Second;
            numericUpDown34.Value = (decimal)CtrlProgramsOptions[ProgSelected].RV_DelayUmax.Minute;
            numericUpDown33.Value = (decimal)CtrlProgramsOptions[ProgSelected].RV_DelayUmax.Second;            
        }

        private void ShowWin_SimpleOptions()
        {
            //label4.Text = string.Format("П{0}:  Простое реле времени", ProgSelected);
            label4.Text = stInTreeP[LangGlobal] + ProgSelected.ToString() + ":  " + stSimpleRT[LangGlobal];
            //UpdateAllFormElements();
            numericUpDown28.Value = CtrlProgramsOptions[ProgSelected].RS_Delay.Minute;
            numericUpDown27.Value = CtrlProgramsOptions[ProgSelected].RS_Delay.Second;

            tabControl1.SelectedTab = tabPage12_SimpleRelay;
        }

        private void ShowWin_ImpulseOptions()
        {
            //label4.Text = string.Format("П{0}:  Импульсное реле времени", ProgSelected);
            label4.Text = stInTreeP[LangGlobal] + ProgSelected.ToString() + ":  " + stPulseRT[LangGlobal];
            //UpdateAllFormElements();            
            numericUpDown17.Value = CtrlProgramsOptions[ProgSelected].RI_BeforeDelay.Minute;
            numericUpDown18.Value = CtrlProgramsOptions[ProgSelected].RI_BeforeDelay.Second;
            numericUpDown20.Value = CtrlProgramsOptions[ProgSelected].RI_OnDelay.Minute;
            numericUpDown19.Value = CtrlProgramsOptions[ProgSelected].RI_OnDelay.Second;
            numericUpDown22.Value = CtrlProgramsOptions[ProgSelected].RI_OffDelay.Minute;
            numericUpDown21.Value = CtrlProgramsOptions[ProgSelected].RI_OffDelay.Second;

            tabControl1.SelectedTab = tabPage11_ImpulseRelay;
        }

        private void ShowWin_ListOfDayEvents()
        {
            //label4.Text = string.Format("П{0}:  События суточного реле времени", ProgSelected);
            label4.Text = stInTreeP[LangGlobal] + ProgSelected.ToString() + ":  " + stEventsDailyRT[LangGlobal];
            //UpdateAllFormElements();  
            radioButton26.Checked = true;       // предустановить состояние контактов
            ReDrawListOfEventsDay();

            tabControl1.SelectedTab = tabPage10_Day;
        }

        private void ShowWin_ListOfWeekEvents()
        {
            //label4.Text = string.Format("П{0}:  События недельного реле времени", ProgSelected);
            label4.Text = stInTreeP[LangGlobal] + ProgSelected.ToString() + ":  " + stEventsWeeklyRT[LangGlobal];
            //UpdateAllFormElements();  
            comboBox1.SelectedIndex = 0;
            radioButton24.Checked = true;       // предустановить состояние контактов
            ReDrawListOfEventsWeek();
            
            tabControl1.SelectedTab = tabPage9_Week;
        }

        private void ShowWin_ListOfMonthEvents()
        {
            //label4.Text = string.Format("П{0}:  События месячного реле времени", ProgSelected);
            label4.Text = stInTreeP[LangGlobal] + ProgSelected.ToString() + ":  " + stEventsMonthlyRT[LangGlobal];
            //UpdateAllFormElements();  
            radioButton22.Checked = true;       // предустановить состояние контактов
            ReDrawListOfEventsMonth();
            
            tabControl1.SelectedTab = tabPage8_Month;
        }

        private void ShowWin_ListOfYearEvents()
        {
            //label4.Text = string.Format("П{0}:  События годового реле времени", ProgSelected);
            label4.Text = stInTreeP[LangGlobal] + ProgSelected.ToString() + ":  " + stEventsYearlyRT[LangGlobal];
            //UpdateAllFormElements();              
            radioButton20.Checked = true;       // предустановить состояние контактов
            ReDrawListOfEventsYear();

            tabControl1.SelectedTab = tabPage7_Year;
        }

        private void ShowWin_ListOfExcept()
        {
            //label4.Text = string.Format("П{0}:  Исключительные события", ProgSelected);
            label4.Text = stInTreeP[LangGlobal] + ProgSelected.ToString() + ":  " + stExceptionEventsRT[LangGlobal];
            //UpdateAllFormElements();  
            radioButton17.Checked = true;       // предустановить состояние контактов
            ReDrawListOfEventsExceptions();     // заполнить список исключительных событий
            
            tabControl1.SelectedTab = tabPage6_ExceptEvents;
        }

        private void ShowWin_WeekEnds()
        {
            //label4.Text = string.Format("П{0}:  Выходные дни", ProgSelected);
            label4.Text = stInTreeP[LangGlobal] + ProgSelected.ToString() + ":  " + stDaysOffRT[LangGlobal];
            //UpdateAllFormElements();              
            int i = 1;
            checkBox4.Checked = CtrlProgramsOptions[ProgSelected].ExceptDaysOfWeek[i++];
            checkBox5.Checked = CtrlProgramsOptions[ProgSelected].ExceptDaysOfWeek[i++];
            checkBox6.Checked = CtrlProgramsOptions[ProgSelected].ExceptDaysOfWeek[i++];
            checkBox7.Checked = CtrlProgramsOptions[ProgSelected].ExceptDaysOfWeek[i++];
            checkBox8.Checked = CtrlProgramsOptions[ProgSelected].ExceptDaysOfWeek[i++];
            checkBox9.Checked = CtrlProgramsOptions[ProgSelected].ExceptDaysOfWeek[i++];
            checkBox10.Checked = CtrlProgramsOptions[ProgSelected].ExceptDaysOfWeek[i++];

            tabControl1.SelectedTab = tabPage4_DayOffs;
        }

        private void ShowWin_Holidays()
        {
            //label4.Text = string.Format("П{0}:  Праздники", ProgSelected);
            label4.Text = stInTreeP[LangGlobal] + ProgSelected.ToString() + ":  " + stHolidaysRT[LangGlobal];
            //UpdateAllFormElements();  
            ReDrawListOfHolidays();     // заполнить список праздников

            tabControl1.SelectedTab = tabPage5_Holidays;
        }

        private void ShowWin_Exceptions()
        {
            //label4.Text = string.Format("П{0}:  Исключения", ProgSelected);
            label4.Text = stInTreeP[LangGlobal] + ProgSelected.ToString() + ":  " + stExceptionsRT[LangGlobal];
            tabControl1.SelectedTab = tabPage2_Except; // пустое окно
        }

        private void ShowWin_RT()
        {
            //label4.Text = string.Format("П{0}:  Реле времени", ProgSelected);
            label4.Text = stInTreeP[LangGlobal] + ProgSelected.ToString() + ":  " + stRT[LangGlobal];
            tabControl1.SelectedTab = tabPage2_RTcom;
        }

        private void ShowWin_RTOptions()
        {
            //label4.Text = string.Format("П{0}:  Общие настройки реле времени", ProgSelected);
            label4.Text = stInTreeP[LangGlobal] + ProgSelected.ToString() + ":  " + stGeneralOptionsRT[LangGlobal];
            //UpdateAllFormElements();  
            //groupBox2.Text = string.Format("Выберите режим работы реле времени программы П{0}:", ProgSelected);
            groupBox2.Text = stChooseModeRT[LangGlobal] + ProgSelected.ToString() + ":";
            switch (CtrlProgramsOptions[ProgSelected].RelayTimeMode)
            {
                case CtrlProgramOptionsClass.RelayTimeModeType.R_T_M_OFF:
                    radioButton16.Checked = true;
                    break;
                case CtrlProgramOptionsClass.RelayTimeModeType.R_T_M_YEAR:
                    radioButton9.Checked = true;
                    break;
                case CtrlProgramOptionsClass.RelayTimeModeType.R_T_M_MONTH:
                    radioButton10.Checked = true;
                    break;
                case CtrlProgramOptionsClass.RelayTimeModeType.R_T_M_WEEK:
                    radioButton11.Checked = true;
                    break;
                case CtrlProgramOptionsClass.RelayTimeModeType.R_T_M_DAY:
                    radioButton12.Checked = true;
                    break;
                case CtrlProgramOptionsClass.RelayTimeModeType.R_T_M_PULSE:
                    radioButton13.Checked = true;
                    break;
                case CtrlProgramOptionsClass.RelayTimeModeType.R_T_M_SIMPLE:
                    radioButton14.Checked = true;
                    break;
            }
            checkBox1.Checked = CtrlProgramsOptions[ProgSelected].AllowDaysoffs;
            checkBox2.Checked = CtrlProgramsOptions[ProgSelected].AllowHolidays;
            checkBox3.Checked = CtrlProgramsOptions[ProgSelected].AllowCyclicity;

            tabControl1.SelectedTab = tabPage2_RtOptions;
        }

        // вывод окна активного канала
        private void ShowWin_Channel()
        {            
            //label4.Text = string.Format("Канал К{0}", ChannelSelected);
            label4.Text = stInTreeChannelN[LangGlobal] + ChannelSelected.ToString();
            //UpdateAllFormElements();
            //groupBox1.Text = string.Format("Выберите программу для управления контактами канала К{0}:", ChannelSelected);
            groupBox1.Text = stChooseProgram[LangGlobal] + ChannelSelected.ToString() + ":";
            // установить программу в соот. с текущими настройками
            switch (DeviceOptions.Channel_CtrlProg[ChannelSelected])
            {
                case 0:
                    radioButton15.Checked = true;
                    break;
                case 1:
                    radioButton1.Checked = true;
                    break;
                case 2:
                    radioButton2.Checked = true;
                    break;
                case 3:
                    radioButton3.Checked = true;
                    break;
                case 4:
                    radioButton4.Checked = true;
                    break;
                case 5:
                    radioButton5.Checked = true;
                    break;
                case 6:
                    radioButton6.Checked = true;
                    break;
                case 7:
                    radioButton7.Checked = true;
                    break;
                case 8:
                    radioButton8.Checked = true;
                    break;
            }

            tabControl1.SelectedTab = tabPage1_ChannelMode;            
        }

        // метод обновления всей информации во всех окнах, отображаемых и неотображаемых в соот. с активным каналом или программой управления
        private void UpdateAllFormElements()
        {
            // обновление элементов общих настроек всего устройства
            numericUpDown42.Value = DeviceOptions.CommonDelay.Minute;
            numericUpDown41.Value = DeviceOptions.CommonDelay.Second;
            checkBox12.Checked = DeviceOptions.DST_OnOff;

            // настройки фото-реле
            radioButton28.Checked = !CtrlProgramsOptions[ProgSelected].RF_OnOff;
            radioButton29.Checked = CtrlProgramsOptions[ProgSelected].RF_OnOff;
            numericUpDown23.Value = CtrlProgramsOptions[ProgSelected].RF_Lpor;
            numericUpDown24.Value = CtrlProgramsOptions[ProgSelected].RF_Lporhyst;
            numericUpDown40.Value = CtrlProgramsOptions[ProgSelected].RF_DelayLmin.Minute;
            numericUpDown39.Value = CtrlProgramsOptions[ProgSelected].RF_DelayLmin.Second;
            numericUpDown38.Value = CtrlProgramsOptions[ProgSelected].RF_DelayLmax.Minute;
            numericUpDown37.Value = CtrlProgramsOptions[ProgSelected].RF_DelayLmax.Second;
            switch (CtrlProgramsOptions[ProgSelected].RF_Condition_Lmin)
            {
                case 0:
                    radioButton34.Checked = true;
                    break;
                case 1:
                    radioButton31.Checked = true;
                    break;
                case 2:
                    radioButton35.Checked = true;
                    break;
                case 3:
                    radioButton32.Checked = true;
                    break;
                case 4:
                    radioButton30.Checked = true;
                    break;
            }
            switch (CtrlProgramsOptions[ProgSelected].RF_Condition_Lmax)
            {
                case 0:
                    radioButton40.Checked = true;
                    break;
                case 1:
                    radioButton39.Checked = true;
                    break;
                case 2:
                    radioButton38.Checked = true;
                    break;
                case 3:
                    radioButton37.Checked = true;
                    break;
                case 4:
                    radioButton36.Checked = true;
                    break;
            }

            // элементы реле напряжения
            radioButton27.Checked = !CtrlProgramsOptions[ProgSelected].RV_OnOff;
            radioButton33.Checked = CtrlProgramsOptions[ProgSelected].RV_OnOff;
            numericUpDown46.Value = (decimal)CtrlProgramsOptions[ProgSelected].RV_Umin;
            numericUpDown45.Value = (decimal)CtrlProgramsOptions[ProgSelected].RV_Uminhyst;
            numericUpDown26.Value = (decimal)CtrlProgramsOptions[ProgSelected].RV_Umax;
            numericUpDown25.Value = (decimal)CtrlProgramsOptions[ProgSelected].RV_Umaxhyst;
            numericUpDown30.Value = (decimal)CtrlProgramsOptions[ProgSelected].RV_DelayUmin.Minute;
            numericUpDown29.Value = (decimal)CtrlProgramsOptions[ProgSelected].RV_DelayUmin.Second;
            numericUpDown32.Value = (decimal)CtrlProgramsOptions[ProgSelected].RV_DelayUnorm.Minute;
            numericUpDown31.Value = (decimal)CtrlProgramsOptions[ProgSelected].RV_DelayUnorm.Second;
            numericUpDown34.Value = (decimal)CtrlProgramsOptions[ProgSelected].RV_DelayUmax.Minute;
            numericUpDown33.Value = (decimal)CtrlProgramsOptions[ProgSelected].RV_DelayUmax.Second;

            // элементы простого реле
            numericUpDown28.Value = CtrlProgramsOptions[ProgSelected].RS_Delay.Minute;
            numericUpDown27.Value = CtrlProgramsOptions[ProgSelected].RS_Delay.Second;

            // элементы импульсного реле
            numericUpDown17.Value = CtrlProgramsOptions[ProgSelected].RI_BeforeDelay.Minute;
            numericUpDown18.Value = CtrlProgramsOptions[ProgSelected].RI_BeforeDelay.Second;
            numericUpDown20.Value = CtrlProgramsOptions[ProgSelected].RI_OnDelay.Minute;
            numericUpDown19.Value = CtrlProgramsOptions[ProgSelected].RI_OnDelay.Second;
            numericUpDown22.Value = CtrlProgramsOptions[ProgSelected].RI_OffDelay.Minute;
            numericUpDown21.Value = CtrlProgramsOptions[ProgSelected].RI_OffDelay.Second;

            // элементы суточных событий
            radioButton26.Checked = true;       // предустановить состояние контактов
            ReDrawListOfEventsDay();

            // элементы недельных собыйти
            comboBox1.SelectedIndex = 0;
            radioButton24.Checked = true;       // предустановить состояние контактов
            ReDrawListOfEventsWeek();

            // элементы месячных событий
            radioButton22.Checked = true;       // предустановить состояние контактов
            ReDrawListOfEventsMonth();

            // элементы годовых событий
            radioButton20.Checked = true;       // предустановить состояние контактов
            ReDrawListOfEventsYear();

            // элементы исключительных событий
            radioButton17.Checked = true;       // предустановить состояние контактов
            ReDrawListOfEventsExceptions();     // заполнить список исключительных событий

            // элементы праздников
            ReDrawListOfHolidays();     // заполнить список праздников

            // элементы выходных дней
            int i = 1;
            checkBox4.Checked = CtrlProgramsOptions[ProgSelected].ExceptDaysOfWeek[i++];
            checkBox5.Checked = CtrlProgramsOptions[ProgSelected].ExceptDaysOfWeek[i++];
            checkBox6.Checked = CtrlProgramsOptions[ProgSelected].ExceptDaysOfWeek[i++];
            checkBox7.Checked = CtrlProgramsOptions[ProgSelected].ExceptDaysOfWeek[i++];
            checkBox8.Checked = CtrlProgramsOptions[ProgSelected].ExceptDaysOfWeek[i++];
            checkBox9.Checked = CtrlProgramsOptions[ProgSelected].ExceptDaysOfWeek[i++];
            checkBox10.Checked = CtrlProgramsOptions[ProgSelected].ExceptDaysOfWeek[i++];

            // элементы настройки реле времени
            //groupBox2.Text = string.Format("Выберите режим работы реле времени программы П{0}:", ProgSelected);
            groupBox2.Text = stChooseModeRT[LangGlobal] + ProgSelected.ToString() + ":";
            switch (CtrlProgramsOptions[ProgSelected].RelayTimeMode)
            {
                case CtrlProgramOptionsClass.RelayTimeModeType.R_T_M_OFF:
                    radioButton16.Checked = true;
                    break;
                case CtrlProgramOptionsClass.RelayTimeModeType.R_T_M_YEAR:
                    radioButton9.Checked = true;
                    break;
                case CtrlProgramOptionsClass.RelayTimeModeType.R_T_M_MONTH:
                    radioButton10.Checked = true;
                    break;
                case CtrlProgramOptionsClass.RelayTimeModeType.R_T_M_WEEK:
                    radioButton11.Checked = true;
                    break;
                case CtrlProgramOptionsClass.RelayTimeModeType.R_T_M_DAY:
                    radioButton12.Checked = true;
                    break;
                case CtrlProgramOptionsClass.RelayTimeModeType.R_T_M_PULSE:
                    radioButton13.Checked = true;
                    break;
                case CtrlProgramOptionsClass.RelayTimeModeType.R_T_M_SIMPLE:
                    radioButton14.Checked = true;
                    break;
            }
            checkBox1.Checked = CtrlProgramsOptions[ProgSelected].AllowDaysoffs;
            checkBox2.Checked = CtrlProgramsOptions[ProgSelected].AllowHolidays;
            checkBox3.Checked = CtrlProgramsOptions[ProgSelected].AllowCyclicity;

            // элементы окна канала
            //groupBox1.Text = string.Format("Выберите программу для управления контактами канала К{0}:", ChannelSelected);
            groupBox1.Text = stChooseProgram[LangGlobal] + ChannelSelected.ToString() + ":";
                    // установить программу в соот. с текущими настройками
            switch (DeviceOptions.Channel_CtrlProg[ChannelSelected])
            {
                case 0:
                    radioButton15.Checked = true;
                    break;
                case 1:
                    radioButton1.Checked = true;
                    break;
                case 2:
                    radioButton2.Checked = true;
                    break;
                case 3:
                    radioButton3.Checked = true;
                    break;
                case 4:
                    radioButton4.Checked = true;
                    break;
                case 5:
                    radioButton5.Checked = true;
                    break;
                case 6:
                    radioButton6.Checked = true;
                    break;
                case 7:
                    radioButton7.Checked = true;
                    break;
                case 8:
                    radioButton8.Checked = true;
                    break;
            }

            ReCalculateEventsInLists();
            ReDrawCorrespondenceProgAndChannel();
        }
                

        // скрывает все панели в области настроек с элементами управления
        private void InVisibleAllPanels()
        {
            //tabControl1.SelectedTab = tabPage3_Empty;       // переход в пустое окно по умолчанию
        }

        

        //============ события выбора режима канала
        private void radioButton15_Click(object sender, EventArgs e)
        {
            // если выбран режим "Отключен"
            SetChannelMode(0);
        }
        
        private void radioButton1_Click(object sender, EventArgs e)
        {
            SetChannelMode(1);
        }        

        private void radioButton2_Click(object sender, EventArgs e)
        {
            SetChannelMode(2);
        }

        private void radioButton3_Click(object sender, EventArgs e)
        {
            SetChannelMode(3);
        }

        private void radioButton4_Click(object sender, EventArgs e)
        {
            SetChannelMode(4);
        }

        private void radioButton5_Click(object sender, EventArgs e)
        {
            SetChannelMode(5);
        }

        private void radioButton6_Click(object sender, EventArgs e)
        {
            SetChannelMode(6);
        }

        private void radioButton7_Click(object sender, EventArgs e)
        {
            SetChannelMode(7);
        }

        private void radioButton8_Click(object sender, EventArgs e)
        {
            SetChannelMode(8);
        }        

        private void SetChannelMode(byte SelProg)
        {
            if (ChannelSelected == 1)
            {
                DeviceOptions.Channel_CtrlProg[1] = SelProg;
            }
            else if (ChannelSelected == 2)
            {
                DeviceOptions.Channel_CtrlProg[2] = SelProg;
            }

            ReDrawCorrespondenceProgAndChannel();       // указать соот. в дереве
        }

        //============== события выбора режима реле времени
        private void radioButton16_Click(object sender, EventArgs e)
        {
            SetRTMode(CtrlProgramOptionsClass.RelayTimeModeType.R_T_M_OFF);

            ReCalculateEventsInLists(ProgSelected);
        }

        private void radioButton9_Click(object sender, EventArgs e)
        {
            SetRTMode(CtrlProgramOptionsClass.RelayTimeModeType.R_T_M_YEAR);

            ReCalculateEventsInLists(ProgSelected);
        }

        private void radioButton10_Click(object sender, EventArgs e)
        {
            SetRTMode(CtrlProgramOptionsClass.RelayTimeModeType.R_T_M_MONTH);

            ReCalculateEventsInLists(ProgSelected);
        }

        private void radioButton11_Click(object sender, EventArgs e)
        {
            SetRTMode(CtrlProgramOptionsClass.RelayTimeModeType.R_T_M_WEEK);

            ReCalculateEventsInLists(ProgSelected);
        }

        private void radioButton12_Click(object sender, EventArgs e)
        {
            SetRTMode(CtrlProgramOptionsClass.RelayTimeModeType.R_T_M_DAY);

            ReCalculateEventsInLists(ProgSelected);
        }

        private void radioButton13_Click(object sender, EventArgs e)
        {
            SetRTMode(CtrlProgramOptionsClass.RelayTimeModeType.R_T_M_PULSE);

            ReCalculateEventsInLists(ProgSelected);
        }

        private void radioButton14_Click(object sender, EventArgs e)
        {
            SetRTMode(CtrlProgramOptionsClass.RelayTimeModeType.R_T_M_SIMPLE);

            ReCalculateEventsInLists(ProgSelected);
        }       
 
        private void SetRTMode(CtrlProgramOptionsClass.RelayTimeModeType Mode)
        {            
            CtrlProgramsOptions[ProgSelected].RelayTimeMode = Mode;

            ReCalculateEventsInLists(ProgSelected);
        }

        //============= события выбора дополнительных настроек реле времени
        private void checkBox1_Click(object sender, EventArgs e)
        {
            CtrlProgramsOptions[ProgSelected].AllowDaysoffs = checkBox1.Checked;
        }

        private void checkBox2_Click(object sender, EventArgs e)
        {
            CtrlProgramsOptions[ProgSelected].AllowHolidays = checkBox2.Checked;
        }

        private void checkBox3_Click(object sender, EventArgs e)
        {
            CtrlProgramsOptions[ProgSelected].AllowCyclicity = checkBox3.Checked;
        }


        //============= события выбора выходного дня
        private void checkBox4_Click(object sender, EventArgs e)
        {
            CtrlProgramsOptions[ProgSelected].ExceptDaysOfWeek[1] = checkBox4.Checked;
        }

        private void checkBox5_Click(object sender, EventArgs e)
        {
            CtrlProgramsOptions[ProgSelected].ExceptDaysOfWeek[2] = checkBox5.Checked;
        }

        private void checkBox6_Click(object sender, EventArgs e)
        {
            CtrlProgramsOptions[ProgSelected].ExceptDaysOfWeek[3] = checkBox6.Checked;
        }

        private void checkBox7_Click(object sender, EventArgs e)
        {
            CtrlProgramsOptions[ProgSelected].ExceptDaysOfWeek[4] = checkBox7.Checked;
        }

        private void checkBox8_Click(object sender, EventArgs e)
        {
            CtrlProgramsOptions[ProgSelected].ExceptDaysOfWeek[5] = checkBox8.Checked;
        }

        private void checkBox9_Click(object sender, EventArgs e)
        {
            CtrlProgramsOptions[ProgSelected].ExceptDaysOfWeek[6] = checkBox9.Checked;
        }

        private void checkBox10_Click(object sender, EventArgs e)
        {
            CtrlProgramsOptions[ProgSelected].ExceptDaysOfWeek[7] = checkBox10.Checked;
        }

        // кнопка добавление нового элемента в список праздников текущей управл. программы
        private void button2_Click(object sender, EventArgs e)
        {
            SaveDataForBack();

            TimerClass newItem = new TimerClass(new DateTime(1996, monthCalendar1.SelectionStart.Month, monthCalendar1.SelectionStart.Day), false);
            CtrlProgramsOptions[ProgSelected].ListHolidays.AddSmart(newItem, 0);
                        
            ReDrawListOfHolidays();            
        }

        // перерисовывает список праздников текущей программы в ListView
        private void ReDrawListOfHolidays()
        {
            listView1.Items.Clear();        // очистить весь список
            int i = 0;
            foreach (TimerClass Holiday in CtrlProgramsOptions[ProgSelected].ListHolidays)
            {                
                listView1.Items.Add(string.Format("{0}", i + 1));                
                listView1.Items[i].SubItems.Add(Holiday.DateAndTime.Day.ToString());
                char ch = ' ';      // разделитель для вычленения названия месяца
                //listView1.Items[i].SubItems.Add(Holiday.DateAndTime.ToString("M").Split(ch)[0]);  // также возвращает только месяц, но с измененным окончанием
                listView1.Items[i].SubItems.Add(Holiday.DateAndTime.ToString("MMMM"));
                i++;
            }

            //ReCalculateEventsInLists();
            ReCalculateEventsInLists(ProgSelected);
        }

        // перерисовать список исключительных событий в ListView
        private void ReDrawListOfEventsExceptions()
        {
            listView2.Items.Clear();
            int i = 0;
            //CtrlProgramsOptions[ProgSelected].ListEventsException.Add(new TimerClass(new System.DateTime(2009, 1, 1, 23, 15, 45), true));
            foreach (TimerClass EventExcept in CtrlProgramsOptions[ProgSelected].ListEventsException)
            {
                listView2.Items.Add(string.Format("{0}", i + 1));
                listView2.Items[i].SubItems.Add(EventExcept.DateAndTime.ToString("HH:mm:ss"));
                if (EventExcept.Condition == true)
                {
                    listView2.Items[i].SubItems.Add(stClose[LangGlobal]);
                }
                else
                {
                    listView2.Items[i].SubItems.Add(stOpen[LangGlobal]);
                }
                i++;
            }

            //ReCalculateEventsInLists();
            ReCalculateEventsInLists(ProgSelected);
        }

        // перерисовать список годовых событий
        private void ReDrawListOfEventsYear()
        {
            //char ch = ' ';      // разделитель для вычленения названия месяца

            //listView3.Items.Clear();
            //int i = 0;
            ////CtrlProgramsOptions[ProgSelected].ListEventsYear.Add(new TimerClass(new System.DateTime(2009, 1, 1, 23, 15, 45), true));
            //foreach (TimerClass Event in CtrlProgramsOptions[ProgSelected].ListEventsYear)
            //{
            //    if(listView3.Items.Count < i+1)
            //        listView3.Items.Add(string.Format("{0}", i + 1));
                
            //    listView3.Items[i].SubItems.Add(Event.DateAndTime.Day.ToString());
            //    listView3.Items[i].SubItems.Add(Event.DateAndTime.ToString("M").Split(ch)[0]);  // также возвращает только месяц, но с измененным окончанием
            //    listView3.Items[i].SubItems.Add(Event.DateAndTime.ToString("HH:mm:ss"));
            //    if (Event.Condition == true)
            //    {
            //        listView3.Items[i].SubItems.Add("Включить");
            //    }
            //    else
            //    {
            //        listView3.Items[i].SubItems.Add("Отключить");
            //    }
            //    i++;
            //}
            //while (i < listView3.Items.Count)
            //{
            //    listView3.Items.RemoveAt(i);        // удаление оставшихся элементов, если такие есть
            //}






            char ch = ' ';      // разделитель для вычленения названия месяца

            listView3.Items.Clear();
            int i = 0;
            //CtrlProgramsOptions[ProgSelected].ListEventsYear.Add(new TimerClass(new System.DateTime(2009, 1, 1, 23, 15, 45), true));
            foreach (TimerClass Event in CtrlProgramsOptions[ProgSelected].ListEventsYear)
            {
                listView3.Items.Add(string.Format("{0}", i + 1));
                listView3.Items[i].SubItems.Add(Event.DateAndTime.Day.ToString());
                //listView3.Items[i].SubItems.Add(Event.DateAndTime.ToString("M").Split(ch)[0]);  // также возвращает только месяц, но с измененным окончанием
                listView3.Items[i].SubItems.Add(Event.DateAndTime.ToString("MMMM"));  // также возвращает только месяц, но с измененным окончанием
                listView3.Items[i].SubItems.Add(Event.DateAndTime.ToString("HH:mm:ss"));
                if (Event.Condition == true)
                {
                    listView3.Items[i].SubItems.Add(stClose[LangGlobal]);
                }
                else
                {
                    listView3.Items[i].SubItems.Add(stOpen[LangGlobal]);
                }
                i++;
            }

            //ReCalculateEventsInLists();
            ReCalculateEventsInLists(ProgSelected);
        }

        // перерисовать список месячных событий
        private void ReDrawListOfEventsMonth()
        {
            listView4.Items.Clear();
            int i = 0;
            //CtrlProgramsOptions[ProgSelected].ListEventsYear.Add(new TimerClass(new System.DateTime(2009, 1, 1, 23, 15, 45), true));
            foreach (TimerClass Event in CtrlProgramsOptions[ProgSelected].ListEventsMonth)
            {
                listView4.Items.Add(string.Format("{0}", i + 1));
                listView4.Items[i].SubItems.Add(Event.DateAndTime.Day.ToString());                
                listView4.Items[i].SubItems.Add(Event.DateAndTime.ToString("HH:mm:ss"));
                if (Event.Condition == true)
                {
                    listView4.Items[i].SubItems.Add(stClose[LangGlobal]);
                }
                else
                {
                    listView4.Items[i].SubItems.Add(stOpen[LangGlobal]);
                }
                i++;
            }

            //ReCalculateEventsInLists();
            ReCalculateEventsInLists(ProgSelected);
        }

        // перерисовать список недельных событий
        private void ReDrawListOfEventsWeek()
        {
            listView5.Items.Clear();
            int i = 0;
            //CtrlProgramsOptions[ProgSelected].ListEventsYear.Add(new TimerClass(new System.DateTime(2009, 1, 1, 23, 15, 45), true));
            foreach (TimerClass Event in CtrlProgramsOptions[ProgSelected].ListEventsWeek)
            {
                listView5.Items.Add(string.Format("{0}", i + 1));
                listView5.Items[i].SubItems.Add(StDaysOfWeek[Event.DateAndTime.Day - 1]);
                listView5.Items[i].SubItems.Add(Event.DateAndTime.ToString("HH:mm:ss"));
                if (Event.Condition == true)
                {
                    listView5.Items[i].SubItems.Add(stClose[LangGlobal]);
                }
                else
                {
                    listView5.Items[i].SubItems.Add(stOpen[LangGlobal]);
                }
                i++;
            }

            //ReCalculateEventsInLists();
            ReCalculateEventsInLists(ProgSelected);
        }

        //перерисовать список суточных событий
        private void ReDrawListOfEventsDay()
        {
            listView6.Items.Clear();
            int i = 0;
            //CtrlProgramsOptions[ProgSelected].ListEventsYear.Add(new TimerClass(new System.DateTime(2009, 1, 1, 23, 15, 45), true));
            foreach (TimerClass Event in CtrlProgramsOptions[ProgSelected].ListEventsDay)
            {
                listView6.Items.Add(string.Format("{0}", i + 1));                
                listView6.Items[i].SubItems.Add(Event.DateAndTime.ToString("HH:mm:ss"));
                if (Event.Condition == true)
                {
                    listView6.Items[i].SubItems.Add(stClose[LangGlobal]);
                }
                else
                {
                    listView6.Items[i].SubItems.Add(stOpen[LangGlobal]);
                }
                i++;
            }

            //ReCalculateEventsInLists();
            ReCalculateEventsInLists(ProgSelected);
        }

        // удалить все праздники
        private void button4_Click(object sender, EventArgs e)
        {
            SaveDataForBack();
            
            CtrlProgramsOptions[ProgSelected].ListHolidays.Clear();
            ReDrawListOfHolidays();     // перерисовать список праздников
        }

        // удалить выделенные праздники
        private void button3_Click(object sender, EventArgs e)
        {
            SaveDataForBack();
            
            ListView.SelectedIndexCollection indexes = listView1.SelectedIndices;            
            
            //for( int i = indexes.Count; i >= 0; i--)            
            //{
            //    CtrlProgramsOptions[ProgSelected].ListHolidays.RemoveAt((int)indexes[i]);
            //}
            int n = 0;
            foreach (int index in indexes)
            {                 
                CtrlProgramsOptions[ProgSelected].ListHolidays.RemoveAt(index - n);
                n++;
            }

            ReDrawListOfHolidays();     // перерисовать список праздников
        }

        // кнопка добавления нового события в список исключительных событий
        private void button7_Click(object sender, EventArgs e)
        {
            SaveDataForBack();
            
            TimerClass newItem = new TimerClass(new DateTime(1996, 1, 1, (int)numericUpDown1.Value, (int)numericUpDown2.Value, (int)numericUpDown3.Value), radioButton18.Checked);
            CtrlProgramsOptions[ProgSelected].ListEventsException.AddSmart(newItem, 1);
                        
            ReDrawListOfEventsExceptions();

            //=======================================
            
            
            //bool AlreadyExist = false;
            //DateTime dNew, dItem;       // времена для выполнения операции сравнения и присвоения
            //bool newCondition, itemCondition;

            //// проверки выхода из допустимого диапазона
            //if (numericUpDown1.Value < 0 && numericUpDown1.Value > 23)
            //    numericUpDown1.Value = 0;
            //if (numericUpDown2.Value < 0 && numericUpDown2.Value > 59)
            //    numericUpDown2.Value = 0;
            //if (numericUpDown3.Value < 0 && numericUpDown3.Value > 59)
            //    numericUpDown3.Value = 0;
                        
            //// получение нового добавляемого события
            //dNew = new DateTime(1996, 1, 1, (int)numericUpDown1.Value, (int)numericUpDown2.Value, (int)numericUpDown3.Value);
            //newCondition = radioButton18.Checked;       // описывает состояние контактов

            //int k = 0;
            //foreach (TimerClass EventExept in CtrlProgramsOptions[ProgSelected].ListEventsException)
            //{
            //    dItem = new DateTime(1996, 1, 1, EventExept.DateAndTime.Hour, EventExept.DateAndTime.Minute, EventExept.DateAndTime.Second);
            //    itemCondition = EventExept.Condition;
            //    if (DateTime.Compare(dNew, dItem) == 0)
            //    {
            //        // если время совпало
            //        if (itemCondition == newCondition)
            //        {
            //            // точно такой же таймер уже существует
            //            AlreadyExist = true;
            //            // если такой праздник уже есть в списке
            //            MessageBox.Show("Событие уже уже есть в списке!",
            //                            "Сообщение", MessageBoxButtons.OK,
            //                            MessageBoxIcon.Asterisk);
            //        }
            //        else
            //        {
            //            // если новый таймер отличается только состоянием контактов, то удалить его, а затем создать снова
            //            //CtrlProgramsOptions[ProgSelected].ListEventsException.Remove(EventExept);   // удалить
            //            AlreadyExist = true;
            //            // изменить состояние контактов
            //            CtrlProgramsOptions[ProgSelected].ListEventsException[k].Condition = !CtrlProgramsOptions[ProgSelected].ListEventsException[k].Condition;
            //            ReDrawListOfEventsExceptions();     // перерисовать список исключительных событий
            //        }
            //    }
            //    k++;
            //}

            //if (AlreadyExist == false)
            //{
            //    // если в коллекции такого таймера нет
            //    // кол-во уже существующих праздников
            //    int imax = CtrlProgramsOptions[ProgSelected].ListEventsException.Count;
            //    CtrlProgramsOptions[ProgSelected].ListEventsException.Add(new TimerClass());       // создание пустого элемента коллекции
            //    if (imax != 0)
            //    {
            //        // если в коллекции уже есть элементы
            //        for (int i = imax - 1; i >= 0; i--)
            //        {
            //            // копирование и приведение текущего пункта
            //            dItem = new DateTime(1996, 1, 1, CtrlProgramsOptions[ProgSelected].ListEventsException[i].DateAndTime.Hour, CtrlProgramsOptions[ProgSelected].ListEventsException[i].DateAndTime.Minute, CtrlProgramsOptions[ProgSelected].ListEventsException[i].DateAndTime.Second);
            //            itemCondition = CtrlProgramsOptions[ProgSelected].ListEventsException[i].Condition;
            //            if (DateTime.Compare(dNew, dItem) > 0)
            //            {
            //                // если новый праздник больше текущего пункта
            //                CtrlProgramsOptions[ProgSelected].ListEventsException[i + 1].DateAndTime = dNew;
            //                CtrlProgramsOptions[ProgSelected].ListEventsException[i + 1].Condition = newCondition;
            //                break;
            //            }
            //            else
            //            {
            //                // если новый праздник меньше текущего
            //                // то скопировать текущий в позицию выше
            //                CtrlProgramsOptions[ProgSelected].ListEventsException[i + 1].DateAndTime = dItem;
            //                CtrlProgramsOptions[ProgSelected].ListEventsException[i + 1].Condition = itemCondition;
            //                if (i == 0)
            //                {
            //                    // если это был первый элемент в списке и послдений проверяемый
            //                    // то скопировать на его место новый элемент
            //                    CtrlProgramsOptions[ProgSelected].ListEventsException[i].DateAndTime = dNew;
            //                    CtrlProgramsOptions[ProgSelected].ListEventsException[i].Condition = newCondition;
            //                }
            //            }
            //        }
            //    }
            //    else
            //    {
            //        // если в коллекции только один элемент, только что добавленный                    
            //        CtrlProgramsOptions[ProgSelected].ListEventsException[0].DateAndTime = dNew;
            //        CtrlProgramsOptions[ProgSelected].ListEventsException[0].Condition = newCondition;
            //    }
            //    ReDrawListOfEventsExceptions();     // перерисовать список исключительных событий
            //}
            //else
            //{
                
            //}
        }

        // очистка списка исключительных событий
        private void button5_Click(object sender, EventArgs e)
        {
            SaveDataForBack();
            
            CtrlProgramsOptions[ProgSelected].ListEventsException.Clear();
            ReDrawListOfEventsExceptions();     // перерисовать список            
        }

        // удаление выделенных элементов
        private void button6_Click(object sender, EventArgs e)
        {
            SaveDataForBack();
            
            ListView.SelectedIndexCollection indexes = listView2.SelectedIndices;
                        
            int n = 0;
            foreach (int index in indexes)
            {
                CtrlProgramsOptions[ProgSelected].ListEventsException.RemoveAt(index - n);
                n++;
            }
            ReDrawListOfEventsExceptions();     // перерисовать список   
        }

        // кнопка добавления нового события в список годовых событий
        private void button10_Click(object sender, EventArgs e)
        {
            SaveDataForBack();
            
            TimerClass newItem = new TimerClass(new DateTime(1996, monthCalendar2.SelectionStart.Month, monthCalendar2.SelectionStart.Day, (int)numericUpDown6.Value, (int)numericUpDown5.Value, (int)numericUpDown4.Value), radioButton19.Checked);
            CtrlProgramsOptions[ProgSelected].ListEventsYear.AddSmart(newItem, 2);                      
            
            ReDrawListOfEventsYear();
            
            //=======================================
            //=======================================
            
            //bool AlreadyExist = false;
            //DateTime dNew, dItem;       // времена для выполнения операции сравнения и присвоения
            //bool newCondition, itemCondition;

            ///*
            //// проверки выхода из допустимого диапазона
            //if (numericUpDown1.Value < 0 && numericUpDown1.Value > 23)
            //    numericUpDown1.Value = 0;
            //if (numericUpDown2.Value < 0 && numericUpDown2.Value > 59)
            //    numericUpDown2.Value = 0;
            //if (numericUpDown3.Value < 0 && numericUpDown3.Value > 59)
            //    numericUpDown3.Value = 0;
            //*/

            //// получение нового добавляемого события
            //dNew = new DateTime(1996, monthCalendar2.SelectionStart.Month, monthCalendar2.SelectionStart.Day, (int)numericUpDown6.Value, (int)numericUpDown5.Value, (int)numericUpDown4.Value);
            //newCondition = radioButton19.Checked;       // описывает состояние контактов

            //int k = 0;
            //foreach (TimerClass Event in CtrlProgramsOptions[ProgSelected].ListEventsYear)
            //{
            //    dItem = new DateTime(1996, Event.DateAndTime.Month, Event.DateAndTime.Day, Event.DateAndTime.Hour, Event.DateAndTime.Minute, Event.DateAndTime.Second);
            //    itemCondition = Event.Condition;
                
            //    if (DateTime.Compare(dNew, dItem) == 0)
            //    {
            //        // если время совпало
            //        if (itemCondition == newCondition)
            //        {
            //            // точно такой же таймер уже существует
            //            AlreadyExist = true;
            //            // если такой праздник уже есть в списке
            //            MessageBox.Show("Событие уже уже есть в списке!",
            //                            "Сообщение", MessageBoxButtons.OK,
            //                            MessageBoxIcon.Asterisk);
            //        }
            //        else
            //        {
            //            // если новый таймер отличается только состоянием контактов, то удалить его, а затем создать снова
            //            //CtrlProgramsOptions[ProgSelected].ListEventsException.Remove(EventExept);   // удалить
            //            AlreadyExist = true;
            //            // изменить состояние контактов
            //            CtrlProgramsOptions[ProgSelected].ListEventsYear[k].Condition = !CtrlProgramsOptions[ProgSelected].ListEventsYear[k].Condition;
            //            ReDrawListOfEventsYear();
            //        }
            //    }
            //    k++;
            //}

            //if (AlreadyExist == false)
            //{
            //    // если в коллекции такого таймера нет
            //    // кол-во уже существующих праздников
            //    int imax = CtrlProgramsOptions[ProgSelected].ListEventsYear.Count;
            //    CtrlProgramsOptions[ProgSelected].ListEventsYear.Add(new TimerClass());       // создание пустого элемента коллекции
            //    if (imax != 0)
            //    {
            //        // если в коллекции уже есть элементы
            //        for (int i = imax - 1; i >= 0; i--)
            //        {
            //            // копирование и приведение текущего пункта
            //            dItem = CtrlProgramsOptions[ProgSelected].ListEventsYear[i].DateAndTime;
            //            //dItem = new DateTime(1996, CtrlProgramsOptions[ProgSelected].ListEventsYear[i].DateAndTime.Month, CtrlProgramsOptions[ProgSelected].ListEventsYear[i].DateAndTime.Day, CtrlProgramsOptions[ProgSelected].ListEventsYear[i].DateAndTime.Hour, CtrlProgramsOptions[ProgSelected].ListEventsYear[i].DateAndTime.Minute, CtrlProgramsOptions[ProgSelected].ListEventsYear[i].DateAndTime.Second);
            //            dItem = new DateTime(1996, dItem.Month, dItem.Day, dItem.Hour, dItem.Minute, dItem.Second);
            //            itemCondition = CtrlProgramsOptions[ProgSelected].ListEventsYear[i].Condition;
            //            if (DateTime.Compare(dNew, dItem) > 0)
            //            {
            //                // если новый праздник больше текущего пункта
            //                CtrlProgramsOptions[ProgSelected].ListEventsYear[i + 1].DateAndTime = dNew;
            //                CtrlProgramsOptions[ProgSelected].ListEventsYear[i + 1].Condition = newCondition;
            //                break;
            //            }
            //            else
            //            {
            //                // если новый праздник меньше текущего
            //                // то скопировать текущий в позицию выше
            //                CtrlProgramsOptions[ProgSelected].ListEventsYear[i + 1].DateAndTime = dItem;
            //                CtrlProgramsOptions[ProgSelected].ListEventsYear[i + 1].Condition = itemCondition;
            //                if (i == 0)
            //                {
            //                    // если это был первый элемент в списке и послдений проверяемый
            //                    // то скопировать на его место новый элемент
            //                    CtrlProgramsOptions[ProgSelected].ListEventsYear[i].DateAndTime = dNew;
            //                    CtrlProgramsOptions[ProgSelected].ListEventsYear[i].Condition = newCondition;
            //                }
            //            }
            //        }
            //    }
            //    else
            //    {
            //        // если в коллекции только один элемент, только что добавленный                    
            //        CtrlProgramsOptions[ProgSelected].ListEventsYear[0].DateAndTime = dNew;
            //        CtrlProgramsOptions[ProgSelected].ListEventsYear[0].Condition = newCondition;
            //    }                
            //    ReDrawListOfEventsYear();
            //}
            //else
            //{

            //}
        }

        // очистка списка годовых событий
        private void button8_Click(object sender, EventArgs e)
        {
            SaveDataForBack();
            
            CtrlProgramsOptions[ProgSelected].ListEventsYear.Clear();            
            ReDrawListOfEventsYear();
        }

        // удалить выделенные годовые события
        private void button9_Click(object sender, EventArgs e)
        {
            SaveDataForBack();
            
            ListView.SelectedIndexCollection indexes = listView3.SelectedIndices;

            int n = 0;
            foreach (int index in indexes)
            {
                CtrlProgramsOptions[ProgSelected].ListEventsYear.RemoveAt(index - n);
                n++;
            }
            ReDrawListOfEventsYear();
        }

        // очистить месячные события
        private void button11_Click(object sender, EventArgs e)
        {
            SaveDataForBack();
            
            CtrlProgramsOptions[ProgSelected].ListEventsMonth.Clear();
            ReDrawListOfEventsMonth();
        }

        // очистить недельные события
        private void button14_Click(object sender, EventArgs e)
        {
            SaveDataForBack();
            
            CtrlProgramsOptions[ProgSelected].ListEventsWeek.Clear();
            ReDrawListOfEventsWeek();
        }

        // очистить суточные события
        private void button17_Click(object sender, EventArgs e)
        {
            SaveDataForBack();
            
            CtrlProgramsOptions[ProgSelected].ListEventsDay.Clear();
            ReDrawListOfEventsDay();
        }

        // удалить выделенные месячные события
        private void button12_Click(object sender, EventArgs e)
        {
            SaveDataForBack();
            
            ListView.SelectedIndexCollection indexes = listView4.SelectedIndices;

            int n = 0;
            foreach (int index in indexes)
            {
                CtrlProgramsOptions[ProgSelected].ListEventsMonth.RemoveAt(index - n);
                n++;
            }
            ReDrawListOfEventsMonth();
        }

        // удалить выделенные недельные события
        private void button15_Click(object sender, EventArgs e)
        {
            SaveDataForBack();
            
            ListView.SelectedIndexCollection indexes = listView5.SelectedIndices;

            int n = 0;
            foreach (int index in indexes)
            {
                CtrlProgramsOptions[ProgSelected].ListEventsWeek.RemoveAt(index - n);
                n++;
            }
            ReDrawListOfEventsWeek();
        }

        // удалить выделенные суточные события
        private void button18_Click(object sender, EventArgs e)
        {
            SaveDataForBack();
            
            ListView.SelectedIndexCollection indexes = listView6.SelectedIndices;

            int n = 0;
            foreach (int index in indexes)
            {
                CtrlProgramsOptions[ProgSelected].ListEventsDay.RemoveAt(index - n);
                n++;
            }
            ReDrawListOfEventsDay();
        }

        // добавить элемент в список месячных событий
        private void button13_Click(object sender, EventArgs e)
        {
            SaveDataForBack();
            
            TimerClass newItem = new TimerClass(new DateTime(1996, 1, (int)numericUpDown16.Value, (int)numericUpDown9.Value, (int)numericUpDown8.Value, (int)numericUpDown7.Value), radioButton21.Checked);
            CtrlProgramsOptions[ProgSelected].ListEventsMonth.AddSmart(newItem, 3);

            ReDrawListOfEventsMonth();

            //=======================================
            
            
            
            //bool AlreadyExist = false;
            //DateTime dNew, dItem;       // времена для выполнения операции сравнения и присвоения
            //bool newCondition, itemCondition;

            ///*
            //// проверки выхода из допустимого диапазона
            //if (numericUpDown1.Value < 0 && numericUpDown1.Value > 23)
            //    numericUpDown1.Value = 0;
            //if (numericUpDown2.Value < 0 && numericUpDown2.Value > 59)
            //    numericUpDown2.Value = 0;
            //if (numericUpDown3.Value < 0 && numericUpDown3.Value > 59)
            //    numericUpDown3.Value = 0;
            //*/

            //// получение нового добавляемого события
            //dNew = new DateTime(1996, 1, (int)numericUpDown16.Value, (int)numericUpDown9.Value, (int)numericUpDown8.Value, (int)numericUpDown7.Value);
            //newCondition = radioButton21.Checked;       // описывает состояние контактов

            //int k = 0;
            //foreach (TimerClass Event in CtrlProgramsOptions[ProgSelected].ListEventsMonth)
            //{
            //    dItem = new DateTime(1996, 1, Event.DateAndTime.Day, Event.DateAndTime.Hour, Event.DateAndTime.Minute, Event.DateAndTime.Second);
            //    itemCondition = Event.Condition;

            //    if (DateTime.Compare(dNew, dItem) == 0)
            //    {
            //        // если время совпало
            //        if (itemCondition == newCondition)
            //        {
            //            // точно такой же таймер уже существует
            //            AlreadyExist = true;
            //            // если такой праздник уже есть в списке
            //            MessageBox.Show("Событие уже уже есть в списке!",
            //                            "Сообщение", MessageBoxButtons.OK,
            //                            MessageBoxIcon.Asterisk);
            //        }
            //        else
            //        {
            //            // если новый таймер отличается только состоянием контактов, то удалить его, а затем создать снова
            //            //CtrlProgramsOptions[ProgSelected].ListEventsException.Remove(EventExept);   // удалить
            //            AlreadyExist = true;
            //            // изменить состояние контактов
            //            CtrlProgramsOptions[ProgSelected].ListEventsMonth[k].Condition = !CtrlProgramsOptions[ProgSelected].ListEventsMonth[k].Condition;
            //            ReDrawListOfEventsMonth();
            //        }
            //    }
            //    k++;
            //}

            //if (AlreadyExist == false)
            //{
            //    // если в коллекции такого таймера нет
            //    // кол-во уже существующих праздников
            //    int imax = CtrlProgramsOptions[ProgSelected].ListEventsMonth.Count;
            //    CtrlProgramsOptions[ProgSelected].ListEventsMonth.Add(new TimerClass());       // создание пустого элемента коллекции
            //    if (imax != 0)
            //    {
            //        // если в коллекции уже есть элементы
            //        for (int i = imax - 1; i >= 0; i--)
            //        {
            //            // копирование и приведение текущего пункта
            //            dItem = CtrlProgramsOptions[ProgSelected].ListEventsMonth[i].DateAndTime;
            //            //dItem = new DateTime(1996, CtrlProgramsOptions[ProgSelected].ListEventsYear[i].DateAndTime.Month, CtrlProgramsOptions[ProgSelected].ListEventsYear[i].DateAndTime.Day, CtrlProgramsOptions[ProgSelected].ListEventsYear[i].DateAndTime.Hour, CtrlProgramsOptions[ProgSelected].ListEventsYear[i].DateAndTime.Minute, CtrlProgramsOptions[ProgSelected].ListEventsYear[i].DateAndTime.Second);
            //            dItem = new DateTime(1996, 1, dItem.Day, dItem.Hour, dItem.Minute, dItem.Second);
            //            itemCondition = CtrlProgramsOptions[ProgSelected].ListEventsMonth[i].Condition;
            //            if (DateTime.Compare(dNew, dItem) > 0)
            //            {
            //                // если новый праздник больше текущего пункта
            //                CtrlProgramsOptions[ProgSelected].ListEventsMonth[i + 1].DateAndTime = dNew;
            //                CtrlProgramsOptions[ProgSelected].ListEventsMonth[i + 1].Condition = newCondition;
            //                break;
            //            }
            //            else
            //            {
            //                // если новый праздник меньше текущего
            //                // то скопировать текущий в позицию выше
            //                CtrlProgramsOptions[ProgSelected].ListEventsMonth[i + 1].DateAndTime = dItem;
            //                CtrlProgramsOptions[ProgSelected].ListEventsMonth[i + 1].Condition = itemCondition;
            //                if (i == 0)
            //                {
            //                    // если это был первый элемент в списке и послдений проверяемый
            //                    // то скопировать на его место новый элемент
            //                    CtrlProgramsOptions[ProgSelected].ListEventsMonth[i].DateAndTime = dNew;
            //                    CtrlProgramsOptions[ProgSelected].ListEventsMonth[i].Condition = newCondition;
            //                }
            //            }
            //        }
            //    }
            //    else
            //    {
            //        // если в коллекции только один элемент, только что добавленный                    
            //        CtrlProgramsOptions[ProgSelected].ListEventsMonth[0].DateAndTime = dNew;
            //        CtrlProgramsOptions[ProgSelected].ListEventsMonth[0].Condition = newCondition;
            //    }
            //    ReDrawListOfEventsMonth();
            //}
            //else
            //{

            //}
        }

        // добавить элемент списка недельных событий
        private void button16_Click(object sender, EventArgs e)
        {
            SaveDataForBack();

            TimerClass newItem = new TimerClass(new DateTime(1996, 1, comboBox1.SelectedIndex + 1, (int)numericUpDown14.Value, (int)numericUpDown13.Value, (int)numericUpDown12.Value), radioButton23.Checked);
            CtrlProgramsOptions[ProgSelected].ListEventsWeek.AddSmart(newItem, 4);

            ReDrawListOfEventsWeek();

            //=======================================


            //bool AlreadyExist = false;
            //DateTime dNew, dItem;       // времена для выполнения операции сравнения и присвоения
            //bool newCondition, itemCondition;
            ////int newDayOfWeek, itemDayOfWeek;

            ///*
            //// проверки выхода из допустимого диапазона
            //if (numericUpDown1.Value < 0 && numericUpDown1.Value > 23)
            //    numericUpDown1.Value = 0;
            //if (numericUpDown2.Value < 0 && numericUpDown2.Value > 59)
            //    numericUpDown2.Value = 0;
            //if (numericUpDown3.Value < 0 && numericUpDown3.Value > 59)
            //    numericUpDown3.Value = 0;
            //*/

            //if (comboBox1.SelectedIndex < 0)
            //    comboBox1.SelectedIndex = 0;

            //// получение нового добавляемого события
            //        // используется 1996 год для сравнения времения, т.к. 1 января этого года начинается с понедельника
            //dNew = new DateTime(1996, 1, comboBox1.SelectedIndex + 1, (int)numericUpDown14.Value, (int)numericUpDown13.Value, (int)numericUpDown12.Value);
            ////newDayOfWeek = comboBox1.SelectedIndex;
            //newCondition = radioButton23.Checked;       // описывает состояние контактов

            //int k = 0;
            //foreach (TimerClass Event in CtrlProgramsOptions[ProgSelected].ListEventsWeek)
            //{
            //    dItem = new DateTime(1996, 1, Event.DateAndTime.Day, Event.DateAndTime.Hour, Event.DateAndTime.Minute, Event.DateAndTime.Second);
            //    //itemDayOfWeek = Event.DayOfWeek;
            //    itemCondition = Event.Condition;
                                
            //    if (DateTime.Compare(dNew, dItem) == 0)
            //    {
            //        // если время совпало
            //        if (itemCondition == newCondition)
            //        {
            //            // точно такой же таймер уже существует
            //            AlreadyExist = true;
            //            // если такой праздник уже есть в списке
            //            MessageBox.Show("Событие уже уже есть в списке!",
            //                            "Сообщение", MessageBoxButtons.OK,
            //                            MessageBoxIcon.Asterisk);
            //        }
            //        else
            //        {
            //            // если новый таймер отличается только состоянием контактов, то удалить его, а затем создать снова
            //            //CtrlProgramsOptions[ProgSelected].ListEventsException.Remove(EventExept);   // удалить
            //            AlreadyExist = true;
            //            // изменить состояние контактов
            //            CtrlProgramsOptions[ProgSelected].ListEventsWeek[k].Condition = !CtrlProgramsOptions[ProgSelected].ListEventsWeek[k].Condition;
            //            ReDrawListOfEventsWeek();
            //        }
            //    }
            //    k++;
            //}

            //if (AlreadyExist == false)
            //{
            //    // если в коллекции такого таймера нет
            //    // кол-во уже существующих праздников
            //    int imax = CtrlProgramsOptions[ProgSelected].ListEventsWeek.Count;
            //    CtrlProgramsOptions[ProgSelected].ListEventsWeek.Add(new TimerClass());       // создание пустого элемента коллекции
            //    if (imax != 0)
            //    {
            //        // если в коллекции уже есть элементы
            //        for (int i = imax - 1; i >= 0; i--)
            //        {
            //            // копирование и приведение текущего пункта
            //            dItem = CtrlProgramsOptions[ProgSelected].ListEventsWeek[i].DateAndTime;
            //            //dItem = new DateTime(1996, CtrlProgramsOptions[ProgSelected].ListEventsYear[i].DateAndTime.Month, CtrlProgramsOptions[ProgSelected].ListEventsYear[i].DateAndTime.Day, CtrlProgramsOptions[ProgSelected].ListEventsYear[i].DateAndTime.Hour, CtrlProgramsOptions[ProgSelected].ListEventsYear[i].DateAndTime.Minute, CtrlProgramsOptions[ProgSelected].ListEventsYear[i].DateAndTime.Second);
            //            dItem = new DateTime(1996, 1, dItem.Day, dItem.Hour, dItem.Minute, dItem.Second);
            //            //itemDayOfWeek = CtrlProgramsOptions[ProgSelected].ListEventsWeek[i].DayOfWeek;
            //            itemCondition = CtrlProgramsOptions[ProgSelected].ListEventsWeek[i].Condition;
            //            if (DateTime.Compare(dNew, dItem) > 0)
            //            {
            //                // если новый праздник больше текущего пункта
            //                CtrlProgramsOptions[ProgSelected].ListEventsWeek[i + 1].DateAndTime = dNew;
            //                CtrlProgramsOptions[ProgSelected].ListEventsWeek[i + 1].Condition = newCondition;
            //                break;
            //            }
            //            else
            //            {
            //                // если новый праздник меньше текущего
            //                // то скопировать текущий в позицию выше
            //                CtrlProgramsOptions[ProgSelected].ListEventsWeek[i + 1].DateAndTime = dItem;
            //                CtrlProgramsOptions[ProgSelected].ListEventsWeek[i + 1].Condition = itemCondition;
            //                if (i == 0)
            //                {
            //                    // если это был первый элемент в списке и послдений проверяемый
            //                    // то скопировать на его место новый элемент
            //                    CtrlProgramsOptions[ProgSelected].ListEventsWeek[i].DateAndTime = dNew;
            //                    CtrlProgramsOptions[ProgSelected].ListEventsWeek[i].Condition = newCondition;
            //                }
            //            }
            //        }
            //    }
            //    else
            //    {
            //        // если в коллекции только один элемент, только что добавленный                    
            //        CtrlProgramsOptions[ProgSelected].ListEventsWeek[0].DateAndTime = dNew;
            //        CtrlProgramsOptions[ProgSelected].ListEventsWeek[0].Condition = newCondition;
            //    }
            //    ReDrawListOfEventsWeek();
            //}
            //else
            //{

            //}
        }

        // добавить элемент списка суточных событий
        private void button19_Click(object sender, EventArgs e)
        {
            SaveDataForBack();
            
            TimerClass newItem = new TimerClass(new DateTime(1996, 1, 1, (int)numericUpDown15.Value, (int)numericUpDown11.Value, (int)numericUpDown10.Value), radioButton25.Checked);
            CtrlProgramsOptions[ProgSelected].ListEventsDay.AddSmart(newItem, 5);

            ReDrawListOfEventsDay();

            //=======================================
            
            
            
            //bool AlreadyExist = false;
            //DateTime dNew, dItem;       // времена для выполнения операции сравнения и присвоения
            //bool newCondition, itemCondition;

            ///*
            //// проверки выхода из допустимого диапазона
            //if (numericUpDown1.Value < 0 && numericUpDown1.Value > 23)
            //    numericUpDown1.Value = 0;
            //if (numericUpDown2.Value < 0 && numericUpDown2.Value > 59)
            //    numericUpDown2.Value = 0;
            //if (numericUpDown3.Value < 0 && numericUpDown3.Value > 59)
            //    numericUpDown3.Value = 0;
            //*/

            //// получение нового добавляемого события
            //dNew = new DateTime(1996, 1, 1, (int)numericUpDown15.Value, (int)numericUpDown11.Value, (int)numericUpDown10.Value);
            //newCondition = radioButton25.Checked;       // описывает состояние контактов

            //int k = 0;
            //foreach (TimerClass Event in CtrlProgramsOptions[ProgSelected].ListEventsDay)
            //{
            //    dItem = new DateTime(1996, 1, 1, Event.DateAndTime.Hour, Event.DateAndTime.Minute, Event.DateAndTime.Second);
            //    itemCondition = Event.Condition;

            //    if (DateTime.Compare(dNew, dItem) == 0)
            //    {
            //        // если время совпало
            //        if (itemCondition == newCondition)
            //        {
            //            // точно такой же таймер уже существует
            //            AlreadyExist = true;
            //            // если такой праздник уже есть в списке
            //            MessageBox.Show("Событие уже уже есть в списке!",
            //                            "Сообщение", MessageBoxButtons.OK,
            //                            MessageBoxIcon.Asterisk);
            //        }
            //        else
            //        {
            //            // если новый таймер отличается только состоянием контактов, то удалить его, а затем создать снова
            //            //CtrlProgramsOptions[ProgSelected].ListEventsException.Remove(EventExept);   // удалить
            //            AlreadyExist = true;
            //            // изменить состояние контактов
            //            CtrlProgramsOptions[ProgSelected].ListEventsDay[k].Condition = !CtrlProgramsOptions[ProgSelected].ListEventsDay[k].Condition;
            //            ReDrawListOfEventsDay();
            //        }
            //    }
            //    k++;
            //}

            //if (AlreadyExist == false)
            //{
            //    // если в коллекции такого таймера нет
            //    // кол-во уже существующих праздников
            //    int imax = CtrlProgramsOptions[ProgSelected].ListEventsDay.Count;
            //    CtrlProgramsOptions[ProgSelected].ListEventsDay.Add(new TimerClass());       // создание пустого элемента коллекции
            //    if (imax != 0)
            //    {
            //        // если в коллекции уже есть элементы
            //        for (int i = imax - 1; i >= 0; i--)
            //        {
            //            // копирование и приведение текущего пункта
            //            dItem = CtrlProgramsOptions[ProgSelected].ListEventsDay[i].DateAndTime;
            //            //dItem = new DateTime(1996, CtrlProgramsOptions[ProgSelected].ListEventsYear[i].DateAndTime.Month, CtrlProgramsOptions[ProgSelected].ListEventsYear[i].DateAndTime.Day, CtrlProgramsOptions[ProgSelected].ListEventsYear[i].DateAndTime.Hour, CtrlProgramsOptions[ProgSelected].ListEventsYear[i].DateAndTime.Minute, CtrlProgramsOptions[ProgSelected].ListEventsYear[i].DateAndTime.Second);
            //            dItem = new DateTime(1996, 1, 1, dItem.Hour, dItem.Minute, dItem.Second);
            //            itemCondition = CtrlProgramsOptions[ProgSelected].ListEventsDay[i].Condition;
            //            if (DateTime.Compare(dNew, dItem) > 0)
            //            {
            //                // если новый праздник больше текущего пункта
            //                CtrlProgramsOptions[ProgSelected].ListEventsDay[i + 1].DateAndTime = dNew;
            //                CtrlProgramsOptions[ProgSelected].ListEventsDay[i + 1].Condition = newCondition;
            //                break;
            //            }
            //            else
            //            {
            //                // если новый праздник меньше текущего
            //                // то скопировать текущий в позицию выше
            //                CtrlProgramsOptions[ProgSelected].ListEventsDay[i + 1].DateAndTime = dItem;
            //                CtrlProgramsOptions[ProgSelected].ListEventsDay[i + 1].Condition = itemCondition;
            //                if (i == 0)
            //                {
            //                    // если это был первый элемент в списке и послдений проверяемый
            //                    // то скопировать на его место новый элемент
            //                    CtrlProgramsOptions[ProgSelected].ListEventsDay[i].DateAndTime = dNew;
            //                    CtrlProgramsOptions[ProgSelected].ListEventsDay[i].Condition = newCondition;
            //                }
            //            }
            //        }
            //    }
            //    else
            //    {
            //        // если в коллекции только один элемент, только что добавленный                    
            //        CtrlProgramsOptions[ProgSelected].ListEventsDay[0].DateAndTime = dNew;
            //        CtrlProgramsOptions[ProgSelected].ListEventsDay[0].Condition = newCondition;
            //    }
            //    ReDrawListOfEventsDay();
            //}
            //else
            //{

            //}
        }

        // изменения в настройках значений импульсного реле        
        private void numericUpDown17_ValueChanged(object sender, EventArgs e)
        {
            CtrlProgramsOptions[ProgSelected].RI_BeforeDelay.Minute = (int)numericUpDown17.Value;
        }

        private void numericUpDown18_ValueChanged(object sender, EventArgs e)
        {
            CtrlProgramsOptions[ProgSelected].RI_BeforeDelay.Second = (int)numericUpDown18.Value;
        }

        private void numericUpDown20_ValueChanged(object sender, EventArgs e)
        {
            CtrlProgramsOptions[ProgSelected].RI_OnDelay.Minute = (int)numericUpDown20.Value;
        }

        private void numericUpDown19_ValueChanged(object sender, EventArgs e)
        {
            CtrlProgramsOptions[ProgSelected].RI_OnDelay.Second = (int)numericUpDown19.Value;
        }

        private void numericUpDown22_ValueChanged(object sender, EventArgs e)
        {
            CtrlProgramsOptions[ProgSelected].RI_OffDelay.Minute = (int)numericUpDown22.Value;
        }

        private void numericUpDown21_ValueChanged(object sender, EventArgs e)
        {
            CtrlProgramsOptions[ProgSelected].RI_OffDelay.Second = (int)numericUpDown21.Value;
        }

        // сохранение изменений в настройках простого реле
        private void numericUpDown28_ValueChanged(object sender, EventArgs e)
        {            
            CtrlProgramsOptions[ProgSelected].RS_Delay.Minute = (int)numericUpDown28.Value;            
        }

        private void numericUpDown27_ValueChanged(object sender, EventArgs e)
        {           
            CtrlProgramsOptions[ProgSelected].RS_Delay.Second = (int)numericUpDown27.Value;            
        }

        // сохранение изменений в настройках реле напряжения
        private void radioButton27_CheckedChanged(object sender, EventArgs e)
        {
            CtrlProgramsOptions[ProgSelected].RV_OnOff = !radioButton27.Checked;

            ReCalculateEventsInLists(ProgSelected);
        }

        private void radioButton33_CheckedChanged(object sender, EventArgs e)
        {
            CtrlProgramsOptions[ProgSelected].RV_OnOff = radioButton33.Checked;

            ReCalculateEventsInLists(ProgSelected);
        }

        private void numericUpDown46_ValueChanged(object sender, EventArgs e)
        {
            if (((int)numericUpDown46.Value + CtrlProgramsOptions[ProgSelected].RV_Uminhyst) < (CtrlProgramsOptions[ProgSelected].RV_Umax - CtrlProgramsOptions[ProgSelected].RV_Umaxhyst))
            {
                // если новое значение
                CtrlProgramsOptions[ProgSelected].RV_Umin = (int)numericUpDown46.Value;
            }
            else
            {
                numericUpDown46.Value = numericUpDown46.Value - 1;
                //if (((int)numericUpDown46.Value + CtrlProgramsOptions[ProgSelected].RV_Uminhyst) < (CtrlProgramsOptions[ProgSelected].RV_Umax - CtrlProgramsOptions[ProgSelected].RV_Umaxhyst))
                //{
                //    MessageBox.Show("Нижний порог напряжения с учетом гистерезиса \n не может быть больше верхнего порога напряжения!", "Сообщение", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                //}
            }
            
        }

        private void numericUpDown45_ValueChanged(object sender, EventArgs e)
        {
            if (((int)numericUpDown45.Value + CtrlProgramsOptions[ProgSelected].RV_Umin) < (CtrlProgramsOptions[ProgSelected].RV_Umax - CtrlProgramsOptions[ProgSelected].RV_Umaxhyst))
            {
                // если новое значение
                CtrlProgramsOptions[ProgSelected].RV_Uminhyst = (int)numericUpDown45.Value;
            }
            else
            {
                numericUpDown45.Value = numericUpDown45.Value - 1;                
            }            
        }

        private void numericUpDown30_ValueChanged(object sender, EventArgs e)
        {
            CtrlProgramsOptions[ProgSelected].RV_DelayUmin.Minute = (int)numericUpDown30.Value;
        }

        private void numericUpDown29_ValueChanged(object sender, EventArgs e)
        {
            CtrlProgramsOptions[ProgSelected].RV_DelayUmin.Second = (int)numericUpDown29.Value;
        }

        private void numericUpDown32_ValueChanged(object sender, EventArgs e)
        {
            CtrlProgramsOptions[ProgSelected].RV_DelayUnorm.Minute = (int)numericUpDown32.Value;
        }

        private void numericUpDown31_ValueChanged(object sender, EventArgs e)
        {
            CtrlProgramsOptions[ProgSelected].RV_DelayUnorm.Second = (int)numericUpDown31.Value;
        }

        private void numericUpDown34_ValueChanged(object sender, EventArgs e)
        {
            CtrlProgramsOptions[ProgSelected].RV_DelayUmax.Minute = (int)numericUpDown34.Value;
        }

        private void numericUpDown33_ValueChanged(object sender, EventArgs e)
        {
            CtrlProgramsOptions[ProgSelected].RV_DelayUmax.Second = (int)numericUpDown33.Value;
        }
        
        private void numericUpDown26_ValueChanged(object sender, EventArgs e)
        {
            if ((CtrlProgramsOptions[ProgSelected].RV_Umin + CtrlProgramsOptions[ProgSelected].RV_Uminhyst) < ((int)numericUpDown26.Value - CtrlProgramsOptions[ProgSelected].RV_Umaxhyst))
            {
                // если новое значение
                CtrlProgramsOptions[ProgSelected].RV_Umax = (int)numericUpDown26.Value;
            }
            else
            {
                numericUpDown26.Value = numericUpDown26.Value + 1;
            } 
        }

        private void numericUpDown25_ValueChanged(object sender, EventArgs e)
        {
            if ((CtrlProgramsOptions[ProgSelected].RV_Umin + CtrlProgramsOptions[ProgSelected].RV_Uminhyst) < (CtrlProgramsOptions[ProgSelected].RV_Umax - (int)numericUpDown25.Value))
            {
                // если новое значение
                CtrlProgramsOptions[ProgSelected].RV_Umaxhyst = (int)numericUpDown25.Value;
            }
            else
            {
                numericUpDown25.Value = numericUpDown25.Value - 1;
            }
        }            
 
        // сохранение изменений в настройках фото-реле
        private void radioButton28_CheckedChanged(object sender, EventArgs e)
        {
            CtrlProgramsOptions[ProgSelected].RF_OnOff = !radioButton28.Checked;

            ReCalculateEventsInLists(ProgSelected);
        }

        private void radioButton29_CheckedChanged(object sender, EventArgs e)
        {
            CtrlProgramsOptions[ProgSelected].RF_OnOff = radioButton29.Checked;

            ReCalculateEventsInLists(ProgSelected);
        }

        private void numericUpDown23_ValueChanged(object sender, EventArgs e)
        {
            CtrlProgramsOptions[ProgSelected].RF_Lpor = (int)numericUpDown23.Value;
        }

        private void numericUpDown24_ValueChanged(object sender, EventArgs e)
        {
            CtrlProgramsOptions[ProgSelected].RF_Lporhyst = (int)numericUpDown24.Value;
        }

        private void numericUpDown40_ValueChanged(object sender, EventArgs e)
        {
            CtrlProgramsOptions[ProgSelected].RF_DelayLmin.Minute = (int)numericUpDown40.Value;
        }

        private void numericUpDown39_ValueChanged(object sender, EventArgs e)
        {
            CtrlProgramsOptions[ProgSelected].RF_DelayLmin.Second = (int)numericUpDown39.Value;
        }

        private void numericUpDown38_ValueChanged(object sender, EventArgs e)
        {
            CtrlProgramsOptions[ProgSelected].RF_DelayLmax.Minute = (int)numericUpDown38.Value;
        }

        private void numericUpDown37_ValueChanged(object sender, EventArgs e)
        {
            CtrlProgramsOptions[ProgSelected].RF_DelayLmax.Second = (int)numericUpDown37.Value;
        }

        private void radioButton34_CheckedChanged(object sender, EventArgs e)
        {
            CtrlProgramsOptions[ProgSelected].RF_Condition_Lmin = 0;
        }

        private void radioButton31_CheckedChanged(object sender, EventArgs e)
        {
            CtrlProgramsOptions[ProgSelected].RF_Condition_Lmin = 1;
        }

        private void radioButton35_CheckedChanged(object sender, EventArgs e)
        {
            CtrlProgramsOptions[ProgSelected].RF_Condition_Lmin = 2;
        }

        private void radioButton32_CheckedChanged(object sender, EventArgs e)
        {
            CtrlProgramsOptions[ProgSelected].RF_Condition_Lmin = 3;
        }

        private void radioButton30_CheckedChanged(object sender, EventArgs e)
        {
            CtrlProgramsOptions[ProgSelected].RF_Condition_Lmin = 4;
        }

        private void radioButton40_CheckedChanged(object sender, EventArgs e)
        {
            CtrlProgramsOptions[ProgSelected].RF_Condition_Lmax = 0;
        }

        private void radioButton39_CheckedChanged(object sender, EventArgs e)
        {
            CtrlProgramsOptions[ProgSelected].RF_Condition_Lmax = 1;
        }

        private void radioButton38_CheckedChanged(object sender, EventArgs e)
        {
            CtrlProgramsOptions[ProgSelected].RF_Condition_Lmax = 2;
        }

        private void radioButton37_CheckedChanged(object sender, EventArgs e)
        {
            CtrlProgramsOptions[ProgSelected].RF_Condition_Lmax = 3;
        }

        private void radioButton36_CheckedChanged(object sender, EventArgs e)
        {
            CtrlProgramsOptions[ProgSelected].RF_Condition_Lmax = 4;
        }

        private void numericUpDown42_ValueChanged(object sender, EventArgs e)
        {
            DeviceOptions.CommonDelay.Minute = (int)numericUpDown42.Value;
        }

        private void numericUpDown41_ValueChanged(object sender, EventArgs e)
        {
            DeviceOptions.CommonDelay.Second = (int)numericUpDown41.Value;
        }

        private void checkBox12_CheckedChanged(object sender, EventArgs e)
        {
            DeviceOptions.DST_OnOff = checkBox12.Checked;
        }

        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        //============== методы сохранения и открытия файлов на диске =========================
        private void SaveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveNewFile();
        }

        private void SaveNewFile()
        {
            DialogResult DialogRes;
            DialogRes = saveFileDialog1.ShowDialog();
            if (DialogRes == DialogResult.OK)
            {                                    
                PathOfFile = saveFileDialog1.FileName;
                char ch = '\\';
                char ch1 = '.';
                this.Text = stNovatek[LangGlobal] + ": " + PathOfFile.Split(ch)[PathOfFile.Split(ch).Length - 1].Split(ch1)[0];

                SaveFile();                
            }
            else
            {

            }
        }

        private void SaveFile()
        {
            try
            {
                DataInFileClass2 DataInFile = new DataInFileClass2();
                DataInFile.DeviceOptions = DeviceOptions;
                DataInFile.CtrlProgramsOptions = CtrlProgramsOptions;
                // получить сериализатор
                IFormatter serializer = new BinaryFormatter();
                // сериализировать сохраняемые данные 
                FileStream saveFile = new FileStream(PathOfFile, FileMode.Create, FileAccess.Write);
                //serializer.Serialize(saveFile, DeviceOptions);
                serializer.Serialize(saveFile, DataInFile);
                saveFile.Close();
                //MessageBox.Show("Настройки устройства и управляющих программ успешно сохранены в файле:\n" + PathOfFile,
                //                            "Сообщение", MessageBoxButtons.OK,
                //                            MessageBoxIcon.Asterisk);
            }
            catch
            {
                MessageBox.Show(stNoAccessToDisk[LangGlobal],
                                            stAttention[LangGlobal], MessageBoxButtons.OK,
                                            MessageBoxIcon.Asterisk);
            }
        }

        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFile();
        }

        private void OpenFile()
        {
            DialogResult DialogRes;
            DialogRes = openFileDialog1.ShowDialog();
            if (DialogRes == DialogResult.OK)
            {
                try
                {
                    DataInFileClass2 DataInFile = new DataInFileClass2();     // создать объект того вида, в котором данные сохранены в файл
                    
                    // получить сериализатор
                    IFormatter serializer = new BinaryFormatter();
                    // десериализировать данные из файла
                    FileStream loadFile = new FileStream(openFileDialog1.FileName, FileMode.Open, FileAccess.Read);                    
                    //DeviceOptions = serializer.Deserialize(loadFile) as DeviceOptionsClass;
                    DataInFile = serializer.Deserialize(loadFile) as DataInFileClass2;
                    loadFile.Close();
                    DeviceOptions = DataInFile.DeviceOptions;
                    CtrlProgramsOptions = DataInFile.CtrlProgramsOptions;
                    UpdateAllFormElements();        // обновить всю информацию в окнах  
                    //MessageBox.Show("Настройки устройства и управляющих программ успешно загружены из файла:\n" + PathOfFile, "Сообщение", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    
                    PathOfFile = openFileDialog1.FileName;
                    char ch = '\\';
                    char ch1 = '.';
                    this.Text = stNovatek[LangGlobal] + ": " + PathOfFile.Split(ch)[PathOfFile.Split(ch).Length - 1].Split(ch1)[0];
                }
                catch
                {
                    MessageBox.Show(stNotGoogFile[LangGlobal],
                                                stAttention[LangGlobal], MessageBoxButtons.OK,
                                                MessageBoxIcon.Error);
                }
            }
            else if (DialogRes == DialogResult.Cancel)
            {
            }
            else
            {
                MessageBox.Show(stFileNotFind[LangGlobal], stAttention[LangGlobal], MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
        } 

        private void OpenToolStripButton_Click(object sender, EventArgs e)
        {
            OpenFile();
        }

        private void SaveToolStripButton_Click(object sender, EventArgs e)
        {
            if (PathOfFile == null)
            {
                SaveNewFile();
            }
            else
            {
                SaveFile();
            }
        }

        private void создатьToolStripButton_Click(object sender, EventArgs e)
        {
            // сброс всех настроек
            if (MessageBox.Show(stFileReset[LangGlobal], stAttention[LangGlobal], MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
            {
                DeviceOptions.Reset();
                foreach (CtrlProgramOptionsClass item in CtrlProgramsOptions)
                {
                    item.Reset();
                }
                UpdateAllFormElements();
                this.Text = stNovatek[LangGlobal];
                PathOfFile = null;
            }
        }

        private void сохранитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (PathOfFile == null)
            {
                SaveNewFile();
            }
            else
            {
                SaveFile();
            }
        } 

                                                                        //===================== методы работы с USB ==================================
        private void button26_Click(object sender, EventArgs e)
        {

        }
        
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            usb.RegisterHandle(Handle);
        }

        protected override void WndProc(ref Message m)
        {
            usb.ParseMessages(ref m);
            base.WndProc(ref m);	// pass message on to base form
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                //this.usb.ProductId = Int32.Parse(this.tb_product.Text, System.Globalization.NumberStyles.HexNumber);
                //this.usb.VendorId = Int32.Parse(this.tb_vendor.Text, System.Globalization.NumberStyles.HexNumber);
                this.usb.CheckDevicePresent();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void usb_OnDeviceArrived(object sender, EventArgs e)
        {
            listBox1.Items.Add("Found a Device");
        }

        private void usb_OnDataRecieved(object sender, UsbLibrary.DataRecievedEventArgs args)
        {
                        
            // принятые данные состоят из двух байт, первый из которых всегда равен нулю
            if (InvokeRequired)
            {
                try
                {
                    Invoke(new DataRecievedEventHandler(usb_OnDataRecieved), new object[] { sender, args });
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
            else
            {

                //string rec_data = "Data: ";
                timer1.Enabled = false;
                timer1.Enabled = true;      // сброс таймера

                timer2.Enabled = false;     // остановка таймера для считывания текущего времени
                //int i = 0;
                //foreach (byte myData in args.data)
                //{
                    //if (i == 0)
                    //    continue;       // пропустить первый служебный нулевой байт
                    
                    //label79.Text = myData.ToString();
                    //listBox1.Items.Add(string.Format("Byte #{0}, NumProgOfData #{1}, CountLoops #{2}: {3}", Connection.ByteInReadBlock + 1, Connection.NumProgOfData, Connection.CountLoops, myData));
                    //listBox1.Items.Add(string.Format("Byte #{0}, NumProgOfData #{1}, CountLoops #{2}: {3}", Connection.ByteInSendBlock + 1, Connection.NumProgOfData, Connection.CountLoops, myData));

                    switch (Connection.ReadData(args.data, ref DeviceOptions, ref CtrlProgramsOptions))
                    {
                        case 0:
                            // из функции обработки полученного байта указание ничего не предпринимать
                            //listBox1.Items.Add("Функция GetData вернула 0");
                            //progressBar1.Value = 0;
                            break;
                        case 1:
                            // перерисовать все данные в окнах
                            //listBox1.Items.Add("Функция GetData вернула 1");
                            timer1.Enabled = false;     // отключить таймер
                            progressBar1.Visible = false;
                            progressBar1.Value = 0;
                            label81.Text = StMessageUSB;
                            UpdateAllFormElements();

                            SaveDataInRelay();      // сохранить считанные данные

                            tabControl1.Enabled = true;
                            MessageBox.Show(stSettingSuccessRead[LangGlobal],
                                            stReceivingData[LangGlobal], MessageBoxButtons.OK,
                                            MessageBoxIcon.Asterisk);
                            break;
                        case 2:
                            // продолжать прием
                            label81.Text = "USB: " + stReceivingData[LangGlobal];
                            progressBar1.Value = Connection.ProgressBarPercent;
                            progressBar1.Visible = true;                            
                            // послать разрешение на получение новых данных
                            if (Connection.NewTypeDataSend(ModesSend.CONTINUE) == true)                               
                                MyUsbSendData();      // вызов функции для продолжения передачи разрешенного запроса                                                                                         
                            break;
                        case 3:
                            // продолжать передачу
                            label81.Text = "USB: " + stTransferData[LangGlobal];
                            //progressBar1.Value = (int)(((float)Connection.NumProgOfData / 8) * 100);        // процесс считывания данных
                            progressBar1.Value = Connection.ProgressBarPercent;
                            progressBar1.Visible = true;
                            // послать разрешение на получение новые данных
                            MyUsbSendData();    // вызов функции для передачи новой порции данных
                            break;
                        case 4:
                            // успешная запись настроек
                            timer1.Enabled = false;     // отключить таймер
                            progressBar1.Visible = false;
                            progressBar1.Value = 0;
                            label81.Text = StMessageUSB;

                            SaveDataInRelay();      // сохранить записанные данные

                            tabControl1.Enabled = true;
                            MessageBox.Show(stSettingSuccessTransfer[LangGlobal],
                                            stTransferData[LangGlobal], MessageBoxButtons.OK,
                                            MessageBoxIcon.Asterisk);                            
                            break;
                        case 5:
                            // продолжить прием данных, инициализация нового запроса приема
                            label81.Text = "USB: " + stReceivingData[LangGlobal];                            
                            // послать разрешение на получение новых данных
                            MyUsbSendData();      // вызов функции для посылки подтвеждения
                            label82.Text = Connection.FullBytes.ToString();
                            break;
                        case 6:
                            // ошибка передачи настроек
                            timer1.Enabled = false;     // отключить таймер
                            label81.Text = StMessageUSB;
                            // послать разрешение на получение новых данных
                            tabControl1.Enabled = true;
                            MessageBox.Show(stOverMemory1[LangGlobal] + Connection.EnablePlacesInDevice.ToString() + stOverMemory2[LangGlobal] + Connection.NeededPlaces.ToString() + stOverMemory3[LangGlobal],
                                            stSaveSettings[LangGlobal], MessageBoxButtons.OK,
                                            MessageBoxIcon.Error);
                            //label82.Text = "Exist:" + Connection.EnablePlacesInDevice.ToString() + "  Need:" + Connection.NeededPlaces;
                            break;
                        case 7:
                            // получена версия программы устройства, которую можно считать
                            if (USB_GetSequence == USB_GetSequenceEnum.GET_OPTIONS)
                            {
                                if (CompareVersions(Connection.VersionOfDeviceProgram, Connection.SubVersionOfDeviceProgram) == true)
                                {
                                    // версия поддерживается можно продолжать
                                    if (Connection.NewTypeDataSend(ModesSend.REQUEST_NUM_EVENTS) == true)
                                        MyUsbSendData();      // вызов функции для продолжения передачи разрешенного запроса
                                }
                                else
                                {
                                    timer1.Enabled = false;     // отключить таймер
                                    label81.Text = StMessageUSB;
                                    USB_GetSequence = USB_GetSequenceEnum.NO;
                                    MessageBox.Show(stVersionDeviceNotSupport_1[LangGlobal] + Connection.VersionOfDeviceProgram.ToString() + '.' + Connection.SubVersionOfDeviceProgram.ToString() + stVersionDeviceNotSupport_2[LangGlobal],
                                            stReceivingData[LangGlobal], MessageBoxButtons.OK,
                                            MessageBoxIcon.Error);                                    
                                }
                            }
                            else if (USB_GetSequence == USB_GetSequenceEnum.SEND_OPTIONS)
                            {
                                if (CompareVersions(Connection.VersionOfDeviceProgram, Connection.SubVersionOfDeviceProgram) == true)
                                {
                                    // версия поддерживается можно продолжать
                                    if (Connection.NewTypeDataSend(ModesSend.REQUEST_NUM_PLACES) == true)
                                    {
                                        // был реализован запрос с соот. номером и дано подтвержающее разрешение 
                                        timer1.Enabled = true;      // запуск выдержки для аварийного сброса связи
                                        Connection.DevOpt = DeviceOptions;
                                        Connection.ProgsOpt = CtrlProgramsOptions;  // сохранение данных в переменные, из которых они будут считываться и передаваться
                                        Connection.NeededPlaces = 0;
                                        for (int i = 1; i <= 8; i++)
                                        {
                                            Connection.NeededPlaces += CtrlProgramsOptions[i].ListHolidays.Count +
                                                CtrlProgramsOptions[i].ListEventsException.Count +
                                                CtrlProgramsOptions[i].ListEventsYear.Count +
                                                CtrlProgramsOptions[i].ListEventsMonth.Count +
                                                CtrlProgramsOptions[i].ListEventsWeek.Count +
                                                CtrlProgramsOptions[i].ListEventsDay.Count;
                                        }
                                        MyUsbSendData();      // вызов функции для начала передачи данных, которые подготавливаются в объекте класса ConnectionClass                
                                    }
                                }
                                else
                                {
                                    timer1.Enabled = false;     // отключить таймер
                                    label81.Text = StMessageUSB;
                                    USB_GetSequence = USB_GetSequenceEnum.NO;
                                    tabControl1.Enabled = true;
                                    MessageBox.Show(stVersionDeviceNotSupport_1[LangGlobal] + Connection.VersionOfDeviceProgram.ToString() + '.' + Connection.SubVersionOfDeviceProgram.ToString() + stVersionDeviceNotSupport_2[LangGlobal],
                                            stTransferData[LangGlobal], MessageBoxButtons.OK,
                                            MessageBoxIcon.Error);
                                }
                            }
                            else
                            {
                                timer1.Enabled = false;     // отключить таймер
                                label81.Text = StMessageUSB;
                                MessageBox.Show(stVersionDevice[LangGlobal] + Connection.VersionOfDeviceProgram.ToString() + '.' + Connection.SubVersionOfDeviceProgram.ToString(),
                                                stREV302[LangGlobal], MessageBoxButtons.OK,
                                                MessageBoxIcon.Information);
                            }
                            break;
                        case 8:
                            // получено текущее время устройства, измеренное напряжение и освещенность
                            timer1.Enabled = false;     // отключить таймер
                            label81.Text = StMessageUSB;
                            DeviceTimeMonitor = Connection.DeviceTime;
                            //label60.Text = string.Format("{0}.{1}.{2}  {3}.{4}.{5}", Connection.DeviceTime.Day, Connection.DeviceTime.Month, Connection.DeviceTime.Year, Connection.DeviceTime.Hour, Connection.DeviceTime.Minute, Connection.DeviceTime.Second);
                            label60.Text = DeviceTimeMonitor.ToString("dd MMMM yyyy");
                            label67.Text = DeviceTimeMonitor.ToString("HH:mm:ss");
                            DeviceTimeMonitorExist = true;

                            label70.Text = Connection.ReadVoltage.ToString();
                            label73.Text = Connection.ReadBright.ToString();
                            break;
                        case 9:
                            // новое время было успешно записано в устройство
                            timer1.Enabled = false;     // отключить таймер
                            label81.Text = StMessageUSB;
                            MessageBox.Show(stNewTimeSend[LangGlobal],
                                                stSetTime[LangGlobal], MessageBoxButtons.OK,
                                                MessageBoxIcon.Information);
                            break;
                        case 10:
                            // переданы новые значения напряжения и освещенности для калибирования
                            timer1.Enabled = false;     // отключить таймер
                            label81.Text = StMessageUSB;
                            MessageBox.Show(" Калибрование успешно завершено.\nПроконтролируйте точность скорректированных значений.",
                                                "Передача данных", MessageBoxButtons.OK,
                                                MessageBoxIcon.Information);
                            break;
                        case 11:
                            // переданы новые значения напряжения и освещенности для калибирования
                            timer1.Enabled = false;     // отключить таймер
                            label81.Text = StMessageUSB;
                            MessageBox.Show(" Калибровочное значение времени успешно записано в устройство.\nПроконтролируйте точность хода времени.",
                                                "Передача данных", MessageBoxButtons.OK,
                                                MessageBoxIcon.Information);
                            break;
                    }
                    timer2.Enabled = true;     // остановка таймера для считывания текущего времени                    
            }
        }

        private bool CompareVersions(int ver, int subver)
        {
            // проверка полученной версии устройства с теми, которые поддерживает данная программ            
            switch (ver)
            {
                case 1:
                    switch (subver)
                    {
                        case 1:
                            // v1.1
                            return true;        // v1.1
                            //break;
                        case 2:
                            return true;        // v1.2
                        default:
                            return false;
                            //break;
                    }
                    //break;
                default:
                    return false;
                    //break;

            }
        }

        private void usb_OnSpecifiedDeviceArrived(object sender, EventArgs e)
        {
            listBox1.Items.Add("My device was found");
            StMessageUSB = stUSBconnectIs[LangGlobal];
            label81.Text = StMessageUSB;

            USB_Connect = true;

            GetDeviceTime();
        }

        // запрашивает время подключенного устройства
        private void GetDeviceTime()
        {
            if (timer1.Enabled == false)
            {
                if (Connection.NewTypeDataSend(ModesSend.REQUEST_DEVICETIME) == true)
                {
                    timer1.Enabled = true;
                    MyUsbSendData();
                }
            }
        }

        private void button27_Click(object sender, EventArgs e)
        {
            try
            {

                byte[] data = new byte[32];
                data[0] = 0;                
                for (byte i = 1; i < 31; i++)
                {
                    data[i] = i;
                }
                data[1] = 100;
                
                // разрешение на передачу получено                    
                if (this.usb.SpecifiedDevice != null)
                {
                    this.usb.SpecifiedDevice.SendData(data);
                }
                else
                {
                    //MessageBox.Show("Устройство не подключено. Проверьте кабель и питание. /n Затем повторите попытку считать данные.");                    
                    MessageBox.Show(stDeviceNotConnect[LangGlobal],
                                        stAttention[LangGlobal], MessageBoxButtons.OK,
                                        MessageBoxIcon.Exclamation);
                }
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        // функция обработки посылки данных вызывается из любого места программы
        // (в основном из методов событий, где появляются запросы к внешним устройствам
        // или необходимость что-то переслать) для пересылки запросов, команд или данных
        // внешним устройствам или из метода события окончания посылки предыдущего байта,
        // для продолжения посылки данных. Передаваемые данные последовательно получаются
        // из метода объекта класса Connection.SendData, где анализируются текущие пересылаемые
        // данные, возвращается следующий байт для пересылки и дается разрешение 
        // на ее посылку
        private void MyUsbSendData()
        {
            try
            {                

                //byte[] data = new byte[2];
                byte[] data = new byte[32];
                data[0] = 0;


                if (Connection.SendData(ref data) == true)       //
                {
                    // разрешение на передачу получено                    
                    if (this.usb.SpecifiedDevice != null)
                    {
                        this.usb.SpecifiedDevice.SendData(data);
                    }
                    else
                    {
                        //MessageBox.Show("Устройство не подключено. Проверьте кабель и питание. /n Затем повторите попытку считать данные.");
                        tabControl1.Enabled = true;
                        timer1.Enabled = false;
                        MessageBox.Show(stDeviceNotConnect[LangGlobal],
                                            stAttention[LangGlobal], MessageBoxButtons.OK,
                                            MessageBoxIcon.Exclamation);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void usb_OnDataSend(object sender, EventArgs e)
        {
            //MyUsbSendData();        // вызов функции посылки следующего байта, если передача не завершена
        }

        // инициализация пересылки настроек из устройства в программу (вызов из меню)
        private void настройкиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // функция события с попыткой дать запрос на считывание данных настроек с устройства по USB
            //if (Connection.NewTypeDataSend(ModesSend.REQUEST_OPTIONS) == true)
            //if (Connection.NewTypeDataSend(ModesSend.REQUEST_NUM_EVENTS) == true)
            if (Connection.NewTypeDataSend(ModesSend.REQUEST_NUM_VERSION) == true)
            {
                // был реализован запрос с соот. номером и дано подтвержающее разрешение 
                // запуск таймера для корректного выхода в случае обрыва передачи
                tabControl1.Enabled = false;

                timer1.Enabled = true;
                USB_GetSequence = USB_GetSequenceEnum.GET_OPTIONS;        // тип последовательности запросов
                USB_GetSeqStep = 1;                      // шаг запроса
                MyUsbSendData();      // вызов функции для начала передачи данных, которые подготавливаются в объекте класса ConnectionClass                
            }
        }

        private void button28_Click(object sender, EventArgs e)
        {
            // тестовые выполнения некоторых расчетов

            //MessageBox.Show(string.Format("П1, нед. соб. №1:\n{0}\n{1} ", CtrlProgramsOptions[1].ListEventsWeek[0].DateAndTime.ToString(),
            //    CtrlProgramsOptions[1].ListEventsWeek[1].DateAndTime.ToString()), 
            //    "Проверка!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

            //MessageBox.Show(string.Format("{0}\n{1}", CtrlProgramsOptions[1].GetHashCode(),
            //       CtrlProgramsOptions[2].GetHashCode()), 
            //       "Проверка!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);


            //InRelayCtrlProgramsOptions.Clear();           
            //foreach (CtrlProgramOptionsClass PrOp in CtrlProgramsOptions)
            //    InRelayCtrlProgramsOptions.Add((CtrlProgramOptionsClass)PrOp.Clone());

            //if (CtrlProgramsOptions.Compare(InRelayCtrlProgramsOptions) == true &&
            //    DeviceOptions.Compare(InRelayDeviceOptions) == true)
            //{
            //    MessageBox.Show("Объекты совпали",
            //                        "Проверка!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            //}
            //else
            //    MessageBox.Show("Объекты не совпали",
            //                        "Проверка!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

            tabControl1.Enabled = false;
        }

        private void button29_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            dlgConnect.Close();
        }

        private void button30_Click(object sender, EventArgs e)
        {
            if (Connection.NewTypeDataSend(ModesSend.CONTINUE) == true)
                MyUsbSendData();      // вызов функции для продолжения передачи разрешенного запроса
        }

        private void button31_Click(object sender, EventArgs e)
        {
            if (dlgConnect.ProcBar1 == 90)
                dlgConnect.ProcBar1 = 45;
            else
                dlgConnect.ProcBar1 = 90;
        }

        private void SendToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // получен запрос на пересылку всех настроек в устройство
            //if (Connection.NewTypeDataSend(ModesSend.PREP_GET_OPTIONS) == true)
                //if (Connection.NewTypeDataSend(ModesSend.REQUEST_NUM_PLACES) == true)
                //{
                //    // был реализован запрос с соот. номером и дано подтвержающее разрешение 
                //    timer1.Enabled = true;      // запуск выдержки для аварийного сброса связи
                //    Connection.DevOpt = DeviceOptions;
                //    Connection.ProgsOpt = CtrlProgramsOptions;  // сохранение данных в переменные, из которых они будут считываться и передаваться
                //    Connection.NeededPlaces = 0;
                //    for (int i = 1; i <= 8; i++)
                //    {
                //        Connection.NeededPlaces += CtrlProgramsOptions[i].ListHolidays.Count +
                //            CtrlProgramsOptions[i].ListEventsException.Count +
                //            CtrlProgramsOptions[i].ListEventsYear.Count +
                //            CtrlProgramsOptions[i].ListEventsMonth.Count +
                //            CtrlProgramsOptions[i].ListEventsWeek.Count +
                //            CtrlProgramsOptions[i].ListEventsDay.Count;
                //    }
                //    MyUsbSendData();      // вызов функции для начала передачи данных, которые подготавливаются в объекте класса ConnectionClass                
                //}
            
            // здесь необходимо переставить фокус на другой элемент в окне, чтобы учесть последние данные
            treeView1.Focus();
            
            
            if (Connection.NewTypeDataSend(ModesSend.REQUEST_NUM_VERSION) == true)
            {
                // был реализован запрос с соот. номером и дано подтвержающее разрешение 
                // запуск таймера для корректного выхода в случае обрыва передачи
                tabControl1.Enabled = false;
                
                timer1.Enabled = true;
                USB_GetSequence = USB_GetSequenceEnum.SEND_OPTIONS;        // тип последовательности запросов
                USB_GetSeqStep = 1;                      // шаг запроса
                MyUsbSendData();      // вызов функции для начала передачи данных, которые подготавливаются в объекте класса ConnectionClass                
            }
        }

        private void usb_OnSpecifiedDeviceRemoved(object sender, EventArgs e)
        {
            listBox1.Items.Add("My device removed");
            StMessageUSB = stUSBconnectionNotIs[LangGlobal];
            label81.Text = StMessageUSB;

            USB_Connect = false;
        }

        private void usb_OnDeviceRemoved(object sender, EventArgs e)
        {
            listBox1.Items.Add("Device removed");
        }

        // событие вызываемое раз в секунду
        // запускается при инициализации приема или передачи
        // если это событие произошло, значит произошел обрыв связи
        private void timer1_Tick(object sender, EventArgs e)
        {
            tabControl1.Enabled = true;

            timer1.Enabled = false;
            Connection.ErrorConnect();
            if (Connection.TypeReadData != ModesRead.SEND_NEWTIME)
            {
                MessageBox.Show(stConnectError[LangGlobal],
                                            stAttention[LangGlobal], MessageBoxButtons.OK,
                                            MessageBoxIcon.Exclamation);
            }
            progressBar1.Visible = false;
            progressBar1.Value = 0;
            label81.Text = StMessageUSB;
        }

        private void опрограммеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            About dlgAbout = new About();
            dlgAbout.ShowDialog();
        }

        private void button32_Click(object sender, EventArgs e)
        {
            TimerClass tmr1 = new TimerClass();
            tmr1.DateAndTime = new DateTime(1996, 1, 1, 0, 0, 0);
            DateTime dt1 = new DateTime(1996, 1, 1, 0, 0, 0);
            for( int i = 0; i < 5000; i++)
            {
                //tmr1.DateAndTime = tmr1.DateAndTime.AddSeconds(1);                
                dt1 = dt1.AddSeconds(5);
                CtrlProgramsOptions[1].ListEventsYear.Add(new TimerClass());
                CtrlProgramsOptions[1].ListEventsYear[i].DateAndTime = new DateTime(1996, dt1.Month, dt1.Day, dt1.Hour, dt1.Minute, dt1.Second);
            }

            UpdateAllFormElements();
        }

        // вызов отобразить версию программы зашитой в устройство
        private void VersionProgToolStripMenuItem_Click(object sender, EventArgs e)
        {
            USB_GetSequence = USB_GetSequenceEnum.NO;
            if (Connection.NewTypeDataSend(ModesSend.REQUEST_NUM_VERSION) == true)
            {
                timer1.Enabled = true;
                MyUsbSendData();      // вызов функции для продолжения передачи разрешенного запроса                     
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            // обработка временных обображений
            DateTime NowTime = DateTime.Now;       // отслеживает текущее время            

            if ( USB_Connect == true && (tabControl1.SelectedTab == tabPage2_Time || tabControl1.SelectedTab == tabPage3_VoltBright) )
            {
                GetDeviceTime();
            }
            else
            {
                label60.Text = stInfo[LangGlobal];        // инициализация монитора текущего времени устройства
                label67.Text = stAbsent[LangGlobal];
                DeviceTimeMonitorExist = false;
            }                      
            //if (DeviceTimeMonitorExist == true)
            //{
            //    // если один раз время было считано 
            //    DeviceTimeMonitor = DeviceTimeMonitor.AddSeconds(1);
            //    label60.Text = DeviceTimeMonitor.ToString("dd MMMM yyyy");
            //    label67.Text = DeviceTimeMonitor.ToString("HH:mm:ss");
            //}                                    
            label68.Text = NowTime.ToString("dd MMMM yyyy");
            label69.Text = NowTime.ToString("HH:mm:ss");
                       

        }

        // ручное требование считать время устройства
        private void button22_Click(object sender, EventArgs e)
        {
            if ((USB_Connect == true) && ((tabControl1.SelectedTab == tabPage2_Time) || (tabControl1.SelectedTab == tabPage3_VoltBright)))
            {
                // если устройство подключено
                GetDeviceTime();
            }
            else
            {
                label60.Text = stInfo[LangGlobal];        // инициализация монитора текущего времени устройства
                label67.Text = stAbsent[LangGlobal];
                DeviceTimeMonitorExist = false;
            }
        }

        // загрузка времени ПК в устройство
        private void button20_Click(object sender, EventArgs e)
        {
            // инициировать передачу времени, но только в момент обновления секунд
            SychTime = DateTime.Now;
            WriteNewTimeInDevice = true;
            timer3.Enabled = true;
        }
        
        private void button21_Click(object sender, EventArgs e)
        {
            if (Connection.NewTypeDataSend(ModesSend.SEND_NEWTIME) == true)
            {
                timer1.Enabled = true;
                Connection.SendTimeToDevice = new DateTime(dateTimePicker6.Value.Year, dateTimePicker6.Value.Month, dateTimePicker6.Value.Day, dateTimePicker5.Value.Hour, dateTimePicker5.Value.Minute, dateTimePicker5.Value.Second);       // время, которое будет записано в устройство
                MyUsbSendData();      // вызов функции для продолжения передачи разрешенного запроса                     
            }    
        }

        // используется для синронизации времени ПК и устройства
        private void timer3_Tick(object sender, EventArgs e)
        {
            DateTime dNow = DateTime.Now;
            if (SychTime.Second != dNow.Second)
            {
                timer3.Enabled = false;
                if (SychShowTime == true)
                {
                    // если время изменилось     
                    SychShowTime = false;
                    timer2.Enabled = true;      // разрешить отображать время                    
                }

                if (WriteNewTimeInDevice == true)
                {                    
                    // если время изменилось                    
                    WriteNewTimeInDevice = false;
                    timer4.Enabled = true;
                }
            }
        }

        // используется для точной подстройки синхронизации установки времени устройства
        private void timer4_Tick(object sender, EventArgs e)
        {
            timer4.Enabled = false;
            Connection.SendTimeToDevice = DateTime.Now.AddSeconds(1);       // время, которое будет записано в устройство
            if (Connection.NewTypeDataSend(ModesSend.SEND_NEWTIME) == true)
            {
                timer1.Enabled = true;                
                MyUsbSendData();      // вызов функции для продолжения передачи разрешенного запроса                     
            }
        }

        // событие нажатие кнопки перекалибровки напряжения
        private void button35_Click(object sender, EventArgs e)
        {
            if (Connection.NewTypeDataSend(ModesSend.SEND_NEWVOLTBRIGHT) == true)
            {
                Connection.WriteBright = 0;     // обязательный сброс освещенности, что указывает, что ее калибровать не надо
                Connection.WriteVoltage = (int)numericUpDown44.Value;        // новове напряжение для передачи устройству
                timer1.Enabled = true;
                MyUsbSendData();      // вызов функции для продолжения передачи разрешенного запроса                     
            }
        }

        // событие нажатие кнопки перекалибровки освещенности
        private void button33_Click(object sender, EventArgs e)
        {
            if (Connection.NewTypeDataSend(ModesSend.SEND_NEWVOLTBRIGHT) == true)
            {
                Connection.WriteBright = (int)numericUpDown43.Value;                       // новове напряжение для передачи устройству
                Connection.WriteVoltage = 0;  // обязательный сброс напряжения, что указывает, что ее калибровать не надо       
                timer1.Enabled = true;
                MyUsbSendData();      // вызов функции для продолжения передачи разрешенного запроса                     
            }
        }

        // вызов окна создания событий привязанных к восходам и заходам солнца
        private void SunRiseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CtrlProgramOptionsClass TempSaveForBack = new CtrlProgramOptionsClass();
            TempSaveForBack.Add(new CtrlProgramOptionsClass());     // создание одного элемента коллекции для возврата последнего действия
            TempSaveForBack[0] = (CtrlProgramOptionsClass)CtrlProgramsOptions[ProgSelected].Clone();
            
            dlgSunRise.ListEvents = CtrlProgramsOptions[ProgSelected].ListEventsYear;   // передача текущего списка годовых событий            
            DialogResult dlgRes = dlgSunRise.ShowDialog();
            if (dlgRes == DialogResult.OK)
            {
                BackCtrlProgramsOptions[0] = (CtrlProgramOptionsClass)TempSaveForBack[0].Clone();
                VisibledBackButtons();
                
                CtrlProgramsOptions[ProgSelected].ListEventsYear = dlgSunRise.ListEvents;
                ReDrawListOfEventsYear();
            }
            else
            {

            }                        
        }

        // массив событий год. 
        private void button24_Click(object sender, EventArgs e)
        {
            CtrlProgramOptionsClass TempSaveForBack = new CtrlProgramOptionsClass();
            TempSaveForBack.Add(new CtrlProgramOptionsClass());     // создание одного элемента коллекции для возврата последнего действия
            TempSaveForBack[0] = (CtrlProgramOptionsClass)CtrlProgramsOptions[ProgSelected].Clone();            
            
            dlgArEv.ChangeShowElements(2);
            dlgArEv.ListEvents = CtrlProgramsOptions[ProgSelected].ListEventsYear;   // передача текущего списка годовых событий            
            DialogResult dlgRes = dlgArEv.ShowDialog();
            if (dlgRes == DialogResult.OK)
            {
                BackCtrlProgramsOptions[0] = (CtrlProgramOptionsClass)TempSaveForBack[0].Clone();
                VisibledBackButtons();
                
                CtrlProgramsOptions[ProgSelected].ListEventsYear = dlgArEv.ListEvents;
                ReDrawListOfEventsYear();
            }
            else
            {

            }
        }

        // массив соб. мес.
        private void button25_Click(object sender, EventArgs e)
        {
            CtrlProgramOptionsClass TempSaveForBack = new CtrlProgramOptionsClass();
            TempSaveForBack.Add(new CtrlProgramOptionsClass());     // создание одного элемента коллекции для возврата последнего действия
            TempSaveForBack[0] = (CtrlProgramOptionsClass)CtrlProgramsOptions[ProgSelected].Clone();            

            dlgArEv.ChangeShowElements(3);
            dlgArEv.ListEvents = CtrlProgramsOptions[ProgSelected].ListEventsMonth;   // передача текущего списка годовых событий            
            DialogResult dlgRes = dlgArEv.ShowDialog();
            if (dlgRes == DialogResult.OK)
            {
                BackCtrlProgramsOptions[0] = (CtrlProgramOptionsClass)TempSaveForBack[0].Clone();
                VisibledBackButtons();
                
                CtrlProgramsOptions[ProgSelected].ListEventsMonth = dlgArEv.ListEvents;
                ReDrawListOfEventsMonth();
            }
            else
            {

            }
        }

        private void button34_Click(object sender, EventArgs e)
        {            
            CtrlProgramOptionsClass TempSaveForBack = new CtrlProgramOptionsClass();
            TempSaveForBack.Add(new CtrlProgramOptionsClass());     // создание одного элемента коллекции для возврата последнего действия
            TempSaveForBack[0] = (CtrlProgramOptionsClass)CtrlProgramsOptions[ProgSelected].Clone();            

            // массив праздников
            dlgArEv.ChangeShowElements(0);
            dlgArEv.ListEvents = CtrlProgramsOptions[ProgSelected].ListHolidays;   // передача текущего списка годовых событий            
            DialogResult dlgRes = dlgArEv.ShowDialog();
            if (dlgRes == DialogResult.OK)
            {
                BackCtrlProgramsOptions[0] = (CtrlProgramOptionsClass)TempSaveForBack[0].Clone();
                VisibledBackButtons();
                
                CtrlProgramsOptions[ProgSelected].ListHolidays = dlgArEv.ListEvents;                
                ReDrawListOfHolidays();
            }
            else
            {

            }
        }

        private void button36_Click(object sender, EventArgs e)
        {
            CtrlProgramOptionsClass TempSaveForBack = new CtrlProgramOptionsClass();
            TempSaveForBack.Add(new CtrlProgramOptionsClass());     // создание одного элемента коллекции для возврата последнего действия
            TempSaveForBack[0] = (CtrlProgramOptionsClass)CtrlProgramsOptions[ProgSelected].Clone();
            
            // массив искл. соб
            dlgArEv.ChangeShowElements(1);
            dlgArEv.ListEvents = CtrlProgramsOptions[ProgSelected].ListEventsException;   // передача текущего списка годовых событий            
            DialogResult dlgRes = dlgArEv.ShowDialog();
            if (dlgRes == DialogResult.OK)
            {
                BackCtrlProgramsOptions[0] = (CtrlProgramOptionsClass)TempSaveForBack[0].Clone();
                VisibledBackButtons();
                
                CtrlProgramsOptions[ProgSelected].ListEventsException = dlgArEv.ListEvents;                
                ReDrawListOfEventsExceptions();
            }
            else
            {

            }
        }

        private void button37_Click(object sender, EventArgs e)
        {
            CtrlProgramOptionsClass TempSaveForBack = new CtrlProgramOptionsClass();
            TempSaveForBack.Add(new CtrlProgramOptionsClass());     // создание одного элемента коллекции для возврата последнего действия
            TempSaveForBack[0] = (CtrlProgramOptionsClass)CtrlProgramsOptions[ProgSelected].Clone();            

            // массив нед. соб.
            dlgArEv.ChangeShowElements(4);
            dlgArEv.ListEvents = CtrlProgramsOptions[ProgSelected].ListEventsWeek;   // передача текущего списка годовых событий            
            DialogResult dlgRes = dlgArEv.ShowDialog();
            if (dlgRes == DialogResult.OK)
            {
                BackCtrlProgramsOptions[0] = (CtrlProgramOptionsClass)TempSaveForBack[0].Clone();
                VisibledBackButtons();
                
                CtrlProgramsOptions[ProgSelected].ListEventsWeek = dlgArEv.ListEvents;
                ReDrawListOfEventsWeek();                
            }
            else
            {

            }
        }

        private void button38_Click(object sender, EventArgs e)
        {
            CtrlProgramOptionsClass TempSaveForBack = new CtrlProgramOptionsClass();
            TempSaveForBack.Add(new CtrlProgramOptionsClass());     // создание одного элемента коллекции для возврата последнего действия
            TempSaveForBack[0] = (CtrlProgramOptionsClass)CtrlProgramsOptions[ProgSelected].Clone();            

            // массив сут. соб.
            dlgArEv.ChangeShowElements(5);
            dlgArEv.ListEvents = CtrlProgramsOptions[ProgSelected].ListEventsDay;   // передача текущего списка годовых событий            
            DialogResult dlgRes = dlgArEv.ShowDialog();
            if (dlgRes == DialogResult.OK)
            {
                BackCtrlProgramsOptions[0] = (CtrlProgramOptionsClass)TempSaveForBack[0].Clone();
                VisibledBackButtons();
                
                CtrlProgramsOptions[ProgSelected].ListEventsDay = dlgArEv.ListEvents;
                ReDrawListOfEventsDay();
            }
            else
            {

            }
        }

        private void button39_Click(object sender, EventArgs e)
        {
            int selpoint = comboBox2.SelectedIndex;
            int selProg = 0;

            for (int i = 0; i <= comboBox2.SelectedIndex; i++)
            {
                selProg++;
                if (selProg == ProgSelected)
                    selProg++;
            }
            if (MessageBox.Show(string.Format(stCopy1[LangGlobal], ProgSelected, selProg),
                                        stCopy2[LangGlobal], MessageBoxButtons.OKCancel,
                                        MessageBoxIcon.Warning) == DialogResult.OK)
            {
                // копирование выбранной программы в текущую                                  
                CtrlProgramsOptions[ProgSelected] = (CtrlProgramOptionsClass)CtrlProgramsOptions[selProg].Clone();
                ReCalculateEventsInLists(ProgSelected);
                MessageBox.Show(string.Format(stCopy3[LangGlobal], selProg, ProgSelected),
                                            stCopy2[LangGlobal], MessageBoxButtons.OK,
                                            MessageBoxIcon.Asterisk);
            }
        }

        private void button41_Click(object sender, EventArgs e)
        {            
            byte[] LSEcal_CalibrationPpm = {0,1,2,3,4,5,6,7,8,9,10,10,11,12,13,14,15,16,17,
                                     18,19,20,21,22,23,24,25,26,27,28,29,30,31,31,32,33,34,
                                     35,36,37,38,39,40,41,42,43,44,45,46,47,48,49,50,51,51,
                                     52,53,54,55,56,57,58,59,60,61,62,63,64,65,66,67,68,69,
                                     70,71,72,72,73,74,75,76,77,78,79,80,81,82,83,84,85,86,
                                     87,88,89,90,91,92,93,93,94,95,96,97,98,99,100,101,102,
                                     103,104,105,106,107,108,109,110,111,112,113,113,114,
                                     115,116,117,118,119,120,121};            
            
            // расчет каллибровочного регистра для времени
            string[] str1;
            int d1, d2;
            double d3, d4, d5;
            int NullsAfterPoint;

            int DeviationInteger;
            byte CountWait = 0;

            str1 = textBox1.Text.Split('.');
            NullsAfterPoint = str1[1].Length;
            d1 = Convert.ToInt32(str1[0]);            
            d2 = Convert.ToInt32(str1[1]);
            //if (str1[0] == null)
            //    d2 = 0;
            //else
            //    d2 = Convert.ToInt32(str1[1]);
            //label74.Text = d1.ToString();
            //label94.Text = d2.ToString();
            
            d3 = (double)d2;
            d3 = d3 / Math.Pow(10, NullsAfterPoint);
            //while( d3 > 1 )
            //{
            //    d3 = d3 / 10;
            //}
            d3 = (d3 + (double)d1) * 0.000001;
            //label94.Text = d3.ToString();
            d4 = (double)32769 / (double)64;        // предделитель часового кварца и на выходе TAMPER-RTC
            d5 = 1 / d3;            // измеренная частота
            //label74.Text = d5.ToString();
            label74.Text = string.Format("{0:#.#####}", d5);
            d5 = d5 - d4;
            d5 = d5 / d4;
            d5 = d5 * 1000000;
            //d5 = ((d5 - d4) / d4) * 1000000;
            //label94.Text = d5.ToString();
            label94.Text = string.Format("{0:#.#####}", d5);

            DeviationInteger = (int)d5;
            if (DeviationInteger <= 121)
            {
                while (CountWait < 127)
                {
                    if (LSEcal_CalibrationPpm[CountWait] == DeviationInteger)
                        break;
                    CountWait++;
                }

            }
            else
                CountWait = 127;

            //label95.Text = CountWait.ToString();
            numericUpDown35.Value = (decimal)CountWait;
        }

        private void button40_Click(object sender, EventArgs e)
        {
            // отослать новые данные для калибровочного регистра времени
            if (timer1.Enabled == false)
            {
                if (Connection.NewTypeDataSend(ModesSend.SEND_FACTORYTIMECALIBR) == true)
                {
                    Connection.RTCCalibrValue = (byte)numericUpDown35.Value;  // высылаемое значение
                    timer1.Enabled = true;
                    MyUsbSendData();
                }
            }
        }

        private void содержаниеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CallHelp();
        }

        // очищает все списки в управляющей программе
        private void button42_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(string.Format(stClearLists1[LangGlobal], ProgSelected),
                                        stClearLists2[LangGlobal], MessageBoxButtons.YesNo,
                                        MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                CtrlProgramsOptions[ProgSelected].ResetCollections();
                ReCalculateEventsInLists();
            }
        }

        private void справкаToolStripButton_Click(object sender, EventArgs e)
        {
            CallHelp();
        }

        private void CallHelp()
        {
            try
            {
                Process SysInfo = new Process();
                SysInfo.StartInfo.ErrorDialog = true;
                if(LangGlobal == 0)
                    SysInfo.StartInfo.FileName = "MultiTimer.chm";                    
                else if (LangGlobal == 1)
                    SysInfo.StartInfo.FileName = "MultiTimerEng.chm";
                SysInfo.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        // ==============
        // методы для обработки отмены последнего действия

        private void SaveDataForBack()
        {
            BackCtrlProgramsOptions[0] = (CtrlProgramOptionsClass)CtrlProgramsOptions[ProgSelected].Clone();     // глубокое копирование в память для отмена            
            VisibledBackButtons();
        }

        private void BackDataFromBack()
        {
            CtrlProgramsOptions[ProgSelected] = (CtrlProgramOptionsClass)BackCtrlProgramsOptions[0].Clone();     // откат к предыдущему действию            
            UnVisibledBackButtons();
            ReDrawAllLists();
        }        

        // перерисовать все списки 
        private void ReDrawAllLists()
        {            
            ReDrawListOfHolidays();     // перерисовать список            
            ReDrawListOfEventsExceptions();            
            ReDrawListOfEventsYear();            
            ReDrawListOfEventsMonth();            
            ReDrawListOfEventsWeek();            
            ReDrawListOfEventsDay(); 
        }

        // скрывает все кнопки отвечающие за отмену действий с таблицами
        private void UnVisibledBackButtons()
        {
            button43.Enabled = false;
            button44.Enabled = false;
            button45.Enabled = false;
            button46.Enabled = false;
            button47.Enabled = false;
            button48.Enabled = false;
        }

        private void VisibledBackButtons()
        {
            button43.Enabled = true;
            button44.Enabled = true;
            button45.Enabled = true;
            button46.Enabled = true;
            button47.Enabled = true;
            button48.Enabled = true;
        }

        private void button43_Click(object sender, EventArgs e)
        {
            BackDataFromBack();
        }

        private void button44_Click(object sender, EventArgs e)
        {
            BackDataFromBack();
        }

        private void button45_Click(object sender, EventArgs e)
        {
            BackDataFromBack();
        }

        private void button46_Click(object sender, EventArgs e)
        {
            BackDataFromBack();
        }

        private void button47_Click(object sender, EventArgs e)
        {
            BackDataFromBack();
        }

        private void button48_Click(object sender, EventArgs e)
        {
            BackDataFromBack();
        }

        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {
            MessageBox.Show("Событие.", "Проверка!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult dr = MessageBox.Show(stSaveSettingsInFile[LangGlobal], stExitProgram[LangGlobal], MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation);
            switch (dr)
            {
                case DialogResult.Yes:
                    if (PathOfFile == null)
                    {
                        SaveNewFile();
                    }
                    else
                    {
                        SaveFile();
                    }
                    break;
                case DialogResult.No:
                    e.Cancel = false;
                    break;
                case DialogResult.Cancel:
                    e.Cancel = true;
                    break; 
            }
        }

        private void button49_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(string.Format(stResetProgramSettings1[LangGlobal], ProgSelected),
                                        stResetProgramSettings2[LangGlobal], MessageBoxButtons.YesNo,
                                        MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                CtrlProgramsOptions[ProgSelected].ResetOptions();
                ReCalculateEventsInLists();
            }
        }

        private void timer5_compDataInRelay_Tick(object sender, EventArgs e)
        {
            // таймер вызывается каждую секунду для сравнения текущих настроек с теми, которые
            // были в последний раз считаны или записаны в реле для отображения соот. сообщения            
            if (CtrlProgramsOptions.Compare(InRelayCtrlProgramsOptions) == true &&
                DeviceOptions.Compare(InRelayDeviceOptions) == true)            
            {
                // если текущие настройки совпадают с последними записанными в реле
                label95.Visible = false;
            }
            else
            {
                label95.Visible = true;
            }
        }

        private void SaveDataInRelay()
        {
            // метод сохраняет данные считанные или записанные в устройство для 
            // возможности вывода соот. информации о необходимости обновить информацию
            InRelayCtrlProgramsOptions.Clear();
            foreach (CtrlProgramOptionsClass PrOp in CtrlProgramsOptions)
                InRelayCtrlProgramsOptions.Add((CtrlProgramOptionsClass)PrOp.Clone());
            InRelayDeviceOptions = (DeviceOptionsClass)DeviceOptions.Clone();

            label95.Visible = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void monthCalendar1_DateChanged(object sender, DateRangeEventArgs e)
        {

        }
    }
}



