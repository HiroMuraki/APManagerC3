using HMUtility.Algorithm;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;
using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Containers = System.Collections.ObjectModel.ObservableCollection<APManagerC3.ViewModel.Container>;
using Filters = System.Collections.ObjectModel.ObservableCollection<APManagerC3.ViewModel.Filter>;

namespace APManagerC3.ViewModel {
    public class Manager : ObservableObject, IViewModel<Model.Manager, Manager> {
        #region 事件
        public event EventHandler<CurrentContainerChangedEventArgs>? CurrentContainerChanged;
        public event EventHandler<CurrentFilterChangedEventArgs>? CurrentFilterChanged;
        #endregion

        #region 公共属性
        public bool CanSortContainers => IsValidCurrentContainer();
        public bool CanAddContainer => IsValidCurrentFilter();
        public bool CanAddRecord => IsValidCurrentContainer();
        public Filters Filters { get; } = new();
        public Containers DisplayedContainers { get; } = new();
        public Filter CurrentFilter { get; private set; } = _noFilter;
        public Container CurrentContainer { get; private set; } = _noContainer;
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
            if (!ReferenceEquals(CurrentFilter, _noFilter)) {
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
                CurrentContainer = _noContainer;
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
        public Filter NewFilter() {
            var newFilter = new Filter() { Category = "新标签" };
            Filters.Add(newFilter);
            SetCurrentFilter(newFilter);
            APManager.SaveRequired = true;
            return newFilter;
        }
        public void RemoveFilter(Filter filter) {
            Filters.Remove(filter);
            // 如果是当前选择的过滤器，则重置
            if (ReferenceEquals(CurrentFilter, filter)) {
                CurrentFilter = _noFilter;
                CurrentContainer = _noContainer;
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
        public Container? NewContainer() {
            if (!IsValidCurrentFilter()) {
                return null;
            }
            var newContainer = CurrentFilter.NewContainer();
            DisplayedContainers.Add(newContainer);
            SetCurrentContainer(newContainer);
            APManager.SaveRequired = true;
            return newContainer;
        }
        public void RemoveContainer(Container container) {
            if (!IsValidCurrentFilter()) {
                return;
            }
            CurrentFilter.RemoveContainer(container);
            if (DisplayedContainers.Contains(container)) {
                DisplayedContainers.Remove(container);
            }
            if (CurrentContainer == container) {
                CurrentContainer = _noContainer;
                OnCurrentContainerChanged();
            }
            APManager.SaveRequired = true;
        }
        public void DuplicateContainer(Container container) {
            if (!IsValidCurrentFilter()) {
                return;
            }
            var duplication = NewContainer()?.LoadFromModel(container.ConvertToModel().GetDeepCopy());
            if (duplication != null) {
                int insertIndex = CurrentFilter.Containers.IndexOf(container);
                CurrentFilter.ResortContainer(insertIndex, duplication);
                DisplayedContainers.ReInsert(insertIndex, duplication);
                SetCurrentContainer(duplication);
            }

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
                return;
            }

            if (IsValidCurrentFilter()) {
                _preFilter = CurrentFilter;
            }
            CurrentFilter = _noFilter;
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
            using (var writer = new StreamWriter(filePath)) {
                writer.Write(JsonConvert.SerializeObject(ConvertToModel().Encrypt(encryptor)));
            }
            APManager.SaveRequired = false;
        }
        public void LoadProfile(string filePath, ITextEncryptor encryptor) {
            using (var reader = new StreamReader(filePath)) {
                var r = JsonConvert.DeserializeObject<Model.Manager>(reader.ReadToEnd())?.Decrypt(encryptor);
                if (r is not null) {
                    LoadFromModel(r);
                }
            }
            APManager.SaveRequired = false;
        }
        public Manager LoadFromModel(Model.Manager model) {
            Filters.Clear();
            DisplayedContainers.Clear();
            foreach (var filterModel in model.APMData) {
                NewFilter()?.LoadFromModel(filterModel);
            }

            SetCurrentFilter(_noFilter);

            return this;
        }
        public Model.Manager ConvertToModel() {
            return new Model.Manager() {
                APMData = ImmutableList.CreateRange<Model.Filter>(from filter in Filters select filter.ConvertToModel()),
            };
        }
        #endregion

        private static readonly Manager _singletonInstance = new();
        private static readonly Filter _noFilter = new();
        private static readonly Container _noContainer = new();
        private Filter _preFilter = new();
        private bool IsValidCurrentFilter() {
            return !ReferenceEquals(CurrentFilter, _noFilter);
        }
        private bool IsValidCurrentContainer() {
            return !ReferenceEquals(CurrentContainer, _noContainer);
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
