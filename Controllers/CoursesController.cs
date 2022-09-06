using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MyCourse.Models.Enums;
using MyCourse.Models.Exceptions;
using MyCourse.Models.InputModels.Courses;
using MyCourse.Models.Options;
using MyCourse.Models.Services.Application.Courses;
using MyCourse.Models.Services.Infrastructure;
using MyCourse.Models.ViewModels;
using MyCourse.Models.ViewModels.Courses;
using Rotativa.AspNetCore.Options;
using Rotativa.AspNetCore;

namespace MyCourse.Controllers
{

    public class CoursesController : Controller{
            
        private readonly ICourseService courseService;
        public CoursesController(ICachedCourseService courseService)
        {
            this.courseService = courseService;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index(CourseListInputModel input)
        {
            ViewData["Title"] = "Catalogo dei corsi";
            ListViewModel<CourseViewModel> courses = await courseService.GetCoursesAsync(input);

            CourseListViewModel viewModel = new()
            {
                Courses = courses,
                Input = input
            };

            return View(viewModel);
        }

        [AllowAnonymous]
        public async Task<IActionResult> Detail(int id)
        {
            CourseDetailViewModel viewModel = await courseService.GetCourseAsync(id);
            ViewData["Title"] = viewModel.Title;
            return View(viewModel);
        }

        [Authorize(Roles = nameof(Role.Teacher))]
        public IActionResult Create()
        {
            ViewData["Title"] = "Nuovo corso";
            CourseCreateInputModel inputModel = new();
            return View(inputModel);
        }

        [HttpPost]
        [Authorize(Roles = nameof(Role.Teacher))]
        public async Task<IActionResult> Create(CourseCreateInputModel inputModel, [FromServices] IAuthorizationService authorizationService, [FromServices] IEmailClient emailClient, [FromServices] IOptionsMonitor<UsersOptions> usersOptions)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    CourseDetailViewModel course = await courseService.CreateCourseAsync(inputModel);

                    // Per non inserire logica nel controller, potremmo spostare questo blocco all'interno del metodo CreateCourseAsync del servizio applicativo
                    // ...ma attenzione a non creare riferimenti circolari! Se il course service dipende da IAuthorizationService
                    // ...e viceversa l'authorization handler dipende dal course service, allora la dependency injection non riuscir� a risolvere nessuno dei due servizi, dandoci un errore
                    AuthorizationResult result = await authorizationService.AuthorizeAsync(User, nameof(Policy.CourseLimit));
                    if (!result.Succeeded)
                    {
                        await emailClient.SendEmailAsync(usersOptions.CurrentValue.NotificationEmailRecipient, "Avviso superamento soglia", $"Il docente {User.Identity.Name} ha creato molti corsi: verifica che riesca a gestirli tutti.");
                    }

                    TempData["ConfirmationMessage"] = "Ok! Il tuo corso � stato creato, ora perch� non inserisci anche gli altri dati?";
                    return RedirectToAction(nameof(Edit), new { id = course.Id });
                }
                catch (CourseTitleUnavailableException)
                {
                    ModelState.AddModelError(nameof(CourseDetailViewModel.Title), "Questo titolo gi� esiste");
                }
            }

            ViewData["Title"] = "Nuovo corso";
            return View(inputModel);
        }

        [Authorize(Policy = nameof(Policy.CourseAuthor))]
        [Authorize(Roles = nameof(Role.Teacher))]
        public async Task<IActionResult> Edit(int id)
        {
            ViewData["Title"] = "Modifica corso";
            CourseEditInputModel inputModel = await courseService.GetCourseForEditingAsync(id);
            return View(inputModel);
        }

        [HttpPost]
        [Authorize(Policy = nameof(Policy.CourseAuthor))]
        [Authorize(Roles = nameof(Role.Teacher))]
        public async Task<IActionResult> Edit(CourseEditInputModel inputModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    CourseDetailViewModel course = await courseService.EditCourseAsync(inputModel);
                    TempData["ConfirmationMessage"] = "I dati sono stati salvati con successo";
                    return RedirectToAction(nameof(Detail), new { id = inputModel.Id });
                }
                catch (CourseImageInvalidException)
                {
                    ModelState.AddModelError(nameof(CourseEditInputModel.Image), "L'immagine selezionata non � valida");
                }
                catch (CourseTitleUnavailableException)
                {
                    ModelState.AddModelError(nameof(CourseEditInputModel.Title), "Questo titolo gi� esiste");
                }
                catch (OptimisticConcurrencyException)
                {
                    ModelState.AddModelError("", "Spiacenti, il salvataggio non � andato a buon fine perch� nel frattempo un altro utente ha aggiornato il corso. Ti preghiamo di aggiornare la pagina e ripetere le modifiche.");
                }
            }

            ViewData["Title"] = "Modifica corso";
            return View(inputModel);
        }

        [HttpPost]
        [Authorize(Policy = nameof(Policy.CourseAuthor))]
        [Authorize(Roles = nameof(Role.Teacher))]
        public async Task<IActionResult> Delete(CourseDeleteInputModel inputModel)
        {
            await courseService.DeleteCourseAsync(inputModel);
            TempData["ConfirmationMessage"] = "Il corso � stato eliminato ma potrebbe continuare a comparire negli elenchi per un breve periodo, finch� la cache non viene aggiornata.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> IsTitleAvailable(string title, int id = 0)
        {
            bool result = await courseService.IsTitleAvailableAsync(title, id);
            return Json(result);
        }

        public async Task<IActionResult> Subscribe(int id, string token)
        {
            CourseSubscribeInputModel inputModel = await courseService.CapturePaymentAsync(id, token);
            await courseService.SubscribeCourseAsync(inputModel);
            TempData["ConfirmationMessage"] = "Grazie per esserti iscritto, guarda subito la prima lezione!";
            return RedirectToAction(nameof(Detail), new { id = id });
        }

        public async Task<IActionResult> Pay(int id)
        {
            string paymentUrl = await courseService.GetPaymentUrlAsync(id);
            return Redirect(paymentUrl);
        }

        [Authorize(Policy = nameof(Policy.CourseSubscriber))]
        public async Task<IActionResult> Vote(int id)
        {
            CourseVoteInputModel inputModel = new()
            {
                Id = id,
                Vote = await courseService.GetCourseVoteAsync(id) ?? 0
            };

            return View(inputModel);
        }

        [Authorize(Policy = nameof(Policy.CourseSubscriber))]
        [HttpPost]
        public async Task<IActionResult> Vote(CourseVoteInputModel inputModel)
        {
            await courseService.VoteCourseAsync(inputModel);
            TempData["ConfirmationMessage"] = "Grazie per aver votato!";
            return RedirectToAction(nameof(Detail), new { id = inputModel.Id });
        }

        [Authorize(Policy = nameof(Policy.CourseSubscriber))]
        public async Task<IActionResult> Receipt(int id)
        {
            CourseSubscriptionViewModel viewModel = await courseService.GetCourseSubscriptionAsync(id);
            // return View(viewModel);

            ViewAsPdf pdf = new ViewAsPdf
            {
                Model = viewModel,
                ViewName = nameof(Receipt),
                PageMargins = new Margins { Top = 10, Left = 10, Right = 10, Bottom = 20 },
                PageSize = Size.A4,
                PageOrientation = Orientation.Portrait,
                FileName = $"{viewModel.Title} - ricevuta iscrizione.pdf"
            };

            byte[] fileContents = await pdf.BuildFile(ControllerContext);

            // Salvare fileContents

            return File(fileContents, "application/pdf", pdf.FileName);
        }

    }

}