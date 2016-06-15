using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace MRNNexusDTOs
{
    [KnownType(typeof(DTO_Base))]
    [DataContract]
    public class DTO_Vendor : DTO_Base
    {
        [DataMember]
        public int VendorID { get; set; }
        [DataMember]
        public int VendorTypeID { get; set; }
        [DataMember]
        public string CompanyName { get; set; }
        [DataMember]
        public string EIN { get; set; }
        [DataMember]
        public string ContactFirstName { get; set; }
        [DataMember]
        public string ContactLastName { get; set; }
        [DataMember]
        public string Suffix { get; set; }
        [DataMember]
        public string VendorAddress { get; set; }
        [DataMember]
        public string Zip { get; set; }
        [DataMember]
        public string Phone { get; set; }
        [DataMember]
        public string CompanyPhone { get; set; }
        [DataMember]
        public string Fax { get; set; }
        [DataMember]
        public string Email { get; set; }
        [DataMember]
        public string Website { get; set; }
        [DataMember]
        public DateTime? GeneralLiabilityExpiration { get; set; }
    }
}
