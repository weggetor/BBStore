using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Bitboxx.DNNModules.BBStore
{
    [Serializable]
    [DataContract()]
    public class AppUserInfo
    {
        //public AppUserInfo()
        //{
        //    UserName = "";
        //    FirstName = "";
        //    LastName = "";
        //    DisplayName = "";
        //    Email = "";
        //    Password = "";
        //    Secret = "";
        //}

        [DataMember()]
        public string UserName { get; set; }
        [DataMember()]
        public string FirstName { get; set; }
        [DataMember()]
        public string LastName { get; set; }
        [DataMember()]
        public string DisplayName { get; set; }
        [DataMember()]
        public string Email { get; set; }
        [DataMember()]
        public string Password { get; set; }
        [DataMember()]
        public string Secret { get; set; }

    }
}