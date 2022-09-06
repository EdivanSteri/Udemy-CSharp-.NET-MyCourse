using FluentValidation;
using MyCourse.Models.InputModels.Courses;
using System.Reflection;

namespace MyCourse.Models.Validators
{
    public class CourseCreateValidator : AbstractValidator<CourseCreateInputModel>
    {
        public CourseCreateValidator()
        {
            RuleFor(model => model.Title)
                .NotEmpty().WithMessage("Il titolo è obbligatorio")
                .MinimumLength(10).WithMessage("Il titolo dev'essere di almeno {MinLenght} caratteri")
                .MaximumLength(100).WithMessage("Il titolo dev'essere di al massimo {MaxLenght} caratteri")
                .Matches(@"^[\w\s\.']+$").WithMessage("Titolo non valido")
                .Must(NotContainMyCourse).WithMessage("La parola MyCourse non può essere usata");
        }

        private bool NotContainMyCourse(string title)
        {
            return !title.Contains("MyCourse");
        }
    }
}
