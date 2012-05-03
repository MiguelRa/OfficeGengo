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
using JobsViewer;
using System.Drawing;
using MyGengoTranslator.Classes;

namespace MyGengoTranslator
{
    class MyJobDataProvider: IJobDataProvider
    {
        public System.Drawing.Image GetTierImage(object job)
        {
            Job jobObj = (Job)job;

            Image tierImage = null;

            if(jobObj.Tier == "standard")
                tierImage = Properties.Resources.mygengo_standard_medium;
            else if (jobObj.Tier == "pro")
                tierImage = Properties.Resources.mygengo_pro_medium;
            else if (jobObj.Tier == "ultra")
                tierImage = Properties.Resources.mygengo_ultra_medium;
            else if (jobObj.Tier == "ultra_pro")
                tierImage = Properties.Resources.mygengo_ultra_medium;
            else if (jobObj.Tier == "machine")
                tierImage = Properties.Resources.mygengo_machine_medium;
         
            return tierImage;
        }

        public string GetId(object job)
        {
            Job jobObj = (Job)job;
            string id = jobObj.Job_Id;
            return id;
        }

        public string GetTitle(object job)
        {
            Job jobObj = (Job)job;
            string title = jobObj.Slug;
            return title;
        }

        public string GetSourceLanguage(object job)
        {
            Job jobObj = (Job)job;
            string srcLanguage = Utils.GetLanguageStr(jobObj.Lc_src);
            return srcLanguage;
        }

        public string GetTargetLanguage(object job)
        {
            Job jobObj = (Job)job;
            string tgtLanguage = Utils.GetLanguageStr(jobObj.Lc_tgt);
            return tgtLanguage;
        }

        public string GetWordCount(object job)
        {
            Job jobObj = (Job)job;
            string wordCount = jobObj.Unit_count.ToString();
            return wordCount;
        }

        public string GetCredits(object job)
        {
            Job jobObj = (Job)job;
            string credits = jobObj.Credits.ToString();
            string creditsFormatted = Utils.ConvertToCurrencyString(credits);
            return creditsFormatted;
        }

        public string GetDate(object job)
        {
            Job jobObj = (Job)job;
            string date = jobObj.Ctime.ToShortDateString();
            return date;
        }

        public string GetStatus(object job)
        {
            Job jobObj = (Job)job;
            string value = jobObj.Status;
            string status;
            if (value == EJobStatus.JOB_STATUS_AVAILABLE)
                status = string.Format(Properties.Resources.JOB_STATUS_AVAILABLE_FRIENDLY, "\n");
            else if (value == EJobStatus.JOB_STATUS_PENDING)
                status = Properties.Resources.JOB_STATUS_PENDING_FRIENDLY;
            else if (value == EJobStatus.JOB_STATUS_REVIEWABLE)
                status = Properties.Resources.JOB_STATUS_REVIEWABLE_FRIENDLY;
            else if (value == EJobStatus.JOB_STATUS_APPROVED)
                status = Properties.Resources.JOB_STATUS_APPROVED_FRIENDLY;
            else if (value == EJobStatus.JOB_STATUS_CANCELLED)
                status = Properties.Resources.JOB_STATUS_CANCELLED_FRIENDLY;
            else if (value == EJobStatus.JOB_STATUS_REVISING)
                status = Properties.Resources.JOB_STATUS_REVISING_FRIENDLY;
            else if (value == EJobStatus.JOB_STATUS_HELD)
                status = string.Format(Properties.Resources.JOB_STATUS_HELD_FRIENDLY, "\n");
            else
                status = value;
           
            return status;
        }

        public bool GetBtnViewEnabled(object job)
        {
            Job jobObj = (Job)job;
            string status = jobObj.Status;

            // disabled: -1, reviewable, other
            // enabled: available, pending, approved, revising, rejected-pending-review
            bool enabled = false;
            if (status == EJobStatus.JOB_STATUS_AVAILABLE ||
                status == EJobStatus.JOB_STATUS_PENDING ||
                status == EJobStatus.JOB_STATUS_APPROVED ||
                status == EJobStatus.JOB_STATUS_REVISING ||
                status == EJobStatus.JOB_STATUS_HELD)
            {
                enabled = true;
            }

            return enabled;
                

        }

        public bool GetBtnReviewEnabled(object job)
        {
            Job jobObj = (Job)job;
            string status = jobObj.Status;

            // disabled: -1, available, pending, approved, other
            // enabled: reviewable
            bool enabled = false;
            if (status == EJobStatus.JOB_STATUS_REVIEWABLE)
            {
                enabled = true;
            }

            return enabled;           
        }

        public bool GetBtnCancelEnabled(object job)
        {
            Job jobObj = (Job)job;
            string status = jobObj.Status;

            // disabled: -1, pending, reviewable, approved, other
            // enabled: available
            bool enabled = false;
            if (status == EJobStatus.JOB_STATUS_AVAILABLE)
            {
                enabled = true;
            }

            return enabled;
        }
        
        public string GetObjectKey(object job)
        {
            return "Job_Id";
        }

        public Color GetStatusColor(object job)
        {
            Job jobObj = (Job)job;
            string status = jobObj.Status;

            if (status == EJobStatus.JOB_STATUS_CANCELLED)
                return Color.Red;
            else
                return Color.DodgerBlue;
        }
    }
}
