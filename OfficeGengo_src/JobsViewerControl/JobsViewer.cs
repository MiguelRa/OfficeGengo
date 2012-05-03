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
using System.Reflection;

namespace JobsViewer
{
    public partial class JobsViewer : UserControl
    {
        #region properties

        private IJobDataProvider _jobDataProvider;
        private Color _jobPanelBgColor = Color.White;
        private Color _jobPanelSelectedBgColor = Color.WhiteSmoke;
        private object[] _jobList;

        public IJobDataProvider JobDataProvider
        {
            get { return _jobDataProvider; }
            set { _jobDataProvider = value; }
        }       

        public Color JobPanelBgColor
        {
            get { return _jobPanelBgColor; }
            set { _jobPanelBgColor = value; }
        }

        public Color JobPanelSelectedBgColor
        {
            get { return _jobPanelSelectedBgColor; }
            set { _jobPanelSelectedBgColor = value; }
        }

        #endregion

        #region constructors

        public JobsViewer()
        {
            InitializeComponent();

            //// if app is running, remove sample table
            //if (!this.DesignMode)
            //{
            //    ResetJobList();
            //}
            
        }

        #endregion

        #region event handlers

        private void tblpnJob_MouseEnter(object sender, EventArgs e)
        {
            // reset bg color on all rows
            foreach (Control panelJob in tblpnJobsContainer.Controls)
            {
                if (panelJob.Controls.Count > 0)
                {
                    Control tableJob = panelJob.Controls[0];
                    tableJob.BackColor = _jobPanelBgColor;
                }
            }

            // highlight selected row
            Control controlHovered = (Control)sender;
            controlHovered.BackColor = _jobPanelSelectedBgColor;
        }

        private void JobsViewer_Load(object sender, EventArgs e)
        {

            // if app is running, remove sample table
            if (!this.DesignMode && (_jobList == null || _jobList.Length == 0))
            {
                ResetJobList();
            }

            /*
            // if app is running, remove sample table
            if (!this.DesignMode && (_jobList == null || _jobList.Length == 0))
            {
                this.pnJobsContainer.Controls.Remove(this.tblpnJobsContainer);

                this.tblpnJobsContainer = new System.Windows.Forms.TableLayoutPanel();
                this.tblpnJobsContainer.AutoScroll = true;
                this.tblpnJobsContainer.ColumnCount = 1;
                this.tblpnJobsContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
                this.tblpnJobsContainer.Dock = System.Windows.Forms.DockStyle.Fill;
                this.tblpnJobsContainer.Location = new System.Drawing.Point(0, 0);
                this.tblpnJobsContainer.Name = "tblpnJobsContainer";
                this.tblpnJobsContainer.RowCount = 1;
                this.tblpnJobsContainer.RowStyles.Add(new System.Windows.Forms.RowStyle());
                this.tblpnJobsContainer.Size = new System.Drawing.Size(791, 395);
                this.tblpnJobsContainer.TabIndex = 0;

                this.pnJobsContainer.Controls.Add(this.tblpnJobsContainer);

            }
             * */
        }

        private void btnView_Click(object sender, EventArgs e)
        {
            Control button = ((Control)sender);
            ViewClick(sender, button.Tag);
        }

        private void btnReview_Click(object sender, EventArgs e)
        {
            Control button = ((Control)sender);
            ReviewClick(sender, button.Tag);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Control button = ((Control)sender);
            CancelClick(sender, button.Tag);
        }

        #endregion

        #region private methods

        private Panel BuildJobPanel(object job,string jobId, Image tierImage, string title, string srcLanguage,
            string tgtLanguage, string wordCount, string credits, string date, string status, bool btnViewEnabled, 
            bool btnReviewEnabled, bool btnCancelEnabled)
        {
            // ***BUILD CELL:TIER**********************
            //pbxTier
            PictureBox pbxTier1 = new System.Windows.Forms.PictureBox();
            pbxTier1.Location = new System.Drawing.Point(0, 6);
            pbxTier1.Name = "pbxTier" + jobId;
            pbxTier1.Size = new System.Drawing.Size(92, 44);
            pbxTier1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            pbxTier1.TabIndex = 1;
            pbxTier1.TabStop = false;
            pbxTier1.Image = tierImage;

            // pnTier
            Panel pnTier1 = new System.Windows.Forms.Panel();
            pnTier1.Controls.Add(pbxTier1);
            pnTier1.Dock = System.Windows.Forms.DockStyle.Fill;
            pnTier1.Location = new System.Drawing.Point(3, 3);
            pnTier1.Name = "pnTier" + jobId;
            pnTier1.Size = new System.Drawing.Size(103, 59);
            pnTier1.TabIndex = 4;
            // ***END BUILD CELL:TIER***

            // ***BUILD CELL:TITLE/LANGUAGES/WORDS*****************       
            //lblLanguages
            Label lblLanguages1 = new System.Windows.Forms.Label();
            lblLanguages1.AutoSize = true;
            lblLanguages1.Location = new System.Drawing.Point(9, 23);
            lblLanguages1.Name = "lblLanguages" + jobId;
            lblLanguages1.Size = new System.Drawing.Size(99, 13);
            lblLanguages1.TabIndex = 1;
            lblLanguages1.Text = string.Format("{0} > {1}", srcLanguage, tgtLanguage);

            //lblWordCount
            Label lblWordCount1 = new System.Windows.Forms.Label();
            lblWordCount1.AutoSize = true;
            lblWordCount1.Location = new System.Drawing.Point(10, 39);
            lblWordCount1.Name = "lblWordCount" + jobId;
            lblWordCount1.Size = new System.Drawing.Size(44, 13);
            lblWordCount1.TabIndex = 1;
            lblWordCount1.Text = wordCount + " words";

            //lblTitle
            Label lblTitle1 = new System.Windows.Forms.Label();
            lblTitle1.AutoSize = true;
            lblTitle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            lblTitle1.ForeColor = System.Drawing.Color.DodgerBlue;
            lblTitle1.Location = new System.Drawing.Point(10, 5);
            lblTitle1.Name = "lblTitle" + jobId;
            lblTitle1.Size = new System.Drawing.Size(46, 15);
            lblTitle1.TabIndex = 1;
            lblTitle1.Text = title;

            // pnTitle
            Panel pnTitle1 = new System.Windows.Forms.Panel();
            pnTitle1.Controls.Add(lblLanguages1);
            pnTitle1.Controls.Add(lblWordCount1);
            pnTitle1.Controls.Add(lblTitle1);
            pnTitle1.Dock = System.Windows.Forms.DockStyle.Fill;
            pnTitle1.Location = new System.Drawing.Point(112, 3);
            pnTitle1.Name = "pnTitle" + jobId;
            pnTitle1.Size = new System.Drawing.Size(238, 59);
            pnTitle1.TabIndex = 1;
            // *** END CELL2-TITLE**********

            // ***BUILD CELL:CREDITS*********
            //lblDate
            Label lblDate1 = new System.Windows.Forms.Label();
            lblDate1.AutoSize = true;
            lblDate1.Location = new System.Drawing.Point(11, 26);
            lblDate1.Name = "lblDate" + jobId;
            lblDate1.Size = new System.Drawing.Size(91, 13);
            lblDate1.TabIndex = 1;
            lblDate1.Text = date;

            //lblCredits
            Label lblCredits1 = new System.Windows.Forms.Label();
            lblCredits1.AutoSize = true;
            lblCredits1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            lblCredits1.ForeColor = System.Drawing.Color.DodgerBlue;
            lblCredits1.Location = new System.Drawing.Point(12, 5);
            lblCredits1.Name = "lblCredits" + jobId;
            lblCredits1.Size = new System.Drawing.Size(70, 15);
            lblCredits1.TabIndex = 1;
            lblCredits1.Text = string.Format("{0} credits", credits);

            // pnCredits
            Panel pnCredits1 = new System.Windows.Forms.Panel();
            pnCredits1.Controls.Add(lblDate1);
            pnCredits1.Controls.Add(lblCredits1);
            pnCredits1.Dock = System.Windows.Forms.DockStyle.Fill;
            pnCredits1.Location = new System.Drawing.Point(356, 3);
            pnCredits1.Name = "pnCredits" + jobId;
            pnCredits1.Size = new System.Drawing.Size(129, 59);
            pnCredits1.TabIndex = 2;
            // ***END BUILD CELL:TIER***

            // ***BUILD CELL:STATUS***************************
            //lblStatus
            Label lblStatus1 = new System.Windows.Forms.Label();
            lblStatus1.AutoSize = true;
            lblStatus1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            lblStatus1.ForeColor = System.Drawing.Color.DodgerBlue;
            lblStatus1.Location = new System.Drawing.Point(3, 5);
            lblStatus1.Name = "lblStatus" + jobId;
            lblStatus1.Size = new System.Drawing.Size(71, 15);
            lblStatus1.TabIndex = 2;
            lblStatus1.Text = status;

            // pnStatus
            Panel pnStatus1 = new System.Windows.Forms.Panel();
            pnStatus1.Controls.Add(lblStatus1);
            pnStatus1.Dock = System.Windows.Forms.DockStyle.Fill;
            pnStatus1.Location = new System.Drawing.Point(491, 3);
            pnStatus1.Name = "pnStatus" + jobId;
            pnStatus1.Size = new System.Drawing.Size(78, 59);
            pnStatus1.TabIndex = 5;
            // ***END BUILD CELL:TIER***

            // ***BUILD CELL:ACTION BUTTONS***
            //btnView
            Button btnView1 = new System.Windows.Forms.Button();
            btnView1.Location = new System.Drawing.Point(20, 8);
            btnView1.Name = "btnView" + jobId;
            btnView1.Size = new System.Drawing.Size(55, 31);
            btnView1.TabIndex = 0;
            btnView1.Text = "View";
            btnView1.UseVisualStyleBackColor = true;
            btnView1.Tag = job;
            btnView1.Enabled = btnViewEnabled;
            btnView1.Click += new System.EventHandler(this.btnView_Click);

            //btnReview
            Button btnReview1 = new System.Windows.Forms.Button();
            btnReview1.Location = new System.Drawing.Point(76, 8);
            btnReview1.Name = "btnReview" + jobId;
            btnReview1.Size = new System.Drawing.Size(55, 31);
            btnReview1.TabIndex = 0;
            btnReview1.Text = "Review";
            btnReview1.UseVisualStyleBackColor = true;
            btnReview1.Tag = job;
            btnReview1.Enabled = btnReviewEnabled;
            btnReview1.Click += new System.EventHandler(this.btnReview_Click);

            //btnCancel
            Button btnCancel1 = new System.Windows.Forms.Button();
            btnCancel1.Location = new System.Drawing.Point(132, 8);
            btnCancel1.Name = "btnCancel" + jobId;
            btnCancel1.Size = new System.Drawing.Size(55, 31);
            btnCancel1.TabIndex = 0;
            btnCancel1.Text = "Cancel";
            btnCancel1.UseVisualStyleBackColor = true;
            btnCancel1.Tag = job;
            btnCancel1.Enabled = btnCancelEnabled;
            btnCancel1.Click += new System.EventHandler(this.btnCancel_Click);

            // pnActionButtons 
            Panel pnActionButtons1 = new System.Windows.Forms.Panel();
            pnActionButtons1.Controls.Add(btnCancel1);
            pnActionButtons1.Controls.Add(btnReview1);
            pnActionButtons1.Controls.Add(btnView1);
            pnActionButtons1.Dock = System.Windows.Forms.DockStyle.Fill;
            pnActionButtons1.Location = new System.Drawing.Point(575, 3);
            pnActionButtons1.Name = "pnActionButtons" + jobId;
            pnActionButtons1.Size = new System.Drawing.Size(207, 59);
            pnActionButtons1.TabIndex = 3;

            // ***END BUILD CELL:ACTION BUTTONS***

            // tblpnJob
            TableLayoutPanel tblpnJob1 = new System.Windows.Forms.TableLayoutPanel();
            tblpnJob1.ColumnCount = 5;
            tblpnJob1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30.91483F));
            tblpnJob1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 69.08517F));
            tblpnJob1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 135F));
            tblpnJob1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 84F));
            tblpnJob1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 212F));
            tblpnJob1.Controls.Add(pnTier1, 0, 0);
            tblpnJob1.Controls.Add(pnTitle1, 1, 0);
            tblpnJob1.Controls.Add(pnCredits1, 2, 0);
            tblpnJob1.Controls.Add(pnStatus1, 3, 0);
            tblpnJob1.Controls.Add(pnActionButtons1, 4, 0);
            tblpnJob1.Dock = System.Windows.Forms.DockStyle.Fill;
            tblpnJob1.Location = new System.Drawing.Point(0, 0);
            tblpnJob1.Name = "tblpnJob" + jobId;
            tblpnJob1.RowCount = 1;
            tblpnJob1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            tblpnJob1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            tblpnJob1.Size = new System.Drawing.Size(785, 65);
            tblpnJob1.TabIndex = 2;
            tblpnJob1.MouseEnter += new System.EventHandler(this.tblpnJob_MouseEnter);

            // pnJob
            Panel pnJob1 = new System.Windows.Forms.Panel();
            pnJob1.BackColor = _jobPanelBgColor;
            pnJob1.Controls.Add(tblpnJob1);
            pnJob1.Dock = System.Windows.Forms.DockStyle.Fill;
            pnJob1.Location = new System.Drawing.Point(3, 3);
            pnJob1.Name = "pnJob" + jobId;
            pnJob1.Size = new System.Drawing.Size(785, 65);
            pnJob1.TabIndex = 0;

            return pnJob1;

        }

        private void ResetJobList()
        {
            this.pnJobsContainer.Controls.Remove(this.tblpnJobsContainer);

            this.tblpnJobsContainer = new System.Windows.Forms.TableLayoutPanel();
            this.tblpnJobsContainer.AutoScroll = true;
            this.tblpnJobsContainer.ColumnCount = 1;
            this.tblpnJobsContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblpnJobsContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblpnJobsContainer.Location = new System.Drawing.Point(0, 0);
            this.tblpnJobsContainer.Name = "tblpnJobsContainer";
            this.tblpnJobsContainer.RowCount = 1;
            this.tblpnJobsContainer.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblpnJobsContainer.Size = new System.Drawing.Size(791, 395);
            this.tblpnJobsContainer.TabIndex = 0;

            this.pnJobsContainer.Controls.Add(this.tblpnJobsContainer);
        }

        #endregion

        #region public methods

        public bool LoadJobs(object[] arrJobs)
        {
            // check if jobDataProvider was set
            if (_jobDataProvider == null)
                throw new Exception("JobDataProvider cannot be null.");

            if (arrJobs == null)
                throw new Exception("Param arrJobs cannot be null");

            // set property
            _jobList = arrJobs;

            // reset table
            ResetJobList();

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
            bool btnViewEnabled;
            bool btnReviewEnabled;
            bool btnCancelEnabled;
            Panel pnJob1;

            this.tblpnJobsContainer.SuspendLayout();
            foreach (object job in arrJobs)
            {
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
                btnViewEnabled = _jobDataProvider.GetBtnViewEnabled(job);
                btnReviewEnabled = _jobDataProvider.GetBtnReviewEnabled(job);
                btnCancelEnabled = _jobDataProvider.GetBtnCancelEnabled(job);

                // build job panel
                pnJob1 = BuildJobPanel(job, jobId, tierImage, title, srcLanguage, tgtLanguage,
                    wordCount, credits, date, status, btnViewEnabled, btnReviewEnabled, btnCancelEnabled);

                // add job panel to root table                
                this.tblpnJobsContainer.Controls.Add(pnJob1, 0, this.tblpnJobsContainer.RowCount - 1);
                this.tblpnJobsContainer.RowStyles[this.tblpnJobsContainer.RowCount - 1].Height = 71F;
                this.tblpnJobsContainer.RowCount = this.tblpnJobsContainer.RowCount + 1;
                this.tblpnJobsContainer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
                
            }

            this.tblpnJobsContainer.ResumeLayout(false);
            this.tblpnJobsContainer.PerformLayout();

            return true;
        }

        private delegate void UpdateJobCallBack(object pJob);

        public void UpdateJob(object pJob)
        {
            if (tblpnJobsContainer.InvokeRequired)
            {
                UpdateJobCallBack d = new UpdateJobCallBack(UpdateJob);
                this.Invoke(d, pJob);
            }
            else
            {
                if (pJob == null)
                    return;

                PropertyInfo jobPropertyInfo = pJob.GetType().GetProperty(_jobDataProvider.GetObjectKey(pJob));
                PropertyInfo jobItemPropertyInfo = null;

                object jobId = jobPropertyInfo.GetValue(pJob, null);

                for (int i = 0; i < _jobList.Length; i++)
                {
                    jobItemPropertyInfo = _jobList[i].GetType().GetProperty(_jobDataProvider.GetObjectKey(pJob));

                    if (jobId.Equals(jobItemPropertyInfo.GetValue(_jobList[i], null)))
                    {
                        _jobList[i] = pJob;

                        //Cogiendo el label para mostrar el status
                        Control lblStatus = this.Controls.Find("lblStatus" + jobId.ToString(), true).FirstOrDefault();
                        string status = _jobDataProvider.GetStatus(pJob);
                        lblStatus.Text = status;
                        lblStatus.ForeColor = _jobDataProvider.GetStatusColor(pJob);

                        //Cogiendo los botones asociados al job para deshabilitarlos
                        Control btnView = this.Controls.Find("btnView" + jobId.ToString(), true).FirstOrDefault();
                        Control btnReview = this.Controls.Find("btnReview" + jobId.ToString(), true).FirstOrDefault();
                        Control btnCancel = this.Controls.Find("btnCancel" + jobId.ToString(), true).FirstOrDefault();

                        btnView.Enabled = _jobDataProvider.GetBtnViewEnabled(pJob);
                        btnReview.Enabled = _jobDataProvider.GetBtnReviewEnabled(pJob);
                        btnCancel.Enabled = _jobDataProvider.GetBtnCancelEnabled(pJob);

                        break;
                    }
                }
            }
        }

        #endregion

        #region events

        public delegate void JobViewerActionHandler(object sender, object job);
        public  event JobViewerActionHandler ViewClick;
        public event JobViewerActionHandler ReviewClick;
        public event JobViewerActionHandler CancelClick;

        #endregion        
    }
}
