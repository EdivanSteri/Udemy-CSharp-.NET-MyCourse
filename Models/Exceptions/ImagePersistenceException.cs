namespace MyCourse.Models.Exceptions
{
    public class ImagePersistenceException : Exception
    {
        public ImagePersistenceException(Exception innerException) : base("Couldn't persist the image", innerException)
        {
        }
    }
}
