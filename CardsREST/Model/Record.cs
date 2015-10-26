using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace CardsREST.Model
{

    [DataContract]
    public class Record
    {
        [DataMember]
        public string id { get; set; }

        [DataMember]
        public string value { get; set; }
    }

}