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

namespace OsisModel.Controllers
{
    public class AcademicYearController : Controller
    {
        private OsisContext db = new OsisContext();
        

        // GET: /AcademicYear/
        public ActionResult Index(int? page)
        {
            
            var pageNumber = page ?? 1;
            int pageSize = Convert.ToInt32(ConfigurationManager.AppSettings["pageSize"]);
            var ayear = db.AcademicYearSingles.SqlQuery("Select AcademicYearID,StartYear,EndYear,StartDate,EndDate,DisplayYear,SchoolREfID,ActiveYear, SchoolName from AcademicYears as ac inner join Schools as sc on sc.SchoolID = ac.SchoolRefID");                                              
            var aclist = ayear.ToList();
            return View(ayear.ToPagedList(pageNumber, pageSize));
            
        }

        // GET: /AcademicYear/Details/5
        //public async Task<ActionResult> Details(int? id)
        //{
        //    //if (id == null)
        //    //{
        //    //    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    //}
        //    ////AcademicYearSingle academicyear = await db.AcademicYears.FindAsync(id);
        //    //if (academicyear == null)
        //    //{
        //    //    return HttpNotFound();
        //    //}
        //    //return View(academicyear);
        //}

        // GET: /AcademicYear/Create
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

                AcademicYear ayObj = Mapper.Map<AcademicYear>(academicyear);
                db.AcademicYears.Add(ayObj);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
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
            AcademicYear academicyear = await db.AcademicYears.FindAsync(id);

            AcademicYearViewModel ayObj = Mapper.Map<AcademicYearViewModel>(academicyear);
            if (academicyear == null)
            {
                return HttpNotFound();
            }
            ViewBag.SchoolRefID = new SelectList(db.Schools.AsNoTracking().Select(x => new {x.SchoolID,x.SchoolName }), "SchoolID", "SchoolName", academicyear.SchoolRefID);
            return View(ayObj);
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
                AcademicYear ayObj = Mapper.Map<AcademicYear>(academicyear);
                db.Entry(ayObj).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.SchoolRefID = new SelectList(db.Schools.AsNoTracking().Select(x => new { x.SchoolID,x.SchoolName}), "SchoolID", "SchoolName", academicyear.SchoolRefID);
            return View(academicyear);
        }

        // GET: /AcademicYear/Delete/5
        //public async Task<ActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    AcademicYearSingle academicyear = await db.AcademicYears.FindAsync(id);
        //    if (academicyear == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(academicyear);
        //}

        // POST: /AcademicYear/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
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
