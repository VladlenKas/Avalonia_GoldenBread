using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldenBread.Desktop.Enums
{
    public enum SectionType
    {
        [Description("Пользователи")]
        Users,

        [Description("Компании")]
        Company,

        [Description("Справочники")]
        References,

        [Description("Персонал")]
        Staff,

        [Description("Производство")]
        Production,

        [Description("Аналитика")]
        Analytics
    }
}
