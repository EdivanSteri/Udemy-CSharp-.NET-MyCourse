using Microsoft.AspNetCore.Mvc;
using MyCourse.Customizations.ModelBinders;
using MyCourse.Models.Options;

namespace MyCourse.Models.InputModels
{
    [ModelBinder(BinderType = typeof(CourseListInputModelBinder))]
    public class CourseListInputModel
    {
        private CoursesOrderOptions orderOptions;

        public CourseListInputModel(string search, int page, string orderby, bool ascending, CoursesOptions courseOptions)
        {

            //Sanitizzazione
            var orderOptions = courseOptions.Order;
            if (!orderOptions.Allow.Contains(orderby))
            {
                orderby = orderOptions.By;
                ascending = orderOptions.Ascending;
            }

            Search = search ?? "";
            Page = Math.Max(1, page);
            OrderBy = orderby;
            Ascending = ascending;

            Limit = Convert.ToInt32(courseOptions.PerPage);
            Offset = (Page - 1) * Limit;
        }

        public string Search { get; }
        public int Page { get; }
        public string OrderBy { get; }
        public bool Ascending { get; }
        public int Limit { get; }
        public int Offset { get; }
       
        
     }
}
