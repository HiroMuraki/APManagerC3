using System;

namespace APManagerC3 {
    public class FilterToggledEventArgs : EventArgs {
        public string FilterName { get; }
        public Status Status { get; }

        public FilterToggledEventArgs(string filterName, Status status) {
            FilterName = filterName;
            Status = status;
        }
    }
}
