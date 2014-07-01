using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bitboxx.DNNModules.BBStore
{
    public class CartSumInfo
    {
        public CartSumInfo()
        {
            Text= "";
            Amount = 0.00m;
        }
        public CartSumInfo(string text, decimal amount)
        {
            Text= text;
            Amount = amount;
        }
        public string Text {get;set;}
        public decimal Amount {get; set;}
    }
}
