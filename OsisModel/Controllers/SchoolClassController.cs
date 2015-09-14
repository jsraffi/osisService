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
using PagedList;
using AutoMapper;
using System.Configuration;


namespace OsisModel.Controllers
{
    public class SchoolClassController : Controller
    {
        private OsisContext db = new OsisContext();

        public JsonResult GetClassOrderBySchoolID(int id)
        {
            var classes = db.SchoolClasses.AsNoTracking().Where(s => s.SchoolRefID == id).Select(c => new { ClassName = c.ClassName, ClassID = c.ClassID , ClassOrder= c.ClassOrder}).ToList();

            return Json(classes, JsonRequestBehavior.AllowGet);
        }

        // GET: /SchoolClass/
        public  ActionResult Index(int? page)
        {
            var pageNumber = page ?? 1;
            int pageSize = Convert.ToInt32(ConfigurationManager.AppSettings["pageSize"]);
            var sclass = db.SchoolClassSingles.OrderBy(c => c.ClassName);
            return View(sclass.ToPagedList(pageNumber, pageSize));
           
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
            ViewBag.SchoolRefID = new SelectList(db.Schools.AsNoTracking().Select(x => new {x.SchoolID,x.SchoolName}), "SchoolID", "SchoolName");
            return View();
        }

        // POST: /SchoolClass/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(SchoolClassViewModel schoolclass)
        {
            if (ModelState.IsValid)
            {
                
                SchoolClass scObj = Mapper.Map<SchoolClass>(schoolclass);
                using(var dbtransaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        
                        var LastClassOrderNo = db.Database.SqlQuery<int>("Select ISNULL(Max(ClassOrder),0) from SchoolClasses with (XLOCK,HOLDLOCK) where SchoolRefID={0}", scObj.SchoolRefID).SingleOrDefault<int>();
                        scObj.ClassOrder = LastClassOrderNo + 1;
                        db.SchoolClasses.Add(scObj);
                        await db.SaveChangesAsync();
                        dbtransaction.Commit();
                        return RedirectToAction("Index");
                        
                    }
                    catch(Exception e)
                    {
                        dbtransaction.Rollback();
                        TempData["errormessage"] = e.Message;
                        ViewBag.SchoolRefID = new SelectList(db.Schools.AsNoTracking().Select(x => new { x.SchoolID, x.SchoolName }), "SchoolID", "SchoolName", schoolclass.SchoolRefID);
                        return View(schoolclass);

                    }
                }
                
                
            }

            ViewBag.SchoolRefID = new SelectList(db.Schools.AsNoTracking().Select(x => new { x.SchoolID,x.SchoolName}), "SchoolID", "SchoolName", schoolclass.SchoolRefID);
            return View(schoolclass);
        }

        // GET: /SchoolClass/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SchoolClass schoolclass = await db.SchoolClasses.FindAsync(id);

            SchoolClassViewModel changetoSchoolViewModel = Mapper.Map<SchoolClassViewModel>(schoolclass);
            if (schoolclass == null)
            {
                return HttpNotFound();
            }
            ViewBag.SchoolRefID = new SelectList(db.Schools.AsNoTracking().Select(x => new { x.SchoolID,x.SchoolName}), "SchoolID", "SchoolName", schoolclass.SchoolRefID);
            return View(changetoSchoolViewModel);
        }

        // POST: /SchoolClass/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(SchoolClassViewModel schoolclass)
        {
            if (ModelState.IsValid)
            {

                SchoolClass changetoschoolclass = Mapper.Map<SchoolClass>(schoolclass);
                db.Entry(changetoschoolclass).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.SchoolRefID = new SelectList(db.Schools.AsNoTracking().Select(x => new { x.SchoolID,x.SchoolName}), "SchoolID", "SchoolName", schoolclass.SchoolRefID);
            return View(schoolclass);
        }

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
    }
}
