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
    public class DTO_Claim : DTO_Base
    {
        [DataMember]
        public int ClaimID { get; set; }

        [DataMember]
        public int LeadID { get; set; }

        [DataMember]
        public int CustomerID { get; set; }

        [DataMember]
        public int BillingID { get; set; }

        [DataMember]
        public int PropertyID { get; set; }

        [DataMember]
        public string MRNNumber { get; set; }
        
        public DateTime LossDate { get; set; }
		[DataMember(Name = "LossDate")]
		private string LossDateForSerialization { get; set; }
		[OnSerializing]
		void onSerializing(StreamingContext context)
		{
            if(LossDate != null)
			    this.LossDateForSerialization = JsonConvert.SerializeObject(this.LossDate).Replace('"', ' ').Trim();
		}
		[OnDeserialized]
		void OnDeserialized(StreamingContext context)
		{
            if(LossDateForSerialization != null)
			this.LossDate = DateTime.Parse(this.LossDateForSerialization);
		}

        [DataMember]
        public string MortgageCompany { get; set; }

        [DataMember]
        public string MortgageAccount { get; set; }

        [DataMember]
        public int InsuranceCompanyID { get; set; }

        [DataMember]
        public string InsuranceClaimNumber { get; set; }

        [DataMember]
        public bool IsOpen { get; set; }

        [DataMember]
        public bool ContractSigned { get; set; }
    }
}
