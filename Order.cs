using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRUD_CosmosDb
{
    public class Order
    {
        public string id { get; set; }
        public string orderID {  get; set; }
        public string category { get; set; }
        public string quantity { get; set; }
    }
}
