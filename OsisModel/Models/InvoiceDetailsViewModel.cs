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
    
    public class InvoiceDetailsViewModel
    {
        [Key]
        public int InvoiceDetailID { get; set; }
                
        public int InvoiceRefID { get; set; }
        
        public string Description { get; set; }

        public int Quantity { get; set; }

        public Decimal UnitPrice { get; set; }

        public Decimal Amount { get; set; }
        
    }
}