# Indicators for NT 8

## DivergenciesEngines.cs
This engine plots divergences between price and the indicator selected as Input\
![image](https://user-images.githubusercontent.com/69223009/134590240-b7a54c69-cb1a-44ec-a4b5-e53d716f7755.png)

Remember to set the same parameters on the indicator you choose as Input for the Divergence and the one you attach to the chart

![image](https://user-images.githubusercontent.com/69223009/134590893-2ed9b370-3261-4406-b000-f6e763c346af.png)

This is an initial version that needs improvements but right now it helps me to find divergencies without the need to focus on it (simplifies my trading)

## SendTelegramMsg.cs
This is just the simplest way to get alerts from NinjaTrader in your telegram bot.\
Replace: 
* the Telegram ID by your ID  (@RawDataBot start then type anything and it will show your ID)
* the Telegram Token by the token of your bot where you will get the messages (@BotFather -> start -> /newbot -> type the bot name and it will show you the token)

## TrendLines2.cs
Very useful plot of price Trend Lines, the original version was taked from http://theindicatorstore.com, i made the adjustment to alert the price cross of a trend line over previous/historic trend lines defined by the input parameter "Number of trend lines", not just the active/last trend line
