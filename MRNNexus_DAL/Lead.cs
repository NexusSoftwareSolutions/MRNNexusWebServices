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
    
    public partial class Lead
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Lead()
        {
            this.CalendarDatas = new HashSet<CalendarData>();
            this.Claims = new HashSet<Claim>();
        }
    
        public int LeadID { get; set; }
        public int LeadTypeID { get; set; }
        public Nullable<int> KnockerResponseID { get; set; }
        public int SalesPersonID { get; set; }
        public int CustomerID { get; set; }
        public int AddressID { get; set; }
        public System.DateTime LeadDate { get; set; }
        public string Status { get; set; }
        public Nullable<int> CreditToID { get; set; }
        public string Temperature { get; set; }
    
        public virtual Address Address { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CalendarData> CalendarDatas { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Claim> Claims { get; set; }
        public virtual Customer Customer { get; set; }
        public virtual Employee Employee { get; set; }
        public virtual KnockerRespons KnockerRespons { get; set; }
        public virtual LU_LeadTypes LU_LeadTypes { get; set; }
    }
}
