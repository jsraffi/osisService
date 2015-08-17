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
    public class Student
    {
           
        [Key()]   
        public System.Guid StudentID { get; set; }
        public string Address_Address1 { get; set; }
        public string Address_Address2 { get; set; }
        public string Address_City { get; set; }
        public string Address_Pincode { get; set; }
        public string Address_Mobile { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string website { get; set; }
        public string Name { get; set; }
        public string NickName { get; set; }
        public string Sex { get; set; }
        public System.DateTime DOB { get; set; }
        public string FathersName { get; set; }
        public string MothersName { get; set; }
        public string FathersOccupation { get; set; }
        public string MothersOccupation { get; set; }
        public string MothersPhone { get; set; }
        public string FathersPhone { get; set; }
        public string FathersQualification { get; set; }
        public string MothersQualifiication { get; set; }
        public string MotherTongue { get; set; }
        public string IdentificationMarks { get; set; }
        public string KnowMedicalCondition { get; set; }
        public string SpecialTalents { get; set; }
        public string ReasonForOlivekids { get; set; }
        public string PlayschoolExperience { get; set; }
        public string DateOfJoining { get; set; }
        public string CenterCode { get; set; }
        public int RegistrationNo { get; set; }
        public decimal AdmissionFee { get; set; }
        public string TotalCourseFee { get; set; }
        public Nullable<decimal> TermFee { get; set; }
        public Nullable<decimal> Height { get; set; }
        public Nullable<decimal> Weight { get; set; }
    
        public virtual IList<StudentCurrentYear> StudentCurrentYear { get; set; }
    }
 }
