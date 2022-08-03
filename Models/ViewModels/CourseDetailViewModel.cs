using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using MyCourse.Models.Enums;
using MyCourse.Models.ValueTypes;

namespace MyCourse.Models.ViewModels
{
    public class CourseDetailViewModel : CourseViewModel
    {
        public string Description {get; set;}
        public List<LessonViewModel> Lessons  {get; set;}

        public TimeSpan TotalDuration {
            get => TimeSpan.FromSeconds(Lessons?.Sum(l => l.Duration.TotalSeconds) ?? 0);
        }

        public static new  CourseDetailViewModel FromDataRow(DataRow courseRow)
        {
            var courseDetailViewModel = new CourseDetailViewModel { 
                Title = (string) courseRow["Title"],
                Description = (string) courseRow["Description"],
                Author = (string) courseRow["Author"],
                ImagePath = (string) courseRow["ImagePath"],
                Rating = Convert.ToDouble(courseRow["Rating"]),
                FullPrice = new Money(
                    Enum.Parse<Currency>(Convert.ToString(courseRow["FullPrice_Currency"])),
                    Convert.ToDecimal(courseRow["FullPrice_Amount"])
                ),
                CurrentPrice = new Money(
                    Enum.Parse<Currency>(Convert.ToString(courseRow["CurrentyPrice_Currency"])),
                    Convert.ToDecimal(courseRow["CurrentyPrice_Amount"])
                ),
                Id = Convert.ToInt32(courseRow["Id"]),
                Lessons = new List<LessonViewModel>()
            };
            return courseDetailViewModel;
        }

    }
}