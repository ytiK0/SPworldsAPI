using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPworldsAPI.Models
{
    public class TransactionResponse
    {
        public string id { get; set; }
        public int amount { get; set; }
        public string comment { get; set; }
    }
}
