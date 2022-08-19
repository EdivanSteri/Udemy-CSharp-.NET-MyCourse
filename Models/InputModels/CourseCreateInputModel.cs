using Microsoft.AspNetCore.Mvc;
using MyCourse.Controllers;
using System.ComponentModel.DataAnnotations;

namespace MyCourse.Models.InputModels
{
    public class CourseCreateInputModel
    {
        [Required(ErrorMessage = "Il titolo è obbligatorio"),
         MinLength(10, ErrorMessage = "Il titolo dev'esseere di almeno {1} caratteri"), 
         MaxLength(100, ErrorMessage = "Il titolo dev'esseere al massimo di {1} caratteri"), 
         RegularExpression(@"^[\w\s\.]+$", ErrorMessage = "Titolo non valido"),
         Remote(action: nameof(CoursesController.IsTitleAvailable), controller: "Courses", ErrorMessage = "Il titolo esiste già")]
        public string Title { get; set; }
    }
}
