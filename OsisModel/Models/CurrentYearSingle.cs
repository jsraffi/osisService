using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OsisModel.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OsisModel.Models
{
    [Table("CurrentYearSingle")]
    public class CurrentYearSingle
    {

        [Key, Column(Order = 0)]
        public  Guid StudentID { get; set; }
        public string SchoolName { get; set; }
        public string ClassName { get; set; }
        [Display(Name="Student Name")]
        public string Name { get; set; }

        public int AcademicYearRefID { get; set; }
        public int SchoolRefID { get; set; }
        
    }
}