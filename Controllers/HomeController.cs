using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MyCourse.Models.Services.Application;
using MyCourse.Models.ViewModels;

namespace MyCourse.Controllers
{
    public class HomeController : Controller
    {

        public async Task<IActionResult> Index([FromServices] ICachedCourseService courseService){
            ViewData["Title"] = "Benvenuto su MyCourse";
            List<CourseViewModel> bestRatingCourses = await courseService.GetBestRatingCoursesAsync();
            List<CourseViewModel> mostRecentCourses = await courseService.GetMostRecentCoursesAsync();

            HomeViewModel viewModel = new HomeViewModel
            {
                BestRatingCourses = bestRatingCourses,
                MostRecentcourses = mostRecentCourses
            };

            return View(viewModel);
        }
    }
}