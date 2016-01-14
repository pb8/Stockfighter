# Stockfighter
A C# client library for [Stockfighter](https://www.stockfighter.io/)

##About##
[Stockfighter](https://www.stockfighter.io/) is a code controlled stock trading game by [Starfighter](https://www.starfighters.io/).

This is a C# client library for their [REST APIs](https://starfighter.readme.io/). The trading, streaming/websocket and gm APIs all have been implemented.

Their naming conventions have been followed as far as possible, except when they use multiple names for the same thing in which case one version has been selected (e.g. symbol/ticker/stock and venue/exchange).

##Dependencies##
* .net 4.5 or newer
* Microsoft.AspNet.WebApi.Client
* Newtonsoft.Json
* WebSocket4Net

All dependencies are managed by nuget.

##Getting started##
###Trading API###

```csharp
using pb8.Stockfighter;
using pb8.Stockfighter.Model;
using pb8.Stockfighter.Enums;

string apiKey = "123456";

string account = "ADW30725379";
string venue = "TESTEX";
string symbol = "FOOBAR";

var api = new Api(apiKey);

var orderRequest = new OrderRequest()
{
	Account = account,
	Direction = Direction.Buy,
	OrderType = OrderType.Limit,
	Price = 1000,
	Quantity = 1,
	Symbol = symbol,
	Venue = venue
};

// submit an order
Order order = api.CreateOrder(orderRequest).Result;

// cancel it
api.CancelOrder(order);
```

###RealTime/WebSocket Feeds API###

```csharp
using pb8.Stockfighter;
using pb8.Stockfighter.Model;

string account = "ADW30725379";
string venue = "TESTEX";

// the Quotes feed streams live pricing information
var quotesFeed = new QuotesFeed(account, venue, reconnect:true);
quotesFeed.Started += (o, e) => Debug.WriteLine("QuotesFeed started");
quotesFeed.Stopped += (o, e) => Debug.WriteLine("QuotesFeed stopped");
quotesFeed.QuoteReceived += QuoteReceived;
quotesFeed.Start();

private void QuoteReceived(object s, Quote q)
{
	Debug.WriteLine("Last {0} trade was at price {1} for {2} shares", q.Symbol, q.Last, q.LastSize);
}

// the Orders feed streams live fills for orders in your account
var ordersFeed = new OrdersFeed(account, venue, reconnect: true);
ordersFeed.Started += (o, e) => Debug.WriteLine("OrdersFeed started");
ordersFeed.Stopped += (o, e) => Debug.WriteLine("OrdersFeed stopped");
ordersFeed.OrderReceived += OrderReceived;
ordersFeed.Start();

private void OrderReceived(object s, Order o)
{
	Debug.WriteLine("Fill received for OrderId={0}, quantity outstanding is now {1}", o.Id, o.Quantity);
}
```

###GameMaster API###

```csharp
using pb8.Stockfighter;
using pb8.Stockfighter.Model;

string apiKey = "123456";

// details: https://discuss.starfighters.io/t/the-gm-api-how-to-start-stop-restart-resume-trading-levels-automagically/143
var gm = new GameMaster(apiKey);

LevelDetails level = gm.Start("first_steps").Result;

string account = level.Account;
string venue = level.Venues.First();
string symbol = level.Symbols.First();

gm.Restart(level.InstanceId);

gm.Stop(level.InstanceId);
```

Contributions are more than welcome!
