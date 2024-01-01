namespace Wordle
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new StartPage()); // Set StartPage as MainPage
        }
    }
}