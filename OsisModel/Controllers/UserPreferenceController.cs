using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OsisModel.Models;
using System.Data;
using System.Data.Entity;
using Microsoft.AspNet.Identity;

namespace OsisModel.Controllers
{
    public class UserPreferenceController : Controller
    {
        private OsisContext db = new OsisContext();
        //
        // GET: /UserPreference/

        public ActionResult UserPrefer(string id)
        {
            //This main view gets user from url(id parameter) or uses logged in userid
            string userid =  String.IsNullOrEmpty(id) ? User.Identity.GetUserId() : id;
            int loggedin = String.IsNullOrEmpty(id) ? 1 : 0;           
            
            //get current year for a give userid from userpreference table
            var userpreferencevalues = db.UserPreferences.Where(u => u.UserID == userid).Select(c => new {c.AcademicYearRefID }).FirstOrDefault();

            // The below view bag values or sent as parameters to partial useprefer views                   
            ViewBag.CurrentYear = (int)userpreferencevalues.AcademicYearRefID;// helps find the current acdemic year selected
            ViewBag.Schools = new SelectList(db.Schools.AsNoTracking().Select(x => new {x.SchoolID,x.SchoolName}),"SchoolID","SchoolName"); // helps build schools drop in the ajax form
            ViewBag.Userid = userid;// this value is save in the model, can be used by httppost userprefer view to  set current yeat and current school in userpreference table
            ViewBag.loggedinuser = loggedin; // telling the view where to redirect after job is done either to student table(in case of logged in user) else user management view
            

            //
            return View(new AjaxUserPreference());
        }

        [HttpPost]
        public ActionResult UserPrefer(AjaxUserPreference ajaxuserprefer)
        {
            var schoolid = ajaxuserprefer.School.SchoolID;
            
            foreach (var acyear in ajaxuserprefer.AcademicYear)
            {
                UserPreference UserSchool = new UserPreference();
                UserSchool.AcademicYearRefID = acyear.ActiveYear;
                UserSchool.SchoolRefID = schoolid;
                UserSchool.UserID = ajaxuserprefer.osisuserid;
                db.UserPreferences.Attach(UserSchool);
                db.Entry(UserSchool).Property("CurrentYear").IsModified = true;
                db.Entry(UserSchool).Property("CurrentSchool").IsModified = true;
                break;
            }


            db.SaveChanges();

            
           if(ajaxuserprefer.loggedin == 1)
           {      
                return RedirectToAction("Index", "Student");
           }
           
          return RedirectToAction("Index", "Account");
           

        }    
//        public PartialViewResult UserPreferlist(int CurrentYear,int schoolfilter, string Userid, int loggedinuser) 
//        {
//            var school = db.Schools.Where(s => s.SchoolID == schoolfilter).FirstOrDefault();

//            var academiclist = db.AcademicYears.Where(s => s.SchoolID == schoolfilter);

//            List<AcademicYear> acyear = new List<AcademicYear>();

//            foreach( var alist in academiclist)
//            {
//                AcademicYear aclocal = new AcademicYear();
//                aclocal.School = alist.School;
//                aclocal.AcademicYearID = alist.AcademicYearID;
//                aclocal.ActiveYear = alist.ActiveYear;
//                aclocal.EndDate = alist.EndDate;
//                aclocal.EndYear = alist.EndYear;
//                aclocal.SchoolID = alist.SchoolID;
//                aclocal.StartDate = alist.StartDate;
//                aclocal.StartYear = alist.StartYear;
//                aclocal.YearName = alist.YearName;

//                acyear.Add(aclocal);
//            }
//            School newschool = new School();

//            newschool.SchoolID = school.SchoolID;
//            newschool.SchoolName = school.SchoolName;
//            newschool.Address1 = school.Address1;
//            newschool.Address2 = school.Address2;
//            newschool.City = school.City;
//            newschool.Mobile = school.Mobile;
//            newschool.Phone = school.Phone;
//            newschool.Pincode = school.Pincode;
//            newschool.Selected = school.Selected;
//            newschool.LastRegNo = school.LastRegNo;

//            ViewBag.SelectedYear = CurrentYear;
//            AjaxUserPreference paritaluserprefer = new AjaxUserPreference()
//            {
//                School = newschool,
//                osisuserid = Userid,
//                loggedin =loggedinuser
//            };
//            paritaluserprefer.AcademicYear = acyear;

//            return PartialView("UserPreferlist", paritaluserprefer);
//        }









//        public ActionResult Index()
//        {
//            return View();
//        }

//        //
//        // GET: /UserPreference/Details/5
//        public ActionResult Details(int id)
//        {
//            return View();
//        }

//        //
//        // GET: /UserPreference/Create
//        public ActionResult Create()
//        {
//            return View();
//        }

//        //
//        // POST: /UserPreference/Create
//        [HttpPost]
//        public ActionResult Create(FormCollection collection)
//        {
//            try
//            {
//                // TODO: Add insert logic here

//                return RedirectToAction("Index");
//            }
//            catch
//            {
//                return View();
//            }
//        }

//        //
//        // GET: /UserPreference/Edit/5
//        public ActionResult Edit(int id)
//        {
//            return View();
//        }

//        //
//        // POST: /UserPreference/Edit/5
//        [HttpPost]
//        public ActionResult Edit(int id, FormCollection collection)
//        {
//            try
//            {
//                // TODO: Add update logic here

//                return RedirectToAction("Index");
//            }
//            catch
//            {
//                return View();
//            }
//        }

//        //
//        // GET: /UserPreference/Delete/5
//        public ActionResult Delete(int id)
//        {
//            return View();
//        }

//        //
//        // POST: /UserPreference/Delete/5
//        [HttpPost]
//        public ActionResult Delete(int id, FormCollection collection)
//        {
//            try
//            {
//                // TODO: Add delete logic here

//                return RedirectToAction("Index");
//            }
//            catch
//            {
//                return View();
//            }
//        }
   }
}
