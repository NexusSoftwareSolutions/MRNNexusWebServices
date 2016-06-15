using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace MRNNexusDTOs
{
    [KnownType(typeof(DTO_Base))]
    [DataContract]
    public class DTO_Adjustment : DTO_Base
    {
        [DataMember]
        public int AdjustmentID { get; set; }
        [DataMember]
        public int AdjusterID { get; set; }
        [DataMember]
        public int ClaimID { get; set; }
        [DataMember]
        public bool Gutters { get; set; }
        [DataMember]
        public bool Exterior { get; set; }
        [DataMember]
        public bool Interior { get; set; }

        public DateTime AdjustmentDate { get; set; }
		[DataMember(Name = "AdjustmentDate")]
		private string AdjustmentDateForSerialization { get; set; }
		[OnSerializing]
		void OnSerializing(StreamingContext context)
		{
            if(AdjustmentDate != null)
			    this.AdjustmentDateForSerialization = JsonConvert.SerializeObject(this.AdjustmentDate).Replace('"', ' ').Trim();
        }
		[OnDeserialized]
		void OnDeserialized(StreamingContext context)
		{
            if(AdjustmentDateForSerialization != null)
			    this.AdjustmentDate = DateTime.Parse(this.AdjustmentDateForSerialization);
		}

		[DataMember]
        public int AdjustmentResultID { get; set; }
        [DataMember]
        public string AdjustmentComment { get; set; }
    }
}
