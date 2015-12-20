using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OsisModel.Models;
using System.Web.Mvc;
using System.Configuration;
using PagedList;
using AutoMapper;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using CrystalDecisions.ReportSource;
using CrystalDecisions.ReportAppServer;
using CrystalDecisions.ReportAppServer.DataDefModel;
using CrystalDecisions.ReportAppServer.ClientDoc;
using System.IO;
using System.Data.Entity;

namespace OsisModel.Services
{
    public class InvoiceService:CommonServices,IInvoiceService
    {
        //dbcontext is created
        private OsisContext db = new OsisContext();


        private ModelStateDictionary _modelstate;

        public ModelStateDictionary ModelState
        {
            get
            {
                return this._modelstate;
            }

            set
            {
                this._modelstate = value;
            }
        }


        //modelstate is added to the constructor
        /*
        public InvoiceService(ModelStateDictionary modelstate)
        {
            _modelstate = modelstate;
        }
        */
        /// <summary>
        /// This method is called by methods requesting for dbcontext
        /// one db context for whole request cycle.
        /// </summary>
        /// <returns>returns db context</returns>
        public OsisContext getDBContext()
        {
            return db;
        }
        /// <summary>
        /// Get the list of invoices for school for current selected school and academicyear
        /// </summary>
        /// <param name="page">querystring parameter for paging used by paged list</param>
        /// <param name="user">Identity module user(logged on user) is required becasuse getInvoiceList is called on a sperated thread(TPL)</param>
        /// <returns>retunrs InvoicesSingle database view as IPagedList</returns>

        public PagedList.IPagedList<InvoiceSingle> getInvoiceList(int? page, string user = null)
        {
            Tuple<int, int> currentschoolandacademicyear = getUserCurrentSchool(db, user);
            int pageSize = Convert.ToInt32(ConfigurationManager.AppSettings["pageSize"]);
            var pageNumber = page ?? 1;
            var invoices = db.InvoiceSingle.AsNoTracking().OrderBy(d => d.InvoiceDate).Where(sa => sa.SchoolRefID == currentschoolandacademicyear.Item1 && sa.AcademicYearRefID == currentschoolandacademicyear.Item2);

            return invoices.ToPagedList(pageNumber, pageSize);
        }

        /// <summary>
        /// List of students records for the year of admission from admission form cab be printed
        /// </summary>
        /// <param name="page">querystring parameter for paging used by paged list</param>
        /// <param name="user">Identity module user(logged on user) is required becasuse getApplicationList is called on a sperated thread(TPL)</param>
        /// <returns>ShowApplicationForm Model</returns>
        public PagedList.IPagedList<ShowApplicationForm> getApplicationList(int? page, string user = null)
        {
            Tuple<int, int> currentschoolandacademicyear = getUserCurrentSchool(db, user);
            int pageSize = Convert.ToInt32(ConfigurationManager.AppSettings["pageSize"]);
            var pageNumber = page ?? 1;

            var appplicationlist = db.ShowApplicationForms.AsNoTracking().OrderBy(d => d.DateOfJoining).Where(sa => sa.SchoolRefID == currentschoolandacademicyear.Item1 && sa.AcademicYearRefID == currentschoolandacademicyear.Item2);

            return appplicationlist.ToPagedList(pageNumber, pageSize);
        }

        /// <summary>
        /// Show a view for creating invoice,with three details grid
        /// </summary>
        /// <param name="id">Student id for whom invoice is being created</param>
        /// <returns>invoice view model</returns>
        public InvoiceViewModel createInvoice(Guid id)
        {
            //get student current class and year details
            StudentCurrentYear studentcurrentyear = db.StudentCurrentYears.Where(sid => sid.StudentRefID == id && sid.Active == true).SingleOrDefault();

            //three empty invoice details are created
            List<InvoiceDetails> InvDetails = new List<InvoiceDetails>()
            {
                new InvoiceDetails()
                {
                    Description="",
                    Quantity =0,
                    UnitPrice=0,
                    Amount=0
                },
                new InvoiceDetails()
                {
                    Description="",
                    Quantity =0,
                    UnitPrice=0,
                    Amount=0
                },
                new InvoiceDetails()
                {
                    Description="",
                    Quantity =0,
                    UnitPrice=0,
                    Amount=0
                }
            };

            var invoice = new Invoice()
            {
                CurrentYearRefID = studentcurrentyear.CurrentYearID,
                Discount =0,
                Latefee =0,
                InvoiceDate =DateTime.Now,
                InvoiceDetails = InvDetails
            };
            InvoiceViewModel invoiceVM = Mapper.Map<InvoiceViewModel>(invoice);
            return invoiceVM;
            

        }

        /// <summary>
        /// validates invoice details data when description field is not null,also validates no invoice detail data
        /// </summary>
        /// <param name="invoiceVM">InvoiceviewModel</param>
        /// <returns>boolean true on success or false on validation failure</returns>
        public bool validateInvoice(InvoiceViewModel invoiceVM)
        {
            Boolean iserror = true;
            int Count = 0;
            for(var i=0;i < invoiceVM.InvoiceDetailsViewModel.Count;i++)
            {
                string desc = (invoiceVM.InvoiceDetailsViewModel[i].Description != null) ? invoiceVM.InvoiceDetailsViewModel[i].Description.Trim() : "";

                //if description is not null checks all other details have valid values
                if (desc != "" )
                {
                    if(invoiceVM.InvoiceDetailsViewModel[i].Quantity <1)
                    {
                        _modelstate.AddModelError("InvoiceDetailsViewModel[" + i + "].Quantity", "less than one");
                        iserror = false;
                    }
                    if(invoiceVM.InvoiceDetailsViewModel[i].UnitPrice <1)
                    {
                        _modelstate.AddModelError("InvoiceDetailsViewModel[" + i + "].UnitPrice", "less than one");
                        iserror = false;

                    }
                    if(invoiceVM.InvoiceDetailsViewModel[i].Amount <1)
                    {
                        _modelstate.AddModelError("InvoiceDetailsViewModel[" + i + "].Amount", "less than one");
                        iserror = false;

                    }
                    //checks if atleast one record is added to invoice details, return false if data is empty
                    
                    Count = Count + 1;
                    
                }
            }
            if (Count == 0)
            {
                iserror = false;
                _modelstate.AddModelError("", "No Records to add");
            }


            return iserror;
        }

        /// <summary>
        /// saves invoice data after validation checks
        /// </summary>
        /// <param name="invoiceVM">InvoiceViewModel</param>
        /// <returns>boolean on save success</returns>
        public bool saveInvoice(InvoiceViewModel invoiceVM)
        {
            if (!validateInvoice(invoiceVM))
            {
                return false;
            }
            try
            {
                //remove empty invoice details record in invoice in reverse order
                //if you remove randomly, collection get changes, and error is thrown
                for (var i = invoiceVM.InvoiceDetailsViewModel.Count - 1; i >= 0; i--)
                {
                    string desc = (invoiceVM.InvoiceDetailsViewModel[i].Description != null) ? invoiceVM.InvoiceDetailsViewModel[i].Description.Trim() : "";

                    if (desc == "")
                    {
                        invoiceVM.InvoiceDetailsViewModel.RemoveAt(i);
                    }
                }
                Invoice addInvoice = Mapper.Map<Invoice>(invoiceVM);
                db.Invoices.Add(addInvoice);
                db.SaveChanges();

                return true;

            }
            catch(Exception e)
            {
                _modelstate.AddModelError("", e.Message);
                return false;
            }
        }

        /// <summary>
        /// generic report method, setting common parameters to all reports reading configuration from web.config
        /// </summary>
        /// <param name="report">Crystal report to be printed</param>
        /// <param name="httpctx">Current httpcontext passed from report creation method</param>
        /// <returns>returns pdf stream</returns>
        private Stream genericReportSetting(ReportDocument report, HttpContext httpctx)
        {
            PropertyBag connectionAttributes = new PropertyBag();
            connectionAttributes.Add("Auto Translate", "-1");
            connectionAttributes.Add("Connect Timeout", "15");
            connectionAttributes.Add("Data Source", ConfigurationManager.AppSettings["Server"]);
            connectionAttributes.Add("General Timeout", "0");
            connectionAttributes.Add("Initial Catalog", ConfigurationManager.AppSettings["Database"]);
            connectionAttributes.Add("Integrated Security", false);
            connectionAttributes.Add("Locale Identifier", "1040");
            connectionAttributes.Add("OLE DB Services", "-5");
            connectionAttributes.Add("Provider", "SQLOLEDB");
            connectionAttributes.Add("Tag with column collation when possible", "0");
            connectionAttributes.Add("Use DSN Default Properties", false);
            connectionAttributes.Add("Use Encryption for Data", "0");

            PropertyBag attributes = new PropertyBag();
            attributes.Add("Database DLL", "crdb_ado.dll");
            attributes.Add("QE_DatabaseName", ConfigurationManager.AppSettings["Database"]);
            attributes.Add("QE_DatabaseType", "OLE DB (ADO)");
            attributes.Add("QE_LogonProperties", connectionAttributes);
            attributes.Add("QE_ServerDescription", httpctx.Server);
            attributes.Add("QESQLDB", true);
            attributes.Add("SSO Enabled", false);

            CrystalDecisions.ReportAppServer.DataDefModel.ConnectionInfo ci = new CrystalDecisions.ReportAppServer.DataDefModel.ConnectionInfo();
            ci.Attributes = attributes;
            ci.Kind = CrConnectionInfoKindEnum.crConnectionInfoKindCRQE;
            ci.UserName = ConfigurationManager.AppSettings["UserID"];
            ci.Password = ConfigurationManager.AppSettings["Password"];

            foreach (CrystalDecisions.ReportAppServer.DataDefModel.Table table in report.ReportClientDocument.DatabaseController.Database.Tables)
            {
                CrystalDecisions.ReportAppServer.DataDefModel.Procedure newTable = new CrystalDecisions.ReportAppServer.DataDefModel.Procedure();

                newTable.ConnectionInfo = ci;
                newTable.Name = table.Name;
                newTable.Alias = table.Alias;
                newTable.QualifiedName = ConfigurationManager.AppSettings["Database"] + ".dbo." + table.Name;
                report.ReportClientDocument.DatabaseController.SetTableLocation(table, newTable);
            }


            Stream stream = report.ExportToStream(ExportFormatType.PortableDocFormat);
            report.Dispose();
            return stream;
        }

        /// <summary>
        /// method outputs Invoice report, by calling generic report setting
        /// </summary>
        /// <param name="id">invoice id of the invoice to be printed</param>
        /// <param name="httpctx">current httpcontext is sent to method, as generareInvoiceReport is called asynchronously using TPL</param>
        /// <returns>pdf stream</returns>
        public Stream generateInVoiceReport(string id,HttpContext httpctx)
        {
            ReportDocument feereport = new ReportDocument();
            
            feereport.Load(httpctx.Server.MapPath("~/Reports/Invoice.rpt"));
            feereport.PrintOptions.PaperSize = PaperSize.PaperA4;
            feereport.SetParameterValue("InvoiceID", id);

            return genericReportSetting(feereport, httpctx);
        }
        /// <summary>
        /// generated total fee collected report between dates
        /// </summary>
        /// <param name="httpctx">current httpcontext is sent generareInvoiceReport is called asynchronously using TPL</param>
        /// <param name="FromDate">date from set to report</param>
        /// <param name="ToDate">to date set to report</param>
        /// <returns>pdf stream</returns>
        public Stream generateTotalFeeReport(HttpContext httpctx,DateTime FromDate,DateTime ToDate)
        {
            ReportClass totalfeereport = new ReportClass();
            totalfeereport.FileName = httpctx.Server.MapPath("~/Reports/TotalFeescollected.rpt");
            totalfeereport.Load();
            totalfeereport.PrintOptions.PaperSize = PaperSize.PaperA4;
            totalfeereport.SetParameterValue("InvoicefrmDate", FromDate);
            totalfeereport.SetParameterValue("Invoicetodate", ToDate);

            return genericReportSetting(totalfeereport, httpctx);
        }

        /// <summary>
        /// Total fee collected by school
        /// </summary>
        /// <param name="httpctx">current httpcontext is sent to method generareInvoiceReport is called asynchronously using TPL</param>
        /// <param name="FromDate">form date set on report</param>
        /// <param name="ToDate">to date set on report</param>
        /// <param name="schoolid">school id parameter set on report</param>
        /// <returns></returns>
        public Stream generateTotalFeeReportBySchool(HttpContext httpctx, DateTime FromDate, DateTime ToDate, int schoolid)
        {
            ReportClass feereportbyschool = new ReportClass();
            feereportbyschool.FileName = httpctx.Server.MapPath("~/Reports/TotalFeescollectedBySchool.rpt");
            feereportbyschool.Load();
            feereportbyschool.PrintOptions.PaperSize = PaperSize.PaperA4;
            feereportbyschool.SetParameterValue("InvoicefrmDate",FromDate);
            feereportbyschool.SetParameterValue("Invoicetodate", ToDate);
            feereportbyschool.SetParameterValue("SchoolID", schoolid);

            return genericReportSetting(feereportbyschool, httpctx);
        }

        /// <summary>
        /// The method reports application form new student addmission
        /// </summary>
        /// <param name="httpctx">this paramters is require as method is call asynchronously on seperate thread,hence current hhtp context</param>
        /// <param name="id">student id,which is in guid form is sent to report</param>
        /// <returns>Reports stream which can converted to pdf</returns>
        public Stream generateApplicationForm(HttpContext httpctx, string id)
        {
            ReportClass ApplicationForm = new ReportClass();
            ApplicationForm.FileName = httpctx.Server.MapPath("~/Reports/ApplicationForm.rpt");
            ApplicationForm.Load();
            ApplicationForm.PrintOptions.PaperSize = PaperSize.PaperA4;
            ApplicationForm.SetParameterValue("StudentID", "{" + id + "}");
            
            return genericReportSetting(ApplicationForm, httpctx);
        }
        /// <summary>
        /// Method allow invoice to be edidted, adding invoice detail row after taking exisitng records
        /// into account say, if one invoice detail row exist only two are adeded, if two invoice detail
        /// row exist one row is added
        /// </summary>
        /// <param name="id">invoice id is passed to fetch which invoice need to be edited</param>
        /// <returns>invoiceviewmodel</returns>
        public InvoiceViewModel getInvoiceForEdit(string id)
        {
            int invoiceid = Convert.ToInt32(id);
            InvoiceViewModel invoiceVM = Mapper.Map<InvoiceViewModel>(db.Invoices.Find(invoiceid));

            for (var i = invoiceVM.InvoiceDetailsViewModel.Count; i < 3 ; i++)
            {
                InvoiceDetailsViewModel invdetail = new InvoiceDetailsViewModel()
                {
                    Description = "",
                    Quantity = 0,
                    UnitPrice = 0,
                    Amount = 0
                };
                invoiceVM.InvoiceDetailsViewModel.Add(invdetail);
            }

            return invoiceVM;
        }
        /// <summary>
        /// This method is similar to saving data after report, here data is saved after report editing
        /// </summary>
        /// <param name="invoiceVM">invoice view model</param>
        /// <returns>booleand wheather data is saved or not</returns>
        public bool saveAfterEdit(InvoiceViewModel invoiceVM)
        {
            if (!validateInvoice(invoiceVM))
            {
                return false;
            }
            try
            { 
                for (var i = invoiceVM.InvoiceDetailsViewModel.Count - 1; i >= 0; i--)
                {
                    string desc = (invoiceVM.InvoiceDetailsViewModel[i].Description != null) ? invoiceVM.InvoiceDetailsViewModel[i].Description.Trim() : "";

                    if (desc == "")
                    {
                        invoiceVM.InvoiceDetailsViewModel.RemoveAt(i);
                    }
                }

                Invoice updateInvoice = Mapper.Map<Invoice>(invoiceVM);

                foreach(var invDetail in updateInvoice.InvoiceDetails)
                {
                    if(invDetail.InvoiceDetailID !=0)
                    {
                        db.Entry(invDetail).State = EntityState.Modified;
                    }
                    else if(invDetail.InvoiceDetailID == 0)
                    {
                        invDetail.InvoiceRefID = invoiceVM.InvoiceID;
                        db.Entry(invDetail).State = EntityState.Added;
                    }
                }

                db.Entry(updateInvoice).State = EntityState.Modified;
                db.SaveChanges();
                return true;

            }
            catch(Exception e)
            {
                _modelstate.AddModelError("", e.Message);
                return false;
            }
           
        }
    }

    public interface IInvoiceService
    {
        ModelStateDictionary ModelState { get; set; }
        OsisContext getDBContext();
        PagedList.IPagedList<InvoiceSingle> getInvoiceList(int? page, string user = null);
        InvoiceViewModel createInvoice(Guid id);
        bool saveInvoice(InvoiceViewModel invoiceVM);
        bool validateInvoice(InvoiceViewModel invoiceVM);
        Stream generateInVoiceReport(string id,HttpContext httpctx);
        InvoiceViewModel getInvoiceForEdit(string id);
        bool saveAfterEdit(InvoiceViewModel invoiceVM);
        Stream generateTotalFeeReport(HttpContext httpctx,DateTime FromDate,DateTime ToDate);
        Stream generateTotalFeeReportBySchool(HttpContext httpctx, DateTime FromDate, DateTime ToDate,int schoolid);
        Stream generateApplicationForm(HttpContext httpctx,string id);
        PagedList.IPagedList<ShowApplicationForm> getApplicationList(int? page, string user = null);
    }
}