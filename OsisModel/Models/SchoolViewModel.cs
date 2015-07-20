using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using OsisModel.Models;
using OsisModel.ComplexTypes;


namespace OsisModel.Models
{
    public class SchoolViewModel
    {

        [Display(Name = "School No")]
        public int SchoolID { get; set; }

        [Required(ErrorMessage = "Please enter a school name")]
        [Display(Name = "School Name")]
        public string SchoolName { get; set; }

        public Address address {get; set;}

        [Display(Name="Land line")]
        public String Phone { get; set; }

        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string Email { get; set; }

        [Display(Name="Websit")]
        public string website { get; set; }

        [Required(ErrorMessage = "Please enter a zero or starting no for students Roll No")]
        [Display(Name = "Start Reg No")]
        public int LastRegNo { get; set; }

        
        [Display(Name = "Current School")]
        public int Selected { get; set; }
    }
}