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
    public class DTO_OrderItem : DTO_Base
    {
        [DataMember]
        public int OrderItemID { get; set; }
        [DataMember]
        public int OrderID { get; set; }
        [DataMember]
        public int ProductID { get; set; }
        [DataMember]
        public int UnitOfMeasureID { get; set; }
        [DataMember]
        public int Quantity { get; set; }
    }
}
