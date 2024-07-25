using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;

namespace SpacedRepetitionApp
{
    public static class Database
    {
        private static readonly string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SpacedRepetitionApp", "subjects.json");

        public static async Task Initialize()
        {
            try
            {
                string directoryPath = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                if (!File.Exists(filePath))
                {
                    using (File.Create(filePath))
                    {
                        // Ensure the file is created
                    }
                    await Task.Run(() => File.WriteAllText(filePath, "[]"));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while initializing the database: {ex.Message}");
            }
        }

        public static async Task<List<Subject>> GetSubjectsAsync()
        {
            try
            {
                var json = await Task.Run(() => File.ReadAllText(filePath));
                return JsonSerializer.Deserialize<List<Subject>>(json);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while reading the database: {ex.Message}");
                return new List<Subject>();
            }
        }

        public static async Task<Subject> GetSubjectByIdAsync(int id)
        {
            var subjects = await GetSubjectsAsync();
            return subjects.FirstOrDefault(s => s.Id == id);
        }

        public static async Task SaveSubjectAsync(Subject subject)
        {
            try
            {
                var subjects = await GetSubjectsAsync();
                var existingSubject = subjects.FirstOrDefault(s => s.Id == subject.Id);

                if (existingSubject != null)
                {
                    subjects.Remove(existingSubject);
                }

                subjects.Add(subject);
                var json = JsonSerializer.Serialize(subjects);
                await Task.Run(() => File.WriteAllText(filePath, json));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while saving the subject: {ex.Message}");
            }
        }

        public static async Task DeleteSubjectAsync(int id)
        {
            try
            {
                var subjects = await GetSubjectsAsync();
                var subjectToRemove = subjects.FirstOrDefault(s => s.Id == id);
                if (subjectToRemove != null)
                {
                    subjects.Remove(subjectToRemove);
                    var json = JsonSerializer.Serialize(subjects);
                    await Task.Run(() => File.WriteAllText(filePath, json));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while deleting the subject: {ex.Message}");
            }
        }
    }
}
