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
    using System.Collections.Generic;
    
    public partial class ClaimContact
    {
        public int ClaimContactID { get; set; }
        public int ClaimID { get; set; }
        public int CustomerID { get; set; }
        public Nullable<int> KnockerID { get; set; }
        public int SalesPersonID { get; set; }
        public Nullable<int> SupervisorID { get; set; }
        public int SalesManagerID { get; set; }
        public Nullable<int> AdjusterID { get; set; }
    
        public virtual Adjuster Adjuster { get; set; }
        public virtual Claim Claim { get; set; }
        public virtual Customer Customer { get; set; }
        public virtual Employee Employee { get; set; }
        public virtual Employee Employee1 { get; set; }
        public virtual Employee Employee2 { get; set; }
        public virtual Employee Employee3 { get; set; }
    }
}