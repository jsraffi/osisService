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
            
            //this new code from previous version optimize for performance
            ViewBag.SchoolRefID = new SelectList(db.Schools.Select(x => new { x.SchoolID, x.SchoolName }),"SchoolID","SchoolName");
            ViewBag.AcademicYearRefID = new SelectList(db.AcademicYears.Select(x => new { x.AcademicYearID, x.DisplayYear }), "AcademicYearID", "DisplayYear");
            ViewBag.ClassRefID = new SelectList(db.SchoolClasses.Select(x => new {x.ClassID,x.ClassName}),"ClassID","ClassName");
            StudentViewModel stuVMObj = new StudentViewModel();

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
               using (var dbosisTransaction = db.Database.BeginTransaction())
               {
                   
               try
                  {
                        if (ModelState.IsValid)
                        {
                            Student studentmap = Mapper.Map<Student>(student);
                            studentmap.StudentID = Guid.NewGuid();

                            studentmap.StudentCurrentYear[0].AcademicYearRefID = student.AcademicYearRefID;
                            studentmap.StudentCurrentYear[0].SchoolRefID = student.SchoolRefID;
                            studentmap.StudentCurrentYear[0].ClassRefID = student.ClassRefID;
                            studentmap.StudentCurrentYear[0].StudentRefID = studentmap.StudentID;
                            studentmap.StudentCurrentYear[0].Active = true;
                            var LastRegNo = db.Database.SqlQuery<int>("Select LastRegNo from Schools with (XLOCK) where SchoolID={0}", student.SchoolRefID).FirstOrDefault<int>();
                            int NewRegNo = LastRegNo + 1;

                            db.Database.ExecuteSqlCommand("UPDATE Schools SET LastRegNo = {0} where schoolID= {1}", NewRegNo, student.SchoolRefID);

                            studentmap.RegistrationNo = NewRegNo;
                            
                            
                            db.Students.Add(studentmap);
                            
                            await db.SaveChangesAsync();
                            dbosisTransaction.Commit();
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            ModelState.AddModelError("", "Error occured in the form");
                            ViewBag.SchoolRefID = new SelectList(db.Schools.Select(x => new { x.SchoolID, x.SchoolName }), "SchoolID", "SchoolName");
                            ViewBag.AcademicYearRefID = new SelectList(db.AcademicYears.Select(x => new { x.AcademicYearID, x.DisplayYear }), "AcademicYearID", "DisplayYear");
                            ViewBag.ClassRefID = new SelectList(db.SchoolClasses.Select(x => new { x.ClassID, x.ClassName }), "ClassID", "ClassName");
            
                            return View(student);
                        } 
                    }
               catch (Exception e)
               {
                   dbosisTransaction.Rollback();
                   ModelState.AddModelError("", e.InnerException);
                   ViewBag.SchoolRefID = new SelectList(db.Schools.Select(x => new { x.SchoolID, x.SchoolName }), "SchoolID", "SchoolName");
                   ViewBag.AcademicYearRefID = new SelectList(db.AcademicYears.Select(x => new { x.AcademicYearID, x.DisplayYear }), "AcademicYearID", "DisplayYear");
                   ViewBag.ClassRefID = new SelectList(db.SchoolClasses.Select(x => new { x.ClassID, x.ClassName }), "ClassID", "ClassName");
                   return View(student);
               }
            }
         }
        // GET: /Student/Edit/5
        public async Task<ActionResult> Edit(Guid? id)
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

        // POST: /Student/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include="StudentID,Address_Address1,Address_Address2,Address_City,Address_Pincode,Address_Mobile,Phone,Email,website,Name,NickName,Sex,DOB,FathersName,MothersName,FathersOccupation,MothersOccupation,MothersPhone,FathersPhone,FathersQualification,MothersQualifiication,MotherTongue,IdentificationMarks,KnowMedicalCondition,SpecialTalents,ReasonForOlivekids,PlayschoolExperience,DateOfJoining,CenterCode,RegistrationNo,AdmissionFee,TotalCourseFee,TermFee,Height,Weight")] Student student)
        {
            if (ModelState.IsValid)
            {
                db.Entry(student).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(student);
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
