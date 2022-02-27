using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace APManagerC3.View {
    /// <summary>
    /// SaveLoadControl.xaml 的交互逻辑
    /// </summary>
    public partial class SaveLoadControl : UserControl {
        public event RoutedEventHandler? Saved;
        public event RoutedEventHandler? Logined;

        public int MaxPasswordLength {
            get {
                return 16;
            }
        }
        public string LoginPassword {
            get {
                return LoginBox.Password;
            }
        }
        public string SavePassword {
            get {
                string password = SaveBox1.Password;
                if (password != SaveBox2.Password) {
                    throw new Exception("两次输入的密码不相同");
                }
                return password;
            }
        }

        public SaveLoadControl() {
            InitializeComponent();
        }

        public void ShowLoadLayer() {
            HideLayout(SaveLayout);
            ShowLayout(LoadLayer);
            LoginBox.Focus();
            LoginBox.SelectAll();
        }
        public void ShowSaveLayer() {
            HideLayout(LoadLayer);
            ShowLayout(SaveLayout);
            SaveBox1.Focus();

        }
        public void Hide() {
            HideLayout(SaveLayout);
            HideLayout(LoadLayer);
        }

        private readonly DoubleAnimation _layoutDisplayAnimation = new DoubleAnimation() {
            AccelerationRatio = 0.2,
            DecelerationRatio = 0.8,
            Duration = TimeSpan.FromMilliseconds(150)
        };
        private bool _logined;
        private void HideLayout(FrameworkElement element) {
            _layoutDisplayAnimation.To = 0;
            element.IsHitTestVisible = false;
            element.BeginAnimation(OpacityProperty, _layoutDisplayAnimation);
            MainFrame.IsHitTestVisible = false;
            MainFrame.BeginAnimation(OpacityProperty, _layoutDisplayAnimation);
        }
        private void ShowLayout(FrameworkElement element) {
            _layoutDisplayAnimation.To = 1;
            element.IsHitTestVisible = true;
            element.BeginAnimation(OpacityProperty, _layoutDisplayAnimation);
            MainFrame.IsHitTestVisible = true;
            MainFrame.BeginAnimation(OpacityProperty, _layoutDisplayAnimation);
        }
        private void LoginBox_KeyDown(object sender, KeyEventArgs e) {
            if (e.Key == Key.Enter) {
                Load_Click(null!, null!);
            }
        }
        private void Load_Click(object sender, RoutedEventArgs e) {
            string password = LoginBox.Password;
            SaveBox1.Password = password;
            SaveBox2.Password = password;
            _logined = true;
            Logined?.Invoke(this, new RoutedEventArgs());
        }
        private void Save_Click(object sender, RoutedEventArgs e) {
            string password = SaveBox1.Password;
            if (!APManager.IsValidPassword(password)) {
                MessageBox.Show("密码应只含字母，数字与特殊字符（如@）", "保存错误", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (password != SaveBox2.Password) {
                MessageBox.Show("两次输入的密码不相同", "保存错误", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            LoginBox.Password = password;
            Saved?.Invoke(this, new RoutedEventArgs());
        }
        private void Hide_Click(object sender, RoutedEventArgs e) {
            if (_logined) {
                Hide();
            } else {
                Application.Current.Shutdown();
            }
        }
    }
}
