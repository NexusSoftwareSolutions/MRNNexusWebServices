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
    
    public partial class proc_GetAllCalendarData_Result
    {
        public int EntryID { get; set; }
        public int AppointmentTypeID { get; set; }
        public int EmployeeID { get; set; }
        public System.DateTime StartTime { get; set; }
        public System.DateTime EndTime { get; set; }
        public Nullable<int> ClaimID { get; set; }
        public Nullable<int> LeadID { get; set; }
        public string Note { get; set; }
    }
}
