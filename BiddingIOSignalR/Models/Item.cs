using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BiddingIOSignalR.Model
{
    public class Item
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int CurrentBid { get; set; }
        public DateTime AuctionEnd { get; set; }
        public DateTime CreationTime { get; set; }
        public string desc { get; set; }
        public int Quantity { get; set; }
    }
}