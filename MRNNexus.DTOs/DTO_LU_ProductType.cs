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
    public class DTO_LU_ProductType : DTO_Base
    {
        [DataMember]
        public int ProductTypeID { get; set; }
        [DataMember]
        public string ProductType { get; set; }
    }
}
