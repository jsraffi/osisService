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
using OsisModel.Services;

namespace OsisModel.Controllers
{// I have put this on github
    public class StudentController : Controller
    {
        private OsisContext db = new OsisContext();

        private IStudentService _service;

        public StudentController(IStudentService service)
        {
            _service = service;
        }

        public StudentController()
        {
            _service = new StudentService(this.ModelState);
        }

        public JsonResult GetClassBySchoolID(int id)
        {

            var dbc = _service.getDBContext();
            var classes = dbc.SchoolClasses.AsNoTracking().Where(s => s.SchoolRefID == id).Select(c => new { Text = c.ClassName, Value = c.ClassID }).ToList();

            return Json(classes, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAcademicYearBySchoolID(int id)
        {

            var dbc = _service.getDBContext();
            var acyear = dbc.AcademicYears.AsNoTracking().Where(s => s.SchoolRefID == id).Select(a => new { Text = a.DisplayYear, Value = a.AcademicYearID }).ToList();

            return Json(acyear, JsonRequestBehavior.AllowGet);
        }

        // GET: /Student/
        public async Task<ActionResult> Index(int? page)
        {
            return View(await Task.Run(() => _service.getStudentList(page,User.Identity.GetUserName())));
        }   

        // GET: /Student/Details/5
        public async Task<ActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }


            StudentViewModel studentVM = await Task.Run(() => _service.findStudentById(id));
            
            if (studentVM == null)
            {
                return HttpNotFound();
            }
            studentVM.Sex = (studentVM.Sex == "M") ? "Male" : "Female";

            return View(studentVM);
        }

        // GET: /Student/Create
        public ActionResult Create()
        {

            var dbc = _service.getDBContext();

            //This new code from previous version optimized for performance
            //context object does not return all the colums of table for dropdown boxes 
            //only id and display columns are fetched(checked with sql profiler)

            ViewBag.SchoolRefID = new SelectList(dbc.Schools.AsNoTracking().Select(x => new { x.SchoolID, x.SchoolName }), "SchoolID", "SchoolName");
            ViewBag.AcademicYearRefID = new SelectList(dbc.AcademicYears.AsNoTracking().Select(x => new { x.AcademicYearID, x.DisplayYear }), "AcademicYearID", "DisplayYear");
            ViewBag.ClassRefID = new SelectList(dbc.SchoolClasses.AsNoTracking().Select(x => new { x.ClassID, x.ClassName }), "ClassID", "ClassName");
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
        public async Task<ActionResult> Create(StudentViewModel StudentVM)
        {
               
               //Create a transaction 
                        if (ModelState.IsValid)
                        {
                            if (await Task.Run(() => _service.CreateStudent(StudentVM) == true))
                            {
                                //Return to student index page
                                return RedirectToAction("Index");
                            }
                        }


            //if model is invalid the following will excuted
            //repoulate the dropdown's with values they had
            //efore post
                        ModelState.AddModelError("", "Error: model state is not valid");
                        ViewBag.SchoolRefID = new SelectList(db.Schools.AsNoTracking().Select(x => new { x.SchoolID, x.SchoolName }), "SchoolID", "SchoolName", StudentVM.SchoolRefID);
                        ViewBag.AcademicYearRefID = new SelectList(db.AcademicYears.AsNoTracking().Select(x => new { x.AcademicYearID, x.DisplayYear }), "AcademicYearID", "DisplayYear", StudentVM.AcademicYearRefID);
                        ViewBag.ClassRefID = new SelectList(db.SchoolClasses.AsNoTracking().Select(x => new { x.ClassID, x.ClassName }), "ClassID", "ClassName", StudentVM.ClassRefID);
                    
                        return View(StudentVM);
              } 
                   
        //This action uses provide editing the school for a student record
        public async Task<ActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //get the student by id using db context
            StudentViewModel studentVM = await Task.Run(() => _service.findStudentById(id));

            if (studentVM == null)
            {
                return HttpNotFound();
            }

            //populate view model with student values retrived from db            
                       
            
            
            // A model is created just to hold a subset StudentCurrentyear Record
            // namely schoolrefid, academictyearrefid and classrefid
            StudentCurrentYearSubset CurrentYear = new StudentCurrentYearSubset();

            //This loop is nesscary as studentcurrentyear object/table may contain 
            // more than one record for student who are studying in school for 
            // more than one year, so the active flag is checked before retriving the
            // the relevant record for editing
            foreach(var studentCurrentRec in studentVM.StudentCurrentYear)
            {
                if (studentCurrentRec.Active == true)
                {   
                    CurrentYear.SchoolRefID = studentCurrentRec.SchoolRefID;
                    CurrentYear.AcademicYearRefID= studentCurrentRec.AcademicYearRefID;
                    CurrentYear.ClassRefID=studentCurrentRec.ClassRefID;
                    break;
                }
            }

            var dbc = _service.getDBContext();
            ViewBag.SchoolRefID = new SelectList(dbc.Schools.AsNoTracking().Select(x => new { x.SchoolID, x.SchoolName }), "SchoolID", "SchoolName", CurrentYear.SchoolRefID);
            ViewBag.AcademicYearRefID = new SelectList(dbc.AcademicYears.AsNoTracking().Where(sch => sch.SchoolRefID == CurrentYear.SchoolRefID).Select(x => new { x.AcademicYearID, x.DisplayYear }), "AcademicYearID", "DisplayYear", CurrentYear.AcademicYearRefID);
            ViewBag.ClassRefID = new SelectList(dbc.SchoolClasses.AsNoTracking().Where(sch => sch.SchoolRefID == CurrentYear.SchoolRefID).Select(x => new { x.ClassID, x.ClassName }), "ClassID", "ClassName", CurrentYear.ClassRefID);
            
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
                if(await Task.Run(() => _service.saveAfterEdit(studentVM)))
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "Error:returned from service class");
                   
                }
                
            }
            else
            {
                ModelState.AddModelError("", "Error: model state is not valid");
            }

            var dbc = _service.getDBContext();
            ViewBag.SchoolRefID = new SelectList(dbc.Schools.AsNoTracking().Select(x => new { x.SchoolID, x.SchoolName }), "SchoolID", "SchoolName", studentVM.StudentCurrentYear[0].SchoolRefID);
            ViewBag.AcademicYearRefID = new SelectList(dbc.AcademicYears.AsNoTracking().Where(sch => sch.SchoolRefID == studentVM.StudentCurrentYear[0].SchoolRefID).Select(x => new { x.AcademicYearID, x.DisplayYear }), "AcademicYearID", "DisplayYear", studentVM.StudentCurrentYear[0].AcademicYearRefID);
            ViewBag.ClassRefID = new SelectList(dbc.SchoolClasses.AsNoTracking().Where(sch => sch.SchoolRefID == studentVM.StudentCurrentYear[0].SchoolRefID).Select(x => new { x.ClassID, x.ClassName }), "ClassID", "ClassName", studentVM.StudentCurrentYear[0].ClassRefID);
            
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

            var students = db.CurrentYearSingles.AsNoTracking().Where(x => x.SchoolRefID == userprefer.SchoolRefID && x.AcademicYearRefID == userprefer.AcademicYearRefID).OrderBy(d => d.Name);
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
