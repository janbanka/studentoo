using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace studentoo
{
    public static class SnackbarService
    {
        private static Border _snackbarContainer;
        private static TextBlock _snackbarMessage;
        private static DispatcherTimer _timer;

        public static void Initialize(Border container, TextBlock messageBlock)
        {
            _snackbarContainer = container;
            _snackbarMessage = messageBlock;
            _timer = new DispatcherTimer();
            _timer.Tick += HideSnackbar;
        }

        public static void Show(string message, int duration = 3000)
        {
            if (_snackbarContainer == null || _snackbarMessage == null)
                throw new InvalidOperationException("SnackbarService nie został zainicjalizowany");

            Application.Current.Dispatcher.Invoke(() =>
            {
                _snackbarMessage.Text = message;
                _snackbarContainer.Visibility = Visibility.Visible;

                var showAnimation = new DoubleAnimation(1, TimeSpan.FromMilliseconds(300));
                _snackbarContainer.BeginAnimation(UIElement.OpacityProperty, showAnimation);

                _timer.Stop();
                _timer.Interval = TimeSpan.FromMilliseconds(duration);
                _timer.Start();
            });
        }

        private static void HideSnackbar(object sender, EventArgs e)
        {
            _timer.Stop();

            var hideAnimation = new DoubleAnimation(0, TimeSpan.FromMilliseconds(500));
            hideAnimation.Completed += (s, args) =>
            {
                _snackbarContainer.Visibility = Visibility.Collapsed;
            };
            _snackbarContainer.BeginAnimation(UIElement.OpacityProperty, hideAnimation);
        }
    }
}