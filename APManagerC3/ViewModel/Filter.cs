using Microsoft.Toolkit.Mvvm.ComponentModel;
using System.Collections.Immutable;
using System.Linq;
using Containers = System.Collections.ObjectModel.ObservableCollection<APManagerC3.ViewModel.Container>;

namespace APManagerC3.ViewModel {
    public class Filter : ObservableObject, IViewModel<Model.Filter, Filter> {
        #region 公共属性
        public string Category {
            get => _category;
            set {
                if (value.Length >= 7) {
                    value = value[0..7];
                }
                SetProperty(ref _category, value);
                APManager.SaveRequired = true;
            }
        }
        public Status Status => _status;
        public string Identifier {
            get => _identifier;
            set {
                if (value.StartsWith("#")) {
                    value = value[1..];
                }
                if (value.Length != 6) {
                    return;
                }
                SetProperty(ref _identifier, value);
                foreach (var container in _containers) {
                    container.Identifier = _identifier;
                }
                APManager.SaveRequired = true;
            }
        }
        public Containers Containers => _containers;
        #endregion

        #region 公共方法
        public Container NewContainer() {
            var container = new Container() {
                Title = "新建",
                Identifier = Identifier
            };
            _containers.Add(container);
            return container;
        }
        public void AddContainer(Container container) {
            container.Identifier = _identifier;
            _containers.Add(container);
        }
        public void InsertContainer(int index, Container container) {
            _containers.Insert(index, container);
        }
        public void RemoveContainer(Container container) {
            _containers.Remove(container);
        }
        public void ResortContainer(int index, Container source) {
            _containers.ReInsert(index, source);
        }
        public void ToggleOff() {
            if (_status == Status.Enable) {
                _status = Status.Disable;
            }
            OnPropertyChanged(nameof(Status));
        }
        public void ToggleOn() {
            if (_status == Status.Disable) {
                _status = Status.Enable;
            }
            OnPropertyChanged(nameof(Status));
        }
        public override string ToString() {
            return $"{_category}\n包含容器数：{_containers.Count}";
        }
        public Filter LoadFromModel(Model.Filter model) {
            Identifier = model.Identifier;
            Category = model.Category;
            Containers.Clear();
            foreach (var item in model.Containers) {
                NewContainer().LoadFromModel(item);
            }
            return this;
        }
        public Model.Filter ConvertToModel() {
            return new() {
                Identifier = _identifier,
                Category = _category,
                Containers = ImmutableList.CreateRange<Model.Container>(from container in _containers select container.ConvertToModel())
            };
        }
        #endregion

        private string _category = "";
        private string _identifier = "006AAB";
        private Status _status = Status.Disable;
        private readonly Containers _containers = new();
    }
}
