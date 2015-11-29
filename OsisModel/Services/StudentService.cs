using AutoMapper;
using OsisModel.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OsisModel.Services
{
    public class StudentService : CommonServices, IStudentService
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
            var students = db.StudentSingles.AsNoTracking().OrderBy(d => d.RegistrationNo).Where(sa => sa.SchoolRefID == currentschoolandacademicyear.Item1 && sa.AcademicYearRefID == currentschoolandacademicyear.Item2 &&).;

            return students.ToPagedList(pageNumber, pageSize);
        }

        public StudentViewModel findStudentById(Guid? id)
        {
           
           return Mapper.Map<StudentViewModel>(db.Students.Find(id));
        }

        public StudentViewModel getStudentForEdit(Guid? id)
        {
            Student StudentModel = db.Students.Find(id);

            StudentViewModel StudentVM = Mapper.Map<StudentViewModel>(StudentModel);
            
            //student view model has constructor which populates
            // Data of joining property to current date
            // hence repopulating date of joining property with database retrived value
            StudentVM.DateOfJoining = StudentModel.DateOfJoining;
            
            return StudentVM;
        }

        public bool saveAfterEdit(StudentViewModel StudentVM)
        {
            Student StudentModel = Mapper.Map<Student>(StudentVM);
            //StudentModel.StudentCurrentYear[1].Active = true;


           foreach (var studentCurrYear in StudentModel.StudentCurrentYear)
            {

                if(studentCurrYear.Active== true)
                {
                    db.StudentCurrentYears.Attach(studentCurrYear);
                    db.Entry(studentCurrYear).State = EntityState.Modified;
                }
                
            }
            db.Entry(StudentModel).State = EntityState.Modified;

            db.SaveChanges();

            return true;

        }
        public bool CreateStudent(StudentViewModel studentVM)
        {
            
            using (var dbosisTransaction = db.Database.BeginTransaction())
            {

                try
                {
                    //user auto mapper to fill student object incluing
                    // navigation object studentcurrentyear
                    Student studentmap = Mapper.Map<Student>(studentVM);
                    studentmap.StudentID = Guid.NewGuid();
                    studentmap.StudentCurrentYear[0].PromotedOn = DateTime.Now;
                    //The current is always true when create a student
                    // for the first time
                    studentmap.StudentCurrentYear[0].Active = true;
                    // the school table/object has registration no
                    // which needs to update for for every student
                    // always store the last registration no given to'
                    // a student
                    var LastRegNo = db.Database.SqlQuery<int>("Select LastRegNo from Schools with (XLOCK) where SchoolID={0}", studentmap.StudentCurrentYear[0].SchoolRefID).FirstOrDefault<int>();

                    // increment the lastregno by one
                    int NewRegNo = LastRegNo + 1;

                    //Update database school table with the new latest regno
                    db.Database.ExecuteSqlCommand("UPDATE Schools SET LastRegNo = {0} where schoolID= {1}", NewRegNo, studentmap.StudentCurrentYear[0].SchoolRefID);

                    // assign new registration to student object Registration No property
                    studentmap.RegistrationNo = NewRegNo;

                    //Add student object to student db context
                    db.Students.Add(studentmap);



                    //User Task Parallel Library(TPL) to save changes in asynchronous(background) way
                    db.SaveChanges();
                    
                    //Commit the transaction
                    dbosisTransaction.Commit();

                    return true;


                }
                catch (Exception e)
                {
                    dbosisTransaction.Rollback();
                    _modelstate.AddModelError("", e.InnerException);
                    return false;

                }
            }
        }

        
    }
    

    public interface IStudentService
    {
        OsisContext getDBContext();
        PagedList.IPagedList<StudentSingle> getStudentList(int? page, string user = null);
        StudentViewModel findStudentById(Guid? id);
        bool CreateStudent(StudentViewModel StudentVM);
        StudentViewModel getStudentForEdit(Guid? id);
        bool saveAfterEdit(StudentViewModel StudentVM);
        
    }
}