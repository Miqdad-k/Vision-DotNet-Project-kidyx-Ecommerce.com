//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace kidyx.com.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class shoporder
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public shoporder()
        {
            this.productlist = new HashSet<productlist>();
        }
    
        public string Id { get; set; }
        public string name { get; set; }
        public string fname { get; set; }
        public string email { get; set; }
        public string address { get; set; }
        public string phone { get; set; }
        public string zipcode { get; set; }
        public string userid { get; set; }
        public string bill_id { get; set; }
        public int total { get; set; }
        public string status { get; set; }
        public int sort { get; set; }
        public Nullable<int> reviewstatus { get; set; }
        public string amount_status { get; set; }
        public Nullable<System.DateTime> datetime { get; set; }
    
        public virtual AspNetUsers AspNetUsers { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<productlist> productlist { get; set; }
    }
}
