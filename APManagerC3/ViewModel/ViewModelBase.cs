using System.ComponentModel;

namespace APManagerC3.ViewModel {
    public class ViewModelBase : INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
