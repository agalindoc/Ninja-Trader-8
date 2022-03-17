using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Serialization;
using NinjaTrader.Cbi;
using NinjaTrader.Gui;
using NinjaTrader.Gui.Chart;
using NinjaTrader.Gui.SuperDom;
using NinjaTrader.Gui.Tools;
using NinjaTrader.Data;
using NinjaTrader.NinjaScript;
using NinjaTrader.Core.FloatingPoint;
using NinjaTrader.NinjaScript.DrawingTools;
using System.IO;
using System.Net;

namespace NinjaTrader.NinjaScript.Strategies
{
    public class SendTelegramMsg : Strategy
    {

        #region Telegram
        [Display(Name = "Telegram ID", Order = 1, GroupName = "Telegram")]
        public string TelegramID
        { get; set; }

        [Display(Name = "Telegram Token", Order = 2, GroupName = "Telegram")]
        public string TelegramToken
        { get; set; }
        #endregion

        protected override void OnStateChange()
        {
            if (State == State.SetDefaults)
            {
                Description = @"SendTelegramMsg";
                Name = "SendTelegramMsg ";
                Calculate = Calculate.OnPriceChange;

                TelegramID = "1111";
                TelegramToken = "aaaaaaa";
            }
        }

        protected override void OnBarUpdate()
        {
            if (CurrentBar < 50)
                return;

            if (CrossAbove(EMA(10), EMA(100), 1))
                SendTelegramMessage("EMA(10) crossed above EMA(100)");
            if (CrossBelow(EMA(10), EMA(100), 1))
                SendTelegramMessage("EMA(10) crossed below EMA(100)");
        }

        private void SendTelegramMessage(string txtMsg)
        {
            if (TelegramToken == "" || TelegramID == "")
                return;
            try
            {
                string TelegramURLString = "https://api.telegram.org/bot{0}/sendMessage?chat_id={1}&text={2}";
                TelegramURLString = String.Format(TelegramURLString, TelegramToken, TelegramID, txtMsg);

                WebRequest request = WebRequest.Create(TelegramURLString);
                Print(TelegramURLString);

                Stream rs = request.GetResponse().GetResponseStream();
                StreamReader reader = new StreamReader(rs);
                string strResponse = reader.ReadToEnd();
                reader.Close();
                Print(strResponse);
            }
            catch
            {
                Print("Error sending Telegram.");
            }
        }

    }
}
