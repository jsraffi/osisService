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
using System.Diagnostics;
using System.IO;
using System.Configuration;
using PagedList;
using AutoMapper;
using OsisModel.Services;

namespace OsisModel.Controllers
{
    public class AcademicYearController : Controller
    {
        private OsisContext db = new OsisContext();
        
        private IAcademicYearService _service;

        public AcademicYearController()
        {
            _service = new AcademicYearService(this.ModelState);
        }
        
        public AcademicYearController(IAcademicYearService service)
        {
            _service = service;
        }
        // GET: /AcademicYear/
        public ActionResult Index(int? page)
        {
            var pageNumber = page ?? 1;
            int pageSize = Convert.ToInt32(ConfigurationManager.AppSettings["pageSize"]);
            return View(_service.getAcademicYearList().ToPagedList(pageNumber, pageSize));
        }

        
        public ActionResult Create()
        {
            ViewBag.SchoolRefID = new SelectList(db.Schools.AsNoTracking().Select(x => new { x.SchoolID,x.SchoolName}), "SchoolID", "SchoolName");
            return View();
        }

        // POST: /AcademicYear/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(AcademicYearViewModel academicyear)
        {
            if (ModelState.IsValid)
            {

                if( await _service.AddNewAcademicYear(academicyear) == true)
                {
                    return RedirectToAction("Index");
                }
                
            }

            ViewBag.SchoolRefID = new SelectList(db.Schools, "SchoolID", "SchoolName", academicyear.SchoolRefID);
            return View(academicyear);
        }

         //GET: /AcademicYear/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            int idnotnull = (int) id;
            
            AcademicYearViewModel academicYearVM = await _service.FindAcademicYearForEditing(idnotnull);
            if (academicYearVM == null)
            {
                return HttpNotFound();
            }
            ViewBag.SchoolRefID = new SelectList(db.Schools.AsNoTracking().Select(x => new {x.SchoolID,x.SchoolName }), "SchoolID", "SchoolName", academicYearVM.SchoolRefID);
            return View(academicYearVM);
        }

        // POST: /AcademicYear/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(AcademicYearViewModel academicyear)
        {
            if (ModelState.IsValid)
            {
                if (await _service.SaveAcademicYearAfterEditing(academicyear) == true)
                {
                    return RedirectToAction("Index");
                }
            }
            ViewBag.SchoolRefID = new SelectList(db.Schools.AsNoTracking().Select(x => new { x.SchoolID,x.SchoolName}), "SchoolID", "SchoolName", academicyear.SchoolRefID);
            return View(academicyear);
        }

        

        // POST: /AcademicYear/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> DeleteConfirmed(int id)
        //{
        //    AcademicYearSingle academicyear = await db.AcademicYears.FindAsync(id);
        //    db.AcademicYears.Remove(academicyear);
        //    await db.SaveChangesAsync();
        //    return RedirectToAction("Index");
        //}

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
