namespace CardsREST.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class Account
    {
        public int accountID { get; set; }
        public string accountNumber { get; set; }
        public Nullable<int> accountType { get; set; }
        public Nullable<decimal> saldo { get; set; }
        public Nullable<int> cardID { get; set; }
        public Nullable<int> status { get; set; }
        public Nullable<int> RM { get; set; }
        public Nullable<int> puntos { get; set; }
        public Nullable<System.DateTime> upd_accounts { get; set; }
    
        public virtual AccountsType AccountsType { get; set; }
        public virtual Card Card { get; set; }
    }
}
