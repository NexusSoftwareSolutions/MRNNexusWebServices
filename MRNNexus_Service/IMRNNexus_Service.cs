using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using MRNNexusDTOs;

using Newtonsoft.Json.Linq;
using System.Collections;

namespace MRNNexus_Service
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IMRNNexus_Service" in both code and config file together.
    [ServiceContract]
    public interface IMRNNexus_Service
    {
        #region DoWork Test
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/DoWork", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DTO_Employee DoWork(DTO_Employee token);


        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/DoMoreWork", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        void DoMoreWork();
        #endregion DoWork Test

        /*NEED TESTING*/

        /*COMPLETED*/
        #region Login
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/Login", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DTO_Employee Login(DTO_User token);
        #endregion

        #region GetUser
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/GetUser", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DTO_User GetUser(DTO_User token);
        #endregion

        #region RegisterUserAccount
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/RegisterUser", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DTO_User RegisterUser(DTO_User token);
        #endregion RegisterUserAccount

        #region ADDS

        #region AddAdditionalSupply
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/AddAdditionalSupply", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DTO_AdditionalSupply AddAdditionalSupply(DTO_AdditionalSupply token);
        #endregion AddAdditionalSupply

        #region AddAddress
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/AddAddress", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DTO_Address AddAddress(DTO_Address token);
        #endregion AddAddress

        #region AddAdjuster
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/AddAdjuster", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DTO_Adjuster AddAdjuster(DTO_Adjuster token);
        #endregion AddAdjuster

        #region AddAdjustment
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/AddAdjustment", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DTO_Adjustment AddAdjustment(DTO_Adjustment token);
        #endregion AddAdjustment

        #region AddCalendarData
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/AddCalendarData", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DTO_CalendarData AddCalendarData(DTO_CalendarData token);
        #endregion AddCalendarData

        #region AddCallLog
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/AddCallLog", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DTO_CallLog AddCallLog(DTO_CallLog token);
        #endregion AddCallLog

        #region AddClaim
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/AddClaim", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DTO_Claim AddClaim(DTO_Claim token);
        #endregion AddClaim

        #region AddClaimContacts
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/AddClaimContacts", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DTO_ClaimContacts AddClaimContacts(DTO_ClaimContacts token);
        #endregion AddClaimContacts

        #region AddClaimDocument
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/AddClaimDocument", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DTO_ClaimDocument AddClaimDocument(DTO_ClaimDocument token);
        #endregion AddClaimDocument

        #region AddClaimStatus
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/AddClaimStatus", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DTO_ClaimStatus AddClaimStatus(DTO_ClaimStatus token);
        #endregion

        #region AddClaimVendor
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/AddClaimVendor", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DTO_ClaimVendor AddClaimVendor(DTO_ClaimVendor token);
        #endregion AddClaimVendor

        #region AddCustomer
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/AddCustomer", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DTO_Customer AddCustomer(DTO_Customer token);
        #endregion AddCustomer

        #region AddDamage
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/AddDamage", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DTO_Damage AddDamage(DTO_Damage token);
        #endregion AddDamage

        #region AddEmployee
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/AddEmployee", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DTO_Employee AddEmployee(DTO_Employee token);
        #endregion AddEmployee

        #region AddInspection
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/AddInspection", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DTO_Inspection AddInspection(DTO_Inspection token);
        #endregion AddInspection

        #region AddInsuranceCompany
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/AddInsuranceCompany", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DTO_InsuranceCompany AddInsuranceCompany(DTO_InsuranceCompany token);
        #endregion AddInsuranceCompany

        #region AddInvoice
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/AddInvoice", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DTO_Invoice AddInvoice(DTO_Invoice token);
        #endregion AddInvoice

        #region AddKnockerResponse
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/AddKnockerResponse", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DTO_KnockerResponse AddKnockerResponse(DTO_KnockerResponse token);
        #endregion AddKNockerResponse

        #region AddLead
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/AddLead", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DTO_Lead AddLead(DTO_Lead token);
        #endregion AddLead

        #region AddNewRoof
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/AddNewRoof", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DTO_NewRoof AddNewRoof(DTO_NewRoof token);
        #endregion AddNewRoof

        #region AddOrderItem
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/AddOrderItem", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DTO_OrderItem AddOrderItem(DTO_OrderItem token);
        #endregion AddOrderItem

        #region AddOrder
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/AddOrder", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DTO_Order AddOrder(DTO_Order token);
        #endregion AddOrder

        #region AddPayment
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/AddPayment", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DTO_Payment AddPayment(DTO_Payment token);
        #endregion AddPayment

        #region AddPlane
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/AddPlane", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DTO_Plane AddPlane(DTO_Plane token);
        #endregion AddPlane

        #region AddReferrer
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/AddReferrer", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DTO_Referrer AddReferrer(DTO_Referrer token);
        #endregion AddReferrer

        #region AddScope
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/AddScope", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DTO_Scope AddScope(DTO_Scope token);
        #endregion AddScope

        #region AddSurplusSupplies
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/AddSurplusSupplies", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DTO_SurplusSupplies AddSurplusSupplies(DTO_SurplusSupplies token);
        #endregion AddSurplusSupplies

        #region AddVendor
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/AddVendor", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DTO_Vendor AddVendor(DTO_Vendor token);
        #endregion AddVendor

        #endregion

        #region GETS

        #region GetAdditionalSuppliesByClaimID
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/GetAdditionalSuppliesByClaimID", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<DTO_AdditionalSupply> GetAdditionalSuppliesByClaimID(DTO_Claim token);
        #endregion GetAdditionalSuppliesByClaimID

        #region GetAddressByID
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/GetAddressByID", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DTO_Address GetAddressByID(DTO_Address token);
        #endregion GetAddressByID

        #region GetAddressesBySalesPersonID
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/GetAddressesBySalesPersonID", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<DTO_Address> GetAddressesBySalesPersonID(DTO_Employee token);
        #endregion GetAddressesBySalesPersonID

        #region GetAdjusterByID
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/GetAdjusterByID", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DTO_Adjuster GetAdjusterByID(DTO_Adjuster token);
        #endregion GetAdjusterByID

        #region GetAdjustmentsByAdjusterID
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/GetAdjustmentsByAdjusterID", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<DTO_Adjustment> GetAdjustmentsByAdjusterID(DTO_Adjuster token);
        #endregion GetAdjustmentsByAdjusterID

        #region GetAdjustmentsByClaimID
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/GetAdjustmentsByClaimID", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<DTO_Adjustment> GetAdjustmentsByClaimID(DTO_Claim token);
        #endregion GetAdjustmentsByClaimID

        #region GetCalendarDataByEmployeeID
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/GetCalendarDataByEmployeeID", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<DTO_CalendarData> GetCalendarDataByEmployeeID(DTO_Employee token);
        #endregion GetCalendarDataByEmployeeID

        #region GetCallLogsByCalimID
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/GetCallLogsByClaimID", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<DTO_CallLog> GetCallLogsByClaimID(DTO_Claim token);
        #endregion GetCallLogsByCalimID

        #region GetClaimByClaimID
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/GetClaimByClaimID", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DTO_Claim GetClaimByClaimID(DTO_Claim token);
        #endregion GetClaimByClaimID

        #region GetClaimContactsByClaimID
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/GetClaimContactsByClaimID", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DTO_ClaimContacts GetClaimContactsByClaimID(DTO_Claim token);
        #endregion GetClaimContactsByClaimID

        #region GetClaimDocumentsByClaimID
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/GetClaimDocumentsByClaimID", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<DTO_ClaimDocument> GetClaimDocumentsByClaimID(DTO_Claim token);
        #endregion GetClaimDocumentsByClaimID

        #region GetClaimStatusByClaimID
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/GetClaimStatusByClaimID", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<DTO_ClaimStatus> GetClaimStatusByClaimID(DTO_Claim token);
        #endregion GetClaimStatusByClaimID

        #region GetClaimStatusDateByTypeIDAndClaimID
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/GetClaimStatusDateByTypeIDAndClaimID", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DTO_ClaimStatus GetClaimStatusDateByTypeIDAndClaimID(DTO_ClaimStatus token);
        #endregion GetClaimStatusDateByTypeIDAndClaimID

        #region GetClaimVendorsByClaimID
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/GetClaimVendorsByClaimID", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<DTO_ClaimVendor> GetClaimVendorsByClaimID(DTO_Claim token);
        #endregion GetClaimVendorsByClaimID

        #region GetClosedClaimsBySalesPersonID
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/GetClosedClaimsBySalesPersonID", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<DTO_Claim> GetClosedClaimsBySalesPersonID(DTO_Employee token);
        #endregion GetClosedClaimsBySalesPersonID

        #region GetCustomerByID
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/GetCustomerByID", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DTO_Customer GetCustomerByID(DTO_Customer token);
        #endregion GetCustomerByID

        #region GetCustomersBySalesPersonID
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/GetCustomersBySalesPersonID", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<DTO_Customer> GetCustomersBySalesPersonID(DTO_Employee token);
        #endregion GetCustomersBySalesPersonID

        #region GetEmployeesBtEmployeeTypeID
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/GetEmployeesByEmployeeTypeID", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<DTO_Employee> GetEmployeesByEmployeeTypeID(DTO_LU_EmployeeType token);
        #endregion GetEmployeesByEmployeeTypeID

        #region GetEmployeeByID
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/GetEmployeeByID", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DTO_Employee GetEmployeeByID(DTO_Employee token);
        #endregion

        #region GetInactiveClaimsBySalesPersonID
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/GetInactiveClaimsBySalesPersonID", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<DTO_Claim> GetInactiveClaimsBySalesPersonID(DTO_Employee token);
        #endregion GetInactiveClaimsBySalesPersonID

        #region GetInspectionByID
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/GetInspectionByID", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DTO_Inspection GetInspectionByID(DTO_Inspection token);
        #endregion GetInspectionByID

        #region GetInspectionsByClaimID
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/GetInspectionsByClaimID", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<DTO_Inspection> GetInspectionsByClaimID(DTO_Claim token);
        #endregion GetInspectionsByClaimID

        #region GetInsuranceCompanyByID
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/GetInsuranceCompanyByID", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DTO_InsuranceCompany GetInsuranceCompanyByID(DTO_InsuranceCompany token);
        #endregion GetInsuranceCompanyByID

        #region GetInvoicesByClaimID
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/GetInvoicesByClaimID", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<DTO_Invoice> GetInvoicesByClaimID(DTO_Claim token);
        #endregion GetInvoicesByClaimID

        #region GetKnockerResponseByID
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/GetKnockerResponseByID", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DTO_KnockerResponse GetKnockerResponseByID(DTO_KnockerResponse token);
        #endregion GetKnockerResponseByID

        #region GetKnockerResponsesByKnockerID
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/GetKnockerResponsesByKnockerID", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<DTO_KnockerResponse> GetKnockerResponsesByKnockerID(DTO_Employee token);
        #endregion GetKnockerResponsesByKnockerID

        #region GetKnockerResponsesByTypeID
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/GetKnockerResponsesByTypeID", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<DTO_KnockerResponse> GetKnockerResponsesByTypeID(DTO_LU_KnockResponseType token);
        #endregion GetKnockerResponsesByTypeID

        #region GetLeadByLeadID
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/GetLeadByLeadID", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DTO_Lead GetLeadByLeadID(DTO_Lead token);
        #endregion GetLeadByID

        #region GetLeadsBySalesPersonID
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/GetLeadsBySalesPersonID", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<DTO_Lead> GetLeadsBySalesPersonID(DTO_Employee token);
        #endregion GetLeadsBySalesPersonID

        #region GetLeadsByStatus
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/GetLeadsByStatus", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<DTO_Lead> GetLeadsByStatus(DTO_Lead token);
        #endregion GetLeadsByStatus

        #region GetLeadsWithNoClaim
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/GetLeadsWithNoClaim", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<DTO_Lead> GetLeadsWithNoClaim();
        #endregion GetLeadsWithNoClaim

        #region GetMostRecentDateByClaimID
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/GetMostRecentDateByClaimID", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DTO_CalculatedResults GetMostRecentDateByClaimID(DTO_Claim token);
        #endregion GetMostRecentDateByClaimID

        #region GetNewRoofByClaimID
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/GetNewRoofByClaimID", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DTO_NewRoof GetNewRoofByClaimID(DTO_Claim token);
        #endregion GetNewRoofByClaimID

        #region GetOldLeadsBySalesPersonID
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/GetOldLeadsBySalesPersonID", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<DTO_Lead> GetOldLeadsBySalesPersonID(DTO_Lead token);
        #endregion GetOldLeadsBySalesPersonID

        #region GetOpenClaimsBySalesPersonID
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/GetOpenClaimsBySalesPersonID", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<DTO_Claim> GetOpenClaimsBySalesPersonID(DTO_Employee token);
        #endregion GetOpenClaimsBySalesPersonID

        #region GetOrderItemsByOrderID
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/GetOrderItemsByOrderID", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<DTO_OrderItem> GetOrderItemsByOrderID(DTO_Order token);
        #endregion GetOrderItemsByOrderID

        #region GetOrdersByClaimID
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/GetOrdersByClaimID", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<DTO_Order> GetOrdersByClaimID(DTO_Claim token);
        #endregion GetOrdersByClaimID

        #region GetOtherClaimsToSchedule
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/GetOtherClaimsToSchedule", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<DTO_Scope> GetOtherClaimsToSchedule();
        #endregion GetOtherClaimsToSchedule

        #region GetPaymentsByClaimID
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/GetPaymentsByClaimID", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<DTO_Payment> GetPaymentsByClaimID(DTO_Claim token);
        #endregion GetPaymentsByClaimID

        #region GetPlanesByInspectionID
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/GetPlanesByInspectionID", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<DTO_Plane> GetPlanesByInspectionID(DTO_Inspection token);
        #endregion GetPlanesByInspectionID

        #region GetReferrerByID
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/GetReferrerByID", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DTO_Referrer GetReferrerByID(DTO_Referrer token);
        #endregion GetReferrerByID

        #region GetRoofClaimsToSchedule
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/GetRoofClaimsToSchedule", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<DTO_Scope> GetRoofClaimsToSchedule();
        #endregion GetRoofClaimsToSchedule

        #region GetScopesByClaimID
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/GetScopesByClaimID", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<DTO_Scope> GetScopesByClaimID(DTO_Claim token);
        #endregion GetScopesByClaimID

        #region GetSumOfInvoicesByClaimID
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/GetSumOfInvoicesByClaimID", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DTO_CalculatedResults GetSumOfInvoicesByClaimID(DTO_Claim token);
        #endregion GetSumOfInvoicesByClaimID

        #region GetSumOfPaymentsByClaimID
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/GetSumOfPaymentsByClaimID", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DTO_CalculatedResults GetSumOfPaymentsByClaimID(DTO_Claim token);
        #endregion GetSumOfPaymentsByClaimID

        #region GetSurplusSuppliesByClaimID
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/GetSurplusSuppliesByClaimID", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<DTO_SurplusSupplies> GetSurplusSuppliesByClaimID(DTO_Claim token);
        #endregion GetSurplusSuppliesByClaimID

        #region GetVendorByID
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/GetVendorByID", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DTO_Vendor GetVendorByID(DTO_Vendor token);
        #endregion GetVendorByID

        #endregion GETS

        #region GET ALLS

        #region GetAllAdditionalSupplies
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/GetAllAdditionalSupplies", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<DTO_AdditionalSupply> GetAllAdditionalSupplies();
        #endregion GetAllAdditionalSupplies

        #region GetAllAddresses
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/GetAllAddresses", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<DTO_Address> GetAllAddresses();
        #endregion GetAllAddresses

        #region GetAllAdjusters
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/GetAllAdjusters", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<DTO_Adjuster> GetAllAdjusters();
        #endregion GetAllAdjusters

        #region GetAllAdjustments
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/GetAllAdjustments", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<DTO_Adjustment> GetAllAdjustments();
        #endregion GetAllAdjustments

        #region GetAllCalendarData
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/GetAllCalendarData", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<DTO_CalendarData> GetAllCalendarData();
        #endregion GetAllCalendarData

        #region GetAllCallLogs
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/GetAllCallLogs", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<DTO_CallLog> GetAllCallLogs();
        #endregion GetAllCallLogs

        #region GetAllClaims
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/GetAllClaims", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<DTO_Claim> GetAllClaims();
        #endregion GetAllClaims

        #region GetAllClaimsToSchedule
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/GetAllClaimsToSchedule", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<DTO_Scope> GetAllClaimsToSchedule();
        #endregion GetAllClaimsToSchedule

        #region GetAllClaimContacts
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/GetAllClaimContacts", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<DTO_ClaimContacts> GetAllClaimContacts();
        #endregion GetAllClaimContacts

        #region GetAllClaimDocuments
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/GetAllClaimDocuments", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<DTO_ClaimDocument> GetAllClaimDocuments();
        #endregion GetAllClaimDocuments

        #region GetAllClaimStatuses
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/GetAllClaimStatuses", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<DTO_ClaimStatus> GetAllClaimStatuses();
        #endregion GetAllClaimStatuses

        #region GetAllClaimVendors
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/GetAllClaimVendors", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<DTO_ClaimVendor> GetAllClaimVendors();
        #endregion GetAllClaimVendors

        #region GetAllCustomers
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/GetAllCustomers", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<DTO_Customer> GetAllCustomers();
        #endregion GetAllCustomers

        #region GetAllDamages
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/GetAllDamages", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<DTO_Damage> GetAllDamages();
        #endregion GetAllDamages

        #region GetAllEmployees
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/GetAllEmployees", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<DTO_Employee> GetAllEmployees();
        #endregion GetAllEmployees

        #region GetAllInspections
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/GetAllInspections", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<DTO_Inspection> GetAllInspections();
        #endregion GetAllInspections
        
        #region GetAllInsuranceCompanies
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/GetAllInsuranceCompanies", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<DTO_InsuranceCompany> GetAllInsuranceCompanies();
        #endregion GetAllInsuranceCompanies

        #region GetAllInsuranceCompanyNames
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/GetAllInsuranceCompanyNames", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<DTO_InsuranceCompany> GetAllInsuranceCompanyNames();
        #endregion GetAllInsuranceCompanyNames

        #region GetAllInvoices
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/GetAllInvoices", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<DTO_Invoice> GetAllInvoices();
        #endregion GetAllInvoices

        #region GetAllKnockerResponses
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/GetAllKnockerResponses", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<DTO_KnockerResponse> GetAllKnockerResponses();
        #endregion GetAllKnockerResponses

        #region GetAllLeads
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/GetAllLeads", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<DTO_Lead> GetAllLeads();
        #endregion GetAllLeads

        #region GetAllNewRoofs
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/GetAllNewRoofs", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<DTO_NewRoof> GetAllNewRoofs();
        #endregion GetAllNewRoofs

        #region GetAllOrderItems
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/GetAllOrderItems", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<DTO_OrderItem> GetAllOrderItems();
        #endregion GetAllOrderItems

        #region GetAllOrders
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/GetAllOrders", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<DTO_Order> GetAllOrders();
        #endregion GetAllOrders

        #region GetAllPayments
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/GetAllPayments", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<DTO_Payment> GetAllPayments();
        #endregion GetAllPayments

        #region GetAllPlanes
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/GetAllPlanes", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<DTO_Plane> GetAllPlanes();
        #endregion GetAllPlanes

        #region GetAllReferrers
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/GetAllReferrers", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<DTO_Referrer> GetAllReferrers();
        #endregion GetAllReferrers

        #region GetAllScopes
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/GetAllScopes", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<DTO_Scope> GetAllScopes();
        #endregion GetAllScopes

        #region GetAllSurplusSupplies
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/GetAllSurplusSupplies", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<DTO_SurplusSupplies> GetAllSurplusSupplies();
        #endregion GetAllSurplusSupplies

        #region GetAllUsers
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/GetAllUsers", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<DTO_User> GetAllUsers();
        #endregion GetAllUsers

        #region GetAllVendors
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/GetAllVendors", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<DTO_Vendor> GetAllVendors();
        #endregion GetAllVendors

        #endregion

        #region FILTERED GETS

        #region GetAllOpenClaims
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/GetAllOpenClaims", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<DTO_Claim> GetAllOpenClaims();
        #endregion GetAllOpenClaims

        #region GetAllClosedClaims
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/GetAllClosedClaims", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<DTO_Claim> GetAllClosedClaims();
        #endregion GetAllClosedClaims

        #region GetAllInactiveClaims
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/GetAllInactiveClaims", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<DTO_Claim> GetAllInactiveClaims();
        #endregion GetAllInactiveClaims

        #region GetRecentClaimsBySalesPersonID
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/GetRecentClaimsBySalesPersonID", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<DTO_Claim> GetRecentClaimsBySalesPersonID(DTO_Employee token);
        #endregion GetRecentClaimsBySalesPersonID

        #region GetRecentLeadsBySalesPersonID
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/GetRecentLeadsBySalesPersonID", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<DTO_Lead> GetRecentLeadsBySalesPersonID(DTO_Employee token);
        #endregion GetRecentLeadsBySalesPersonID

        #region GetRecentInspectionsBySalesPersonID
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/GetRecentInspectionsBySalesPersonID", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<DTO_Inspection> GetRecentInspectionsBySalesPersonID(DTO_Employee token);
        #endregion GetRecentInspectionsBySalesPersonID

        #endregion

        #region UPDATES
        #region UpdateAdditionalSupplies
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/UpdateAdditionalSupplies", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DTO_AdditionalSupply UpdateAdditionalSupplies(DTO_AdditionalSupply token);
        #endregion UpdateAdditionalSupplies

        #region UpdateAddress
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/UpdateAddress", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DTO_Address UpdateAddress (DTO_Address token);
        #endregion UpdateAddress

        #region UpdateAdjuster
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/UpdateAdjuster", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DTO_Adjuster UpdateAdjuster(DTO_Adjuster token);
        #endregion UpdateAdjuster

        #region UpdateAdjustment
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/UpdateAdjustment", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DTO_Adjustment UpdateAdjustment(DTO_Adjustment token);
        #endregion UpdateAdjustment

        #region UpdateCalendarData
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/UpdateCalendarData", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DTO_CalendarData UpdateCalendarData (DTO_CalendarData token);
        #endregion UpdateCalendarData

        #region UpdateClaim
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/UpdateClaim", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DTO_Claim UpdateClaim(DTO_Claim token);
        #endregion UpdateClaim

        #region UpdateClaimContacts
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/UpdateClaimContacts", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DTO_ClaimContacts UpdateClaimContacts(DTO_ClaimContacts token);
        #endregion UpdateClaimContacts

        #region UpdateClaimStatuses
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/UpdateClaimStatuses", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DTO_ClaimStatus UpdateClaimStatuses(DTO_ClaimStatus token);
        #endregion UpdateClaimStatuses

        #region UpdateCustomer
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/UpdateCustomer", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DTO_Customer UpdateCustomer(DTO_Customer token);
        #endregion UpdateCustomer

        #region UpdateDamage
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/UpdateDamage", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DTO_Damage UpdateDamage(DTO_Damage token);
        #endregion UpdateDamage

        #region UpdateEmployee
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/UpdateEmployee", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DTO_Employee UpdateEmployee(DTO_Employee token);
        #endregion UpdateEmployee

        #region UpdateInspection
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/UpdateInspection", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DTO_Inspection UpdateInspection(DTO_Inspection token);
        #endregion UpdateInspection

        #region UpdateInsuranceCompany
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/UpdateInsuranceCompany", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DTO_InsuranceCompany UpdateInsuranceCompany(DTO_InsuranceCompany token);
        #endregion UpdateInsuranceCompany

        #region UpdateInvoice
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/UpdateInvoice", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DTO_Invoice UpdateInvoice(DTO_Invoice token);
        #endregion UpdateInvoice

        #region UpdateKnockerResponse
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/UpdateKnockerResponse", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DTO_KnockerResponse UpdateKnockerResponse(DTO_KnockerResponse token);
        #endregion UpdateKnockerResponse

        #region UpdateLead
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/UpdateLead", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DTO_Lead UpdateLead(DTO_Lead token);
        #endregion UpdateLead

        #region UpdateNewRoof
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/UpdateNewRoof", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DTO_NewRoof UpdateNewRoof(DTO_NewRoof token);
        #endregion UpdateNewRoof

        #region UpdateOrder
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/UpdateOrder", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DTO_Order UpdateOrder(DTO_Order token);
        #endregion UpdateOrder

        #region UpdateOrderItem
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/UpdateOrderItem", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DTO_OrderItem UpdateOrderItem(DTO_OrderItem token);
        #endregion UpdateOrderItem

        #region UpdatePayment
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/UpdatePayment", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DTO_Payment UpdatePayment(DTO_Payment token);
        #endregion UpdatePayment

        #region UpdatePlane
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/UpdatePlane", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DTO_Plane UpdatePlane(DTO_Plane token);
        #endregion UpdatePlane

        #region UpdateReferrer
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/UpdateReferrer", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DTO_Referrer UpdateReferrer(DTO_Referrer token);
        #endregion UpdateReferrer

        #region UpdateScope
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/UpdateScope", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DTO_Scope UpdateScope(DTO_Scope token);
        #endregion UpdateScope

        #region UpdateSurplusSupplies
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/UpdateSurplusSupplies", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DTO_SurplusSupplies UpdateSurplusSupplies(DTO_SurplusSupplies token);
        #endregion UpdateSurplusSupplies

        #region UpdateUser
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/UpdateUser", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DTO_User UpdateUser(DTO_User token);
        #endregion UpdateUser

        #region UpdateVendor
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/UpdateVendor", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DTO_Vendor UpdateVendor(DTO_Vendor token);
        #endregion UpdateVendor


        #endregion UPDATES

        #region GetLookUpTables
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/GetAdjustmentResults", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<DTO_LU_AdjustmentResult> GetAdjustmentResults();

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/GetAppointmentTypes", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<DTO_LU_AppointmentTypes> GetAppointmentTypes();

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/GetClaimDocumentTypes", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<DTO_LU_ClaimDocumentType> GetClaimDocumentTypes();

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/GetClaimStatusTypes", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<DTO_LU_ClaimStatusTypes> GetClaimStatusTypes();
        
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/GetDamageTypes", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<DTO_LU_DamageTypes> GetDamageTypes();

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/GetEmployeeTypes", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<DTO_LU_EmployeeType> GetEmployeeTypes();

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/GetInvoiceTypes", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<DTO_LU_InvoiceType> GetInvoiceTypes();

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/GetKnockResponseTypes", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<DTO_LU_KnockResponseType> GetKnockResponseTypes();

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/GetLeadTypes", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<DTO_LU_LeadType> GetLeadTypes();

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/GetPayFrequencies", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<DTO_LU_PayFrequncy> GetPayFrequencies();

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/GetPayDescriptions", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<DTO_LU_PaymentDescription> GetPayDescriptions();

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/GetPaymentTypes", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<DTO_LU_PaymentType> GetPaymentTypes();

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/GetPayTypes", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<DTO_LU_PayType> GetPayTypes();

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/GetPermissions", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<DTO_LU_Permissions> GetPermissions();

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/GetPlaneTypes", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<DTO_LU_PlaneTypes> GetPlaneTypes();

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/GetProducts", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<DTO_LU_Product> GetProducts();

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/GetProductTypes", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<DTO_LU_ProductType> GetProductTypes();

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/GetRidgeMaterialTypes", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<DTO_LU_RidgeMaterialType> GetRidgeMaterialTypes();

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/GetScopeTypes", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<DTO_LU_ScopeType> GetScopeTypes();

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/GetServiceTypes", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<DTO_LU_ServiceTypes> GetServiceTypes();

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/GetShingleTypes", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<DTO_LU_ShingleType> GetShingleTypes();

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/GetUnitsOfMeasure", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<DTO_LU_UnitOfMeasure> GetUnitsOfMeasure();

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/GetVendorTypes", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<DTO_LU_VendorTypes> GetVendorTypes();

        #endregion

    }
}
