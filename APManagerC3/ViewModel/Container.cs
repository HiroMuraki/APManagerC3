using System.Collections.Generic;
using Records = System.Collections.ObjectModel.ObservableCollection<APManagerC3.ViewModel.Record>;

namespace APManagerC3.ViewModel {
    public class Container : ViewModelBase, IViewModel<Model.Container, Container> {
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
        public Record NewRecord() {
            Record record = new Record();
            _records.Add(record);
            APManager.SaveRequired = true;
            return record;
        }
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
        public void ResortRecord(int newIndex, Record source) {
            _records.ReInsert(newIndex, source);
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
        public override string ToString() {
            return $"{_title}\n{_description}\n包含容器数：{_records.Count}";
        }
        public Container LoadFromModel(Model.Container model) {
            Title = model.Title;
            Description = model.Description;
            Records.Clear();
            foreach (var item in model.Records) {
                NewRecord().LoadFromModel(item);
            }
            return this;
        }
        public Model.Container ConvertToModel() {
            var model = new Model.Container();

            model.Title = Title;
            model.Description = Description;
            foreach (var item in Records) {
                model.Records.Add(item.ConvertToModel());
            }

            return model;
        }
        #endregion

        private string _title = "";
        private string _description = "";
        private string _identifier = "";
        private Status _status = Status.Disable;
        private readonly Records _records = new Records();
    }
}
