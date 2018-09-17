//===========================================================================
//        OrderReliable.mqh
//
//   A library for MT4 expert advisors,  intended to give more
//   reliable order handling.   This is under development.
//
//  Use:
//         Put this file in the experts directory.  Include the line
// #include "OrderReliable.mqh"
// in your EA. 
// 
//  Version:   0.1.1
//  Contents:
//
//      OrderSendReliable()  
//           This is intended to be a drop-in replacement for OrderSend() which, one hopes
//           is more resistant to various forms of errors prevalent with MetaTrader.
//
//     
//===========================================================================
//  History:
//   2006-06-05:  Added ERR_TRADE_CONTEXT_BUSY to retryable errors.    0.1.1
//   2006-05-29:  Created.  Only OrderSendReliable() implemented       0.1
//       
// LICENSING:  This is free, open source software, licensed under
// Version 2 of the GNU General Public License (GPL). 
// 
// In particular, this means that distribution of this software in a binary format,
// e.g. as compiled in as part of a .ex4, must be
// accompanied by the non-obfuscated source code of both this file, OrderReliable.mqh, AND the
// .mq4 source files which it is compiled with, or you must make such files available at 
// no charge to binary recipients.   If you do not agree with such terms
// you must not use this code.  Detailed terms of the GPL are widely 
// available on the Internet.
//  
// Copyright (2006), Matthew Kennel     
//===========================================================================


//===========================================================================
//                         OrderSendReliable()
//
/*         This is intended to be a drop-in replacement for OrderSend() which, one hopes
           is more resistant to various forms of errors prevalent with MetaTrader.
           
syntax:
         
    int OrderSendReliable(string symbol, int cmd, double volume, double price,
		   int slippage, double stoploss, double takeprofit,
		   string comment, int magic, datetime expiration =
		   0, color arrow_color = CLR_NONE) //    

returns: ticket number or -1 under some error conditions. 

    Features:
       * re-trying under some error conditions, sleeping
         a random time defined by an exponential probability distribution.
       * automatic normalization of Digits
       * automatically makes sure that stop levels are more than
         the minimum stop distance, as given by the server.
       * automatically converts limit orders to market orders 
         when the limit orders are rejected by the server for being
         to close to market.
       * displays various error messages on the log for debugging.
 
  Matt Kennel, mbkennel@gmail.com  2006-05-28
 */
  
//===========================================================================
#include <stdlib.mqh>
#include <stderror.mqh> 

int retry_attempts = 6; 
double sleep_time = 2.0, sleep_maximum = 25.0;  // in seconds

int OrderSendReliable(string symbol, int cmd, double volume, double price,
		   int slippage, double stoploss, double takeprofit,
		   string comment, int magic, datetime expiration =
		   0, color arrow_color = CLR_NONE) {

//
// check basic conditions see if trade is possible. 
//
   if (!IsConnected()) {
      Print("OrderSendReliable:  error: IsConnected() == false");
      return(-1);
   }
   if (IsStopped()) {
      Print("OrderSendReliable:  error: IsStopped() == true");
      return(-1);
   }
   int cnt = 0;
   while(!IsTradeAllowed() && cnt < retry_attempts) {
      OrderReliable_SleepRandomTime(sleep_time,sleep_maximum); 
      cnt++;
   }
   if (!IsTradeAllowed()) {
      Print("OrderSendReliable: error: no operation possible because IsTradeAllowed()==false, even after retries.");
      return(-1);  
   }

   // let's normalize all price/stoploss/takeprofit to the proper # of digits.
   int digits = MarketInfo(symbol,MODE_DIGITS);
   if (digits > 0) {
      price = NormalizeDouble(price,digits);
      stoploss = NormalizeDouble(price,digits);
      takeprofit = NormalizeDouble(price,digits); 
   }
   if (stoploss != 0) OrderReliable_EnsureValidStop(symbol,price,stoploss); 

   int err = GetLastError(); // so we clear the global variable.  
   err = 0; 
   bool exit_loop = false;
   bool limit_to_market = false; 
   // limit/stop order. 
   int ticket=-1;

   if ((cmd == OP_BUYSTOP) || (cmd == OP_SELLSTOP)) {
       cnt = 0;
       while (!exit_loop) {
          if (IsTradeAllowed()) {
             ticket = OrderSend(symbol, cmd, volume, price, slippage, stoploss, takeprofit, comment, magic, expiration, arrow_color);
             err = GetLastError();
          } else {
            cnt++;
          } 
          switch (err) {
            case ERR_NO_ERROR:
               exit_loop = true;
               break;
            case ERR_SERVER_BUSY:
            case ERR_NO_CONNECTION:
            case ERR_INVALID_PRICE:
            case ERR_OFF_QUOTES:
            case ERR_BROKER_BUSY:
            case ERR_TRADE_CONTEXT_BUSY: 
               cnt++; // a retryable error
               break;
            case ERR_PRICE_CHANGED:
            case ERR_REQUOTE:
               RefreshRates();
               continue; // we can apparently retry immediately according to MT docs.
            case ERR_INVALID_STOPS:
               double servers_min_stop = MarketInfo(symbol,MODE_STOPLEVEL)*MarketInfo(symbol,MODE_POINT); 
               if (cmd == OP_BUYSTOP) {
                 if (MathAbs(Ask - price) <= servers_min_stop)
                  // we are too close to put in a limit/stop order so go to market.
                    limit_to_market = true; 
               } else if (cmd == OP_SELLSTOP) {
                 if (MathAbs(Bid - price) <= servers_min_stop)
                    limit_to_market = true; 
               }
               exit_loop = true; 
               break; 
            default:
               // an apparently serious error.
               exit_loop = true;
               break; 
          }  // end switch 

          if (cnt > retry_attempts) exit_loop = true; 
          if (exit_loop) {
            if (err != ERR_NO_ERROR) {
               Print("OrderSendReliable: non-retryable error: " +ErrorDescription(err)); 
            }
            if (cnt > retry_attempts) {
               Print("OrderSendReliable: retry attempts maxed at "+retry_attempts); 
            }
          }
          if (!exit_loop) {
              Print("OrderSendReliable: retryable error ("+cnt+"/"+retry_attempts+"): " + ErrorDescription(err)); 
              OrderReliable_SleepRandomTime(sleep_time,sleep_maximum); 
              RefreshRates(); 
          }
       }
      // we have now exited from loop. 
      if (err == ERR_NO_ERROR) {
         Print("OrderSendReliable: apparently successful OP_BUYSTOP or OP_SELLSTOP order placed, details follow.");
         OrderSelect(ticket, SELECT_BY_TICKET, MODE_TRADES); 
         OrderPrint(); 
         return(ticket); // SUCCESS! 
      } 
      if (!limit_to_market) {
         Print("OrderSendReliable: failed to execute OP_BUYSTOP/OP_SELLSTOP, after "+cnt+" retries");
         Print("OrderSendReliable: failed trade: " + OrderReliable_CommandString(cmd)+" "+symbol+"@"+price+" tp@"+takeprofit+" sl@"+stoploss); 
         Print("OrderSendReliable: last error: "+ErrorDescription(err)); 
         return(-1); 
      }
   }  // end     
  
   if (limit_to_market) {
      Print("OrderSendReliable: going from limit order to market order because market is too close.");
      if (cmd == OP_BUYSTOP) {
         cmd = OP_BUY;
         price = Ask;
      } else if (cmd == OP_SELLSTOP) {
         cmd = OP_SELL;
         price = Bid;
      }   
   }
   
   // we now have a market order.
   err = GetLastError(); // so we clear the global variable.  
   err = 0; 
   ticket=-1;

   if ((cmd == OP_BUY) || (cmd == OP_SELL)) {
       cnt = 0;
       while (!exit_loop) {
          if (IsTradeAllowed()) {
             ticket = OrderSend(symbol, cmd, volume, price, slippage, stoploss, takeprofit, comment, magic, expiration, arrow_color);
             err = GetLastError();
          } else {
            cnt++;
          } 
          switch (err) {
            case ERR_NO_ERROR:
               exit_loop = true;
               break;
            case ERR_SERVER_BUSY:
            case ERR_NO_CONNECTION:
            case ERR_INVALID_PRICE:
            case ERR_OFF_QUOTES:
            case ERR_BROKER_BUSY:
            case ERR_TRADE_CONTEXT_BUSY: 
               cnt++; // a retryable error
               break;
            case ERR_PRICE_CHANGED:
            case ERR_REQUOTE:
               RefreshRates();
               continue; // we can apparently retry immediately according to MT docs.
            default:
               // an apparently serious, unretryable error.
               exit_loop = true;
               break; 
          }  // end switch 

          if (cnt > retry_attempts) exit_loop = true; 
          if (!exit_loop) {
              Print("OrderSendReliable: retryable error ("+cnt+"/"+retry_attempts+"): " + ErrorDescription(err)); 
              OrderReliable_SleepRandomTime(sleep_time,sleep_maximum); 
              RefreshRates(); 
          }
          if (exit_loop) {
            if (err != ERR_NO_ERROR) {
               Print("OrderSendReliable: non-retryable error: " +ErrorDescription(err)); 
            }
            if (cnt > retry_attempts) {
               Print("OrderSendReliable: retry attempts maxed at "+retry_attempts); 
            }
          }
       }
      // we have now exited from loop. 
      if (err == ERR_NO_ERROR) {
         Print("OrderSendReliable: apparently successful OP_BUY or OP_SELL order placed, details follow.");
         OrderSelect(ticket, SELECT_BY_TICKET, MODE_TRADES); 
         OrderPrint(); 
         return(ticket); // SUCCESS! 
      } 
      Print("OrderSendReliable: failed to execute OP_BUY/OP_SELL, after "+cnt+" retries");
      Print("OrderSendReliable: failed trade: " + OrderReliable_CommandString(cmd)+" "+symbol+"@"+price+" tp@"+takeprofit+" sl@"+stoploss); 
      Print("OrderSendReliable: last error: "+ErrorDescription(err)); 
      return(-1); 
   }
}  // end     
   
   
 
//===========================================================================


//===========================================================================
//                          Utility Functions
//===========================================================================
void OrderReliable_EnsureValidStop(string symbol, double price, double& sl) {
// adjust stop loss so that it is legal.
   if (sl == 0) return;
   
   double servers_min_stop = MarketInfo(symbol,MODE_STOPLEVEL)*MarketInfo(symbol,MODE_POINT); 
   
   if (MathAbs(price-sl) <= servers_min_stop) {
      // we have to adjust the stop.
      if (price > sl) { // we are buying
         sl = price - servers_min_stop;
      } else if (price < sl) {
         sl = price + servers_min_stop;
      } else {
         Print("OrderReliable_EnsureValidStop: error, passed in price == sl, cannot adjust"); 
      }
      sl = NormalizeDouble(sl,MarketInfo(symbol,MODE_DIGITS)); 
   }
}

string OrderReliable_CommandString(int cmd) {
   if (cmd == OP_BUY) {
      return("OP_BUY");
   }
   if (cmd == OP_SELL) {
      return("OP_SELL");
   }
   if (cmd == OP_BUYSTOP) {
      return("OP_BUYSTOP");
   }
   if (cmd == OP_SELLSTOP) {
      return("OP_SELLSTOP");
   }
   return("(CMD=="+cmd+")"); 

}
void OrderReliable_SleepRandomTime(double mean_time, double max_time) {
// This sleeps a random amount of time defined by 
// an exponential probability distribution. The mean time, in Seconds
// is given in 'mean_time'
// This is the back-off strategy used by Ethernet.  This will 
// quantize in tenths of seconds, so don't call this with a too 
// small a number.  This returns immediately if we are backtesting
// and does not sleep.
//
// Matt Kennel mbkennel@gmail.com.
//
   if (IsTesting()) return; // return immediately if backtesting.

   double tenths = MathCeil(mean_time / 0.1);
   if (tenths <= 0) return; 
    
   int maxtenths = MathRound(max_time/0.1); 
   double p = 1.0 - 1.0/tenths; 
     
   Sleep(1000); // one tenth of a second
   for(int i=0; i<maxtenths; i++)  {
      if (MathRand() > p*32768) break; 
      // MathRand() returns in 0..32767
      Sleep(1000); 
   }
}  