using MyCourse.Models.InputModels;
using MyCourse.Models.ViewModels;

namespace MyCourse.Models.Services.Application
{
    public interface ICachedCourseService : ICourseService
    {
        Task<CourseDetailViewModel> CreateCourseAsync(CourseCreateInputModel inputModel);
        Task<List<CourseViewModel>> GetBestRatingCoursesAsync();
        Task<List<CourseViewModel>> GetMostRecentCoursesAsync();
        Task<bool> IsTitleAvailableAsync(string title);
    }
}
