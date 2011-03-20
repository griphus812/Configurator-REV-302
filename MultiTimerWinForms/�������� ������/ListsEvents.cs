using System;
using System.Collections;
using System.Collections.Generic;
//using System.Linq;
using System.Text;

namespace MultiTimerWinForms
{
    class ListsEvents : CollectionBase
    {
        
        //============= методы обслуживающие норамальную работу коллекции ========= 
        public void Add(ListsEvents newProgram)
        {
            List.Add(newProgram);
        }
        public void Remove(ListsEvents oldProgram)
        {
            List.Remove(oldProgram);
        }
        public ListsEvents this[int ProgramIndex]
        {
            get
            {
                return (ListsEvents)List[ProgramIndex];
            }
            set
            {
                List[ProgramIndex] = value;
            }
        }
    }
}
