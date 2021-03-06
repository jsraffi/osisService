﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using OsisModel.Models;
using PagedList;
using AutoMapper;
using System.Configuration;
using OsisModel.Services;

namespace OsisModel.Controllers
{
    public class SchoolClassController : Controller
    {
        private OsisContext db = new OsisContext();

        private ISchoolClass _service;

        public SchoolClassController()
        {
            _service = new SchoolClassService(this.ModelState);
        }

        public SchoolClassController(ISchoolClass service)
        {
            _service = service;
        }
        
        public JsonResult GetClassOrderBySchoolID(int id)
        {
            var dbc = _service.getDBContext();
            var classes = dbc.SchoolClasses.AsNoTracking().Where(s => s.SchoolRefID == id).Select(c => new { ClassName = c.ClassName, ClassID = c.ClassID , ClassOrder= c.ClassOrder}).ToList();
            return Json(classes, JsonRequestBehavior.AllowGet);
        }

        // GET: /SchoolClass/
        public  ActionResult Index(int? page)
        {
            var pageNumber = page ?? 1;
            int pageSize = Convert.ToInt32(ConfigurationManager.AppSettings["pageSize"]);
            return View(_service.getListofClassesofSchool().ToPagedList(pageNumber, pageSize));
           
        }

        // GET: /SchoolClass/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SchoolClass schoolclass = await db.SchoolClasses.FindAsync(id);
            if (schoolclass == null)
            {
                return HttpNotFound();
            }
            return View(schoolclass);
        }

        // GET: /SchoolClass/Create
        public ActionResult Create()
        {
            var dbc = _service.getDBContext();
            ViewBag.SchoolRefID = new SelectList(dbc.Schools.AsNoTracking().Select(x => new {x.SchoolID,x.SchoolName}), "SchoolID", "SchoolName");
            return View();
        }

        // POST: /SchoolClass/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(SchoolClassViewModel schoolVM)
        {
            
            if (ModelState.IsValid)
            {
                if(await Task.Run(() => _service.addNewClasstoSchool(schoolVM)) == true)
                {
                    return RedirectToAction("Index");
                }
                
            }
            var dbc = _service.getDBContext();
            ViewBag.SchoolRefID = new SelectList(dbc.Schools.AsNoTracking().Select(x => new { x.SchoolID,x.SchoolName}), "SchoolID", "SchoolName", schoolVM.SchoolRefID);
            return View(schoolVM);
        }

        // GET: /SchoolClass/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SchoolClassViewModel SchoolClassVM = await Task.Run(() => _service.findClassByID(id));
            if (SchoolClassVM == null)
            {
                return HttpNotFound();
            }
            var dbc = _service.getDBContext();
            ViewBag.SchoolRefID = new SelectList(dbc.Schools.AsNoTracking().Select(x => new { x.SchoolID,x.SchoolName}), "SchoolID", "SchoolName", SchoolClassVM.SchoolRefID);
            return View(SchoolClassVM);
        }

        // POST: /SchoolClass/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(SchoolClassViewModel SchoolClassVM)
        {
            if (ModelState.IsValid)
            {
                if (await Task.Run(() => _service.saveClassAfterEditing(SchoolClassVM)) == true)
                {
                    return RedirectToAction("Index");
                }
            }
            var dbc = _service.getDBContext();
            ViewBag.SchoolRefID = new SelectList(dbc.Schools.AsNoTracking().Select(x => new { x.SchoolID,x.SchoolName}), "SchoolID", "SchoolName", SchoolClassVM.SchoolRefID);
            return View(SchoolClassVM);
        }
        /*
                // GET: /SchoolClass/Delete/5
                public async Task<ActionResult> Delete(int? id)
                {
                    if (id == null)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }
                    SchoolClass schoolclass = await db.SchoolClasses.FindAsync(id);
                    if (schoolclass == null)
                    {
                        return HttpNotFound();
                    }
                    return View(schoolclass);
                }

                // POST: /SchoolClass/Delete/5
                [HttpPost, ActionName("Delete")]
                [ValidateAntiForgeryToken]
                public async Task<ActionResult> DeleteConfirmed(int id)
                {
                    SchoolClass schoolclass = await db.SchoolClasses.FindAsync(id);
                    db.SchoolClasses.Remove(schoolclass);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }

                protected override void Dispose(bool disposing)
                {
                    if (disposing)
                    {
                        db.Dispose();
                    }
                    base.Dispose(disposing);
                }
                */
    }
}
