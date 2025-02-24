using Minecraft_Modded_Launcher.Controllers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// Init.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Init : Window
    {
        private AnimationController animationController = new AnimationController();
        public static MainApplication application = new MainApplication();

        public Init()
        {
            InitializeComponent();
            mainborder.Opacity = 0;
            mainborder.Height = 0;
            Grid_main.Opacity = 0;
        }

        private async void window_loaded(object sender, RoutedEventArgs e)
        {
            animationController.BeginAnimation(mainborder, 0, 720, HeightProperty, 0.7, System.Windows.Media.Animation.EasingMode.EaseInOut, new QuarticEase());
            animationController.BeginAnimation(mainborder, 0, 1, OpacityProperty, 0.5, System.Windows.Media.Animation.EasingMode.EaseOut, new QuarticEase());
            await Task.Delay(400);
            animationController.BeginAnimation(Grid_main, 0,1, OpacityProperty,0.4,EasingMode.EaseOut, new SineEase());
            await Task.Delay(new Random().Next(1200,1700));

            // 먼저 마인크래프트 프로세스 확인 및 종료
            Process[] minecraftProcesses = Process.GetProcessesByName("minecraft"); // "minecraft.exe" 또는 "javaw.exe" 등
            Process[] javawProcesses = Process.GetProcessesByName("javaw");
            if (minecraftProcesses.Length > 0 || javawProcesses.Length > 0)
            {
                HandyControl.Controls.MessageBox.Show("마인크래프트 런처가 실행 중입니다. 마인크래프트를 종료합니다.", "알림", MessageBoxButton.OK, MessageBoxImage.Information);

                // 마인크래프트 관련 프로세스 종료 (주의: 강제 종료)
                foreach (var process in minecraftProcesses)
                {
                    try
                    {
                        process.Kill(); // 프로세스 강제 종료
                        process.WaitForExit(); // 프로세스가 종료될 때까지 기다림 (선택 사항)
                    }
                    catch (Exception ex)
                    {
                        //종료 시도중 오류 무시.
                    }
                }
                foreach (var process in javawProcesses)
                {
                    try
                    {
                        process.Kill(); // 프로세스 강제 종료
                        process.WaitForExit(); // 프로세스가 종료될 때까지 기다림 (선택 사항)
                    }
                    catch (Exception ex)
                    {
                        //종료 시도중 오류 무시.
                    }
                }
            }

            animationController.BeginAnimation(mainborder, 1, 0, OpacityProperty, 0.4, EasingMode.EaseOut, new SineEase());
            await Task.Delay(400);
            this.Close();
            application.Show();
        }
    }
}
