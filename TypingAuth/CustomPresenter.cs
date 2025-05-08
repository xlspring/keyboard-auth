using Avalonia;
using Avalonia.Controls.Primitives;

namespace AvaloniaPopup.Views
{
    public class CustomPresenter : PickerPresenterBase
    {
        public string Text
        {
            get => GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public static readonly StyledProperty<string> TextProperty =
            AvaloniaProperty.Register<CustomPresenter, string>(nameof(Text));
    }
}