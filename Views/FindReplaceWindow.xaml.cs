using System.Windows;

namespace NotepadPlusPlus.Views
{
    public partial class FindReplaceWindow : Window
    {
        public FindReplaceWindow() => InitializeComponent();

        private void Close_Click(object sender, RoutedEventArgs e) => Close();
    }
}