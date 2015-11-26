using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OsisModel.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


//This model where classes are added to school
namespace OsisModel.Models
{
    [Table("Invoices")]
    public class Invoice
    {
        [Key, Column(Order = 0)]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int InvoiceID { get; set; }

        public string ClassName { get; set; }
        
        [ForeignKey("Student")]
        public Guid StudentRefID { get; set; }

        public Decimal Discount { get; set; }

        public Decimal Latefee { get; set; }

        public DateTime InvoiceDate { get; set; }

        [ForeignKey("StudentCurrentYear")]
        public int CurrentYearID { get; set; }
    
        //THis proeprty is used by Entity Framework for navigating to school model and getting school Name
        // for the front end, school name selecting drop down.
        public virtual Student Student { get; set; }

        public virtual StudentCurrentYear StudentCurrentYear { get; set; }
        
    }
}