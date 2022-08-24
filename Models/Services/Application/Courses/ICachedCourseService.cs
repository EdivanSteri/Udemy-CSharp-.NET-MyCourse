using MyCourse.Models.InputModels.Courses;
using MyCourse.Models.ViewModels.Courses;

namespace MyCourse.Models.Services.Application.Courses
{
    public interface ICachedCourseService : ICourseService
    {
        Task<CourseDetailViewModel> CreateCourseAsync(CourseCreateInputModel inputModel);
        Task<CourseDetailViewModel> EditCourseAsync(CourseEditInputModel inputModel);
        Task<List<CourseViewModel>> GetBestRatingCoursesAsync();
        Task<CourseEditInputModel> GetCourseForEditingAsync(int id);
        Task<List<CourseViewModel>> GetMostRecentCoursesAsync();
        Task<bool> IsTitleAvailableAsync(string title, int id);
    }
}
