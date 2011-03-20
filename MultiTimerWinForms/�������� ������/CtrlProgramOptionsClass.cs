using System;
using System.Collections;
using System.Collections.Generic;
//using System.Linq;
using System.Text;

namespace MultiTimerWinForms
{
    // клас определяющий свойства управляющей программы 
    [Serializable]
    public class CtrlProgramOptionsClass : CollectionBase, ICloneable
    {
        //============= перечисления и структуры =================        
        //[Serializable]
        public enum RelayTimeModeType { R_T_M_OFF = 0, 
                                        R_T_M_YEAR = 1, 
                                        R_T_M_MONTH = 2, 
                                        R_T_M_WEEK = 3, 
                                        R_T_M_DAY = 4, 
                                        R_T_M_PULSE = 5, 
                                        R_T_M_SIMPLE = 6};

        [Serializable]
        public struct AdjTime       // структура для хранения выдержек
        {
            public int Minute;
            public int Second;
        }        
        
        //============= переменные ===============================
        public RelayTimeModeType RelayTimeMode;       // тип режима            
        public bool AllowDaysoffs;			// разрешение на учет выходных дней
        public bool AllowHolidays;			// разрешение на учет праздников
        public bool AllowCyclicity;		// разрешение на циклическое выполнение	                       
        
        // исключительные дни недели
        public bool[] ExceptDaysOfWeek = new bool[8];  // 1 - Monday, 2 - Tuesday et.

        // определение коллекций списков таймеров
        public TimerClass ListHolidays = new TimerClass();      // список праздников
        public TimerClass ListEventsException = new TimerClass();   // список исключительных событий
        public TimerClass ListEventsYear = new TimerClass();    // список годовых событий
        public TimerClass ListEventsMonth = new TimerClass();
        public TimerClass ListEventsWeek = new TimerClass();
        public TimerClass ListEventsDay = new TimerClass();
               
        // временные выдержки
        // реле импульсное
        public AdjTime RI_BeforeDelay;
        public AdjTime RI_OnDelay;
        public AdjTime RI_OffDelay;

        // реле простое
        public AdjTime RS_Delay;

        // реле напряжения
        public bool RV_OnOff;
        public int RV_Umin;
        public int RV_Uminhyst;
        public int RV_Umax;
        public int RV_Umaxhyst;
        public AdjTime RV_DelayUmin;
        public AdjTime RV_DelayUnorm;
        public AdjTime RV_DelayUmax;

        // фото-реле
        public bool RF_OnOff;
        public int RF_Lpor;
        public int RF_Lporhyst;
        public AdjTime RF_DelayLmin;
        public AdjTime RF_DelayLmax;
        public int RF_Condition_Lmin;
        public int RF_Condition_Lmax;
        
        //================== методы и функции =====================
        public object Clone()
        {
            CtrlProgramOptionsClass newProgOpt = (CtrlProgramOptionsClass)this.MemberwiseClone();

            TimerClass nListHolidays = new TimerClass();      // список праздников
            TimerClass nListEventsException = new TimerClass();   // список исключительных событий
            TimerClass nListEventsYear = new TimerClass();    // список годовых событий
            TimerClass nListEventsMonth = new TimerClass();
            TimerClass nListEventsWeek = new TimerClass();
            TimerClass nListEventsDay = new TimerClass();

            //TimerClass nListHolidays        = this.ListHolidays;
            //TimerClass nListEventsException = this.ListEventsException;
            //TimerClass nListEventsYear      = this.ListEventsYear;            
            //TimerClass nListEventsMonth     = this.ListEventsMonth;
            //TimerClass nListEventsWeek      = this.ListEventsWeek;
            //TimerClass nListEventsDay       = this.ListEventsDay;

            foreach (TimerClass TC in this.ListHolidays)
                nListHolidays.Add(new TimerClass(TC.DateAndTime, TC.Condition));
            foreach (TimerClass TC in this.ListEventsException)
                nListEventsException.Add(new TimerClass(TC.DateAndTime, TC.Condition));
            foreach (TimerClass TC in this.ListEventsYear)
                nListEventsYear.Add(new TimerClass(TC.DateAndTime, TC.Condition));
            foreach (TimerClass TC in this.ListEventsMonth)
                nListEventsMonth.Add(new TimerClass(TC.DateAndTime, TC.Condition));
            foreach (TimerClass TC in this.ListEventsWeek)
                nListEventsWeek.Add(new TimerClass(TC.DateAndTime, TC.Condition));
            foreach (TimerClass TC in this.ListEventsDay)
                nListEventsDay.Add(new TimerClass(TC.DateAndTime, TC.Condition));
            
            
            newProgOpt.ListHolidays        = nListHolidays;        
            newProgOpt.ListEventsException = nListEventsException; 
            newProgOpt.ListEventsYear      = nListEventsYear;      
            newProgOpt.ListEventsMonth     = nListEventsMonth;    
            newProgOpt.ListEventsWeek      = nListEventsWeek;
            newProgOpt.ListEventsDay       = nListEventsDay;


            bool[] ExDayWeek = { this.ExceptDaysOfWeek[0], 
                                   this.ExceptDaysOfWeek[1], 
                                   this.ExceptDaysOfWeek[2], 
                                   this.ExceptDaysOfWeek[3], 
                                   this.ExceptDaysOfWeek[4], 
                                   this.ExceptDaysOfWeek[5], 
                                   this.ExceptDaysOfWeek[6], 
                                   this.ExceptDaysOfWeek[7] };

            newProgOpt.ExceptDaysOfWeek = ExDayWeek;


            return newProgOpt;
            //return new CtrlProgramOptionsClass(this);
        }        

        public CtrlProgramOptionsClass()
        {
            Init();
        }

        private void Init()
        {
            // инициализация по умолчанию реле времени
            ResetOptions();
            ResetCollections();
        }

        public void Reset()
        {
            Init();
        }

        public void ResetOptions()
        {
            // инициализация по умолчанию реле времени
            RelayTimeMode = RelayTimeModeType.R_T_M_OFF;

            AllowDaysoffs = false;
            AllowHolidays = false;
            AllowCyclicity = true;

            for (int i = 1; i <= 7; i++)    // сброс исключит. дней недели
            {
                ExceptDaysOfWeek[i] = false;
            }

            // инициализация импульсного реле
            RI_BeforeDelay.Minute = 0;
            RI_BeforeDelay.Second = 0;
            RI_OnDelay.Minute = 0;
            RI_OnDelay.Second = 1;
            RI_OffDelay.Minute = 0;
            RI_OffDelay.Second = 1;

            // инициализация простого реле
            RS_Delay.Minute = 0;
            RS_Delay.Second = 0;

            // инициализация по умолчанию реле напряжения
            RV_OnOff = false;
            RV_Umin = 200;
            RV_Uminhyst = 5;
            RV_Umax = 240;
            RV_Umaxhyst = 5;
            RV_DelayUmin.Minute = 0;
            RV_DelayUmin.Second = 0;
            RV_DelayUnorm.Minute = 0;
            RV_DelayUnorm.Second = 1;
            RV_DelayUmax.Minute = 0;
            RV_DelayUmax.Second = 0;

            // инициализация по умолчанию фото-реле
            RF_OnOff = false;
            RF_Lpor = 30;
            RF_Lporhyst = 5;
            RF_DelayLmin.Minute = 0;
            RF_DelayLmin.Second = 1;
            RF_DelayLmax.Minute = 0;
            RF_DelayLmax.Second = 1;
            RF_Condition_Lmin = 0;
            RF_Condition_Lmax = 0;
        }

        public void ResetCollections()
        {
            // удаление всех коллекций            
            ListHolidays.Clear();
            ListEventsException.Clear();
            ListEventsYear.Clear();
            ListEventsMonth.Clear();
            ListEventsWeek.Clear();
            ListEventsDay.Clear();
        }
        

        public void Test1()
        {
            foreach (CtrlProgramOptionsClass PrOp in this)
            {
                PrOp.RV_OnOff = true;
            }
        }

        public bool Compare(CtrlProgramOptionsClass PrOp1)
        {
            bool Result = true;
            if (PrOp1.Count == this.Count)
            {
                int i = 0;
                foreach (CtrlProgramOptionsClass PrOp in this)
                {
                    if (PrOp.RelayTimeMode == PrOp1[i].RelayTimeMode &&
                        PrOp.AllowDaysoffs == PrOp1[i].AllowDaysoffs &&
                        PrOp.AllowHolidays == PrOp1[i].AllowHolidays &&
                        PrOp.AllowCyclicity == PrOp1[i].AllowCyclicity &&
                        PrOp.RI_BeforeDelay.Minute == PrOp1[i].RI_BeforeDelay.Minute &&
                        PrOp.RI_BeforeDelay.Second == PrOp1[i].RI_BeforeDelay.Second &&
                        PrOp.RI_OnDelay.Minute == PrOp1[i].RI_OnDelay.Minute &&
                        PrOp.RI_OnDelay.Second == PrOp1[i].RI_OnDelay.Second &&
                        PrOp.RI_OffDelay.Minute == PrOp1[i].RI_OffDelay.Minute &&
                        PrOp.RI_OffDelay.Second == PrOp1[i].RI_OffDelay.Second &&
                        PrOp.RS_Delay.Minute == PrOp1[i].RS_Delay.Minute &&
                        PrOp.RS_Delay.Second == PrOp1[i].RS_Delay.Second &&
                        PrOp.RV_OnOff == PrOp1[i].RV_OnOff &&
                        PrOp.RV_Umin == PrOp1[i].RV_Umin &&
                        PrOp.RV_Uminhyst == PrOp1[i].RV_Uminhyst &&
                        PrOp.RV_Umax == PrOp1[i].RV_Umax &&
                        PrOp.RV_Umaxhyst == PrOp1[i].RV_Umaxhyst &&
                        PrOp.RV_DelayUmin.Minute == PrOp1[i].RV_DelayUmin.Minute &&
                        PrOp.RV_DelayUmin.Second == PrOp1[i].RV_DelayUmin.Second &&
                        PrOp.RV_DelayUnorm.Minute == PrOp1[i].RV_DelayUnorm.Minute &&
                        PrOp.RV_DelayUnorm.Second == PrOp1[i].RV_DelayUnorm.Second &&
                        PrOp.RV_DelayUmax.Minute == PrOp1[i].RV_DelayUmax.Minute &&
                        PrOp.RV_DelayUmax.Second == PrOp1[i].RV_DelayUmax.Second &&
                        PrOp.RF_OnOff == PrOp1[i].RF_OnOff &&
                        PrOp.RF_Lpor == PrOp1[i].RF_Lpor &&
                        PrOp.RF_Lporhyst == PrOp1[i].RF_Lporhyst &&
                        PrOp.RF_DelayLmin.Minute == PrOp1[i].RF_DelayLmin.Minute &&
                        PrOp.RF_DelayLmin.Second == PrOp1[i].RF_DelayLmin.Second &&
                        PrOp.RF_DelayLmax.Minute == PrOp1[i].RF_DelayLmax.Minute &&
                        PrOp.RF_DelayLmax.Second == PrOp1[i].RF_DelayLmax.Second &&
                        PrOp.RF_Condition_Lmin == PrOp1[i].RF_Condition_Lmin &&
                        PrOp.RF_Condition_Lmax == PrOp1[i].RF_Condition_Lmax
                        )
                    {}
                    else
                    {
                        Result = false;
                    }

                    for (int k = 1; k <= 7; k++)    // сброс исключит. дней недели
                    {
                        if( PrOp.ExceptDaysOfWeek[k] != PrOp1[i].ExceptDaysOfWeek[k] )
                            Result = false;
                    }

                    if (PrOp.ListHolidays.Compare(PrOp1[i].ListHolidays) == true &&
                        PrOp.ListEventsException.Compare(PrOp1[i].ListEventsException) == true &&
                        PrOp.ListEventsYear.Compare(PrOp1[i].ListEventsYear) == true &&
                        PrOp.ListEventsMonth.Compare(PrOp1[i].ListEventsMonth) == true &&
                        PrOp.ListEventsWeek.Compare(PrOp1[i].ListEventsWeek) == true &&
                        PrOp.ListEventsDay.Compare(PrOp1[i].ListEventsDay) == true)
                    { }
                    else
                    {
                        Result = false;
                    }

                    i++;
                }
            }
            else
                Result = false;

            return Result;
        }
                
        //============= методы обслуживающие норамальную работу коллекции ========= 
        public void Add(CtrlProgramOptionsClass newProgram)
        {
            List.Add(newProgram);
        }
        public void Remove(CtrlProgramOptionsClass oldProgram)
        {
            List.Remove(oldProgram);
        }        
        public CtrlProgramOptionsClass this[int ProgramIndex]
        {
            get
            {
                return (CtrlProgramOptionsClass)List[ProgramIndex];
            }
            set
            {
                List[ProgramIndex] = value;
            }
        }
    }
}
