using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows.Input;

using Minecraft_Modded_Launcher.Helpers;

namespace Minecraft_Modded_Launcher.ViewModels
{
    internal class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private string _logText = "";
        public string LogText
        {
            get { return _logText; }
            set { _logText = value; OnPropertyChanged(nameof(LogText)); }
        }

        private double _progressValue;
        public double ProgressValue
        {
            get { return _progressValue; }
            set { _progressValue = value; OnPropertyChanged(nameof(ProgressValue)); }
        }

        private bool _isForgeInstalled;
        public bool IsForgeInstalled
        {
            get { return _isForgeInstalled; }
            set { _isForgeInstalled = value; OnPropertyChanged(nameof(IsForgeInstalled)); }
        }

        private bool _isModpackInstalled;
        public bool IsModpackInstalled
        {
            get { return _isModpackInstalled; }
            set { _isModpackInstalled = value; OnPropertyChanged(nameof(IsModpackInstalled)); }
        }

        public ICommand InstallForgeCommand { get; }
        public ICommand InstallModpackCommand { get; }
        public ICommand LaunchMinecraftCommand { get; }

        public MainViewModel()
        {
            InstallForgeCommand = new RelayCommand(InstallForge);
            InstallModpackCommand = new RelayCommand(InstallModpack);
            LaunchMinecraftCommand = new RelayCommand(LaunchMinecraft, CanLaunchMinecraft); // CanExecute 조건 추가

            // 초기 상태 설정 (테스트용)
            IsForgeInstalled = false;
            IsModpackInstalled = false;

        }

        private void InstallForge()
        {
            // Forge 설치 로직 (구현 예정)
            LogText += "Forge 설치 시작...\n";
            ProgressValue = 0; // 진행률 초기화

            // 예시: 진행률 업데이트 (실제로는 다운로드/설치 과정에 따라 업데이트)
            for (int i = 0; i <= 100; i += 10)
            {
                System.Threading.Thread.Sleep(200); // 데모용 지연
                ProgressValue = i;
                LogText += $"Forge 설치 진행 중... {i}%\n";
            }
            IsForgeInstalled = true; // 설치 완료
            LogText += "Forge 설치 완료!\n";
        }

        private void InstallModpack()
        {
            // 모드팩 설치 로직 (구현 예정)
            LogText += "모드팩 설치 시작...\n";
            // ...
            IsModpackInstalled = true;
            LogText += "모드팩 설치 완료!\n";
        }

        private void LaunchMinecraft()
        {
            // 마인크래프트 실행 로직 (구현 예정)
            LogText += "마인크래프트 실행...\n";
            // ...
        }

        // 마인크래프트를 실행할 수 있는지 확인하는 메서드 (CanExecute)
        private bool CanLaunchMinecraft()
        {
            return IsForgeInstalled && IsModpackInstalled;
        }
    }
}
