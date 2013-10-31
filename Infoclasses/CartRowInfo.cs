using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bitboxx.DNNModules.BBStore
{
    public class CartRowInfo
    {
        public CartRowInfo()
        {
            Description = "";
            NetTotal = 0.00m;
            TaxTotal = 0.00m;
            SubTotal = 0.00m;
        }
        public CartRowInfo(string desc, decimal nettotal, decimal taxtotal, decimal subtotal)
        {
            Description = desc;
            NetTotal = nettotal;
            TaxTotal = taxtotal;
            SubTotal = subtotal;
        }

        public string Description { get; set; }
        public decimal NetTotal { get; set; }
        public decimal TaxTotal { get; set; }
        public decimal SubTotal { get; set; }
    }
}

 