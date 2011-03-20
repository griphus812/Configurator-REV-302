using System;
using System.Collections.Generic;
using System.Text;

namespace MultiTimerWinForms
{
    // класс использется для глобальных настроек программы 
    class SystemSettings
    {
        public enum TypeLanguage
        {
            RUSSIAN,
            ENGLISH
        }

        // переменная отвечает выбранный язык
        public TypeLanguage Lang;

        public SystemSettings()
        {
            //Lang = TypeLanguage.RUSSIAN;
            Lang = TypeLanguage.ENGLISH;
        }        
    }
}
