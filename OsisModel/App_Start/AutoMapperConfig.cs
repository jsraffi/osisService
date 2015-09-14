using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using OsisModel.Models;

namespace OsisModel
{
    public static class AutoMapperConfig
    {
        public static void RegisterMappings()
        {
            AutoMapper.Mapper.CreateMap<School, SchoolViewModel>();
            AutoMapper.Mapper.CreateMap<School, SchoolViewModel>().ReverseMap();
            AutoMapper.Mapper.CreateMap<AcademicYear, AcademicYearViewModel>();
            AutoMapper.Mapper.CreateMap<AcademicYear, AcademicYearViewModel>().ReverseMap();
            AutoMapper.Mapper.CreateMap<SchoolClass, SchoolClassViewModel>();
            AutoMapper.Mapper.CreateMap<SchoolClass, SchoolClassViewModel>().ReverseMap();
            AutoMapper.Mapper.CreateMap<StudentViewModel, Student>();
            AutoMapper.Mapper.CreateMap<StudentViewModel, Student>().ReverseMap();
            AutoMapper.Mapper.CreateMap<StudentListVM, StudentList>();
            AutoMapper.Mapper.CreateMap<StudentListVM, StudentList>().ReverseMap();    
        }
    }
}