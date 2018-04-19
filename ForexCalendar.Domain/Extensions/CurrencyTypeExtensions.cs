using System;
using ForexCalendar.Domain.Models;

namespace ForexCalendar.Domain.Extensions
{
    public static class CurrencyTypeExtensions
    {
        public static CurrencyTypeEnum FromString(this CurrencyType currencyType, string inputCurrency)
        {
            CurrencyTypeEnum currResult;
            if (Enum.TryParse<CurrencyTypeEnum>(inputCurrency, out currResult))
            {
                return currResult;
            }
            return CurrencyTypeEnum.Unknown;
        }

        public static string ToString(this CurrencyType currencyType, CurrencyTypeEnum inputCurrency)
        {
            return inputCurrency.ToString();
        }
    }
}
