using System;

namespace TestApp.Controls.Events
{
    public class OptionSelectedEventsArgs : EventArgs
    {

        public object ItemSelected { get; set; }
        public bool IsSelected { get; set; }



        public OptionSelectedEventsArgs(object itemSelected, bool isSelected)
        {
            ItemSelected = itemSelected;
            IsSelected = isSelected;
        }
    }
}
