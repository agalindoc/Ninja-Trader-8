// 
// Copyright (C) 2021 Alejandro Galindo Cháirez
// 

using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Windows.Media;
using System.Xml.Serialization;
using NinjaTrader.Gui;
using NinjaTrader.NinjaScript;
using NinjaTrader.NinjaScript.DrawingTools;

namespace NinjaTrader.NinjaScript.Indicators
{
    public class DivergenciesEngine : Indicator
    {
        #region variables        
        Indicator inputInd;
        private Brush _bearColor = Brushes.Red;
        private Brush _bullColor = Brushes.Green;
        private Brush _hiddenBullColor = Brushes.Cyan; // new SolidColorBrush(Brushes.Green.Color);        
        private Brush _hiddenBearColor = Brushes.Magenta;  // new SolidColorBrush(Brushes.Red.Color);        
        #endregion

        #region values
        [XmlIgnore]
        [Browsable(false)]
        public Series<double> oscHLshape
        {
            get { return Values[0]; }
        }

        [XmlIgnore]
        [Browsable(false)]
        public Series<double> oscLHshape
        {
            get { return Values[1]; }
        }

        [XmlIgnore]
        [Browsable(false)]
        public Series<double> oscLLshape
        {
            get { return Values[2]; }
        }

        [XmlIgnore]
        [Browsable(false)]
        public Series<double> oscHHshape
        {
            get { return Values[3]; }
        }
        #endregion

        #region parameters
        [NinjaScriptProperty, Display(Name = "Show divergences", GroupName = "Parameters", Order = 0)]
        public bool showDivergences
        { get; set; }

        [NinjaScriptProperty, Display(Name = "Pivot lookback Right", GroupName = "Divergence Parameters", Order = 0)]
        public int lbR
        { get; set; }

        [NinjaScriptProperty, Display(Name = "Pivot lookback Left", GroupName = "Divergence Parameters", Order = 1)]
        public int lbL
        { get; set; }

        [NinjaScriptProperty, Display(Name = "Max of Lookback Range", GroupName = "Divergence Parameters", Order = 2)]
        public int rangeUpper
        { get; set; }

        [NinjaScriptProperty, Display(Name = "Min of Lookback Range", GroupName = "Divergence Parameters", Order = 3)]
        public int rangeLower
        { get; set; }

        [NinjaScriptProperty, Display(Name = "Plot Bullish", GroupName = "Divergence Parameters", Order = 4)]
        public bool plotBull
        { get; set; }

        [NinjaScriptProperty, Display(Name = "Plot Hidden Bullish", GroupName = "Divergence Parameters", Order = 5)]
        public bool plotHiddenBull
        { get; set; }

        [NinjaScriptProperty, Display(Name = "Plot Bearish", GroupName = "Divergence Parameters", Order = 6)]
        public bool plotBear
        { get; set; }

        [NinjaScriptProperty, Display(Name = "Plot Hidden Bearish", GroupName = "Divergence Parameters", Order = 7)]
        public bool plotHiddenBear
        { get; set; }

        [NinjaScriptProperty, Display(Name = "Plot Bullish arrows", GroupName = "Divergence Parameters", Order = 8)]
        public bool plotBullArrows
        { get; set; }

        [NinjaScriptProperty, Display(Name = "Plot Bearish arrows", GroupName = "Divergence Parameters", Order = 9)]
        public bool plotBearArrows
        { get; set; }

        [NinjaScriptProperty, Display(Name = "Arrows offset in ticks, GroupName = ", GroupName = "Divergence Parameters", Order = 10)]
        public int arrowsOffset
        { get; set; }

        [XmlIgnore()]
        [NinjaScriptProperty, Display(Name = "Bearish divergence lines color", GroupName = "Divergence Parameters", Order = 11)]
        public Brush bearishDivergenceLinesColor
        { get; set; }

        // Serialize our Color object
        [Browsable(false)]
        public string bearishDivergenceLinesColorSerialize
        {
            get { return Serialize.BrushToString(bearishDivergenceLinesColor); }
            set { bearishDivergenceLinesColor = Serialize.StringToBrush(value); }
        }

        [XmlIgnore()]
        [NinjaScriptProperty, Display(Name = "Bullish divergence lines color", GroupName = "Divergence Parameters", Order = 12)]
        public Brush bullishDivergenceLinesColor
        { get; set; }

        // Serialize our Color object
        [Browsable(false)]
        public string bullishDivergenceLinesColorSerialize
        {
            get { return Serialize.BrushToString(bullishDivergenceLinesColor); }
            set { bullishDivergenceLinesColor = Serialize.StringToBrush(value); }
        }

        [XmlIgnore()]
        [NinjaScriptProperty, Display(Name = "Hidden bullish divergence lines color", GroupName = "Divergence Parameters", Order = 13)]
        public Brush hiddenBullishDivergenceLinesColor
        { get; set; }

        // Serialize our Color object
        [Browsable(false)]
        public string hiddenBullishDivergenceLinesColorSerialize
        {
            get { return Serialize.BrushToString(hiddenBullishDivergenceLinesColor); }
            set { hiddenBullishDivergenceLinesColor = Serialize.StringToBrush(value); }
        }

        [XmlIgnore()]
        [NinjaScriptProperty, Display(Name = "Hidden bearish divergence lines color", GroupName = "Divergence Parameters", Order = 14)]
        public Brush hiddenBearishDivergenceLinesColor
        { get; set; }

        // Serialize our Color object
        [Browsable(false)]
        public string hiddenBearishDivergenceLinesColorSerialize
        {
            get { return Serialize.BrushToString(hiddenBearishDivergenceLinesColor); }
            set { hiddenBearishDivergenceLinesColor = Serialize.StringToBrush(value); }
        }

        [NinjaScriptProperty, Display(Name = "Max consecutive divergences", GroupName = "Divergence Parameters", Order = 15)]
        public int maxConsecutive
        { get; set; }

        [NinjaScriptProperty, Display(Name = "Plot divergences on chart", GroupName = "Divergence Parameters", Order = 16)]
        public bool plotDivLinesChart
        { get; set; }

        #endregion

        private void SetDefaultParameters()
        {
            // Divergence parameters
            lbR = 5;
            lbL = 25;
            rangeUpper = 60;
            rangeLower = 5;
            plotBull = true;
            plotHiddenBull = true;
            plotBear = true;
            plotHiddenBear = true;
            plotBullArrows = true;
            plotBearArrows = true;
            showDivergences = true;
            arrowsOffset = 15;
            bullishDivergenceLinesColor = Brushes.Green;
            bearishDivergenceLinesColor = Brushes.Red;
            hiddenBullishDivergenceLinesColor = Brushes.DarkCyan;
            hiddenBearishDivergenceLinesColor = Brushes.Magenta;
            maxConsecutive = 2;
            plotDivLinesChart = true;
        }

        protected override void OnStateChange()
        {
            if (State == State.SetDefaults)
            {
                Description = @"Divergencies engine. Plots divregencies on the indicator asociated as input and on the price";
                Name = "Divergence engine";

                SetDefaultParameters();
                AddPlot(new Stroke(_bullColor, 3f), PlotStyle.Dot, "Bullish Circle");
                AddPlot(new Stroke(_bearColor, 3f), PlotStyle.Dot, "Bearish Circle");
                AddPlot(new Stroke(_hiddenBullColor, 3f), PlotStyle.Dot, "Hidden Bullish Circle");
                AddPlot(new Stroke(_hiddenBearColor, 3f), PlotStyle.Dot, "Hidden Bearish Circle");
                
                Calculate = Calculate.OnPriceChange;
                IsOverlay = false;
                PaintPriceMarkers = false;
            }
            else if (State == State.Configure)
            {

            }
            else if (State == State.DataLoaded)
            {
                inputInd = Input as Indicator;                
            }
        }

        protected override void OnBarUpdate()
        {
            
            if (CurrentBar < rangeUpper + rangeLower)
                return;          

            // Divergence plot
            if (showDivergences)
            {
                CatchBullishDivergence(2);                
                CatchBearishDivergence(2);
            }
        }

        #region Divergences


        private void CatchBullishDivergence(int shift)
        {
            if (!IsIndicatorTrough(shift))
                return;
            int currentTrough = shift;
            int lastTrough = GetIndicatorLastTrough(shift);
            // bullish
            if (plotBull && lastTrough>0)
                if (Input[currentTrough] > Input[lastTrough] && Low[currentTrough] < Low[lastTrough])
                {
                    oscHLshape[currentTrough] = Input[currentTrough];
                    // plot divergence lines 
                    Draw.Line(this, "PBDLine" + Instrument.FullName + CurrentBar, false, lastTrough, Low[lastTrough], currentTrough, Low[currentTrough], bullishDivergenceLinesColor, DashStyleHelper.Dot, 1);
                    DrawOnPricePanel = false;
                    Draw.Line(this, "BDLine" + Instrument.FullName + CurrentBar, false, lastTrough, Input[lastTrough], currentTrough, Input[currentTrough], bullishDivergenceLinesColor, DashStyleHelper.Dot, 1);
                    DrawOnPricePanel = true;
                    // plot the arrow 
                    if (plotBullArrows)
                    {
                        Draw.ArrowUp(this, "BullA" + Math.Round(Input[currentTrough], 4) + Instrument.FullName + CurrentBar, true, 2, Low[currentTrough] - TickSize * arrowsOffset, Plots[0].Brush);
                    }


                }
            // hidden bullish divergence
            if (plotHiddenBull && lastTrough > 0)
                if (Input[currentTrough] < Input[lastTrough] && Low[currentTrough] > Input[lastTrough])
                {
                    oscLLshape[currentTrough] = Input[currentTrough];
                    // plot divergence lines 
                    Draw.Line(this, "PHBDLine" + Instrument.FullName + CurrentBar, false, lastTrough, Low[lastTrough], currentTrough, Low[currentTrough], hiddenBullishDivergenceLinesColor, DashStyleHelper.Dot, 1);
                    DrawOnPricePanel = false;
                    Draw.Line(this, "HBDLine" + Instrument.FullName + CurrentBar, false, lastTrough, Input[lastTrough], currentTrough, Input[currentTrough], hiddenBullishDivergenceLinesColor, DashStyleHelper.Dot, 1);
                    DrawOnPricePanel = true;
                    // plot the arrow
                    if (plotBullArrows)
                    {
                        Draw.ArrowUp(this, "HBullA" + Math.Round(Input[currentTrough], 4) + Instrument.FullName + CurrentBar, true, 2, Low[currentTrough] - TickSize * arrowsOffset, Plots[2].Brush);
                    }

                }
        }

        private void CatchBearishDivergence(int shift)
        {
            if (!IsIndicatorPeak(shift))
                return;
            int currentPeak = shift;
            int lastPeak = GetIndicatorLastPeak(shift);
            // Bearish divergence
            if (plotBear && lastPeak>0)
                if (Input[currentPeak] < Input[lastPeak] && High[currentPeak] > High[lastPeak])
                {
                    oscLHshape[currentPeak] = Input[currentPeak];
                    // plot divergence lines 
                    Draw.Line(this, "PBearDLine" + Instrument.FullName + CurrentBar, false, lastPeak, High[lastPeak], currentPeak, High[currentPeak], bearishDivergenceLinesColor, DashStyleHelper.Dot, 1);
                    DrawOnPricePanel = false;
                    Draw.Line(this, "BearDLine" + Instrument.FullName + CurrentBar, false, lastPeak, Input[lastPeak], currentPeak, Input[currentPeak], bearishDivergenceLinesColor, DashStyleHelper.Dot, 1);
                    DrawOnPricePanel = true;
                    // plot the arrow 
                    if (plotBearArrows)
                    {
                        Draw.ArrowDown(this, "BearA" + Math.Round(Input[currentPeak], 4) + Instrument.FullName + CurrentBar, true, 2, High[currentPeak] + TickSize * arrowsOffset, Plots[1].Brush);
                    }

                }
            // Hidden bearish divergence
            if (plotHiddenBear && lastPeak>0)
                if (Input[currentPeak] > Input[lastPeak] && High[currentPeak] < High[lastPeak])
                {
                    oscHHshape[currentPeak] = Input[currentPeak];
                    // plot divergence lines 
                    Draw.Line(this, "PBearHDLine" + Instrument.FullName + CurrentBar, false, lastPeak, High[lastPeak], currentPeak, High[currentPeak], hiddenBearishDivergenceLinesColor, DashStyleHelper.Dot, 1);
                    DrawOnPricePanel = false;
                    Draw.Line(this, "BearHDLine" + Instrument.FullName + CurrentBar, false, lastPeak, Input[lastPeak], currentPeak, Input[currentPeak], hiddenBearishDivergenceLinesColor, DashStyleHelper.Dot, 1);
                    DrawOnPricePanel = true;
                    // plot the arrow 
                    if (plotBearArrows)
                    {
                        Draw.ArrowDown(this, "HBearA" + Math.Round(Input[currentPeak], 4) + Instrument.FullName + CurrentBar, true, 2, High[currentPeak] + TickSize * arrowsOffset, Plots[3].Brush);
                    }


                }
        }

        private bool IsIndicatorPeak(int shift)
        {
            if (Input[shift] >= Input[shift + 1] && Input[shift] > Input[shift + 2] && Input[shift] > Input[shift - 1])
                return true;
            else
                return false;
        }

        private bool IsIndicatorTrough(int shift)
        {
            if (Input[shift] <= Input[shift + 1] && Input[shift] < Input[shift + 2] && Input[shift] < Input[shift - 1])
                return true;
            else
                return false;
        }

        private int GetIndicatorLastPeak(int shift)
        {
            int value = -1;
            int tops = 0;
            //for (int i=shift+lbR; i < Count - rangeUpper - rangeLower; i++)
            //for (int i = shift + lbR; i < rangeUpper+rangeLower; i++)
            for (int i = shift + lbR; i < rangeUpper; i++)
            {
                if (Input[i] >= Input[i + 1] && Input[i] >= Input[i + 2] && Input[i] >= Input[i - 1] && Input[i] >= Input[i - 2])
                {
                    //if (value < 0)
                    //    value = i;
                    //else if (_osc[i] > _osc[value])
                    value = i;
                    tops++;
                }
                if (tops >= maxConsecutive)
                    break;
            }
            return value;
        }

        private int GetIndicatorLastTrough(int shift)
        {
            int value = -1;
            int bottoms = 0;
            //for (int i = shift + lbR; i < Count - rangeUpper-rangeLower; i++)
            //for (int i = shift + lbR; i < rangeUpper + rangeLower; i++)
            for (int i = shift + lbR; i < rangeUpper; i++)
            {
                if (Input[i] <= Input[i + 1] && Input[i] <= Input[i + 2] && Input[i] <= Input[i - 1] && Input[i] <= Input[i - 2])
                {
                    //if (value <0)
                    //    value = i;
                    //else if (_osc[i] < _osc[value])
                    value = i;
                    bottoms++;
                }
                if (bottoms >= maxConsecutive)
                    break;
            }
            return value;
        }

        #endregion
    }
}

#region NinjaScript generated code. Neither change nor remove.

namespace NinjaTrader.NinjaScript.Indicators
{
	public partial class Indicator : NinjaTrader.Gui.NinjaScript.IndicatorRenderBase
	{
		private DivergenciesEngine[] cacheDivergenciesEngine;
		public DivergenciesEngine DivergenciesEngine(bool showDivergences, int lbR, int lbL, int rangeUpper, int rangeLower, bool plotBull, bool plotHiddenBull, bool plotBear, bool plotHiddenBear, bool plotBullArrows, bool plotBearArrows, int arrowsOffset, Brush bearishDivergenceLinesColor, Brush bullishDivergenceLinesColor, Brush hiddenBullishDivergenceLinesColor, Brush hiddenBearishDivergenceLinesColor, int maxConsecutive, bool plotDivLinesChart)
		{
			return DivergenciesEngine(Input, showDivergences, lbR, lbL, rangeUpper, rangeLower, plotBull, plotHiddenBull, plotBear, plotHiddenBear, plotBullArrows, plotBearArrows, arrowsOffset, bearishDivergenceLinesColor, bullishDivergenceLinesColor, hiddenBullishDivergenceLinesColor, hiddenBearishDivergenceLinesColor, maxConsecutive, plotDivLinesChart);
		}

		public DivergenciesEngine DivergenciesEngine(ISeries<double> input, bool showDivergences, int lbR, int lbL, int rangeUpper, int rangeLower, bool plotBull, bool plotHiddenBull, bool plotBear, bool plotHiddenBear, bool plotBullArrows, bool plotBearArrows, int arrowsOffset, Brush bearishDivergenceLinesColor, Brush bullishDivergenceLinesColor, Brush hiddenBullishDivergenceLinesColor, Brush hiddenBearishDivergenceLinesColor, int maxConsecutive, bool plotDivLinesChart)
		{
			if (cacheDivergenciesEngine != null)
				for (int idx = 0; idx < cacheDivergenciesEngine.Length; idx++)
					if (cacheDivergenciesEngine[idx] != null && cacheDivergenciesEngine[idx].showDivergences == showDivergences && cacheDivergenciesEngine[idx].lbR == lbR && cacheDivergenciesEngine[idx].lbL == lbL && cacheDivergenciesEngine[idx].rangeUpper == rangeUpper && cacheDivergenciesEngine[idx].rangeLower == rangeLower && cacheDivergenciesEngine[idx].plotBull == plotBull && cacheDivergenciesEngine[idx].plotHiddenBull == plotHiddenBull && cacheDivergenciesEngine[idx].plotBear == plotBear && cacheDivergenciesEngine[idx].plotHiddenBear == plotHiddenBear && cacheDivergenciesEngine[idx].plotBullArrows == plotBullArrows && cacheDivergenciesEngine[idx].plotBearArrows == plotBearArrows && cacheDivergenciesEngine[idx].arrowsOffset == arrowsOffset && cacheDivergenciesEngine[idx].bearishDivergenceLinesColor == bearishDivergenceLinesColor && cacheDivergenciesEngine[idx].bullishDivergenceLinesColor == bullishDivergenceLinesColor && cacheDivergenciesEngine[idx].hiddenBullishDivergenceLinesColor == hiddenBullishDivergenceLinesColor && cacheDivergenciesEngine[idx].hiddenBearishDivergenceLinesColor == hiddenBearishDivergenceLinesColor && cacheDivergenciesEngine[idx].maxConsecutive == maxConsecutive && cacheDivergenciesEngine[idx].plotDivLinesChart == plotDivLinesChart && cacheDivergenciesEngine[idx].EqualsInput(input))
						return cacheDivergenciesEngine[idx];
			return CacheIndicator<DivergenciesEngine>(new DivergenciesEngine(){ showDivergences = showDivergences, lbR = lbR, lbL = lbL, rangeUpper = rangeUpper, rangeLower = rangeLower, plotBull = plotBull, plotHiddenBull = plotHiddenBull, plotBear = plotBear, plotHiddenBear = plotHiddenBear, plotBullArrows = plotBullArrows, plotBearArrows = plotBearArrows, arrowsOffset = arrowsOffset, bearishDivergenceLinesColor = bearishDivergenceLinesColor, bullishDivergenceLinesColor = bullishDivergenceLinesColor, hiddenBullishDivergenceLinesColor = hiddenBullishDivergenceLinesColor, hiddenBearishDivergenceLinesColor = hiddenBearishDivergenceLinesColor, maxConsecutive = maxConsecutive, plotDivLinesChart = plotDivLinesChart }, input, ref cacheDivergenciesEngine);
		}
	}
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
	public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
	{
		public Indicators.DivergenciesEngine DivergenciesEngine(bool showDivergences, int lbR, int lbL, int rangeUpper, int rangeLower, bool plotBull, bool plotHiddenBull, bool plotBear, bool plotHiddenBear, bool plotBullArrows, bool plotBearArrows, int arrowsOffset, Brush bearishDivergenceLinesColor, Brush bullishDivergenceLinesColor, Brush hiddenBullishDivergenceLinesColor, Brush hiddenBearishDivergenceLinesColor, int maxConsecutive, bool plotDivLinesChart)
		{
			return indicator.DivergenciesEngine(Input, showDivergences, lbR, lbL, rangeUpper, rangeLower, plotBull, plotHiddenBull, plotBear, plotHiddenBear, plotBullArrows, plotBearArrows, arrowsOffset, bearishDivergenceLinesColor, bullishDivergenceLinesColor, hiddenBullishDivergenceLinesColor, hiddenBearishDivergenceLinesColor, maxConsecutive, plotDivLinesChart);
		}

		public Indicators.DivergenciesEngine DivergenciesEngine(ISeries<double> input , bool showDivergences, int lbR, int lbL, int rangeUpper, int rangeLower, bool plotBull, bool plotHiddenBull, bool plotBear, bool plotHiddenBear, bool plotBullArrows, bool plotBearArrows, int arrowsOffset, Brush bearishDivergenceLinesColor, Brush bullishDivergenceLinesColor, Brush hiddenBullishDivergenceLinesColor, Brush hiddenBearishDivergenceLinesColor, int maxConsecutive, bool plotDivLinesChart)
		{
			return indicator.DivergenciesEngine(input, showDivergences, lbR, lbL, rangeUpper, rangeLower, plotBull, plotHiddenBull, plotBear, plotHiddenBear, plotBullArrows, plotBearArrows, arrowsOffset, bearishDivergenceLinesColor, bullishDivergenceLinesColor, hiddenBullishDivergenceLinesColor, hiddenBearishDivergenceLinesColor, maxConsecutive, plotDivLinesChart);
		}
	}
}

namespace NinjaTrader.NinjaScript.Strategies
{
	public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
	{
		public Indicators.DivergenciesEngine DivergenciesEngine(bool showDivergences, int lbR, int lbL, int rangeUpper, int rangeLower, bool plotBull, bool plotHiddenBull, bool plotBear, bool plotHiddenBear, bool plotBullArrows, bool plotBearArrows, int arrowsOffset, Brush bearishDivergenceLinesColor, Brush bullishDivergenceLinesColor, Brush hiddenBullishDivergenceLinesColor, Brush hiddenBearishDivergenceLinesColor, int maxConsecutive, bool plotDivLinesChart)
		{
			return indicator.DivergenciesEngine(Input, showDivergences, lbR, lbL, rangeUpper, rangeLower, plotBull, plotHiddenBull, plotBear, plotHiddenBear, plotBullArrows, plotBearArrows, arrowsOffset, bearishDivergenceLinesColor, bullishDivergenceLinesColor, hiddenBullishDivergenceLinesColor, hiddenBearishDivergenceLinesColor, maxConsecutive, plotDivLinesChart);
		}

		public Indicators.DivergenciesEngine DivergenciesEngine(ISeries<double> input , bool showDivergences, int lbR, int lbL, int rangeUpper, int rangeLower, bool plotBull, bool plotHiddenBull, bool plotBear, bool plotHiddenBear, bool plotBullArrows, bool plotBearArrows, int arrowsOffset, Brush bearishDivergenceLinesColor, Brush bullishDivergenceLinesColor, Brush hiddenBullishDivergenceLinesColor, Brush hiddenBearishDivergenceLinesColor, int maxConsecutive, bool plotDivLinesChart)
		{
			return indicator.DivergenciesEngine(input, showDivergences, lbR, lbL, rangeUpper, rangeLower, plotBull, plotHiddenBull, plotBear, plotHiddenBear, plotBullArrows, plotBearArrows, arrowsOffset, bearishDivergenceLinesColor, bullishDivergenceLinesColor, hiddenBullishDivergenceLinesColor, hiddenBearishDivergenceLinesColor, maxConsecutive, plotDivLinesChart);
		}
	}
}

#endregion
