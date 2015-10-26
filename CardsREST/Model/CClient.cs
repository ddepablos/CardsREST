using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text;

namespace CardsREST.Model
{
    [DataContract]
    public class CClient
    {
        //[Key, Column(Order = 1)]
        [DataMember]
        public string numdoc { get; set; }
        [DataMember]
        public string nameclient { get; set; }
        [DataMember]
        public string pan { get; set; }
        [DataMember]
        public string printed { get; set; }
        [DataMember]
        public string estatus { get; set; }
        [DataMember]
        public string tarjeta { get; set; }

        /* Atributos de Manejo de Excepciones */
        [DataMember]
        public string excode { get; set; }
        [DataMember]
        public string exdetail { get; set; }

    }
}