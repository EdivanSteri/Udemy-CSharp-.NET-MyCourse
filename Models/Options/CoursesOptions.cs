namespace MyCourse.Models.Options
{
    public class CoursesOptions
    {
        public string PerPage { get; set; }
        public CoursesOrderOptions Order { get; set; }
    }

    public class CoursesOrderOptions
    {
        public string By { get; set; }
        public bool Ascending { get; set; }
        public string[] Allow { get; set; }
    }


}
