using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using OsisModel.Models;


namespace OsisModel.Services
{
    public abstract class CommonServices:IDisposable
    {
        
        //allow inheriting class to get current school when dbcontext is class passed
        public virtual Tuple<int,int> getUserCurrentSchool(OsisContext db)
        {
            string username = getCurrentUserName();
            //Get logged in users school and academic year preference
            var userprefer = db.UserPreferences.AsNoTracking().Where(a => a.UserName == username).Select(x => new { x.SchoolRefID ,x.AcademicYearRefID}).FirstOrDefault();

            Tuple<int, int> loggedinuserpreference = new Tuple<int, int>(userprefer.SchoolRefID, userprefer.AcademicYearRefID);

            return loggedinuserpreference;
        }

        //returns the current name of logged in user
        private string getCurrentUserName()
        {
            return HttpContext.Current.User.Identity.Name;
        }


        public void Dispose()
        {
            Dispose(true /* disposing */);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
        }
    }

    public interface ICommonService
    {
        int getUserCurrentSchool(OsisContext db);
        

    }
}