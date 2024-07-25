using System;
using System.Collections.Generic;

namespace SpacedRepetitionApp
{
    public class Subject
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Tag { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsComplete { get; set; }
        public List<DateTime> ReviewDates { get; set; }

        public List<DateTime> GetReviewDates()
        {
            var reviewDates = new List<DateTime>();
            var intervals = new int[] { 1, 3, 6, 10 };

            foreach (var interval in intervals)
            {
                reviewDates.Add(CreatedDate.AddDays(interval));
            }

            return reviewDates;
        }

        public bool IsSubjectDueForReview()
        {
            var reviewDates = GetReviewDates();
            var today = DateTime.Today;

            foreach (var date in reviewDates)
            {
                if (date.Date == today)
                {
                    return true;
                }
            }

            return false;
        }

        public bool IsSubjectNew()
        {
            return CreatedDate.Date == DateTime.Today;
        }

        public bool IsReviewComplete()
        {
            var lastReviewDate = CreatedDate.AddDays(10);
            return DateTime.Today > lastReviewDate;
        }

        public bool IsOverdue()
        {
            return !IsComplete && IsReviewComplete();
        }
    }
}
