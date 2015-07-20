using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace OsisModel.ComplexTypes
{   [ComplexType]
    public class Address
    {
        [Required(ErrorMessage="Please enter address")]
        public string Address1 { get; set; }
        [Required(ErrorMessage = "Please enter address")]
        public string Address2 { get; set; }

        [Required(ErrorMessage = "Please enter city")]
        public string City { get; set; }
        [Required(ErrorMessage = "Please enter pincode")]
        public string Pincode { get; set; }

        [Required(ErrorMessage = "Please enter mobile")]
        public string Mobile { get; set; }
        
        
    }
}