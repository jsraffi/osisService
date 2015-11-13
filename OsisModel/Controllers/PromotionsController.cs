using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OsisModel.Models;
using OsisModel.Services;
using Microsoft.AspNet.Identity;
using AutoMapper;
using System.Threading.Tasks;

namespace OsisModel.Controllers
{

    public class PromotionsController : Controller
    {
        private IPromotionService _service;

        public PromotionsController()
        {
            _service = new PromotionService(this.ModelState);

        }
        public PromotionsController(IPromotionService service)
        {
            _service = service;
        }
        
        
        public ActionResult PromotionsSelectClass()
        {

            //get current school by passing promotion service db context
            Tuple<int,int> currentuserpreference = _service.getUserCurrentSchool(_service.getDBContext());

            //get promotion service db context to be used for dropdownlist
            OsisContext dbc = _service.getDBContext();

            ViewBag.ClassRefID = new SelectList(dbc.SchoolClasses.AsNoTracking().OrderBy(x => x.ClassOrder).Where(sch => sch.SchoolRefID == currentuserpreference.Item1).Select(x => new { ClassOrder = x.ClassOrder + "" + x.ClassID, x.ClassName }), "ClassOrder", "ClassName");

            return View(new PromotionsSelectVM());
        
        }
        
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> PromotionsSelectClass(PromotionsSelectVM promotionsVM)
        {
            Tuple<int, int> currentuserpreference = _service.getUserCurrentSchool(_service.getDBContext());

            OsisContext dbc = _service.getDBContext();
            // store list from db in variable to used commonly when model fails or valid
            var dropdownlist = new SelectList(dbc.SchoolClasses.AsNoTracking().Where(sch => sch.SchoolRefID == currentuserpreference.Item1).Select(x => new { ClassOrder = x.ClassOrder + "" + x.ClassID, x.ClassName }), "ClassOrder", "ClassName");
            
            if(ModelState.IsValid)
            {
                if (await Task.Run(() =>_service.ValidatePromotions(promotionsVM.ClassTo, promotionsVM.ClassFrom)))
                { 
                    ViewBag.ClassRefID = dropdownlist;
                    //passing 1 paramter to getpromotionlist we get list of studentS for userpreference school and academicyear in a class
                     return View(await Task.Run(() => _service.getPromotionList(promotionsVM.ClassFrom,promotionsVM.ClassTo,1,User.Identity.GetUserName())));
                }
            }

            ViewBag.ClassRefID = dropdownlist;
            //passing no parameter value for thrid parameter(ie valid) dafault to zero, hence a empty dataset is 
            //returned(studentlist of PromotionSelectVM)
            return View(await Task.Run(() => _service.getPromotionList(promotionsVM.ClassFrom,promotionsVM.ClassTo,0,User.Identity.GetUserName())));
        }

        [HttpGet]
        public ActionResult PromotedStudentList()
        {
            //casting the object stored in tempdata back to PromotionsSelctVM
            PromotionsSelectVM studVM = (PromotionsSelectVM)TempData["studentVM"];
            return View(studVM);
        }
        
        
        public async Task<ActionResult> PromotionStudentListPartial(PromotionsSelectVM promotionVM )
        {
            //ViewBag.Success = "Student Promoted";
            //TempData["studentVM"] = _service.getPromotionList(model.ClassFrom, model.ClassTo, 1);
            if(await Task.Run(() => _service.promoteStudents(promotionVM,User.Identity.GetUserName()))== false)
            {
               TempData["academicyearMsg"] = "No next academic year from current userprefered academic year";
                //buy passing 0 to getpromotionlist the data returned is empty studentlist in PromotionSelectVM
                // which is stored in Tempdata to be accesed by PromotedStudentList view
                TempData["studentVM"] = _service.getPromotionList(promotionVM.ClassFrom, promotionVM.ClassTo, 0,User.Identity.GetUserName());
            }
            else
            {
                TempData["academicyearMsg"] = "Following students are promoted";
                //buy passing 2 to getpromotionlist the data returned is promoted to next year students in studentlist of PromotionSelectVM 
                // which is stored in Tempdata to be accesed by PromotedStudentList view
                TempData["studentVM"] = _service.getPromotionList(promotionVM.ClassFrom, promotionVM.ClassTo, 2,User.Identity.GetUserName());
            }
            return RedirectToAction("PromotedStudentList");
        }

        
    }
}
