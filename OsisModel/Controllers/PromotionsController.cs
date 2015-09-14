using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OsisModel.Models;
using Microsoft.AspNet.Identity;
using AutoMapper;

namespace OsisModel.Controllers
{

    public class PromotionsController : Controller
    {
        private OsisContext db = new OsisContext();
        // GET: /Promotions/
        
        public ActionResult PromotionsSelectClass()
        {

            //Get current logged in user need reference to Microsoft.AspNet.Identity
            string userid = User.Identity.GetUserId();

            //Get logged in users school and academic year preference
            var userprefer = db.UserPreferences.AsNoTracking().Where(a => a.UserID == userid).Select(x => new { x.SchoolRefID}).FirstOrDefault();


            ViewBag.ClassRefID = new SelectList(db.SchoolClasses.AsNoTracking().OrderBy(x => x.ClassOrder).Where(sch => sch.SchoolRefID == userprefer.SchoolRefID).Select(x => new { ClassOrder = x.ClassOrder + "" + x.ClassID, x.ClassName }), "ClassOrder", "ClassName");

            return View(new PromotionsSelectVM());
        
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PromotionsSelectClass(PromotionsSelectVM promotions)
        {
            string userid = User.Identity.GetUserId();
            
            //Get logged in users school and academic year preference
            var userprefer = db.UserPreferences.AsNoTracking().Where(a => a.UserID == userid).Select(x => new { x.SchoolRefID }).FirstOrDefault();

            
            if(ModelState.IsValid)
            {
                ViewBag.ClassRefID = new SelectList(db.SchoolClasses.AsNoTracking().Where(sch => sch.SchoolRefID == userprefer.SchoolRefID).Select(x => new { ClassOrder = x.ClassOrder + "" + x.ClassID, x.ClassName }), "ClassOrder", "ClassName");
                
                int classid = getClassID(promotions.ClassFrom);
                var studentlist = db.AjaxStudentLists.AsNoTracking().Where(s => s.SchoolRefID == userprefer.SchoolRefID && s.ClassRefID == classid ).ToList();
                IList<StudentListVM> studentVM = Mapper.Map<IList<StudentListVM>>(studentlist);
                
                PromotionsSelectVM PVM = new PromotionsSelectVM()
                {
                    ClassFrom = promotions.ClassFrom,
                    ClassTo = promotions.ClassTo,
                    StudentLists = studentVM
                };

                return View(PVM);
            }

            //Get current logged in user need reference to Microsoft.AspNet.Identity
            
            //This view bag is use to populate both ClassFrom and classTo dropdown.
            ViewBag.ClassRefID = new SelectList(db.SchoolClasses.AsNoTracking().Where(sch => sch.SchoolRefID == userprefer.SchoolRefID).Select(x => new { ClassOrder = x.ClassOrder + "" + x.ClassID, x.ClassName }), "ClassOrder", "ClassName");
            var studentlisterror = db.AjaxStudentLists.AsNoTracking().Where(s => s.SchoolRefID == 0 && s.ClassRefID == 0).ToList();
            IList<StudentListVM> studenterrorVM = Mapper.Map<IList<StudentListVM>>(studentlisterror);
            PromotionsSelectVM PVMerror = new PromotionsSelectVM()
            {
                ClassFrom = promotions.ClassFrom,
                ClassTo = promotions.ClassTo,
                StudentLists = studenterrorVM
            };            
            return View(PVMerror);
        }

        private int getClassID( int classfrom)
        {   
            string cofValue = Convert.ToString(classfrom);
            int classorderFrom = Convert.ToInt32(cofValue.Substring(1, 1));
            return classorderFrom;
        }
        
        [HttpGet]
        public ActionResult PromotedStudentList()
        {
            PromotionsSelectVM studVM = (PromotionsSelectVM)TempData["studentVM"];
            return View(studVM);
        }
        public ActionResult PromotionStudentListPartial(PromotionsSelectVM model )
        {
            //int classFrom = Convert.ToInt32(Request["PromoVM.ClassFrom"]);
            //int classTo = Convert.ToInt32(Request["ClassTo"]);

            /*
                string userid = User.Identity.GetUserId();

                //Get logged in users school and academic year preference
                var userprefer = db.UserPreferences.AsNoTracking().Where(a => a.UserID == userid).Select(x => new { x.SchoolRefID }).FirstOrDefault();

                var studentlist = db.AjaxStudentLists.AsNoTracking().Where(s => s.SchoolRefID == userprefer.SchoolRefID && s.ClassRefID == model.PromoVM.ClassFrom).ToList();

                PromotionsSelectVM PVM = new PromotionsSelectVM()
                {
                         ClassFrom = model.PromoVM.ClassFrom,
                         ClassTo = model.PromoVM.ClassTo
                };
                PromotionClassStudentListWrapper wrapper = new PromotionClassStudentListWrapper()
                {
                    PromoVM = PVM,
                    StudentLists = studentlist
                };
            */
            string userid = User.Identity.GetUserId();
            var userprefer = db.UserPreferences.AsNoTracking().Where(a => a.UserID == userid).Select(x => new { x.SchoolRefID }).FirstOrDefault();
            int classidFrom = getClassID(model.ClassFrom);
            var studentlist = db.AjaxStudentLists.AsNoTracking().Where(s => s.SchoolRefID == userprefer.SchoolRefID && s.ClassRefID == classidFrom).ToList();
            IList<StudentListVM> studentVM = Mapper.Map<IList<StudentListVM>>(studentlist);
            PromotionsSelectVM modelVM = new PromotionsSelectVM()
            {
                ClassFrom = model.ClassFrom,
                ClassTo=model.ClassTo,
                StudentLists = studentVM
            };
            //Get logged in users school and academic year preference
            

            ViewBag.ClassRefID = new SelectList(db.SchoolClasses.AsNoTracking().Where(sch => sch.SchoolRefID == userprefer.SchoolRefID).Select(x => new { ClassOrder = x.ClassOrder + "" + x.ClassID, x.ClassName }), "ClassOrder", "ClassName");
            ViewBag.Success = "Student Promoted";
            TempData["studentVM"] = modelVM;
            return RedirectToAction("PromotedStudentList");
                

            
        }
        //
        // GET: /Promotions/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /Promotions/Create
        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Promotions/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Promotions/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /Promotions/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Promotions/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /Promotions/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
