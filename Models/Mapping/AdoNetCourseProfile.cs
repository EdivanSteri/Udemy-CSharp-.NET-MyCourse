using AutoMapper;
using MyCourse.Models.Mapping.Resolvers;
using MyCourse.Models.ViewModels.Courses;
using MyCourse.Models.ViewModels.Lessons;
using System.Data;

namespace MyCourse.Models.Mapping
{
    public class AdoNetCourseProfile : Profile
    {
        public AdoNetCourseProfile()
        {
            CreateMap<DataRow, CourseViewModel>()
                .ForMember(viewModel => viewModel.Id, config => config.MapFrom(new IdResolver()))
                .ForMember(viewModel => viewModel.CurrentPrice, config => config.MapFrom(new MoneyResolver("CurrentPrice")))
                .ForMember(viewModel => viewModel.FullPrice, config => config.MapFrom(new MoneyResolver("FullPrice")))
                .ForAllMembers(config => config.MapFrom(new DefaultResolver(config.DestinationMember.Name)));
            CreateMap<DataRow, CourseDetailViewModel>()
            //.IncludeBase<DataRow, CourseViewModel>()
                .ForMember(viewModel => viewModel.Id, config => config.MapFrom(new IdResolver()))
                .ForMember(viewModel => viewModel.CurrentPrice, config => config.MapFrom(new MoneyResolver("CurrentPrice")))
                .ForMember(viewModel => viewModel.FullPrice, config => config.MapFrom(new MoneyResolver("FullPrice")))
                .ForMember(viewModel => viewModel.Lessons, config => config.Ignore())
                .ForMember(viewModel => viewModel.TotalCourseDuration, config => config.Ignore())
                .ForAllMembers(config => config.MapFrom(new DefaultResolver(config.DestinationMember.Name)));

            CreateMap<DataRow, LessonViewModel>()
                .ForMember(viewModel => viewModel.Id, config => config.MapFrom(new IdResolver()))
                .ForMember(viewModel => viewModel.Duration, config => config.MapFrom(new TimeSpanResolver("Duration")))
                .ForAllMembers(config => config.MapFrom(new DefaultResolver(config.DestinationMember.Name)));
        }
    }
}

