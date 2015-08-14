using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OsisModel.Models
{
    public class UserChangeAcademicYearVM
    {
        public int SchoolRefID{ get; set; }

        [Display(Name="School Name")]
        public string SchoolName { get; set; }

        [Required(ErrorMessage = "Please select a Academic Year")]
        [Display(Name = "Current Academic Year")]
        public int AcademicYearID { get; set; }
    }
}