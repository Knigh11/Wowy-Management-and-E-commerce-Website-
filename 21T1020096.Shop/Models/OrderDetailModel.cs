﻿using SV21T1020096.DomainModels;
namespace SV21T1020096.Shop.Models
{
    public class OrderDetailModel
    {
        public Order? Order { get; set; }
        public required List<OrderDetail> Details { get; set; }
    }
}
