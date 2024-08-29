using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace SpacedRepetitionApp
{
    public class StatusToBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Subject subject)
            {
                if (subject.IsOverdue())
                    return Brushes.Red;

                if (subject.IsSubjectDueForReview())
                    return Brushes.Orange;
            }

            return Brushes.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class StatusToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Subject subject)
            {
                if (subject.IsOverdue())
                    return "⚠️"; // أو يمكنك استخدام أيقونة أفضل

                if (subject.IsSubjectDueForReview())
                    return "🕒"; // أو يمكنك استخدام أيقونة أفضل
            }

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class StatusToTooltipConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Subject subject)
            {
                if (subject.IsOverdue())
                    return "This subject is overdue for review.";

                if (subject.IsSubjectDueForReview())
                    return "This subject is due for review.";
            }

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
