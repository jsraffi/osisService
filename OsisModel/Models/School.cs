using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using OsisModel.ComplexTypes;

namespace OsisModel.Models
{
    [Table("Schools")]
    public class School
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int SchoolID { get; set; }
       
        public string SchoolName { get; set; }

        public Address address { get; set; }
        
        public string Phone { get; set; }

        public string Email { get; set; }

        public string website { get; set; }
        
        public int LastRegNo { get; set; }

        public int Selected { get; set; }
    
    }
}