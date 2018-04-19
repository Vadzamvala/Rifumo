using ForexCalendar.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForexCalendar.Domain.Extensions
{
    public static class ImpactTypeExtensions
    {
        public static ImpactType FromString(this ImpactType impactType, string inputImpactType)
        {
            ImpactType currResult;
            if (Enum.TryParse<ImpactType>(inputImpactType, out currResult))
            {
                return currResult;
            }
            return ImpactType.Unspecified;
        }

        public static string ToString(this ImpactType impactType, ImpactType inputImpactType)
        {
            return inputImpactType.ToString();
        }
    }
}
