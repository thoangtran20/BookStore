﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.Models.ViewModel
{
    public class ShoppingCartVM
    {
        public IEnumerable<ShoppingCart> ListCart {  get; set; }
        public double CartTotal { get; set; }
    }
}