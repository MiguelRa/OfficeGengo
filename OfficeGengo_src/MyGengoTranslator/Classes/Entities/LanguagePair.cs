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
    public class LanguagePair
    {
        //"lc_src": "de",
        //"lc_tgt": "en",
        //"tier": "machine",
        //"unit_price": "0.0000"

        private string _lc_src;
        public string Lc_src
        {
            get { return _lc_src; }
            set { _lc_src = value; }
        }

        private string _lc_tgt;
        public string Lc_tgt
        {
            get { return _lc_tgt; }
            set { _lc_tgt = value; }
        }

        private string _tier;
        public string Tier
        {
            get { return _tier; }
            set { _tier = value; }
        }

        private float _unit_price;
        public float Unit_price
        {
            get { return _unit_price; }
            set { _unit_price = value; }
        }

        public LanguagePair() { }
    }
}
