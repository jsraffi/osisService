using System;
using System.Net;
using System.Web.Mvc;
using OsisModel.Models;
using PagedList;
using System.Configuration;
using OsisModel.Services;
using System.Threading.Tasks;

namespace OsisModel.Controllers
{
    public class SchoolController : Controller
    {
        private ISchoolService _service;

        public SchoolController(ISchoolService service)
        {
            _service = service;
        }

        public SchoolController()
        {
            _service = new SchoolService(this.ModelState);
        }
        public ActionResult Index(int? page)
        {
            var pageNumber = page ?? 1;
            int pageSize = Convert.ToInt32(ConfigurationManager.AppSettings["pageSize"]);
            return View(_service.getSchoolList().ToPagedList(pageNumber,pageSize));
        }

        // GET: /School/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            
            SchoolViewModel schoolVM = await Task.Run(() => _service.findSchoolById(id));

            if (schoolVM == null)
            {
                return HttpNotFound();
            }
            return View(schoolVM);
        }

        // GET: /School/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /School/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(SchoolViewModel school)
        {
            if (ModelState.IsValid)
            {
                if (await _service.createNewSchool(school) == true)
                {
                    return RedirectToAction("Index");
                }
            }

            return View(school);
        }

        // GET: /School/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SchoolViewModel schoolVM = await Task.Run(() => _service.findSchoolById(id));
            if (schoolVM == null)
            {
                return HttpNotFound();
            }
            return View(schoolVM);
        }

        // POST: /School/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(SchoolViewModel schoolVM)
        {
            if (ModelState.IsValid)
            {
                if(await _service.saveAfterEdit(schoolVM) == true)
                {
                    return RedirectToAction("Index");
                }
                
            }
            return View(schoolVM);
        }

        // GET: /School/Delete/5
        /*public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            School school = db.Schools.Find(id);
            if (school == null)
            {
                return HttpNotFound();
            }
            return View(school);
        }

        // POST: /School/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            School school = db.Schools.Find(id);
            db.Schools.Remove(school);
            db.SaveChanges();
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
