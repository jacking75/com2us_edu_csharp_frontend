using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace MultiPage
{
    public partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            // Enter construction logic here...
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        { 
            Uri uri = new Uri("/Page1.xaml", UriKind.Relative);
            PageContainer.Source = uri;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Uri uri = new Uri("/Page2.xaml", UriKind.Relative);
            PageContainer.Source = uri;
        }
    }
}
