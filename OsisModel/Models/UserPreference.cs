using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OsisModel.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace OsisModel.Models
{
    [Table("UserPreferences")]
    public class UserPreference
    {
        [Key,DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string UserID { get; set; }
        public string UserName { get; set; }
        public int SchoolRefID{ get; set; }
        public int AcademicYearRefID { get; set; }
    }
}