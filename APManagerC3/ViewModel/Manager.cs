using HMUtility.Algorithm;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text.RegularExpressions;
using Containers = System.Collections.ObjectModel.ObservableCollection<APManagerC3.ViewModel.Container>;
using Filters = System.Collections.ObjectModel.ObservableCollection<APManagerC3.ViewModel.Filter>;

namespace APManagerC3.ViewModel {
    public class Manager : ViewModelBase, IViewModel<Model.Manager, Manager> {
        public static readonly Filter NoFilter = new Filter();
        public static readonly Container NoContainer = new Container();

        #region 事件
        public event EventHandler<CurrentContainerChangedEventArgs>? CurrentContainerChanged;
        public event EventHandler<CurrentFilterChangedEventArgs>? CurrentFilterChanged;
        #endregion

        #region 公共属性
        public bool CanSortContainers {
            get {
                return IsValidCurrentFilter();
            }
        }
        public bool CanAddContainer {
            get {
                return IsValidCurrentFilter();
            }
        }
        public bool CanAddRecord {
            get {
                return IsValidCurrentContainer();
            }
        }
        public Filters Filters { get; } = new Filters();
        public Containers DisplayedContainers { get; } = new Containers();
        public Filter CurrentFilter { get; private set; } = NoFilter;
        public Container CurrentContainer { get; private set; } = NoContainer;
        #endregion

        #region 公共方法
        public static Manager GetInstance() {
            if (_singletonInstance == null) {
                throw new Exception("FATAL ERROR, RESTART REQUIRED");
            }
            return _singletonInstance;
        }
        public void SetCurrentFilter(Filter filter) {
            if (ReferenceEquals(CurrentFilter, filter)) {
                return;
            }
            foreach (var item in Filters) {
                item.ToggleOff();
            }
            CurrentFilter = filter;
            if (!ReferenceEquals(CurrentFilter, NoFilter)) {
                filter.ToggleOn();
                DisplayedContainers.Clear();
                foreach (var item in filter.Containers) {
                    DisplayedContainers.Add(item);
                }
                OnDisplayedContainersChanged();
            }
            OnCurrentFilterChanged();
            // 设置容器状态，如果有启用状态的容器，则选中，否则设为null
            bool containerFocused = false;
            foreach (var item in DisplayedContainers) {
                if (item.Status == Status.Enable) {
                    SetCurrentContainer(item);
                    containerFocused = true;
                    break;
                }
            }
            if (!containerFocused) {
                CurrentContainer = NoContainer;
                OnCurrentContainerChanged();
            }
        }
        public void SetCurrentContainer(Container container) {
            if (ReferenceEquals(CurrentContainer, container)) {
                return;
            }
            foreach (var item in DisplayedContainers) {
                item.ToggleOff();
            }
            container.ToggleOn();
            CurrentContainer = container;
            OnCurrentContainerChanged();
            CurrentContainerChanged?.Invoke(this, new CurrentContainerChangedEventArgs());
        }
        public void NewFilter() {
            Filter filter = new Filter() { Category = "新标签" };
            Filters.Add(filter);
            SetCurrentFilter(filter);
            APManager.SaveRequired = true;
        }
        public void AddFilter(Filter filter) {
            Filters.Add(filter);
            APManager.SaveRequired = true;
        }
        public void RemoveFilter(Filter filter) {
            Filters.Remove(filter);
            // 如果是当前选择的过滤器，则重置
            if (ReferenceEquals(CurrentFilter, filter)) {
                CurrentFilter = NoFilter;
                CurrentContainer = NoContainer;
                DisplayedContainers.Clear();
                OnCurrentFilterChanged();
                OnCurrentContainerChanged();
                OnDisplayedContainersChanged();
            }
            APManager.SaveRequired = true;
        }
        public void ResortFilter(int newInex, Filter source) {
            Filters.ReInsert(newInex, source);
            APManager.SaveRequired = true;
        }
        public void NewContainer() {
            if (!IsValidCurrentFilter()) {
                return;
            }
            var newContianer = CurrentFilter.NewContainer();
            DisplayedContainers.Add(newContianer);
            SetCurrentContainer(newContianer);
            APManager.SaveRequired = true;
        }
        public void AddContainer(Container container) {
            if (!IsValidCurrentFilter()) {
                return;
            }
            CurrentFilter.AddContainer(container);
            APManager.SaveRequired = true;
        }
        public void RemoveContainer(Container container) {
            if (!IsValidCurrentFilter()) {
                return;
            }
            CurrentFilter.RemoveContainer(container);
            if (CurrentContainer == container) {
                CurrentContainer = NoContainer;
                OnCurrentContainerChanged();
            }
            APManager.SaveRequired = true;
        }
        public void DuplicateContainer(Container container) {
            if (!IsValidCurrentFilter()) {
                return;
            }
            CurrentFilter.DuplicateContainer(container);
            APManager.SaveRequired = true;
        }
        public void ResortContainer(int index, Container source) {
            CurrentFilter.ResortContainer(index, source);
            APManager.SaveRequired = true;
        }
        public void SearchContainer(string key) {
            DisplayedContainers.Clear();
            OnDisplayedContainersChanged();
            if (string.IsNullOrEmpty(key)) {
                SetCurrentFilter(_preFilter);
            } else {
                if (IsValidCurrentFilter()) {
                    _preFilter = CurrentFilter;
                }
                CurrentFilter = NoFilter;
                OnCurrentFilterChanged();
                var keyReg = new Regex($"{key.ToUpper()}");
                foreach (var filter in Filters) {
                    foreach (var container in filter.Containers) {
                        if (keyReg.IsMatch(container.Title.ToUpper()) || keyReg.IsMatch(container.Description.ToUpper())) {
                            DisplayedContainers.Add(container);
                        }
                    }
                }
                if (DisplayedContainers.Count > 0) {
                    SetCurrentContainer(DisplayedContainers[0]);
                }
            }
        }
        public void ChangeContainerFilter(Container container, Filter newFilter) {
            if (ReferenceEquals(CurrentFilter, newFilter)) {
                return;
            }
            CurrentFilter.RemoveContainer(container);
            newFilter.AddContainer(container);
            APManager.SaveRequired = true;
        }
        public void SaveProfile(string filePath, ITextEncryptor encryptor) {
            // 加密并储存
            using (StreamWriter reader = new StreamWriter(filePath)) {
                reader.Write(JsonConvert.SerializeObject(ConvertToModel().GetEncrypt(encryptor)));
            }
            APManager.SaveRequired = false;
        }
        public void LoadProfile(string filePath, ITextEncryptor encryptor) {
            using (StreamReader reader = new StreamReader(filePath)) {
                if (JsonConvert.DeserializeObject<Model.Manager>(reader.ReadToEnd()) is Model.Manager managerModel) {
                    LoadFromModel(managerModel.GetDecrypt(encryptor));
                }
            }
            APManager.SaveRequired = false;
        }
        public void LoadFromModel(Model.Manager model) {
            Filters.Clear();
            DisplayedContainers.Clear();
            CurrentFilter = NoFilter;
            CurrentContainer = NoContainer;
            OnCurrentFilterChanged();
            OnCurrentContainerChanged();
            foreach (var filterModel in model.APMData) {
                Filter filter = new Filter() {
                    Category = filterModel.Category,
                    Identifier = filterModel.Identifier,
                };
                Filters.Add(filter);
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
        }
        public Model.Manager ConvertToModel() {
            var result = new Model.Manager();

            foreach (var item in Filters) {
                result.APMData.Add(item.ConvertToModel());
            }

            return result;
        }
        #endregion

        private static readonly Manager _singletonInstance = new Manager();
        private static readonly DataContractJsonSerializer _serializer = new DataContractJsonSerializer(typeof(Model.Manager));
        private Filter _preFilter = NoFilter;
        private bool IsValidCurrentFilter() {
            return !ReferenceEquals(CurrentFilter, NoFilter);
        }
        private bool IsValidCurrentContainer() {
            return !ReferenceEquals(CurrentContainer, NoContainer);
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
    }
}
