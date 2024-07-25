using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace SpacedRepetitionApp
{
    public static class Database
    {
        private static readonly string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "subjects.json");

        public static async Task Initialize()
        {
            if (!File.Exists(filePath))
            {
                using (File.Create(filePath))
                {
                    // Ensure the file is created
                }
                await Task.Run(() => File.WriteAllText(filePath, "[]"));
            }
        }

        public static async Task<List<Subject>> GetSubjectsAsync()
        {
            var json = await Task.Run(() => File.ReadAllText(filePath));
            return JsonSerializer.Deserialize<List<Subject>>(json);
        }

        public static async Task<Subject> GetSubjectByIdAsync(int id)
        {
            var subjects = await GetSubjectsAsync();
            return subjects.FirstOrDefault(s => s.Id == id);
        }

        public static async Task SaveSubjectAsync(Subject subject)
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

        public static async Task DeleteSubjectAsync(int id)
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
    }
}
