using Microsoft.Maui.Controls;

namespace Wordle
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private void OnGenerateGridClicked(object sender, EventArgs e)
        {
            // Clear existing grid content
            WordleGrid.Children.Clear();

            int rows = 6;
            int columns = 5;

            // Create rows and columns
            for (int i = 0; i < rows; i++)
            {
                WordleGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(80) });
            }

            for (int j = 0; j < columns; j++)
            {
                WordleGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(80) });
            }

            // Add frames reminiscent of Wordle
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    Frame frame = new Frame
                    {
                        BackgroundColor = Color.FromArgb("#D3D3D3"),
                        Padding = 0,
                        HasShadow = false
                    };

                    Grid.SetRow(frame, i);
                    Grid.SetColumn(frame, j);

                    WordleGrid.Children.Add(frame);
                }
            }
        }
    }
}
