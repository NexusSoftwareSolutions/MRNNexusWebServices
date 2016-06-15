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
    public class DTO_LU_PaymentDescription : DTO_Base
    {
        [DataMember]
        public int PaymentDescriptionID { get; set; }
        [DataMember]
        public string PaymentDescription { get; set; }
    }
}
