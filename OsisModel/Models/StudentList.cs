using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;


namespace OsisModel.Models
{
    [Table("AjaxStudentList")]
    public class StudentList
    {
        [Key, Column(Order = 0)]
        public Guid StudentRefID { get; set; }

        public string Name { get; set; }

        public string ClassName { get; set; }

        public int ClassRefID { get; set; }

        public int SchoolRefID { get; set; }

        public int AcademicYearRefID { get; set; }

        public bool Active { get; set; }

        public DateTime PromotedOn { get; set; }
    }
}