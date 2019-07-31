using CakeShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CakeShop.BindingModels
{
    public class TransactionBindingModel
    {
        public int TransactionId { get; set; }

        public int CakeId { get; set; }

        public string Buyer { get; set; }

        public int Quantity { get; set; }

        public virtual Cake Cake { get; set; }
    }
}
