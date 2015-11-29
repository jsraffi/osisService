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
    [Table("InvoiceDetails")]
    public class InvoiceDetails
    {
        [Key, Column(Order = 0)]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int InvoiceDetailID { get; set; }

        [ForeignKey("Invoices")]
        public int InvoiceRefID { get; set; }
        
        
        public string Description { get; set; }

        public int Quantity { get; set; }

        public Decimal UnitPrice { get; set; }

        public Decimal Amount { get; set; }
    
        //THis proeprty is used by Entity Framework for navigating to school model and getting school Name
        // for the front end, school name selecting drop down.
        public virtual Invoice Invoices { get; set; }
        
    }
}