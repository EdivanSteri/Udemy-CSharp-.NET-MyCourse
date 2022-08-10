namespace MyCourse.Models.Exceptions
{
    public class CourseNotFoundException : Exception
    {
        public CourseNotFoundException(int id) : base($"Course {id} not found")
        {
        }
    }
}
