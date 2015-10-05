using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OsisModel.Models;
using AutoMapper;
using System.Threading.Tasks;
using System.Data.Entity;

namespace OsisModel.Services
{
    public class AcademicYearService:CommonServices,IAcademicYearService
    {   
        private ModelStateDictionary _modelstate;
        private OsisContext db = new OsisContext();
        
        public AcademicYearService(ModelStateDictionary modelstate)
        {
            _modelstate = modelstate;

        }

        public List<AcademicYearSingle> getAcademicYearList()
        {
            return db.AcademicYearSingles.ToList();

        }

        protected bool ValidateAcademicYear(AcademicYearViewModel acamedicyearVM)
        {
            if(CheckStartandEndYear(acamedicyearVM.StartYear,acamedicyearVM.EndYear) == false)
            {
                _modelstate.AddModelError("StartYear", "Difference between start and end year is not one"); 
            }
            if(CheckDisplayYear(acamedicyearVM.DisplayYear)== false)
            {
                _modelstate.AddModelError("DisplayYear", "Display should be in the format of(2015-2016) yyyy-yyyy");
            }
            if (!CheckYearSequence(acamedicyearVM.StartYear, acamedicyearVM.EndYear))
            {
                _modelstate.AddModelError("StartYear", "Start and endyear is not in sequence with previous years"); 
            }
            return _modelstate.IsValid;
        }
        public bool CheckYearSequence(int startyear, int endyear)
        {

            Tuple<int, int> schoolandacademyicyear = getUserCurrentSchool(db);

            int checkfirstyear = db.AcademicYears.Where(sch => sch.SchoolRefID == schoolandacademyicyear.Item1).Count();

            if (checkfirstyear > 0)
            {
                int LastMaxEndYear = db.AcademicYears.Where(sch => sch.SchoolRefID == schoolandacademyicyear.Item1).Select(x => x.EndYear).Max();

                if (startyear == LastMaxEndYear)
                {
                    return true;

                }

            }
            return false;
        }
        private bool CheckStartandEndYear(int startyear, int endyear)
        {
            if(startyear.ToString().Length  == 4 && endyear.ToString().Length  == 4)
            {
                if(startyear == (endyear -1))
                {
                    return true;
                }
            }
            return false;
        }

        
        private bool CheckDisplayYear(string displayyear)
        {
            string[] EndyearandStartyear = displayyear.Split('-');
            int startyear;
            int endyear;
            if(Int32.TryParse(EndyearandStartyear[0], out startyear) && Int32.TryParse(EndyearandStartyear[1], out endyear ))
            {
                return CheckStartandEndYear(startyear, endyear);
            }
            
            return false;
        }
        public async Task<bool> AddNewAcademicYear(AcademicYearViewModel academicyear)
        {
            if (!ValidateAcademicYear(academicyear))
            {
                return false;
            }
            AcademicYear academicYearModel = Mapper.Map<AcademicYear>(academicyear);
            db.AcademicYears.Add(academicYearModel);
            await db.SaveChangesAsync();
            return true;
            
        }

        public async Task<bool> SaveAcademicYearAfterEditing(AcademicYearViewModel academicyear)
        {
            if(!ValidateAcademicYear(academicyear))
            {
                return false;
            }
            AcademicYear academicYearModel = Mapper.Map<AcademicYear>(academicyear);
            db.Entry(academicYearModel).State = EntityState.Modified;
            await db.SaveChangesAsync();
            return true;
        }
        public async Task<AcademicYearViewModel> FindAcademicYearForEditing(int id)
        {

            AcademicYearViewModel academicYearVM = Mapper.Map<AcademicYearViewModel>(await db.AcademicYears.FindAsync(id));
            return academicYearVM;
        }
    }

    public interface IAcademicYearService
    {
        List<AcademicYearSingle> getAcademicYearList();
        Task<bool> AddNewAcademicYear(AcademicYearViewModel academicyear);
        Task<AcademicYearViewModel> FindAcademicYearForEditing(int id);
        Task<bool> SaveAcademicYearAfterEditing(AcademicYearViewModel academicyear);
    }
}