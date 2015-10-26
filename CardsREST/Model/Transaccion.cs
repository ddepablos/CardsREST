using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace CardsREST.Model
{
    [DataContract]
    public class Transaccion
    {

        [DataMember]
        public string TransCode { get; set; }

        [DataMember]
        public string TransResponse { get; set; }

    }
}