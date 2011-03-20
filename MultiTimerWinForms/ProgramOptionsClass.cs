using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MultiTimerWinForms
{
    // клас определяющий свойства управляющей программы 
    class CtrlProgramOptionsClass : CollectionBase
    {
        //============= перечисления и структуры =================        
        public enum RelayTimeModeType { R_T_M_OFF, 
                                        R_T_M_YEAR, 
                                        R_T_M_MONTH, 
                                        R_T_M_WEEK, 
                                        R_T_M_DAY, 
                                        R_T_M_PULSE, 
                                        R_T_M_SIMPLE };
        
        
        //============= переменные ===============================
        public RelayTimeModeType RelayTimeMode;       // тип режима            
        public bool AllowDaysoffs;			// разрешение на учет выходных дней
        public bool AllowHolidays;			// разрешение на учет праздников
        public bool AllowCyclicity;		// разрешение на циклическое выполнение	
                       
        /*
        // исключения дней недели
        public bool Monday;
        public bool Tuesday;
        public bool Wednesday;
        public bool Thursday;
        public bool Friday;
        public bool Saturday;
        public bool Sunday;
         */
        // исключительные дни недели
        bool[] ExceptDaysOfWeek = new bool[8];  // 1 - Monday, 2 - Tuesday et.

        
        //================== методы и функции =====================
        public CtrlProgramOptionsClass()
        {
            RelayTimeMode = RelayTimeModeType.R_T_M_OFF;
            
            AllowDaysoffs = false;
            AllowHolidays = false;
            AllowCyclicity = true;

            /*
            foreach (bool Day in ExceptDaysOfWeek)  // сброс исключит. дней
            {
                Day = false;
            }
             */

            for (int i = 1; i <= 7; i++)
            {
                ExceptDaysOfWeek[i] = false;
            }
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
