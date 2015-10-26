using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CardsREST.Model
{
    public class CBalance
    {
        public string numdoc { get; set; }
        public string pan { get;set;}
        public string accountnumber { get; set; }
        public string accounttype { get; set; }
        public string accountname { get; set; }
        public string saldo { get; set; }
        public string cardstatus { get; set; }
    }
}