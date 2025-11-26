using GoldenBread.Desktop.Enums;
using Humanizer;
using ReactiveUI.Validation.Helpers;

namespace GoldenBread.Desktop.ViewModels.Controls
{
    public class SidebarItem : ReactiveValidationObject
    {
        public string Title { get; set; }
        public SectionType Section { get; set; }
        public object IconTag { get; set; }

        public SidebarItem(SectionType section, object iconTag)
        {
            Section = section;
            Title = section.Humanize();
            IconTag = iconTag;
        }
    }
}
