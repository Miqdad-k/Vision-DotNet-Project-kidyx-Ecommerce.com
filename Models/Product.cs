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
    
    public partial class Product
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Product()
        {
            this.productlist = new HashSet<productlist>();
            this.Wishlist = new HashSet<Wishlist>();
        }
    
        public int P_id { get; set; }
        public string P_name { get; set; }
        public string P_brand { get; set; }
        public int P_retailprice { get; set; }
        public Nullable<int> P_discprice { get; set; }
        public string P_color { get; set; }
        public int P_quantity { get; set; }
        public string P_discription { get; set; }
        public int P_size { get; set; }
        public string P_img1 { get; set; }
        public string P_img2 { get; set; }
        public string P_img3 { get; set; }
        public string P_img4 { get; set; }
        public string P_img5 { get; set; }
        public Nullable<int> P_cid { get; set; }
        public string P_c_id { get; set; }
    
        public virtual catagory catagory { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<productlist> productlist { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Wishlist> Wishlist { get; set; }
    }
}
