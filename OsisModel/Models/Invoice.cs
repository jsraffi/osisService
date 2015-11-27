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
        public Invoice()
        {
            this.InvoiceDetails = new List<InvoiceDetails>();
        }
        [Key, Column(Order = 0)]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int InvoiceID { get; set; }

        public Decimal Discount { get; set; }

        public Decimal Latefee { get; set; }

        public DateTime InvoiceDate { get; set; }

        [ForeignKey("StudentCurrentYear")]
        public int CurrentYearRefID { get; set; }

        //THis proeprty is used by Entity Framework for navigating to school model and getting school Name
        // for the front end, school name selecting drop down.


        public virtual StudentCurrentYear StudentCurrentYear { get; set; }

        public virtual IList<InvoiceDetails> InvoiceDetails { get; set; }

    }
}
