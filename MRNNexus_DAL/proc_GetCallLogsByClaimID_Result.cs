//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MRNNexus_DAL
{
    using System;
    
    public partial class proc_GetCallLogsByClaimID_Result
    {
        public int CallLogID { get; set; }
        public int ClaimID { get; set; }
        public int EmployeeID { get; set; }
        public string WhoWasCalled { get; set; }
        public string ReasonForCall { get; set; }
        public string WhoAnswered { get; set; }
        public string CallResults { get; set; }
    }
}
