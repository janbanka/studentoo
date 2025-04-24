using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace studentoo
{
    public class ChatMessageTemplateSelector : DataTemplateSelector
    {
        public DataTemplate PartnerTemplate { get; set; }
        public DataTemplate UserTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is messages vm)
                return vm.IsCurrentUser ? UserTemplate : PartnerTemplate;

            return base.SelectTemplate(item, container);
        }
    }
}
