using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OsisModel.Models;
using OsisModel.Services;
using AutoMapper;
using System.Data.Entity;
using System.Threading.Tasks;



namespace OsisModel.Services
{
    //Service class for promotion controller dervied form Commonservices and implements Ipromotionservice interface
    public class PromotionService:CommonServices,IPromotionService
    {
        //Create Model state variable for validation, use adderror to add errors
        private ModelStateDictionary _modelState;

        //Create dbcontext for one unitofwork for one request
        private OsisContext db = new OsisContext();

        //adding model state to constructor to be called when object creation
        public PromotionService(ModelStateDictionary modelstate)
        {
            _modelState = modelstate;
        }

        //get current school and current academic year from common service base class
        //this method is used get current school and academic year from controller
        public override Tuple<int,int> getUserCurrentSchool(OsisContext db)
        {
            return base.getUserCurrentSchool(db);
        }
        
        //using one dbcontext for unitofwork,this method provides access to dbcontext for dropdownlist creation from controller
        //no need to create dropdownlist's list method in service class and marshall baack to controller
        public OsisContext getDBContext()
        {
            return db;
        }

        //Validating Promotion viewmodel,the classto dropdown and classfrom dropdown are
        //concatinated with order of the class(ie order which the class are define in schoool
        //from lower to higher like LKG to class12.The below method is used to validate
        //promotion is sought from lower to higher class and also checks students are promoted to
        //only to next class in the hierarchy 
        public bool ValidatePromotions(int ClassTo,int ClassFrom)
        {
            if(!CheckClassOrder(ClassTo, ClassFrom))
            {
                _modelState.AddModelError("", "'Class from' should be lesser than 'class to' and the difference should be one level");
            }
            
            return _modelState.IsValid;
        }

        //This method does all checking for validating order of the class lower to higher
        //by spliting value returned by classto and classfrom dropdown and also checking the promotion only for
        // the next class
        private bool CheckClassOrder(int ClassTo, int ClassFrom)
        {
            bool isInOrder = false;
            string classFromValue = Convert.ToString(ClassFrom);
            string classToValue = Convert.ToString(ClassTo);
            int classorderFrom = Convert.ToInt32(classFromValue.Substring(0, 1));
            int classorderTo = Convert.ToInt32(classToValue.Substring(0, 1));
            if (classorderTo > classorderFrom && (classorderTo-classorderFrom == 1))
            {
                isInOrder = true;
            }
            return isInOrder;
        }

        //gets list of students when classfrom and classto parameters are passed from view
        //valid method parameter with default value is used, so that the same method is called when
        //model is valid or has error by changing the value of the valid parameter so that an empty PromotionselectVM is used.
        public PromotionsSelectVM getPromotionList(int classfrom, int classto,int valid=0)
        {
            Tuple<int, int> userprerence = base.getUserCurrentSchool(db);
            //1 validation works for listing students to be promoted(current year academicyearid from userpreference)
            // the view returned is PromotionSelectClass
            if (valid == 1)
            {
                int classid = getClassID(classfrom);
                return getPromotionlistCommon(classfrom, classto,userprerence.Item1,userprerence.Item2, classid);
            }
            else if(valid == 2)
            {   //2 validation works for listing students who got promoted(list's using next year academicyear id)  
                int classid = getClassID(classto);
                return getPromotionlistCommon(classto, 0, userprerence.Item1, getNextYearfromCurrent(userprerence.Item2, userprerence.Item1), classid);
            }
            //not 1 or 2 then an empty PromotionSelectVM is returned(no studenta to promote)
           return getPromotionlistCommon(classfrom, classto, 0, 0,0);
                
        }

        //this method is used to promote students from one class to other
        public async Task<bool> promoteStudents(PromotionsSelectVM model)
        {
            //This mehod is called to check if next academic year from the next exist
            
            if (!CheckNextAcademicYear())
            {
                return false;
                
            }
            //create a transaction for multiple student records to inserted into studentCurrentYear table
            //transaction makes this process atomic
            using (var dbtrans = db.Database.BeginTransaction())
            {
                //using try catch block to rollback transaction in case of error
                try
                {   //iterate through studentlist object of PromotionsSelectVM
                    foreach (var studentlist in model.StudentLists)
                    {
                        // if detained is false then
                        if (studentlist.Detained == false)
                        {
                            //fetch the active academicyear record from database
                            StudentCurrentYear currentacademicyear = db.StudentCurrentYears.Where(scy => scy.StudentRefID == studentlist.StudentRefID && scy.Active == true).SingleOrDefault();
                            //Change the active flag to false
                            currentacademicyear.Active = false;
                            //add record to be updated by entityframework
                            db.StudentCurrentYears.Attach(currentacademicyear);
                            db.Entry(currentacademicyear).State = EntityState.Modified;
                            //Create a new record, inserted for newly promoted class
                            StudentCurrentYear addStudentCurrentYear = new StudentCurrentYear
                            {
                                //get the next academicyear available in the academicyear table
                                //by using method getNextyearfromCurrent by passing students academicyear
                                //and current school id's
                                AcademicYearRefID = getNextYearfromCurrent(studentlist.AcademicYearRefID, studentlist.SchoolRefID),
                                ClassRefID = getClassID(model.ClassTo),
                                SchoolRefID = studentlist.SchoolRefID,
                                StudentRefID = studentlist.StudentRefID,
                                Active = true

                            };
                            db.StudentCurrentYears.Add(addStudentCurrentYear);
                        }
                    }

                    await db.SaveChangesAsync();
                    dbtrans.Commit();
                    return true;
                }
                catch (Exception e)
                {
                    dbtrans.Rollback();
                    return false;
                }
            

            }
        }

        private bool CheckNextAcademicYear()
        {
            Tuple<int, int> userpreferSchoolandAcademicYear = getUserCurrentSchool(db);

            var currrentacademicyear = db.AcademicYears.Where(ac => ac.AcademicYearID == userpreferSchoolandAcademicYear.Item2).Select(x => new { x.StartYear, x.EndYear }).SingleOrDefault();

            int nextacademicyear = db.AcademicYears.Where(ay => ay.StartYear == currrentacademicyear.EndYear  && ay.SchoolRefID == userpreferSchoolandAcademicYear.Item1).Count();

              
            if (nextacademicyear > 0)
            {
                return true;
            }
            return false;
        }
        private int getNextYearfromCurrent(int currentacademicyearid, int schoolid)
        {
            
            var currentendyear = db.AcademicYears.Where(ay => ay.AcademicYearID == currentacademicyearid).Select(x => new {x.EndYear}).SingleOrDefault();

            var nextacademicyear = db.AcademicYears.Where(ay => ay.StartYear == currentendyear.EndYear && ay.SchoolRefID == schoolid).Select(x => new { x.AcademicYearID }).SingleOrDefault();

            return nextacademicyear.AcademicYearID;
        }

        //by changing the value schoolredid and classid, we can get different datasets
        private PromotionsSelectVM getPromotionlistCommon(int classfrom,int classto,int schoolrefid,int academicyearid,int classid)
        {
            var studentlist = db.AjaxStudentLists.AsNoTracking().Where(s => s.SchoolRefID == schoolrefid && s.ClassRefID == classid && s.AcademicYearRefID == academicyearid && s.Active == true).ToList();
            IList<StudentListVM> studentVM = Mapper.Map<IList<StudentListVM>>(studentlist);
            PromotionsSelectVM PVM = new PromotionsSelectVM()
            {
                ClassFrom = classfrom,
                ClassTo = classto,
                StudentLists = studentVM
            };
            return PVM;
        }
        
        //this method is used to split the classfrom value from view dropdown, as this value is
        //concatination of classorder and claassid, and classid is returned
        private int getClassID(int classorder)
        {
            string coValue = Convert.ToString(classorder);
            int classid = Convert.ToInt32(coValue.Substring(1, 1));
            return classid;
        }

    }

    public interface IPromotionService
    {
       Tuple<int,int> getUserCurrentSchool(OsisContext db);
       OsisContext getDBContext();
       PromotionsSelectVM getPromotionList(int classfrom,int classto, int valid=0);
       Task<bool> promoteStudents(PromotionsSelectVM model);
       bool ValidatePromotions(int ClassTo, int ClassFrom);

    }
}