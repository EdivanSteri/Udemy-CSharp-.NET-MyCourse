using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using MyCourse.Models.Exceptions;
using MyCourse.Models.InputModels.Courses;
using MyCourse.Models.Services.Application.Courses;
using MyCourse.Models.ViewModels;
using MyCourse.Models.ViewModels.Courses;

namespace MyCourse.Controllers
{
    public class CoursesController : Controller{
        private readonly ICachedCourseService courseService;
        public CoursesController(ICachedCourseService courseServices){
            this.courseService = courseServices;   
        }
        public async Task<IActionResult> Index(CourseListInputModel input)
        {
            ViewData["Title"] = "Catalogo Corsi";
            ListViewModel<CourseViewModel> courses = await courseService.GetCoursesAsync(input);

            CourseListViewModel viewModel = new CourseListViewModel
            {
                Courses = courses,
                Input = input
            };

            return View(viewModel);
        }

        public async Task<IActionResult> Detail(int id){
            CourseDetailViewModel viewModel = await courseService.GetCourseAsync(id);
            ViewData["Title"] = viewModel.Title;
            return View(viewModel);
        }

        public IActionResult Create()
        {
            ViewData["Title"] = "Nuovo Corso";
            var inputModel = new CourseCreateInputModel();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CourseCreateInputModel inputModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    CourseDetailViewModel course = await courseService.CreateCourseAsync(inputModel);
                    return RedirectToAction(nameof(Index));
                }
                catch (CourseTitleUnavailableException)
                {
                    ModelState.AddModelError(nameof(CourseDetailViewModel.Title), "Questo titolo già esiste");
                }
            }
           
            ViewData["Title"] = "Nuovo Corso";
            return View(inputModel);
        }

        public async Task<IActionResult> IsTitleAvailable(string title, int id=0)
        {
            bool result = await courseService.IsTitleAvailableAsync(title, id);
            return Json(result);
        }

        public async Task<IActionResult> Edit(int id)
        {
            ViewData["Title"] = "Modifica Corso";
            CourseEditInputModel inputNodel = await courseService.GetCourseForEditingAsync(id);
            return View(inputNodel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(CourseEditInputModel inputModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    CourseDetailViewModel course = await courseService.EditCourseAsync(inputModel);
                    TempData["ConfirmationMessage"] = "I dati sono stati salvati con successo";
                    return RedirectToAction(nameof(Detail), new {id= inputModel.Id});
                }
                catch (CourseImageInvalidException)
                {
                    ModelState.AddModelError(nameof(CourseEditInputModel.Image), "L'immagine selezionata non è valida");
                }
                catch (CourseTitleUnavailableException)
                {
                    ModelState.AddModelError(nameof(CourseEditInputModel.Title), "Questo titolo già esiste");
                }
                catch (OptimisticConcurrencyException)
                {
                    ModelState.AddModelError("", "Spiacenti, il salvataggio non è andato a buon fine perché nel frattempo un altro utente ha aggiornato il corso. Ti preghiamo di aggiornare la pagina e ripetere le modifiche.");
                }
            }

            ViewData["Title"] = "Modifica Corso";
            return View(inputModel);
        }
    }  

}