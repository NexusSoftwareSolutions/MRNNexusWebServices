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
    public class DTO_LU_Product : DTO_Base
    {
        [DataMember]
        public int ProductID { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public int ProductTypeID { get; set; }
        [DataMember]
        public string Color { get; set; }

        [DataMember]
        public double Price { get; set; }
        [DataMember]
        public int Quantity { get; set; }
        [DataMember]
        public string Info { get; set; }
        [DataMember]
        public int VendorID { get; set; }
        [DataMember]
        public string Warranty { get; set; }
    }
}
