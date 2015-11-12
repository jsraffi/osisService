using AutoMapper;
using OsisModel.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace OsisModel.Services
{
    public class SchoolService:CommonServices,ISchoolService
    {
        private ModelStateDictionary _modelstate;

        public SchoolService(ModelStateDictionary modelstate)
        {
            _modelstate = modelstate;
        }

        private OsisContext db = new OsisContext();

        public OsisContext getDBContext()
        {
            return db;
        }

        public List<School> getSchoolList()
        {
            return db.Schools.OrderBy(sch => sch.SchoolName).ToList();
        }

        public SchoolViewModel findSchoolById(int? id)
        {
             School school =  db.Schools.Find(id);
             SchoolViewModel SchoolVM = Mapper.Map<SchoolViewModel>(school);

            return SchoolVM;
        }

        public  bool createNewSchool(SchoolViewModel schoolVM)
        {
            School schoolmodel = Mapper.Map<School>(schoolVM);
            db.Schools.Add(schoolmodel);
            db.SaveChanges();
            return true;
        }

        public bool saveAfterEdit(SchoolViewModel schoolVM)
        {
            School school = Mapper.Map<School>(schoolVM);
            db.Entry(school).State = EntityState.Modified;
            db.SaveChanges();
            return true;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

    }

    public interface ISchoolService
    {
        OsisContext getDBContext();
        List<School> getSchoolList();
        SchoolViewModel findSchoolById(int? id);
        bool createNewSchool(SchoolViewModel schooolVM);
        bool saveAfterEdit(SchoolViewModel schoolVM);
    }

}