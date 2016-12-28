using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using BiddingIOSignalR.Model;
using System.Web.Script.Serialization;
using System.Diagnostics;
using System.Threading.Tasks;

namespace BiddingIOSignalR
{
    public class BiddingHub : Hub
    {
        private ModelManager manager = ModelManager.GetInstance();

        public void BidIncrease(int Id, int Amount)
        {
            Debug.WriteLine("Called bid");

            var result = manager.IncreaseBid(Id, Amount);

            if (result) {
                Clients.All.GetAllAuctions(new JavaScriptSerializer().Serialize(manager.GetAuctions()));
                Clients.Group(Id.ToString()).GetAuction(manager.GetAuction(Id));
            }
        }

        public Task GetAuction(int Id)
        {
            var item = manager.GetAuction(Id);
            if (item != null) {
                Clients.Caller.GetAuction(item);
                
                //Removes client from group. If the client is in any group
                RemoveFromGroup(Context.ConnectionId);

                //Adds the client to the group with the specified auction
                manager.AddToGroup(Id, Context.ConnectionId);
                return Groups.Add(Context.ConnectionId, Id.ToString());
              
            } else {
                Debug.WriteLine("Auction not found!");
            }
            return null;
        }

        public void GetAllAuctions()
        {
            Clients.Caller.GetAllAuctions(new JavaScriptSerializer().Serialize(manager.GetAuctions()));
        }

        public void CreateNewAuction(string title, string desc, string date, int quantity, int currentBid)
        {
            Item item = new Item();

            item.Title = title;
            item.desc = desc;
            item.AuctionEnd = DateTime.Parse(date);
            item.Quantity = quantity;
            item.CurrentBid = currentBid;
            item.CreationTime = DateTime.Now;
            
            manager.AddAuction(item);

            Clients.All.GetAllAuctions(new JavaScriptSerializer().Serialize(manager.GetAuctions()));
        }

        public void RemoveFromGroup(string connId)
        {
            int group = manager.IsInGroup(connId);
            if (group != -1) {
                Groups.Remove(Context.ConnectionId, group.ToString());
                manager.RemoveFromGroup(group, connId);
            }
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            RemoveFromGroup(Context.ConnectionId);

            return base.OnDisconnected(stopCalled);
        }
    }
}