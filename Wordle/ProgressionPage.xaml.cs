using Microsoft.Maui.Controls;

namespace Wordle
{
    public partial class ProgressionPage : ContentPage
    {
        public ProgressionPage()
        {
            InitializeComponent();

            // Display saved progression data
            string allData = Preferences.Get("ProgressionData", "");
            LabelProgressionData.Text = allData;
        }

        private void OnBackToStartClicked(object sender, EventArgs e)
        {
            Navigation.PopAsync(); // Go back to the previous page (StartPage)
        }
    }
}
