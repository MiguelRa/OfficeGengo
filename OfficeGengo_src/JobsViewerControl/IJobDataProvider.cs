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
using System.Drawing;

namespace JobsViewer
{
    public interface IJobDataProvider
    {
        string GetId(object job);

        Image GetTierImage(object job);

        string GetTitle(object job);

        string GetSourceLanguage(object job);

        string GetTargetLanguage(object job);

        string GetWordCount(object job);

        string GetCredits(object job);

        string GetDate(object job);

        string GetStatus(object job);

        bool GetBtnViewEnabled(object job);

        bool GetBtnReviewEnabled(object job);

        bool GetBtnCancelEnabled(object job);

        Color GetStatusColor(object job);

        string GetObjectKey(object job);        
    }
}
