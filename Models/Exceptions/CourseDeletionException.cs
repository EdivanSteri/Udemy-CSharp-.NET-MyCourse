namespace MyCourse.Models.Exceptions
{
    public class CourseDeletionException : Exception
    {
        public CourseDeletionException(int Id) : base($"The Course {Id} cannot be deleted because there is at least one subscribed user in it")
        {
        }
    }
}
