//+------------------------------------------------------------------+
//|                                               Rifumo-signals.mq4 |
//|                        Copyright 2017, MetaQuotes Software Corp. |
//|                                             https://www.mql5.com |
//+------------------------------------------------------------------+
#property copyright "Copyright 2017, MetaQuotes Software Corp."
#property link      "https://www.mql5.com"
#property version   "1.00"
#property strict
#property indicator_chart_window
#property indicator_buffers 2

#property indicator_color1 Lime
#property indicator_color2 Red

extern double  ExtDepth          =  12;
extern double  ExtDeviation      =  5;
extern double  ExtBackstep       =  3;
extern int     PipBuffer         = 0;

//indicator Buffers
double CrossUp[];
double CrossDown[];
double BuyBuffer[];
double SellBuffer[];
//+------------------------------------------------------------------+
//| Custom indicator initialization function                         |
//+------------------------------------------------------------------+
int OnInit()
  {
   SetIndexStyle(0,DRAW_ARROW,EMPTY,3);
   SetIndexBuffer(0,CrossUp);
   SetIndexLabel(0,"BuyBuffer("+ExtDepth+","+ExtDeviation+","+ExtBackstep+")");
   SetIndexArrow(0,233);
   SetIndexEmptyValue(1,EMPTY_VALUE);

   SetIndexStyle(1,DRAW_ARROW,EMPTY,3);
   SetIndexBuffer(1,CrossDown);
   SetIndexLabel(1,"SellBuffer("+ExtDepth+","+ExtDeviation+","+ExtBackstep+")");
   SetIndexArrow(1,234);
   SetIndexEmptyValue(1,EMPTY_VALUE);

   IndicatorShortName("Rifumo Signals");

   return(INIT_SUCCEEDED);
  }
//+------------------------------------------------------------------+
//| Custom indicator iteration function                              |
//+------------------------------------------------------------------+
int mCurrentBar=0;
double avgRange=0,avgBodySize=0, range;
datetime mLastSignalTime;
//+------------------------------------------------------------------+
//|                                                                  |
//+------------------------------------------------------------------+
int start()
  {
   double limit=(Bars-6);
   double signalLevel=EMPTY_VALUE;

   for(int i=limit; i>=0; i--)
     {
      avgRange=0;avgBodySize=0;
      mCurrentBar=i;

      int hasSignal=ZigZagSignal(signalLevel,i);

      Print(i+". SignalLevel = {"+signalLevel+"}");

      if(signalLevel>0 && hasSignal==OP_BUY)
        {
         CrossUp[i]=Low[i]-range*0.5;
         BuyBuffer[0]=Low[i];
         CrossDown[i]=EMPTY_VALUE;
        }

      if(signalLevel>0 && hasSignal==OP_SELL)
        {
         CrossDown[i]=High[i]+range*0.5;
         SellBuffer[0]=High[i];
         CrossUp[i]=EMPTY_VALUE;
        }

      if(hasSignal)
        {
         mLastSignalTime=Time[0];
        }
     }
   return(0);
  }
//+------------------------------------------------------------------+
//|                                                                  |
//+------------------------------------------------------------------+
int ZigZagSignal(double &entryPrice,int shift,bool useABC=false)
  {

   int PointShift=shift;
   string ConfirmedPoint="Not Found";
   string PointShiftDirection="None";
   double ZZ,ZZ2,ZZ3;

   if(!useABC)
     {
      ZZ=iCustom(NULL,0,"ZigZag",ExtDepth,ExtDeviation,ExtBackstep,0,PointShift);
      if(iHigh(NULL,0,PointShift)==ZZ || iLow(NULL,0,PointShift)==ZZ)
        {
         if(iHigh(NULL,0,PointShift)==ZZ)
           {
               entryPrice=(ZZ+(PipBuffer*Point));
               return OP_SELL;
           }
         if(iLow(NULL,0,PointShift)==ZZ)
           {
               entryPrice=(ZZ-(PipBuffer*Point));
               return OP_BUY;
           }
        }
     }

//ZigZag ABC  
   if(useABC)
     {

      while(ConfirmedPoint!="Found")
        {
         ZZ=iCustom(NULL,0,"ZigZag",ExtDepth,ExtDeviation,ExtBackstep,0,PointShift);
         if(iHigh(NULL,0,PointShift)==ZZ || iLow(NULL,0,PointShift)==ZZ)
           {
            ConfirmedPoint="Found";
            if(iHigh(NULL,0,PointShift)==ZZ)
              {
               PointShiftDirection="High";
               break;
              }
            if(iLow(NULL,0,PointShift)==ZZ)
              {
               PointShiftDirection="Low";
               break;
              }
           }
         PointShift++;
        }

      int PointShift2=PointShift;
      string ConfirmedPoint2="Not Found";

      while(ConfirmedPoint2!="Found")
        {
         ZZ2=iCustom(NULL,0,"ZigZag",ExtDepth,ExtDeviation,ExtBackstep,0,PointShift2);
         if(iHigh(NULL,0,PointShift2)==ZZ2 && PointShiftDirection=="Low")
           {
            ConfirmedPoint2="Found";
            break;
           }
         if(iLow(NULL,0,PointShift2)==ZZ2 && PointShiftDirection=="High")
           {
            ConfirmedPoint2="Found";
            break;
           }
         PointShift2++;
        }

      int PointShift3=PointShift2;
      string ConfirmedPoint3="Not Found";

      while(ConfirmedPoint3!="Found")
        {
         ZZ3=iCustom(NULL,0,"ZigZag",ExtDepth,ExtDeviation,ExtBackstep,0,PointShift3);
         if(iHigh(NULL,0,PointShift3)==ZZ3 && PointShiftDirection=="High")
           {
            ConfirmedPoint3="Found";
            break;
           }
         if(iLow(NULL,0,PointShift3)==ZZ3 && PointShiftDirection=="Low")
           {
            ConfirmedPoint3="Found";
            break;
           }
         PointShift3++;
        }

      double ZZZ1 = iCustom(NULL, 0, "ZigZag", ExtDepth, ExtDeviation, ExtBackstep, 0, PointShift);
      double ZZZ2 = iCustom(NULL, 0, "ZigZag", ExtDepth, ExtDeviation, ExtBackstep, 0, PointShift2);
      double ZZZ3 = iCustom(NULL, 0, "ZigZag", ExtDepth, ExtDeviation, ExtBackstep, 0, PointShift3);

      double longEntry=EMPTY_VALUE;
      double shortEntry=EMPTY_VALUE;

      if(ZZZ3<ZZZ2 && ZZZ2>ZZZ1 && ZZZ1>ZZZ3)
        {
         entryPrice=(ZZZ2+(PipBuffer*Point));   //longEntry
         return OP_BUY;
        }

      if(ZZZ3>ZZZ2 && ZZZ2<ZZZ1 && ZZZ1<ZZZ3)
        {
         entryPrice=(ZZZ2 -(PipBuffer*Point));  //shortEntry
         return OP_SELL;
        }
     }

   return EMPTY_VALUE;
  }
//+------------------------------------------------------------------+
