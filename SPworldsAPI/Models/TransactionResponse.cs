using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPworldsAPI.Models
{
    public class TransactionResponse
    {
        public string Id { get; set; }
        public int Amount { get; set; }
        public string Comment { get; set; }
    }
}
