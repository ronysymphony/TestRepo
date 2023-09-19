using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shampan.Models
{
    public class PeramModel
    {
        public string UserLogInId { get; set; }  
        public string ItemNo { get; set; }
        public string TransactionType { get; set; }
        public string FromLocation { get; set; }
        public string ToLocation { get; set; }
    }
}
