using OsisModel.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OsisModel.Services
{
    public class StudentService:CommonServices,IStudentService
    {
        private OsisContext db = new OsisContext();

        private ModelStateDictionary _modelstate;

        public StudentService(ModelStateDictionary modelstate)
        {
            _modelstate = modelstate;
        }

        public OsisContext getDBContext()
        {
            return db;
        }

        public PagedList.IPagedList<StudentSingle> getStudentList(int? page, string user = null)
        {
            Tuple<int,int> currentschoolandacademicyear = getUserCurrentSchool(db, user);
            int pageSize = Convert.ToInt32(ConfigurationManager.AppSettings["pageSize"]);
            var pageNumber = page ?? 1;
            var students = db.StudentSingles.AsNoTracking().OrderBy(d => d.RegistrationNo).Where(sa => sa.SchoolRefID == currentschoolandacademicyear.Item1 && sa.AcademicYearRefID == currentschoolandacademicyear.Item2);

            return students.ToPagedList(pageNumber, pageSize);
        }
    }

    public interface IStudentService
    {
        OsisContext getDBContext();
        PagedList.IPagedList<StudentSingle> getStudentList(int? page, string user = null);
    }
}