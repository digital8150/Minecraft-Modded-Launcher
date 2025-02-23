using Minecraft_Modded_Launcher.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Minecraft_Modded_Launcher.Views
{
    /// <summary>
    /// MainApplication.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainApplication : Window
    {
        private const int MillisecondsDelay = 125;
        private AnimationController animationController = new AnimationController();

        public MainApplication()
        {
            InitializeComponent();
            mainborder.Opacity = 0;
            mainborder.Height = 0;
            applicationLogo.Opacity = 0;
            borderMinecraftStatus.Opacity = 0;
            borderProfileStatus.Opacity = 0;
            buttonInstall.Opacity = 0;
            carouselMain.Opacity = 0;
        }

        private async void window_loaded(object sender, RoutedEventArgs e)
        {
            animationController.BeginAnimation(mainborder, 0, 720, HeightProperty, 0.7, System.Windows.Media.Animation.EasingMode.EaseInOut, new QuarticEase());
            animationController.BeginAnimation(mainborder, 0, 1, OpacityProperty, 0.5, System.Windows.Media.Animation.EasingMode.EaseOut, new QuarticEase());
            await Task.Delay(700);
            animationController.BeginAnimation(applicationLogo, 0, 1, OpacityProperty, 0.5);
            await Task.Delay(MillisecondsDelay);
            animationController.BeginAnimation(borderMinecraftStatus, 0, 1, OpacityProperty, 0.5);
            await Task.Delay(MillisecondsDelay);
            animationController.BeginAnimation(borderProfileStatus, 0, 1, OpacityProperty, 0.5);
            await Task.Delay(MillisecondsDelay);
            animationController.BeginAnimation(buttonInstall, 0, 1, OpacityProperty, 0.5);
            await Task.Delay(MillisecondsDelay);
            animationController.BeginAnimation(carouselMain, 0, 1, OpacityProperty, 0.5);
            await Task.Delay(MillisecondsDelay);
        }

        private void installModpack(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            button.IsEnabled = false;

            Installer installer = new Installer();
            installer.Show();
        }
    }
}
