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

namespace MyGengoTranslator.Classes
{
    public class Language
    {
      //        "language": "English",
      //"localized_name": "English",
      //"lc": "en",
      //"unit_type": "word"

        private string _languageName;
        public string LanguageName
        {
            get { return _languageName; }
            set { _languageName = value; }
        }

        private string _localizedName;
        public string LocalizedName
        {
            get { return _localizedName; }
            set { _localizedName = value; }
        }

        private string _lc;
        public string Lc
        {
            get { return _lc; }
            set { _lc = value; }
        }

        private string _unitType;

        public string UnitType
        {
            get { return _unitType; }
            set { _unitType = value; }
        }

        public Language() { }
    }
}
