using System;
using Containers = System.Collections.ObjectModel.ObservableCollection<APManagerC3.ViewModel.Container>;

namespace APManagerC3.ViewModel {

    public class Filter : ViewModelBase {
        private string _category;
        private Status _status;
        private string _identifier;
        private readonly Containers _containers;

        #region 公共事件
        public EventHandler<FilterToggledEventArgs> FilterToggled;
        #endregion

        #region 公共属性
        public string Category {
            get {
                return _category;
            }
            set {
                if (value.Length >= 7) {
                    value = value[0..7];
                }
                _category = value;
                OnPropertyChanged(nameof(Category));
                APManager.SaveRequired = true;
            }
        }
        public Status Status {
            get {
                return _status;
            }
        }
        public string Identifier {
            get {
                return _identifier;
            }
            set {
                if (value.StartsWith("#")) {
                    value = value[1..];
                }
                if (value.Length != 6) {
                    return;
                }
                _identifier = value;
                OnPropertyChanged(nameof(Identifier));
                foreach (var container in _containers) {
                    container.Identifier = _identifier;
                }
                APManager.SaveRequired = true;
            }
        }
        public Containers Containers {
            get {
                return _containers;
            }
        }
        #endregion

        #region 公共方法
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
        public void ResortContainer(Container source, Container target) {
            if (ReferenceEquals(source, target)) {
                return;
            }
            if (!_containers.Contains(source) || !_containers.Contains(target)
                || !_containers.Contains(source) || !_containers.Contains(target)) {
                return;
            }
            _containers.Remove(source);
            _containers.Insert(_containers.IndexOf(target), source);
        }
        public void DuplicateContainer(Container container) {
            _containers.Insert(_containers.IndexOf(container), container.GetDeepCopy());
        }
        public void Toggle() {
            if (_status == Status.Enable) {
                _status = Status.Disable;
            }
            else {
                _status = Status.Enable;
            }
            OnPropertyChanged(nameof(Status));
            FilterToggled?.Invoke(this, new FilterToggledEventArgs(_category, _status));
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
        #endregion

        public Filter() {
            _category = "";
            _identifier = "006AAB";
            _status = Status.Disable;
            _containers = new Containers();
        }
    }
}
