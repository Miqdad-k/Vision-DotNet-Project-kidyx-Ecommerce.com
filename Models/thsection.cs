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
    
    public partial class thsection
    {
        public int Id { get; set; }
        public string picture { get; set; }
        public string name { get; set; }
        public string link { get; set; }
        public int catagoryid { get; set; }
    
        public virtual catagory catagory { get; set; }
    }
}
