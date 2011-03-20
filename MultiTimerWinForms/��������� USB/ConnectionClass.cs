using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;

namespace MultiTimerWinForms
{
    public enum ModesSend        // все возможное режимы для передачи данных
    {
        NO = 0,                 // ничего не передается
        REQUEST_OPTIONS = 10,     // запрос на прием настроек устройства        
        PREP_GET_OPTIONS = 11,  // приготовиться устройству получить все настройки
        SEND_OPTIONS = 12,      // посылает команды начала передачи всех настроек и начинает передавать
        REQUEST_NUM_EVENTS = 14,          // запрос на количество событий записанных в списках реле
        REQUEST_NUM_PLACES = 15,         // запрос у устройства выслать кол-во мест под события (для сравнения с тем, что можно записать)
        REQUEST_NUM_VERSION = 16,       // запрос выслать номер текущей версии программы устройства        
        REQUEST_DEVICETIME = 17,        // запрашивает текущие установки времени устройства
        SEND_NEWTIME = 18,              // пересылает новое время для установки
        SEND_NEWVOLTBRIGHT = 19,        // пересылает новое значение напряжения и/или освещенности (если 0, то ничего менять не надо), с помощью которого необходимо скорректировать напряжение
        SEND_FACTORYTIMECALIBR = 20,    // пересылает новое заводское значение для калибровочного регистра
        CONTINUE = 127          // посылка устройству разрешения на продолжение передачи данных
    };

    public enum ModesRead        // все возможные режимы для приема данных
    {
        NO = 0,                     // ничего не принимается
        OPTIONS = 10,               // прием настроек устройства
        NUM_EVENTS = 14,            // ожидание информации о кол-ве событий в списках устройства
        NUM_PLACES = 15,            // ожидание информации о кол-ве места в памяти под события
        NUM_VERSION = 16,           // ожидается прием номера версии программы
        DEVICETIME = 17,            // ожидает приема текущий настроек времени устройства
        SEND_NEWTIME = 18,          // было отослано новое время для установки и ожидается подтверждение
        SEND_NEWVOLTBRIGHT = 19,    // было отослано новое значение напряжения и/или освещенности (если 0, то ничего менять не надо), с помощью которого необходимо скорректировать напряжение        
        SEND_FACTORYTIMECALIBR = 20,     // ожидается подтверждения получения нового калибр. коэф.
        CONTINUE = 127              // получено подтверждение принятых данных, продолжить передачу
    }; 
    
    public class ConnectionClass
    {
        private int BytesCount;     // подсчитывает текущий байт от первого полученного, каждый второй содержит информацию        
        public int ByteInSendBlock;    // подсчет информационных данных в пересылаемом блоке данных
        public int ByteInReadBlock;     // подсчет информ. данных в блоке получаемых данных
        /*
        public int TypeReadData;           // тип принимаемых данных    
                    // 0 - никакие данные не принимаются
                    // 10  - блок с общими данными
        public int TypeSendData;       // тип передаваемых данных
                    // 0 - никакие данные не посылаются
                    // 10 - отправить запрос на прием данных настроек (без списков)

        // все режимы, в состоянии которых может быть процесс пересылки данных
        public enum ModesSendData { RequestOptions = 10 };

        // все режимы, в состоянии которых может находиться процесс приема данных
        public enum ModesReadData { Options = 10 };
         */
        public ModesSend TypeSendData;
        public ModesRead TypeReadData;
        public int NumProgOfData;      // программа для которой передаются данные
        public int CountLoops;         // счетчик для использования в локальных циклах во время передач
        private int MaxItemsList;       // кол-во событий в каком-либо списке
        private int CountItemsList;     // текущее принимаемое или передаваемое событие в списке
        
        private TimerClass[] ListsEvents = new TimerClass[6];       // массив коллекций списков для быстрой обработки при передаче

        public int ProgressBarPercent;     // расчитывается текущий процент выполненной операции
        public int FullBytes;            // примерная оценка кол-ва байт, которые необходимо получить или передать
        private int ByteInSendBlockAbs;        // подсчет абсолютного количесва переданных байт от начала передачи
        private int ByteInReadBlockAbs;        // подсчет абсолютного количесва принятых байт от начала передачи
        public int EnablePlacesInDevice;        // хранит кол-во мест под события в последнем считанном устройстве
        public int NeededPlaces;            // кол-во необходимых мест для записи
        public int VersionOfDeviceProgram;  // сохраняет полученный номер версии программы устройства
        public int SubVersionOfDeviceProgram;  // сохраняет полученный номер подверсии программы устройства
        public DateTime DeviceTime;     // время устройства для посылки или приема
        public DateTime SendTimeToDevice;       // для посылки нового времени устройству
        public int ReadVoltage;     // считанное напряжение
        public int ReadBright;      // считанное освещение
        public int WriteVoltage;        // напряжение для записи
        public int WriteBright;         // освещенность для записи
        public byte RTCCalibrValue;     // значение для калибр. регистра
        
        // создание объектов для временного хранения поступающих или передаваемых данных
        public DeviceOptionsClass DevOpt = new DeviceOptionsClass();
        public CtrlProgramOptionsClass ProgsOpt = new CtrlProgramOptionsClass();
        
        // создание объектов, которые будут ссылаться на обновляемые или передаваемые данные
        //public DeviceOptionsClass DeviceOptions = new DeviceOptionsClass();
        //public CtrlProgramOptionsClass CtrlProgramsOptions = new CtrlProgramOptionsClass();

        public ConnectionClass()
        {
            BytesCount = 0;
        }
        
        public void LetLinksOnOptions(DeviceOptionsClass Dev1, CtrlProgramOptionsClass Prog1)
        {               
            //DeviceOptions = Dev1;           // получение ссылок на обрабатываемые данные
            //CtrlProgramsOptions = Prog1;            
            ResetVars();
        }
                
        // сбрасывает объекты временного хранения полученных данных
        public void ResetVars()
        {
            // создать новые объекты,
            DeviceOptionsClass DevOpt1 = new DeviceOptionsClass();
            CtrlProgramOptionsClass ProgsOpt1 = new CtrlProgramOptionsClass();
            for(int i = 0; i <= 8+1; i++ )
            {
                ProgsOpt1.Add(new CtrlProgramOptionsClass());         // добавить коллекцию программ настроек
            }

            // скопировать ссылки на новые пустые объекты для их последующего наполнения
            DevOpt = DevOpt1;
            ProgsOpt = ProgsOpt1;     
        }

        // вызывается при нарушении связи и полностью сбрасывает все настройки связи
        public void ErrorConnect()
        {
            TypeSendData = ModesSend.NO;
            TypeReadData = ModesRead.NO;
            ResetVars();
        }

        
        // инициализации пересылки новых данных по USB
        // возвращает true, если пересылка инициализирована и необходимо вызвать ф-цию посылки
        public bool NewTypeDataSend(ModesSend NewTypeSend)
        {
            bool ret = false;       // возвращаемая в конце функции переменная
            
            if( TypeSendData == ModesSend.NO )
            {
                // если в текущий момент не происходит пересылка или передача каких-либо данных
                TypeSendData = NewTypeSend; // ввести режим пересылки данных
                ByteInSendBlock = 0;        // сброс счетчика передаваемых байт
                ByteInSendBlockAbs = 0; // для расчета времени окончания передачи
                ret = true;     // разрешить начало передачи
            }         
            
            return ret;
        }

        // инициализация типа получаемых данных по USB
        // возвращает true, если все в порядке и следующие данные будут правильно считываться
        public bool NewTypeDataRead(ModesRead NewTypeRead)
        {
            bool ret = false;       // возвращаемая в конце функции переменная

            if ( TypeReadData == ModesRead.NO )
            {
                // если в текущий момент не происходит пересылка или передача каких-либо данных
                TypeReadData = NewTypeRead;
                ByteInReadBlock = 0;        // сброс счетчика передаваемых байт
                ByteInReadBlockAbs = 0;
                //ResetVars();        // сброс буфферных переменных
                ret = true;     // разрешить начало передачи
            }
            return ret;
        }

        // функция перевода bool в byte
        private byte BoolToByte(bool data)
        {
            if (data == false)
                return 0;
            else
                return 1;
        }


        
        // функции получения следующих данных для пересылки
        // в RefDataByte возвращается следующий передаваемый байт
        // если true, то можно передавать данные по USB
        public bool SendData(ref byte[] RefDataByte)
        {
            bool ret = false;       // созвращаемое значение            
            RefDataByte[0] = 0;
            switch (TypeSendData)
            {
                case ModesSend.NO:
                    break;
                case ModesSend.REQUEST_OPTIONS:        // если необходимо дать запрос на получения настроек устройства                    
                    RefDataByte[1] = (byte)ModesSend.REQUEST_OPTIONS;       // послать соот. запрос
                    for (int i = 2; i <= 31; i++)
                        RefDataByte[i] = 0;     // заполнение нулями остальную часть массива
                    TypeSendData = ModesSend.NO;       // завершить передачу
                    NewTypeDataRead(ModesRead.OPTIONS);     // инициализировать прием определенного типа данных                    
                    ret = true;     // разрешить передать байт данных
                    break;
                case ModesSend.CONTINUE:
                    RefDataByte[1] = (byte)ModesSend.CONTINUE;       // послать соот. запрос на продолжение передачи данных
                    for (int i = 2; i <= 31; i++)
                        RefDataByte[i] = 0;     // заполнение нулями остальную часть массива
                    TypeSendData = ModesSend.NO;       // завершить передачу                    
                    ret = true;     // разрешить передать байт данных
                    break;
                case ModesSend.PREP_GET_OPTIONS:
                    RefDataByte[1] = (byte)ModesSend.PREP_GET_OPTIONS;
                    for (int i = 2; i <= 31; i++)
                        RefDataByte[i] = 0;     // заполнение нулями остальную часть массива                    
                    TypeSendData = ModesSend.SEND_OPTIONS;
                    ByteInSendBlock = 0;        // сброс счетчика передаваемых байт
                    ret = true;     // разрешить передать байт данных
                    break;                    
                case ModesSend.REQUEST_NUM_EVENTS:
                    RefDataByte[1] = (byte)ModesSend.REQUEST_NUM_EVENTS;
                    for (int i = 2; i <= 31; i++)
                        RefDataByte[i] = 0;     // заполнение нулями остальную часть массива
                    TypeSendData = ModesSend.NO;        // пока не будут получены данные о кол-ве событий ничего не посылать
                    NewTypeDataRead(ModesRead.NUM_EVENTS);
                    ret = true;
                    break;
                case ModesSend.REQUEST_NUM_PLACES:
                    RefDataByte[1] = (byte)ModesSend.REQUEST_NUM_PLACES;
                    for (int i = 2; i <= 31; i++)
                        RefDataByte[i] = 0;     // заполнение нулями остальную часть массива
                    TypeSendData = ModesSend.NO;        // пока не будут получены данные о кол-ве событий ничего не посылать
                    NewTypeDataRead(ModesRead.NUM_PLACES);
                    ret = true;
                    break;
                case ModesSend.REQUEST_NUM_VERSION:
                    RefDataByte[1] = (byte)ModesSend.REQUEST_NUM_VERSION;
                    for (int i = 2; i <= 31; i++)
                        RefDataByte[i] = 0;     // заполнение нулями остальную часть массива
                    TypeSendData = ModesSend.NO;        // пока не будут получены данные о кол-ве событий ничего не посылать
                    NewTypeDataRead(ModesRead.NUM_VERSION);
                    ret = true;
                    break;
                case ModesSend.REQUEST_DEVICETIME:
                    RefDataByte[1] = (byte)ModesSend.REQUEST_DEVICETIME;
                    for (int i = 2; i <= 31; i++)
                        RefDataByte[i] = 0;     // заполнение нулями остальную часть массива
                    TypeSendData = ModesSend.NO;        // пока не будут получены данные о кол-ве событий ничего не посылать
                    NewTypeDataRead(ModesRead.DEVICETIME);
                    ret = true;
                    break;
                case ModesSend.SEND_NEWTIME:
                    RefDataByte[1] = (byte)ModesSend.SEND_NEWTIME;
                    RefDataByte[2] = (byte)(SendTimeToDevice.Year & 0x00FF);
                    RefDataByte[3] = (byte)((SendTimeToDevice.Year >> 8) & 0x00FF);
                    RefDataByte[4] = (byte)SendTimeToDevice.Month;
                    RefDataByte[5] = (byte)SendTimeToDevice.Day;
                    RefDataByte[6] = (byte)SendTimeToDevice.Hour;
                    RefDataByte[7] = (byte)SendTimeToDevice.Minute;
                    RefDataByte[8] = (byte)SendTimeToDevice.Second;
                    for (int i = 9; i <= 31; i++)
                        RefDataByte[i] = 0;     // заполнение нулями остальную часть массива
                    TypeSendData = ModesSend.NO;        // пока не будут получены данные о кол-ве событий ничего не посылать                    
                    //NewTypeDataRead(ModesRead.SEND_NEWTIME);
                    TypeReadData = ModesRead.SEND_NEWTIME;
                    ret = true;
                    break;
                case ModesSend.SEND_NEWVOLTBRIGHT:
                    RefDataByte[1] = (byte)ModesSend.SEND_NEWVOLTBRIGHT;
                    RefDataByte[2] = (byte)(WriteVoltage & 0x00FF);
                    RefDataByte[3] = (byte)((WriteVoltage >> 8) & 0x00FF);
                    RefDataByte[4] = (byte)(WriteBright & 0x00FF);
                    RefDataByte[5] = (byte)((WriteBright >> 8) & 0x00FF);
                    for (int i = 6; i <= 31; i++)
                        RefDataByte[i] = 0;     // заполнение нулями остальную часть массива
                    TypeSendData = ModesSend.NO;        // пока не будут получены данные о кол-ве событий ничего не посылать                    
                    //NewTypeDataRead(ModesRead.SEND_NEWTIME);
                    TypeReadData = ModesRead.SEND_NEWVOLTBRIGHT;
                    ret = true;
                    break;
                case ModesSend.SEND_FACTORYTIMECALIBR:
                    RefDataByte[1] = (byte)ModesSend.SEND_FACTORYTIMECALIBR;
                    RefDataByte[2] = RTCCalibrValue;
                    for (int i = 3; i <= 31; i++)
                        RefDataByte[i] = 0;     // заполнение нулями остальную часть массива
                    TypeSendData = ModesSend.NO;        // пока не будут получены данные о кол-ве событий ничего не посылать                                        
                    TypeReadData = ModesRead.SEND_FACTORYTIMECALIBR;
                    ret = true;
                    break;
                
                case ModesSend.SEND_OPTIONS:    // пересылка всех настроек в устройство
                    ret = true;         // разрешить передать байт данных
                    for (int i = 1; i <= 31; i++)
                    {
                        ByteInSendBlock++;
                        ByteInSendBlockAbs++;
                        if (FullBytes != 0)
                            ProgressBarPercent = ByteInSendBlockAbs * 100 / FullBytes;
                        switch (ByteInSendBlock)
                        {
                            case 1:         // первый байт в блоке
                                RefDataByte[i] = DevOpt.Channel_CtrlProg[1];       // сохранение номера упр. программы для 1-го канала                    
                                NumProgOfData = 1;
                                CountLoops = 1;     // следующее циклич. считывание исключит. дней недели, массив нач. с 1-ой позиции

                                FullBytes = 500;
                                for (int k = 1; k <= 8; k++)
                                {
                                    ListsEvents[0] = ProgsOpt[k].ListHolidays;
                                    ListsEvents[1] = ProgsOpt[k].ListEventsException;
                                    ListsEvents[2] = ProgsOpt[k].ListEventsYear;
                                    ListsEvents[3] = ProgsOpt[k].ListEventsMonth;
                                    ListsEvents[4] = ProgsOpt[k].ListEventsWeek;
                                    ListsEvents[5] = ProgsOpt[k].ListEventsDay;
                                    foreach (TimerClass List in ListsEvents)
                                    {
                                        FullBytes += List.Count * 8;
                                    }
                                }
                                break;
                            case 2:         // второй байт в блоке
                                RefDataByte[i] = DevOpt.Channel_CtrlProg[2];
                                break;

                            case 3:
                                RefDataByte[i] = BoolToByte(DevOpt.DST_OnOff);
                                break;
                            case 4:
                                RefDataByte[i] = (byte)DevOpt.CommonDelay.Minute;
                                break;
                            case 5:
                                RefDataByte[i] = (byte)DevOpt.CommonDelay.Second;
                                break;
                            case 6:
                                RefDataByte[i] = (byte)ProgsOpt[NumProgOfData].RelayTimeMode;
                                //ProgsOpt[NumProgOfData].RelayTimeMode = (CtrlProgramOptionsClass.RelayTimeModeType)newDataByte;
                                CountLoops = 1;
                                break;

                            case 7:
                                RefDataByte[i] = BoolToByte(ProgsOpt[NumProgOfData].ExceptDaysOfWeek[CountLoops]);
                                if (CountLoops < 7)
                                {
                                    CountLoops++;
                                    ByteInSendBlock = 6;
                                }
                                else
                                {
                                    CountLoops = 0;     //уст. 0, т.к. для следующий массив начинается с нуля
                                }
                                break;
                            case 8:
                                RefDataByte[i] = BoolToByte(ProgsOpt[NumProgOfData].AllowDaysoffs);
                                break;
                            case 9:
                                RefDataByte[i] = BoolToByte(ProgsOpt[NumProgOfData].AllowHolidays);
                                break;
                            case 10:
                                RefDataByte[i] = BoolToByte(ProgsOpt[NumProgOfData].AllowCyclicity);
                                CountLoops = 0;
                                break;
                            case 11:
                                // считывание минут временных выдержек
                                switch (CountLoops)
                                {
                                    case 0: RefDataByte[i] = (byte)ProgsOpt[NumProgOfData].RI_BeforeDelay.Minute; break;
                                    case 1: RefDataByte[i] = (byte)ProgsOpt[NumProgOfData].RI_OnDelay.Minute; break;
                                    case 2: RefDataByte[i] = (byte)ProgsOpt[NumProgOfData].RI_OffDelay.Minute; break;
                                    case 3: RefDataByte[i] = (byte)ProgsOpt[NumProgOfData].RS_Delay.Minute; break;
                                    case 4: RefDataByte[i] = (byte)ProgsOpt[NumProgOfData].RV_DelayUmin.Minute; break;
                                    case 5: RefDataByte[i] = (byte)ProgsOpt[NumProgOfData].RV_DelayUnorm.Minute; break;
                                    case 6: RefDataByte[i] = (byte)ProgsOpt[NumProgOfData].RV_DelayUmax.Minute; break;
                                    case 7: RefDataByte[i] = (byte)ProgsOpt[NumProgOfData].RF_DelayLmin.Minute; break;
                                    case 8: RefDataByte[i] = (byte)ProgsOpt[NumProgOfData].RF_DelayLmax.Minute; break;
                                }
                                break;
                            case 12:
                                // считывание секунд временных выдержек
                                switch (CountLoops)
                                {
                                    case 0: RefDataByte[i] = (byte)ProgsOpt[NumProgOfData].RI_BeforeDelay.Second; break;
                                    case 1: RefDataByte[i] = (byte)ProgsOpt[NumProgOfData].RI_OnDelay.Second; break;
                                    case 2: RefDataByte[i] = (byte)ProgsOpt[NumProgOfData].RI_OffDelay.Second; break;
                                    case 3: RefDataByte[i] = (byte)ProgsOpt[NumProgOfData].RS_Delay.Second; break;
                                    case 4: RefDataByte[i] = (byte)ProgsOpt[NumProgOfData].RV_DelayUmin.Second; break;
                                    case 5: RefDataByte[i] = (byte)ProgsOpt[NumProgOfData].RV_DelayUnorm.Second; break;
                                    case 6: RefDataByte[i] = (byte)ProgsOpt[NumProgOfData].RV_DelayUmax.Second; break;
                                    case 7: RefDataByte[i] = (byte)ProgsOpt[NumProgOfData].RF_DelayLmin.Second; break;
                                    case 8: RefDataByte[i] = (byte)ProgsOpt[NumProgOfData].RF_DelayLmax.Second; break;
                                }

                                if (CountLoops < 8)
                                {
                                    CountLoops++;
                                    ByteInSendBlock = 10;
                                }
                                else
                                {
                                    CountLoops = 0;
                                }
                                break;
                            case 13:
                                RefDataByte[i] = BoolToByte(ProgsOpt[NumProgOfData].RV_OnOff);
                                break;
                            case 14:
                                RefDataByte[i] = (byte)(ProgsOpt[NumProgOfData].RV_Umin & 0x00FF);
                                break;
                            case 15:
                                RefDataByte[i] = (byte)((ProgsOpt[NumProgOfData].RV_Umin >> 8) & 0x00FF);
                                break;
                            case 16:
                                RefDataByte[i] = (byte)(ProgsOpt[NumProgOfData].RV_Umax & 0x00FF);
                                break;
                            case 17:
                                RefDataByte[i] = (byte)((ProgsOpt[NumProgOfData].RV_Umax >> 8) & 0x00FF);
                                break;
                            case 18:
                                RefDataByte[i] = (byte)ProgsOpt[NumProgOfData].RV_Uminhyst;
                                break;
                            case 19:
                                RefDataByte[i] = (byte)ProgsOpt[NumProgOfData].RV_Umaxhyst;
                                break;

                            case 20:
                                RefDataByte[i] = BoolToByte(ProgsOpt[NumProgOfData].RF_OnOff);
                                break;
                            case 21:
                                RefDataByte[i] = (byte)(ProgsOpt[NumProgOfData].RF_Lpor & 0x00FF);
                                break;
                            case 22:
                                RefDataByte[i] = (byte)((ProgsOpt[NumProgOfData].RF_Lpor >> 8) & 0x00FF);
                                break;
                            case 23:
                                RefDataByte[i] = (byte)(ProgsOpt[NumProgOfData].RF_Lporhyst & 0x00FF);
                                break;
                            case 24:
                                RefDataByte[i] = (byte)((ProgsOpt[NumProgOfData].RF_Lporhyst >> 8) & 0x00FF);
                                break;
                            case 25:
                                RefDataByte[i] = (byte)ProgsOpt[NumProgOfData].RF_Condition_Lmin;
                                break;
                            case 26:
                                RefDataByte[i] = (byte)ProgsOpt[NumProgOfData].RF_Condition_Lmax;
                                // инициализация цикла для считывания списков
                                CountLoops = 0;
                                ListsEvents[0] = ProgsOpt[NumProgOfData].ListHolidays;
                                ListsEvents[1] = ProgsOpt[NumProgOfData].ListEventsException;
                                ListsEvents[2] = ProgsOpt[NumProgOfData].ListEventsYear;
                                ListsEvents[3] = ProgsOpt[NumProgOfData].ListEventsMonth;
                                ListsEvents[4] = ProgsOpt[NumProgOfData].ListEventsWeek;
                                ListsEvents[5] = ProgsOpt[NumProgOfData].ListEventsDay;
                                break;

                            case 27:
                                // получение кол-ва событий в следующем списке   
                                MaxItemsList = ListsEvents[CountLoops].Count;
                                RefDataByte[i] = (byte)(MaxItemsList & 0x00FF);
                                break;
                            case 28:
                                RefDataByte[i] = (byte)((MaxItemsList >> 8) & 0x00FF);
                                if (MaxItemsList == 0)
                                    ByteInSendBlock = 36 - 1;
                                else
                                    CountItemsList = 1;
                                break;

                            case 29:    // принимается номер месяца
                                // начало приема следующего списка, если он не пустой
                                RefDataByte[i] = (byte)ListsEvents[CountLoops][CountItemsList - 1].DateAndTime.Month;
                                break;
                            case 30:        // принимается день месяца
                                RefDataByte[i] = (byte)ListsEvents[CountLoops][CountItemsList - 1].DateAndTime.Day;
                                break;
                            case 31:
                                // место считывания дня недели
                                RefDataByte[i] = (byte)(ListsEvents[CountLoops][CountItemsList - 1].DateAndTime.Day - 1);
                                break;
                            case 32:    // принимается час
                                RefDataByte[i] = (byte)ListsEvents[CountLoops][CountItemsList - 1].DateAndTime.Hour;
                                break;
                            case 33:    // принимаются минуты
                                RefDataByte[i] = (byte)ListsEvents[CountLoops][CountItemsList - 1].DateAndTime.Minute;
                                break;
                            case 34:    // принимаются секунды
                                RefDataByte[i] = (byte)ListsEvents[CountLoops][CountItemsList - 1].DateAndTime.Second;
                                break;
                            case 35:
                                RefDataByte[i] = BoolToByte(ListsEvents[CountLoops][CountItemsList - 1].Condition);

                                if (CountItemsList < MaxItemsList)
                                {
                                    CountItemsList++;
                                    ByteInSendBlock = 29 - 1;
                                }
                                else
                                {
                                    ByteInSendBlock = 36 - 1;
                                }
                                break;
                            case 36:
                                if (CountLoops < 5)
                                {
                                    CountLoops++;
                                    ByteInSendBlock = 27 - 1;
                                }
                                else
                                {
                                    ByteInSendBlock = 37 - 1;
                                    CountLoops = 0;
                                }
                                break;
                            case 37:
                                if (NumProgOfData < 8)          // считывание следующей программы
                                {
                                    NumProgOfData++;
                                    ByteInSendBlock = 6 - 1;
                                    CountLoops = 1;     // инициализация цикла для считывания выходных дней
                                }
                                break;
                            case 38:
                                // если это последний принятый байт в посылке (блоке)                                                        
                                TypeSendData = ModesSend.NO;       // прием данных в этом блоке завершен и соот. сброс переменной, где указывается тип принимаемых данных                                    
                                //Ret = 1;       // указание на необходимость перерисовать все окна
                                break;
                        }
                    }
                    break;
            }

            return ret;
        }

        // вызывается из основной программы, где были получены данные
        // обработка полученных данных
        // возвращает отчет выполненных операций, со следующими значениями
        // 0 - никаких действи не предпринимать
        // 1 - перерисовать все данные в окнах приложения (или только отображаемого окна)
        // 2 - продолжать прием
        // 3 - продолжать передачу
        // 4 - успешная запись настроек
        // 5 - продолжить прием данных, инициализация нового запроса приема
        // 6 - ошибка передачи настроек
        // 7 - получена версия программы устройства, которую можно считать
        // 8 - получены установки времени
        // 9 - подтверждение полученного нового времени
        public int ReadData(byte[] newDataByte, ref DeviceOptionsClass DeviceOptions, ref CtrlProgramOptionsClass CtrlProgramsOptions)
        {
            DateTime tmpTime;   // объект для временного хранения времени
            
            int Ret = 0;        // возвращаемое значение            

                // обнаруживается каждый второй байт данных
                BytesCount = 0;    // сброс счетчика
                // получен информационный байт
                // проверка, не идет ли в данный момент получение блока данных
                // и, если да, то какой тип данных принимается                
            switch (TypeReadData)   // в TypeReadData хранится тип принимаемых данных, устанавливается в предыдущих случаях приема данныъ
            {
                case ModesRead.NO:
                    // никакие данные в текущий момент не принимаются,
                    // но получен запрос на какие-либо действия
                    switch (newDataByte[1])
                    {
                        case 127:
                            if (TypeSendData != ModesSend.NO)
                                Ret = 3;        // указание главному окну отображать передачу данных и вызвать функцию передачи
                            else
                                Ret = 4;        // передача закончена
                            break;
                    }                        
                    //TypeReadData = (ModesRead)newDataByte;     // сохранение типа получаемых данных
                    //ByteInReadBlock = 0;        // сброс счетчика байтов в блоке
                    //Ret = 0;
                    break; 
                case ModesRead.NUM_EVENTS:
                    //ByteInReadBlock++;  // приращение байта в полученном блоке
                    ProgressBarPercent = 0;     // пока ничего не принято                        
                    //switch (ByteInReadBlock)
                    //{
                        //case 1:
                            FullBytes = (int)newDataByte[1];
                        //    Ret = 2;        // для подтверждения получ. данных
                        //    break;
                        //case 2:
                            FullBytes |= (int)newDataByte[2] << 8;
                            FullBytes = (FullBytes * 8) + 500;      // расчет общего кол-ва принимаемых данных в байтах
                            TypeReadData = ModesRead.NO;        // завершить прием
                            NewTypeDataSend(ModesSend.REQUEST_OPTIONS);     // послать запрос на прием настроек
                            Ret = 5;        // требование вызвать функции посылки просто отослать данные
                            //break;
                    //}
                    break;
                case ModesRead.NUM_PLACES:
                    //ByteInReadBlock++;  // приращение байта в полученном блоке
                    ProgressBarPercent = 0;     // пока ничего не передано                   
                    //switch (ByteInReadBlock)
                    //{
                        //case 1:
                            EnablePlacesInDevice = (int)newDataByte[1];       // в FullBytes пока будет хранится кол-во мест под события
                            //Ret = 2;        // для подтверждения получ. данных
                            //break;
                        //case 2:
                            EnablePlacesInDevice |= (int)newDataByte[2] << 8;
                            if (EnablePlacesInDevice > 5000)
                                EnablePlacesInDevice = 5000;        // ограничение для сохранения небольшого запаса свободного места
                            if (NeededPlaces > EnablePlacesInDevice)
                            {
                                TypeReadData = ModesRead.NO;        // завершить прием
                                Ret = 6;        // потребовать вывести окно с предупреждением и заврешить отображать передачу
                            }
                            else
                            {
                                TypeReadData = ModesRead.NO;        // завершить прием
                                NewTypeDataSend(ModesSend.PREP_GET_OPTIONS);     // послать запрос на прием настроек
                                Ret = 5;        // требование вызвать функции посылки, просто отослать данные
                            }
                            //break;
                    //}
                    break;
                case ModesRead.NUM_VERSION:
                    VersionOfDeviceProgram = (int)newDataByte[1];
                    SubVersionOfDeviceProgram = (int)newDataByte[2];
                    TypeReadData = ModesRead.NO;        // завершить прием
                    Ret = 7;        
                    break;
                case ModesRead.DEVICETIME:
                    int Year;
                    Year = EnablePlacesInDevice = (int)newDataByte[1];
                    Year |= (int)newDataByte[2] << 8;
                    DeviceTime = new DateTime(Year, (int)newDataByte[3], (int)newDataByte[4],
                                              (int)newDataByte[5], (int)newDataByte[6], (int)newDataByte[7]);
                    ReadVoltage = (int)newDataByte[8];
                    ReadVoltage |= (int)newDataByte[9] << 8;
                    ReadBright = (int)newDataByte[10];
                    ReadBright |= (int)newDataByte[11] << 8;
                    TypeReadData = ModesRead.NO;        // завершить прием
                    Ret = 8;        
                    break;
                case ModesRead.SEND_NEWTIME:                    
                    // были высланы новые данные для установки времени и получено подтверждение получения
                    TypeReadData = ModesRead.NO;        // завершить прием
                    Ret = 9;
                    break;
                case ModesRead.SEND_NEWVOLTBRIGHT:
                    // были высланы новые данные для установки времени и получено подтверждение получения
                    TypeReadData = ModesRead.NO;        // завершить прием
                    Ret = 10;
                    break;
                case ModesRead.SEND_FACTORYTIMECALIBR:
                    TypeReadData = ModesRead.NO;
                    Ret = 11;
                    break;

                case ModesRead.OPTIONS:
                    // сканирование типов передачи
                    //Ret = AdaptReadDataType10(newDataByte, ref DeviceOptions, ref CtrlProgramsOptions);

                    Ret = 2;        // по умолчанию разрешить дальнейший прием данных
                    for (int i = 1; i <= 31; i++)       // перебор байт в пакете
                    {
                        ByteInReadBlock++;  // приращение байта в полученном блоке
                        ByteInReadBlockAbs++;
                        ProgressBarPercent = 0;
                        if (FullBytes != 0)
                            ProgressBarPercent = ByteInReadBlockAbs * 100 / FullBytes;                                                
                        switch (ByteInReadBlock)
                        {
                            case 1:         // первый байт в блоке
                                ResetVars();    // первоначальный сброс переменных для сохранения
                                DevOpt.Channel_CtrlProg[1] = newDataByte[i];       // сохранение номера упр. программы для 1-го канала                    
                                NumProgOfData = 1;
                                CountLoops = 1;     // следующее циклич. считывание исключит. дней недели, массив нач. с 1-ой позиции
                                break;
                            case 2:         // второй байт в блоке
                                DevOpt.Channel_CtrlProg[2] = newDataByte[i];
                                break;
                            case 3:
                                if (newDataByte[i] == 0)
                                    DevOpt.DST_OnOff = false;
                                else
                                    DevOpt.DST_OnOff = true;
                                break;
                            case 4:
                                DevOpt.CommonDelay.Minute = newDataByte[i];
                                break;
                            case 5:
                                DevOpt.CommonDelay.Second = newDataByte[i];
                                break;
                            case 6:
                                ProgsOpt[NumProgOfData].RelayTimeMode = (CtrlProgramOptionsClass.RelayTimeModeType)newDataByte[i];
                                CountLoops = 1;
                                break;
                            case 7:
                                if (newDataByte[i] == 0)
                                    ProgsOpt[NumProgOfData].ExceptDaysOfWeek[CountLoops] = false;
                                else
                                    ProgsOpt[NumProgOfData].ExceptDaysOfWeek[CountLoops] = true;

                                if (CountLoops < 7)
                                {
                                    CountLoops++;
                                    ByteInReadBlock = 6;
                                }
                                else
                                {
                                    CountLoops = 0;     //уст. 0, т.к. для следующий массив начинается с нуля
                                }
                                break;
                            case 8:
                                if (newDataByte[i] == 0)
                                    ProgsOpt[NumProgOfData].AllowDaysoffs = false;
                                else
                                    ProgsOpt[NumProgOfData].AllowDaysoffs = true;
                                break;
                            case 9:
                                if (newDataByte[i] == 0)
                                    ProgsOpt[NumProgOfData].AllowHolidays = false;
                                else
                                    ProgsOpt[NumProgOfData].AllowHolidays = true;
                                break;
                            case 10:
                                if (newDataByte[i] == 0)
                                    ProgsOpt[NumProgOfData].AllowCyclicity = false;
                                else
                                    ProgsOpt[NumProgOfData].AllowCyclicity = true;
                                CountLoops = 0;
                                break;
                            case 11:
                                // считывание минут временных выдержек
                                switch (CountLoops)
                                {
                                    case 0: ProgsOpt[NumProgOfData].RI_BeforeDelay.Minute = newDataByte[i]; break;
                                    case 1: ProgsOpt[NumProgOfData].RI_OnDelay.Minute = newDataByte[i]; break;
                                    case 2: ProgsOpt[NumProgOfData].RI_OffDelay.Minute = newDataByte[i]; break;
                                    case 3: ProgsOpt[NumProgOfData].RS_Delay.Minute = newDataByte[i]; break;
                                    case 4: ProgsOpt[NumProgOfData].RV_DelayUmin.Minute = newDataByte[i]; break;
                                    case 5: ProgsOpt[NumProgOfData].RV_DelayUnorm.Minute = newDataByte[i]; break;
                                    case 6: ProgsOpt[NumProgOfData].RV_DelayUmax.Minute = newDataByte[i]; break;
                                    case 7: ProgsOpt[NumProgOfData].RF_DelayLmin.Minute = newDataByte[i]; break;
                                    case 8: ProgsOpt[NumProgOfData].RF_DelayLmax.Minute = newDataByte[i]; break;
                                }
                                break;
                            case 12:
                                // считывание секунд временных выдержек
                                switch (CountLoops)
                                {
                                    case 0: ProgsOpt[NumProgOfData].RI_BeforeDelay.Second = newDataByte[i]; break;
                                    case 1: ProgsOpt[NumProgOfData].RI_OnDelay.Second = newDataByte[i]; break;
                                    case 2: ProgsOpt[NumProgOfData].RI_OffDelay.Second = newDataByte[i]; break;
                                    case 3: ProgsOpt[NumProgOfData].RS_Delay.Second = newDataByte[i]; break;
                                    case 4: ProgsOpt[NumProgOfData].RV_DelayUmin.Second = newDataByte[i]; break;
                                    case 5: ProgsOpt[NumProgOfData].RV_DelayUnorm.Second = newDataByte[i]; break;
                                    case 6: ProgsOpt[NumProgOfData].RV_DelayUmax.Second = newDataByte[i]; break;
                                    case 7: ProgsOpt[NumProgOfData].RF_DelayLmin.Second = newDataByte[i]; break;
                                    case 8: ProgsOpt[NumProgOfData].RF_DelayLmax.Second = newDataByte[i]; break;
                                }

                                if (CountLoops < 8)
                                {
                                    CountLoops++;
                                    ByteInReadBlock = 10;
                                }
                                else
                                {
                                    CountLoops = 0;
                                }
                                break;
                            case 13:
                                if (newDataByte[i] == 0)
                                    ProgsOpt[NumProgOfData].RV_OnOff = false;
                                else
                                    ProgsOpt[NumProgOfData].RV_OnOff = true;
                                break;
                            case 14:
                                ProgsOpt[NumProgOfData].RV_Umin = (int)newDataByte[i];
                                break;
                            case 15:
                                ProgsOpt[NumProgOfData].RV_Umin |= (int)newDataByte[i] << 8;
                                break;
                            case 16:
                                ProgsOpt[NumProgOfData].RV_Umax = (int)newDataByte[i];
                                break;
                            case 17:
                                ProgsOpt[NumProgOfData].RV_Umax |= (int)newDataByte[i] << 8;
                                break;
                            case 18:
                                ProgsOpt[NumProgOfData].RV_Uminhyst = (int)newDataByte[i];
                                break;
                            case 19:
                                ProgsOpt[NumProgOfData].RV_Umaxhyst = (int)newDataByte[i];
                                break;
                            case 20:
                                if (newDataByte[i] == 0)
                                    ProgsOpt[NumProgOfData].RF_OnOff = false;
                                else
                                    ProgsOpt[NumProgOfData].RF_OnOff = true;
                                break;
                            case 21:
                                ProgsOpt[NumProgOfData].RF_Lpor = (int)newDataByte[i];
                                break;
                            case 22:
                                ProgsOpt[NumProgOfData].RF_Lpor |= (int)newDataByte[i] << 8;
                                break;
                            case 23:
                                ProgsOpt[NumProgOfData].RF_Lporhyst = newDataByte[i];
                                break;
                            case 24:
                                ProgsOpt[NumProgOfData].RF_Lporhyst |= (int)newDataByte[i] << 8;
                                break;
                            case 25:
                                ProgsOpt[NumProgOfData].RF_Condition_Lmin = (int)newDataByte[i];
                                break;
                            case 26:
                                ProgsOpt[NumProgOfData].RF_Condition_Lmax = (int)newDataByte[i];
                                // инициализация цикла для считывания списков
                                CountLoops = 0;
                                break;

                            case 27:
                                // получение кол-ва событий в следующем списке
                                MaxItemsList = (int)newDataByte[i];
                                break;
                            case 28:
                                MaxItemsList |= (int)newDataByte[i] << 8;
                                if (MaxItemsList == 0)
                                    ByteInReadBlock = 36 - 1;
                                else
                                    CountItemsList = 1;
                                break;
                            case 29:    // принимается номер месяца
                                // начало приема следующего списка, если он не пустой
                                switch (CountLoops)
                                {
                                    case 0:     // список праздников
                                        ProgsOpt[NumProgOfData].ListHolidays.Add(new TimerClass());
                                        ProgsOpt[NumProgOfData].ListHolidays[CountItemsList - 1].DateAndTime = new DateTime(1996, (int)newDataByte[i], 1, 0, 0, 0);
                                        break;
                                    case 1:     // список исключительных событий
                                        ProgsOpt[NumProgOfData].ListEventsException.Add(new TimerClass());
                                        ProgsOpt[NumProgOfData].ListEventsException[CountItemsList - 1].DateAndTime = new DateTime(1996, 1, 1, 0, 0, 0);
                                        break;
                                    case 2:     // список годовых событий
                                        ProgsOpt[NumProgOfData].ListEventsYear.Add(new TimerClass());
                                        ProgsOpt[NumProgOfData].ListEventsYear[CountItemsList - 1].DateAndTime = new DateTime(1996, (int)newDataByte[i], 1, 0, 0, 0);
                                        break;
                                    case 3:     // список месячных событий
                                        ProgsOpt[NumProgOfData].ListEventsMonth.Add(new TimerClass());
                                        ProgsOpt[NumProgOfData].ListEventsMonth[CountItemsList - 1].DateAndTime = new DateTime(1996, (int)newDataByte[i], 1, 0, 0, 0);
                                        break;
                                    case 4:     // список недельных событий
                                        ProgsOpt[NumProgOfData].ListEventsWeek.Add(new TimerClass());
                                        ProgsOpt[NumProgOfData].ListEventsWeek[CountItemsList - 1].DateAndTime = new DateTime(1996, 1, 1, 0, 0, 0);
                                        break;
                                    case 5:     // список суточных событий
                                        ProgsOpt[NumProgOfData].ListEventsDay.Add(new TimerClass());
                                        ProgsOpt[NumProgOfData].ListEventsDay[CountItemsList - 1].DateAndTime = new DateTime(1996, 1, 1, 0, 0, 0);
                                        break;
                                }
                                break;
                            case 30:        // принимается день месяца
                                switch (CountLoops)
                                {
                                    case 0:
                                        tmpTime = ProgsOpt[NumProgOfData].ListHolidays[CountItemsList - 1].DateAndTime;
                                        ProgsOpt[NumProgOfData].ListHolidays[CountItemsList - 1].DateAndTime = new DateTime(1996, tmpTime.Month, (int)newDataByte[i], tmpTime.Hour, tmpTime.Minute, tmpTime.Second);
                                        break;
                                    case 1:
                                        break;
                                    case 2:     // список годовых событий
                                        tmpTime = ProgsOpt[NumProgOfData].ListEventsYear[CountItemsList - 1].DateAndTime;
                                        ProgsOpt[NumProgOfData].ListEventsYear[CountItemsList - 1].DateAndTime = new DateTime(1996, tmpTime.Month, (int)newDataByte[i], tmpTime.Hour, tmpTime.Minute, tmpTime.Second);
                                        break;
                                    case 3:     // список месячных событий
                                        tmpTime = ProgsOpt[NumProgOfData].ListEventsMonth[CountItemsList - 1].DateAndTime;
                                        ProgsOpt[NumProgOfData].ListEventsMonth[CountItemsList - 1].DateAndTime = new DateTime(1996, tmpTime.Month, (int)newDataByte[i], tmpTime.Hour, tmpTime.Minute, tmpTime.Second);
                                        break;
                                    case 4:     // список недельных событий                                        
                                        break;
                                    case 5:     // список суточных событий                                        
                                        break;
                                }
                                break;
                            case 31:
                                // место считывания дня недели
                                switch (CountLoops)
                                {
                                    case 0:
                                        break;
                                    case 1:
                                        break;
                                    case 2:     // список годовых событий                                        
                                        break;
                                    case 3:     // список месячных событий                                        
                                        break;
                                    case 4:     // список недельных событий                                        
                                        tmpTime = ProgsOpt[NumProgOfData].ListEventsWeek[CountItemsList - 1].DateAndTime;
                                        ProgsOpt[NumProgOfData].ListEventsWeek[CountItemsList - 1].DateAndTime = new DateTime(1996, 1, (int)newDataByte[i] + 1, tmpTime.Hour, tmpTime.Minute, tmpTime.Second);                                        
                                        break;
                                    case 5:     // список суточных событий                                        
                                        break;
                                }
                                break;
                            case 32:    // принимается час
                                switch (CountLoops)
                                {
                                    case 0:
                                        break;
                                    case 1:
                                        tmpTime = ProgsOpt[NumProgOfData].ListEventsException[CountItemsList - 1].DateAndTime;
                                        ProgsOpt[NumProgOfData].ListEventsException[CountItemsList - 1].DateAndTime = new DateTime(1996, tmpTime.Month, tmpTime.Day, (int)newDataByte[i], tmpTime.Minute, tmpTime.Second);
                                        break;
                                    case 2:     // список годовых событий
                                        tmpTime = ProgsOpt[NumProgOfData].ListEventsYear[CountItemsList - 1].DateAndTime;
                                        ProgsOpt[NumProgOfData].ListEventsYear[CountItemsList - 1].DateAndTime = new DateTime(1996, tmpTime.Month, tmpTime.Day, (int)newDataByte[i], tmpTime.Minute, tmpTime.Second);
                                        break;
                                    case 3:     // список месячных событий
                                        tmpTime = ProgsOpt[NumProgOfData].ListEventsMonth[CountItemsList - 1].DateAndTime;
                                        ProgsOpt[NumProgOfData].ListEventsMonth[CountItemsList - 1].DateAndTime = new DateTime(1996, tmpTime.Month, tmpTime.Day, (int)newDataByte[i], tmpTime.Minute, tmpTime.Second);
                                        break;
                                    case 4:     // список недельных событий 
                                        tmpTime = ProgsOpt[NumProgOfData].ListEventsWeek[CountItemsList - 1].DateAndTime;
                                        ProgsOpt[NumProgOfData].ListEventsWeek[CountItemsList - 1].DateAndTime = new DateTime(1996, tmpTime.Month, tmpTime.Day, (int)newDataByte[i], tmpTime.Minute, tmpTime.Second);
                                        break;
                                    case 5:     // список суточных событий 
                                        tmpTime = ProgsOpt[NumProgOfData].ListEventsDay[CountItemsList - 1].DateAndTime;
                                        ProgsOpt[NumProgOfData].ListEventsDay[CountItemsList - 1].DateAndTime = new DateTime(1996, tmpTime.Month, tmpTime.Day, (int)newDataByte[i], tmpTime.Minute, tmpTime.Second);
                                        break;
                                }
                                break;
                            case 33:    // принимаются минуты
                                switch (CountLoops)
                                {
                                    case 0:
                                        break;
                                    case 1:
                                        tmpTime = ProgsOpt[NumProgOfData].ListEventsException[CountItemsList - 1].DateAndTime;
                                        ProgsOpt[NumProgOfData].ListEventsException[CountItemsList - 1].DateAndTime = new DateTime(1996, tmpTime.Month, tmpTime.Day, tmpTime.Hour, (int)newDataByte[i], tmpTime.Second);
                                        break;
                                    case 2:     // список годовых событий
                                        tmpTime = ProgsOpt[NumProgOfData].ListEventsYear[CountItemsList - 1].DateAndTime;
                                        ProgsOpt[NumProgOfData].ListEventsYear[CountItemsList - 1].DateAndTime = new DateTime(1996, tmpTime.Month, tmpTime.Day, tmpTime.Hour, (int)newDataByte[i], tmpTime.Second);
                                        break;
                                    case 3:     // список месячных событий
                                        tmpTime = ProgsOpt[NumProgOfData].ListEventsMonth[CountItemsList - 1].DateAndTime;
                                        ProgsOpt[NumProgOfData].ListEventsMonth[CountItemsList - 1].DateAndTime = new DateTime(1996, tmpTime.Month, tmpTime.Day, tmpTime.Hour, (int)newDataByte[i], tmpTime.Second);
                                        break;
                                    case 4:     // список недельных событий 
                                        tmpTime = ProgsOpt[NumProgOfData].ListEventsWeek[CountItemsList - 1].DateAndTime;
                                        ProgsOpt[NumProgOfData].ListEventsWeek[CountItemsList - 1].DateAndTime = new DateTime(1996, tmpTime.Month, tmpTime.Day, tmpTime.Hour, (int)newDataByte[i], tmpTime.Second);
                                        break;
                                    case 5:     // список суточных событий 
                                        tmpTime = ProgsOpt[NumProgOfData].ListEventsDay[CountItemsList - 1].DateAndTime;
                                        ProgsOpt[NumProgOfData].ListEventsDay[CountItemsList - 1].DateAndTime = new DateTime(1996, tmpTime.Month, tmpTime.Day, tmpTime.Hour, (int)newDataByte[i], tmpTime.Second);
                                        break;
                                }
                                break;
                            case 34:    // принимаются секунды
                                switch (CountLoops)
                                {
                                    case 0:
                                        tmpTime = ProgsOpt[NumProgOfData].ListHolidays[CountItemsList - 1].DateAndTime;
                                        ProgsOpt[NumProgOfData].ListHolidays[CountItemsList - 1].DateAndTime = new DateTime(1996, tmpTime.Month, tmpTime.Day, tmpTime.Hour, tmpTime.Minute, (int)newDataByte[i]);
                                        break;
                                    case 1:
                                        tmpTime = ProgsOpt[NumProgOfData].ListEventsException[CountItemsList - 1].DateAndTime;
                                        ProgsOpt[NumProgOfData].ListEventsException[CountItemsList - 1].DateAndTime = new DateTime(1996, tmpTime.Month, tmpTime.Day, tmpTime.Hour, tmpTime.Minute, (int)newDataByte[i]);
                                        break;
                                    case 2:     // список годовых событий
                                        tmpTime = ProgsOpt[NumProgOfData].ListEventsYear[CountItemsList - 1].DateAndTime;
                                        ProgsOpt[NumProgOfData].ListEventsYear[CountItemsList - 1].DateAndTime = new DateTime(1996, tmpTime.Month, tmpTime.Day, tmpTime.Hour, tmpTime.Minute, (int)newDataByte[i]);
                                        break;
                                    case 3:     // список месячных событий
                                        tmpTime = ProgsOpt[NumProgOfData].ListEventsMonth[CountItemsList - 1].DateAndTime;
                                        ProgsOpt[NumProgOfData].ListEventsMonth[CountItemsList - 1].DateAndTime = new DateTime(1996, tmpTime.Month, tmpTime.Day, tmpTime.Hour, tmpTime.Minute, (int)newDataByte[i]);
                                        break;
                                    case 4:     // список недельных событий 
                                        tmpTime = ProgsOpt[NumProgOfData].ListEventsWeek[CountItemsList - 1].DateAndTime;
                                        ProgsOpt[NumProgOfData].ListEventsWeek[CountItemsList - 1].DateAndTime = new DateTime(1996, tmpTime.Month, tmpTime.Day, tmpTime.Hour, tmpTime.Minute, (int)newDataByte[i]);
                                        break;
                                    case 5:     // список суточных событий 
                                        tmpTime = ProgsOpt[NumProgOfData].ListEventsDay[CountItemsList - 1].DateAndTime;
                                        ProgsOpt[NumProgOfData].ListEventsDay[CountItemsList - 1].DateAndTime = new DateTime(1996, tmpTime.Month, tmpTime.Day, tmpTime.Hour, tmpTime.Minute, (int)newDataByte[i]);
                                        break;
                                }
                                break;
                            case 35:
                                switch (CountLoops)
                                {
                                    case 0:
                                        if (newDataByte[i] == 0)
                                            ProgsOpt[NumProgOfData].ListHolidays[CountItemsList - 1].Condition = false;
                                        else
                                            ProgsOpt[NumProgOfData].ListHolidays[CountItemsList - 1].Condition = true;
                                        break;
                                    case 1:
                                        if (newDataByte[i] == 0)
                                            ProgsOpt[NumProgOfData].ListEventsException[CountItemsList - 1].Condition = false;
                                        else
                                            ProgsOpt[NumProgOfData].ListEventsException[CountItemsList - 1].Condition = true;
                                        break;
                                    case 2:     // список годовых событий
                                        if (newDataByte[i] == 0)
                                            ProgsOpt[NumProgOfData].ListEventsYear[CountItemsList - 1].Condition = false;
                                        else
                                            ProgsOpt[NumProgOfData].ListEventsYear[CountItemsList - 1].Condition = true;
                                        break;
                                    case 3:     // список месячных событий
                                        if (newDataByte[i] == 0)
                                            ProgsOpt[NumProgOfData].ListEventsMonth[CountItemsList - 1].Condition = false;
                                        else
                                            ProgsOpt[NumProgOfData].ListEventsMonth[CountItemsList - 1].Condition = true;
                                        break;
                                    case 4:     // список недельных событий 
                                        if (newDataByte[i] == 0)
                                            ProgsOpt[NumProgOfData].ListEventsWeek[CountItemsList - 1].Condition = false;
                                        else
                                            ProgsOpt[NumProgOfData].ListEventsWeek[CountItemsList - 1].Condition = true;
                                        break;
                                    case 5:     // список суточных событий 
                                        if (newDataByte[i] == 0)
                                            ProgsOpt[NumProgOfData].ListEventsDay[CountItemsList - 1].Condition = false;
                                        else
                                            ProgsOpt[NumProgOfData].ListEventsDay[CountItemsList - 1].Condition = true;
                                        break;
                                }


                                if (CountItemsList < MaxItemsList)
                                {
                                    CountItemsList++;
                                    ByteInReadBlock = 29 - 1;
                                }
                                else
                                {
                                    ByteInReadBlock = 36 - 1;
                                }
                                break;
                            case 36:
                                if (CountLoops < 5)
                                {
                                    CountLoops++;
                                    ByteInReadBlock = 27 - 1;
                                }
                                else
                                {
                                    ByteInReadBlock = 37 - 1;
                                    CountLoops = 0;
                                }
                                break;
                            case 37:
                                if (NumProgOfData < 8)          // считывание следующей программы
                                {
                                    NumProgOfData++;
                                    ByteInReadBlock = 6 - 1;
                                    CountLoops = 1;     // инициализация цикла для считывания выходных дней
                                }
                                break;
                            case 38:
                                // если это последний принятый байт в посылке (блоке)
                                DeviceOptions = DevOpt;         // загрузка данных в приложение 
                                CtrlProgramsOptions = ProgsOpt;
                                TypeReadData = ModesRead.NO;       // прием данных в этом блоке завершен и соот. сброс переменной, где указывается тип принимаемых данных                                    
                                Ret = 1;       // указание на необходимость перерисовать все окна
                                break;
                            default:
                                // обработка оставшихся пустых байт в пакете после завершения приема                                
                                break;
                        }
                    }
                    break;                    
            }            
            return Ret;
        }        
    }
}
