using Minecraft_Modded_Launcher.Controllers;
using System;
using System.Collections.Generic;
using System.IO;
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
using Path = System.IO.Path;

namespace Minecraft_Modded_Launcher.Views
{
    /// <summary>
    /// MainApplication.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainApplication : Window
    {
        string minecraftFolderPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ".minecraft");
        string backupFolderPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ".minecraft_backup");

        public Boolean isMcrohdongInstalled = false;

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
            buttonInstall.Visibility = Visibility.Collapsed;
            buttonRevoke.Visibility = Visibility.Collapsed;
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
            if (Path.Exists(minecraftFolderPath))
            {
                textMinecraftStatus.Text = "마인크래프트가 설치되어 있습니다.";
            }
            else
            {
                textMinecraftStatus.Text = "마인크래프트가 설치되어 있지 않습니다.";
            }

            if (Path.Exists(backupFolderPath))
            {
                textBackupStatus.Text = "MCROHDONG이 설치되어 있으며 백업파일이 존재합니다.";
                isMcrohdongInstalled = true;
            }
            else
            {
                textBackupStatus.Text = "MCROHDONG이 설치되어 있지 않습니다.";
                isMcrohdongInstalled = false;
            }
            await Task.Delay(MillisecondsDelay);
            updateButtonState();

        }

        public void updateButtonState()
        {
            if (isMcrohdongInstalled)
            {
                buttonInstall.Visibility = Visibility.Collapsed;
                buttonRevoke.Visibility = Visibility.Visible;
                buttonRevoke.IsEnabled = true;
            }
            else
            {
                buttonInstall.Visibility = Visibility.Visible;
                buttonInstall.IsEnabled = true;
                buttonRevoke.Visibility = Visibility.Collapsed;
            }
            animationController.BeginAnimation(buttonInstall, 0, 1, OpacityProperty);
            animationController.BeginAnimation(buttonRevoke, 0, 1, OpacityProperty);
        }

        private void installModpack(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            button.IsEnabled = false;

            Installer installer = new Installer();
            installer.Show();
        }

        private void borderDragMove(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private async void closeApp(object sender, RoutedEventArgs e)
        {
            // 종료 애니메이션 (시작 애니메이션의 역순)
            int MillisecondsDelay = 125; // 각 애니메이션 사이의 지연 시간 (필요에 따라 조정)
            animationController.BeginAnimation(carouselMain, 1, 0, OpacityProperty, 0.5);
            await Task.Delay(MillisecondsDelay);
            animationController.BeginAnimation(buttonInstall, 1, 0, OpacityProperty, 0.5);
            await Task.Delay(MillisecondsDelay);
            animationController.BeginAnimation(borderProfileStatus, 1, 0, OpacityProperty, 0.5);
            await Task.Delay(MillisecondsDelay);
            animationController.BeginAnimation(borderMinecraftStatus, 1, 0, OpacityProperty, 0.5);
            await Task.Delay(MillisecondsDelay);
            animationController.BeginAnimation(applicationLogo, 1, 0, OpacityProperty, 0.5);
             // 마지막 delay는 mainborder에 적용.
            animationController.BeginAnimation(mainborder, 1, 0, OpacityProperty, 0.7);
            await Task.Delay(700);
            this.Close();
        }

        private async void revokeModpack(object sender, RoutedEventArgs e)
        {
            if(HandyControl.Controls.MessageBox.Show("계속하시면 리빌드가 삭제되며 이전에 백업해두었던 환경으로 복구합니다. 계속하시겠습니까?", null, MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
            {
                return;
            }

            Button button = (Button) sender;
            button.IsEnabled = false;
            try
            {
                // 1. .minecraft 폴더가 존재하는지 확인 (삭제 대상)
                if (Directory.Exists(minecraftFolderPath))
                {
                    // .minecraft 폴더 삭제 (재귀적으로)
                    await Task.Run(() => Directory.Delete(minecraftFolderPath, true));
                }
                else
                {
                    MessageBox.Show(".minecraft 폴더가 존재하지 않습니다.", "알림", MessageBoxButton.OK, MessageBoxImage.Information);
                    //return;  // .minecraft 폴더가 없으면 여기서 중단할지, 아니면 백업 폴더 복원 시도할지 결정
                }


                // 2. .minecraft_backup 폴더가 존재하는지 확인 (복원 대상)
                if (Directory.Exists(backupFolderPath))
                {
                    // .minecraft_backup 폴더 이름을 .minecraft로 변경
                    await Task.Run(() => Directory.Move(backupFolderPath, minecraftFolderPath));
                    MessageBox.Show("모드팩 복구가 완료되었습니다.", "완료", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show(".minecraft_backup 폴더를 찾을 수 없습니다.  복구할 수 없습니다.", "오류", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                isMcrohdongInstalled = false;
                
            }
            catch (Exception ex)
            {
                MessageBox.Show($"복구 중 오류 발생: {ex.Message}", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                updateButtonState();
            }
        }
    }
}
