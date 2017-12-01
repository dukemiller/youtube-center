using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace youtube_center.Classes.Xaml
{

    // https://stackoverflow.com/questions/23316274/inputbindings-work-only-when-focused
    public class InputBindingBehavior
    {
        public static bool GetPropagateInputBindingsToWindow(FrameworkElement obj)
        {
            return (bool)obj.GetValue(PropagateInputBindingsToWindowProperty);
        }

        public static void SetPropagateInputBindingsToWindow(FrameworkElement obj, bool value)
        {
            obj.SetValue(PropagateInputBindingsToWindowProperty, value);
        }

        public static readonly DependencyProperty PropagateInputBindingsToWindowProperty =
            DependencyProperty.RegisterAttached("PropagateInputBindingsToWindow", typeof(bool), typeof(InputBindingBehavior),
                new PropertyMetadata(false, OnPropagateInputBindingsToWindowChanged));

        private static void OnPropagateInputBindingsToWindowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((FrameworkElement)d).Loaded += frameworkElement_Loaded;
        }

        private static void frameworkElement_Loaded(object sender, RoutedEventArgs e)
        {
            var frameworkElement = (FrameworkElement)sender;
            frameworkElement.Loaded -= frameworkElement_Loaded;

            var window = Window.GetWindow(frameworkElement);
            if (window == null)
                return;

            foreach (var input in frameworkElement.InputBindings.Cast<InputBinding>().Reverse())
            {
                switch (input)
                {
                    // case MouseBinding _:
                    case KeyBinding _:
                        window.InputBindings.Add(input);
                        frameworkElement.InputBindings.Remove(input);
                        break;
                }
            }
            
        }
    }
}