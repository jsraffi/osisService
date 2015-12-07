using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace OsisModel.Models
{
    public class FeeCollectedViewModel
    {
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        [Display(Name="Date from")]
        [Required(ErrorMessage="Please enter a date in dd/mm/yyyy format")]
        public DateTime DateFrom { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        [Display(Name="Date to")]
        [Required(ErrorMessage="Please enter a date in dd/mm/yyyy format")]
        public DateTime DateTo { get; set; }

    }
}