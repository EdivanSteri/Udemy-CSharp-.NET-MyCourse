using MyCourse.Models.InputModels.Courses;
using MyCourse.Models.ViewModels;
using MyCourse.Models.ViewModels.Courses;

namespace MyCourse.Models.Services.Application.Courses
{
    public interface ICachedCourseService : ICourseService
    {
        Task<ListViewModel<CourseViewModel>> GetCoursesAsync(CourseListInputModel model);
        Task<CourseDetailViewModel> GetCourseAsync(int id);
        Task<List<CourseViewModel>> GetMostRecentCoursesAsync();
        Task<List<CourseViewModel>> GetBestRatingCoursesAsync();
        Task<CourseEditInputModel> GetCourseForEditingAsync(int id);
        Task<CourseDetailViewModel> CreateCourseAsync(CourseCreateInputModel inputModel);
        Task<CourseDetailViewModel> EditCourseAsync(CourseEditInputModel inputModel);
        Task DeleteCourseAsync(CourseDeleteInputModel inputModel);
        Task<bool> IsTitleAvailableAsync(string title, int excludeId);
        Task<string> GetCourseAuthorIdAsync(int courseId);
        Task SendQuestionToCourseAuthorAsync(int courseId, string question);
        Task<int> GetCourseCountByAuthorIdAsync(string authorId);
        Task SubscribeCourseAsync(CourseSubscribeInputModel inputModel);
        Task<bool> IsCourseSubscribedAsync(int courseId, string userId);
        Task<string> GetPaymentUrlAsync(int id);
        Task<CourseSubscribeInputModel> CapturePaymentAsync(int id, string token);
        Task<int?> GetCourseVoteAsync(int id);
        Task VoteCourseAsync(CourseVoteInputModel inputModel);
        Task<List<CourseDetailViewModel>> GetCoursesByAuthorAsync(string userId);
        Task<CourseSubscriptionViewModel> GetCourseSubscriptionAsync(int courseId);
    }
}
