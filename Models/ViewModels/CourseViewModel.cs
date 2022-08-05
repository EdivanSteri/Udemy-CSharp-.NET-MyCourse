using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using MyCourse.Models.Entities;
using MyCourse.Models.Enums;
using MyCourse.Models.ValueTypes;

namespace MyCourse.Models.ViewModels
{
    public class CourseViewModel
    {
        public int Id {get; set;}
        public string Title {get; set;}
        public string ImagePath {get; set;}
        public string Author {get; set;}
        
        public double Rating {get; set;}

        public Money FullPrice {get; set;}

        public Money CurrentPrice {get; set;}

        public static CourseViewModel FromDataRow(DataRow courseRow){
            var courseViewModel = new CourseViewModel { 
                Title = (string) courseRow["Title"],
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
                Id = Convert.ToInt32(courseRow["Id"])
            };
            return courseViewModel;
        }

        public static CourseViewModel FromEntity(Course course)
        {
            var courseViewModel = new CourseViewModel
            {
                Title = course.Title,
                Author = course.Author,
                ImagePath = course.ImagePath,
                Rating =course.Rating,
                FullPrice =course.FullPrice,
                CurrentPrice = course.CurrentyPrice,
                Id =(int) course.Id
            };
            return courseViewModel;
        }
    }
}