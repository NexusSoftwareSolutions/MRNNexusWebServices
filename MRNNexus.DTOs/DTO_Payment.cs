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
    public class DTO_Payment : DTO_Base
    {
        [DataMember]
        public int PaymentID { get; set; }
        [DataMember]
        public int ClaimID { get; set; }
        [DataMember]
        public int PaymentTypeID { get; set; }
        [DataMember]
        public int PaymentDescriptionID { get; set; }
        [DataMember]
        public double Amount { get; set; }
        
        public DateTime PaymentDate { get; set; }
		[DataMember(Name = "PaymentDate")]
		private string PaymentDateForSerialization { get; set; }
		[OnSerializing]
		void onSerializing(StreamingContext context)
		{
            if(PaymentDate != null)
			    this.PaymentDateForSerialization = JsonConvert.SerializeObject(this.PaymentDate).Replace('"', ' ').Trim();
		}
		[OnDeserialized]
		void OnDeserialized(StreamingContext context)
		{
            if(PaymentDateForSerialization != null)
			    this.PaymentDate = DateTime.Parse(this.PaymentDateForSerialization);
		}
	}
}
