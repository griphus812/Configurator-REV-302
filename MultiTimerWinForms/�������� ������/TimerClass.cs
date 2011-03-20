using System;
using System.Collections;
using System.Collections.Generic;
//using System.Linq;
using System.Text;

namespace MultiTimerWinForms
{
    // класс описывающий таймер для создание коллекций таймеров
    [Serializable]
    public class TimerClass : CollectionBase
    {              
        
        public DateTime DateAndTime;      // дата и время
        public bool Condition;         // состояние контактов
        // информация о дне недели хранится в DataAndTime в дне месяца, но предварительно выставлено 1996 январь, чтобы номер дня говорил о дне недели
        //public int DayOfWeek;       // день недели
                                        // день недели хранится в январе относительно 1996 года

        // конструктор таймера
        public TimerClass()
        {            
        }
        public TimerClass(DateTime newDateAndTime)
        {
            DateAndTime = newDateAndTime;
            Condition = false;
        }
        public TimerClass(DateTime newDateAndTime, bool newCondition)
        {
            DateAndTime = newDateAndTime;
            Condition = newCondition;
        }



        //============= методы обслуживающие норамальную работу коллекции ========= 


        // возвращает true, если коллекции равны
        public bool Compare(TimerClass TC1)
        {
            bool Result = true;

            if (TC1.Count == this.Count)
            {
                int i = 0;
                foreach (TimerClass TC in this)
                {
                    if (DateTime.Compare(TC.DateAndTime, TC1[i].DateAndTime) == 0 && TC.Condition == TC1[i].Condition)
                    { }
                    else
                        Result = false;

                    i++;
                }
            }
            else
            {
                Result = false;
            }

            return Result;
        }


        // добавление нового элемента с проверкой месторасположения и возвращением номера элемента в списке (считая с нуля)
        // TypeOfListEvets - указывает какого типа список
        public int AddSmart(TimerClass newItem, int TypeOfListEvets)     
        {
            // предварительное формирование данных для сравнения с существующими событиями в списке
            switch (TypeOfListEvets)
            {
                case 0:
                    //если список праздников
                    newItem.DateAndTime = new DateTime(1996, newItem.DateAndTime.Month, newItem.DateAndTime.Day);
                    break;
                case 1:
                    // если список искл. событий
                    newItem.DateAndTime = new DateTime(1996, 1, 1, newItem.DateAndTime.Hour, newItem.DateAndTime.Minute, newItem.DateAndTime.Second);
                    break;
                case 2:
                    // если список годовых событий
                    newItem.DateAndTime = new DateTime(1996, newItem.DateAndTime.Month, newItem.DateAndTime.Day, newItem.DateAndTime.Hour, newItem.DateAndTime.Minute, newItem.DateAndTime.Second);
                    break;
                case 3:
                    // если список месячных событий
                    newItem.DateAndTime = new DateTime(1996, 1, newItem.DateAndTime.Day, newItem.DateAndTime.Hour, newItem.DateAndTime.Minute, newItem.DateAndTime.Second);
                    break;
                case 4:
                    // если список недельных событий
                    newItem.DateAndTime = new DateTime(1996, 1, newItem.DateAndTime.Day, newItem.DateAndTime.Hour, newItem.DateAndTime.Minute, newItem.DateAndTime.Second);
                    break;
                case 5:
                    // если список суточных событий
                    newItem.DateAndTime = new DateTime(1996, 1, 1, newItem.DateAndTime.Hour, newItem.DateAndTime.Minute, newItem.DateAndTime.Second);
                    break;
            }
            
            
            List.Add(new TimerClass());      // добавление пустого нового элемента
            //int i = List.Count - 2;
            if (this.Count == 1)
            {
                this[0] = newItem;
                return 0;
            }
            else
            {
                int i = this.Count - 2;
                while (i >= 0)
                {
                    int ResComp = DateTime.Compare(this[i].DateAndTime, newItem.DateAndTime);
                    if (ResComp == 1)
                    {
                        // если текущей элемент списка больше нового, то копирование его в след. элемент
                        this[i + 1] = this[i];
                    }
                    else if (ResComp == 0)
                    {
                        // если равны, то просто на место старого копирование нового с удалением пустого следующего
                        this[i] = newItem;
                        this.RemoveAt(i + 1);
                        return i;
                    }
                    else if (ResComp == -1)
                    {
                        // если элемент списка меньше нового, то вставка нового на образовавшееся пустое место
                        this[i + 1] = newItem;
                        return i + 1;
                    }
                    i--;
                }
                this[0] = newItem;

                return i + 1;
            }
        } 
        
        public void Add(TimerClass newItem)
        {
            List.Add(newItem);
        }
        public void Remove(TimerClass oldItem)
        {
            List.Remove(oldItem);            
        }        
        public void RemoveAt(int index)
        {
            List.RemoveAt(index);
        }        
        public void Clear()
        {
            List.Clear();
        }
        public TimerClass this[int ItemIndex]
        {
            get
            {
                return (TimerClass)List[ItemIndex];
            }
            set
            {
                List[ItemIndex] = value;
            }
        }
    }    
}
