using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OsisModel.Models
{
    public class AjaxUserPreference
    {
        public AjaxUserPreference() 
        {
            this. AcademicYear = new List< AcademicYear>();
        }
        public School School { get; set; }

        public string osisuserid { get; set; }

        public int loggedin { get; set; }

        public virtual IList<AcademicYear> AcademicYear {get; set;}
    }
}