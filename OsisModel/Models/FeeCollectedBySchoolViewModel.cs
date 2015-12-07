using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace OsisModel.Models
{
    public class FeeCollectedBySchoolViewModel
    {

        [Display(Name="School Name")]
        [Required(ErrorMessage="Please select a School")]
        public int SchoolID { get; set; }
        
        [Display(Name="Date from")]
        [Required(ErrorMessage="Please enter a date in dd/mm/yyyy format")]
        public DateTime DateFrom { get; set; }
        
        [Display(Name="Date to")]
        [Required(ErrorMessage="Please enter a date in dd/mm/yyyy format")]
        public DateTime DateTo { get; set; }

    }
}