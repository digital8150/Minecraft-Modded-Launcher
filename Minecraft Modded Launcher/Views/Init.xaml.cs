﻿using Minecraft_Modded_Launcher.Controllers;
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
    /// Init.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Init : Window
    {
        private AnimationController animationController = new AnimationController();
        public MainApplication application = new MainApplication();

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

            animationController.BeginAnimation(mainborder, 1, 0, OpacityProperty, 0.4, EasingMode.EaseOut, new SineEase());
            await Task.Delay(400);
            this.Close();
            application.Show();
        }
    }
}
