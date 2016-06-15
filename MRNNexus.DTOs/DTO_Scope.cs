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
    public class DTO_Scope : DTO_Base
    {
		[DataMember]
        public int ScopeID { get; set; }
		[DataMember]
		public int ScopeTypeID { get; set; }
		[DataMember]
		public int ClaimID { get; set; }
		[DataMember]
		public double Interior { get; set; }
		[DataMember]
		public double Exterior { get; set; }
		[DataMember]
		public double Gutter { get; set; }
		[DataMember]
		public double Tax { get; set; }
		[DataMember]
		public double Deductible { get; set; }
		[DataMember]
		public double Total { get; set; }
		[DataMember]
		public double OandP { get; set; }
        [DataMember]
        public double RoofAmount { get; set; }
        [DataMember]
        public bool Accepted { get; set; }

    }
}
