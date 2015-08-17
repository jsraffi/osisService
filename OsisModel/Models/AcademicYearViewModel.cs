using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OsisModel.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OsisModel.Models
{
    
    public class AcademicYearViewModel
    {
        
        public int AcademicYearID { get; set; }

        [Required(ErrorMessage="Please select a school")]
        [Display(Name="School Name")]
        public int SchoolRefID { get; set; }


        [Range(1000,9999,ErrorMessage="Please enter a valid numeric year")]
        [Required(ErrorMessage="Please enter strating year in yyyy format")]
        [Display(Name="Starting Year Only")]
        public int StartYear { get; set; }

        [Range(1000, 9999, ErrorMessage = "Please enter a valid numeric year")]
        [Required(ErrorMessage = "Please enter Ending year in yyyy format")]
        [Display(Name = "End Year only")]
        public int EndYear { get; set; }

        [Required(ErrorMessage = "Please enter starting year in dd/mm/yyyy format")]
        [Display(Name = "Start date in dd/mm/yyyy")]
        public DateTime StartDate { get; set; }

        [UIHint("DateTime")]
        [Required(ErrorMessage = "Please enter ending year in dd/mm/yyyy format")]
        [Display(Name = "End date in dd/mm/yyyy")]
        public DateTime EndDate { get; set; }

        [Display(Name = "Active Year")]
        public int ActiveYear { get; set; }

        [Required(ErrorMessage = "Please enter annual year in yyyy-yyyy, eg:2014-2015")]
        [Display(Name = "Year(yyyy-yyyy)")]
        public string DisplayYear { get; set; }

        public virtual School School { get; set; }
    }
}