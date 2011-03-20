using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;

namespace MultiTimerWinForms
{
    [Serializable]
    public class DataInFileClass2       // последняя цифра означает версию сохраняемого файла
    {
        // класс формирует в одном объекте все данные, которые необходимо сохранить
        // и сопровождающую их служебную информацию (например о версии программы и т.д.)                
        public DeviceOptionsClass DeviceOptions = new DeviceOptionsClass();   // создание объекта класса для хранения и обмена данными с устройством
        public CtrlProgramOptionsClass CtrlProgramsOptions = new CtrlProgramOptionsClass();     // определение коллекции настроек управляющих программ
    }

    /*
     * DataInFileClass      - пилотная версия
     * DataInFileClass2     - исправлена ошибка связанная с неправильным считыванием данных из устройства для недельных событий 
     *      (вместо 2 0 0 7 запис. 2 0 0 8, что влияло на сортировку). В новой версии все года заменены на 1996 год.
     * 
     * 
     * 
     * 
     * 
     * 
     * 
     * 
     */
}
