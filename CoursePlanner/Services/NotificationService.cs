using Plugin.LocalNotification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoursePlanner.Services
{
    class NotificationService
    {
        public enum AlertType
        {
            CourseStart = 1,
            CourseEnd = 2,
            AssessmentStart = 3,
            AssessmentEnd = 4
        }

        public static async Task InitializeAsync()
        {
            await LocalNotificationCenter.Current.RequestNotificationPermission();
        }

        private static int BuildNotificationId(int itemId, AlertType alertType)
        {
            return (itemId * 1000) + (int)alertType;
        }

        public static async Task ScheduleNotificationAsync(
            int itemId,
            string title,
            string message,
            DateTime notifyTime,
            AlertType alertType)
        {

            if (notifyTime <= DateTime.Now)
                return;

            int notificationId = BuildNotificationId(itemId, alertType);

            var request = new NotificationRequest
            {
                NotificationId = notificationId,
                Title = title,
                Description = message,
                CategoryType = NotificationCategoryType.Reminder,
                Schedule = new NotificationRequestSchedule
                {
                    NotifyTime = notifyTime
                }
            };

            await LocalNotificationCenter.Current.Show(request);
        }

        public static void CancelNotification(int itemId, AlertType alertType)
        {
            int notificationId = BuildNotificationId(itemId, alertType);
            LocalNotificationCenter.Current.Cancel(notificationId);
        }

        public static void CancelAllForCourse(int courseId)
        {
            CancelNotification(courseId, AlertType.CourseStart);
            CancelNotification(courseId, AlertType.CourseEnd);
        }

        public static void CancelAllForAssessment(int assessmentId)
        {
            CancelNotification(assessmentId, AlertType.AssessmentStart);
            CancelNotification(assessmentId, AlertType.AssessmentEnd);
        }
    }
}
