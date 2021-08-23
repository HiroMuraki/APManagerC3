using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text.RegularExpressions;
using Containers = System.Collections.ObjectModel.ObservableCollection<APManagerC3.ViewModel.Container>;
using Filters = System.Collections.ObjectModel.ObservableCollection<APManagerC3.ViewModel.Filter>;

namespace APManagerC3.ViewModel {
    public class Manager : ViewModelBase {
        private static readonly Manager _singletonInstance;
        private static readonly DataContractJsonSerializer _serializer;
        private readonly Filters _filters;
        private Containers _displayedContainers;
        private Filter _preFilter;
        private Filter _currentFilter;
        private Container _currentContainer;

        #region 事件
        public event EventHandler<CurrentContainerChangedEventArgs> CurrentContainerChanged;
        public event EventHandler<CurrentFilterChangedEventArgs> CurrentFilterChanged;
        #endregion

        #region 公共属性
        public bool CanSortContainers {
            get {
                return _currentFilter != null && _filters.Contains(_currentFilter);
            }
        }
        public bool CanAddContainer {
            get {
                if (_currentFilter == null) {
                    return false;
                }
                return true;
            }
        }
        public bool CanAddRecord {
            get {
                if (_currentContainer == null) {
                    return false;
                }
                return true;
            }
        }
        public bool MultiFilter {
            get {
                return false;
            }
        }
        public Filters Filters {
            get {
                return _filters;
            }
        }
        public Containers DisplayedContainers {
            get {
                return _displayedContainers;
            }
        }
        public Filter CurrentFilter {
            get {
                return _currentFilter;
            }
        }
        public Container CurrentContainer {
            get {
                return _currentContainer;
            }
        }
        #endregion

        #region 公共方法
        public void SetCurrentFilter(Filter filter) {
            if (_currentFilter == filter) {
                return;
            }
            foreach (var item in _filters) {
                item.ToggleOff();
            }
            filter.Toggle();
            _currentFilter = filter;
            OnCurrentFilterChanged();
            // 设置容器状态，如果有启用状态的容器，则选中，否则设为null
            bool containerFocused = false;
            foreach (var item in _displayedContainers) {
                if (item.Status == Status.Enable) {
                    SetCurrentContainer(item);
                    containerFocused = true;
                    break;
                }
            }
            if (!containerFocused) {
                _currentContainer = null;
                OnCurrentContainerChanged();
            }
        }
        public void SetCurrentContainer(Container container) {
            if (_currentContainer == container) {
                return;
            }
            foreach (var item in _displayedContainers) {
                item.ToggleOff();
            }
            container.ToggleOn();
            _currentContainer = container;
            OnCurrentContainerChanged();
            CurrentContainerChanged?.Invoke(this, new CurrentContainerChangedEventArgs());
        }
        public void AddFilter(Filter filter) {
            filter.FilterToggled += FilterToggled;
            _filters.Add(filter);
            SetCurrentFilter(filter);
            APManager.SaveRequired = true;
        }
        public void RemoveFilter(Filter filter) {
            // 如果是当前选择的过滤器，则重置
            if (_currentFilter == filter) {
                _currentFilter = null;
                _currentContainer = null;
                _displayedContainers = null;
                OnCurrentFilterChanged();
                OnCurrentContainerChanged();
                OnDisplayedContainersChanged();
            }
            _filters.Remove(filter);
            APManager.SaveRequired = true;
        }
        public void ResortFilter(int newInex, Filter source) {
            _filters.ReInsert(newInex, source);
            APManager.SaveRequired = true;
        }
        public void AddContainer(Container container) {
            if (_currentFilter == null || !_filters.Contains(_currentFilter)) {
                return;
            }
            _currentFilter.AddContainer(container);
            SetCurrentContainer(container);
            APManager.SaveRequired = true;
        }
        public void RemoveContainer(Container container) {
            if (_currentFilter == null || !_filters.Contains(_currentFilter)) {
                return;
            }
            _currentFilter.RemoveContainer(container);
            if (_currentContainer == container) {
                _currentContainer = null;
                OnCurrentContainerChanged();
            }
            APManager.SaveRequired = true;
        }
        public void DuplicateContainer(Container container) {
            if (_currentFilter == null || !_filters.Contains(_currentFilter)) {
                return;
            }
            _currentFilter.DuplicateContainer(container);
            APManager.SaveRequired = true;
        }
        public void ResortContainer(int index, Container source) {
            _currentFilter.ResortContainer(index, source);
            APManager.SaveRequired = true;
        }
        public void SearchContainer(string key) {
            _displayedContainers = new Containers();
            OnDisplayedContainersChanged();
            if (string.IsNullOrEmpty(key)) {
                SetCurrentFilter(_preFilter);
            }
            else {
                if (_currentFilter != null) {
                    _preFilter = _currentFilter;
                }
                _currentFilter = null;
                OnCurrentFilterChanged();
                Regex keyReg = new Regex($"{key.ToUpper()}");
                foreach (var filter in _filters) {
                    foreach (var container in filter.Containers) {
                        if (keyReg.IsMatch(container.Title.ToUpper()) || keyReg.IsMatch(container.Description.ToUpper())) {
                            _displayedContainers.Add(container);
                        }
                    }
                }
                if (_displayedContainers.Count > 0) {
                    SetCurrentContainer(_displayedContainers[0]);
                }
            }
        }
        public void ChangeContainerFilter(Container container, Filter newFilter) {
            if (ReferenceEquals(_currentFilter, newFilter)) {
                return;
            }
            _currentFilter.RemoveContainer(container);
            newFilter.AddContainer(container);
            APManager.SaveRequired = true;
        }
        public void SaveProfile(string filePath, IEncrypter encrypter) {
            Model.Manager managerModel = new Model.Manager();
            foreach (var filter in _filters) {
                Model.Filter filterModel = new Model.Filter() {
                    Category = filter.Category,
                    Identifier = filter.Identifier
                };
                foreach (var container in filter.Containers) {
                    Model.Container containerModel = new Model.Container() {
                        Title = container.Title,
                        Description = container.Description,
                    };
                    foreach (var record in container.Records) {
                        Model.Record recordModel = new Model.Record() {
                            Label = record.Label,
                            Information = record.Information
                        };
                        containerModel.Records.Add(recordModel);
                    }
                    filterModel.Containers.Add(containerModel);
                }
                managerModel.APMData.Add(filterModel);
            }
            // 加密并储存
            using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write)) {
                if (encrypter != null) {
                    managerModel.EncryptData(encrypter);
                }
                _serializer.WriteObject(fs, managerModel);
            }
            APManager.SaveRequired = false;
        }
        public void LoadProfile(string filePath, IEncrypter encrypter) {
            Model.Manager managerModel = null;
            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read)) {
                managerModel = _serializer.ReadObject(fs) as Model.Manager;
            }
            if (managerModel == null) {
                return;
            }
            if (encrypter != null) {
                managerModel.DecryptData(encrypter);
            }
            _filters.Clear();
            _displayedContainers.Clear();
            _currentFilter = null;
            _currentContainer = null;
            OnCurrentFilterChanged();
            OnCurrentContainerChanged();
            foreach (var filterModel in managerModel.APMData) {
                Filter filter = new Filter() {
                    Category = filterModel.Category,
                    Identifier = filterModel.Identifier,
                };
                AddFilter(filter);
                foreach (var containerModel in filterModel.Containers) {
                    Container container = new Container() {
                        Identifier = filter.Identifier,
                        Title = containerModel.Title,
                        Description = containerModel.Description
                    };
                    foreach (var recordModel in containerModel.Records) {
                        Record record = new Record() {
                            Label = recordModel.Label,
                            Information = recordModel.Information
                        };
                        container.Records.Add(record);
                    }
                    filter.AddContainer(container);
                }
            }
            APManager.SaveRequired = false;
        }
        #endregion
        private void FilterToggled(object sender, FilterToggledEventArgs e) {
            Filter filter = sender as Filter;
            _displayedContainers = filter.Containers;
            OnDisplayedContainersChanged();
        }
        private void OnCurrentFilterChanged() {
            OnPropertyChanged(nameof(CurrentFilter));
            OnPropertyChanged(nameof(CanAddContainer));
            CurrentFilterChanged?.Invoke(this, new CurrentFilterChangedEventArgs());
        }
        private void OnCurrentContainerChanged() {
            OnPropertyChanged(nameof(CurrentContainer));
            OnPropertyChanged(nameof(CanAddRecord));
            CurrentFilterChanged?.Invoke(this, new CurrentFilterChangedEventArgs());
        }
        private void OnDisplayedContainersChanged() {
            OnPropertyChanged(nameof(DisplayedContainers));
        }

        static Manager() {
            _singletonInstance = new Manager();
            _serializer = new DataContractJsonSerializer(typeof(Model.Manager));
        }
        private Manager() {
            _filters = new Filters();
            _displayedContainers = new Containers();
        }
        public static Manager GetInstance() {
            if (_singletonInstance == null) {
                throw new Exception("FATAL ERROR, RESTART REQUIRED");
            }
            return _singletonInstance;
        }

    }
}
