namespace Chip8EmulatorGui
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            MainPage = new AppShell();
        }

        protected override Window CreateWindow(IActivationState activationState)
        {
            var windows = base.CreateWindow(activationState);

            windows.Height = 500;
            windows.Width = 600;

            return windows;
        }
    }
}