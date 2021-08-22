using APManagerC3.ViewModel;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace APManagerC3 {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private readonly DoubleAnimation _filterAreaExpandAnimation = new DoubleAnimation() {
            AccelerationRatio = 0.2,
            DecelerationRatio = 0.8,
            Duration = TimeSpan.FromMilliseconds(150)
        };

        public Manager Manager { get; }

        public MainWindow() {
            Manager = Manager.GetInstance();
            InitializeComponent();
            GridRoot.MaxWidth = SystemParameters.WorkArea.Width;
            GridRoot.MaxHeight = SystemParameters.WorkArea.Height;
            if (APManager.ProfileExisted) {
                VerifyLayer.ShowLoadLayer();
            }
        }

        #region 数据交互
        private void Filter_Add(object sender, RoutedEventArgs e) {
            Manager.AddFilter(new Filter() { Category = "新标签" });
            FilterScroller.ScrollToEnd();
        }
        private void Filter_Remove(object sender, RoutedEventArgs e) {
            Manager.RemoveFilter(GetFilterFrom(sender));
        }
        private void Filter_Edit(object sender, RoutedEventArgs e) {
            FilterSettingPanel.DataContext = GetFilterFrom(sender);
            FilterSettingPanel.IsOpen = true;
            FilterNameInputBox.Focus();
            FilterNameInputBox.SelectAll();
        }
        private void Container_Add(object sender, RoutedEventArgs e) {
            Manager.AddContainer(new Container() { Title = "新建" });
            ContainerScroller.ScrollToEnd();
        }
        private void Container_Remove(object sender, RoutedEventArgs e) {
            Manager.RemoveContainer(GetContainerFrom(sender));
        }
        private void Container_Duplicate(object sender, RoutedEventArgs e) {
            Manager.DuplicateContainer(GetContainerFrom(sender));
        }
        private void Record_Add(object sender, RoutedEventArgs e) {
            Manager.CurrentContainer.AddRecord(new Record());
            RecordScroller.ScrollToEnd();
        }
        private void Record_Remove(object sender, RoutedEventArgs e) {
            Manager.CurrentContainer.RemoveRecord(GetRecordFrom(sender));
        }
        private void SaveProfile_Click(object sender, RoutedEventArgs e) {
            VerifyLayer.ShowSaveLayer();
        }
        private void LoadProfile_Click(object sender, RoutedEventArgs e) {
            VerifyLayer.ShowLoadLayer();
        }
        private void SearchBar_TextChanged(object sender, TextChangedEventArgs e) {
            Manager.SearchContainer((sender as TextBox)?.Text);
        }
        private void Load_Click(object sender, RoutedEventArgs e) {
            try {
                string password = VerifyLayer.LoginPassword;
                // 如果密码为空，不加密
                if (string.IsNullOrEmpty(password)) {
                    APManager.Encrypter = new NoEncrypter();
                }
                else {
                    APManager.Encrypter = new AESEncrypter(password);
                }
                Manager.LoadProfile(APManager.ProfileFile, APManager.Encrypter);
                VerifyLayer.Hide();
                SearchBox.Focus();
            }
            catch (Exception exp) {
                MessageBox.Show(exp.Message, "读取错误", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        private void Save_Click(object sender, RoutedEventArgs e) {
            try {
                string password = VerifyLayer.SavePassword;
                // 如果密码为空，不加密
                if (string.IsNullOrEmpty(password)) {
                    APManager.Encrypter = new NoEncrypter();
                }
                else {
                    APManager.Encrypter = new AESEncrypter(password);
                }
                Manager.SaveProfile(APManager.ProfileFile, APManager.Encrypter);
                VerifyLayer.Hide();
            }
            catch (Exception exp) {
                MessageBox.Show(exp.Message, "保存错误", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        #endregion

        #region 拖动排序
        private async void Filter_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            Manager.SetCurrentFilter(GetFilterFrom(sender));
            bool isOk = await Task.Run(() => {
                for (int i = 0; i < 4; i++) {
                    Task.Delay(TimeSpan.FromMilliseconds(50)).Wait();
                    if (e.LeftButton != MouseButtonState.Pressed) {
                        return false;
                    }
                }
                return true;
            });
            if (isOk) {
                DataObject data = new DataObject(GetFilterFrom(sender));
                DragDrop.DoDragDrop(FilterList, data, DragDropEffects.Move);
            }
        }
        private void Filter_Drop(object sender, DragEventArgs e) {
            Filter target = GetFilterFrom(sender);
            if (target == null) {
                return;
            }
            Filter source = e.Data.GetData(typeof(Filter)) as Filter;
            if (source == null) {
                Container container = e.Data.GetData(typeof(Container)) as Container;
                if (container == null) {
                    return;
                }
                Manager.ChangeContainerFilter(container, target);
                return;
            }
            Manager.ResortFilter(source, target);
        }
        private void FilterScroller_DragOver(object sender, DragEventArgs e) {
            Point pos = e.GetPosition(FilterScroller);
            if (pos.Y < 50) {
                FilterScroller.ScrollToVerticalOffset(FilterScroller.VerticalOffset - 50 - pos.Y);
            }
            else if (pos.Y > (FilterScroller.ActualHeight - 50)) {
                FilterScroller.ScrollToVerticalOffset(FilterScroller.VerticalOffset + (pos.Y - (FilterScroller.ActualHeight - 50)));
            }
        }
        private async void Container_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            Manager.SetCurrentContainer(GetContainerFrom(sender));
            bool isOk = await Task.Run(() => {
                for (int i = 0; i < 4; i++) {
                    Task.Delay(TimeSpan.FromMilliseconds(50)).Wait();
                    if (e.LeftButton != MouseButtonState.Pressed) {
                        return false;
                    }
                }
                return true;
            });
            if (isOk) {
                DataObject data = new DataObject(GetContainerFrom(sender));
                DragDrop.DoDragDrop(CurrentContainerList, data, DragDropEffects.Move);
            }
        }
        private void Container_Drop(object sender, DragEventArgs e) {
            Container source = e.Data.GetData(typeof(Container)) as Container;
            if (source == null) {
                return;
            }
            Container target = GetContainerFrom(sender);
            if (target == null) {
                return;
            }
            if (Manager.CanSortContainers) {
                Manager.ResortContainer(source, target);
            }
        }
        private void ContainerScroller_DragOver(object sender, DragEventArgs e) {
            Point pos = e.GetPosition(ContainerScroller);
            if (pos.Y < 50) {
                ContainerScroller.ScrollToVerticalOffset(ContainerScroller.VerticalOffset - (50 - pos.Y));
            }
            if (pos.Y > 410) {
                ContainerScroller.ScrollToVerticalOffset(ContainerScroller.VerticalOffset + (pos.Y - 410));
            }
        }
        private async void Record_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            bool isOk = await Task.Run(() => {
                for (int i = 0; i < 4; i++) {
                    Task.Delay(TimeSpan.FromMilliseconds(50)).Wait();
                    if (e.LeftButton != MouseButtonState.Pressed) {
                        return false;
                    }
                }
                return true;
            });
            if (isOk) {
                DataObject data = new DataObject(GetRecordFrom(sender));
                DragDrop.DoDragDrop(CurrentContainerList, data, DragDropEffects.Move);
            }
        }
        private void Record_Drop(object sender, DragEventArgs e) {
            Record source = e.Data.GetData(typeof(Record)) as Record;
            if (source == null) {
                return;
            }
            Record target = GetRecordFrom(sender);
            if (target == null) {
                return;
            }
            Manager.CurrentContainer.ResortRecord(source, target);
        }
        private void RecordScroller_DragOver(object sender, DragEventArgs e) {
            Point pos = e.GetPosition(RecordScroller);
            if (pos.Y < 50) {
                RecordScroller.ScrollToVerticalOffset(RecordScroller.VerticalOffset - 50 - pos.Y);
            }
            else if (pos.Y > (RecordScroller.ActualHeight - 50)) {
                RecordScroller.ScrollToVerticalOffset(RecordScroller.VerticalOffset + (pos.Y - (RecordScroller.ActualHeight - 50)));
            }
        }
        #endregion

        private void Window_KeyDown(object sender, KeyEventArgs e) {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) && Keyboard.IsKeyDown(Key.S)) {
                SaveProfile_Click(null, null);
            }
        }
        private void Window_Move(object sender, MouseButtonEventArgs e) {
            if (e.ClickCount >= 2) {
                Window_Maximum(null, null);
            }
            else {
                DragMove();
            }
        }
        private void Window_Minimum(object sender, RoutedEventArgs e) {
            WindowState = WindowState.Minimized;
        }
        private void Window_Maximum(object sender, RoutedEventArgs e) {
            if (WindowState != WindowState.Maximized) {
                WindowState = WindowState.Maximized;
            }
            else {
                WindowState = WindowState.Normal;
            }
        }
        private void Window_Close(object sender, RoutedEventArgs e) {
            if (APManager.SaveRequired) {
                var result = MessageBox.Show("当前有更改未保存，是否关闭？", "未保存关闭", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                if (result != MessageBoxResult.OK) {
                    return;
                }
            }
            Close();
        }
        private void LoginBox_KeyDown(object sender, KeyEventArgs e) {
            if (e.Key == Key.Enter) {
                Load_Click(null, null);
            }
        }
        private void FilterArea_MouseEnter(object sender, MouseEventArgs e) {
            _filterAreaExpandAnimation.To = 135;
            FilterArea.BeginAnimation(WidthProperty, _filterAreaExpandAnimation);
        }
        private void FilterArea_MouseLeave(object sender, MouseEventArgs e) {
            _filterAreaExpandAnimation.To = 85;
            FilterArea.BeginAnimation(WidthProperty, _filterAreaExpandAnimation);
        }
        private void FilterArea_DragEnter(object sender, DragEventArgs e) {
            FilterArea_MouseEnter(null, null);
        }
        private void RecordsArea_DragEnter(object sender, DragEventArgs e) {
            if (Manager.CurrentContainer == null) {
                return;
            }
            if (!IsValidDataDrag(e.Data.GetFormats())) {
                return;
            }
            FileLoadHotArea.IsHitTestVisible = true;
        }
        private void RecordsArea_DragLeave(object sender, DragEventArgs e) {
            FileLoadHotArea.IsHitTestVisible = false;

        }
        private void RecordsArea_Drop(object sender, DragEventArgs e) {
            var formatList = e.Data.GetFormats();
            try {
                // 尝试以文本读取
                if (formatList.Contains(DataFormats.Text)) {
                    var text = e.Data.GetData(DataFormats.Text) as string;
                    Manager.CurrentContainer?.AddRecords(Record.GetRecordsByText(text));
                }
                // 尝试以文件列表读取
                else if (formatList.Contains(DataFormats.FileDrop)) {
                    var fileList = e.Data.GetData(DataFormats.FileDrop) as string[];
                    foreach (var file in fileList) {
                        Manager.CurrentContainer?.AddRecords(Record.GetRecordsByFile(file));
                    }
                }

            }
            catch (Exception exp) {
                MessageBox.Show(exp.Message, "读取出错", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            finally {
                FileLoadHotArea.IsHitTestVisible = false;
            }

        }
        private void FilterSettingPanel_MouseLeave(object sender, MouseEventArgs e) {
            FilterSettingPanel.IsOpen = false;
        }

        private Filter GetFilterFrom(object sender) {
            return (sender as FrameworkElement).Tag as Filter;
        }
        private Container GetContainerFrom(object sender) {
            return (sender as FrameworkElement).Tag as Container;
        }
        private Record GetRecordFrom(object sender) {
            return (sender as FrameworkElement).Tag as Record;
        }
        private bool IsValidDataDrag(string[] formatList) {
            // 纯文本
            if (formatList.Contains(DataFormats.Text)) {
                return true;
            }
            // 尝试以文件列表读取
            else if (formatList.Contains(DataFormats.FileDrop)) {
                return true;
            }
            return false;
        }
    }
}
