using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using CrystalDecisions.ReportSource;
using CrystalDecisions.ReportAppServer;
using CrystalDecisions.ReportAppServer.DataDefModel;
using CrystalDecisions.ReportAppServer.ClientDoc;
using System.IO;
using OsisModel.Models;
using PagedList;
using System.Configuration;
using Microsoft.AspNet.Identity;
using OsisModel.Services;
namespace okisSIS.Controllers
{
    public class InvoiceController : Controller
    {
        private IInvoiceService _service;

        public InvoiceController(IInvoiceService service)
        {
            _service = service;
        }
        public InvoiceController()
        {
            _service = new InvoiceService(this.ModelState);
        }
        public async Task<ActionResult> Index(int? page)
        {
            return View(await Task.Run(() => _service.getInvoiceList(page, User.Identity.GetUserName())));
        }

        // GET: /Invoice/Create
        public async Task<ActionResult> Create(Guid id)
        {   
            return View(await Task.Run(() => _service.createInvoice(id)));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(InvoiceViewModel invoiceVM)
        {
                if (ModelState.IsValid)
                {
                    if(await Task.Run(() => _service.saveInvoice(invoiceVM)))
                    {
                        return RedirectToAction("Index");
                    }
                }

                //ViewBag.StudentID = new SelectList(db.Students, "StudentID", "NameOfChild", invoices.StudentID);
                return View(invoiceVM);
        }

        public async Task<ActionResult> ShowInvoiceReport(string id)
        {
            var cntx = System.Web.HttpContext.Current;

            return File(await Task.Run(()=> _service.generateInVoiceReport(id,cntx)), "application/pdf");
            
        }

        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            InvoiceViewModel  invoiceVM = await Task.Run(() =>_service.getInvoiceForEdit(id));
            if (invoiceVM == null)
            {
                return HttpNotFound();
            }
            return View(invoiceVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(InvoiceViewModel invoiceVM)
        {
            if(ModelState.IsValid)
            {
                if(await Task.Run(() => _service.saveAfterEdit(invoiceVM)))
                {
                    return RedirectToAction("Index");
                }
            }

            return View(invoiceVM);
        }
        public ActionResult FeeCollectedReport()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> FeeCollectedReport(FeeCollectedViewModel model)
        {

            if (ModelState.IsValid)
            {
                var cntx = System.Web.HttpContext.Current;

                return File(await Task.Run(() => _service.generateTotalFeeReport(cntx,model.DateFrom,model.DateTo)), "application/pdf");

            }

            return View(model);
        }

        public ActionResult FeeCollectedReportBySchool()
        {

                var dbc = _service.getDBContext();
                ViewBag.SchoolID = new SelectList(dbc.Schools.AsNoTracking(), "SchoolID", "SchoolName").ToList();

                return View();
           
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> FeeCollectedReportBySchool(FeeCollectedBySchoolViewModel model)
        {

            if (ModelState.IsValid)
            {
                var cntx = System.Web.HttpContext.Current;
                return File(await Task.Run(() => _service.generateTotalFeeReportBySchool(cntx, model.DateFrom, model.DateTo, model.SchoolID)), "application/pdf");
            }

            var dbc = _service.getDBContext(); 
            ViewBag.SchoolID = new SelectList(dbc.Schools, "SchoolID", "SchoolName", model.SchoolID).ToList();
            return View();
        }

        public async Task<ActionResult>ShowApplicationForm(string id)
        {

            var cntx = System.Web.HttpContext.Current;
            return File(await Task.Run(() => _service.generateApplicationForm(cntx, id)), "application/pdf");


        }
        public async Task<ActionResult>ShowApplicationIndex(int? page)
        {
            return View(await Task.Run(() => _service.getApplicationList(page, User.Identity.GetUserName())));
        }
                /*

                            // GET: /Invoice/Details/5
                            public ActionResult Details(int? id)
                            {
                                using (var db = new OsisContext())
                                {
                                    if (id == null)
                                    {
                                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                                    }
                                    var invoices = db.Invoices.Include("Student").Include("Class").FirstOrDefault(d => d.InvoiceID == id); 


                                    if (invoices == null)
                                    {
                                        return HttpNotFound();
                                    }
                                    Invoice detailview = new Invoice();

                                    detailview.InvoiceID = invoices.InvoiceID;
                                    detailview.Discount = invoices.Discount;
                                    detailview.LateFee = invoices.LateFee;
                                    detailview.InvoiceDate = invoices.InvoiceDate;
                                    detailview.Class = invoices.Class;
                                    detailview.Student = invoices.Student;
                                    detailview.InvoiceDetail = invoices.InvoiceDetail;

                                    return View(detailview);
                                }
                            }

                            public ActionResult Delete(int? id)
                            {

                                using(var db = new OsisContext())
                                { 
                                    if (id == null)
                                    {
                                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                                    }
                                    var invoices = db.Invoices.Include("Student").Include("Class").FirstOrDefault(d => d.InvoiceID == id); 

                                    if (invoices == null)
                                    {
                                        return HttpNotFound();
                                    }
                                    Invoice detailview = new Invoice();

                                    detailview.InvoiceID = invoices.InvoiceID;
                                    detailview.Discount = invoices.Discount;
                                    detailview.LateFee = invoices.LateFee;
                                    detailview.InvoiceDate = invoices.InvoiceDate;
                                    detailview.Class = invoices.Class;
                                    detailview.Student = invoices.Student;

                                    return View(detailview);
                                }
                            }

                            [HttpPost, ActionName("Delete")]
                            [ValidateAntiForgeryToken]
                            public async Task<ActionResult> DeleteConfirmed(int id)
                            {
                                using(var db = new OsisContext())
                                {
                                    Invoice invoices = await db.Invoices.FindAsync(id);
                                    db.Invoices.Remove(invoices);
                                    await db.SaveChangesAsync();
                                    return RedirectToAction("Index");
                                }
                            }



                            public ActionResult FeeCollectedReport()
                            {
                                return View();
                            }

                            [HttpPost]
                            [ValidateAntiForgeryToken]
                            public ActionResult FeeCollectedReport(FeeCollectedViewModel model)
                            {

                                if (ModelState.IsValid)
                                {
                                    ReportClass feereport = new ReportClass();
                                    feereport.FileName = Server.MapPath("~/Reports/TotalFeescollected.rpt");
                                    feereport.Load();
                                    feereport.PrintOptions.PaperSize = PaperSize.PaperA4;
                                    feereport.SetParameterValue("InvoicefrmDate", model.DateFrom);
                                    feereport.SetParameterValue("Invoicetodate", model.DateTo);
                                    //feereport.SetDatabaseLogon("osisv1", "osisv1234", "TRAINING10-PC\\SQLEXPRESS", "OSISV1");

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
                                    attributes.Add("QE_ServerDescription", Server);
                                    attributes.Add("QESQLDB", true);
                                    attributes.Add("SSO Enabled", false);

                                    CrystalDecisions.ReportAppServer.DataDefModel.ConnectionInfo ci = new CrystalDecisions.ReportAppServer.DataDefModel.ConnectionInfo();
                                    ci.Attributes = attributes;
                                    ci.Kind = CrConnectionInfoKindEnum.crConnectionInfoKindCRQE;
                                    ci.UserName = ConfigurationManager.AppSettings["UserID"];
                                    ci.Password = ConfigurationManager.AppSettings["Password"];

                                    foreach (CrystalDecisions.ReportAppServer.DataDefModel.Table table in feereport.ReportClientDocument.DatabaseController.Database.Tables)
                                    {
                                        CrystalDecisions.ReportAppServer.DataDefModel.Procedure newTable = new CrystalDecisions.ReportAppServer.DataDefModel.Procedure();

                                        newTable.ConnectionInfo = ci;
                                        newTable.Name = table.Name;
                                        newTable.Alias = table.Alias;
                                        newTable.QualifiedName = ConfigurationManager.AppSettings["Database"] + ".dbo." + table.Name;
                                        feereport.ReportClientDocument.DatabaseController.SetTableLocation(table, newTable);
                                    }

                                    Stream stream = feereport.ExportToStream(ExportFormatType.PortableDocFormat);
                                    feereport.Dispose();
                                    return File(stream, "application/pdf");
                                }

                                return View(model);
                            }

                            public ActionResult FeeCollectedReportBySchool()
                            {
                                using (var db = new OsisContext())
                                {
                                    ViewBag.SchoolID = new SelectList(db.Schools, "SchoolID", "SchoolName").ToList();

                                    return View();
                                }
                            }

                            [HttpPost]
                            [ValidateAntiForgeryToken]
                            public ActionResult FeeCollectedReportBySchool( FeeCollectedBySchoolViewModel model)
                            {

                                if (ModelState.IsValid)
                                {
                                    ReportClass feereport = new ReportClass();
                                    feereport.FileName = Server.MapPath("~/Reports/TotalFeescollectedBySchool.rpt");
                                    feereport.Load();
                                    feereport.PrintOptions.PaperSize = PaperSize.PaperA4;
                                    feereport.SetParameterValue("InvoicefrmDate", model.DateFrom);
                                    feereport.SetParameterValue("Invoicetodate", model.DateTo);
                                    feereport.SetParameterValue("SchoolID", model.SchoolID);
                                    //feereport.SetDatabaseLogon("osisv1", "osisv1234", "TRAINING10-PC\\SQLEXPRESS", "OSISV1");


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
                                    attributes.Add("QE_ServerDescription", Server);
                                    attributes.Add("QESQLDB", true);
                                    attributes.Add("SSO Enabled", false);

                                    CrystalDecisions.ReportAppServer.DataDefModel.ConnectionInfo ci = new CrystalDecisions.ReportAppServer.DataDefModel.ConnectionInfo();
                                    ci.Attributes = attributes;
                                    ci.Kind = CrConnectionInfoKindEnum.crConnectionInfoKindCRQE;
                                    ci.UserName = ConfigurationManager.AppSettings["UserID"];
                                    ci.Password = ConfigurationManager.AppSettings["Password"];

                                    foreach (CrystalDecisions.ReportAppServer.DataDefModel.Table table in feereport.ReportClientDocument.DatabaseController.Database.Tables)
                                    {
                                        CrystalDecisions.ReportAppServer.DataDefModel.Procedure newTable = new CrystalDecisions.ReportAppServer.DataDefModel.Procedure();

                                        newTable.ConnectionInfo = ci;
                                        newTable.Name = table.Name;
                                        newTable.Alias = table.Alias;
                                        newTable.QualifiedName = ConfigurationManager.AppSettings["Database"] + ".dbo." + table.Name;
                                        feereport.ReportClientDocument.DatabaseController.SetTableLocation(table, newTable);
                                    }

                                    Stream stream = feereport.ExportToStream(ExportFormatType.PortableDocFormat);
                                    feereport.Dispose();
                                    return File(stream, "application/pdf");
                                }
                                using (var db = new OsisContext())
                                {
                                    ViewBag.SchoolID = new SelectList(db.Schools, "SchoolID", "SchoolName", model.SchoolID).ToList();

                                    return View();
                                }
                            }




                            [HttpPost]
                            [ValidateAntiForgeryToken]
                            public async Task<ActionResult> Create(Invoice invoices)
                            {
                                using(var db = new OsisContext())
                                {

                                    if (ModelState.IsValid)
                                    {
                                        var invoiceinsert = new Invoice()
                                        {
                                            StudentID = invoices.StudentID,
                                            SchoolID = invoices.SchoolID,
                                            ClassID = invoices.ClassID,
                                            AcademicYearID = invoices.AcademicYearID,
                                            Discount = invoices.Discount,
                                            LateFee = invoices.LateFee,
                                            InvoiceDate= invoices.InvoiceDate

                                        };

                                        Boolean isdata = false;
                                        foreach(var invdetailview in invoices.InvoiceDetail)
                                        {
                                            String desc = invdetailview.Description.Trim();
                                            if(invdetailview.Amount > 0 && invdetailview.Quantity>0 && invdetailview.UnitPrice >0 &&   desc !="")
                                            {
                                                isdata = true;

                                                var invdetview = new InvoiceDetail();
                                                invdetview.Description = invdetailview.Description;
                                                invdetview.Quantity = invdetailview.Quantity;
                                                invdetview.UnitPrice = invdetailview.UnitPrice;
                                                invdetview.Amount = invdetailview.Amount;
                                                invdetview.StudentID = invoices.StudentID;
                                                invoiceinsert.InvoiceDetail.Add(invdetview);

                                            }
                                        }

                                        if (isdata) 
                                        {

                                            db.Invoices.Add(invoiceinsert);
                                            await db.SaveChangesAsync();
                                            return RedirectToAction("Index", "Invoice");
                                        }
                                        else
                                        {
                                            ViewBag.StudentName="No data to create receipt";
                                        }


                                    }

                                    //ViewBag.StudentID = new SelectList(db.Students, "StudentID", "NameOfChild", invoices.StudentID);
                                    return View(invoices);
                                }
                            }

                            public async Task<ActionResult> Edit(int? id)
                            {
                                using (var db = new OsisContext())
                                { 
                                    if (id == null)
                                    {
                                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                                    }
                                    Invoice invoices = await db.Invoices.FindAsync(id);
                                    if (invoices == null)
                                    {
                                        return HttpNotFound();
                                    }



                                    for (var i = invoices.InvoiceDetail.Count; i < 4; i++)
                                    {
                                        InvoiceDetail invdetail = new InvoiceDetail() 
                                        {
                                            StudentID = invoices.StudentID,
                                            Description = " ",
                                            Quantity = 0,
                                            UnitPrice = 0,
                                            Amount = 0

                                        };

                                        invoices.InvoiceDetail.Add(invdetail);
                                    }

                                    return View(invoices);
                                }
                            }


                            [HttpPost]
                            [ValidateAntiForgeryToken]
                            public async Task<ActionResult> Edit(Invoice invoices)
                            {
                                using(var db = new OsisContext())
                                { 
                                    if (ModelState.IsValid)
                                    {

                                        var invoiceupdate = new Invoice()
                                        {
                                            InvoiceID = invoices.InvoiceID,
                                            StudentID = invoices.StudentID,
                                            SchoolID = invoices.SchoolID,
                                            ClassID = invoices.ClassID,
                                            AcademicYearID = invoices.AcademicYearID,
                                            Discount = invoices.Discount,
                                            LateFee = invoices.LateFee,
                                            InvoiceDate = invoices.InvoiceDate

                                        };



                                        Boolean isdata = false;
                                        foreach (var invdetailview in invoices.InvoiceDetail)
                                        {
                                            String desc = invdetailview.Description.Trim();
                                            if (invdetailview.Amount > 0 && invdetailview.Quantity > 0 && invdetailview.UnitPrice > 0 && desc != "")
                                            {
                                                isdata = true;

                                                var invdetview = new InvoiceDetail();
                                                invdetview.Description = invdetailview.Description;
                                                invdetview.Quantity = invdetailview.Quantity;
                                                invdetview.UnitPrice = invdetailview.UnitPrice;
                                                invdetview.Amount = invdetailview.Amount;
                                                invdetview.StudentID = invoices.StudentID;

                                                if (invdetailview.InvoiceDetailId != 0)
                                                {
                                                    invdetview.InvoiceDetailId = invdetailview.InvoiceDetailId;
                                                    invdetview.InvoiceId = invdetailview.InvoiceId;
                                                    db.Entry(invdetview).State = EntityState.Modified;    
                                                }
                                                else
                                                {
                                                    invdetview.InvoiceId = invoices.InvoiceID;
                                                    invoiceupdate.InvoiceDetail.Add(invdetview);
                                                    db.Entry(invdetview).State = EntityState.Added;
                                                }

                                            }
                                        }

                                        if (isdata)
                                        {

                                            db.Entry(invoiceupdate).State = EntityState.Modified;
                                            await db.SaveChangesAsync();
                                            return RedirectToAction("Index", "Invoice");
                                        }
                                        else
                                        {
                                            ViewBag.StudentName = "No data to create receipt";
                                        }

                                        await db.SaveChangesAsync();
                                        return RedirectToAction("Index");
                                    }

                                    return View(invoices);

                                }
                            }

                    */

            }
        }
