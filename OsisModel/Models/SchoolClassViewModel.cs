using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OsisModel.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OsisModel.Models
{
    [Table("SchoolClasses")]
    public class SchoolClassViewModel
    {   
        public int ClassID { get; set; }
        
        [Required(ErrorMessage="Please enter a class Name")]
        public string ClassName { get; set; }
        
        [ForeignKey("School")]
        public int SchoolRefID { get; set; }
    
        public virtual School School { get; set; }
        
    }
}