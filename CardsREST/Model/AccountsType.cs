namespace CardsREST.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class AccountsType
    {
        public AccountsType()
        {
            this.Accounts = new HashSet<Account>();
        }
    
        public int accountType { get; set; }
        public string nname { get; set; }
        public Nullable<int> aactive { get; set; }
    
        public virtual ICollection<Account> Accounts { get; set; }
    }
}
