using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace OsisModel.Models
{

    [Table("ShowApplicationForm")]
    public class ShowApplicationForm
    {   
        [Key]
        public Guid StudentID { get; set; }

        public string Name { get; set; }

        public DateTime DateOfJoining { get; set; }

        public string SchoolName { get; set; }

        public string ClassName { get; set; }

        public int SchoolRefID { get; set; }

        public int AcademicYearRefID { get; set; }

        public string DisplayYear { get; set; }
    }
}