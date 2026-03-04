using System.Windows;
using System.Windows.Controls;

namespace NotepadPlusPlus.Behaviors
{
    public static class TextBoxBehavior
    {
        public static readonly DependencyProperty SelectionStartProperty =
            DependencyProperty.RegisterAttached("SelectionStart", typeof(int), typeof(TextBoxBehavior),
                new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    OnSelectionStartChanged));

        public static readonly DependencyProperty SelectionLengthProperty =
            DependencyProperty.RegisterAttached("SelectionLength", typeof(int), typeof(TextBoxBehavior),
                new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    OnSelectionLengthChanged));

        public static readonly DependencyProperty CaretIndexProperty =
            DependencyProperty.RegisterAttached("CaretIndex", typeof(int), typeof(TextBoxBehavior),
                new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    OnCaretIndexChanged));

        public static readonly DependencyProperty IsAttachedProperty =
            DependencyProperty.RegisterAttached("IsAttached", typeof(bool), typeof(TextBoxBehavior),
                new PropertyMetadata(false, OnIsAttachedChanged));

        private static readonly DependencyProperty IsUpdatingProperty =
            DependencyProperty.RegisterAttached("IsUpdating", typeof(bool), typeof(TextBoxBehavior),
                new PropertyMetadata(false));

        public static int GetSelectionStart(DependencyObject o) => (int)o.GetValue(SelectionStartProperty);
        public static void SetSelectionStart(DependencyObject o, int v) => o.SetValue(SelectionStartProperty, v);
        public static int GetSelectionLength(DependencyObject o) => (int)o.GetValue(SelectionLengthProperty);
        public static void SetSelectionLength(DependencyObject o, int v) => o.SetValue(SelectionLengthProperty, v);
        public static int GetCaretIndex(DependencyObject o) => (int)o.GetValue(CaretIndexProperty);
        public static void SetCaretIndex(DependencyObject o, int v) => o.SetValue(CaretIndexProperty, v);
        public static bool GetIsAttached(DependencyObject o) => (bool)o.GetValue(IsAttachedProperty);
        public static void SetIsAttached(DependencyObject o, bool v) => o.SetValue(IsAttachedProperty, v);

        private static void OnIsAttachedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TextBox tb && (bool)e.NewValue)
            {
                tb.SelectionChanged -= OnSelectionChanged;
                tb.SelectionChanged += OnSelectionChanged;
            }
        }

        private static void OnSelectionStartChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not TextBox tb || (bool)tb.GetValue(IsUpdatingProperty)) return;
            tb.SetValue(IsUpdatingProperty, true);
            tb.SelectionStart = (int)e.NewValue;
            tb.Focus();
            tb.SetValue(IsUpdatingProperty, false);
        }

        private static void OnSelectionLengthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not TextBox tb || (bool)tb.GetValue(IsUpdatingProperty)) return;
            tb.SetValue(IsUpdatingProperty, true);
            tb.SelectionLength = (int)e.NewValue;
            tb.Focus();
            tb.SetValue(IsUpdatingProperty, false);
        }

        private static void OnCaretIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not TextBox tb || (bool)tb.GetValue(IsUpdatingProperty)) return;
            tb.SetValue(IsUpdatingProperty, true);
            tb.CaretIndex = (int)e.NewValue;
            var line = tb.GetLineIndexFromCharacterIndex(tb.CaretIndex);
            if (line >= 0) tb.ScrollToLine(line);
            tb.Focus();
            tb.SetValue(IsUpdatingProperty, false);
        }

        private static void OnSelectionChanged(object sender, RoutedEventArgs e)
        {
            if (sender is not TextBox tb || (bool)tb.GetValue(IsUpdatingProperty)) return;
            tb.SetValue(IsUpdatingProperty, true);
            SetSelectionStart(tb, tb.SelectionStart);
            SetSelectionLength(tb, tb.SelectionLength);
            SetCaretIndex(tb, tb.CaretIndex);
            tb.SetValue(IsUpdatingProperty, false);
        }
    }
}