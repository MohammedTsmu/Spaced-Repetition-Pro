using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Media;

namespace SpacedRepetitionApp
{
    public partial class MainWindow : Window
    {
        private int editingSubjectId = -1;

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
            subjects.ForEach(subject => subject.ReviewDates = subject.GetReviewDates());
            SubjectsListView.ItemsSource = subjects;
        }

        private async void AddSubjectButton_Click(object sender, RoutedEventArgs e)
        {
            if (editingSubjectId == -1)
            {
                await AddNewSubject();
            }
            else
            {
                await UpdateExistingSubject();
                editingSubjectId = -1;
                AddSubjectButton.Content = "Add Subject";
            }

            LoadSubjects();
            ClearInputFields();
        }

        private async System.Threading.Tasks.Task AddNewSubject()
        {
            var subjects = await Database.GetSubjectsAsync();
            int nextId = subjects.Count > 0 ? subjects.Last().Id + 1 : 1;

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
        }

        private async System.Threading.Tasks.Task UpdateExistingSubject()
        {
            var subject = await Database.GetSubjectByIdAsync(editingSubjectId);
            if (subject != null)
            {
                subject.Name = SubjectNameTextBox.Text;
                subject.Description = DescriptionTextBox.Text;
                subject.Tag = TagTextBox.Text;
                await Database.SaveSubjectAsync(subject);
            }
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

        private void SubjectsListView_MouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (SubjectsListView.SelectedItem is Subject selectedSubject)
            {
                var contextMenu = new ContextMenu();

                var editMenuItem = new MenuItem { Header = "Edit" };
                editMenuItem.Click += (s, args) => EditSubject(selectedSubject);

                var deleteMenuItem = new MenuItem { Header = "Delete" };
                deleteMenuItem.Click += (s, args) => DeleteSubject(selectedSubject);

                contextMenu.Items.Add(editMenuItem);
                contextMenu.Items.Add(deleteMenuItem);

                contextMenu.IsOpen = true;
            }
        }

        private async void EditSubject(Subject subject)
        {
            editingSubjectId = subject.Id;
            SubjectNameTextBox.Text = subject.Name;
            DescriptionTextBox.Text = subject.Description;
            TagTextBox.Text = subject.Tag;
            AddSubjectButton.Content = "Update Subject";
            RemovePlaceholder(SubjectNameTextBox, null);
            RemovePlaceholder(DescriptionTextBox, null);
            RemovePlaceholder(TagTextBox, null);
        }

        private async void DeleteSubject(Subject subject)
        {
            await Database.DeleteSubjectAsync(subject.Id);
            LoadSubjects();
        }

        private void PlayNotificationSoundIfNeeded()
        {
            var subjects = SubjectsListView.ItemsSource as System.Collections.Generic.List<Subject>;
            if (subjects != null && subjects.Any(subject => subject.IsSubjectDueForReview()))
            {
                SystemSounds.Beep.Play();
            }
        }
    }
}
