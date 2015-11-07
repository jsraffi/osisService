using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;


namespace OsisModel.Models
{
    
    public class PromotionsSelectVM
    {
        public PromotionsSelectVM()
        {
            this.StudentLists = new List<StudentListVM>();
        }

        [Required(ErrorMessage ="Please enter a class from")]
        public int ClassFrom{ get; set; }

        [Required(ErrorMessage ="Please enter a class to")]
        public int ClassTo{ get; set; }

        public IList<StudentListVM> StudentLists { get; set; }
    }
}