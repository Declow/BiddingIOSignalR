using BiddingIOSignalR.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace BiddingIOSignalR
{
    public class ModelManager
    {
        private static ModelManager _instance;

        private List<Item> _AuctionList = new List<Item>();
        private int _CurrentId = 0;
        private Dictionary<int, List<string>> _GroupConn = new Dictionary<int, List<string>>();

        public static ModelManager GetInstance()
        {
            if (_instance == null) {
                _instance = new ModelManager();
                _instance.fillArrayOnStartup();
                return _instance;
            } else {
                return _instance;
            }
        }

        public List<Item> GetAuctions()
        {
            return _AuctionList;
        }

        //Add new auction to the list
        public void AddAuction(Item item)
        {
            item.Id = _CurrentId;
            _AuctionList.Add(item);
            _CurrentId += 1;
        }

        //Increase bid using Id and amount
        public bool IncreaseBid(int Id, int amount)
        {
            bool test = false;
            foreach (var item in _AuctionList) {
                if (item.Id == Id && item.CurrentBid < amount) {
                    item.CurrentBid = amount;
                    test = true;
                }
            }
            return test;
        }

        //Get auction from auction Id
        public Item GetAuction(int Id)
        {
            foreach (var item in _AuctionList) {
                if (item.Id == Id) {
                    return item;
                }
            }
            return null;
        }

        public void fillArrayOnStartup()
        {
            Random r = new Random();
            for (int i = 0; i < 3; i++) {
                
                Item item = new Item();
                item.Id = _CurrentId;
                item.Title = "Auction" + i;
                item.CurrentBid = r.Next(50);
                item.AuctionEnd = DateTime.Today.AddDays(i);
                item.desc = "Very good description" + i;
                item.Quantity = r.Next(50);
                item.CreationTime = DateTime.Today;

                Debug.WriteLine("DateTime: " + item.AuctionEnd);

                _AuctionList.Add(item);
                _CurrentId += 1;
            }
        }

        //Add client to auction group
        public void AddToGroup(int Id, string connId)
        {
            try {
                _GroupConn[Id].Add(connId);
            } catch {
                List<string> l = new List<string>();
                l.Add(connId);
                _GroupConn.Add(Id, l);
            }
        }

        //Remove client from auction group
        public void RemoveFromGroup(int Id, string connId)
        {
            _GroupConn[Id].Remove(connId);
        }

        //Checks specific group for client 
        public bool IsInGroup(int Id, string connId)
        {
            try {
                return _GroupConn[Id].Contains(connId);
            } catch {
                return false;
            }
        }

        //Check client for groups
        public int IsInGroup(string connId)
        {
            foreach (var i in _GroupConn) {
                if (i.Value.Contains(connId)) {
                    return i.Key;
                }
            }
            return -1;
        }
    }
}