using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using youtube_center.ViewModels.Components;

namespace youtube_center.Views.Components
{
    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class Home : UserControl
    {
        public Home()
        {
            InitializeComponent();
        }

        private void Home_OnLoaded(object sender, RoutedEventArgs e)
        {
            var vm = (HomeViewModel) DataContext;

            PreviewMouseWheel += (o, args) =>
            {
                if (args.Delta > 0)
                    vm.ChangeVideoPageCommand.Execute("up");

                else if (args.Delta < 0)
                    vm.ChangeVideoPageCommand.Execute("down");
            };
        }
    }
}
