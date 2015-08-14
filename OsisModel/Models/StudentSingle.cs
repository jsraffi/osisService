using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OsisModel.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace OsisModel.Models
{   
    [Table("StudentSingle")]
    public class StudentSingle
    {
        [Key()]
        public System.Guid StudentID { get; set; }
        public int RegistrationNo { get; set; }
        public string Name { get; set; }
        public string FathersName { get; set; }
        public string MothersName { get; set; }
        public string MothersPhone { get; set; }
        public string FathersPhone { get; set; }
        
        [UIHint("ShortDateTime")]
        public DateTime DateOfJoining { get; set; }

        public string ClassName { get; set; }

        public int AcademicYearRefID { get; set; }

        public int SchoolRefID { get; set; }
    }
 }
