using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OsisModel.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OsisModel.Models
{
    [Table("AcademicYearSingle")]
    public class AcademicYearSingle
    {

        [Key, Column(Order = 0)]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int AcademicYearID { get; set; }

        [ForeignKey("School")]
        public int SchoolRefID { get; set; }

        public int StartYear { get; set; }

        
        public int EndYear { get; set; }

        
        public DateTime StartDate { get; set; }

        
        public DateTime EndDate { get; set; }

        public int ActiveYear { get; set; }

        public string DisplayYear { get; set; }

        public string SchoolName { get; set; }

        public virtual School School { get; set; }
    }
}