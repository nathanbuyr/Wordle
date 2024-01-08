using Microsoft.Maui.Controls;

namespace Wordle
{
    public partial class SettingsPage : ContentPage
    {
        public SettingsPage()
        {
            InitializeComponent();
        }

        private void OnDarkModeSwitchToggled(object sender, ToggledEventArgs e)
        {
            // Handle dark mode setting
            bool isDarkMode = e.Value;

            if (isDarkMode)
            {
                SetDarkModeStyles();
            }
            else
            {
                SetLightModeStyles();
            }
        }

        private void SetDarkModeStyles()
        {
            // Apply dark mode styles
            Application.Current.Resources["PageBackgroundColor"] = Color.FromArgb("#000000");
            Application.Current.Resources["LabelStyle"] = Application.Current.Resources["DarkModeLabelStyle"];
            Application.Current.Resources["PageStyle"] = Application.Current.Resources["DarkModePageStyle"];
        }

        private void SetLightModeStyles()
        {
            // Apply light mode styles
            Application.Current.Resources["PageBackgroundColor"] = Color.FromArgb("#DCDCDC");
            Application.Current.Resources["LabelStyle"] = Application.Current.Resources["LightModeLabelStyle"];
            Application.Current.Resources["PageStyle"] = Application.Current.Resources["LightModePageStyle"];

            // Additional adjustment for text color
            Application.Current.Resources["LabelTextColor"] = Color.FromArgb("#000000");
        }

        private void OnFontSizeSliderValueChanged(object sender, ValueChangedEventArgs e)
        {
            // Handle font size setting
            double fontSize = e.NewValue;
            Application.Current.Resources["GlobalFontSize"] = fontSize;
        }
        private void OnBackToStartClicked(object sender, EventArgs e)
        {
            Navigation.PopAsync(); // Go back to the previous page (StartPage)
        }


    }
}
