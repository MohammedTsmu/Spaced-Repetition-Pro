using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace SpacedRepetitionApp
{
    public class Database
    {
        private static readonly string databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "SpacedRepetitionApp", "data.json");

        public static async Task Initialize()
        {
            if (!File.Exists(databasePath))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(databasePath));
                var initialData = new List<Subject>();
                await Task.Run(() => File.WriteAllText(databasePath, JsonConvert.SerializeObject(initialData)));
            }
        }

        public static async Task<List<Subject>> GetSubjectsAsync()
        {
            return await Task.Run(() =>
            {
                var json = File.ReadAllText(databasePath);
                return JsonConvert.DeserializeObject<List<Subject>>(json);
            });
        }

        public static async Task<Subject> GetSubjectAsync(int id)
        {
            var subjects = await GetSubjectsAsync();
            return subjects.Find(s => s.Id == id);
        }

        public static async Task SaveSubjectAsync(Subject subject)
        {
            var subjects = await GetSubjectsAsync();
            var existingSubject = subjects.Find(s => s.Id == subject.Id);

            if (existingSubject != null)
            {
                subjects.Remove(existingSubject);
            }

            subjects.Add(subject);
            await Task.Run(() => File.WriteAllText(databasePath, JsonConvert.SerializeObject(subjects)));
        }

        public static async Task DeleteSubjectAsync(int id)
        {
            var subjects = await GetSubjectsAsync();
            var subjectToRemove = subjects.Find(s => s.Id == id);

            if (subjectToRemove != null)
            {
                subjects.Remove(subjectToRemove);
                await Task.Run(() => File.WriteAllText(databasePath, JsonConvert.SerializeObject(subjects)));
            }
        }
    }
}
