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
    public class DTO_LU_ClaimDocumentType : DTO_Base
    {
        [DataMember]
        public int ClaimDocumentTypeID { get; set; }
        [DataMember]
        public string ClaimDocumentType { get; set; }
    }
}
