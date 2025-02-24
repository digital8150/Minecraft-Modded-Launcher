﻿using Minecraft_Modded_Launcher.Controllers;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Path = System.IO.Path;

namespace Minecraft_Modded_Launcher.Views
{
    /// <summary>
    /// Installer.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Installer : Window
    {
        private AnimationController animationController = new AnimationController();
        public Installer()
        {
            InitializeComponent();
            borderApplicaion.Opacity = 0;
        }

        private void closeInstaller(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private async void window_loaded(object sender, RoutedEventArgs e)
        {
            animationController.BeginAnimation(borderApplicaion, 0, 1, OpacityProperty);
            await Task.Delay(1000);
            
            await backupMinecraftFolder();
            spinnerMinecraftBackup.Icon = FontAwesome6.EFontAwesomeIcon.Regular_CircleCheck;
            spinnerMinecraftBackup.Spin = false;
            spinnerMinecraftBackup.Foreground = Brushes.Green;
            await installRebuild();
            spinnerMinecraftInstall.Icon = FontAwesome6.EFontAwesomeIcon.Regular_CircleCheck;
            spinnerMinecraftInstall.Spin = false;
            spinnerMinecraftInstall.Foreground = Brushes.Green;
            await instalJDK8();
            spinnerJavaInstall.Icon = FontAwesome6.EFontAwesomeIcon.Regular_CircleCheck;
            spinnerJavaInstall.Spin = false;
            spinnerJavaInstall.Foreground = Brushes.Green;

            buttonExit.IsEnabled = true;
            buttonExit.Visibility = Visibility.Visible;
            animationController.BeginAnimation(buttonExit, 0, 1, OpacityProperty);
            Init.application.isMcrohdongInstalled = true;
            Init.application.updateButtonState();
        }


        private async Task backupMinecraftFolder()
        {
            try
            {
                // 1. .minecraft 폴더 경로 설정
                string minecraftFolderPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ".minecraft");
                string backupFolderPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ".minecraft_backup");

                // 2. .minecraft 폴더가 존재하는지 확인
                if (Directory.Exists(minecraftFolderPath))
                {
                    // 3. 백업 폴더 경로 설정 (.minecraft_backup)
                    

                    // 4. 이미 .minecraft_backup 폴더가 존재하면 삭제 (덮어쓰기)
                    if (Directory.Exists(backupFolderPath))
                    {
                        Directory.Delete(backupFolderPath, true); // true: 하위 폴더 및 파일 모두 삭제
                    }

                    // 5. .minecraft 폴더 이름을 .minecraft_backup으로 변경
                    Directory.Move(minecraftFolderPath, backupFolderPath);

                    
                }
                else
                {
                    HandyControl.Controls.MessageBox.Show("마인크래프트 폴더가 존재하지 않으므로 백업 하지 않습니다.");
                    Directory.CreateDirectory(backupFolderPath);
                }
            }
            catch (Exception ex)
            {

                HandyControl.Controls.MessageBox.Show($"백업 중 오류가 발생했습니다.\n{ex.Message}", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task installRebuild()
        {
            string minecraftFolderPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ".minecraft");

            if (!Directory.Exists(minecraftFolderPath))
            {
                // LogText += "Re:Build 파일 다운로드 및 설치를 시작합니다...\n";  // 제거
                

                try
                {
                    // 1. 서버에서 Re:Build 파일 (minecraft.zip) 다운로드
                    string rebuildFileUrl = "https://mc2.codingbot.kr/mc2/minecraft.zip";
                    string tempZipPath = Path.Combine(Path.GetTempPath(), "minecraft.zip");

                    using (var client = new HttpClient())
                    {
                        client.Timeout = TimeSpan.FromMinutes(10); // 다운로드 타임아웃

                        using (var response = await client.GetAsync(rebuildFileUrl, HttpCompletionOption.ResponseHeadersRead))
                        {
                            response.EnsureSuccessStatusCode();

                            long? totalBytes = response.Content.Headers.ContentLength;

                            using (var contentStream = await response.Content.ReadAsStreamAsync())
                            using (var fileStream = new FileStream(tempZipPath, FileMode.Create, FileAccess.Write, FileShare.None))
                            {
                                var buffer = new byte[8192];
                                long downloadedBytes = 0;
                                int bytesRead;

                                while ((bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                                {
                                    await fileStream.WriteAsync(buffer, 0, bytesRead);
                                    downloadedBytes += bytesRead;

                                    if (totalBytes.HasValue)
                                    {
                                        
                                        // LogText += $"다운로드 중... {downloadedBytes / 1024 / 1024}MB / {(totalBytes / 1024 / 1024)}MB ({ProgressValue:F1}%)\n"; // 제거
                                    }
                                }
                            }
                        }
                    }


                    // 2. 다운로드한 minecraft.zip 파일의 압축을 .minecraft 폴더에 해제
                    // LogText += "압축 해제 중...\n"; // 제거
                    await Task.Run(() => ZipFile.ExtractToDirectory(tempZipPath, minecraftFolderPath, true));

                    // 3. 임시 파일 삭제
                    File.Delete(tempZipPath);

                    // LogText += "Re:Build 파일 설치 완료!\n"; // 제거
                    HandyControl.Controls.Growl.Info("Re:Build 파일 설치가 완료되었습니다."); // 완료 메시지
                }
                catch (Exception ex)
                {
                    // LogText += $"Re:Build 파일 설치 중 오류 발생: {ex.Message}\n"; // 제거
                    HandyControl.Controls.MessageBox.Show($"Re:Build 파일 설치 중 오류가 발생했습니다.\n{ex.Message}", "오류", MessageBoxButton.OK, MessageBoxImage.Error);

                    //오류 발생시 임시파일 삭제
                    string tempZipPath = Path.Combine(Path.GetTempPath(), "minecraft.zip");
                    if (File.Exists(tempZipPath))
                    {
                        File.Delete(tempZipPath);
                    }
                }
            }
            else
            {
                // LogText += ".minecraft 폴더가 이미 존재하여 Rebuild를 설치 하지 않았습니다.\n"; // 제거
                HandyControl.Controls.MessageBox.Show("백업중 오류가 발생하여 리빌드를 설치하지 않았습니다!", "알림", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        
        private async Task instalJDK8()
        {
            try
            {
                // 1. JDK8 다운로드
                string jdkDownloadUrl = "http://mc2.codingbot.kr/mc2/java8.zip";
                string tempZipPath = Path.Combine(Path.GetTempPath(), "java8.zip");
                string extractionPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location); // 실행 파일 위치

                

                using (var client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromMinutes(5); // 타임아웃 설정

                    using (var response = await client.GetAsync(jdkDownloadUrl, HttpCompletionOption.ResponseHeadersRead))
                    {
                        response.EnsureSuccessStatusCode();

                        long? totalBytes = response.Content.Headers.ContentLength;

                        using (var contentStream = await response.Content.ReadAsStreamAsync())
                        using (var fileStream = new FileStream(tempZipPath, FileMode.Create, FileAccess.Write, FileShare.None))
                        {
                            var buffer = new byte[8192];
                            long downloadedBytes = 0;
                            int bytesRead;

                            while ((bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                            {
                                await fileStream.WriteAsync(buffer, 0, bytesRead);
                                downloadedBytes += bytesRead;

                                if (totalBytes.HasValue)
                                {
                                    
                                }
                            }
                        }
                    }
                }

                // 2. JDK8 압축 해제 (실행 파일 위치에)
                await Task.Run(() => ZipFile.ExtractToDirectory(tempZipPath, extractionPath, true)); // 기존 파일 덮어쓰기
                File.Delete(tempZipPath); // 임시 zip 파일 삭제

                // 3. launcher_profiles.json 파일 수정
                string launcherProfilesPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ".minecraft", "launcher_profiles.json");

                if (File.Exists(launcherProfilesPath))
                {
                    string json = File.ReadAllText(launcherProfilesPath);
                    JObject launcherProfiles = JObject.Parse(json);

                    // "profiles" 객체 가져오기
                    JObject profiles = (JObject)launcherProfiles["profiles"];

                    if (profiles != null)  // profiles 가 null 인 경우 예외처리.
                    {
                        // 모든 프로필 순회
                        foreach (var profile in profiles.Properties())
                        {
                            JObject profileObject = (JObject)profile.Value;

                            // "javaDir" 속성 수정
                            if (profileObject.ContainsKey("javaDir"))
                            {
                                // javaDir 경로 수정.  "java-se-8u41-ri" 폴더가 실행 파일 위치에 있다고 가정.
                                profileObject["javaDir"] = Path.Combine(extractionPath, "java-se-8u41-ri", "bin", "javaw.exe");
                            }
                        }

                        // 수정된 JSON 파일 저장
                        string updatedJson = launcherProfiles.ToString(Newtonsoft.Json.Formatting.Indented); // 보기 좋게 들여쓰기
                        File.WriteAllText(launcherProfilesPath, updatedJson);
                    }

                }
                else
                {
                    MessageBox.Show("launcher_profiles.json 파일을 찾을 수 없습니다.", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                HandyControl.Controls.Growl.Info("JDK8 설치 및 설정이 완료되었습니다.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"JDK8 설치 중 오류 발생: {ex.Message}", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
