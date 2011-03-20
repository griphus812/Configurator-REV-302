
//#define KANALS_QUANTITY   8 //

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;



namespace MultiTimerWinForms
{
    public class DeviceOptionsClass
    {
        // некоторые переменные и константы
        public byte ChannelsMax = 2;
        public byte ProgramsMax = 8;

        public enum RelayTimeModeType { R_T_M_OFF, R_T_M_YEAR, R_T_M_MONTH, R_T_M_WEEK, R_T_M_DAY, R_T_M_PULSE, R_T_M_SIMPLE };
        public struct AdjTime          // регулируемые отрезки времени в настройках
        {
            byte Second;
            byte Minute;
        };
        //enum DaysOfWeekEnum 
        //{
        //    None = 0x00,
        //    Monday = 0x01, 
        //    Tuesday = 0x02,
        //    Wednesday = 0x04,
        //    Thursday = 0x08,
        //    Friday = 0x10,
        //    Saturday = 0x20,
        //    Sunday = 0x40
        //};          // для указания одновременно нескольки
        
        // настройки устройства
        public byte[] Channel_CtrlProg = new byte[2 + 1];    // массив хранит номер управл. программы для соот. канала, нулевой канал опущен; значение ноль означает канал отключен
        
        
        public struct CtrlProgramOptionsStruct
        {
            public RelayTimeModeType RelayTimeMode;       // тип режима            
            public bool AllowDaysoffs;			// разрешение на учет выходных дней
            public bool AllowHolidays;			// разрешение на учет праздников
            public bool AllowCyclicity;		// разрешение на циклическое выполнение	
               
            // исключения дней недели
            public bool Monday;
            public bool Tuesday;
            public bool Wednesday;
            public bool Thursday;
            public bool Friday;
            public bool Saturday;
            public bool Sunday;

            //bool[] ExceptWeekDays = new bool[7];    // исключительные дни недели            
        };

        public CtrlProgramOptionsStruct[] CtrlProgramOptions = new CtrlProgramOptionsStruct[9];        // создание массива структур настроек для каждой управляющей программы
            
        /*
        // структура настроек устройства
        public struct DeviceOptionStruct
        {
            
        }
        */

        // коллекции списков событий (таймеров) для управления контактами каналов реле
        ArrayList ListHolidays = new ArrayList();
        
        // конструктор
        public DeviceOptionsClass()
        {
            // инициализации всех переменных при создание объекта
            //for (int i = 1; i <= ChannelsMax; i++)
            //{
            //    Channel_CtrlProg[i] = 0;        // начальная инициализация переменных
            //}
            Channel_CtrlProg[1] = 0;
            Channel_CtrlProg[2] = 0;

            for (int i = 1; i <= 8; i++)
            {
                // сканирование каждой программы и настройка ее параметров                
                CtrlProgramOptions[i].RelayTimeMode = RelayTimeModeType.R_T_M_OFF;
                
                CtrlProgramOptions[i].Monday = false;
                CtrlProgramOptions[i].Tuesday = false;
                CtrlProgramOptions[i].Wednesday = false;
                CtrlProgramOptions[i].Thursday = false;
                CtrlProgramOptions[i].Friday = false;
                CtrlProgramOptions[i].Saturday = false;
                CtrlProgramOptions[i].Sunday = false;

                CtrlProgramOptions[i].AllowDaysoffs = false;
                CtrlProgramOptions[i].AllowHolidays = false;
                CtrlProgramOptions[i].AllowCyclicity = true;
            }
        }
    }    
}
