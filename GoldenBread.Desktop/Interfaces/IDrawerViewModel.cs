using GoldenBread.Desktop.Bases;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;

namespace GoldenBread.Desktop.Interfaces
{
    public interface IDrawerViewModel
    {
        DrawerMode CurrentMode { get; }
        ReactiveCommand<Unit, Unit> CloseCommand { get; }
    }
}
