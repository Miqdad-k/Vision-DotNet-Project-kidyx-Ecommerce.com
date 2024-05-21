using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace kidyx.com.Models
{
    public class cart
    {
        public int productid { get; set; }
        public string productname { get; set; }
        public string productpic { get; set; }
        public int productprice{ get; set; }
        public int qty { get; set; }
    }
}