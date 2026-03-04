using System.Windows;

namespace NotepadPlusPlus.Views
{
    public partial class AboutWindow : Window
    {
        public AboutWindow() => InitializeComponent();

        private void Close_Click(object sender, RoutedEventArgs e) => Close();
    }
}