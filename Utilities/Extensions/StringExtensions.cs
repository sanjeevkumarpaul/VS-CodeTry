using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using Utilities.Constants;

namespace Utilities.Extensions
{
    public static class StringExtensions
    {
        public static string AllUpper(this string str)
        {
            return str.ToUpper();
        }

        public static string[] ToLower(this string[] strs)
        {
            for (var index = 0; index < strs.Length; index++) strs[index] = strs[index].ToLower();

            return strs;
        }

        public static string ToEmpty(this string str)
        {
            if (string.IsNullOrWhiteSpace(str)) str = string.Empty;

            return str;
        }

        public static string ToNull(this string str, bool suppressSQLNull = false)
        {
            if (str.Empty()) return null;

            if (suppressSQLNull && str.ToUpper().Equals("NULL")) return null;

            return str;
        }

        public static bool Empty(this string str)
        {
            return string.IsNullOrWhiteSpace(str);
        }

        public static string RemoveSeprators(this string str)
        {
            return Regex.Replace(str, "[^0-9a-zA-Z]+", "");
        }

        public static string[] SplitCamalized(this string str)
        {
            return Regex.Split(str, "([A-Z])");
        }

        public static string SpaceCamalized(this string str)
        {
            string res = string.Empty;
            var words = Regex.Split(str, "([A-Z])").ToList();

            if (words.Count > 1)
                words.ForEach(s => { res += $"{ (Regex.IsMatch(s, "^[A-Z]") ? " " : "") + s }"; });
            else
                res = words.JoinExt().Captalize();

            return res.TrimEx(" ");
        }
        
        public static string RightTill(this string str, string delimiter = " ")
        {
            if (str.Empty()) return "";

            List<string> entries = str.SplitEx(delimiter).ToList();

            if (entries.Count() > 1)
                entries.RemoveAt(entries.Count() - 1);

            return entries.JoinExt(delimiter);
        }

        public static string Captalize(this string str)
        {
            if (!str.Empty())
            {
                return str[0].ToString().ToUpper() + str.Substring(1);
            }

            return null;
        }

        public static string RepeatFormatPlaceHolder(this string str, int length, string separator = "," )
        {
            str = "{0}";

            for (int i = 1; i < length; i++)
                str += "," + "{" + i.ToString()  +"}";
            
            return str;

        }

        public static string Repeat(this string str, int length)
        {
            if (str.Empty()) return null;
            var temp = str;

            for (int r = 1; r < length; r++) str += temp;

            return str;
        }

        public static string[] SplitEx(this string str, char delimiter = ' ', bool removeEmpty = true)
        {
            return str.Split(new char[] { delimiter }, removeEmpty ? StringSplitOptions.RemoveEmptyEntries : StringSplitOptions.None);            
        }

        public static string[] SplitEx(this string str, string delimiter = " ", bool removeEmpty = true)
        {
            return str.Split(new string[] { delimiter }, removeEmpty ? StringSplitOptions.RemoveEmptyEntries : StringSplitOptions.None);
        }

        public static int ShotterLength(this string str, int defaultLen = 3)
        {
            return (str.Length <= defaultLen) ? str.Length : defaultLen;
        }

        public static string ToTitleCase(this string str)
        {
            var tokens = str.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            for (var i = 0; i < tokens.Length; i++)
            {
                var token = tokens[i];
                tokens[i] = token.Substring(0, 1).ToUpper() + token.Substring(1);
            }

            return string.Join(" ", tokens);
        }

        public static string FriendlyColumnName(this String str)
        {
            str = String.Join(" ", str.Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries));

            // TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            // str = textInfo.ToTitleCase(str);

            str = str.ToTitleCase();

            return str;
        }

        public static bool EqualsIgnoreCase(this string str, string withStr)
        {
            if (str.Null()) return false;

            return str.Equals(withStr, StringComparison.CurrentCultureIgnoreCase);
        }

        public static string RemoveDomain(this string str)
        {
            //int pos = -1;

            //return (!str.Empty() && (pos = str.LastIndexOf('\\')) >= 0) ? str.Substring(pos + 1) : str; 
            return str.TakeLast('\\');
        }

        public static string TakeLast(this string str, char delimter = '.')
        {
            int pos = -1;

            return (!str.Empty() && (pos = str.LastIndexOf(delimter)) >= 0) ? str.Substring(pos + 1) : str;
        }

        public static string UnescapeXmlNotations(this string str)
        {
            if (str.Empty()) return str;

            return str.Replace("&lt;", "<").Replace("&gt;", ">").Replace("&quot;", "\"").Replace("&amp;", "&");
        }


        public static Int32 ToInt(this string str)
        {
            int result = 0;

            Int32.TryParse(str, out result);

            return result;
        }

        public static DateTime ToDateTime(this string str, bool OnlySeconds = false)
        {
            DateTime mDateTime;
            DateTime.TryParse(str, out mDateTime);
            mDateTime = (mDateTime.Equals(DateTime.MinValue)) ? DateTime.Now : mDateTime;

            return OnlySeconds ? Convert.ToDateTime(mDateTime.ToString(AppConstant.DateFormat)) : mDateTime;
        }

        public static string TrimEx(this string str, string param = " ")
        {
            if (str.Empty()) return str;
            if (!str.StartsWith(param) && !str.EndsWith(param)) return str;

            string tstr = str;

            while (tstr.EndsWith(param))
            {
                tstr = tstr.Substring(0, tstr.Length - param.Length);
            }
            while (tstr.StartsWith(param))
            {
                tstr = tstr.Substring(param.Length);
            }

            return tstr;
        }

        public static string SQLQueryFormat(this string strTable, string condition = null, bool EmptyWithoutCondition = true)
        {
            string query = "";
            if (!(EmptyWithoutCondition && condition.Empty()))
            {
                if (!strTable.Empty())  query += string.Format("SELECT * FROM [{0}] ", strTable.ToUpper());
                if (!condition.Empty()) query += string.Format(" WHERE ( {0} ) ",condition.ToUpper());
            }
            return query;
        }
        
        public static string SQLTrimScehma(this string strTable)
        {
            string[] schemas = strTable.SplitEx(".");
            return schemas[schemas.Length - 1];
        }

        public static string RemoveSQLVariableDeclarations(this string query)
        {
            if (!query.Empty())
            {
                string _query = query.ToUpper().Replace(" =", "=").Replace("= ", "=").Replace("=", "= ");
                
                Int32 FROM_Index = query.IndexOf("FROM");
                string _part = _query.Substring(0, FROM_Index - 1);

                Int32 AT_Index = _part.IndexOf("@");
                if (AT_Index <= 0) return query;

                foreach (string variable in Regex.Matches(_query, @"\@\w*=").OfType<Match>().Select(m => m.ToString()))
                {
                    _query = _query.Replace(variable, "");
                }
                query = _query;
            }

            return query;
        }
        
        public static bool IsSQL(this string str)
        {
            str = str.ToUpper();

            return !str.Empty() && ( str.StartsWith("SELECT") || str.StartsWith("UPDATE") || str.StartsWith("DELETE") || str.StartsWith("BEGIN"));
        }

        public static string SqlQueryType(this string str)
        {
            str = str.ToUpper().Trim();
            if (str.StartsWith("SELECT")) return "SELECT";
            else if (str.StartsWith("UPDATE")) return "UPDATE";
            else if (str.StartsWith("DELETE")) return "DELETE";
            else if (str.StartsWith("BEGIN")) return "TRANSACTION";
            else return "";
        }
    }
}