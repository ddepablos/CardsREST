using System.Runtime.Serialization;

namespace CardsREST.Model
{
    [DataContract]
    public class Response
    {

        [DataMember]
        public string excode { get; set; }

        [DataMember]
        public string exdetail { get; set; }

        [DataMember]
        public string exsource { get; set; }

    }
}
