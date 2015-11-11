using OsisModel.Models;
using System;
using System.Collections.Generic;
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

        }

    }

    public interface ISchoolClass
    {
        OsisContext getDBContext();
        List<SchoolClassSingle> getListofClassesofSchool();
        bool addNewClasstoSchool(SchoolClassViewModel schoolVM);


    }
}