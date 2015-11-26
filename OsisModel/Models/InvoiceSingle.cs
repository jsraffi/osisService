using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OsisModel.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OsisModel.Models
{
    [Table("InvoiceSingle")]
    public class InvoiceSingle
    {
        [Key, Column(Order = 0)]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int InvoiceID { get; set; }

        public DateTime InvoiceDate { get; set; }
               
        public Decimal TotalAmount { get; set; }

        [Display(Name="Student Name")]
        public string Name{ get; set; }

        [Display(Name="Parents Name")]
        public String MothersName { get; set; }

        public int SchoolRefID { get; set; }

        public int AcademicYearRefID { get; set; }
        
    }
}