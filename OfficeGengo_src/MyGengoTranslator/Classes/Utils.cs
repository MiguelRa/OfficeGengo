#region copyright
/**
 * Joint Copyright (c) 2012 Miguel A. Ramos, Eddy Jimenez 
 * (mramosr85@gmail.com)
 *
 * This file is part of OfficeGengoAddins.
 *
 * OfficeGengoAddins is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * OfficeGengoAddins is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.

 * You should have received a copy of the GNU General Public License
 * along with OfficeGengoAddins.  If not, see <http://www.gnu.org/licenses/>.
 */
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Threading;

namespace MyGengoTranslator.Classes
{
    public static class Utils
    {
        private static List<Language> _languageList = null;

        public static List<Language> LanguageList
        {
            get { return Utils._languageList; }
            set { Utils._languageList = value; }
        }

        public static string GetLanguageStr(string pLc)
        {
            if (_languageList == null)
                return string.Empty;
            return _languageList.Where(l => l.Lc == pLc).FirstOrDefault().LanguageName;
        }

        public static DateTime ConvertFromUnixTimeStamp(double timestamp)
        {
            DateTime dateTimeOrgin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return dateTimeOrgin.AddSeconds(timestamp);

        }

        public static string ConvertToCurrencyString(string numberStr)
        {
            if (string.IsNullOrEmpty(numberStr))
                return numberStr;

            try
            {
                // get comma position
                int commaIndex = numberStr.IndexOf(',');
                if (commaIndex == -1)
                    commaIndex = numberStr.IndexOf('.');

                CultureInfo cultureInfo = Thread.CurrentThread.CurrentUICulture;
                // if no decimal digits, add ".00"
                if (commaIndex == -1)
                {
                    numberStr = numberStr + cultureInfo.NumberFormat.CurrencyDecimalSeparator + "00";
                    return numberStr;
                }

                // if only one digit after comma, add "0"
                string stringAfterComma = string.Empty;
                if (commaIndex != -1)
                    stringAfterComma = numberStr.Substring(commaIndex + 1);

                // add last digit
                if (stringAfterComma.Length == 1)
                    numberStr = numberStr + "0";

                return numberStr;
            }
            catch (Exception ex)
            {
                return numberStr;
            }

        }
    }
}
