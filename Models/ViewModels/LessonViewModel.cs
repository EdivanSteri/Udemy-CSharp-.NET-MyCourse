using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace MyCourse.Models.ViewModels
{
    public class LessonViewModel
    {
        public string Title { get; set; }
        public TimeSpan Duration {get; set;}

        public static LessonViewModel FromDataRow(DataRow lessonRow)
        {
            var lessonViewModel = new LessonViewModel { 
                Title = Convert.ToString(lessonRow["Title"]),
                Duration = TimeSpan.Parse(Convert.ToString(lessonRow["Duration"]))
            };
            return lessonViewModel;
        }
    }
}