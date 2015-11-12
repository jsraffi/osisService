using AutoMapper;
using OsisModel.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OsisModel.Services
{
    public class SchoolClassService:CommonServices,ISchoolClass
    {   
        private OsisContext db = new OsisContext();

        private ModelStateDictionary _modelState;

        public SchoolClassService(ModelStateDictionary modelstate)
        {
            _modelState = modelstate;
        }

        public OsisContext getDBContext()
        {
            return db;
        }
        public List<SchoolClassSingle> getListofClassesofSchool()
        {
            return db.SchoolClassSingles.OrderBy(c => c.ClassName).ToList();
        }

        public bool addNewClasstoSchool(SchoolClassViewModel SchoolVM)
        {
            SchoolClass schoolclassmodel = Mapper.Map<SchoolClass>(SchoolVM);
            using (var dbtransaction = db.Database.BeginTransaction())
            {
                try
                {

                    var LastClassOrderNo = db.Database.SqlQuery<int>("Select ISNULL(Max(ClassOrder),0) from SchoolClasses with (XLOCK,HOLDLOCK) where SchoolRefID={0}", schoolclassmodel.SchoolRefID).SingleOrDefault<int>();
                    schoolclassmodel.ClassOrder = LastClassOrderNo + 1;
                    db.SchoolClasses.Add(schoolclassmodel);
                    db.SaveChanges();
                    dbtransaction.Commit();
                    return true;

                }
                catch (Exception e)
                {
                    dbtransaction.Rollback();
                    _modelState.AddModelError("", "The class was not able to add to school.Please try again later" + e.Message);
                    return false;
                }
            }
        }

        public SchoolClassViewModel findClassByID(int? id)
        {
            SchoolClass schoolclassmodel = db.SchoolClasses.Find(id);
            SchoolClassViewModel SchoolclassVM = Mapper.Map<SchoolClassViewModel>(schoolclassmodel);
            return SchoolclassVM;
        }

        public bool saveClassAfterEditing(SchoolClassViewModel SchoolClassVM)
        {
            SchoolClass SchoolClassModel = Mapper.Map<SchoolClass>(SchoolClassVM);
            db.Entry(SchoolClassModel).State = EntityState.Modified;
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

    public interface ISchoolClass
    {
        OsisContext getDBContext();
        List<SchoolClassSingle> getListofClassesofSchool();
        bool addNewClasstoSchool(SchoolClassViewModel schoolVM);
        SchoolClassViewModel findClassByID(int? id);
        bool saveClassAfterEditing(SchoolClassViewModel SchoolClassVM);

    }
}