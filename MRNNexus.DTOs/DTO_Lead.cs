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
    public class DTO_Lead : DTO_Base
    {
        [DataMember]
        public int LeadID { get; set; }
        [DataMember]
        public int LeadTypeID { get; set; }
        [DataMember]
        public int? KnockerResponseID { get; set; }
        [DataMember]
        public int SalesPersonID { get; set; }
        [DataMember]
        public int AddressID { get; set; }


        public DateTime LeadDate { get; set; }
		[DataMember(Name = "LeadDate")]
		private string LeadDateForSerialization { get; set; }
		[OnSerializing]
		void onSerializing(StreamingContext context)
		{
            if(LeadDate != null)
    			this.LeadDateForSerialization = JsonConvert.SerializeObject(this.LeadDate).Replace('"', ' ').Trim();
		}
		[OnDeserialized]
		void OnDeserialized(StreamingContext context)
		{
            if(LeadDateForSerialization != null)
			    this.LeadDate = DateTime.Parse(this.LeadDateForSerialization);
		}

        [DataMember]
        public int CustomerID { get; set; }
		[DataMember]
        public string Status { get; set; }
        [DataMember]
        public int? CreditToID { get; set; }
        [DataMember]
        public string Temperature { get; set; }

        // Special member for retrieving leads that are so old.
        [DataMember]
        public int? NumberOfDays { get; set; }
    }
}
