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

    public class InvoiceViewModel
    {
        public InvoiceViewModel()
        {
            this.InvoiceDetailsViewModel = new List<InvoiceDetailsViewModel>();
        }

        [Key]
        public int InvoiceID { get; set; }
         
        [Range(0.00,Double.MaxValue,ErrorMessage ="Please enter a postive numeric value")]       
        public Decimal Discount { get; set; }

        [Range(0.00, Double.MaxValue, ErrorMessage = "Please enter a positive numeric value")]
        [Display(Name ="Late fee fine")]
        public Decimal Latefee { get; set; }

        [DisplayFormat(DataFormatString ="{0:dd/MM/yyyy}")]
        [Required(ErrorMessage ="Please enter a date in dd/mm/yyyy format")]
        [Display(Name ="Invoice Date")]
        public DateTime InvoiceDate { get; set; }

        public int CurrentYearRefID { get; set; }


        public virtual IList<InvoiceDetailsViewModel> InvoiceDetailsViewModel { get; set; }

    }
}
