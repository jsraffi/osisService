using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using OsisModel.Models;
using System.Configuration;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using PagedList;
using AutoMapper;
using System.Globalization;

namespace OsisModel.Controllers
{// I have put this on github
    public class StudentController : Controller
    {
        private OsisContext db = new OsisContext();


        public JsonResult GetClassBySchoolID(int id)
        {
            var classes = db.SchoolClasses.Where(s => s.SchoolRefID == id).Select(c => new { Text = c.ClassName, Value = c.ClassID }).ToList();

            return Json(classes, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAcademicYearBySchoolID(int id)
        {
            

            var acyear = db.AcademicYears.Where(s => s.SchoolRefID == id).Select(a => new { Text = a.DisplayYear, Value = a.AcademicYearID }).ToList();

            return Json(acyear, JsonRequestBehavior.AllowGet);
        }

        // GET: /Student/
        public ActionResult Index(int? page)
        {
            //14-Aug-2015-changing the student index view to display just students of the current 
            //logged user's school and academic year preference.

            //Get current logged in user need reference to Microsoft.AspNet.Identity
            string userid = User.Identity.GetUserId();

            //Get logged in users school and academic year preference
            var userprefer = db.UserPreferences.Where(a => a.UserID == userid).Select(x => new { x.SchoolRefID, x.AcademicYearRefID }).FirstOrDefault();
            
            //need this for paging if page null then 1
            var pageNumber = page ?? 1;
            
            //get the page size from config file
            int pageSize = Convert.ToInt32(ConfigurationManager.AppSettings["pageSize"]);
            
            //to display fields from three different table a database view is used
            //a model Studentsingle just for index page listing is used.
            // all fields required for display and used in where clause 
            //needs to be there in StudentSingles model
            var students = db.StudentSingles.OrderBy(d => d.RegistrationNo).Where(sa => sa.SchoolRefID == userprefer.SchoolRefID && sa.AcademicYearRefID == userprefer.AcademicYearRefID );
            
            //PagedList is Nugget package for just paging
            return View(students.ToPagedList(pageNumber, pageSize));
        }   

        // GET: /Student/Details/5
        public async Task<ActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            
            Student student = await db.Students.FindAsync(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        // GET: /Student/Create
        public ActionResult Create()
        {
            
            //This new code from previous version optimized for performance
            //context object does not return all the colums of table for dropdown boxes 
            //only id and display columns are fetched(checked with sql profiler)
 
            ViewBag.SchoolRefID = new SelectList(db.Schools.Select(x => new { x.SchoolID, x.SchoolName }),"SchoolID","SchoolName");
            ViewBag.AcademicYearRefID = new SelectList(db.AcademicYears.Select(x => new { x.AcademicYearID, x.DisplayYear }), "AcademicYearID", "DisplayYear");
            ViewBag.ClassRefID = new SelectList(db.SchoolClasses.Select(x => new {x.ClassID,x.ClassName}),"ClassID","ClassName");
            StudentViewModel stuVMObj = new StudentViewModel();

            //Create a empty row in Current year table/object
            //data is returned will form gets posted
            List<StudentCurrentYear> CyObj = new List<StudentCurrentYear>
            {
                new StudentCurrentYear
                {
                    SchoolRefID=0,
                    AcademicYearRefID=0,
                    ClassRefID =0,
                    StudentRefID =Guid.NewGuid(),
                    Active = true
                }
            };

            stuVMObj.StudentCurrentYear = CyObj;

            return View(stuVMObj);
        }

        // POST: /Student/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(StudentViewModel student)
        {
               
               //Create a transaction 
               using (var dbosisTransaction = db.Database.BeginTransaction())
               {
                   
               try
                  {
                        if (ModelState.IsValid)
                        {
                            //user auto mapper to fill student object incluing
                            // navigation object studentcurrentyear
                            Student studentmap = Mapper.Map<Student>(student);
                            studentmap.StudentID = Guid.NewGuid();

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
                            await db.SaveChangesAsync();
                            
                            //Commit the transaction
                            dbosisTransaction.Commit();
                            
                            //Return to student index page
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            //if model is invalid the following will excuted
                            //repoulate the dropdown's with values they had
                            //before post
                            ModelState.AddModelError("", "Error: model state is not valid");
                            ViewBag.SchoolRefID = new SelectList(db.Schools.Select(x => new { x.SchoolID, x.SchoolName }), "SchoolID", "SchoolName", student.SchoolRefID);
                            ViewBag.AcademicYearRefID = new SelectList(db.AcademicYears.Select(x => new { x.AcademicYearID, x.DisplayYear }), "AcademicYearID", "DisplayYear",student.AcademicYearRefID);
                            ViewBag.ClassRefID = new SelectList(db.SchoolClasses.Select(x => new { x.ClassID, x.ClassName }), "ClassID", "ClassName", student.ClassRefID);
            
                            return View(student);
                        } 
                    }
               catch (Exception e)
               {
                   dbosisTransaction.Rollback();
                   ModelState.AddModelError("", e.InnerException);
                   TempData["errormessage"] = e.Message;
                   ViewBag.SchoolRefID = new SelectList(db.Schools.Select(x => new { x.SchoolID, x.SchoolName }), "SchoolID", "SchoolName",student.SchoolRefID);
                   ViewBag.AcademicYearRefID = new SelectList(db.AcademicYears.Select(x => new { x.AcademicYearID, x.DisplayYear }), "AcademicYearID", "DisplayYear",student.AcademicYearRefID);
                   ViewBag.ClassRefID = new SelectList(db.SchoolClasses.Select(x => new { x.ClassID, x.ClassName }), "ClassID", "ClassName",student.ClassRefID);
                   return View(student);
               }
            }
         }
        //This action uses provide editing the school for a student record
        public async Task<ActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            
            //if the student by id using db context
            Student student = await db.Students.FindAsync(id);

            if (student == null)
            {
                return HttpNotFound();
            }

            //populate view model with student values retrived from db            
            StudentViewModel studentVM = Mapper.Map<StudentViewModel>(student);

            //student view model has constructor which populates
            // Data of joining property to current date
            // hence repopulating date of joining property with database retrived value
            studentVM.DateOfJoining = student.DateOfJoining;
                       
            
            
            // A model is created just to hold a subset StudentCurrentyear Record
            // namely schoolrefid, academictyearrefid and classrefid
            StudentCurrentYearSubset CurrentYear = new StudentCurrentYearSubset();

            //This loop is nesscary as studentcurrentyear object/table may contain 
            // more than one record for student who are studying in school for 
            // more than one year, so the active flag is checked before retriving the
            // the relevant record for editing
            foreach(var studentCurrentRec in student.StudentCurrentYear)
            {
                if (studentCurrentRec.Active == true)
                {   
                    CurrentYear.SchoolRefID = studentCurrentRec.SchoolRefID;
                    CurrentYear.AcademicYearRefID= studentCurrentRec.AcademicYearRefID;
                    CurrentYear.ClassRefID=studentCurrentRec.ClassRefID;
                    break;
                }
            }
            

            ViewBag.SchoolRefID = new SelectList(db.Schools.Select(x => new { x.SchoolID, x.SchoolName }), "SchoolID", "SchoolName", CurrentYear.SchoolRefID);
            ViewBag.AcademicYearRefID = new SelectList(db.AcademicYears.Where(sch => sch.SchoolRefID == CurrentYear.SchoolRefID).Select(x => new { x.AcademicYearID, x.DisplayYear }), "AcademicYearID", "DisplayYear",CurrentYear.AcademicYearRefID);
            ViewBag.ClassRefID = new SelectList(db.SchoolClasses.Where(sch => sch.SchoolRefID == CurrentYear.SchoolRefID).Select(x => new { x.ClassID, x.ClassName }), "ClassID", "ClassName",CurrentYear.ClassRefID);
            
            return View(studentVM);
        }

        // POST: /Student/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(StudentViewModel studentVM)
        {
            if (ModelState.IsValid)
            {
                Student studentDBC = Mapper.Map<Student>(studentVM);
                studentDBC.StudentCurrentYear[0].Active = true;
                

                foreach(var studCY in studentDBC.StudentCurrentYear)
                {
                    db.StudentCurrentYears.Attach(studCY);
                    db.Entry(studCY).State = EntityState.Modified;
                }
                db.Entry(studentDBC).State = EntityState.Modified;
                
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError("", "Error: model state is not valid");
            }
            ViewBag.SchoolRefID = new SelectList(db.Schools.Select(x => new { x.SchoolID, x.SchoolName }), "SchoolID", "SchoolName", studentVM.StudentCurrentYear[0].SchoolRefID);
            ViewBag.AcademicYearRefID = new SelectList(db.AcademicYears.Where(sch => sch.SchoolRefID == studentVM.StudentCurrentYear[0].SchoolRefID).Select(x => new { x.AcademicYearID, x.DisplayYear }), "AcademicYearID", "DisplayYear", studentVM.StudentCurrentYear[0].AcademicYearRefID);
            ViewBag.ClassRefID = new SelectList(db.SchoolClasses.Where(sch => sch.SchoolRefID == studentVM.StudentCurrentYear[0].SchoolRefID).Select(x => new { x.ClassID, x.ClassName }), "ClassID", "ClassName",studentVM.StudentCurrentYear[0].ClassRefID);
            
            return View(studentVM);
        }

        // GET: /Student/Delete/5
        public async Task<ActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = await db.Students.FindAsync(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        // POST: /Student/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(Guid id)
        {
            Student student = await db.Students.FindAsync(id);
            db.Students.Remove(student);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        public ActionResult StudentCurrentYearIndex(int? page)
        {
            var pageNumber = page ?? 1;
            int pageSize = Convert.ToInt32(ConfigurationManager.AppSettings["pageSize"]);
            var userprefer = db.UserPreferences.Where(a => a.UserName == User.Identity.Name).Select(x => new { x.SchoolRefID, x.AcademicYearRefID }).FirstOrDefault();

            var students = db.CurrentYearSingles.Where(x => x.SchoolRefID == userprefer.SchoolRefID && x.AcademicYearRefID == userprefer.AcademicYearRefID).OrderBy(d => d.Name);
            return View(students.ToPagedList(pageNumber, pageSize));
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
}
