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
    public class DTO_ClaimContacts : DTO_Base
    {
        [DataMember]
        public int ClaimContactID { get; set; }

        [DataMember]
        public int ClaimID { get; set; }

        [DataMember]
        public  Nullable<int> KnockerID { get; set; }

        [DataMember]
        public int SalesPersonID { get; set; }

        [DataMember]
        public int SalesManagerID { get; set; }

        [DataMember]
        public int CustomerID { get; set; }

        [DataMember]
        public Nullable<int> SupervisorID { get; set; }

        [DataMember]
        public Nullable<int> AdjusterID { get; set; }
    }
}
