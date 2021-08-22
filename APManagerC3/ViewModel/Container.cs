using System.Collections.Generic;
using Records = System.Collections.ObjectModel.ObservableCollection<APManagerC3.ViewModel.Record>;

namespace APManagerC3.ViewModel {
    public class Container : ViewModelBase {
        private string _title;
        private string _description;
        private string _identifier;
        private Status _status;
        private readonly Records _records;


        #region 公共属性
        public string Identifier {
            get {
                return _identifier;
            }
            set {
                _identifier = value;
                OnPropertyChanged(nameof(Identifier));
            }
        }
        public Status Status {
            get {
                return _status;
            }
        }
        public string Title {
            get {
                return _title;
            }
            set {
                _title = value;
                OnPropertyChanged(nameof(Title));
                APManager.SaveRequired = true;
            }
        }
        public string Description {
            get {
                return _description;
            }
            set {
                _description = value;
                OnPropertyChanged(nameof(Description));
                APManager.SaveRequired = true;
            }
        }
        public Records Records {
            get {
                return _records;
            }
        }
        #endregion

        #region 公共方法
        public void AddRecord(Record record) {
            _records.Add(record);
            APManager.SaveRequired = true;
        }
        public void AddRecords(IEnumerable<Record> records) {
            foreach (var record in records) {
                _records.Add(record);
            }
        }
        public void RemoveRecord(Record record) {
            _records.Remove(record);
            APManager.SaveRequired = true;
        }
        public void ResortRecord(Record source, Record target) {
            if (ReferenceEquals(source, target)) {
                return;
            }
            _records.Remove(source);
            _records.Insert(_records.IndexOf(target), source);
            APManager.SaveRequired = true;
        }
        public void ToggleOn() {
            _status = Status.Enable;
            OnPropertyChanged(nameof(Status));
        }
        public void ToggleOff() {
            _status = Status.Disable;
            OnPropertyChanged(nameof(Status));
        }
        public Container GetDeepCopy() {
            Container copy = new Container();
            copy._title = _title;
            copy._description = _description;
            copy._identifier = _identifier;
            foreach (var record in _records) {
                copy.Records.Add(record.GetDeepCopy());
            }
            return copy;
        }
        public override string ToString() {
            return $"{_title}\n{_description}\n包含容器数：{_records.Count}";
        }
        #endregion

        public Container() {
            _title = "";
            _description = "";
            _status = Status.Disable;
            _records = new Records();
        }
    }
}
