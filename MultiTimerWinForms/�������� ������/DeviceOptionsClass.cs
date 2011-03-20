
//#define KANALS_QUANTITY   8 //

using System;
using System.Collections;
using System.Collections.Generic;
//using System.Linq;
using System.Text;



namespace MultiTimerWinForms
{
    [Serializable]
    public class DeviceOptionsClass : ICloneable
    {
        [Serializable]
        public struct AdjTime       // структура для хранения выдержек
        {
            public int Minute;
            public int Second;
        }
        
                
        // некоторые переменные и константы
        public byte ChannelsMax = 2;
        public byte ProgramsMax = 8;
                
        // настройки устройства
        public byte[] Channel_CtrlProg = new byte[2 + 1];    // массив хранит номер управл. программы для соот. канала, нулевой канал опущен; значение ноль означает канал отключен

        public bool DST_OnOff;          // управление включением летнего времени

        public AdjTime CommonDelay;     // общая задержка после подачи питания

        // конструктор
        public DeviceOptionsClass()
        {
            Init();            
        }

        public void Reset()
        {
            Init();
        }

        private void Init()
        {
            // инициализации всех переменных при создание объекта
            //for (int i = 1; i <= ChannelsMax; i++)
            //{
            //    Channel_CtrlProg[i] = 0;        // начальная инициализация переменных
            //}
            Channel_CtrlProg[1] = 0;
            Channel_CtrlProg[2] = 0;

            CommonDelay.Minute = 0;
            CommonDelay.Second = 0;

            DST_OnOff = true;
        }

        public object Clone()
        {
            DeviceOptionsClass newDevOpt = (DeviceOptionsClass)this.MemberwiseClone();

            byte[] newChannel_CtrlProg = { this.Channel_CtrlProg[0], this.Channel_CtrlProg[1], this.Channel_CtrlProg[2] };

            newDevOpt.Channel_CtrlProg = newChannel_CtrlProg;

            return newDevOpt;
        }

        public bool Compare(DeviceOptionsClass DevOpt)
        {
            bool Result = true;

            if (this.Channel_CtrlProg[1] == DevOpt.Channel_CtrlProg[1] &&
                this.Channel_CtrlProg[2] == DevOpt.Channel_CtrlProg[2] &&
                this.CommonDelay.Minute == DevOpt.CommonDelay.Minute &&
                this.CommonDelay.Second == DevOpt.CommonDelay.Second &&
                this.DST_OnOff == DevOpt.DST_OnOff)
            { }
            else
                Result = false;

            return Result;
        }
    }    
}
