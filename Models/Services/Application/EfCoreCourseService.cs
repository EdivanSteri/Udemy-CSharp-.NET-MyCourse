using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyCourse.Models.Services.Infrastructure;
using MyCourse.Models.ViewModels;

namespace MyCourse.Models.Services.Application
{
    public class EfCoreCourseService : ICourseService
    {
        private readonly MyCourseDbContext dbContext;
        public EfCoreCourseService (MyCourseDbContext dbContext){
            this.dbContext = dbContext;
        }

        public async Task<CourseDetailViewModel> GetCourseAsync(int id)
        {

            var course = await dbContext.Courses
                .AsNoTracking()
                .Include(course => course.Lessons)
                .Where(course => course.Id == id)
                .SingleAsync();

            var courseDetailViewModel = new CourseDetailViewModel
            {
                Id = (int)course.Id,
                Title = course.Title,
                ImagePath = course.ImagePath,
                Author = course.Author,
                Rating = course.Rating,
                CurrentPrice = course.CurrentyPrice,
                FullPrice = course.FullPrice,
                Lessons = course.Lessons.Select(lesson => new LessonViewModel
                {
                    Id = (int)lesson.Id,
                    Title = lesson.Title,
                    Description = lesson.Description,
                    Duration = lesson.Duration
                })
                .ToList()
            };
            //.FirstOrDefault(); //restituisce null se l'elenco è vuoto e non solleva mai un'eccezione
            //.SingleOrDefault() //tollera il fatto che l'elenco sia vuoto e in quel caso restituisce null, oppure se l'elenco contiene più di un elemento solleva un'eccezione
            //.First(); //restituisce il primo elemento dell'elenco, ma se l'lelenco è vuoto solleva un'eccezione
            //restituisce il primo elemento dell'elenco, ma se l'lelenco ne contiene 0 o più di uno mi solleva un'eccezione

            return courseDetailViewModel;            
        }

        public async Task<List<CourseViewModel>> GetCoursesAsync()
        {
                IQueryable<CourseViewModel> queryLinq = dbContext.Courses
                .AsNoTracking()
                .Select(course => CourseViewModel.FromEntity(course)) ;

                List<CourseViewModel> courses = await queryLinq.ToListAsync();

            return courses;
        }
    }
}