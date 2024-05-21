using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace kidyx.com.Models
{
    public class ordermanagement
    {
       
        public string user_pname { get; set; }
        public string user_fname { get; set; }
        public string email { get; set; }
        public string address1 { get; set; }
        public System.DateTime datet { get; set; }
        public string phoneno { get; set; }
        public string zipcode { get; set; }
    }
}