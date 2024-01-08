using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;

namespace Wordle
{

    public partial class MainPage : ContentPage
    {
        private const string WordsFilePath = "words.txt";
        private List<string> wordList;
        private string selectedWord;
        private int currentRowIndex = 0;
        private List<Frame> currentRowFrames = new List<Frame>();
        private bool isAnimationPlaying = false;

        private string playerName;

    public MainPage(string playerName)
    {
        InitializeComponent();
        this.playerName = playerName;
        LoadWordsFile();
    }

        private async Task DownloadWordsFile()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    // Download the file content
                    string fileContent = await client.GetStringAsync("https://raw.githubusercontent.com/DonH-ITS/jsonfiles/main/words.txt");

                    // Save the file content to the local file
                    string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), WordsFilePath);
                    File.WriteAllText(filePath, fileContent);

                    // Update wordList and selectedWord
                    wordList = fileContent.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    selectedWord = GetRandomWord();
                }
            }
            catch (Exception)
            {
                selectedWord = "Error downloading words"; // Set a default value to avoid errors
            }
        }

        private async void LoadWordsFile()
        {
            // Check if the file exists locally
            string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), WordsFilePath);
            if (File.Exists(filePath))
            {
                // Read words from the local file
                string fileContent = File.ReadAllText(filePath);
                wordList = fileContent.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).ToList(); // Reads every word and splits them into an array of strings, then turns array into a list
                selectedWord = GetRandomWord();
            }
            else
            {
                // File doesn't exist locally, download it
                await DownloadWordsFile();
            }
        }

        private string GetRandomWord()
        {
            if (wordList != null && wordList.Count > 0)
            {
                Random random = new Random();
                int randomIndex = random.Next(0, wordList.Count);
                return wordList[randomIndex].Trim().ToUpper(); // Convert to uppercase for case-insensitive comparison
            }
            return "No words available";
        }


        // Minimum frame size
        private const double MIN_FRAME_SIZE = 70;
        public void OnGenerateGridClicked(object sender, EventArgs e)
        {
            // Check word for easier debugging
            //DisplayAlert("Random Word", $"The randomly selected word is: {selectedWord}", "OK");

            // Clear existing grid content
            WordleGrid.Children.Clear();
            currentRowIndex = 0;
            currentRowFrames.Clear();

            int rows = 6;
            int columns = 5;

            // Row and column creation
            for (int i = 0; i < rows; i++)
            {
                WordleGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            }

            for (int j = 0; j < columns; j++)
            {
                WordleGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            }

            // Calculating the size of each frame based on the available space
            double frameSize = Math.Max(Math.Min(WordleGrid.Width / columns, WordleGrid.Height / rows), MIN_FRAME_SIZE);

            // Adding frames
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    Frame frame = new Frame
                    {
                        BackgroundColor = Color.FromArgb("#5A5A5A"),
                        Padding = 0,
                        HasShadow = false,
                        WidthRequest = frameSize,
                        HeightRequest = frameSize
                    };

                    Grid.SetRow(frame, i);
                    Grid.SetColumn(frame, j);

                    WordleGrid.Children.Add(frame);
                }
            }
        }
        private void OnKeyboardButtonClicked(object sender, EventArgs e)
        {
            if (currentRowIndex >= WordleGrid.RowDefinitions.Count)
            {
                // All rows are filled, do nothing
                return;
            }

            // Handle keyboard button click here
            if (sender is Button button)
            {
                // Access the Text property of the clicked button
                string buttonText = button.Text;

                // Find the frame in the current row
                Frame frame = currentRowFrames.Count > 0 ? currentRowFrames[currentRowFrames.Count - 1] : null;

                if (frame == null || frame.Content != null)
                {
                    // Create a new frame if the current one is filled or doesn't exist
                    frame = new Frame
                    {
                        BackgroundColor = Color.FromArgb("#5A5A5A"),
                        Padding = 0,
                        HasShadow = false
                    };

                    Grid.SetRow(frame, currentRowIndex);
                    Grid.SetColumn(frame, currentRowFrames.Count);

                    WordleGrid.Children.Add(frame);
                    currentRowFrames.Add(frame);
                }

                // Set the content of the frame to the clicked button text
                Label label = new Label
                {
                    Text = buttonText,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center
                };

                if (frame.Content == null || currentRowFrames.Count < 5)
                {
                    // Add the label to the frame if the frame is empty or there are less than 5 letters in the current row
                    frame.Content = label;
                }
                else
                {
                    // Delete the 5th letter and add the new letter
                    currentRowFrames[4].Content = null;
                    currentRowFrames.Insert(4, frame);
                }
            }
        }
        private async void OnEnterButtonClicked(object sender, EventArgs e)
        {
            if (isAnimationPlaying)
            {
                // Animation is already playing, ignore the button press
                return;
            }

            // Check if exactly 5 letters are entered
            if (currentRowFrames.Count != 5)
            {
                await DisplayAlert("Invalid Input", "Please enter exactly 5 letters.", "OK");
                return;
            }

            // For each frame, checks if the content is a label and retrives it, then converts it to an array, then concatenates it into a string
            string enteredWord = new string(currentRowFrames.Select(frame => (frame.Content as Label)?.Text.FirstOrDefault() ?? ' ').ToArray());

            // Check if the entered word is in the word list
            if (!wordList.Contains(enteredWord.ToLower())) // Convert to lowercase for case-insensitive comparison
            {
                await DisplayAlert("Invalid Word", "Please enter a valid word from the word list.", "OK");
                return;
            }

            // Set the animation flag to true
            isAnimationPlaying = true;

            // Disable the keyboard during the animation
            DisableKeyboard();

            // Flipping animation
            foreach (Frame frame in currentRowFrames)
            {
                await frame.RotateYTo(90, 250);
                frame.BackgroundColor = GetFrameColor(frame);
                await frame.RotateYTo(0, 250);
            }

            // Increment the current row index
            currentRowIndex++;

            // Call CheckWord method
            CheckWord();

            // Ends game if word isn't guessed in 6 tries
            if (currentRowIndex >= WordleGrid.RowDefinitions.Count)
            {
                await DisplayAlert("Too bad!", $"The randomly selected word is: {selectedWord}\nGo back to the start menu to try again!", "OK");
                DisableKeyboard();
                return;
            }

            // Clear the current row frames list
            currentRowFrames.Clear();

            // Set the animation flag back to false
            isAnimationPlaying = false;
        }

        private void DisableKeyboard()
        {
            // Disable the keyboard buttons
            foreach (var child in KeyboardGrid.Children)
            {
                if (child is Button button)
                {
                    button.IsEnabled = false;
                }
            }
        }
        private void EnableKeyboard()
        {
            // Enable the keyboard buttons
            foreach (var child in KeyboardGrid.Children)
            {
                if (child is Button button)
                {
                    button.IsEnabled = true;
                }
            }
        }

        private Color GetFrameColor(Frame frame)
        {
            Label label = frame.Content as Label;
            if (label != null)
            {
                char inputChar = label.Text.FirstOrDefault();

                if (selectedWord.Contains(inputChar))
                {
                    if (selectedWord[currentRowFrames.IndexOf(frame)] == inputChar)
                    {
                        // Letter is in the correct position
                        return Color.FromArgb("#9ACD32");
                    }
                    else
                    {
                        // Letter is in the word but not in the correct position
                        return Color.FromArgb("#CCCC00");
                    }
                }
            }
            // Letter is not in the random word
            return Color.FromArgb("#5A5A5A");
        }
        private void OnBackspaceButtonClicked(object sender, EventArgs e)
        {
            // Handle backspace button click
            if (currentRowIndex >= 0)
            {
                // Find the last frame in the current row
                int currentRow = currentRowIndex;

                for (int i = currentRowFrames.Count - 1; i >= 0; i--)
                {
                    Frame frame = currentRowFrames[i];

                    if (frame.Content is Label label && !string.IsNullOrEmpty(label.Text))
                    {
                        // Remove the last letter in the frame
                        label.Text = label.Text.Substring(0, label.Text.Length - 1);

                        // If the label is empty, clear the entire content
                        if (string.IsNullOrEmpty(label.Text))
                        {
                            frame.Content = null;
                        }

                        // Break once a letter is removed
                        break;
                    }
                    else if (frame.Content == null)
                    {
                        // If the frame is empty, remove it from the currentRowFrames list
                        currentRowFrames.RemoveAt(i);
                    }
                }
            }
        }

        private void CheckWord()
        {
            string inputWord = string.Join("", currentRowFrames.Select(frame => frame.Content is Label label ? label.Text : ""));

                // Check if the input word matches the selected word
                if (string.Equals(inputWord, selectedWord, StringComparison.OrdinalIgnoreCase))
                {
                    // Correct answer: display alert and disable the keyboard
                    DisplayAlert("Congratulations!", "You guessed the word!", "OK");
                    DisableKeyboard();
                    // Save player's name and selected word to Preferences
                    SaveToPreferences(playerName, selectedWord, currentRowIndex);
                }
                else
                {
                    EnableKeyboard();
                }
        }

        private async void OnSettingsButtonClicked(object sender, EventArgs e)
        {
            // Navigate to the SettingsPage
            await Navigation.PushAsync(new SettingsPage());
        }

        private void OnBackToStartClicked(object sender, EventArgs e)
        {
            Navigation.PopAsync(); // Go back to the previous page (StartPage)
        }
        private void OnProgressionClicked(object sender, EventArgs e)
        {
            // Navigate to the ProgressionPage
            Navigation.PushAsync(new ProgressionPage());
        }

        private void SaveToPreferences(string playerName, string selectedWord, int currentRowIndex)
        {
            // Retrieve existing data from Preferences
            string existingData = Preferences.Get("ProgressionData", "");

            // Append new data
            string newData = $"{DateTime.Now}: Player Name: {playerName}, Selected Word: {selectedWord}, Amount of Tries: {currentRowIndex}\n";
            string allData = existingData + newData;

            // Save the updated data to Preferences
            Preferences.Set("ProgressionData", allData);
        }




    }
}
