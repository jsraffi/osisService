using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.Common;
using System.Diagnostics;
using OsisModel.ComplexTypes;
using OsisModel.Models;


namespace OsisModel.Models
{

    public class OsisContext :DbContext
    {

        public OsisContext()
            : base("oModelconnection")
        {
            
        }

        public DbSet<InvoiceSingle> InvoiceSingle { get; set; }
        public DbSet<StudentList> AjaxStudentLists { get; set; }
        public DbSet<StudentSingle> StudentSingles {get; set;}
        public DbSet<School> Schools { get; set; }
        public DbSet<AcademicYearSingle> AcademicYearSingles { get; set; }
        public DbSet<AcademicYear> AcademicYears { get; set; }
        public DbSet<SchoolClass> SchoolClasses { get; set; }
        public DbSet<SchoolClassSingle> SchoolClassSingles { get; set; }

        public DbSet<StudentCurrentYear> StudentCurrentYears { get; set; }

        public DbSet<CurrentYearSingle> CurrentYearSingles { get; set; }
        public DbSet<UserPreference> UserPreferences { get; set; }

        public DbSet<Student> Students { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }

       

        
    }
}