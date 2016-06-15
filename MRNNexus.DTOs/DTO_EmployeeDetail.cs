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
    public class DTO_EmployeeDetail : DTO_Base
    {
        [DataMember]
        public int EmployeeDetailsID { get; set; }
        [DataMember]
        public int EmployeeID { get; set; }
        [DataMember]
        public string MailingAddress { get; set; }
        [DataMember]
        public string Zip { get; set; }
                
        public DateTime DateHired { get; set; }
		[DataMember(Name = "DateHired")]
		private string DateHiredForSerialization { get; set; }
		[OnSerializing]
		void onSerializing(StreamingContext context)
		{
            if(DateHired != null)
			    this.DateHiredForSerialization = JsonConvert.SerializeObject(this.DateHired).Replace('"', ' ').Trim();

            if(DateReleased != null)
                this.DateReleasedForSerialization = JsonConvert.SerializeObject(this.DateReleased).Replace('"', ' ').Trim();
        }
		[OnDeserialized]
		void OnDeserialized(StreamingContext context)
		{
            if(DateHiredForSerialization != null)
			    this.DateHired = DateTime.Parse(this.DateHiredForSerialization);

            if (DateReleasedForSerialization != null)
                this.DateReleased = DateTime.Parse(this.DateReleasedForSerialization);
        }

        public Nullable<DateTime> DateReleased { get; set; }
		[DataMember(Name = "DateReleased")]
		private string DateReleasedForSerialization { get; set; }
		
		[DataMember]
        public string ShirtSize { get; set; }
        [DataMember]
        public Nullable<double> PayRate { get; set; }
        [DataMember]
        public Nullable<int> PayTypeID { get; set; }
        [DataMember]
        public Nullable<int> PayFrequencyID { get; set; }
        [DataMember]
        public bool PreviousEmployee { get; set; }
        [DataMember]
        public string DLPhotoPath { get; set; }
        [DataMember]
        public string CompanyPhotoPath { get; set; }
        [DataMember]
        public string SignaturePath { get; set; }
    }
}
