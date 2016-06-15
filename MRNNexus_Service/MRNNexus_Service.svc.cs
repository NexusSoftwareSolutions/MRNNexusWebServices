using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using MRNNexusDTOs;
using MRNNexus_DAL;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data.Entity.Core.Objects;
using System.Security.Cryptography;
using System.Collections;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;

namespace MRNNexus_Service
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "MRNNexus_Service" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select MRNNexus_Service.svc or MRNNexus_Service.svc.cs at the Solution Explorer and start debugging.
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession,
                 ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class MRNNexus_Service : IMRNNexus_Service
    {
        private const int SaltByteLength = 24;
        private const int DerivedKeyLength = 24;
        private const string FTPURL = "ftp://services.mrncontracting.com/";
        private const string FTPUSERNAME = "serviceuploader";
        private const string FTPPASSWORD = "Scrappy!";

        #region DoWork
        public DTO_Employee DoWork(DTO_Employee token)
        {
            token.FirstName = "Hello World";
            return token;
        }

        public void DoMoreWork()
        {

        }
        #endregion

        #region Login --verifies password and returns an emp id
        public DTO_Employee Login(DTO_User token)
        {
            DTO_Employee returnToken = new DTO_Employee();
            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                try
                {
                    var outputParameter = new ObjectParameter("employeeID", typeof(int));

                    var user = context.proc_GetUser(token.Username).Single();

                    if (VerifyPassword(token.Pass, user.Pass))
                    {
                        returnToken.EmployeeID = user.EmployeeID;
                        returnToken.SuccessFlag = true;
                    }
                    else
                    {
                        returnToken.Message = "Invalid Username/Password.";
                    }
                }
                catch (InvalidOperationException e)
                {
                    returnToken.Message = "InvalidOperationException in Login() " + e.Message;
                }
                catch (Exception ex)
                {
                    returnToken = (DTO_Employee)populateException(ex, returnToken);
                }
                return returnToken;
            }
        }

        public DTO_User GetUser(DTO_User token)
        {
            
            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                try
                {
                    var outputParameter = new ObjectParameter("employeeID", typeof(int));

                    var user = context.proc_GetUser(token.Username).Single();


                    token = new DTO_User
                    {
                        PermissionID = user.PermissionID,
                        Username = user.Username,
                        Active = user.Active

                    };

                }
                catch (InvalidOperationException e)
                {
                    token.Message = "InvalidOperationException in Login() " + e.Message;
                }
                catch (Exception ex)
                {
                    token = (DTO_User)populateException(ex, token);
                }
                return token;
            }
        }
        #endregion

        #region RegisterUserAccount -- REMOVE QUERY Replace with proc
        public DTO_User RegisterUser(DTO_User token)
        {
            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                var outputParameter = new ObjectParameter("new_identity", typeof(int));

                DTO_Employee emp = new DTO_Employee();
                emp.LastName = "BOB";
                emp.CellPhone = "678-367-9565";
                emp.Email = "asdsad@gmail.com";
                try
                {
                    // Retrieve employee data needed for creating a new user and auto generating starting password
                    //var employee = context.Employees.Where(e => e.EmployeeID == token.EmployeeID).Single();
                    //emp.LastName = employee.LastName;
                    //emp.CellPhone = employee.CellPhone;
                    //emp.Email = employee.Email;
                    token.Username = emp.Email;

                    string hash = CreatePasswordHASH(emp);
                    bool worked = VerifyPassword(emp.LastName + emp.CellPhone.Substring(emp.CellPhone.Length - 4), hash);

                    if (worked)
                    {
                        var id = context.proc_RegisterUser(token.EmployeeID, token.Pass, token.PermissionID, outputParameter);

                        id = (int)outputParameter.Value;

                        token.UserID = id;
                        token.Pass = hash;
                        token.SuccessFlag = true;
                    }
                    else
                    {
                        token.Message = "Error Registering User. Please contact the system administrator.";
                    }
                }
                catch (Exception ex) //if GetEmployee FAILS
                {
                    token = (DTO_User)populateException(ex, token);
                }
            }
            return token;

        }
        #endregion

        #region Claim Post Processing

        private void ClaimCreatedPostProcessing(ref DTO_Claim token)
        {
            bool directoriesCreated = createDirectoryStructure(GetClaimByClaimID(token).MRNNumber);
            bool claimContactsCreated = AutoAddClaimContacts(ref token);
            bool claimStatusCreated = true;

            if (directoriesCreated)
            {
                if (claimContactsCreated)
                {
                    if (claimStatusCreated)
                    {

                    }
                    else
                    {
                        token.SuccessFlag = false;
                        token.Message += "Could Not Create Claim Status Records";
                    }
                }
                else
                {
                    token.SuccessFlag = false;
                    token.Message += "Could Not Create Claim Contacts Records.";
                }
            }
            else
            {
                token.SuccessFlag = false;
                token.Message += "Could Not Create Directory Structure.";
            }
        }

        private bool AutoAddClaimContacts(ref DTO_Claim token)
        {
            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                try
                {
                    var outputParameter = new ObjectParameter("new_identity", typeof(int));

                    var result = context.proc_AutoAddClaimContacts(token.ClaimID, outputParameter);

                    if((int)outputParameter.Value > 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    token = (DTO_Claim)populateException(ex, token);
                    return false;
                }
            }
        }

        #endregion

        /*NEED TESTING*/
        public List<DTO_ClaimDocument> GetClaimDocumentsByClaimID(DTO_Claim token)
        {
            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                List<DTO_ClaimDocument> returnList = new List<DTO_ClaimDocument>();
                try
                {
                    var results = context.proc_GetClaimDocumentsByClaimID(token.ClaimID).ToList();

                    foreach (var entity in results)
                    {
                        DTO_ClaimDocument cd = new DTO_ClaimDocument();
                        cd.DocTypeID = entity.DocTypeID;
                        cd.DocumentDate = entity.DocumentDate;
                        cd.DocumentID = entity.DocumentID;
                        cd.FileExt = entity.FileExt;
                        cd.FileName = entity.FileName;
                        cd.FilePath = entity.FilePath;
                        cd.InitialImagePath = entity.InitialImagePath;
                        cd.SignatureImagePath = entity.SignatureImagePath;
                        cd.NumInitials = entity.NumInitials;
                        cd.NumSignatures = entity.NumSignatures;
                        cd.SuccessFlag = true;

                        //if (entity.NumInitials == null) { cd.NumInitials = 0; }
                        //else { cd.NumInitials = (int)entity.NumInitials; }
                        //if (entity.NumSignatures == null) { cd.NumSignatures = 0; }
                        //else { cd.NumSignatures = (int)entity.NumSignatures; }




                        returnList.Add(cd);
                    }

                }
                catch (InvalidOperationException e)
                {
                    token.Message = "InvalidOperationException in GetClaimDocumentsByClaimID() " + e.Message;
                }
                catch (Exception ex)
                {
                    DTO_ClaimDocument obj = (DTO_ClaimDocument)populateException(ex, new DTO_ClaimDocument());
                    returnList.Add(obj);
                }

                return returnList;
            }

        }

        /*COMPLETED*/
        #region ADDS

        #region AddAdditionalSupply
        public DTO_AdditionalSupply AddAdditionalSupply(DTO_AdditionalSupply token)
        {
            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                try
                {
                    var outputParameter = new ObjectParameter("new_identity", typeof(int));

                    var result = context.proc_AddAdditionalSupplies(token.ClaimID, token.PickUpDate,
                        token.DropOffDate, token.Items, token.Cost, token.ReceiptImagePath, outputParameter);

                    token.AdditionalSuppliesID = (int)outputParameter.Value;

                }
                catch (Exception ex)
                {
                    token = (DTO_AdditionalSupply)populateException(ex, token);
                }
            }

            return token;
        }
        #endregion

        #region AddAddress
        public DTO_Address AddAddress(DTO_Address token)
        {
            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                try
                {
                    var outputParameter = new ObjectParameter("new_identity", typeof(int));
                    var id = context.proc_AddAddress(token.CustomerID, token.Address, token.Zip, outputParameter);

                    token.AddressID = (int)outputParameter.Value;
                }
                catch (Exception ex)
                {
                    token = (DTO_Address)populateException(ex, token);
                }
            }
            return token;
        }
        #endregion

        #region AddAdjuster
        public DTO_Adjuster AddAdjuster(DTO_Adjuster token)
        {
            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                try
                {
                    var outputParameter = new ObjectParameter("new_identity", typeof(int));

                    var result = context.proc_AddAdjuster(token.FirstName, token.LastName, token.Suffix, token.PhoneNumber,
                        token.PhoneExt, token.Email, token.InsuranceCompanyID, token.Comments, outputParameter);

                    token.AdjusterID = (int)outputParameter.Value;
                }
                catch (Exception ex)
                {
                    token = (DTO_Adjuster)populateException(ex, token);
                }
            }

            return token;
        }
        #endregion

        #region AddAdjustment
        public DTO_Adjustment AddAdjustment(DTO_Adjustment token)
        {
            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                try
                {
                    var outputParameter = new ObjectParameter("new_identity", typeof(int));

                    var result = context.proc_AddAdjustment(token.AdjusterID, token.ClaimID, token.Gutters,
                        token.Exterior, token.Interior, token.AdjustmentResultID, token.AdjustmentDate,
                        token.AdjustmentComment, outputParameter);

                    token.AdjustmentID = (int)outputParameter.Value;
                }
                catch (Exception ex)
                {
                    token = (DTO_Adjustment)populateException(ex, token);
                }
            }

            return token;
        }
        #endregion

        #region AddCalendatData
        public DTO_CalendarData AddCalendarData(DTO_CalendarData token)
        {
            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                try
                {
                    int id;
                    var outputParameter = new ObjectParameter("new_identity", typeof(int));

                    id = context.proc_AddCalendarData(token.AppointmentTypeID, token.EmployeeID, token.StartTime, token.EndTime, token.ClaimID, token.LeadID, token.Note, outputParameter);

                    id = (int)outputParameter.Value;

                    token.EntryID = id;
                }
                catch (Exception ex)
                {
                    token = (DTO_CalendarData)populateException(ex, token);
                }
            }

            return token;
        }
        #endregion

        #region AddCallLog
        public DTO_CallLog AddCallLog(DTO_CallLog token)
        {
            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                try
                {
                    var outputParameter = new ObjectParameter("new_identity", typeof(int));

                    var result = context.proc_AddCallLog(token.ClaimID, token.EmployeeID, token.WhoWasCalled, token.ReasonForCall, token.WhoAnswered,
                        token.CallResults, outputParameter);

                    token.CallLogID = (int)outputParameter.Value;

                }
                catch (Exception ex)
                {
                    token = (DTO_CallLog)populateException(ex, token);
                }
            }

            return token;
        }
        #endregion

        #region AddClaim
        public DTO_Claim AddClaim(DTO_Claim token)
        {
            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                try
                {
                    var outputParameter = new ObjectParameter("new_identity", typeof(int));

                    var result = context.proc_AddClaim(token.CustomerID, token.LeadID, token.BillingID, token.PropertyID, token.InsuranceCompanyID,
                        token.InsuranceClaimNumber, token.LossDate, token.MortgageCompany, token.MortgageAccount,
                        outputParameter);

                    token.ClaimID = (int)outputParameter.Value;

                    ClaimCreatedPostProcessing(ref token);
                }
                catch (Exception ex)
                {
                    token = (DTO_Claim)populateException(ex, token);
                }
            }
            return token;
        }
        #endregion

        #region AddClaimContacts
        public DTO_ClaimContacts AddClaimContacts(DTO_ClaimContacts token)
        {
            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                try
                {
                    var outputParameter = new ObjectParameter("new_identity", typeof(int));

                    var result = context.proc_AddClaimContact(token.ClaimID, token.CustomerID, token.KnockerID, token.SalesPersonID, token.SupervisorID,
                        token.SalesManagerID, token.AdjusterID, outputParameter);

                    token.ClaimContactID = (int)outputParameter.Value;

                }
                catch (Exception ex)
                {
                    token = (DTO_ClaimContacts)populateException(ex, token);
                }
            }

            return token;
        }
        #endregion

        #region AddClaimDocument
        public DTO_ClaimDocument AddClaimDocument(DTO_ClaimDocument token)
        {
            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                try
                {
                    uploadClaimDocument(ref token);

                    var outputParameter = new ObjectParameter("new_identity", typeof(int));

                    var result = context.proc_AddClaimDocuments(token.ClaimID, token.FilePath, token.FileName, token.FileExt, token.DocTypeID, token.DocumentDate,
                        token.SignatureImagePath, token.NumSignatures, token.InitialImagePath, token.NumInitials,  outputParameter);

                    token.DocumentID = (int)outputParameter.Value;
                }
                catch (Exception ex)
                {
                    token = (DTO_ClaimDocument)populateException(ex, token);
                }
            }
            return token;
        }
        #endregion

        #region AddClaimStatus
        public DTO_ClaimStatus AddClaimStatus(DTO_ClaimStatus token)
        {
            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                try
                {
                    var outputParameter = new ObjectParameter("new_identity", typeof(int));

                    var result = context.proc_AddClaimStatus(token.ClaimID, token.ClaimStatusTypeID, token.ClaimStatusDate, outputParameter);

                    token.ClaimStatusID = (int)outputParameter.Value;
                }
                catch (Exception ex)
                {
                    token = (DTO_ClaimStatus)populateException(ex, token);
                }
            }
            return token;
        }
        #endregion  

        #region AddClaimVendor
        public DTO_ClaimVendor AddClaimVendor(DTO_ClaimVendor token)
        {
            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                try
                {
                    var outputParameterClaim = new ObjectParameter("output_ClaimID", typeof(int));
                    var outputParameterVendor = new ObjectParameter("output_VendorID", typeof(int));

                    var result = context.proc_AddClaimVendor(token.ClaimID, token.VendorID, token.ServiceTypeID, outputParameterClaim, outputParameterVendor);

                    token.ClaimID = (int)outputParameterClaim.Value;
                    token.VendorID = (int)outputParameterVendor.Value;
                }
                catch (Exception ex)
                {
                    token = (DTO_ClaimVendor)populateException(ex, token);
                }
            }
            return token;
        }
        #endregion

        #region AddCustomer ADDTO SERVICELAYER
        public DTO_Customer AddCustomer(DTO_Customer token)
        {
            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                try
                {
                    var outputParameter = new ObjectParameter("new_identity", typeof(int));
                    var id = context.proc_AddCustomer(token.FirstName, token.MiddleName, token.LastName, token.Suffix, token.PrimaryNumber,
                        token.SecondaryNumber, token.Email, token.MailPromos, outputParameter);

                    token.CustomerID = (int)outputParameter.Value;
                }
                catch (Exception ex)
                {
                    token = (DTO_Customer)populateException(ex, token);
                }
            }
            return token;
        }
        #endregion

        #region AddDamage
        public DTO_Damage AddDamage(DTO_Damage token)
        {
            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                try
                {
                    var outputParameter = new ObjectParameter("new_identity", typeof(int));
                    var id = context.proc_AddDamage(token.PlaneID, token.DamageTypeID, token.DocumentID, token.DamageMeasurement, token.XCoordinate, token.YCoordinate, outputParameter);

                    token.DamageID = (int)outputParameter.Value;
                }
                catch (Exception ex)
                {
                    token = (DTO_Damage)populateException(ex, token);
                }
            }
            return token;
        }
        #endregion

        #region AddEmployee
        public DTO_Employee AddEmployee(DTO_Employee token)
        {
            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                try
                {
                    var outputParameter = new ObjectParameter("new_identity", typeof(int));

                    var result = context.proc_AddEmployee(token.EmployeeTypeID, token.FirstName, token.LastName,
                        token.Suffix, token.Email, token.CellPhone, outputParameter);

                    token.EmployeeID = (int)outputParameter.Value;
                }
                catch (Exception ex)
                {
                    token = (DTO_Employee)populateException(ex, token);
                }
            }
            return token;
        }
        #endregion

        #region AddInspection
        public DTO_Inspection AddInspection(DTO_Inspection token)
        {
            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                try
                {
                    var outputParameter = new ObjectParameter("new_identity", typeof(int));
                    var result = context.proc_AddInspection(token.ClaimID, token.RidgeMaterialTypeID,
                        token.ShingleTypeID, token.InspectionDate, token.SkyLights, token.Leaks, token.GutterDamage,
                        token.DrivewayDamage, token.MagneticRollers, token.IceWaterShield, token.EmergencyRepair,
                        token.EmergencyRepairAmount, token.QualityControl, token.ProtectLandscaping, token.RemoveTrash,
                        token.FurnishPermit, token.CoverPool, token.InteriorDamage, token.ExteriorDamage, token.LighteningProtection,
                        token.TearOff, token.Satellite, token.SolarPanels, token.RoofAge, token.Comments, outputParameter);

                    int id = (int)outputParameter.Value;

                    token.InspectionID = id;
                }
                catch (Exception ex)
                {
                    token = (DTO_Inspection)populateException(ex, token);
                }
            }
            return token;
        }
        #endregion

        #region AddInsuranceCompany
        public DTO_InsuranceCompany AddInsuranceCompany(DTO_InsuranceCompany token)
        {
            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                try
                {
                    var outputParameter = new ObjectParameter("new_identity", typeof(int));

                    var result = context.proc_AddInsuranceCompany(token.CompanyName, token.Address, token.Zip, token.ClaimPhoneNumber,
                        token.ClaimPhoneExt, token.FaxNumber, token.FaxExt, token.Email, token.Independent, outputParameter);

                    token.InsuranceCompanyID = (int)outputParameter.Value;
                }
                catch (Exception ex)
                {
                    token = (DTO_InsuranceCompany)populateException(ex, token);
                }
            }

            return token;
        }
        #endregion

        #region AddInvoice
        public DTO_Invoice AddInvoice(DTO_Invoice token)
        {
            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                try
                {
                    var outputParameter = new ObjectParameter("new_identity", typeof(int));

                    var result = context.proc_AddInvoice(token.ClaimID, token.InvoiceTypeID, token.InvoiceAmount, token.InvoiceDate, token.Paid, outputParameter);

                    token.InvoiceID = (int)outputParameter.Value;

                }
                catch (Exception ex)
                {
                    token = (DTO_Invoice)populateException(ex, token);
                }
            }

            return token;
        }
        #endregion

        #region AddKnockerResponse
        public DTO_KnockerResponse AddKnockerResponse(DTO_KnockerResponse token)
        {
            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                try
                {
                    var outputParameter = new ObjectParameter("new_identity", typeof(int));

                    var result = context.proc_AddKnockerResponse(token.KnockerID, token.KnockResponseTypeID, token.Address,
                        token.Zip, token.Latitude, token.Longitude, outputParameter);

                    token.KnockerResponseID = (int)outputParameter.Value;
                }
                catch (Exception ex)
                {
                    token = (DTO_KnockerResponse)populateException(ex, token);
                }
            }

            return token;
        }
        #endregion

        #region AddLead
        public DTO_Lead AddLead(DTO_Lead token)
        {
            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                try
                {
                    var outputParameter = new ObjectParameter("new_identity", typeof(int));

                    var result = context.proc_AddLead(token.LeadTypeID, token.KnockerResponseID, token.SalesPersonID, token.CustomerID, token.AddressID, token.LeadDate,
                        token.CreditToID, token.Temperature, outputParameter);

                    token.LeadID = (int)outputParameter.Value;
                }
                catch (Exception ex)
                {
                    token = (DTO_Lead)populateException(ex, token);
                }
            }

            return token;
        }
        #endregion

        #region AddNewRoof
        public DTO_NewRoof AddNewRoof(DTO_NewRoof token)
        {
            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                try
                {
                    var outputParameter = new ObjectParameter("new_identity", typeof(int));

                    var result = context.proc_AddNewRoof(token.ClaimID, token.ProductID, token.UpgradeCost, token.Comments, outputParameter);

                    token.NewRoofID = (int)outputParameter.Value;

                }
                catch (Exception ex)
                {
                    token = (DTO_NewRoof)populateException(ex, token);
                }
            }

            return token;
        }
        #endregion

        #region AddOrder
        public DTO_Order AddOrder(DTO_Order token)
        {
            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                try
                {
                    var outputParameter = new ObjectParameter("new_identity", typeof(int));

                    var result = context.proc_AddOrder(token.VendorID, token.ClaimID, token.DateOrdered, token.DateDropped, token.ScheduledInstallation,
                        outputParameter);

                    token.OrderID = (int)outputParameter.Value;

                }
                catch (Exception ex)
                {
                    token = (DTO_Order)populateException(ex, token);
                }
            }

            return token;
        }
        #endregion

        #region AddOrderItem
        public DTO_OrderItem AddOrderItem(DTO_OrderItem token)
        {
            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                try
                {
                    var outputParameter = new ObjectParameter("new_identity", typeof(int));

                    var result = context.proc_AddOrderItem(token.OrderID, token.ProductID, token.UnitOfMeasureID, token.Quantity,
                        outputParameter);

                    token.OrderItemID = (int)outputParameter.Value;

                }
                catch (Exception ex)
                {
                    token = (DTO_OrderItem)populateException(ex, token);
                }
            }

            return token;
        }
        #endregion

        #region AddPayment
        public DTO_Payment AddPayment(DTO_Payment token)
        {
            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                try
                {
                    var outputParameter = new ObjectParameter("new_identity", typeof(int));

                    var result = context.proc_AddPayment(token.ClaimID, token.PaymentTypeID, token.PaymentDescriptionID, token.Amount, token.PaymentDate, outputParameter);

                    token.PaymentID = (int)outputParameter.Value;

                }
                catch (Exception ex)
                {
                    token = (DTO_Payment)populateException(ex, token);
                }
            }

            return token;
        }
        #endregion

        #region AddPlane
        public DTO_Plane AddPlane(DTO_Plane token)
        {
            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                try
                {
                    var outputParameter = new ObjectParameter("new_identity", typeof(int));

                    var result = context.proc_AddPlane(token.PlaneTypeID, token.InspectionID, token.GroupNumber, token.NumOfLayers, token.ThreeAndOne, token.FourAndUp, token.Pitch,
                        token.HipValley, token.RidgeLength, token.RidgeLength, token.EaveHeight, token.EaveLength, token.NumberDecking, token.StepFlashing,
                        token.SquareFootage, token.ItemSpec, outputParameter);

                    token.PlaneID = (int)outputParameter.Value;
                }
                catch (Exception ex)
                {
                    token = (DTO_Plane)populateException(ex, token);
                }
            }
            return token;
        }
        #endregion

        #region AddReferrer
        public DTO_Referrer AddReferrer(DTO_Referrer token)
        {
            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                try
                {
                    var outputParameter = new ObjectParameter("new_identity", typeof(int));

                    var result = context.proc_AddReferrer(token.FirstName, token.LastName, token.Suffix, token.MailingAddress, token.Zip,
                        token.Email, token.CellPhone, outputParameter);

                    token.ReferrerID = (int)outputParameter.Value;
                }
                catch (Exception ex)
                {
                    token = (DTO_Referrer)populateException(ex, token);
                }
            }

            return token;
        }
        #endregion

        #region AddScope
        public DTO_Scope AddScope(DTO_Scope token)
        {
            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                try
                {
                    var outputParameter = new ObjectParameter("new_identity", typeof(int));

                    var result = context.proc_AddScope(token.ScopeTypeID, token.ClaimID, token.Interior, token.Exterior, token.Gutter, token.RoofAmount, 
                        token.Tax, token.Deductible, token.Total, token.OandP, outputParameter);

                    token.ScopeID = (int)outputParameter.Value;
                }
                catch (Exception ex)
                {
                    token = (DTO_Scope)populateException(ex, token);
                }
            }

            return token;
        }
        #endregion

        #region AddSurplusSupplies
        public DTO_SurplusSupplies AddSurplusSupplies(DTO_SurplusSupplies token)
        {
            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                try
                {
                    var outputParameter = new ObjectParameter("new_identity", typeof(int));

                    var result = context.proc_AddSurplusSupplies(token.ClaimID, token.UnitOfMeasureID, token.Quantity, 
                        token.PickUpDate, token.DropOffDate, token.Items, outputParameter);

                    token.SurplusSuppliesID = (int)outputParameter.Value;

                }
                catch (Exception ex)
                {
                    token = (DTO_SurplusSupplies)populateException(ex, token);
                }
            }

            return token;
        }
        #endregion

        #region AddVendor
        public DTO_Vendor AddVendor(DTO_Vendor token)
        {
            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                try
                {
                    var outputParameter = new ObjectParameter("new_identity", typeof(int));
                    var id = context.proc_AddVendor(token.VendorTypeID, token.CompanyName, token.EIN, token.ContactFirstName, token.ContactLastName, token.Suffix,
                        token.VendorAddress, token.Zip, token.Phone, token.CompanyPhone, token.Fax, token.Email, token.Website, token.GeneralLiabilityExpiration, outputParameter);

                    token.VendorID = (int)outputParameter.Value;
                }
                catch (Exception ex)
                {
                    token = (DTO_Vendor)populateException(ex, token);
                }
            }
            return token;
        }
        #endregion

        #endregion ADDS

        #region GETS

        #region GetAdditionalSuppliesByClaimID
        public List<DTO_AdditionalSupply> GetAdditionalSuppliesByClaimID(DTO_Claim token)
        {
            List<DTO_AdditionalSupply> returnList = new List<DTO_AdditionalSupply>();
            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                try
                {
                    var results = context.proc_GetAdditionalSuppliesByClaimID(token.ClaimID).ToList();

                    foreach (var a in results)
                    {
                        DTO_AdditionalSupply adj = new DTO_AdditionalSupply
                        {
                            AdditionalSuppliesID = a.AdditionalSuppliesID,
                            ClaimID = a.ClaimID,
                            Cost = a.Cost,
                            DropOffDate = a.DropOffDate,
                            Items = a.Items,
                            PickUpDate = a.PickUpDate,
                            ReceiptImagePath = a.ReceiptImagePath,
                            SuccessFlag = true
                        };
                        returnList.Add(adj);
                    }
                }
                catch (Exception ex)
                {
                    DTO_AdditionalSupply obj = (DTO_AdditionalSupply)populateException(ex, new DTO_AdditionalSupply());
                    returnList.Add(obj);
                }
                
            }
            return returnList;
        }
        #endregion

        #region GetAddressByID
        public DTO_Address GetAddressByID(DTO_Address token)
        {
            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                try
                {
                    var result = context.proc_GetAddressByID(token.AddressID).Single();
                    
                    token.CustomerID = result.CustomerID;
                    token.Address = result.Address;
                    token.Zip = result.Zip;
                    token.SuccessFlag = true;
                }
                catch (InvalidOperationException e)
                {
                    token.Message = "InvalidOperationException in GetAddressByID() " + e.Message;
                }
                catch (Exception ex)
                {
                    token = (DTO_Address)populateException(ex, token);
                }
            }
            return token;
        }
        #endregion

        #region GetAddressesBySalesPersonID
        public List<DTO_Address> GetAddressesBySalesPersonID(DTO_Employee token)
        {
            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                List<DTO_Address> returnList = new List<DTO_Address>();
                try
                {
                    var results = context.proc_GetAddressesBySalesPersonID(token.EmployeeTypeID).ToList();

                    foreach (var entity in results)
                    {
                        DTO_Address address = new DTO_Address
                        {
                            AddressID = entity.AddressID,
                            CustomerID = entity.CustomerID,
                            Address = entity.Address,
                            Zip = entity.Zip,                            
                            SuccessFlag = true
                        };

                        returnList.Add(address);
                    }
                }
                catch (InvalidOperationException e)
                {
                    token.Message = "InvalidOperationException in GetAddressesBySalesPersonID() " + e.Message;
                }
                catch (Exception ex)
                {
                    DTO_Address obj = (DTO_Address)populateException(ex, new DTO_Address());
                    returnList.Add(obj);
                }

                return returnList;
            }
        }
        #endregion

        #region GetAdjusterByID
        public DTO_Adjuster GetAdjusterByID(DTO_Adjuster token)
        {
            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                try
                {
                    var result = context.proc_GetAdjusterByID(token.AdjusterID).Single();

                    token.Comments = result.Comments;
                    token.Email = result.Email;
                    token.FirstName = result.FirstName;
                    token.LastName = result.LastName;
                    token.Suffix = result.Suffix;
                    token.InsuranceCompanyID = result.InsuranceCompanyID;
                    token.PhoneNumber = result.PhoneNumber;
                    token.PhoneExt = result.PhoneExt;
                    token.SuccessFlag = true;
                }
                catch (InvalidOperationException e)
                {
                    token.Message = "InvalidOperationException in GetAdjusterByID() " + e.Message;
                }
                catch (Exception ex)
                {
                    token = (DTO_Adjuster)populateException(ex, token);
                }
            }
            return token;
        }
        #endregion

        #region GetAdjustmentsByAdjusterID
        public List<DTO_Adjustment> GetAdjustmentsByAdjusterID(DTO_Adjuster token)
        {
            List<DTO_Adjustment> returnList = new List<DTO_Adjustment>();
            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                try
                {
                    var results = context.proc_GetAdjustmentsByAdjusterID(token.AdjusterID).ToList();

                    foreach (var a in results)
                    {
                        DTO_Adjustment adj = new DTO_Adjustment
                        {
                            AdjusterID = a.AdjusterID,
                            AdjustmentComment = a.AdjustmentComment,
                            AdjustmentDate = a.AdjustmentDate,
                            AdjustmentID = a.AdjustmentID,
                            AdjustmentResultID = a.AdjustmentResultID,
                            ClaimID = a.ClaimID,
                            Exterior = a.Exterior,
                            Gutters = a.Gutters,
                            Interior = a.Interior,
                            SuccessFlag = true
                        };
                        returnList.Add(adj);
                    }
                }
                catch (Exception ex)
                {
                    DTO_Adjustment obj = (DTO_Adjustment)populateException(ex, new DTO_Adjustment());
                    returnList.Add(obj);
                }
            }
            return returnList;
        }
        #endregion

        #region GetAdjustmentsByClaimID
        public List<DTO_Adjustment> GetAdjustmentsByClaimID(DTO_Claim token)
        {
            List<DTO_Adjustment> returnList = new List<DTO_Adjustment>();
            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                try
                {
                    var results = context.proc_GetAdjustmentsByClaimID(token.ClaimID).ToList();

                    foreach (var a in results)
                    {
                        DTO_Adjustment adj = new DTO_Adjustment
                        {
                            AdjusterID = a.AdjusterID,
                            AdjustmentComment = a.AdjustmentComment,
                            AdjustmentDate = a.AdjustmentDate,
                            AdjustmentID = a.AdjustmentID,
                            AdjustmentResultID = a.AdjustmentResultID,
                            ClaimID = a.ClaimID,
                            Exterior = a.Exterior,
                            Gutters = a.Gutters,
                            Interior = a.Interior,
                            SuccessFlag = true
                        };
                        returnList.Add(adj);
                    }
                }
                catch (Exception ex)
                {
                    DTO_Adjustment obj = (DTO_Adjustment)populateException(ex, new DTO_Adjustment());
                    returnList.Add(obj);
                }
            }
            return returnList;
        }
        #endregion

        #region GetCalendarDataByEmployeeID
        public List<DTO_CalendarData> GetCalendarDataByEmployeeID(DTO_Employee token)
        {
            List<DTO_CalendarData> returnList = new List<DTO_CalendarData>();
            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                try
                {
                    var results = context.proc_GetCalendarDataByEmployeeID(token.EmployeeID).ToList();

                    foreach (var entity in results)
                    {
                        DTO_CalendarData cd = new DTO_CalendarData();

                        cd.EntryID = entity.EntryID;
                        cd.AppointmentTypeID = entity.AppointmentTypeID;
                        cd.EmployeeID = entity.EmployeeID;
                        cd.StartTime = entity.StartTime;
                        cd.EndTime = entity.EndTime;
                        cd.Note = entity.Note;
                        cd.ClaimID = entity.ClaimID;
                        cd.LeadID = entity.LeadID;
                        cd.SuccessFlag = true;


                        //if (entity.ClaimID != null) { cd.ClaimID = (int)entity.ClaimID; }
                        //else { cd.ClaimID = 0; }

                        //if (entity.LeadID != null) { cd.LeadID = (int)entity.LeadID; }
                        //else { cd.LeadID = 0; }

                        returnList.Add(cd);
                    }
                }
                catch (Exception ex)
                {
                    DTO_CalendarData obj = (DTO_CalendarData)populateException(ex, new DTO_CalendarData());
                    returnList.Add(obj);
                }
            }
            return returnList;
        }
        #endregion

        #region GetCallLogsByClaimID
        public List<DTO_CallLog> GetCallLogsByClaimID(DTO_Claim token)
        {
            List<DTO_CallLog> returnList = new List<DTO_CallLog>();
            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                try
                {
                    var results = context.proc_GetCallLogsByClaimID(token.ClaimID).ToList();

                    foreach(var l in results)
                    {
                        DTO_CallLog cl = new DTO_CallLog
                        {
                            CallLogID = l.CallLogID,
                            ClaimID = l.ClaimID,
                            CallResults = l.CallResults,
                            ReasonForCall = l.ReasonForCall,
                            WhoWasCalled = l.WhoWasCalled,
                            EmployeeID = l.EmployeeID,
                            WhoAnswered = l.WhoAnswered,
                            SuccessFlag = true

                        };

                        returnList.Add(cl);
                    }
                }
                catch (InvalidOperationException e)
                {
                    token.Message = "InvalidOperationException in GetCallLogsByClaimID() " + e.Message;
                }
                catch (Exception ex)
                {
                    DTO_CallLog obj = (DTO_CallLog)populateException(ex, new DTO_CallLog());
                    returnList.Add(obj);
                }
            }
            return returnList;
        }
        #endregion

        #region GetClaimByClaimID
        public DTO_Claim GetClaimByClaimID(DTO_Claim token)
        {
            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                try
                {
                    var result = context.proc_GetClaimByClaimID(token.ClaimID).Single();
                    token.BillingID = result.BillingID;
                    token.CustomerID = result.CustomerID;
                    token.InsuranceClaimNumber = result.InsuranceClaimNumber;
                    token.InsuranceCompanyID = result.InsuranceCompanyID;
                    token.LeadID = result.LeadID;
                    token.LossDate = result.LossDate;
                    token.MortgageAccount = result.MortgageAccount;
                    token.MortgageCompany = result.MortgageCompany;
                    token.MRNNumber = result.MRNNumber;
                    token.PropertyID = result.PropertyID;
                    token.IsOpen = result.IsOpen;
                    token.ContractSigned = result.ContractSigned;
                    token.SuccessFlag = true;

                }
                catch (InvalidOperationException e)
                {
                    token.Message = "InvalidOperationException in GetClaimByClaimID() " + e.Message;
                }
                catch (Exception ex)
                {
                    token = (DTO_Claim)populateException(ex, token);
                }
            }
            return token;
        }
        #endregion

        #region GetClaimContactsByClaimID
        public DTO_ClaimContacts GetClaimContactsByClaimID(DTO_Claim token)
        {
            DTO_ClaimContacts returnToken = new DTO_ClaimContacts();
            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                try
                {
                    var result = context.proc_GetClaimContactsByClaimID(token.ClaimID).Single();
                    returnToken.ClaimContactID = result.ClaimContactID;
                    returnToken.ClaimID = result.ClaimID;
                    returnToken.CustomerID = result.CustomerID;
                    returnToken.SalesPersonID = result.SalesPersonID;
                    returnToken.SalesManagerID = result.SalesManagerID;
                    returnToken.KnockerID = result.KnockerID;
                    returnToken.SupervisorID = result.SupervisorID;
                    returnToken.AdjusterID = result.AdjusterID;
                    returnToken.SuccessFlag = true;

                    //if (result.KnockerID != null) { returnToken.KnockerID = (int)result.KnockerID; }
                    //else { returnToken.KnockerID = 0; }

                    //if (result.SupervisorID != null) { returnToken.SupervisorID = (int)result.SupervisorID; }
                    //else { returnToken.SupervisorID = 0; }

                    //if (result.InteriorInstallerID != null) { returnToken.InteriorInstallerID = (int)result.InteriorInstallerID; }
                    //else { returnToken.InteriorInstallerID = 0; }

                    //if (result.ExteriorInstallerID != null) { returnToken.ExteriorInstallerID = (int)result.ExteriorInstallerID; }
                    //else { returnToken.ExteriorInstallerID = 0; }

                    //if (result.GutterInstallerID != null) { returnToken.GutterInstallerID = (int)result.GutterInstallerID; }
                    //else { returnToken.GutterInstallerID = 0; }

                    //if (result.RoofInstallerID != null) { returnToken.RoofInstallerID = (int)result.RoofInstallerID; }
                    //else { returnToken.RoofInstallerID = 0; }

                    //if (result.AdjusterID != null) { returnToken.AdjusterID = (int)result.AdjusterID; }
                    //else { returnToken.AdjusterID = 0; }


                }
                catch (InvalidOperationException e)
                {
                    returnToken.Message = "InvalidOperationException in GetClaimContactsByClaimID() " + e.Message;
                }
                catch (Exception ex)
                {
                    returnToken = (DTO_ClaimContacts)populateException(ex, returnToken);
                }
            }
            return returnToken;
        }
        #endregion      

        #region GetClaimStatusByClaimID
        public List<DTO_ClaimStatus> GetClaimStatusByClaimID(DTO_Claim token)
        {
            List<DTO_ClaimStatus> returnList = new List<DTO_ClaimStatus>();
            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                try
                {
                    var result = context.proc_GetClaimStatusByClaimID(token.ClaimID).ToList();

                    foreach(var s in result)
                    {
                        DTO_ClaimStatus cs = new DTO_ClaimStatus{
                            ClaimID = s.ClaimID,
                            ClaimStatusDate = s.ClaimStatusDate,
                            ClaimStatusID = s.ClaimStatusID,
                            ClaimStatusTypeID = s.ClaimStatusTypeID,
                            SuccessFlag = true
                        };

                        returnList.Add(cs);
                    }

                }
                catch (InvalidOperationException e)
                {
                    token.Message = "InvalidOperationException in GetClaimStatusByID() " + e.Message;
                }
                catch (Exception ex)
                {
                    DTO_ClaimStatus obj = (DTO_ClaimStatus)populateException(ex, new DTO_ClaimStatus());
                    returnList.Add(obj);
                }
            }
            return returnList;
        }
        #endregion

        #region GetClaimStatusDateByTypeIDAndClaimID
        public DTO_ClaimStatus GetClaimStatusDateByTypeIDAndClaimID(DTO_ClaimStatus token)
        {
            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                try
                {
                    var result = context.proc_GetClaimStatusDateByTypeIDAndClaimID(token.ClaimStatusTypeID, token.ClaimID).Single();

                    token.ClaimStatusDate = result.ClaimStatusDate;
                    token.ClaimStatusID = result.ClaimStatusID;
                    token.SuccessFlag = true;
                    
                }
                catch (InvalidOperationException e)
                {
                    token.Message = "InvalidOperationException in GetClaimStatusByID() " + e.Message;
                }
                catch (Exception ex)
                {
                    token = (DTO_ClaimStatus)populateException(ex, token);
                }
            }
            return token;
        }
        #endregion

        #region GetClaimVendorsByClaimID
        public List<DTO_ClaimVendor> GetClaimVendorsByClaimID(DTO_Claim token)
        {
            List<DTO_ClaimVendor> returnList = new List<DTO_ClaimVendor>();
            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                try
                {
                    var results = context.proc_GetClaimVendorsByClaimID(token.ClaimID).ToList();

                    foreach (var entity in results)
                    {
                        DTO_ClaimVendor c = new DTO_ClaimVendor
                        {
                            ClaimID = entity.ClaimID,
                            VendorID = entity.VendorID,
                            ServiceTypeID = entity.ServiceTypeID,
                            SuccessFlag = true
                        };
                        returnList.Add(c);
                    }
                }
                catch (Exception ex)
                {
                    DTO_ClaimVendor obj = (DTO_ClaimVendor)populateException(ex, new DTO_ClaimVendor());
                    returnList.Add(obj);
                }
            }
            return returnList;
        }
        #endregion

        #region GetClosedClaimsBySalesPersonID
        public List<DTO_Claim> GetClosedClaimsBySalesPersonID(DTO_Employee token)
        {
            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                List<DTO_Claim> returnList = new List<DTO_Claim>();
                try
                {
                    var results = context.proc_GetClosedClaimsBySalesPersonID(token.EmployeeID).ToList();

                    foreach (var entity in results)
                    {
                        DTO_Claim s = new DTO_Claim
                        {
                            ClaimID = entity.ClaimID,
                            BillingID = entity.BillingID,
                            CustomerID = entity.CustomerID,
                            InsuranceClaimNumber = entity.InsuranceClaimNumber,
                            InsuranceCompanyID = entity.InsuranceCompanyID,
                            LeadID = entity.LeadID,
                            LossDate = entity.LossDate,
                            MortgageAccount = entity.MortgageAccount,
                            MortgageCompany = entity.MortgageCompany,
                            MRNNumber = entity.MRNNumber,
                            PropertyID = entity.PropertyID,
                            IsOpen = entity.IsOpen,
                            ContractSigned = entity.ContractSigned,
                            SuccessFlag = true
                        };

                        returnList.Add(s);
                    }
                }
                catch (Exception ex)
                {
                    DTO_Claim obj = (DTO_Claim)populateException(ex, new DTO_Claim());
                    returnList.Add(obj);
                }

                return returnList;
            }
        }
        #endregion

        #region GetCustomerByID
        public DTO_Customer GetCustomerByID(DTO_Customer token)
        {
            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                try
                {
                    var result = context.proc_GetCustomerByID(token.CustomerID).Single();

                    token.FirstName = result.FirstName;
                    token.LastName = result.LastName;
                    token.PrimaryNumber = result.PrimaryNumber;
                    token.Email = result.Email;
                    token.MailPromos = result.MailPromos;
                    token.SecondaryNumber = result.SecondaryNumber;
                    token.Suffix = result.Suffix;
                    token.MiddleName = result.MiddleName;
                    token.SuccessFlag = true;
                }
                catch (Exception ex)
                {
                    token = (DTO_Customer)populateException(ex, token);
                }
            }
            return token;
        }
        #endregion

        #region GetCustomersBySalesPersonID
        public List<DTO_Customer> GetCustomersBySalesPersonID(DTO_Employee token)
        {
            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                List<DTO_Customer> returnList = new List<DTO_Customer>();
                try
                {
                    var results = context.proc_GetCustomersBySalesPersonID(token.EmployeeTypeID).ToList();

                    foreach (var entity in results)
                    {
                        DTO_Customer customer = new DTO_Customer
                        {
                            CustomerID = entity.CustomerID,
                            FirstName = entity.FirstName,
                            MiddleName = entity.MiddleName,
                            LastName = entity.LastName,
                            Suffix = entity.Suffix,
                            PrimaryNumber = entity.PrimaryNumber,
                            SecondaryNumber = entity.SecondaryNumber,
                            Email = entity.Email,
                            MailPromos = entity.MailPromos,
                            SuccessFlag = true
                        };

                        returnList.Add(customer);
                    }
                }
                catch (InvalidOperationException e)
                {
                    token.Message = "InvalidOperationException in GetCustomersBySalesPersonID() " + e.Message;
                }
                catch (Exception ex)
                {
                    DTO_Customer obj = (DTO_Customer)populateException(ex, new DTO_Customer());
                    returnList.Add(obj);
                }

                return returnList;
            }
        }
        #endregion

        #region GetEmployeesByEmployeeType
        public List<DTO_Employee> GetEmployeesByEmployeeTypeID(DTO_LU_EmployeeType token)
        {
            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                List<DTO_Employee> returnList = new List<DTO_Employee>();
                try
                {
                    var results = context.proc_GetEmployeesByTypeID(token.EmployeeTypeID).ToList();

                    foreach (var entity in results)
                    {
                        DTO_Employee emp = new DTO_Employee();
                        emp.EmployeeID = entity.EmployeeID;
                        emp.EmployeeTypeID = entity.EmployeeTypeID;
                        emp.FirstName = entity.FirstName;
                        emp.LastName = entity.LastName;
                        emp.Suffix = entity.Suffix;
                        emp.Email = entity.Email;
                        emp.CellPhone = entity.CellPhone;
                        emp.Active = entity.Active;
                        emp.SuccessFlag = true;

                        returnList.Add(emp);
                    }
                }
                catch (InvalidOperationException e)
                {
                    token.Message = "InvalidOperationException in GetEmployeeByEmployeeType() " + e.Message;
                }
                catch (Exception ex)
                {
                    DTO_Employee obj = (DTO_Employee)populateException(ex, new DTO_Employee());
                    returnList.Add(obj);
                }

                return returnList;
            }
        }
        #endregion

        #region GetEmployeeByID
        public DTO_Employee GetEmployeeByID(DTO_Employee token)
        {
            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                try
                {
                    var result = context.proc_GetEmployeeByID(token.EmployeeID).Single();

                    token.EmployeeTypeID = result.EmployeeTypeID;
                    token.FirstName = result.FirstName;
                    token.LastName = result.LastName;
                    token.Suffix = result.Suffix;
                    token.Email = result.Email;
                    token.CellPhone = result.CellPhone;
                    token.Active = result.Active;
                    token.SuccessFlag = true;
                }
                catch (Exception ex)
                {
                    token = (DTO_Employee)populateException(ex, token);
                }
            }
            return token;
        }
        #endregion

        #region GetInactiveClaimsBySalesPersonID
        public List<DTO_Claim> GetInactiveClaimsBySalesPersonID(DTO_Employee token)
        {
            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                List<DTO_Claim> returnList = new List<DTO_Claim>();
                try
                {
                    var results = context.proc_GetInactiveClaimsBySalesPersonID(token.EmployeeID).ToList();

                    foreach (var entity in results)
                    {
                        DTO_Claim s = new DTO_Claim
                        {
                            ClaimID = entity.ClaimID,
                            BillingID = entity.BillingID,
                            CustomerID = entity.CustomerID,
                            InsuranceClaimNumber = entity.InsuranceClaimNumber,
                            InsuranceCompanyID = entity.InsuranceCompanyID,
                            LeadID = entity.LeadID,
                            LossDate = entity.LossDate,
                            MortgageAccount = entity.MortgageAccount,
                            MortgageCompany = entity.MortgageCompany,
                            MRNNumber = entity.MRNNumber,
                            PropertyID = entity.PropertyID,
                            IsOpen = entity.IsOpen,
                            ContractSigned = entity.ContractSigned,
                            SuccessFlag = true
                        };

                        returnList.Add(s);
                    }
                }
                catch (Exception ex)
                {
                    DTO_Claim obj = (DTO_Claim)populateException(ex, new DTO_Claim());
                    returnList.Add(obj);
                }

                return returnList;
            }
        }
        #endregion

        #region GetInspectionByID
        public DTO_Inspection GetInspectionByID(DTO_Inspection token)
        {
            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                try
                {
                    var result = context.proc_GetInspectionByID(token.InspectionID).Single();

                    token.ClaimID = result.ClaimID;
                    token.Comments = result.Comments;
                    token.CoverPool = result.CoverPool;
                    token.DrivewayDamage = result.DrivewayDamage;
                    token.EmergencyRepair = result.EmergencyRepair;
                    token.EmergencyRepairAmount = result.EmergencyRepairAmount;
                    token.ExteriorDamage = result.ExteriorDamage;
                    token.FurnishPermit = result.FurnishPermit;
                    token.GutterDamage = result.GutterDamage;
                    token.IceWaterShield = result.IceWaterShield;
                    token.InspectionDate = result.InspectionDate;
                    token.InteriorDamage = result.InteriorDamage;
                    token.Leaks = result.Leaks;
                    token.MagneticRollers = result.MagneticRollers;
                    token.ProtectLandscaping = result.ProtectLandscaping;
                    token.QualityControl = result.QualityControl;
                    token.RemoveTrash = result.RemoveTrash;
                    token.RidgeMaterialTypeID = result.RidgeMaterialTypeID;
                    token.RoofAge = result.RoofAge;
                    token.Satellite = result.Satellite;
                    token.ShingleTypeID = result.ShingleTypeID;
                    token.SkyLights = result.SkyLights;
                    token.SolarPanels = result.SolarPanels;
                    token.TearOff = result.TearOff;
                    token.LighteningProtection = result.LightningProtection;
                    token.SuccessFlag = true;

                }
                catch (InvalidOperationException e)
                {
                    token.Message = "InvalidOperationException in GetAddressByID() " + e.Message;
                }
                catch (Exception ex)
                {
                    token = (DTO_Inspection)populateException(ex, token);
                }
            }
            return token;
        }
        #endregion

        #region GetInspectionsByClaimID
        public List<DTO_Inspection> GetInspectionsByClaimID(DTO_Claim token)
        {
            List<DTO_Inspection> returnList = new List<DTO_Inspection>();
            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                try
                {
                    var results = context.proc_GetInspectionsByClaimID(token.ClaimID).ToList();

                    foreach (var i in results)
                    {
                        DTO_Inspection inv = new DTO_Inspection
                        {
                            ClaimID = i.ClaimID,
                            Comments = i.Comments,
                            CoverPool = i.CoverPool,
                            DrivewayDamage = i.DrivewayDamage,
                            EmergencyRepair = i.EmergencyRepair,
                            EmergencyRepairAmount = i.EmergencyRepairAmount,
                            ExteriorDamage = i.ExteriorDamage,
                            FurnishPermit = i.FurnishPermit,
                            GutterDamage = i.GutterDamage,
                            IceWaterShield = i.IceWaterShield,
                            InspectionDate = i.InspectionDate,
                            InteriorDamage = i.InteriorDamage,
                            InspectionID = i.InspectionID,
                            Leaks = i.Leaks,
                            MagneticRollers = i.MagneticRollers,
                            ProtectLandscaping = i.ProtectLandscaping,
                            QualityControl = i.QualityControl,
                            RemoveTrash = i.RemoveTrash,
                            RidgeMaterialTypeID = i.RidgeMaterialTypeID,
                            RoofAge = i.RoofAge,
                            Satellite = i.Satellite,
                            ShingleTypeID = i.ShingleTypeID,
                            SkyLights = i.SkyLights,
                            SolarPanels = i.SolarPanels,
                            TearOff = i.TearOff,
                            LighteningProtection = i.LightningProtection,
                            SuccessFlag = true
                        };
                        returnList.Add(inv);
                    }
                }
                catch (Exception ex)
                {
                    DTO_Inspection obj = (DTO_Inspection)populateException(ex, new DTO_Inspection());
                    returnList.Add(obj);
                }
            }
            return returnList;
        }
        #endregion

        #region GetInsuranceCompanyByID
        public DTO_InsuranceCompany GetInsuranceCompanyByID(DTO_InsuranceCompany token)
        {
            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                try
                {
                    var result = context.proc_GetInsuranceCompanyByID(token.InsuranceCompanyID).Single();

                    token.Address = result.Address;
                    token.ClaimPhoneExt = result.ClaimPhoneExt;
                    token.ClaimPhoneNumber = result.ClaimPhoneNumber;
                    token.CompanyName = result.CompanyName;
                    token.Email = result.Email;
                    token.FaxExt = result.FaxExt;
                    token.FaxNumber = result.FaxNumber;
                    token.Independent = result.Independent;
                    token.Zip = result.Zip;
                    token.SuccessFlag = true;
                }
                catch (InvalidOperationException e)
                {
                    token.Message = "InvalidOperationException in GetInsuranceCompanyByID() " + e.Message;
                }
                catch (Exception ex)
                {
                    token = (DTO_InsuranceCompany)populateException(ex, token);
                }
            }
            return token;
        }
        #endregion

        #region GetInvoicesByClaimID
        public List<DTO_Invoice> GetInvoicesByClaimID(DTO_Claim token)
        {
            List<DTO_Invoice> returnList = new List<DTO_Invoice>();
            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                try
                {
                    var results = context.proc_GetInvoicesByClaimID(token.ClaimID).ToList();

                    foreach (var i in results)
                    {
                        DTO_Invoice inv = new DTO_Invoice
                        {
                            InvoiceID = i.InvoiceID,
                            InvoiceTypeID = i.InvoiceTypeID,
                            InvoiceDate = i.InvoiceDate,
                            ClaimID = i.ClaimID,
                            InvoiceAmount = i.InvoiceAmount,
                            Paid = i.Paid,
                            SuccessFlag = true
                        };
                        returnList.Add(inv);
                    }
                }
                catch (Exception ex)
                {
                    DTO_Invoice obj = (DTO_Invoice)populateException(ex, new DTO_Invoice());
                    returnList.Add(obj);
                }
            }
            return returnList;
        }
        #endregion

        #region GetKnockerResponseByID
        public DTO_KnockerResponse GetKnockerResponseByID(DTO_KnockerResponse token)
        {
            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                try
                {
                    var result = context.proc_GetKnockerResponseByID(token.KnockerResponseID).Single();

                    token.KnockerID = result.KnockerID;
                    token.KnockResponseTypeID = result.KnockResponseTypeID;
                    token.Address = result.Address;
                    token.Latitude = result.Lat;
                    token.Longitude = result.Long;
                    token.Zip = result.Zip;
                    token.SuccessFlag = true;
                }
                catch (Exception ex)
                {
                    token = (DTO_KnockerResponse)populateException(ex, token);
                }
            }
            return token;
        }
        #endregion

        #region GetKnockerResponsesByKnockerID
        public List<DTO_KnockerResponse> GetKnockerResponsesByKnockerID(DTO_Employee token)
        {

            List<DTO_KnockerResponse> returnList = new List<DTO_KnockerResponse>();
            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                try
                {
                    var results = context.proc_GetKnockerResponsesByKnockerID(token.EmployeeID).ToList();

                    foreach(var k in results)
                    {
                        DTO_KnockerResponse kr = new DTO_KnockerResponse
                        {
                            Address = k.Address,
                            KnockerID = k.KnockerID,
                            KnockerResponseID = k.KnockerResponseID,
                            KnockResponseTypeID = k.KnockResponseTypeID,
                            Latitude = k.Lat,
                            Longitude = k.Long,
                            Zip = k.Zip,
                            SuccessFlag = true
                        };

                        returnList.Add(kr);
                    }
                }
                catch (Exception ex)
                {
                    DTO_KnockerResponse obj = (DTO_KnockerResponse)populateException(ex, new DTO_KnockerResponse());
                    returnList.Add(obj);
                }
            }
            return returnList;
        }
        #endregion

        #region GetKnockerResponsesByTypeID
        public List<DTO_KnockerResponse> GetKnockerResponsesByTypeID(DTO_LU_KnockResponseType token)
        {

            List<DTO_KnockerResponse> returnList = new List<DTO_KnockerResponse>();
            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                try
                {
                    var results = context.proc_GetKnockerResponsesByTypeID(token.KnockResponseTypeID).ToList();

                    foreach (var k in results)
                    {
                        DTO_KnockerResponse kr = new DTO_KnockerResponse
                        {
                            Address = k.Address,
                            KnockerID = k.KnockerID,
                            KnockerResponseID = k.KnockerResponseID,
                            KnockResponseTypeID = k.KnockResponseTypeID,
                            Latitude = k.Lat,
                            Longitude = k.Long,
                            Zip = k.Zip,
                            SuccessFlag = true
                        };

                        returnList.Add(kr);
                    }
                }
                catch (Exception ex)
                {
                    DTO_KnockerResponse obj = (DTO_KnockerResponse)populateException(ex, new DTO_KnockerResponse());
                    returnList.Add(obj);
                }
            }
            return returnList;
        }
        #endregion

        #region GetLeadByLeadID
        public DTO_Lead GetLeadByLeadID(DTO_Lead token)
        {
            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                try
                {
                    var result = context.proc_GetLeadByLeadID(token.LeadID).Single();

                    token.LeadTypeID = result.LeadTypeID;
                    token.CustomerID = result.CustomerID;
                    token.AddressID = result.AddressID;
                    token.LeadDate = result.LeadDate;
                    token.Status = result.Status;
                    token.CreditToID = result.CreditToID;
                    token.Temperature = result.Temperature;
                    token.SuccessFlag = true;

                }
                catch (Exception ex)
                {
                    token = (DTO_Lead)populateException(ex, token);
                }
            }
            return token;
        }
        #endregion

        #region GetLeadsBySalesPersonID
        public List<DTO_Lead> GetLeadsBySalesPersonID(DTO_Employee token)
        {
            MRNNexusTestEntities context = new MRNNexusTestEntities();

            List<DTO_Lead> returnList = new List<DTO_Lead>();

            try
            {
                var results = context.proc_GetLeadsBySalesPersonID(token.EmployeeID).ToList();

                foreach (var entity in results)
                {
                    returnList.Add(new DTO_Lead
                    {
                        LeadID = entity.LeadID,
                        LeadTypeID = entity.LeadTypeID,
                        KnockerResponseID = entity.KnockerResponseID,
                        SalesPersonID = entity.SalesPersonID,
                        CustomerID = entity.CustomerID,
                        AddressID = entity.AddressID,
                        LeadDate = entity.LeadDate,
                        Status = entity.Status,
                        CreditToID = entity.CreditToID,
                        Temperature = entity.Temperature,
                        SuccessFlag = true
                    });

                }
                
            }
            catch (Exception ex)
            {
                DTO_Lead obj = (DTO_Lead)populateException(ex, new DTO_Lead());
                returnList.Add(obj);

            }

            return returnList;
        }
        #endregion

        #region GetLeadsByStatus
        public List<DTO_Lead> GetLeadsByStatus(DTO_Lead token)
        {
            MRNNexusTestEntities context = new MRNNexusTestEntities();

            List<DTO_Lead> returnList = new List<DTO_Lead>();

            try
            {
                var results = context.proc_GetLeadsByStatus(token.Status, token.SalesPersonID).ToList();

                foreach (var entity in results)
                {
                    returnList.Add(new DTO_Lead
                    {
                        LeadID = entity.LeadID,
                        LeadTypeID = entity.LeadTypeID,
                        KnockerResponseID = entity.KnockerResponseID,
                        SalesPersonID = entity.SalesPersonID,
                        CustomerID = entity.CustomerID,
                        AddressID = entity.AddressID,
                        LeadDate = entity.LeadDate,
                        Status = entity.Status,
                        CreditToID = entity.CreditToID,
                        Temperature = entity.Temperature,
                        SuccessFlag = true
                    });

                }

            }
            catch (Exception ex)
            {
                DTO_Lead obj = (DTO_Lead)populateException(ex, new DTO_Lead());
                returnList.Add(obj);
            }

            return returnList;
        }
        #endregion

        #region GetLeadsWithNoClaim
        public List<DTO_Lead> GetLeadsWithNoClaim()
        {
            MRNNexusTestEntities context = new MRNNexusTestEntities();

            List<DTO_Lead> returnList = new List<DTO_Lead>();

            try
            {
                var results = context.proc_GetLeadsWithNoClaim().ToList();

                foreach (var entity in results)
                {
                    returnList.Add(new DTO_Lead
                    {
                        LeadID = entity.LeadID,
                        LeadTypeID = entity.LeadTypeID,
                        KnockerResponseID = entity.KnockerResponseID,
                        SalesPersonID = entity.SalesPersonID,
                        CustomerID = entity.CustomerID,
                        AddressID = entity.AddressID,
                        LeadDate = entity.LeadDate,
                        Status = entity.Status,
                        CreditToID = entity.CreditToID,
                        Temperature = entity.Temperature,
                        SuccessFlag = true
                    });
                }
            }
            catch (Exception ex)
            {
                DTO_Lead obj = (DTO_Lead)populateException(ex, new DTO_Lead());
                returnList.Add(obj);
            }

            return returnList;
        }
        #endregion

        #region GetMostRecentDateByClaimID
        public DTO_CalculatedResults GetMostRecentDateByClaimID(DTO_Claim token)
        {
            DTO_CalculatedResults returnToken = new DTO_CalculatedResults();
            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                try
                {
                    var outputParameter = new ObjectParameter("MostRecentDate", typeof(DateTime));
                    var result = context.proc_GetMostRecentDateByClaimID(token.ClaimID, outputParameter);

                    returnToken.MostRecentDate = (DateTime)outputParameter.Value;
                    returnToken.SuccessFlag = true;
                }
                catch (Exception ex)
                {
                    returnToken = (DTO_CalculatedResults)populateException(ex, returnToken);
                }
            }
            return returnToken;
        }
        #endregion

        #region GetNewRoofByClaimID
        public DTO_NewRoof GetNewRoofByClaimID(DTO_Claim token)
        {
            DTO_NewRoof returnToken = new DTO_NewRoof();
            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                try
                {
                    var result = context.proc_GetNewRoofByClaimID(token.ClaimID).Single();

                    returnToken.ClaimID = result.ClaimID;
                    returnToken.Comments = result.Comments;
                    returnToken.NewRoofID = result.NewRoofID;
                    returnToken.ProductID = result.ProductID;
                    returnToken.UpgradeCost = result.UpgradeCost;
                    returnToken.SuccessFlag = true;
                }
                catch (Exception ex)
                {
                    returnToken = (DTO_NewRoof)populateException(ex, returnToken);
                }
            }
            return returnToken;
        }
        #endregion

        #region GetOldLeadsBySalesPersonID
        public List<DTO_Lead> GetOldLeadsBySalesPersonID(DTO_Lead token)
        {
            MRNNexusTestEntities context = new MRNNexusTestEntities();

            List<DTO_Lead> returnList = new List<DTO_Lead>();

            try
            {
                var results = context.proc_GetOldLeadsBySalesPersonID(token.NumberOfDays, token.SalesPersonID).ToList();

                foreach (var entity in results)
                {
                    returnList.Add(new DTO_Lead
                    {
                        LeadID = entity.LeadID,
                        LeadTypeID = entity.LeadTypeID,
                        KnockerResponseID = entity.KnockerResponseID,
                        SalesPersonID = entity.SalesPersonID,
                        CustomerID = entity.CustomerID,
                        AddressID = entity.AddressID,
                        LeadDate = entity.LeadDate,
                        Status = entity.Status,
                        CreditToID = entity.CreditToID,
                        Temperature = entity.Temperature,
                        SuccessFlag = true
                    });

                }

            }
            catch (Exception ex)
            {
                DTO_Lead obj = (DTO_Lead)populateException(ex, new DTO_Lead());
                returnList.Add(obj);

            }

            return returnList;
        }
        #endregion

        #region GetOpenClaimsBySalespersonID
        public List<DTO_Claim> GetOpenClaimsBySalesPersonID(DTO_Employee token)
        {
            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                List<DTO_Claim> returnList = new List<DTO_Claim>();
                try
                {
                    var results = context.proc_GetOpenClaimsBySalesPersonID(token.EmployeeID).ToList();

                    foreach (var entity in results)
                    {
                        DTO_Claim s = new DTO_Claim
                        {
                            ClaimID = entity.ClaimID,
                            BillingID = entity.BillingID,
                            CustomerID = entity.CustomerID,
                            InsuranceClaimNumber = entity.InsuranceClaimNumber,
                            InsuranceCompanyID = entity.InsuranceCompanyID,
                            LeadID = entity.LeadID,
                            LossDate = entity.LossDate,
                            MortgageAccount = entity.MortgageAccount,
                            MortgageCompany = entity.MortgageCompany,
                            MRNNumber = entity.MRNNumber,
                            PropertyID = entity.PropertyID,
                            IsOpen = entity.IsOpen,
                            ContractSigned = entity.ContractSigned,
                            SuccessFlag = true
                        };

                        returnList.Add(s);
                    }
                }
                catch (Exception ex)
                {
                    DTO_Claim obj = (DTO_Claim)populateException(ex, new DTO_Claim());
                    returnList.Add(obj);
                }

                return returnList;
            }
        }
        #endregion

        #region GetOrderItemsByOrderID
        public List<DTO_OrderItem> GetOrderItemsByOrderID(DTO_Order token)
        {
            List<DTO_OrderItem> returnList = new List<DTO_OrderItem>();
            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                try
                {
                    var results = context.proc_GetOrderItemsByOrderID(token.OrderID).ToList();

                    foreach (var i in results)
                    {
                        DTO_OrderItem oi = new DTO_OrderItem
                        {
                            OrderID = i.OrderID,
                            OrderItemID = i.OrderItemID,
                            ProductID = i.ProductID,
                            Quantity = i.Quantity,
                            UnitOfMeasureID = i.UnitOfMeasureID,
                            SuccessFlag = true
                        };
                        returnList.Add(oi);
                    }
                }
                catch (Exception ex)
                {
                    DTO_OrderItem obj = (DTO_OrderItem)populateException(ex, new DTO_OrderItem());
                    returnList.Add(obj);
                }
            }
            return returnList;
        }
        #endregion

        #region GetOrdersByClaimID
        public List<DTO_Order> GetOrdersByClaimID(DTO_Claim token)
        {
            List<DTO_Order> returnList = new List<DTO_Order>();
            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                try
                {
                    var results = context.proc_GetOrdersByClaimID(token.ClaimID).ToList();

                    foreach (var o in results)
                    {
                        DTO_Order ord = new DTO_Order
                        {
                            ClaimID = o.ClaimID,
                            DateDropped = o.DateDropped,
                            DateOrdered = o.DateOrdered,
                            OrderID = o.OrderID,
                            ScheduledInstallation = o.ScheduledInstallation,
                            VendorID = o.VendorID,
                            SuccessFlag = true
                        };
                        returnList.Add(ord);
                    }
                }
                catch (Exception ex)
                {
                    DTO_Order obj = (DTO_Order)populateException(ex, new DTO_Order());
                    returnList.Add(obj);
                }
            }
            return returnList;
        }
        #endregion

        #region GetOtherClaimsToSchedule
        public List<DTO_Scope> GetOtherClaimsToSchedule()
        {
            List<DTO_Scope> returnList = new List<DTO_Scope>();
            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                try
                {
                    var results = context.proc_GetOtherClaimsToSchedule().ToList();

                    foreach (var s in results)
                    {
                        DTO_Scope scope = new DTO_Scope
                        {
                            ClaimID = s.ClaimID,
                            Deductible = s.Deductible,
                            Exterior = s.Exterior,
                            Gutter = s.Gutter,
                            Interior = s.Interior,
                            OandP = s.OandP,
                            ScopeID = s.ScopeID,
                            ScopeTypeID = s.ScopeTypeID,
                            Tax = s.Tax,
                            Total = s.Total,
                            RoofAmount = s.RoofAmount,
                            Accepted = s.Accepted,
                            SuccessFlag = true
                        };
                        returnList.Add(scope);
                    }
                }
                catch (Exception ex)
                {
                    DTO_Scope obj = (DTO_Scope)populateException(ex, new DTO_Scope());
                    returnList.Add(obj);
                }
            }
            return returnList;
        }
        #endregion

        #region GetPaymentsByClaimID
        public List<DTO_Payment> GetPaymentsByClaimID(DTO_Claim token)
        {
            List<DTO_Payment> returnList = new List<DTO_Payment>();
            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                try
                {
                    var results = context.proc_GetPaymentsByClaimID(token.ClaimID).ToList();

                    foreach (var p in results)
                    {
                        DTO_Payment pay = new DTO_Payment
                        {
                            PaymentID = p.PaymentID,
                            PaymentTypeID = p.PaymentTypeID,
                            PaymentDescriptionID = p.PaymentDescriptionID,
                            PaymentDate = p.PaymentDate,
                            ClaimID = p.ClaimID,
                            Amount = p.Amount,
                            SuccessFlag = true
                        };
                        returnList.Add(pay);
                    }
                }
                catch (Exception ex)
                {
                    DTO_Payment obj = (DTO_Payment)populateException(ex, new DTO_Payment());
                    returnList.Add(obj);
                }
            }
            return returnList;
        }
        #endregion

        #region GetPlanesByInspectionID
        public List<DTO_Plane> GetPlanesByInspectionID(DTO_Inspection token)
        {
            List<DTO_Plane> returnList = new List<DTO_Plane>();
            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                try
                {
                    var results = context.proc_GetPlanesByInspectionID(token.InspectionID).ToList();

                    foreach (var p in results)
                    {
                        DTO_Plane pay = new DTO_Plane
                        {
                            PlaneTypeID = p.PlaneTypeID,
                            ItemSpec = p.ItemSpec,
                            EaveHeight = p.EaveHeight,
                            EaveLength = p.EaveLength,
                            FourAndUp = p.FourAndUp,
                            GroupNumber = p.GroupNumber,
                            NumberDecking = p.NumberDecking,
                            PlaneID = p.PlaneID,
                            Pitch = p.Pitch,
                            NumOfLayers = p.NumOfLayers,
                            ThreeAndOne = p.ThreeAndOne,
                            HipValley = p.HipValley,
                            RakeLength = p.RakeLength,
                            RidgeLength = p.RidgeLength,
                            SquareFootage = p.SquareFootage,
                            StepFlashing = p.StepFlashing,
                            SuccessFlag = true
                        };
                        returnList.Add(pay);
                    }
                }
                catch (Exception ex)
                {
                    DTO_Plane obj = (DTO_Plane)populateException(ex, new DTO_Plane());
                    returnList.Add(obj);
                }
            }
            return returnList;
        }
        #endregion

        #region GetReferrerByID
        public DTO_Referrer GetReferrerByID(DTO_Referrer token)
        {
            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                try
                {
                    var result = context.proc_GetReferrerByID(token.ReferrerID).Single();

                    token.ReferrerID = result.ReferrerID;
                    token.CellPhone = result.CellPhone;
                    token.Email = result.Email;
                    token.FirstName = result.FirstName;
                    token.LastName = result.LastName;
                    token.MailingAddress = result.MailingAddress;
                    token.Zip = result.Zip;
                    token.Suffix = result.Suffix;
                    token.SuccessFlag = true;
                }
                catch (Exception ex)
                {
                    token = (DTO_Referrer)populateException(ex, token);
                }
            }
            return token;
        }
        #endregion

        #region GetRoofClaimsToSchedule
        public List<DTO_Scope> GetRoofClaimsToSchedule()
        {
            List<DTO_Scope> returnList = new List<DTO_Scope>();
            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                try
                {
                    var results = context.proc_GetRoofClaimsToSchedule().ToList();

                    foreach (var s in results)
                    {
                        DTO_Scope scope = new DTO_Scope
                        {
                            ClaimID = s.ClaimID,
                            Deductible = s.Deductible,
                            Exterior = s.Exterior,
                            Gutter = s.Gutter,
                            Interior = s.Interior,
                            OandP = s.OandP,
                            ScopeID = s.ScopeID,
                            ScopeTypeID = s.ScopeTypeID,
                            Tax = s.Tax,
                            Total = s.Total,
                            RoofAmount = s.RoofAmount,
                            Accepted = s.Accepted,
                            SuccessFlag = true
                        };
                        returnList.Add(scope);
                    }
                }
                catch (Exception ex)
                {
                    DTO_Scope obj = (DTO_Scope)populateException(ex, new DTO_Scope());
                    returnList.Add(obj);
                }
            }
            return returnList;
        }
        #endregion

        #region GetScopesByClaimID
        public List<DTO_Scope> GetScopesByClaimID(DTO_Claim token)
        {
            List<DTO_Scope> returnList = new List<DTO_Scope>();
            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                try
                {
                    var results = context.proc_GetScopesByClaimID(token.ClaimID).ToList();

                    foreach (var s in results)
                    {
                        DTO_Scope scope = new DTO_Scope
                        {
                            ClaimID = s.ClaimID,
                            Deductible = s.Deductible,
                            Exterior = s.Exterior,
                            Gutter = s.Gutter,
                            Interior = s.Interior,
                            OandP = s.OandP,
                            ScopeID = s.ScopeID,
                            ScopeTypeID = s.ScopeTypeID,
                            Tax = s.Tax,
                            Total = s.Total,
                            RoofAmount = s.RoofAmount,
                            Accepted = s.Accepted,
                            SuccessFlag = true
                        };
                        returnList.Add(scope);
                    }
                }
                catch (Exception ex)
                {
                    DTO_Scope obj = (DTO_Scope)populateException(ex, new DTO_Scope());
                    returnList.Add(obj);
                }
            }
            return returnList;
        }
        #endregion

        #region GetSumOfInvoicesByClaimID
        public DTO_CalculatedResults GetSumOfInvoicesByClaimID(DTO_Claim token)
        {
            DTO_CalculatedResults returnToken = new DTO_CalculatedResults();
            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                try
                {
                    var outputParameter = new ObjectParameter("result", typeof(double));
                    var result = context.proc_GetSumOfInvoicesByClaimID(token.ClaimID, outputParameter);

                    returnToken.SumOfInvoices = (double)outputParameter.Value;
                    returnToken.SuccessFlag = true;

                }
                catch (Exception ex)
                {
                    returnToken = (DTO_CalculatedResults)populateException(ex, returnToken);
                }
            }
            return returnToken;
        }
        #endregion

        #region GetSumOfPaymentsByClaimID
        public DTO_CalculatedResults GetSumOfPaymentsByClaimID(DTO_Claim token)
        {
            DTO_CalculatedResults returnToken = new DTO_CalculatedResults();
            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                try
                {
                    var outputParameter = new ObjectParameter("result", typeof(double));
                    var result = context.proc_GetSumOfPaymentsByClaimID(token.ClaimID, outputParameter);
                    
                    returnToken.SumOfPayments = (double)outputParameter.Value;
                    returnToken.SuccessFlag = true;
                }
                catch (Exception ex)
                {
                    returnToken = (DTO_CalculatedResults)populateException(ex, returnToken);
                }
            }
            return returnToken;
        }
        #endregion

        #region GetSurplusSuppliesByClaimID
        public List<DTO_SurplusSupplies> GetSurplusSuppliesByClaimID(DTO_Claim token)
        {
            List<DTO_SurplusSupplies> returnList = new List<DTO_SurplusSupplies>();
            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                try
                {
                    var results = context.proc_GetSurplusSuppliesByClaimID(token.ClaimID).ToList();

                    foreach (var s in results)
                    {
                        DTO_SurplusSupplies ss = new DTO_SurplusSupplies
                        {
                            ClaimID = s.ClaimID,
                            DropOffDate = s.DropOffDate,
                            Items = s.Items,
                            PickUpDate = s.PickUpDate,
                            Quantity = s.Quantity,
                            SurplusSuppliesID = s.SurplusSuppliesID,
                            UnitOfMeasureID = s.UnitOfMeasureID,
                            SuccessFlag = true

                        };
                        returnList.Add(ss);
                    }
                }
                catch (Exception ex)
                {
                    DTO_SurplusSupplies obj = (DTO_SurplusSupplies)populateException(ex, new DTO_SurplusSupplies());
                    returnList.Add(obj);
                }
            }
            return returnList;
        }
        #endregion

        #region GetVendorByID
        public DTO_Vendor GetVendorByID(DTO_Vendor token)
        {
            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                try
                {
                    var result = context.proc_GetVendorByID(token.VendorID).Single();

                    token.VendorID = result.VendorID;
                    token.VendorTypeID = result.VendorTypeID;
                    token.CompanyName = result.CompanyName;
                    token.EIN = result.EIN;
                    token.ContactFirstName = result.ContactFirstName;
                    token.ContactLastName = result.ContactLastName;
                    token.Suffix = result.Suffix;
                    token.VendorAddress = result.VendorAddress;
                    token.Zip = result.Zip;
                    token.Phone = result.Phone;
                    token.CompanyPhone = result.CompanyPhone;
                    token.Fax = result.Fax;
                    token.Email = result.Email;
                    token.Website = result.Website;
                    token.GeneralLiabilityExpiration = result.GeneralLiabilityExpiration;
                    token.SuccessFlag = true;
                }
                catch (Exception ex)
                {
                    token = (DTO_Vendor)populateException(ex, token);
                }
            }
            return token;
        }
        #endregion

        #endregion GETS

        #region GET ALLS

        #region GetAllAdditionalSupplies
        public List<DTO_AdditionalSupply> GetAllAdditionalSupplies()
        {
            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                List<DTO_AdditionalSupply> returnList = new List<DTO_AdditionalSupply>();
                try
                {
                    var results = context.proc_GetAllAdditionalSupplies().ToList();

                    foreach (var entity in results)
                    {
                        DTO_AdditionalSupply s = new DTO_AdditionalSupply
                        {
                            AdditionalSuppliesID = entity.AdditionalSuppliesID,
                            ClaimID = entity.ClaimID,
                            Cost = entity.Cost,
                            DropOffDate = entity.DropOffDate,
                            PickUpDate = entity.PickUpDate,
                            Items = entity.Items,
                            ReceiptImagePath = entity.ReceiptImagePath,
                            SuccessFlag = true
                        };

                        returnList.Add(s);
                    }
                }
                catch (Exception ex)
                {
                    DTO_AdditionalSupply obj = (DTO_AdditionalSupply)populateException(ex, new DTO_AdditionalSupply());
                    returnList.Add(obj);
                }

                return returnList;
            }
        }
        #endregion

        #region GetAllAddresses
        public List<DTO_Address> GetAllAddresses()
        {
            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                List<DTO_Address> returnList = new List<DTO_Address>();
                try
                {
                    var results = context.proc_GetAllAddresses().ToList();

                    foreach (var entity in results)
                    {
                        DTO_Address s = new DTO_Address
                        {
                            Address = entity.Address,
                            AddressID = entity.AddressID,
                            CustomerID = entity.CustomerID,
                            Zip = entity.Zip,
                            SuccessFlag = true
                        };

                        returnList.Add(s);
                    }
                }
                catch (Exception ex)
                {
                    DTO_Address obj = (DTO_Address)populateException(ex, new DTO_Address());
                    returnList.Add(obj);
                }

                return returnList;
            }
        }
        #endregion

        #region GetAllAdjusters
        public List<DTO_Adjuster> GetAllAdjusters()
        {
            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                List<DTO_Adjuster> returnList = new List<DTO_Adjuster>();
                try
                {
                    var results = context.proc_GetAllAdjusters().ToList();

                    foreach (var entity in results)
                    {
                        DTO_Adjuster s = new DTO_Adjuster
                        {
                            AdjusterID = entity.AdjusterID,
                            Comments = entity.Comments,
                            Email = entity.Email,
                            FirstName = entity.FirstName,
                            LastName = entity.LastName,
                            InsuranceCompanyID = entity.InsuranceCompanyID,
                            PhoneExt = entity.PhoneExt,
                            PhoneNumber = entity.PhoneNumber,
                            Suffix = entity.Suffix,
                            SuccessFlag = true
                        };

                        returnList.Add(s);
                    }
                }
                catch (Exception ex)
                {
                    DTO_Adjuster obj = (DTO_Adjuster)populateException(ex, new DTO_Adjuster());
                    returnList.Add(obj);
                }

                return returnList;
            }
        }
        #endregion

        #region GetAllAdjustments
        public List<DTO_Adjustment> GetAllAdjustments()
        {
            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                List<DTO_Adjustment> returnList = new List<DTO_Adjustment>();
                try
                {
                    var results = context.proc_GetAllAdjustments().ToList();

                    foreach (var entity in results)
                    {
                        DTO_Adjustment s = new DTO_Adjustment
                        {
                            AdjusterID = entity.AdjusterID,
                            AdjustmentComment = entity.AdjustmentComment,
                            AdjustmentDate = entity.AdjustmentDate,
                            AdjustmentID = entity.AdjustmentID,
                            AdjustmentResultID = entity.AdjustmentResultID,
                            ClaimID = entity.ClaimID,
                            Exterior = entity.Exterior,
                            Gutters = entity.Gutters,
                            Interior = entity.Interior,
                            SuccessFlag = true
                        };

                        returnList.Add(s);
                    }
                }
                catch (Exception ex)
                {
                    DTO_Adjustment obj = (DTO_Adjustment)populateException(ex, new DTO_Adjustment());
                    returnList.Add(obj);
                }

                return returnList;
            }
        }
        #endregion

        #region GetAllCalendarData
        public List<DTO_CalendarData> GetAllCalendarData()
        {
            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                List<DTO_CalendarData> returnList = new List<DTO_CalendarData>();
                try
                {
                    var results = context.proc_GetAllCalendarData().ToList();

                    foreach (var entity in results)
                    {
                        DTO_CalendarData s = new DTO_CalendarData
                        {
                            AppointmentTypeID = entity.AppointmentTypeID,
                            ClaimID = entity.ClaimID,
                            EmployeeID = entity.EmployeeID,
                            EndTime = entity.EndTime,
                            StartTime = entity.StartTime,
                            Note = entity.Note,
                            LeadID = entity.LeadID,
                            EntryID = entity.EntryID,
                            SuccessFlag = true
                        };

                        returnList.Add(s);
                    }
                }
                catch (Exception ex)
                {
                    DTO_CalendarData obj = (DTO_CalendarData)populateException(ex, new DTO_CalendarData());
                    returnList.Add(obj);
                }

                return returnList;
            }
        }
        #endregion

        #region GetAllCallLogs
        public List<DTO_CallLog> GetAllCallLogs()
        {
            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                List<DTO_CallLog> returnList = new List<DTO_CallLog>();
                try
                {
                    var results = context.proc_GetAllCallLogs().ToList();

                    foreach (var entity in results)
                    {
                        DTO_CallLog s = new DTO_CallLog
                        {
                            CallLogID = entity.CallLogID,
                            ClaimID = entity.ClaimID,
                            CallResults = entity.CallResults,
                            ReasonForCall = entity.ReasonForCall,
                            WhoWasCalled = entity.WhoWasCalled,
                            EmployeeID = entity.EmployeeID,
                            WhoAnswered = entity.WhoAnswered,
                            SuccessFlag = true
                        };

                        returnList.Add(s);
                    }
                }
                catch (Exception ex)
                {
                    DTO_CallLog obj = (DTO_CallLog)populateException(ex, new DTO_CallLog());
                    returnList.Add(obj);
                }

                return returnList;
            }
        }
        #endregion

        #region GetAllClaims
        public List<DTO_Claim> GetAllClaims()
        {
            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                List<DTO_Claim> returnList = new List<DTO_Claim>();
                try
                {
                    var results = context.proc_GetAllClaims().ToList();

                    foreach (var entity in results)
                    {
                        DTO_Claim c = new DTO_Claim
                        {
                            ClaimID = entity.ClaimID,
                            BillingID = entity.BillingID,
                            CustomerID = entity.CustomerID,
                            InsuranceClaimNumber = entity.InsuranceClaimNumber,
                            InsuranceCompanyID = entity.InsuranceCompanyID,
                            LeadID = entity.LeadID,
                            LossDate = entity.LossDate,
                            MortgageAccount = entity.MortgageAccount,
                            MortgageCompany = entity.MortgageCompany,
                            MRNNumber = entity.MRNNumber,
                            PropertyID = entity.PropertyID,
                            IsOpen = entity.IsOpen,
                            ContractSigned = entity.ContractSigned,
                            SuccessFlag = true
                        };

                        returnList.Add(c);
                    }
                }
                catch (Exception ex)
                {
                    DTO_Claim obj = (DTO_Claim)populateException(ex, new DTO_Claim());
                    returnList.Add(obj);
                }

                return returnList;
            }
        }
        #endregion

        #region GetAllClaimsToSchedule
        public List<DTO_Scope> GetAllClaimsToSchedule()
        {
            List<DTO_Scope> returnList = new List<DTO_Scope>();
            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                try
                {
                    var results = context.proc_GetAllClaimsToSchedule().ToList();

                    foreach (var s in results)
                    {
                        DTO_Scope scope = new DTO_Scope
                        {
                            ClaimID = s.ClaimID,
                            Deductible = s.Deductible,
                            Exterior = s.Exterior,
                            Gutter = s.Gutter,
                            Interior = s.Interior,
                            OandP = s.OandP,
                            ScopeID = s.ScopeID,
                            ScopeTypeID = s.ScopeTypeID,
                            Tax = s.Tax,
                            Total = s.Total,
                            RoofAmount = s.RoofAmount,
                            Accepted = s.Accepted,
                            SuccessFlag = true
                        };
                        returnList.Add(scope);
                    }
                }
                catch (Exception ex)
                {
                    DTO_Scope obj = (DTO_Scope)populateException(ex, new DTO_Scope());
                    returnList.Add(obj);
                }
            }
            return returnList;
        }
        #endregion

        #region GetAllClaimContacts
        public List<DTO_ClaimContacts> GetAllClaimContacts()
        {
            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                List<DTO_ClaimContacts> returnList = new List<DTO_ClaimContacts>();
                try
                {
                    var results = context.proc_GetAllClaimContacts().ToList();

                    foreach (var entity in results)
                    {
                        DTO_ClaimContacts s = new DTO_ClaimContacts
                        {
                            AdjusterID = entity.AdjusterID,
                            ClaimContactID = entity.ClaimContactID,
                            ClaimID = entity.ClaimID,
                            CustomerID = entity.CustomerID,
                            KnockerID = entity.KnockerID,
                            SalesManagerID = entity.SalesManagerID,
                            SalesPersonID = entity.SalesPersonID,
                            SupervisorID = entity.SupervisorID,
                            SuccessFlag = true
                        };

                        returnList.Add(s);
                    }
                }
                catch (Exception ex)
                {
                    DTO_ClaimContacts obj = (DTO_ClaimContacts)populateException(ex, new DTO_ClaimContacts());
                    returnList.Add(obj);
                }

                return returnList;
            }
        }
        #endregion

        #region GetAllClaimDocuments
        public List<DTO_ClaimDocument> GetAllClaimDocuments()
        {
            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                List<DTO_ClaimDocument> returnList = new List<DTO_ClaimDocument>();
                try
                {
                    var results = context.proc_GetAllClaimDocuments().ToList();

                    foreach (var entity in results)
                    {
                        DTO_ClaimDocument s = new DTO_ClaimDocument
                        {
                            ClaimID = entity.ClaimID,
                            DocTypeID = entity.DocTypeID,
                            DocumentDate = entity.DocumentDate,
                            DocumentID = entity.DocumentID,
                            FileExt = entity.FileExt,
                            FileName = entity.FileName,
                            FilePath = entity.FilePath,
                            InitialImagePath = entity.InitialImagePath,
                            NumInitials = entity.NumInitials,
                            SignatureImagePath = entity.SignatureImagePath,
                            NumSignatures = entity.NumSignatures,
                            SuccessFlag = true
                        };

                        returnList.Add(s);
                    }
                }
                catch (Exception ex)
                {
                    DTO_ClaimDocument obj = (DTO_ClaimDocument)populateException(ex, new DTO_ClaimDocument());
                    returnList.Add(obj);
                }

                return returnList;
            }
        }
        #endregion

        #region GetAllClaimStatuses
        public List<DTO_ClaimStatus> GetAllClaimStatuses()
        {
            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                List<DTO_ClaimStatus> returnList = new List<DTO_ClaimStatus>();
                try
                {
                    var results = context.proc_GetAllClaimStatuses().ToList();

                    foreach (var entity in results)
                    {
                        DTO_ClaimStatus s = new DTO_ClaimStatus
                        {
                            ClaimID = entity.ClaimID,
                            ClaimStatusDate = entity.ClaimStatusDate,
                            ClaimStatusID = entity.ClaimStatusID,
                            ClaimStatusTypeID = entity.ClaimStatusTypeID,
                            SuccessFlag = true
                        };

                        returnList.Add(s);
                    }
                }
                catch (Exception ex)
                {
                    DTO_ClaimStatus obj = (DTO_ClaimStatus)populateException(ex, new DTO_ClaimStatus());
                    returnList.Add(obj);
                }

                return returnList;
            }
        }
        #endregion

        #region GetAllClaimVendors
        public List<DTO_ClaimVendor> GetAllClaimVendors()
        {
            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                List<DTO_ClaimVendor> returnList = new List<DTO_ClaimVendor>();
                try
                {
                    var results = context.proc_GetAllClaimVendors().ToList();

                    foreach (var entity in results)
                    {
                        DTO_ClaimVendor c = new DTO_ClaimVendor
                        {
                            ClaimID = entity.ClaimID,
                            VendorID = entity.VendorID,
                            ServiceTypeID = entity.ServiceTypeID,
                            SuccessFlag = true
                        };

                        returnList.Add(c);
                    }
                }
                catch (Exception ex)
                {
                    DTO_ClaimVendor obj = (DTO_ClaimVendor)populateException(ex, new DTO_ClaimVendor());
                    returnList.Add(obj);
                }

                return returnList;
            }
        }
        #endregion

        #region GetAllCustomers
        public List<DTO_Customer> GetAllCustomers()
        {
            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                List<DTO_Customer> returnList = new List<DTO_Customer>();
                try
                {
                    var results = context.proc_GetAllCustomers().ToList();

                    foreach (var entity in results)
                    {
                        DTO_Customer s = new DTO_Customer
                        {
                            CustomerID = entity.CustomerID,
                            Email = entity.Email,
                            FirstName = entity.FirstName,
                            LastName = entity.LastName,
                            MailPromos = entity.MailPromos,
                            MiddleName = entity.MiddleName,
                            PrimaryNumber = entity.PrimaryNumber,
                            SecondaryNumber = entity.SecondaryNumber,
                            Suffix = entity.Suffix,
                            SuccessFlag = true
                        };

                        returnList.Add(s);
                    }
                }
                catch (Exception ex)
                {
                    DTO_Customer obj = (DTO_Customer)populateException(ex, new DTO_Customer());
                    returnList.Add(obj);
                }

                return returnList;
            }
        }
        #endregion

        #region GetAllDamages
        public List<DTO_Damage> GetAllDamages()
        {
            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                List<DTO_Damage> returnList = new List<DTO_Damage>();
                try
                {
                    var results = context.proc_GetAllDamages().ToList();

                    foreach (var entity in results)
                    {
                        DTO_Damage d = new DTO_Damage
                        {
                            DamageID = entity.DamageID,
                            PlaneID = entity.PlaneID,
                            DamageTypeID = entity.DamageTypeID,
                            DocumentID = entity.DocumentID,
                            DamageMeasurement = entity.DamageMeasurement,
                            XCoordinate = entity.xCoordinate,
                            YCoordinate = entity.yCoordinate,
                            SuccessFlag = true
                        };

                        returnList.Add(d);
                    }
                }
                catch (Exception ex)
                {
                    DTO_Damage obj = (DTO_Damage)populateException(ex, new DTO_Damage());
                    returnList.Add(obj);
                }

                return returnList;
            }
        }
        #endregion

        #region GetAllEmployees
        public List<DTO_Employee> GetAllEmployees()
        {
            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                List<DTO_Employee> returnList = new List<DTO_Employee>();
                try
                {
                    var results = context.proc_GetAllEmployees().ToList();

                    foreach (var entity in results)
                    {
                        DTO_Employee s = new DTO_Employee
                        {
                            Active = entity.Active,
                            CellPhone = entity.CellPhone,
                            Email = entity.Email,
                            EmployeeID = entity.EmployeeID,
                            EmployeeTypeID = entity.EmployeeTypeID,
                            FirstName = entity.FirstName,
                            LastName = entity.LastName,
                            Suffix = entity.Suffix,
                            SuccessFlag = true
                        };

                        returnList.Add(s);
                    }
                }
                catch (Exception ex)
                {
                    DTO_Employee obj = (DTO_Employee)populateException(ex, new DTO_Employee());
                    returnList.Add(obj);
                }

                return returnList;
            }
        }
        #endregion

        #region GetAllInspections
        public List<DTO_Inspection> GetAllInspections()
        {
            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                List<DTO_Inspection> returnList = new List<DTO_Inspection>();
                try
                {
                    var results = context.proc_GetAllInspections().ToList();

                    foreach (var entity in results)
                    {
                        DTO_Inspection s = new DTO_Inspection
                        {
                            ClaimID = entity.ClaimID,
                            Comments = entity.Comments,
                            CoverPool = entity.CoverPool,
                            DrivewayDamage = entity.DrivewayDamage,
                            EmergencyRepair = entity.EmergencyRepair,
                            EmergencyRepairAmount = entity.EmergencyRepairAmount,
                            ExteriorDamage = entity.ExteriorDamage,
                            FurnishPermit = entity.FurnishPermit,
                            GutterDamage = entity.GutterDamage,
                            IceWaterShield = entity.IceWaterShield,
                            InspectionDate = entity.InspectionDate,
                            InteriorDamage = entity.InteriorDamage,
                            InspectionID = entity.InspectionID,
                            Leaks = entity.Leaks,
                            MagneticRollers = entity.MagneticRollers,
                            ProtectLandscaping = entity.ProtectLandscaping,
                            QualityControl = entity.QualityControl,
                            RemoveTrash = entity.RemoveTrash,
                            RidgeMaterialTypeID = entity.RidgeMaterialTypeID,
                            RoofAge = entity.RoofAge,
                            Satellite = entity.Satellite,
                            ShingleTypeID = entity.ShingleTypeID,
                            SkyLights = entity.SkyLights,
                            SolarPanels = entity.SolarPanels,
                            TearOff = entity.TearOff,
                            LighteningProtection = entity.LightningProtection,
                            SuccessFlag = true
                        };

                        returnList.Add(s);
                    }
                }
                catch (Exception ex)
                {
                    DTO_Inspection obj = (DTO_Inspection)populateException(ex, new DTO_Inspection());
                    returnList.Add(obj);
                }

                return returnList;
            }
        }
        #endregion

        #region GetAllInsuranceCompanies
        public List<DTO_InsuranceCompany> GetAllInsuranceCompanies()
        {
            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                List<DTO_InsuranceCompany> returnList = new List<DTO_InsuranceCompany>();
                try
                {
                    var results = context.proc_GetAllInsuranceCompanies().ToList();

                    foreach (var entity in results)
                    {
                        DTO_InsuranceCompany i = new DTO_InsuranceCompany
                        {
                            InsuranceCompanyID = entity.InsuranceCompanyID,
                            CompanyName = entity.CompanyName,
                            Address = entity.Address,
                            Zip = entity.Zip,
                            ClaimPhoneNumber = entity.ClaimPhoneNumber,
                            ClaimPhoneExt = entity.ClaimPhoneExt,
                            FaxNumber = entity.FaxNumber,
                            FaxExt = entity.FaxExt,
                            Email = entity.Email,
                            Independent = entity.Independent,
                            SuccessFlag = true
                        };

                        returnList.Add(i);
                    }
                }
                catch (Exception ex)
                {
                    DTO_InsuranceCompany obj = (DTO_InsuranceCompany)populateException(ex, new DTO_InsuranceCompany());
                    returnList.Add(obj);
                }

                return returnList;
            }
        }
        #endregion

        #region GetAllInsuranceCompanyNames
        public List<DTO_InsuranceCompany> GetAllInsuranceCompanyNames()
        {
            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                List<DTO_InsuranceCompany> returnList = new List<DTO_InsuranceCompany>();
                try
                {
                    var results = context.proc_GetAllInsuranceCompanyNames().ToList();

                    foreach (var entity in results)
                    {
                        DTO_InsuranceCompany i = new DTO_InsuranceCompany
                        {
                            InsuranceCompanyID = entity.InsuranceCompanyID,
                            CompanyName = entity.CompanyName,
                            SuccessFlag = true
                        };

                        returnList.Add(i);
                    }
                }
                catch (Exception ex)
                {
                    DTO_InsuranceCompany obj = (DTO_InsuranceCompany)populateException(ex, new DTO_InsuranceCompany());
                    returnList.Add(obj);
                }

                return returnList;
            }
        }
        #endregion

        #region GetAllInvoices
        public List<DTO_Invoice> GetAllInvoices()
        {
            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                List<DTO_Invoice> returnList = new List<DTO_Invoice>();
                try
                {
                    var results = context.proc_GetAllInvoices().ToList();

                    foreach (var entity in results)
                    {
                        DTO_Invoice i = new DTO_Invoice
                        {
                            InvoiceID = entity.InvoiceID,
                            ClaimID = entity.ClaimID,
                            InvoiceTypeID = entity.InvoiceTypeID,
                            InvoiceAmount = entity.InvoiceAmount,
                            InvoiceDate = entity.InvoiceDate,
                            Paid = entity.Paid,
                            SuccessFlag = true
                        };

                        returnList.Add(i);
                    }
                }
                catch (Exception ex)
                {
                    DTO_Invoice obj = (DTO_Invoice)populateException(ex, new DTO_Invoice());
                    returnList.Add(obj);
                }

                return returnList;
            }
        }
        #endregion

        #region GetAllKnockerResponses
        public List<DTO_KnockerResponse> GetAllKnockerResponses()
        {
            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                List<DTO_KnockerResponse> returnList = new List<DTO_KnockerResponse>();
                try
                {
                    var results = context.proc_GetAllKnockerResponses().ToList();

                    foreach (var entity in results)
                    {
                        DTO_KnockerResponse k = new DTO_KnockerResponse
                        {
                            KnockerResponseID = entity.KnockerResponseID,
                            KnockerID = entity.KnockerID,
                            KnockResponseTypeID = entity.KnockResponseTypeID,
                            Address = entity.Address,
                            Zip = entity.Zip,
                            Latitude = entity.Lat,
                            Longitude = entity.Long,
                            SuccessFlag = true
                        };

                        returnList.Add(k);
                    }
                }
                catch (Exception ex)
                {
                    DTO_KnockerResponse obj = (DTO_KnockerResponse)populateException(ex, new DTO_KnockerResponse());
                    returnList.Add(obj);
                }

                return returnList;
            }
        }
        #endregion

        #region GetAllLeads
        public List<DTO_Lead> GetAllLeads()
        {
            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                List<DTO_Lead> returnList = new List<DTO_Lead>();
                try
                {
                    var results = context.proc_GetAllLeads().ToList();

                    foreach (var entity in results)
                    {
                        DTO_Lead l = new DTO_Lead
                        {
                            LeadID = entity.LeadID,
                            LeadTypeID = entity.LeadTypeID,
                            KnockerResponseID = entity.KnockerResponseID,
                            SalesPersonID = entity.SalesPersonID,
                            CustomerID = entity.CustomerID,
                            AddressID = entity.AddressID,
                            LeadDate = entity.LeadDate,
                            Status = entity.Status,
                            CreditToID = entity.CreditToID,
                            Temperature = entity.Temperature,
                            SuccessFlag = true
                        };

                        returnList.Add(l);
                    }
                }
                catch (Exception ex)
                {
                    DTO_Lead obj = (DTO_Lead)populateException(ex, new DTO_Lead());
                    returnList.Add(obj);
                }

                return returnList;
            }
        }
        #endregion

        #region GetAllNewRoofs
        public List<DTO_NewRoof> GetAllNewRoofs()
        {
            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                List<DTO_NewRoof> returnList = new List<DTO_NewRoof>();
                try
                {
                    var results = context.proc_GetAllNewRoofs().ToList();

                    foreach (var entity in results)
                    {
                        DTO_NewRoof n = new DTO_NewRoof
                        {
                            NewRoofID = entity.NewRoofID,
                            ClaimID = entity.ClaimID,
                            ProductID = entity.ProductID,
                            UpgradeCost = entity.UpgradeCost,
                            Comments = entity.Comments,
                            SuccessFlag = true
                        };

                        returnList.Add(n);
                    }
                }
                catch (Exception ex)
                {
                    DTO_NewRoof obj = (DTO_NewRoof)populateException(ex, new DTO_NewRoof());
                    returnList.Add(obj);
                }

                return returnList;
            }
        }
        #endregion

        #region GetAllOrderItems
        public List<DTO_OrderItem> GetAllOrderItems()
        {
            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                List<DTO_OrderItem> returnList = new List<DTO_OrderItem>();
                try
                {
                    var results = context.proc_GetAllOrderItems().ToList();

                    foreach (var entity in results)
                    {
                        DTO_OrderItem o = new DTO_OrderItem
                        {
                            OrderItemID = entity.OrderItemID,
                            OrderID = entity.OrderID,
                            ProductID = entity.ProductID,
                            UnitOfMeasureID = entity.UnitOfMeasureID,
                            Quantity = entity.Quantity,
                            SuccessFlag = true
                        };

                        returnList.Add(o);
                    }
                }
                catch (Exception ex)
                {
                    DTO_OrderItem obj = (DTO_OrderItem)populateException(ex, new DTO_OrderItem());
                    returnList.Add(obj);
                }

                return returnList;
            }
        }
        #endregion

        #region GetAllOrders
        public List<DTO_Order> GetAllOrders()
        {
            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                List<DTO_Order> returnList = new List<DTO_Order>();
                try
                {
                    var results = context.proc_GetAllOrders().ToList();

                    foreach (var entity in results)
                    {
                        DTO_Order o = new DTO_Order
                        {
                            OrderID = entity.OrderID,
                            VendorID = entity.VendorID,
                            ClaimID = entity.ClaimID,
                            DateOrdered = entity.DateOrdered,
                            DateDropped = entity.DateDropped,
                            ScheduledInstallation = entity.ScheduledInstallation,
                            SuccessFlag = true
                        };

                        returnList.Add(o);
                    }
                }
                catch (Exception ex)
                {
                    DTO_Order obj = (DTO_Order)populateException(ex, new DTO_Order());
                    returnList.Add(obj);
                }

                return returnList;
            }
        }
        #endregion

        #region GetAllPayments
        public List<DTO_Payment> GetAllPayments()
        {
            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                List<DTO_Payment> returnList = new List<DTO_Payment>();
                try
                {
                    var results = context.proc_GetAllPayments().ToList();

                    foreach (var entity in results)
                    {
                        DTO_Payment p = new DTO_Payment
                        {
                            Amount = entity.Amount,
                            ClaimID = entity.ClaimID,
                            PaymentDate = entity.PaymentDate,
                            PaymentDescriptionID = entity.PaymentDescriptionID,
                            PaymentID = entity.PaymentID,
                            PaymentTypeID = entity.PaymentTypeID,
                            SuccessFlag = true
                        };

                        returnList.Add(p);
                    }
                }
                catch (Exception ex)
                {
                    DTO_Payment obj = (DTO_Payment)populateException(ex, new DTO_Payment());
                    returnList.Add(obj);
                }

                return returnList;
            }
        }
        #endregion

        #region GetAllPlanes
        public List<DTO_Plane> GetAllPlanes()
        {
            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                List<DTO_Plane> returnList = new List<DTO_Plane>();
                try
                {
                    var results = context.proc_GetAllPlanes().ToList();

                    foreach (var entity in results)
                    {
                        DTO_Plane p = new DTO_Plane
                        {
                            PlaneTypeID = entity.PlaneTypeID,
                            ItemSpec = entity.ItemSpec,
                            EaveHeight = entity.EaveHeight,
                            EaveLength = entity.EaveLength,
                            FourAndUp = entity.FourAndUp,
                            GroupNumber = entity.GroupNumber,
                            HipValley = entity.HipValley,
                            InspectionID = entity.InspectionID,
                            NumberDecking = entity.NumberDecking,
                            NumOfLayers = entity.NumOfLayers,
                            Pitch = entity.Pitch,
                            PlaneID = entity.PlaneID,
                            RidgeLength = entity.RidgeLength,
                            RakeLength = entity.RakeLength,
                            StepFlashing = entity.StepFlashing,
                            SquareFootage = entity.SquareFootage,
                            ThreeAndOne = entity.ThreeAndOne,
                            SuccessFlag = true
                        };

                        returnList.Add(p);
                    }
                }
                catch (Exception ex)
                {
                    DTO_Plane obj = (DTO_Plane)populateException(ex, new DTO_Plane());
                    returnList.Add(obj);
                }

                return returnList;
            }
        }
        #endregion

        #region GetAllReferrers
        public List<DTO_Referrer> GetAllReferrers()
        {
            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                List<DTO_Referrer> returnList = new List<DTO_Referrer>();
                try
                {
                    var results = context.proc_GetAllReferrers().ToList();

                    foreach (var entity in results)
                    {
                        DTO_Referrer r = new DTO_Referrer
                        {
                            CellPhone = entity.CellPhone,
                            Email = entity.Email,
                            FirstName = entity.FirstName,
                            LastName = entity.LastName,
                            MailingAddress = entity.MailingAddress,
                            ReferrerID = entity.ReferrerID,
                            Suffix = entity.Suffix,
                            Zip = entity.Zip,
                            SuccessFlag = true
                        };

                        returnList.Add(r);
                    }
                }
                catch (Exception ex)
                {
                    DTO_Referrer obj = (DTO_Referrer)populateException(ex, new DTO_Referrer());
                    returnList.Add(obj);
                }

                return returnList;
            }
        }
        #endregion

        #region GetAllScopes
        public List<DTO_Scope> GetAllScopes()
        {
            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                List<DTO_Scope> returnList = new List<DTO_Scope>();
                try
                {
                    var results = context.proc_GetAllScopes().ToList();

                    foreach (var entity in results)
                    {
                        DTO_Scope s = new DTO_Scope
                        {
                            ClaimID = entity.ClaimID,
                            Deductible = entity.Deductible,
                            Exterior = entity.Exterior,
                            Gutter = entity.Gutter,
                            Interior = entity.Interior,
                            OandP = entity.OandP,
                            ScopeID = entity.ScopeID,
                            ScopeTypeID = entity.ScopeTypeID,
                            Tax = entity.Tax,
                            Total = entity.Total,
                            RoofAmount = entity.RoofAmount,
                            Accepted = entity.Accepted,
                            SuccessFlag = true
                        };

                        returnList.Add(s);
                    }
                }
                catch (Exception ex)
                {
                    DTO_Scope obj = (DTO_Scope)populateException(ex, new DTO_Scope());
                    returnList.Add(obj);
                }

                return returnList;
            }
        }
        #endregion

        #region GetAllSurplusSupplies
        public List<DTO_SurplusSupplies> GetAllSurplusSupplies()
        {
            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                List<DTO_SurplusSupplies> returnList = new List<DTO_SurplusSupplies>();
                try
                {
                    var results = context.proc_GetAllSurplusSupplies().ToList();

                    foreach (var entity in results)
                    {
                        DTO_SurplusSupplies s = new DTO_SurplusSupplies
                        {
                            ClaimID = entity.ClaimID,
                            DropOffDate = entity.DropOffDate,
                            Items = entity.Items,
                            PickUpDate = entity.PickUpDate,
                            Quantity = entity.Quantity,
                            SurplusSuppliesID = entity.SurplusSuppliesID,
                            UnitOfMeasureID = entity.UnitOfMeasureID,
                            SuccessFlag = true
                        };

                        returnList.Add(s);
                    }
                }
                catch (Exception ex)
                {
                    DTO_SurplusSupplies obj = (DTO_SurplusSupplies)populateException(ex, new DTO_SurplusSupplies());
                    returnList.Add(obj);
                }

                return returnList;
            }
        }
        #endregion

        #region GetAllUsers
        public List<DTO_User> GetAllUsers()
        {
            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                List<DTO_User> returnList = new List<DTO_User>();
                try
                {
                    var results = context.proc_GetAllUsers().ToList();

                    foreach (var entity in results)
                    {
                        DTO_User u = new DTO_User
                        {
                            Active = entity.Active,
                            EmployeeID = entity.EmployeeID,
                            Pass = entity.Pass,
                            PermissionID = entity.PermissionID,
                            UserID = entity.UserID,
                            Username = entity.Username,
                            SuccessFlag = true
                        };

                        returnList.Add(u);
                    }
                }
                catch (Exception ex)
                {
                    DTO_User obj = (DTO_User)populateException(ex, new DTO_User());
                    returnList.Add(obj);
                }

                return returnList;
            }
        }
        #endregion 

        #region GetAllVendors
        public List<DTO_Vendor> GetAllVendors()
        {
            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                List<DTO_Vendor> returnList = new List<DTO_Vendor>();
                try
                {
                    var results = context.proc_GetAllVendors().ToList();

                    foreach (var entity in results)
                    {
                        DTO_Vendor v = new DTO_Vendor
                        {
                            VendorID = entity.VendorID,
                            VendorTypeID = entity.VendorTypeID,
                            CompanyName = entity.CompanyName,
                            EIN = entity.EIN,
                            ContactFirstName = entity.ContactFirstName,
                            ContactLastName = entity.ContactLastName,
                            Suffix = entity.Suffix,
                            VendorAddress = entity.VendorAddress,
                            Zip = entity.Zip,
                            Phone = entity.Phone,
                            CompanyPhone = entity.CompanyPhone,
                            Fax = entity.Fax,
                            Email = entity.Email,
                            Website = entity.Website,
                            GeneralLiabilityExpiration = entity.GeneralLiabilityExpiration,
                            SuccessFlag = true
                        };

                        returnList.Add(v);
                    }
                }
                catch (Exception ex)
                {
                    DTO_Vendor obj = (DTO_Vendor)populateException(ex, new DTO_Vendor());
                    returnList.Add(obj);
                }

                return returnList;
            }
        }
        #endregion

        #endregion

        #region FILTERED GETS

        #region GetAllClosedClaims
        public List<DTO_Claim> GetAllClosedClaims()
        {
            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                List<DTO_Claim> returnList = new List<DTO_Claim>();
                try
                {
                    var results = context.proc_GetAllClosedClaims().ToList();

                    foreach (var entity in results)
                    {
                        DTO_Claim s = new DTO_Claim
                        {
                            ClaimID = entity.ClaimID,
                            BillingID = entity.BillingID,
                            CustomerID = entity.CustomerID,
                            InsuranceClaimNumber = entity.InsuranceClaimNumber,
                            InsuranceCompanyID = entity.InsuranceCompanyID,
                            LeadID = entity.LeadID,
                            LossDate = entity.LossDate,
                            MortgageAccount = entity.MortgageAccount,
                            MortgageCompany = entity.MortgageCompany,
                            MRNNumber = entity.MRNNumber,
                            PropertyID = entity.PropertyID,
                            IsOpen = entity.IsOpen,
                            ContractSigned = entity.ContractSigned,                            
                            SuccessFlag = true
                        };

                        returnList.Add(s);
                    }
                }
                catch (Exception ex)
                {
                    DTO_Claim obj = (DTO_Claim)populateException(ex, new DTO_Claim());
                    returnList.Add(obj);
                }

                return returnList;
            }
        }
        #endregion

        #region GetAllInactiveClaims
        public List<DTO_Claim> GetAllInactiveClaims()
        {
            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                List<DTO_Claim> returnList = new List<DTO_Claim>();
                try
                {
                    var results = context.proc_GetAllInactiveClaims().ToList();

                    foreach (var entity in results)
                    {
                        DTO_Claim s = new DTO_Claim
                        {
                            ClaimID = entity.ClaimID,
                            BillingID = entity.BillingID,
                            CustomerID = entity.CustomerID,
                            InsuranceClaimNumber = entity.InsuranceClaimNumber,
                            InsuranceCompanyID = entity.InsuranceCompanyID,
                            LeadID = entity.LeadID,
                            LossDate = entity.LossDate,
                            MortgageAccount = entity.MortgageAccount,
                            MortgageCompany = entity.MortgageCompany,
                            MRNNumber = entity.MRNNumber,
                            PropertyID = entity.PropertyID,
                            IsOpen = entity.IsOpen,
                            ContractSigned = entity.ContractSigned,
                            SuccessFlag = true
                        };

                        returnList.Add(s);
                    }
                }
                catch (Exception ex)
                {
                    DTO_Claim obj = (DTO_Claim)populateException(ex, new DTO_Claim());
                    returnList.Add(obj);
                }

                return returnList;
            }
        }
        #endregion

        #region GetAllOpenClaims
        public List<DTO_Claim> GetAllOpenClaims()
        {
            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                List<DTO_Claim> returnList = new List<DTO_Claim>();
                try
                {
                    var results = context.proc_GetAllOpenClaims().ToList();

                    foreach (var entity in results)
                    {
                        DTO_Claim s = new DTO_Claim
                        {
                            ClaimID = entity.ClaimID,
                            BillingID = entity.BillingID,
                            CustomerID = entity.CustomerID,
                            InsuranceClaimNumber = entity.InsuranceClaimNumber,
                            InsuranceCompanyID = entity.InsuranceCompanyID,
                            LeadID = entity.LeadID,
                            LossDate = entity.LossDate,
                            MortgageAccount = entity.MortgageAccount,
                            MortgageCompany = entity.MortgageCompany,
                            MRNNumber = entity.MRNNumber,
                            PropertyID = entity.PropertyID,
                            IsOpen = entity.IsOpen,
                            ContractSigned = entity.ContractSigned,
                            SuccessFlag = true
                        };

                        returnList.Add(s);
                    }
                }
                catch (Exception ex)
                {
                    DTO_Claim obj = (DTO_Claim)populateException(ex, new DTO_Claim());
                    returnList.Add(obj);
                }

                return returnList;
            }
        }
        #endregion

        #region GetRecentClaimsBySalesPersonID
        public List<DTO_Claim> GetRecentClaimsBySalesPersonID(DTO_Employee token)
        {
            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                List<DTO_Claim> returnList = new List<DTO_Claim>();
                try
                {
                    var results = context.proc_GetRecentClaimsBySalesPersonID(token.EmployeeID).ToList();

                    foreach (var entity in results)
                    {
                        DTO_Claim s = new DTO_Claim
                        {
                            ClaimID = entity.ClaimID,
                            BillingID = entity.BillingID,
                            CustomerID = entity.CustomerID,
                            InsuranceClaimNumber = entity.InsuranceClaimNumber,
                            InsuranceCompanyID = entity.InsuranceCompanyID,
                            LeadID = entity.LeadID,
                            LossDate = entity.LossDate,
                            MortgageAccount = entity.MortgageAccount,
                            MortgageCompany = entity.MortgageCompany,
                            MRNNumber = entity.MRNNumber,
                            PropertyID = entity.PropertyID,
                            IsOpen = entity.IsOpen,
                            ContractSigned = entity.ContractSigned,
                            SuccessFlag = true
                        };

                        returnList.Add(s);
                    }
                }
                catch (Exception ex)
                {
                    DTO_Claim obj = (DTO_Claim)populateException(ex, new DTO_Claim());
                    returnList.Add(obj);
                }

                return returnList;
            }
        }
        #endregion

        #region GetRecentLeadsBySalesPersonID
        public List<DTO_Lead> GetRecentLeadsBySalesPersonID(DTO_Employee token)
        {
            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                List<DTO_Lead> returnList = new List<DTO_Lead>();
                try
                {
                    var results = context.proc_GetRecentLeadsBySalesPersonID(token.EmployeeID).ToList();

                    foreach (var entity in results)
                    {
                        DTO_Lead l = new DTO_Lead
                        {
                            LeadID = entity.LeadID,
                            LeadTypeID = entity.LeadTypeID,
                            KnockerResponseID = entity.KnockerResponseID,
                            SalesPersonID = entity.SalesPersonID,
                            CustomerID = entity.CustomerID,
                            AddressID = entity.AddressID,
                            LeadDate = entity.LeadDate,
                            Status = entity.Status,
                            CreditToID = entity.CreditToID,
                            Temperature = entity.Temperature,
                            SuccessFlag = true
                        };

                        returnList.Add(l);
                    }
                }
                catch (Exception ex)
                {
                    DTO_Lead obj = (DTO_Lead)populateException(ex, new DTO_Lead());
                    returnList.Add(obj);
                }

                return returnList;
            }
        }
        #endregion

        #region GetRecentInspectionsBySalesPersonID
        public List<DTO_Inspection> GetRecentInspectionsBySalesPersonID(DTO_Employee token)
        {
            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                List<DTO_Inspection> returnList = new List<DTO_Inspection>();
                try
                {
                    var results = context.proc_GetRecentInspectionsBySalesPersonID(token.EmployeeID).ToList();

                    foreach (var entity in results)
                    {
                        DTO_Inspection s = new DTO_Inspection
                        {
                            ClaimID = entity.ClaimID,
                            Comments = entity.Comments,
                            CoverPool = entity.CoverPool,
                            DrivewayDamage = entity.DrivewayDamage,
                            EmergencyRepair = entity.EmergencyRepair,
                            EmergencyRepairAmount = entity.EmergencyRepairAmount,
                            ExteriorDamage = entity.ExteriorDamage,
                            FurnishPermit = entity.FurnishPermit,
                            GutterDamage = entity.GutterDamage,
                            IceWaterShield = entity.IceWaterShield,
                            InspectionDate = entity.InspectionDate,
                            InteriorDamage = entity.InteriorDamage,
                            InspectionID = entity.InspectionID,
                            Leaks = entity.Leaks,
                            MagneticRollers = entity.MagneticRollers,
                            ProtectLandscaping = entity.ProtectLandscaping,
                            QualityControl = entity.QualityControl,
                            RemoveTrash = entity.RemoveTrash,
                            RidgeMaterialTypeID = entity.RidgeMaterialTypeID,
                            RoofAge = entity.RoofAge,
                            Satellite = entity.Satellite,
                            ShingleTypeID = entity.ShingleTypeID,
                            SkyLights = entity.SkyLights,
                            SolarPanels = entity.SolarPanels,
                            TearOff = entity.TearOff,
                            LighteningProtection = entity.LightningProtection,
                            SuccessFlag = true
                        };

                        returnList.Add(s);
                    }
                }
                catch (Exception ex)
                {
                    DTO_Inspection obj = (DTO_Inspection)populateException(ex, new DTO_Inspection());
                    returnList.Add(obj);
                }

                return returnList;
            }
        }
        #endregion

        #endregion

        #region UPDATES

        #region UpdateAdditionalSupplies
        public DTO_AdditionalSupply UpdateAdditionalSupplies(DTO_AdditionalSupply token)
        {
            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                try
                {
                    var outputParameter = new ObjectParameter("SuccessFlag", typeof(bool));

                    var result = context.proc_UpdateAdditionalSupplies(token.AdditionalSuppliesID, token.PickUpDate, token.DropOffDate,
                        token.Items, token.Cost, token.ReceiptImagePath, outputParameter);

                    if ((bool)outputParameter.Value)
                    {
                        token.SuccessFlag = true;
                    }
                }
                catch (Exception ex)
                {
                    token = (DTO_AdditionalSupply)populateException(ex, token);
                }
            }

            return token;
        }
        #endregion

        #region UpdateAddress
        public DTO_Address UpdateAddress(DTO_Address token)
        {
            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                try
                {
                    var outputParameter = new ObjectParameter("SuccessFlag", typeof(bool));

                    var result = context.proc_UpdateAddress(token.AddressID, token.Address, token.Zip, outputParameter);

                    if ((bool)outputParameter.Value)
                    {
                        token.SuccessFlag = true;
                    }
                }
                catch (Exception ex)
                {
                    token = (DTO_Address)populateException(ex, token);
                }
            }

            return token;
        }
        #endregion

        #region UpdateAdjuster
        public DTO_Adjuster UpdateAdjuster(DTO_Adjuster token)
        {
            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                try
                {
                    var outputParameter = new ObjectParameter("SuccessFlag", typeof(bool));

                    var result = context.proc_UpdateAdjuster(token.AdjusterID, token.FirstName, token.LastName, token.Suffix, token.PhoneNumber,
                        token.PhoneExt, token.Email, token.InsuranceCompanyID, token.Comments, outputParameter);

                    if ((bool)outputParameter.Value)
                    {
                        token.SuccessFlag = true;
                    }
                }
                catch (Exception ex)
                {
                    token = (DTO_Adjuster)populateException(ex, token);
                }
            }

            return token;
        }
        #endregion

        #region UpdateAdjustment
        public DTO_Adjustment UpdateAdjustment(DTO_Adjustment token)
        {
            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                try
                {
                    var outputParameter = new ObjectParameter("SuccessFlag", typeof(bool));

                    var result = context.proc_UpdateAdjustment(token.AdjustmentID, token.Gutters, token.Exterior, token.Interior,
                        token.AdjustmentResultID, token.AdjustmentDate, token.AdjustmentComment, outputParameter);

                    if ((bool)outputParameter.Value)
                    {
                        token.SuccessFlag = true;
                    }
                }
                catch (Exception ex)
                {
                    token = (DTO_Adjustment)populateException(ex, token);
                }
            }

            return token;
        }
        #endregion

        #region UpdatedCalendatData
        public DTO_CalendarData UpdateCalendarData(DTO_CalendarData token)
        {
            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                try
                {
                    var outputParameter = new ObjectParameter("SuccessFlag", typeof(bool));

                    var result = context.proc_UpdateCalendarData(token.EntryID, token.AppointmentTypeID, token.EmployeeID, token.StartTime, 
                        token.EndTime, token.ClaimID, token.LeadID, token.Note, outputParameter);

                    if ((bool)outputParameter.Value)
                    {
                        token.SuccessFlag = true;
                    }
                }
                catch (Exception ex)
                {
                    token = (DTO_CalendarData)populateException(ex, token);
                }
            }

            return token;
        }
        #endregion

        #region UpdateClaim
        public DTO_Claim UpdateClaim(DTO_Claim token)
        {
            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                try
                {
                    var outputParameter = new ObjectParameter("SuccessFlag", typeof(bool));

                    var result = context.proc_UpdateClaim(token.ClaimID, token.CustomerID, token.BillingID, token.PropertyID,
                        token.InsuranceCompanyID, token.InsuranceClaimNumber, token.LossDate, token.MortgageCompany,
                        token.MortgageAccount, token.ContractSigned, outputParameter);

                    if ((bool)outputParameter.Value)
                    {
                        token.SuccessFlag = true;
                    }
                }
                catch (Exception ex)
                {
                    token = (DTO_Claim)populateException(ex, token);
                }
            }

            return token;
        }
        #endregion

        #region UpdateClaimContacts
        public DTO_ClaimContacts UpdateClaimContacts(DTO_ClaimContacts token)
        {
            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                try
                {
                    var outputParameter = new ObjectParameter("SuccessFlag", typeof(bool));

                    var result = context.proc_UpdateClaimContacts(token.ClaimContactID, token.CustomerID, token.KnockerID, token.SalesPersonID,
                        token.SupervisorID, token.SalesManagerID, token.AdjusterID, outputParameter);

                    if ((bool)outputParameter.Value)
                    {
                        token.SuccessFlag = true;
                    }
                }
                catch (Exception ex)
                {
                    token = (DTO_ClaimContacts)populateException(ex, token);
                }
            }

            return token;
        }
        #endregion

        #region UpdateClaimStatuses
        public DTO_ClaimStatus UpdateClaimStatuses(DTO_ClaimStatus token)
        {
            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                try
                {
                    var outputParameter = new ObjectParameter("SuccessFlag", typeof(bool));

                    var result = context.proc_UpdateClaimStatuses(token.ClaimStatusID, token.ClaimStatusTypeID, token.ClaimStatusDate, outputParameter);

                    if ((bool)outputParameter.Value)
                    {
                        token.SuccessFlag = true;
                    }
                }
                catch (Exception ex)
                {
                    token = (DTO_ClaimStatus)populateException(ex, token);
                }
            }

            return token;
        }
        #endregion

        #region UpdateCustomer
        public DTO_Customer UpdateCustomer(DTO_Customer token)
        {
            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                try
                {
                    var outputParameter = new ObjectParameter("SuccessFlag", typeof(bool));

                    var result = context.proc_UpdateCustomer(token.CustomerID, token.FirstName, token.MiddleName, token.LastName,
                        token.Suffix, token.PrimaryNumber, token.SecondaryNumber, token.Email, token.MailPromos, outputParameter);

                    if ((bool)outputParameter.Value)
                    {
                        token.SuccessFlag = true;
                    }
                }
                catch (Exception ex)
                {
                    token = (DTO_Customer)populateException(ex, token);
                }
            }

            return token;
        }
        #endregion

        #region UpdateDamage
        public DTO_Damage UpdateDamage(DTO_Damage token)
        {
            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                try
                {
                    var outputParameter = new ObjectParameter("SuccessFlag", typeof(bool));

                    var result = context.proc_UpdateDamage(token.DamageID, token.PlaneID, token.DamageTypeID, token.DocumentID, token.DamageMeasurement, token.XCoordinate, token.YCoordinate, outputParameter);

                    if ((bool)outputParameter.Value)
                    {
                        token.SuccessFlag = true;
                    }
                }
                catch (Exception ex)
                {
                    token = (DTO_Damage)populateException(ex, token);
                }
            }

            return token;
        }
        #endregion

        #region UpdateEmployee
        public DTO_Employee UpdateEmployee(DTO_Employee token)
        {
            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                try
                {
                    var outputParameter = new ObjectParameter("SuccessFlag", typeof(bool));

                    var result = context.proc_UpdateEmployee(token.EmployeeID, token.EmployeeTypeID, token.FirstName, token.LastName,
                        token.Suffix, token.Email, token.CellPhone, token.Active, outputParameter);

                    if ((bool)outputParameter.Value)
                    {
                        token.SuccessFlag = true;
                    }
                }
                catch (Exception ex)
                {
                    token = (DTO_Employee)populateException(ex, token);
                }
            }

            return token;
        }
        #endregion

        #region UpdateInspection
        public DTO_Inspection UpdateInspection(DTO_Inspection token)
        {
            using(MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                try
                {
                    var outputParameter = new ObjectParameter("SuccessFlag", typeof(bool));

                    var result = context.proc_UpdateInspection(token.InspectionID, token.RidgeMaterialTypeID,
                        token.ShingleTypeID, token.InspectionDate, token.SkyLights, token.Leaks, token.GutterDamage,
                        token.DrivewayDamage, token.MagneticRollers, token.IceWaterShield, token.EmergencyRepair,
                        token.EmergencyRepairAmount, token.QualityControl, token.ProtectLandscaping, token.RemoveTrash,
                        token.FurnishPermit, token.CoverPool, token.InteriorDamage, token.ExteriorDamage, token.LighteningProtection,
                        token.TearOff, token.Satellite, token.SolarPanels, token.RoofAge, token.Comments, outputParameter);

                    if ((bool)outputParameter.Value)
                    {
                        token.SuccessFlag = true;
                    }
                }
                catch (Exception ex)
                {
                    token = (DTO_Inspection)populateException(ex, token);
                }
            }

            return token;
        }
        #endregion

        #region UpdateInsuranceCompany
        public DTO_InsuranceCompany UpdateInsuranceCompany(DTO_InsuranceCompany token)
        {
            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                try
                {
                    var outputParameter = new ObjectParameter("SuccessFlag", typeof(bool));

                    var result = context.proc_UpdateInsuranceCompany(token.InsuranceCompanyID, token.CompanyName,
                        token.Address, token.Zip, token.ClaimPhoneNumber, token.ClaimPhoneExt, token.FaxNumber,
                        token.FaxExt, token.Email, token.Independent, outputParameter);

                    if ((bool)outputParameter.Value)
                    {
                        token.SuccessFlag = true;
                    }
                }
                catch (Exception ex)
                {
                    token = (DTO_InsuranceCompany)populateException(ex, token);
                }
            }

            return token;
        }
        #endregion

        #region UpdateInvoice
        public DTO_Invoice UpdateInvoice(DTO_Invoice token)
        {
            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                try
                {
                    var outputParameter = new ObjectParameter("SuccessFlag", typeof(bool));

                    var result = context.proc_UpdateInvoice(token.InvoiceID, token.InvoiceTypeID, token.InvoiceAmount, token.InvoiceDate,
                        token.Paid, outputParameter);

                    if ((bool)outputParameter.Value)
                    {
                        token.SuccessFlag = true;
                    }
                }
                catch (Exception ex)
                {
                    token = (DTO_Invoice)populateException(ex, token);
                }
            }

            return token;
        }
        #endregion

        #region UpdateKnockerResponse
        public DTO_KnockerResponse UpdateKnockerResponse(DTO_KnockerResponse token)
        {
            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                try
                {
                    var outputParameter = new ObjectParameter("SuccessFlag", typeof(bool));

                    var result = context.proc_UpdateKnockerResponse(token.KnockerResponseID, token.KnockResponseTypeID, token.Address,
                        token.Zip, token.Latitude, token.Longitude, outputParameter);

                    if ((bool)outputParameter.Value)
                    {
                        token.SuccessFlag = true;
                    }
                }
                catch (Exception ex)
                {
                    token = (DTO_KnockerResponse)populateException(ex, token);
                }
            }

            return token;
        }
        #endregion

        #region UpdateLead
        public DTO_Lead UpdateLead(DTO_Lead token)
        {
            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                try
                {
                    var outputParameter = new ObjectParameter("SuccessFlag", typeof(bool));

                    var result = context.proc_UpdateLead(token.LeadID, token.CustomerID, token.AddressID, token.Status, token.CreditToID, token.Temperature, outputParameter);

                    token.SuccessFlag = (bool)outputParameter.Value;
                }
                catch (Exception ex)
                {

                    token = (DTO_Lead)populateException(ex, token);
                }
            }

            return token;
        }
        #endregion

        #region UpdateNewRoof
        public DTO_NewRoof UpdateNewRoof(DTO_NewRoof token)
        {
            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                try
                {
                    var outputParameter = new ObjectParameter("SuccessFlag", typeof(bool));

                    var result = context.proc_UpdateNewRoof(token.NewRoofID, token.ProductID, token.UpgradeCost, token.Comments, outputParameter);

                    token.SuccessFlag = (bool)outputParameter.Value;
                }
                catch (Exception ex)
                {

                    token = (DTO_NewRoof)populateException(ex, token);
                }
            }

            return token;
        }
        #endregion

        #region UpdateOrder
        public DTO_Order UpdateOrder(DTO_Order token)
        {
            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                try
                {
                    var outputParameter = new ObjectParameter("SuccessFlag", typeof(bool));

                    var result = context.proc_UpdateOrder(token.OrderID, token.VendorID, token.DateOrdered, token.DateDropped, token.ScheduledInstallation, outputParameter);

                    token.SuccessFlag = (bool)outputParameter.Value;
                }
                catch (Exception ex)
                {

                    token = (DTO_Order)populateException(ex, token);
                }
            }

            return token;
        }
        #endregion

        #region UpdateOrderItem
        public DTO_OrderItem UpdateOrderItem(DTO_OrderItem token)
        {
            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                try
                {
                    var outputParameter = new ObjectParameter("SuccessFlag", typeof(bool));

                    var result = context.proc_UpdateOrderItem(token.OrderItemID, token.ProductID, token.UnitOfMeasureID, token.Quantity, outputParameter);

                    token.SuccessFlag = (bool)outputParameter.Value;
                }
                catch (Exception ex)
                {

                    token = (DTO_OrderItem)populateException(ex, token);
                }
            }

            return token;
        }
        #endregion

        #region UpdatePayment
        public DTO_Payment UpdatePayment(DTO_Payment token)
        {
            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                try
                {
                    var outputParameter = new ObjectParameter("SuccessFlag", typeof(bool));

                    var result = context.proc_UpdatePayment(token.PaymentID, token.PaymentTypeID, token.PaymentDescriptionID, token.Amount, token.PaymentDate, outputParameter);

                    token.SuccessFlag = (bool)outputParameter.Value;
                }
                catch (Exception ex)
                {

                    token = (DTO_Payment)populateException(ex, token);
                }
            }

            return token;
        }
        #endregion

        #region UpdatePlane
        public DTO_Plane UpdatePlane(DTO_Plane token)
        {
            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                try
                {
                    var outputParameter = new ObjectParameter("SuccessFlag", typeof(bool));

                    var result = context.proc_UpdatePlane(token.PlaneID, token.PlaneTypeID, token.GroupNumber, token.NumOfLayers,
                        token.ThreeAndOne, token.FourAndUp, token.Pitch, token.HipValley, token.RidgeLength, token.RakeLength,
                        token.EaveHeight, token.EaveLength, token.NumberDecking, token.StepFlashing, token.SquareFootage, token.ItemSpec, outputParameter);

                    if ((bool)outputParameter.Value)
                    {
                        token.SuccessFlag = true;
                    }
                }
                catch (Exception ex)
                {
                    token = (DTO_Plane)populateException(ex, token);
                }
            }

            return token;
        }
        #endregion

        #region UpdateReferrer
        public DTO_Referrer UpdateReferrer(DTO_Referrer token)
        {
            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                try
                {
                    var outputParameter = new ObjectParameter("SuccessFlag", typeof(bool));

                    var result = context.proc_UpdateReferrer(token.ReferrerID, token.FirstName, token.LastName, token.Suffix, token.MailingAddress, token.Zip, token.Email, token.CellPhone, outputParameter);

                    token.SuccessFlag = (bool)outputParameter.Value;
                }
                catch (Exception ex)
                {

                    token = (DTO_Referrer)populateException(ex, token);
                }
            }

            return token;
        }
        #endregion

        #region UpdateScope
        public DTO_Scope UpdateScope(DTO_Scope token)
        {
            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                try
                {
                    var outputParameter = new ObjectParameter("SuccessFlag", typeof(bool));

                    var result = context.proc_UpdateScope(token.ScopeID, token.ScopeTypeID, token.Interior,
                        token.Exterior, token.Gutter, token.RoofAmount, token.Tax, token.Deductible, token.Total, token.OandP, token.Accepted, outputParameter);

                    if ((bool)outputParameter.Value)
                    {
                        token.SuccessFlag = true;
                    }
                }
                catch (Exception ex)
                {
                    token = (DTO_Scope)populateException(ex, token);
                }
            }

            return token;
        }
        #endregion

        #region UpdateSurplusSupplies
        public DTO_SurplusSupplies UpdateSurplusSupplies(DTO_SurplusSupplies token)
        {
            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                try
                {
                    var outputParameter = new ObjectParameter("SuccessFlag", typeof(bool));

                    var result = context.proc_UpdateSurplusSupplies(token.SurplusSuppliesID, token.UnitOfMeasureID, token.Quantity, token.PickUpDate, token.DropOffDate, token.Items, outputParameter);

                    token.SuccessFlag = (bool)outputParameter.Value;
                }
                catch (Exception ex)
                {
                    token = (DTO_SurplusSupplies)populateException(ex, token);
                }
            }

            return token;
        }
        #endregion

        #region UpdateUser
        public DTO_User UpdateUser(DTO_User token)
        {
            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                try
                {
                    var outputParameter = new ObjectParameter("SuccessFlag", typeof(bool));

                    var result = context.proc_UpdateUser(token.UserID, token.Username, token.Pass, token.PermissionID, token.Active, outputParameter);

                    token.SuccessFlag = (bool)outputParameter.Value;
                }
                catch (Exception ex)
                {

                    token = (DTO_User)populateException(ex, token);
                }

            }

            return token;
        }
        #endregion

        #region UpdateVendor
        public DTO_Vendor UpdateVendor(DTO_Vendor token)
        {
            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                try
                {
                    var outputParameter = new ObjectParameter("SuccessFlag", typeof(bool));

                    var result = context.proc_UpdateVendor(token.VendorID, token.VendorTypeID, token.CompanyName, token.EIN, token.ContactFirstName, token.ContactLastName,
                        token.Suffix, token.VendorAddress, token.Zip, token.Phone, token.CompanyPhone, token.Fax, token.Email, token.Website, token.GeneralLiabilityExpiration, outputParameter);

                    if ((bool)outputParameter.Value)
                    {
                        token.SuccessFlag = true;
                    }
                }
                catch (Exception ex)
                {
                    token = (DTO_Vendor)populateException(ex, token);
                }
            }

            return token;
        }
        #endregion

        #endregion UPDATES

        #region LOOKUP LISTS
        public List<DTO_LU_AdjustmentResult> GetAdjustmentResults()
        {
            List<DTO_LU_AdjustmentResult> token = new List<DTO_LU_AdjustmentResult>();
            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                var results = context.proc_Get_LU_AdjustmentResults().ToList();

                foreach (var e in results)
                {
                    DTO_LU_AdjustmentResult o = new DTO_LU_AdjustmentResult();
                    o.AdjustmentResultID = e.AdjustmentResultID;
                    o.AdjustmentResult = e.AdjustmentResult;

                    token.Add(o);
                }
            }
            return token;
        }

        public List<DTO_LU_AppointmentTypes> GetAppointmentTypes()
        {
            List<DTO_LU_AppointmentTypes> token = new List<DTO_LU_AppointmentTypes>();

            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                var results = context.proc_Get_LU_AppointmentTypes().ToList();

                foreach (var e in results)
                {
                    DTO_LU_AppointmentTypes o = new DTO_LU_AppointmentTypes();
                    o.AppointmentTypeID = e.AppointmentTypeID;
                    o.AppointmentType = e.AppointmentType;

                    token.Add(o);
                }
            }
            return token;
        }

        public List<DTO_LU_ClaimDocumentType> GetClaimDocumentTypes()
        {
            List<DTO_LU_ClaimDocumentType> token = new List<DTO_LU_ClaimDocumentType>();

            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                var results = context.proc_Get_LU_ClaimDocumentTypes().ToList();

                foreach (var e in results)
                {
                    DTO_LU_ClaimDocumentType o = new DTO_LU_ClaimDocumentType();
                    o.ClaimDocumentTypeID = e.ClaimDocumentTypeID;
                    o.ClaimDocumentType = e.ClaimDocumentType;

                    token.Add(o);
                }
            }
            return token;
        }

        public List<DTO_LU_ClaimStatusTypes> GetClaimStatusTypes()
        {
            List<DTO_LU_ClaimStatusTypes> token = new List<DTO_LU_ClaimStatusTypes>();

            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                var results = context.proc_Get_LU_ClaimStatusTypes().ToList();

                foreach (var e in results)
                {
                    DTO_LU_ClaimStatusTypes o = new DTO_LU_ClaimStatusTypes{
                        ClaimStatusTypeID = e.ClaimStatusTypeID,
                        ClaimStatusType = e.ClaimStatusType
                    };

                    token.Add(o);
                }
            }
            return token;
        }

        public List<DTO_LU_DamageTypes> GetDamageTypes()
        {
            List<DTO_LU_DamageTypes> token = new List<DTO_LU_DamageTypes>();
            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                var results = context.proc_Get_LU_DamageTypes().ToList();

                foreach (var e in results)
                {
                    DTO_LU_DamageTypes o = new DTO_LU_DamageTypes();
                    o.DamageTypeID = e.DamageTypeID;
                    o.DamageType = e.DamageType;

                    token.Add(o);
                }
            }
            return token;
        }

        public List<DTO_LU_EmployeeType> GetEmployeeTypes()
        {
            List<DTO_LU_EmployeeType> token = new List<DTO_LU_EmployeeType>();

            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                var results = context.proc_Get_LU_EmployeeTypes().ToList();

                foreach (var e in results)
                {
                    DTO_LU_EmployeeType o = new DTO_LU_EmployeeType();
                    o.EmployeeTypeID = e.EmployeeTypeID;
                    o.EmployeeType = e.EmployeeType;

                    token.Add(o);
                }
            }
            return token;
        }

        public List<DTO_LU_InvoiceType> GetInvoiceTypes()
        {
            List<DTO_LU_InvoiceType> token = new List<DTO_LU_InvoiceType>();

            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                var results = context.proc_Get_LU_InvoiceTypes().ToList();

                foreach (var e in results)
                {
                    DTO_LU_InvoiceType o = new DTO_LU_InvoiceType();
                    o.InvoiceTypeID = e.InvoiceTypeID;
                    o.InvoiceType = e.InvoiceType;

                    token.Add(o);
                }
            }
            return token;
        }

        public List<DTO_LU_KnockResponseType> GetKnockResponseTypes()
        {
            List<DTO_LU_KnockResponseType> token = new List<DTO_LU_KnockResponseType>();

            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                var results = context.proc_Get_LU_KnockResponseTypes().ToList();

                foreach (var e in results)
                {
                    DTO_LU_KnockResponseType o = new DTO_LU_KnockResponseType();
                    o.KnockResponseTypeID = e.KnockResponseTypeID;
                    o.KnockResponseType = e.KnockResponseType;

                    token.Add(o);
                }
            }
            return token;
        }

        public List<DTO_LU_LeadType> GetLeadTypes()
        {
            List<DTO_LU_LeadType> token = new List<DTO_LU_LeadType>();

            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                var results = context.proc_Get_LU_LeadTypes().ToList();

                foreach (var e in results)
                {
                    DTO_LU_LeadType o = new DTO_LU_LeadType();
                    o.LeadTypeID = e.LeadTypeID;
                    o.LeadType = e.LeadType;

                    token.Add(o);
                }
            }
            return token;
        }

        public List<DTO_LU_PayFrequncy> GetPayFrequencies()
        {
            List<DTO_LU_PayFrequncy> token = new List<DTO_LU_PayFrequncy>();

            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                var results = context.proc_Get_LU_PayFrequencies().ToList();

                foreach (var e in results)
                {
                    DTO_LU_PayFrequncy o = new DTO_LU_PayFrequncy();
                    o.PayFrequencyID = e.PayFrequencyID;
                    o.PayFrequency = e.PayFrequency;

                    token.Add(o);
                }
            }            
            return token;
        }

        public List<DTO_LU_PaymentDescription> GetPayDescriptions()
        {
            List<DTO_LU_PaymentDescription> token = new List<DTO_LU_PaymentDescription>();

            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {

                var results = context.proc_Get_LU_PaymentDescriptions().ToList();

                foreach (var e in results)
                {
                    DTO_LU_PaymentDescription o = new DTO_LU_PaymentDescription();
                    o.PaymentDescriptionID = e.PaymentDescriptionID;
                    o.PaymentDescription = e.PaymentDescription;

                    token.Add(o);
                }
            }
            return token;
        }

        public List<DTO_LU_PaymentType> GetPaymentTypes()
        {
            List<DTO_LU_PaymentType> token = new List<DTO_LU_PaymentType>();

            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                var results = context.proc_Get_LU_PaymentTypes().ToList();

                foreach (var e in results)
                {
                    DTO_LU_PaymentType o = new DTO_LU_PaymentType();
                    o.PaymentTypeID = e.PaymentTypeID;
                    o.PaymentType = e.PaymentType;

                    token.Add(o);
                }
            }
            return token;
        }

        public List<DTO_LU_PayType> GetPayTypes()
        {
            List<DTO_LU_PayType> token = new List<DTO_LU_PayType>();

            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                var results = context.proc_Get_LU_PayTypes().ToList();

                foreach (var e in results)
                {
                    DTO_LU_PayType o = new DTO_LU_PayType();
                    o.PayTypeID = e.PayTypeID;
                    o.PayType = e.PayType;

                    token.Add(o);
                }
            }
            return token;
        }

        public List<DTO_LU_Permissions> GetPermissions()
        {
            List<DTO_LU_Permissions> token = new List<DTO_LU_Permissions>();

            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                var results = context.proc_Get_LU_Permissions().ToList();

                foreach (var e in results)
                {
                    DTO_LU_Permissions o = new DTO_LU_Permissions();
                    o.PermissionID = e.PermissionID;
                    o.PerssionLevel = e.PermissionLevel;

                    token.Add(o);
                }
            }
            return token;
        }

        public List<DTO_LU_PlaneTypes> GetPlaneTypes()
        {
            List<DTO_LU_PlaneTypes> token = new List<DTO_LU_PlaneTypes>();
            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                var results = context.proc_Get_LU_PlaneTypes().ToList();

                foreach (var e in results)
                {
                    DTO_LU_PlaneTypes o = new DTO_LU_PlaneTypes();
                    o.PlaneTypeID = e.PlaneTypeID;
                    o.PlaneType = e.PlaneType;

                    token.Add(o);
                }
            }
            return token;
        }

        public List<DTO_LU_Product> GetProducts() 
        {
            List<DTO_LU_Product> token = new List<DTO_LU_Product>();

            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                var results = context.proc_Get_LU_Products().ToList();

                using (context)
                {
                    
                }

                foreach (var e in results)
                {
                    DTO_LU_Product o = new DTO_LU_Product();
                    o.ProductID = e.ProductID;
                    o.Name = e.Name;
                    o.ProductTypeID = e.ProductTypeID;
                    o.Color = e.Color;
                    o.Price = e.Price;
                    o.Info = e.Info;
                    o.VendorID = e.VendorID;
                    o.Warranty = e.Warranty;

                    token.Add(o);
                }
            }
            return token;
        }

        public List<DTO_LU_ProductType> GetProductTypes()
        {
            List<DTO_LU_ProductType> token = new List<DTO_LU_ProductType>();

            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                var results = context.proc_Get_LU_ProductTypes().ToList();

                foreach (var e in results)
                {
                    DTO_LU_ProductType o = new DTO_LU_ProductType();
                    o.ProductTypeID = e.ProductTypeID;
                    o.ProductType = e.ProductType;

                    token.Add(o);
                }
            }
            return token;
        }

        public List<DTO_LU_RidgeMaterialType> GetRidgeMaterialTypes()
        {
            List<DTO_LU_RidgeMaterialType> token = new List<DTO_LU_RidgeMaterialType>();

            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                var results = context.proc_Get_LU_RidgeMaterialTypes().ToList();

                foreach (var e in results)
                {
                    DTO_LU_RidgeMaterialType o = new DTO_LU_RidgeMaterialType();
                    o.RidgeMaterialTypeID = e.RidgeMaterialTypeID;
                    o.RidgeMaterialType = e.RidgeMaterialType;

                    token.Add(o);
                }
            }
            return token;
        }

        public List<DTO_LU_ScopeType> GetScopeTypes()
        {
            List<DTO_LU_ScopeType> token = new List<DTO_LU_ScopeType>();

            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                var results = context.proc_Get_LU_ScopeTypes().ToList();

                foreach (var e in results)
                {
                    DTO_LU_ScopeType o = new DTO_LU_ScopeType();
                    o.ScopeTypeID = e.ScopeTypeID;
                    o.ScopeType = e.ScopeType;

                    token.Add(o);
                }
            }
            return token;
        }

        public List<DTO_LU_ServiceTypes> GetServiceTypes()
        {
            List<DTO_LU_ServiceTypes> token = new List<DTO_LU_ServiceTypes>();
            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                var results = context.proc_Get_LU_ServiceTypes().ToList();

                foreach (var e in results)
                {
                    DTO_LU_ServiceTypes o = new DTO_LU_ServiceTypes();
                    o.ServiceTypeID = e.ServiceTypeID;
                    o.ServiceType = e.ServiceType;

                    token.Add(o);
                }
            }
            return token;
        }

        public List<DTO_LU_ShingleType> GetShingleTypes()
        {
            List<DTO_LU_ShingleType> token = new List<DTO_LU_ShingleType>();

            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                var results = context.proc_Get_LU_ShingleTypes().ToList();

                foreach (var e in results)
                {
                    DTO_LU_ShingleType o = new DTO_LU_ShingleType();
                    o.ShingleTypeID = e.ShingleTypeID;
                    o.ShingleType = e.ShingleType;

                    token.Add(o);
                }
            }
            return token;
        }

        public List<DTO_LU_UnitOfMeasure> GetUnitsOfMeasure()
        {
            List<DTO_LU_UnitOfMeasure> token = new List<DTO_LU_UnitOfMeasure>();

            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                var results = context.proc_Get_LU_UnitOfMeasures().ToList();

                foreach (var e in results)
                {
                    DTO_LU_UnitOfMeasure o = new DTO_LU_UnitOfMeasure();
                    o.UnitOfMeasureID = e.UnitOfMeasureID;
                    o.UnitOfMeasure = e.UnitOfMeasure;

                    token.Add(o);
                }
            }
            return token;
        }

        public List<DTO_LU_VendorTypes> GetVendorTypes()
        {
            List<DTO_LU_VendorTypes> token = new List<DTO_LU_VendorTypes>();
            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                var results = context.proc_Get_LU_VendorTypes().ToList();

                foreach (var e in results)
                {
                    DTO_LU_VendorTypes o = new DTO_LU_VendorTypes();
                    o.VendorTypeID = e.VendorTypeID;
                    o.VendorType = e.VendorType;

                    token.Add(o);
                }
            }
            return token;
        }
        #endregion

        #region INTERNAL SERVICES
        private DTO_Base populateException(Exception ex, DTO_Base token)
        {
            if (ex.InnerException != null)
            {
                token.Message = ex.InnerException.Message;
            }
            else
            {
                token.Message = ex.Message + "\nPlease contact your system administrator.";

            }
            return token;
        }

        #region FTP
        private static bool ftpDirectoryExists(string directory)
        {

            try
            {
                var request = (FtpWebRequest)WebRequest.Create(directory);
                request.Credentials = new NetworkCredential(FTPUSERNAME, FTPPASSWORD);
                request.Method = WebRequestMethods.Ftp.ListDirectory;

                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
            }
            catch (WebException ex)
            {
                FtpWebResponse response = (FtpWebResponse)ex.Response;
                if (response.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable)
                    if (createFtpDirectory(directory))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                else
                    return true;
            }
            return true;

        }

        private static bool createFtpDirectory(string directory)
        {
            string path = directory;

            var address = Regex.Match(path, @"^(ftp://)?(\w*|.?)*/").Value.Replace("ftp://", "").Replace("/", "");
            var dirs = Regex.Split(path.Replace(address, "").Replace("ftp://", ""), "/").Where(x => x.Length > 0);

            string buildpath = FTPURL;

            WebRequest request;





            foreach (var dir in dirs)
            {
                try
                {
                    request = WebRequest.Create(buildpath + dir + "/");
                    request.Credentials = new NetworkCredential(FTPUSERNAME, FTPPASSWORD);
                    request.Method = WebRequestMethods.Ftp.MakeDirectory;
                    var resp = (FtpWebResponse)request.GetResponse();
                    buildpath += dir + "/";
                }
                catch (WebException ex)
                {
                    buildpath += dir + "/";
                }


            }

            try
            {
                request = (FtpWebRequest)WebRequest.Create(directory);
                request.Credentials = new NetworkCredential(FTPUSERNAME, FTPPASSWORD);
                request.Method = WebRequestMethods.Ftp.ListDirectory;
                var resp = (FtpWebResponse)request.GetResponse();
            }
            catch (WebException ex)
            {
                return false;
            }

            return true;
        }

        private static bool createDirectoryStructure(string mrnNumber)
        {
            string directory = FTPURL + mrnNumber + "/Inspection/Images";
            if (createFtpDirectory(directory))
            {
                directory = FTPURL + mrnNumber + "/Scopes";

                if (createFtpDirectory(directory))
                {
                    directory = FTPURL + mrnNumber + "/Invoice-Orders";

                    if (createFtpDirectory(directory))
                    {
                        directory = FTPURL + mrnNumber + "/Checks";
                        if (createFtpDirectory(directory))
                        {
                            return true;
                        }
                        else { return false; } //Could not create Checks directory
                    }
                    else { return false; } // Could not create Invoice-Orders directory
                }
                else { return false; } // Could not create Scopes directory
            }
            else { return false; } // Could not create Inspection/Images directories
        }

        private static bool downloadFile(ref DTO_ClaimDocument token)
        {
            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                try
                {
                    string mrnNumber = context.proc_GetClaimByClaimID(token.ClaimID).Single().MRNNumber;

                    string folder = mrnNumber + getFolderPathByDocTypeID(token.DocTypeID);

                    byte[] fileBytes;

                    if (ftpDirectoryExists(FTPURL + folder + "/"))
                    {
                        //getsize
                        var request = (FtpWebRequest)WebRequest.Create(FTPURL + folder + "/" + token.FileName + token.FileExt);
                        request.Method = WebRequestMethods.Ftp.GetFileSize;
                        request.Credentials = new NetworkCredential(FTPUSERNAME, "Scrappy!");
                        FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                        long size = response.ContentLength;
                        response.Close();

                        fileBytes = new byte[size];

                        //download
                        request = (FtpWebRequest)WebRequest.Create(FTPURL + folder + "/" + token.FileName + token.FileExt);
                        request.Method = WebRequestMethods.Ftp.DownloadFile;
                        request.Credentials = new NetworkCredential(FTPUSERNAME, "Scrappy!");

                        response = (FtpWebResponse)request.GetResponse();

                        Stream responseStream = response.GetResponseStream();

                        responseStream.Read(fileBytes, 0, fileBytes.Length);
                        response.Close();

                        token.FileBytes = Convert.ToBase64String(fileBytes);

                        return true;
                    }
                    return false;
                }
                catch
                {
                    return false;
                }
            }
        }

        private static bool uploadClaimDocument(ref DTO_ClaimDocument token)
        {
            using (MRNNexusTestEntities context = new MRNNexusTestEntities())
            {
                try
                {
                    string mrnNumber = context.proc_GetClaimByClaimID(token.ClaimID).Single().MRNNumber;

                    string folder = mrnNumber + getFolderPathByDocTypeID(token.DocTypeID);

                    byte[] fileBytes = Convert.FromBase64String(token.FileBytes);

                    if (ftpDirectoryExists(FTPURL + folder + "/"))
                    {
                        var request = (FtpWebRequest)WebRequest.Create(FTPURL + folder + "/" + token.FileName + token.FileExt);
                        request.Method = WebRequestMethods.Ftp.UploadFile;
                        request.Credentials = new NetworkCredential(FTPUSERNAME, FTPPASSWORD);
                        request.ContentLength = fileBytes.Length;

                        Stream requestStream = request.GetRequestStream();
                        requestStream.Write(fileBytes, 0, fileBytes.Length);
                        requestStream.Close();

                        FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                        response.Close();

                        token.FilePath = "services.mrncontracting.com/files/" + folder + "/";
                        return true;
                    }
                    else
                    {
                        throw new Exception();
                    }
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }

        private static string getFolderPathByDocTypeID(int docTypeID)
        {
            switch (docTypeID)
            {
                case 1:
                case 3:
                    return "/Inspection";
                case 2:
                case 4:
                    return "/Inspection/Images";
                case 5:
                case 17:
                case 18:
                case 19:
                case 20:
                case 21:
                case 22:
                    return "";
                case 6:
                case 7:
                case 8:
                    return "/Scopes";
                case 9:
                case 10:
                case 11:
                case 12:
                    return "/Invoice-Orders";
                case 13:
                case 14:
                case 15:
                case 16:
                    return "/Checks";
                default:
                    return "";
            };




        }
        #endregion

        #region PasswordHASH
        public string CreatePasswordHASH(DTO_Employee token)
        {
            var salt = GenerateRandomSalt();
            var iterationCount = 3;
            var hashValue = GenerateHashValue(token.LastName + token.CellPhone.Substring(token.CellPhone.Length - 4), salt, iterationCount);
            var iterationCountBtyeArr = BitConverter.GetBytes(iterationCount);
            var valueToSave = new byte[SaltByteLength + DerivedKeyLength + iterationCountBtyeArr.Length];
            Buffer.BlockCopy(salt, 0, valueToSave, 0, SaltByteLength);
            Buffer.BlockCopy(hashValue, 0, valueToSave, SaltByteLength, DerivedKeyLength);
            Buffer.BlockCopy(iterationCountBtyeArr, 0, valueToSave, salt.Length + hashValue.Length, iterationCountBtyeArr.Length);
            return Convert.ToBase64String(valueToSave);
        }
        
        private static byte[] GenerateRandomSalt()
        {
            var csprng = new RNGCryptoServiceProvider();
            var salt = new byte[SaltByteLength];
            csprng.GetBytes(salt);
            return salt;
        }

        private static byte[] GenerateHashValue(string password, byte[] salt, int iterationCount)
        {
            byte[] hashValue;
            var valueToHash = string.IsNullOrEmpty(password) ? string.Empty : password;
            using (var pbkdf2 = new Rfc2898DeriveBytes(valueToHash, salt, iterationCount))
            {
                hashValue = pbkdf2.GetBytes(DerivedKeyLength);
            }
            return hashValue;
        }

        private static bool VerifyPassword(string passwordGuess, string actualSavedHashResults)
        {
            //ingredient #1: password salt byte array
            var salt = new byte[SaltByteLength];

            //ingredient #2: byte array of password
            var actualPasswordByteArr = new byte[DerivedKeyLength];

            //convert actualSavedHashResults to byte array
            var actualSavedHashResultsBtyeArr = Convert.FromBase64String(actualSavedHashResults);

            //ingredient #3: iteration count
            var iterationCountLength = actualSavedHashResultsBtyeArr.Length - (salt.Length + actualPasswordByteArr.Length);
            var iterationCountByteArr = new byte[iterationCountLength];
            Buffer.BlockCopy(actualSavedHashResultsBtyeArr, 0, salt, 0, SaltByteLength);
            Buffer.BlockCopy(actualSavedHashResultsBtyeArr, SaltByteLength, actualPasswordByteArr, 0, actualPasswordByteArr.Length);
            Buffer.BlockCopy(actualSavedHashResultsBtyeArr, (salt.Length + actualPasswordByteArr.Length), iterationCountByteArr, 0, iterationCountLength);
            var passwordGuessByteArr = GenerateHashValue(passwordGuess, salt, BitConverter.ToInt32(iterationCountByteArr, 0));
            return ConstantTimeComparison(passwordGuessByteArr, actualPasswordByteArr);
        }

        private static bool ConstantTimeComparison(byte[] passwordGuess, byte[] actualPassword)
        {
            uint difference = (uint)passwordGuess.Length ^ (uint)actualPassword.Length;
            for (var i = 0; i < passwordGuess.Length && i < actualPassword.Length; i++)
            {
                difference |= (uint)(passwordGuess[i] ^ actualPassword[i]);
            }

            return difference == 0;
        }
        #endregion

        #endregion


    }
}
