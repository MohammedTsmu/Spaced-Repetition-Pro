using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Media;

namespace SpacedRepetitionApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await Database.Initialize();
            LoadSubjects();
            AddPlaceholder(SubjectNameTextBox, null);
            AddPlaceholder(DescriptionTextBox, null);
            AddPlaceholder(TagTextBox, null);
            PlayNotificationSoundIfNeeded();
        }

        private async void LoadSubjects()
        {
            var subjects = await Database.GetSubjectsAsync();
            foreach (var subject in subjects)
            {
                subject.ReviewDates = subject.GetReviewDates();
            }
            SubjectsListView.ItemsSource = subjects;
        }

        private async void AddSubjectButton_Click(object sender, RoutedEventArgs e)
        {
            var subjects = await Database.GetSubjectsAsync();
            int nextId = subjects.Count > 0 ? subjects[subjects.Count - 1].Id + 1 : 1;

            var subject = new Subject
            {
                Id = nextId,
                Name = SubjectNameTextBox.Text,
                Description = DescriptionTextBox.Text,
                Tag = TagTextBox.Text,
                CreatedDate = DateTime.Now,
                IsComplete = false
            };
            await Database.SaveSubjectAsync(subject);
            LoadSubjects();
            ClearInputFields();
        }

        private void ClearInputFields()
        {
            SubjectNameTextBox.Text = "";
            DescriptionTextBox.Text = "";
            TagTextBox.Text = "";
            AddPlaceholder(SubjectNameTextBox, null);
            AddPlaceholder(DescriptionTextBox, null);
            AddPlaceholder(TagTextBox, null);
        }

        private void RemovePlaceholder(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (textBox.Text == (string)textBox.Tag)
            {
                textBox.Text = "";
                textBox.Foreground = System.Windows.Media.Brushes.Black;
            }
        }

        private void AddPlaceholder(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Text = (string)textBox.Tag;
                textBox.Foreground = System.Windows.Media.Brushes.Gray;
            }
        }

        private void PlayNotificationSoundIfNeeded()
        {
            var subjects = SubjectsListView.Items.Cast<Subject>();
            if (subjects.Any(subject => subject.IsOverdue() || subject.IsSubjectDueForReview()))
            {
                SystemSounds.Exclamation.Play();
            }
        }
    }
}
