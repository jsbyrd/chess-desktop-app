using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using ChessLogic;

namespace ChessUI
{
    /// <summary>
    /// Interaction logic for StartMenu.xaml
    /// </summary>
    public partial class StartMenu : UserControl
    {
        public event Action<Option> OptionSelected;
        public StartMenu()
        {
            InitializeComponent();
            PlayerButton.Content = Player.White;
        }

        private void PlayerButton_Click(object sender, RoutedEventArgs e)
        {
            Player currentPlayerSelection = (Player) PlayerButton.Content;
            Player newPlayerSelection = currentPlayerSelection.Opponent();

            if (newPlayerSelection == Player.White) OptionSelected?.Invoke(Option.White);
            else if (newPlayerSelection == Player.Black) OptionSelected?.Invoke(Option.Black);

            PlayerButton.Content = newPlayerSelection;

        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            OptionSelected?.Invoke(Option.Start);
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            OptionSelected?.Invoke(Option.Exit);
        }

        private void OpponentSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selectedOpponent = OpponentSelect.SelectedItem.ToString().Split(" ")[1];
            Trace.WriteLine(selectedOpponent);
            if (selectedOpponent == "Freestyle") OptionSelected?.Invoke(Option.Freestyle);
            else if (selectedOpponent == "Random") OptionSelected?.Invoke(Option.Random);
            else if (selectedOpponent == "Engine") OptionSelected?.Invoke(Option.Engine);
        }
    }
}
