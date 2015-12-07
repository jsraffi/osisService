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
        private OsisContext db = new OsisContext();

        private ModelStateDictionary _modelstate;

        public InvoiceService(ModelStateDictionary modelstate)
        {
            _modelstate = modelstate;
        }

        public OsisContext getDBContext()
        {
            return db;
        }
        public PagedList.IPagedList<InvoiceSingle> getInvoiceList(int? page, string user = null)
        {
            Tuple<int, int> currentschoolandacademicyear = getUserCurrentSchool(db, user);
            int pageSize = Convert.ToInt32(ConfigurationManager.AppSettings["pageSize"]);
            var pageNumber = page ?? 1;
            var invoices = db.InvoiceSingle.AsNoTracking().OrderBy(d => d.InvoiceDate).Where(sa => sa.SchoolRefID == currentschoolandacademicyear.Item1 && sa.AcademicYearRefID == currentschoolandacademicyear.Item2);

            return invoices.ToPagedList(pageNumber, pageSize);
        }
        public PagedList.IPagedList<ShowApplicationForm> getApplicationList(int? page, string user = null)
        {
            Tuple<int, int> currentschoolandacademicyear = getUserCurrentSchool(db, user);
            int pageSize = Convert.ToInt32(ConfigurationManager.AppSettings["pageSize"]);
            var pageNumber = page ?? 1;

            var appplicationlist = db.ShowApplicationForms.AsNoTracking().OrderBy(d => d.DateOfJoining).Where(sa => sa.SchoolRefID == currentschoolandacademicyear.Item1 && sa.AcademicYearRefID == currentschoolandacademicyear.Item2);

            return appplicationlist.ToPagedList(pageNumber, pageSize);
        }
        public InvoiceViewModel createInvoice(Guid id)
        {
            StudentCurrentYear studentcurrentyear = db.StudentCurrentYears.Where(sid => sid.StudentRefID == id && sid.Active == true).SingleOrDefault();

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
        public bool validateInvoice(InvoiceViewModel invoiceVM)
        {
            Boolean iserror = true;
            int Count = 0;
            for(var i=0;i < invoiceVM.InvoiceDetailsViewModel.Count;i++)
            {
                string desc = (invoiceVM.InvoiceDetailsViewModel[i].Description != null) ? invoiceVM.InvoiceDetailsViewModel[i].Description.Trim() : "";

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

                    if(iserror)
                    {
                        Count = Count + 1;

                    }
                }
            }
            if (Count == 0)
            {
                iserror = false;
                _modelstate.AddModelError("", "No Records to add");
            }


            return iserror;
        }

        public bool saveInvoice(InvoiceViewModel invoiceVM)
        {
            if(!validateInvoice(invoiceVM))
            {
                return false;
            }
            for (var i = invoiceVM.InvoiceDetailsViewModel.Count-1;i >=0; i--)
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
        public Stream generateInVoiceReport(string id,HttpContext httpctx)
        {
            ReportDocument feereport = new ReportDocument();
            
            feereport.Load(httpctx.Server.MapPath("~/Reports/Invoice.rpt"));
            feereport.PrintOptions.PaperSize = PaperSize.PaperA4;
            feereport.SetParameterValue("InvoiceID", id);

            return genericReportSetting(feereport, httpctx);
        }
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

        public Stream generateApplicationForm(HttpContext httpctx, string id)
        {
            ReportClass ApplicationForm = new ReportClass();
            ApplicationForm.FileName = httpctx.Server.MapPath("~/Reports/ApplicationForm.rpt");
            ApplicationForm.Load();
            ApplicationForm.PrintOptions.PaperSize = PaperSize.PaperA4;
            ApplicationForm.SetParameterValue("StudentID", "{" + id + "}");
            
            return genericReportSetting(ApplicationForm, httpctx);
        }
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

        public bool saveAfterEdit(InvoiceViewModel invoiceVM)
        {
            if (!validateInvoice(invoiceVM))
            {
                return false;
            }
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
    }

    public interface IInvoiceService
    {
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