using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Records = System.Collections.ObjectModel.ObservableCollection<APManagerC3.ViewModel.Record>;
using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace APManagerC3.ViewModel {
    public class Container : ObservableObject, IViewModel<Model.Container, Container> {
        #region 公共属性
        public string Identifier {
            get => _identifier;
            set => SetProperty(ref _identifier, value);
        }
        public Status Status => _status;
        public string Title {
            get => _title;
            set {
                SetProperty(ref _title, value);
                APManager.SaveRequired = true;
            }
        }
        public string Description {
            get => _description;
            set {
                SetProperty(ref _description, value);
                APManager.SaveRequired = true;
            }
        }
        public Records Records => _records;
        #endregion

        #region 公共方法
        public Record NewRecord() {
            var record = new Record();
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
            return new() {
                Title = _title,
                Description = _description,
                Records = ImmutableList.CreateRange<Model.Record>(from item in Records select item.ConvertToModel())
            };
        }
        #endregion

        private string _title = "";
        private string _description = "";
        private string _identifier = "";
        private Status _status = Status.Disable;
        private readonly Records _records = new();
    }
}
