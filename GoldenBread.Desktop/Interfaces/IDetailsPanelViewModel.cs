using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GoldenBread.Desktop.Interfaces
{
    public interface IDetailsPanelViewModel
    {
        public ICommand? CloseCommand { get; }
    }
}
