using Galgje.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Galgje
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Properties
        private string HiddenWord { get; set; }
        private int MinWordCharacters { get; set; } = 3;
        private int Lives { get; set; } = 10;
        private int TimerCount { get; set; } = 10;

        private DispatcherTimer Timer { get; set; } = new DispatcherTimer();
        private List<GameCharLabel> GameCharacterLabels { get; } = new List<GameCharLabel>();
        private List<string> Guesses { get; } = new List<string>();
        #endregion

        #region Constructors
        public MainWindow()
        {
            InitializeComponent();
            InputField.Focus();
            Timer.Interval = TimeSpan.FromSeconds(1);
            Timer.Tick += Timer_Tick;
        }
        #endregion

        #region Timer
        private void Timer_Tick(object sender, EventArgs e)
        {
            if(TimerCount <= 0)
            {
                GameWindow.Background = new SolidColorBrush(Color.FromRgb(250, 250, 250));
                ResetAndStartTimer();
                InputField.IsEnabled = true;
                InputField.Clear();
                InputField.Focus();
                return;
            }

            TimerCount--;
            LabelTimeLimit.Content = TimerCount.ToString();

            if(TimerCount <= 0)
            {
                GameWindow.Background = new SolidColorBrush(Color.FromRgb(255, 80, 80));
                InputField.IsEnabled = false;
                Lives--;
                UpdateAfterGuess();
            }
        }
        private void ResetTimer()
        {
            Timer.Stop();
            TimerCount = 10;
            LabelTimeLimit.Content = TimerCount;
            GameWindow.Background = new SolidColorBrush(Color.FromRgb(250, 250, 250));
        }

        private void ResetAndStartTimer()
        {
            ResetTimer();
            Timer.Start();
        }
        #endregion

        #region Handlers
        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            SetActiveBorder(sender as Label);
            StartGame();
            SetDefaultBorder(sender as Label);
        }
        private void BtnReset_Click(object sender, RoutedEventArgs e)
        {
            SetActiveBorder(sender as Label);
            ResetGame();
            SetDefaultBorder(sender as Label);
        }

        private void BtnGuess_Click(object sender, RoutedEventArgs e)
        {
            string guess = InputField.Text.ToLower();
            Guess(guess);
            SetDefaultBorder(sender as Label);
        }

        private void Btn_Hover(object sender, MouseEventArgs e)
        {
            SetActiveBorder(sender as Label);
        }
        private void Btn_ResetHover(object sender, MouseEventArgs e)
        {
            SetDefaultBorder(sender as Label);
        }

        private void SetActiveBorder(Label label)
        {
            label.BorderBrush = Brushes.Black;
        }

        private void SetDefaultBorder(Label label)
        {
            label.BorderBrush = new SolidColorBrush(Color.FromRgb(175,175,175));
        }

        private void InputField_TextChanged(object sender, TextChangedEventArgs e)
        {

            // Bij het aanmaken van het input veld word een TextChanged event getriggered,
            // op dit moment bestaat de BtnStart nog niet omdat deze pas hierna word aangemaakt.
            if (BtnStart == null)
                return;

            HideErrorMessage();

            if (InputField.Text.Length >= MinWordCharacters)
                BtnStart.IsEnabled = true;
            else
                BtnStart.IsEnabled = false;
        }
        private void InputField_KeyDown(object sender, KeyEventArgs e)
        {
            HideErrorMessage();

            switch (e.Key)
            {
                case Key.Enter:
                    EnterHandler();
                    break;
                case Key.Escape:
                    EscapeHandler();
                    break;
            }
        }
        #endregion

        #region KeyHandlers
        private void EnterHandler()
        {
            if (HiddenWord == null)
            {
                if (InputField.Text.Length >= MinWordCharacters)
                {
                    StartGame();
                }
                else
                {
                    ShowErrorMessage($"Woord moet minstens {MinWordCharacters} letters bevatten");
                }
            }
            else
            {
                string guess = InputField.Text.ToLower();
                Guess(guess);
            }
        }

        private void EscapeHandler()
        {
            if(HiddenWord == null)
            {
                Close();
            } else
            {
                ResetGame();
            }
        }
        #endregion

        #region Methods
        private void StartGame()
        {
            BtnStart.Visibility = Visibility.Hidden;
            BtnGuess.Visibility = Visibility.Visible;

            Statistics.Visibility = Visibility.Visible;

            LabelHeader.Text = "Geef een letter of een woord in.";
            LabelBadGuessesCharacters.Content = "";
            LabelLives.Content = Lives;

            BtnReset.IsEnabled = true;

            HiddenWord = InputField.Text.ToLower();

            ClearAndFocusInput();
            CreateGamePanel();
            ResetAndStartTimer();
        }

        private void ResetGame()
        {

            HangmanImage.Visibility = Visibility.Hidden;

            BtnGuess.Visibility = Visibility.Hidden;
            BtnStart.Visibility = Visibility.Visible;

            Statistics.Visibility = Visibility.Hidden;

            LabelHeader.Text = "Geef een geheim woord in.";

            BtnReset.IsEnabled = false;
            BtnStart.IsEnabled = false;

            InputField.IsEnabled = true;

            HiddenWord = null;
            Lives = 10;

            GameCharacterLabels.Clear();
            Guesses.Clear();
            GamePanel.Children.Clear();

            ClearAndFocusInput();
            ResetTimer();
        }

        private void EndGame(bool won)
        {
            ResetTimer();
            BtnGuess.IsEnabled = false;
            InputField.IsEnabled = false;

            if(won)
            {
                LabelHeader.Text = "Hoera! Je hebt het woord geraden!";
            } else
            {
                LabelHeader.Text = "Jammer! Je hebt het woord niet geraden.";
            }

            char[] characters = HiddenWord.ToCharArray();
            foreach (var character in characters)
            {
                if(String.IsNullOrWhiteSpace(character.ToString()) || character.Equals(" ") || Guesses.Contains(character.ToString())) {
                    continue;
                }

                List<GameCharLabel> filledLabels = FillCharacter(character);
                if(filledLabels.Count > 0)
                {
                    foreach (var label in filledLabels)
                    {
                        label.Foreground = won ? Brushes.Black : Brushes.Red;
                    }
                }
            }

        }

        private void Guess(string guess)
        {
            HideErrorMessage();

            if(guess.Length == 0 || String.IsNullOrWhiteSpace(guess.Trim(' ')))
            {
                ClearAndFocusInput();
                ShowErrorMessage("Geef een letter of een woord in!");
                return;
            }

            if (Guesses.Contains(guess))
            {
                ClearAndFocusInput();
                ShowErrorMessage("Je hebt dit al eens geraden!");
                return;
            }

            Guesses.Add(guess);
            ResetAndStartTimer();

            if (guess.Length > 1)
            {
                GuessWord(guess);
            } 
            else
            {
                GuessCharacter(guess);
            }

            ClearAndFocusInput();
            UpdateAfterGuess();
        }

        private void UpdateAfterGuess()
        {
            LabelLives.Content = Lives;
            SetCorrectImage();

            if (Lives <= 0)
            {
                EndGame(false);
            }
        }

        private void GuessWord(string guess)
        {
            if(HiddenWord.Equals(guess))
            {
                EndGame(true);
            } else
            {
                Lives--;
            }
        }

        private void GuessCharacter(string guess)
        {
            char character = guess.ToCharArray().First();
            if(HiddenWord.Contains(guess))
            {
                FillCharacter(character);
                bool guessed = CheckIfWordIsGuessed();
                if(guessed)
                {
                    EndGame(guessed);
                }
            } 
            else
            {
                Lives--;
                LabelBadGuessesCharacters.Content = $"{LabelBadGuessesCharacters.Content} {character}";
            }
        }

        private void SetCorrectImage()
        {
            var step = 10 - Lives;
            HangmanImage.Visibility = Visibility.Visible;
            HangmanImage.Source = new BitmapImage(new Uri($@"/Images/hangman_{step}.png", UriKind.Relative));
        }

        private List<GameCharLabel> FillCharacter(char character)
        {
            char[] wordCharacters = HiddenWord.ToCharArray();
            GameCharLabel[] labels = GameCharacterLabels.ToArray();

            List<GameCharLabel> filledLabels = new List<GameCharLabel>();

            for (int i = 0; i < wordCharacters.Length; i++)
            {
                if (wordCharacters[i].Equals(character))
                {
                    GameCharLabel label = labels[i];

                    if(label.Content == null)
                    {
                        label.Content = character;
                        filledLabels.Add(label);
                    }

                }
            }

            return filledLabels;
        }

        private bool CheckIfWordIsGuessed()
        {
            return GameCharacterLabels.Where(x => x != null && x.Content == null).Count() == 0;
        }

        private void ClearAndFocusInput()
        {
            InputField.Text = String.Empty;
            InputField.Focus();
        }

        private void CreateGamePanel()
        {
            string[] words = HiddenWord.Split(' ');

            for (int i = 0; i < words.Length; i++)
            {
                GameStackPanel panel = new GameStackPanel();
                char[] characters = words[i].ToCharArray();
                for (int j = 0; j < characters.Length; j++)
                {
                    GameCharLabel label = new GameCharLabel();
                    panel.Children.Add(label);
                    GameCharacterLabels.Add(label);
                }

                GameCharacterLabels.Add(null);
                GamePanel.Children.Add(panel);
            }

            GameCharacterLabels.RemoveAt(GameCharacterLabels.Count - 1);

        }

        private void ShowErrorMessage(string error)
        {
            LabelError.Content = error;
            LabelError.Visibility = Visibility.Visible;
        }

        private void HideErrorMessage()
        {
            LabelError.Visibility = Visibility.Hidden;
        }
        #endregion
    }
}