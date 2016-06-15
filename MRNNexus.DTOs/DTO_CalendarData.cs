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
    public class DTO_CalendarData : DTO_Base
    {
        [DataMember]
        public int EntryID { get; set; }
        [DataMember]
        public int EmployeeID { get; set; }
        [DataMember]
        public int AppointmentTypeID { get; set; }
        [DataMember]
        public Nullable<int> LeadID { get; set; }

        public DateTime StartTime { get; set; }
		[DataMember(Name = "StartTime")]
		private string StartTimeForSerialization { get; set; }
		[OnSerializing]
		void onSerializing(StreamingContext context)
		{
            if(StartTime != null)
			    this.StartTimeForSerialization = JsonConvert.SerializeObject(this.StartTime).Replace('"', ' ').Trim();

            if(EndTime != null)
                this.EndTimeForSerialization = JsonConvert.SerializeObject(this.EndTime).Replace('"', ' ').Trim();
        }
		[OnDeserialized]
		void OnDeserialized(StreamingContext context)
		{
            if(StartTimeForSerialization != null)
			    this.StartTime = DateTime.Parse(this.StartTimeForSerialization);

            if(EndTimeForSerialization != null)
                this.EndTime = DateTime.Parse(this.EndTimeForSerialization);
        }

        public DateTime EndTime { get; set; }
		[DataMember(Name = "EndTime")]
		private string EndTimeForSerialization { get; set; }

		[DataMember]
        public Nullable<int> ClaimID { get; set; }
        [DataMember]
        public string Note { get; set; }
    }
}
