using Cybersecurity_chatbot;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace CybersecurityChatbot
{
    public partial class MainWindow : Window
    {
        private Chatbot bot;
        private Audio audio = new Audio();
        private List<string> activityLog = new List<string>();
        private int quizScore = 0;
        private int quizIndex = 0;

        private List<(string Question, string[] Options, string Correct)> quizQuestions = new List<(string, string[], string)>
        {
            ("Which is a strong password?", new[] {"123456", "password", "Pa$$w0rd!", "admin"}, "Pa$$w0rd!"),
            ("What is phishing?", new[] {"A sport", "Cyberattack", "Encryption", "Antivirus"}, "Cyberattack"),
            ("Two-factor authentication is:", new[] {"Insecure", "Optional", "Stronger", "Outdated"}, "Stronger"),
            ("True or False: You should share your passwords.", new[] {"True", "False"}, "False"),
            ("Which is a secure network?", new[] {"Public WiFi", "Home WPA2", "Open hotspot", "None"}, "Home WPA2")
        };

        public MainWindow()
        {
            InitializeComponent();
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            string name = NameInput.Text.Trim();

            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Please enter your name to continue.");
                return;
            }

            string topic = Interaction.InputBox("What’s your favorite cybersecurity topic?", "Cybersecurity Chatbot");

            bot = new Chatbot(name, topic);
            chatBox.Items.Add("[BOT] Welcome, " + name + "! Ask about cybersecurity.");

            StartPage.Visibility = Visibility.Collapsed;
            MainPage.Visibility = Visibility.Visible;

            audio.PlayWav();
            new Logo(); // Console-based ASCII logo
        }

        private void ProcessInput_Click(object sender, RoutedEventArgs e)
        {
            string input = userInput.Text;
            if (string.IsNullOrWhiteSpace(input)) return;

            chatBox.Items.Add("[YOU] " + input);
            string response = bot.RespondToInput(input);
            chatBox.Items.Add("[BOT] " + response);
            activityLog.Add($"[{DateTime.Now}] User: '{input}' => {response}");
            userInput.Clear();
        }

        private void ShowNextQuestion()
        {
            var question = quizQuestions[quizIndex];
            quizPanel.Children.Clear();
            quizPanel.Children.Add(new TextBlock { Text = question.Question, FontWeight = FontWeights.Bold, Margin = new Thickness(5) });

            foreach (var option in question.Options)
            {
                var btn = new Button { Content = option, Margin = new Thickness(2), Padding = new Thickness(5) };
                btn.Click += QuizOption_Click;
                quizPanel.Children.Add(btn);
            }
        }

        private void QuizOption_Click(object sender, RoutedEventArgs e)
        {
            var clicked = sender as Button;
            string answer = clicked.Content.ToString();
            string correct = quizQuestions[quizIndex].Correct;

            if (answer == correct)
            {
                quizScore++;
                MessageBox.Show("Correct!");
            }
            else
            {
                MessageBox.Show("Incorrect. Correct answer: " + correct);
            }

            quizIndex++;
            if (quizIndex < quizQuestions.Count)
                ShowNextQuestion();
            else
            {
                MessageBox.Show($"Final score: {quizScore}/{quizQuestions.Count}");
                activityLog.Add($"[{DateTime.Now}] Quiz complete: {quizScore}/{quizQuestions.Count}");
                quizPanel.Children.Clear();
            }
        }

        private void StartQuiz_Click(object sender, RoutedEventArgs e)
        {
            quizScore = 0;
            quizIndex = 0;
            ShowNextQuestion();
            activityLog.Add($"[{DateTime.Now}] Quiz started");
        }

        private void ShowLog_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(string.Join("\n", activityLog), "Activity Log");
        }

        private void AddReminder_Click(object sender, RoutedEventArgs e)
        {
            string msg = Interaction.InputBox("Reminder text:", "Add Reminder");
            string y = Interaction.InputBox("Year:", "Reminder Date");
            string m = Interaction.InputBox("Month:", "Reminder Date");
            string d = Interaction.InputBox("Day:", "Reminder Date");

            if (int.TryParse(y, out int year) && int.TryParse(m, out int month) && int.TryParse(d, out int day))
            {
                try
                {
                    DateTime date = new DateTime(year, month, day);
                    string reminder = $"{msg} on {date:yyyy-MM-dd}";
                    chatBox.Items.Add("[BOT] Reminder saved: " + reminder);
                    activityLog.Add($"[{DateTime.Now}] Reminder: {reminder}");
                }
                catch
                {
                    MessageBox.Show("Invalid date entered.");
                }
            }
            else
            {
                MessageBox.Show("Please enter valid numeric values for year, month, and day.");
            }
        }

        private void CreateTask_Click(object sender, RoutedEventArgs e)
        {
            string task = Interaction.InputBox("Task description:", "Create Task");
            if (!string.IsNullOrWhiteSpace(task))
            {
                chatBox.Items.Add("[BOT] Task created: " + task);
                activityLog.Add($"[{DateTime.Now}] Task: {task}");
            }
        }
    }
}
