
Upam, da sem pravilno dojel nalogo in da implementiran MetaExchangeEngine dejansko poène kar bi naj poèel.

Output iz konzolne za prikaz logike (samo prvih 5 orderbook-ov, kupujemo 2 btc ):

*******************************************
    Exchange states:
            '1548759600.25189' (EUR=9508,03673990, BTC=1,83963713)
            '1548759601.33694' (EUR=2584,72735835, BTC=4,97468842)
            '1548759602.41701' (EUR=2832,13003391, BTC=5,30287293)
            '1548759603.47307' (EUR=2987,48389677, BTC=1,18212997)
            '1548759604.60313' (EUR=8491,09793198, BTC=3,12848501)

    --> Buy 2 BTC

    RESPONSE: Status: ok / TotalAmount=2,000, TotalPrice=5928,58000, Price=2964,29
            Order to 1548759600.25189: Buy 0,405 BTC at 2964,29 EUR
            Order to 1548759601.33694: Buy 0,405 BTC at 2964,29 EUR
            Order to 1548759602.41701: Buy 0,405 BTC at 2964,29 EUR
            Order to 1548759603.47307: Buy 0,405 BTC at 2964,29 EUR
            Order to 1548759604.60313: Buy 0,380 BTC at 2964,29 EUR

    Exchange states:
            '1548759600.25189' (EUR=8307,49928990, BTC=2,24463713)
            '1548759601.33694' (EUR=1384,18990835, BTC=5,37968842)
            '1548759602.41701' (EUR=1631,59258391, BTC=5,70787293)
            '1548759603.47307' (EUR=1786,94644677, BTC=1,58712997)
            '1548759604.60313' (EUR=7364,66773198, BTC=3,50848501)

*******************************************

Smell.Api:
Projekt uporablja appsettings.json, kjer najdemo pot do data datoteke in skip/take parametre za branje samo podmnožico vrstic:
    "CryptoExchange": {
        "FilePath": "Data\\order_books_data",
        "SkipLines": 0,
        "TakeLines": 5
    }

Smell.ConsoleApp:
Sprejme tudi dva command line argumenta, ki predstavljata skip/take za definiranje podmnožice. 

Smell.LibTests
Nekateri testi berejo data file, nekateri hardcode-an OrderBook.
