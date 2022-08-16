using MyCourse.Models.ViewModels;

namespace MyCourse.Models.Services.Application
{
    public interface ICachedCourseService : ICourseService
    {
        Task<List<CourseViewModel>> GetBestRatingCoursesAsync();
        Task<List<CourseViewModel>> GetMostRecentCoursesAsync();
    }
}
