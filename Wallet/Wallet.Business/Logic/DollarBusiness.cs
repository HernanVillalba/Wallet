using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Wallet.Business.Operations;

namespace Wallet.Business.Logic
{
    public class DollarBusiness
    {
        public DollarBusiness()
        {
            Url = "https://www.dolarsi.com/api/api.php?type=valoresprincipales";
            Json = new WebClient().DownloadString(Url);
            List = JsonConvert.DeserializeObject<IEnumerable<Root>>(Json);
        }

        public string Url { get; }
        public string Json { get; }
        public IEnumerable<Root> List { get; }

        public Root GetDollarByName(string name)
        {
            if (name != null && List != null)
            {
                var dollar = List.FirstOrDefault(e=>e.Casa.Nombre.ToLower() == name.ToLower() );
                if(dollar != null) { return dollar; }
                else { return null; }
            }
            else { return null; }
        }
    }
}
