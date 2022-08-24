namespace MyCourse.Models.Exceptions
{
    public class LessonNotFoundException : Exception
    {
        public LessonNotFoundException(int lessonId) : base($"Lesson {lessonId} not found")
        {
        }
    }
}
