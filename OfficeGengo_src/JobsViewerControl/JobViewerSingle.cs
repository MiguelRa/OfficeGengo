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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace JobsViewer
{
    public partial class JobViewerSingle : UserControl
    {
        private delegate bool LoadJobCallBack(object job);

        private IJobDataProvider _jobDataProvider;

        public IJobDataProvider JobDataProvider
        {
            get { return _jobDataProvider; }
            set { _jobDataProvider = value; }
        }
        
        public Color JobPanelBgColor
        {
            get { return pnJob.BackColor; }
            set { pnJob.BackColor = value; }
        }

        public JobViewerSingle()
        {
            InitializeComponent();

            // set default bgColor
            this.JobPanelBgColor = Color.White;
        }

        public bool LoadJob(object job)
        {
            if (lblIdJobIdValue.InvokeRequired)
            {
                LoadJobCallBack d = new LoadJobCallBack(LoadJob);
                this.Invoke(d, job);
            }
            else
            {
                // check if jobDataProvider was set
                if (_jobDataProvider == null)
                    throw new Exception("JobDataProvider cannot be null.");

                if (job == null)
                    throw new Exception("Param Job cannot be null");

                // for each job, build a table row
                string jobId;
                Image tierImage;
                string title;
                string srcLanguage;
                string tgtLanguage;
                string wordCount;
                string credits;
                string date;
                string status;

                // get job data from external provider                
                jobId = _jobDataProvider.GetId(job);
                tierImage = _jobDataProvider.GetTierImage(job);
                title = _jobDataProvider.GetTitle(job);
                srcLanguage = _jobDataProvider.GetSourceLanguage(job);
                tgtLanguage = _jobDataProvider.GetTargetLanguage(job);
                wordCount = _jobDataProvider.GetWordCount(job);
                credits = _jobDataProvider.GetCredits(job);
                date = _jobDataProvider.GetDate(job);
                status = _jobDataProvider.GetStatus(job);

                // fill UI controls with job values
                lblIdJobIdValue.Text = jobId;
                pbxTier.Image = tierImage;
                lblTitle.Text = title;
                lblLanguages.Text = string.Format("{0} > {1}", srcLanguage, tgtLanguage);
                lblWordCount.Text = wordCount + " words";
                lblCredits.Text = string.Format("{0} credits", credits);
                lblDate.Text = date;
                lblStatus.Text = status;
            }
            return true;
        }
    }
}
