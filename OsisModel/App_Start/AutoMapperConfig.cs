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
            AutoMapper.Mapper.CreateMap<Invoice, InvoiceViewModel>().ForMember(dest => dest.InvoiceDetailsViewModel, opt => opt.MapFrom(src => src.InvoiceDetails));
            AutoMapper.Mapper.CreateMap<InvoiceViewModel, Invoice>().ForMember(dest => dest.InvoiceDetails, opt => opt.MapFrom(src => src.InvoiceDetailsViewModel));
            AutoMapper.Mapper.CreateMap<InvoiceDetails, InvoiceDetailsViewModel>();
            AutoMapper.Mapper.CreateMap<InvoiceDetails, InvoiceDetailsViewModel>().ReverseMap();

        }
    }
}