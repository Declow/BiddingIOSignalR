

$(function () {
    // Reference the auto-generated proxy for the hub.
    var hub = $.connection.biddingHub;
    $.connection.hub.logging = true;

    // Create a function that the hub can call back to display messages.


    hub.client.getAuction = function (item) {

        console.log("Getting auction: " + item.Id);

        document.getElementById("auctionDetails").style.display = "block";
        document.getElementById("auctionNotYet").style.display = "none";

        var id = document.getElementById("auctionId");
        var title = document.getElementById("auctionTitle");
        var desc = document.getElementById("auctionDesc");
        var created = document.getElementById("auctionCreateDate");
        var end = document.getElementById("auctionEndDate");
        var quantity = document.getElementById("auctionQuantity");
        var price = document.getElementById("auctionPrice");
        
        console.log(item);

        id = parseInt(item.Id);
        title.innerText = item.Title;

        desc.innerText = item.desc;
        created.innerText = item.CreationTime;
        end.innerText = item.AuctionEnd;
        quantity.innerText = item.Quantity;
        price.innerText = item.CurrentBid;

        (function () {
            // Call the Send method on the hub.

            var userBidField = document.getElementById('userBid');
            var userBid = parseInt(userBidField.value);

            $("#btnSubmit").unbind().click(function () {
                console.log("Placed bid: " + item.Id);
                var bid = $("#userBid").val();

                console.log("Id: " + item.Id + "\n userBid: " + bid);
                if (!isNaN(bid)) {
                    hub.server.bidIncrease(item.Id, bid);
                }
            });
        })();    
    };

    hub.client.getAllAuctions = function (auctionList) {
        auctionList = JSON.parse(auctionList);
        // Add the message to the page.
        var htmlList = document.getElementById("auctionList");
        htmlList.innerHTML = "";
        var int = 0;
        for (var i in auctionList) {
            var item = auctionList[i];
            $('#auctionList').append(
                '<tr scope="row">' +
                '<td>' + item.Id + '</td>' +
                '<td>' + item.Title + '</td>' +
                '<td>' + item.CurrentBid + '</td>' +
                '<td>' + new Date(parseInt(item.AuctionEnd.replace("/Date(", "").replace(")/",""), 10)) + '</td>' +
                '<td>' + "<button type='button' id='btn" + int + "' class='btn btn-primary'>See more</button>" + '</td>' +
                '</tr>');
            int += 1;
        }

        //Create a function for each button
        for (i in auctionList) {
            (function (i) {
                $("#btn" + i).click(function () {
                    console.log("Getting auction with Id: " + i);
                    hub.server.getAuction(i);
                });
            })(i);
        };
    };

    $.connection.hub.start().done(function () {
        (function () {
            console.log("Connected, Transport: " + $.connection.hub.transport.name);
            // Call the Send method on the hub.
            hub.server.getAllAuctions();
        })();
    });

    $("#btnCreate").click(function () {
        console.log("Creating auction");
        
        var title = $("#setTitle").val();
        var desc = $("#setDescription").val();
        var date = $("#setEnding").val();
        var quantity = $("#setQuantity").val();
        var price = $("#minAmount").val();

        hub.server.createNewAuction(
            title,
            desc,
            date,
            quantity,
            price
            );
    });
});