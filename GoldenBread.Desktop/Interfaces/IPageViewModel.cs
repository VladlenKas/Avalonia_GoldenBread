using GoldenBread.Desktop.ViewModels.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldenBread.Desktop.Interfaces
{
    public interface IPageViewModel
    {
        IDetailsPanelViewModel DetailsPanel { get; }
        object? SelectedItem { get; set; }
    }
}
