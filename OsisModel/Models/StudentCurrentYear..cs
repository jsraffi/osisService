using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OsisModel.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OsisModel.Models
{
    [Table("StudentCurrentYears")]
    public class StudentCurrentYear
    {

        [Key, Column(Order = 0)]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int CurrentYearID { get; set; }


        [ForeignKey("Student")]
        public System.Guid StudentRefID { get; set; }

        
        [Required(ErrorMessage="Please select a school")]
        [ForeignKey("School")]
        public int SchoolRefID { get; set; }

        [Required(ErrorMessage = "Please select a Academic Year")]
        [ForeignKey("AcademicYear")]
        public int AcademicYearRefID { get; set; }

        [Required(ErrorMessage="Please select a class")]
        [ForeignKey("SchoolClass")]
        public int ClassRefID { get; set; }

        public bool Active { get; set; }

        public DateTime PromotedOn { get; set; }

        public virtual AcademicYear AcademicYear { get; set; }
        public virtual School School { get; set; }
        public virtual Student Student { get; set; }
        
        public virtual SchoolClass SchoolClass { get; set; } 

       

    }
}