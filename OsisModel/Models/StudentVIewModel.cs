using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OsisModel.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;


namespace OsisModel.Models
{   
    [Table("Students")]
    public class StudentViewModel
    {   
        
        public StudentViewModel()
        {
            
            this.StudentCurrentYear = new List<StudentCurrentYear>();
            this.DateOfJoining = Convert.ToDateTime(DateTime.Now.ToShortDateString());
            
        }
        

        [Required(ErrorMessage="Please enter a student name")]
        public string Name { get; set; }

        public string NickName { get; set; }

        [Required(ErrorMessage = "Please enter a gender")]
        public string Sex { get; set; }
        
        
        [Required(ErrorMessage="Enter date in dd/mm/yyyy format")]
        public DateTime DOB { get; set; }
        
        
        [Required(ErrorMessage = "Please enter address")]
        public string Address_Address1 { get; set; }

        [Required(ErrorMessage = "Please enter address")]
        public string Address_Address2 { get; set; }

        [Required(ErrorMessage = "Please enter a city")]
        public string Address_City { get; set; }

        [Required(ErrorMessage = "Please enter a pincode")]
        public string Address_Pincode { get; set; }
        public string Phone { get; set; }
        
        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage="Please enter a vaild emailid")]
        [EmailAddress(ErrorMessage="Emailaddres is not valid")]
        public string Email { get; set; }
        public string website { get; set; }

        [Required(ErrorMessage="Please enter father's name")]
        public string FathersName { get; set; }
        [Display(Name = "Mother's Name")]
        [Required(ErrorMessage = "Please enter mother's name")]
        public string MothersName { get; set; }

        [Required(ErrorMessage = "Please enter father's occupation")]
        public string FathersOccupation { get; set; }
        [Display(Name = "Mother's Occupation")]

        [Required(ErrorMessage = "Please enter mother's occupation")]
        public string MothersOccupation { get; set; }

        [Required(ErrorMessage = "Please enter mother's mobile no")]
        public string MothersPhone { get; set; }


        
        public string FathersPhone { get; set; }

        [Required(ErrorMessage = "Please enter mother's qualification")]
        public string FathersQualification { get; set; }

        [Required(ErrorMessage = "Please enter mother's qualification")]
        public string MothersQualifiication { get; set; }

        [Required(ErrorMessage = "Please enter mother tongue")]
        public string MotherTongue { get; set; }

        
        public string IdentificationMarks { get; set; }

        public string KnowMedicalCondition { get; set; }

        public string SpecialTalents { get; set; }

        public string ReasonForOlivekids { get; set; }

        public string PlayschoolExperience { get; set; }
        
        
        public string CenterCode { get; set; }

        [Display(Name = "Admission Fees")]
        public decimal AdmissionFee { get; set; }

        [Display(Name = "Total course fees")]
        public string TotalCourseFee { get; set; }

        [Display(Name = "Term Fees")]
        public Nullable<decimal> TermFee { get; set; }
        public Nullable<decimal> Height { get; set; }
        public Nullable<decimal> Weight { get; set; }
        public int RegistrationNo { get; set; }

        [Required(ErrorMessage="Please enter a valid date(dd/mm/yyy)")]
        public DateTime DateOfJoining { get; set; }
        public System.Guid StudentID { get; set; }
        
        [Required(ErrorMessage="Please select a school")]
        [ForeignKey("StudentCurrentYear")]
        public int SchoolRefID { get; set; }
        
        
        [Required(ErrorMessage="Please select an academic year")]
        [ForeignKey("StudentCurrentYear")]
        public int AcademicYearRefID { get; set; }

        [Required(ErrorMessage="Please select a class")]
        [ForeignKey("StudentCurrentYear")]
        public int ClassRefID { get; set; }

        public virtual IList<StudentCurrentYear> StudentCurrentYear { get; set; }

        
    }
 }
