using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bitboxx.DNNModules.BBStore
{
    [Flags]
    public enum ModuleKindEnum
    {
        Admin = 0,
        Product= 1,
        Cart = 2,
        ProductGroup = 4,
        ProductList = 8,
        FeatureList = 16,
        Contact = 32,
        MiniCart = 64,
        Search = 128
    }
}