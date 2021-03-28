using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Wallet.Entities
{
    public class ErrorModel
    {
        public int status { get; set; }
        public Dictionary<string, List<string>> errors { get; set; }
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
