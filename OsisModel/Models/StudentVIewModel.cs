using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OsisModel.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace OsisModel.Models
{   
    [Table("Students")]
    public class StudentViewModel
    {   
        
        public StudentViewModel()
        {
            DateOfJoining = System.DateTime.Now;
            this.StudentCurrentYear = new List<StudentCurrentYear>();
        }

        [Required(ErrorMessage="Please enter a student name")]
        [Display(Name="Student Name")]
        public string Name { get; set; }

        [Display(Name="Nick Name")]
        public string NickName { get; set; }

        [Required(ErrorMessage = "Please enter a gender")]
        public string Sex { get; set; }
        
        [Required(ErrorMessage="Enter date in dd/mm/yyyy format")]
        public System.DateTime DOB { get; set; }
        
        
        [Required(ErrorMessage = "Please enter address")]
        [Display(Name="Address1")]
        public string Address_Address1 { get; set; }

        [Required(ErrorMessage = "Please enter address")]
        [Display(Name = "Address2")]
        public string Address_Address2 { get; set; }

        [Required(ErrorMessage = "Please enter a city")]
        [Display(Name="City")]
        public string Address_City { get; set; }

        [Required(ErrorMessage = "Please enter a pincode")]
        [Display(Name="Pincode")]
        public string Address_Pincode { get; set; }
        [Display(Name= "Landline")]
        public string Phone { get; set; }
        
        [EmailAddress(ErrorMessage="Emailaddres is not valid")]
        public string Email { get; set; }
        public string website { get; set; }

        [Display(Name = "Father's Name")]
        public string FathersName { get; set; }
        [Display(Name = "Mother's Name")]
        public string MothersName { get; set; }

        [Display(Name = "Father's Occupation")]
        public string FathersOccupation { get; set; }
        [Display(Name = "Mother's Occupation")]
        public string MothersOccupation { get; set; }

        [Display(Name = "Mother's Phone")]
        public string MothersPhone { get; set; }


        [Display(Name = "Father's Phone")]
        public string FathersPhone { get; set; }

        [Display(Name = "Father's Qualification")]
        public string FathersQualification { get; set; }

        [Display(Name="Mothers Qualification")]
        public string MothersQualifiication { get; set; }
        
        [Display(Name = "Mother tongue")]
        public string MotherTongue { get; set; }

        [Display(Name = "Identification marks")]
        public string IdentificationMarks { get; set; }


        [Display(Name = "Known Medical condition")]
        public string KnowMedicalCondition { get; set; }

        [Display(Name = "Special talents")]
        public string SpecialTalents { get; set; }

        [Display(Name = "Reason for choosing us")]
        public string ReasonForOlivekids { get; set; }

        [Display(Name = "Previous play school experience")]
        public string PlayschoolExperience { get; set; }

        [Display(Name = "Center Code")]
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

        [Required(ErrorMessage="Please tick the checkbox")]
        public bool Active { get; set; }
        public virtual IList<StudentCurrentYear> StudentCurrentYear { get; set; }

        
    }
 }
