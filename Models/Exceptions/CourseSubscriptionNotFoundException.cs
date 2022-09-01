namespace MyCourse.Models.Exceptions
{
    public class CourseSubscriptionNotFoundException : Exception
    {
        public CourseSubscriptionNotFoundException(int courseId) : base($"Subscription to course {courseId} not found")
        {
        }
    }
}
