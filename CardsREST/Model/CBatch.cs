using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web;

namespace CardsREST.Model
{
    public class CBatch
    {
        [Key, Column(Order = 1)]
        public int batchid { get; set; }
        public string fecha { get; set; }
        public string pan { get; set; }
        public string transcode { get; set; }
        public string transname { get; set; }
        public string saldo { get; set; }
        public string puntos { get; set; }
        public string isodescription { get; set; }
        public string numdoc { get; set; }
        public int? transyear { get; set; }
        public int? transmonth { get; set; }
        public int? transday { get; set; }
        public string transdate { get; set; }
        public DateTime DateValue { get; set; }
    }
}