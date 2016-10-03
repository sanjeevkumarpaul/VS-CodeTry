using System;

namespace Utilities.Constants
{
    public class AppConstant
    {
        public static readonly DateTime SQLDateTimeMin = new DateTime(1972,1,1);
        public static readonly DateTime SQLDateTimeMax = new DateTime(2999,12,31);

        public const string DateFormat = "yyyy/MM/dd";
    }   
}