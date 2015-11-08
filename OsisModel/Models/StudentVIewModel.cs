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
        
        [Display(Name ="Student's Name")]
        [Required(ErrorMessage="Please enter a student name")]
        public string Name { get; set; }

        [Display(Name = "Nick Name")]
        public string NickName { get; set; }

        [Display(Name = "Gender")]
        [Required(ErrorMessage = "Please enter a gender")]
        public string Sex { get; set; }

        [Display(Name = "Date of birth")]
        [Required(ErrorMessage="Enter date in dd/mm/yyyy format")]
        public DateTime DOB { get; set; }

        [Display(Name = "Address 1")]
        [Required(ErrorMessage = "Please enter address")]
        public string Address_Address1 { get; set; }

        [Display(Name = "Address 2")]
        [Required(ErrorMessage = "Please enter address")]
        public string Address_Address2 { get; set; }

        [Display(Name = "City")]
        [Required(ErrorMessage = "Please enter a city")]
        public string Address_City { get; set; }

        [Display(Name = "Pincode")]
        [Required(ErrorMessage = "Please enter a pincode")]
        public string Address_Pincode { get; set; }

        [Display(Name = "Land Line No")]
        public string Phone { get; set; }


        [Display(Name = "Email address")]
        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage="Please enter a vaild emailid")]
        [EmailAddress(ErrorMessage="Emailaddres is not valid")]
        public string Email { get; set; }

        [Display(Name = "Web site address")]
        public string website { get; set; }

        [Display(Name = "Father's Name")]
        [Required(ErrorMessage="Please enter father's name")]
        public string FathersName { get; set; }

        [Display(Name = "Mother's Name")]
        [Required(ErrorMessage = "Please enter mother's name")]
        public string MothersName { get; set; }

        [Display(Name = "Father's Occupation")]
        [Required(ErrorMessage = "Please enter father's occupation")]
        public string FathersOccupation { get; set; }

        [Display(Name = "Mother's Occupation")]
        [Required(ErrorMessage = "Please enter mother's occupation")]
        public string MothersOccupation { get; set; }

        [Display(Name = "Mother's mobile Phone")]
        [Required(ErrorMessage = "Please enter mother's mobile no")]
        public string MothersPhone { get; set; }


        [Display(Name = "Father's mobile Phone")]
        public string FathersPhone { get; set; }

        [Display(Name = "Father's qualification")]
        [Required(ErrorMessage = "Please enter mother's qualification")]
        public string FathersQualification { get; set; }


        [Display(Name = "Mother's qualification")]
        [Required(ErrorMessage = "Please enter mother's qualification")]
        public string MothersQualifiication { get; set; }


        [Display(Name = "Mother tongue")]
        [Required(ErrorMessage = "Please enter mother tongue")]
        public string MotherTongue { get; set; }

        [Display(Name = "Identification marks")]
        public string IdentificationMarks { get; set; }

        [Display(Name = "Known medical condition")]
        public string KnowMedicalCondition { get; set; }

        [Display(Name = "Special talents")]
        public string SpecialTalents { get; set; }

        [Display(Name = "Reason for choosing olive kids")]
        public string ReasonForOlivekids { get; set; }

        [Display(Name = "Play school experience")]
        public string PlayschoolExperience { get; set; }


        [Display(Name = "Center code")]
        public string CenterCode { get; set; }

        [Display(Name = "Admission Fees")]
        public decimal AdmissionFee { get; set; }

        [Display(Name = "Total course fees")]
        public string TotalCourseFee { get; set; }

        [Display(Name = "Term Fees")]
        public Nullable<decimal> TermFee { get; set; }
        public Nullable<decimal> Height { get; set; }
        public Nullable<decimal> Weight { get; set; }

        [Display(Name = "Registration No")]
        public int RegistrationNo { get; set; }

        [Display(Name = "Date of joining")]
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
