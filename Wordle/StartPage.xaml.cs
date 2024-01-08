using Microsoft.Maui.Controls;

namespace Wordle
{
    public partial class StartPage : ContentPage
    {
        private string playerName = "Player"; // Default player name

        public StartPage()
        {
            InitializeComponent();
        }

        private async void OnStartGameClicked(object sender, EventArgs e)
        {
            // Display an alert to get the user's name
            string enteredName = await DisplayPromptAsync("Enter Your Name", "What's your name?", "OK", "Cancel", "Player");

            if (!string.IsNullOrWhiteSpace(enteredName))
            {
                playerName = enteredName;
                UpdateWelcomeMessage();
               // Pass the player's name to the MainPage
                MainPage mainPage = new MainPage(playerName);
                await Navigation.PushAsync(mainPage);
                mainPage.OnGenerateGridClicked(sender, e); // Trigger grid generation
            }
            else
            {
                await DisplayAlert("Error", "Please enter a valid name.", "OK");
            }
        }


        private void UpdateWelcomeMessage()
        {
            // Prints welcome message
            WelcomeLabel.Text = $"Welcome, {playerName}, to Wordle!";
        }
    }
}
