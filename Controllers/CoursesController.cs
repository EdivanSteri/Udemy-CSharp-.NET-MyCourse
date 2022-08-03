using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MyCourse.Models.Services.Application;
using MyCourse.Models.ViewModels;

namespace MyCourse.Controllers
{
    public class CoursesController : Controller{
        private readonly ICourseService courseService;
        public CoursesController(ICourseService courseServices){
            this.courseService = courseServices;   
        }
        public async Task<IActionResult> Index(){
            List<CourseViewModel> courses = await courseService.GetCoursesAsync();
            ViewData["Title"] = "Catalogo Corsi";
            return View(courses);
        }

        public async Task<IActionResult> Detail(int id){
            CourseDetailViewModel viewModel = await courseService.GetCourseAsync(id);
            ViewData["Title"] = viewModel.Title;
            return View(viewModel);
        }
    

    }  

}