using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OsisModel.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OsisModel.Models
{
    public class UserPreferenceViewModel
    {
        [Required(ErrorMessage="Please select a year")]
        [Display(Name="Current Year")]
        public int CurrentYear { get; set; }

        [Required(ErrorMessage = "Please select a School")]
        [Display(Name = "Current School")]
        public int CurrentSchool { get; set; }
    }
}