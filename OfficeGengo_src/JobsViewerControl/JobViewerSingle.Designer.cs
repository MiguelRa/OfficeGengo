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

namespace JobsViewer
{
    partial class JobViewerSingle
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.pnJobsContainer = new System.Windows.Forms.Panel();
            this.tblpnJobsContainer = new System.Windows.Forms.TableLayoutPanel();
            this.pnJob = new System.Windows.Forms.Panel();
            this.tblpnJob = new System.Windows.Forms.TableLayoutPanel();
            this.pnTitle = new System.Windows.Forms.Panel();
            this.lblLanguages = new System.Windows.Forms.Label();
            this.lblWordCount = new System.Windows.Forms.Label();
            this.lblTitle = new System.Windows.Forms.Label();
            this.pnCredits = new System.Windows.Forms.Panel();
            this.lblDate = new System.Windows.Forms.Label();
            this.lblCredits = new System.Windows.Forms.Label();
            this.pnTier = new System.Windows.Forms.Panel();
            this.pbxTier = new System.Windows.Forms.PictureBox();
            this.lblStatus = new System.Windows.Forms.Label();
            this.pnStatus = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblJobIdName = new System.Windows.Forms.Label();
            this.lblIdJobIdValue = new System.Windows.Forms.Label();
            this.pnJobsContainer.SuspendLayout();
            this.tblpnJobsContainer.SuspendLayout();
            this.pnJob.SuspendLayout();
            this.tblpnJob.SuspendLayout();
            this.pnTitle.SuspendLayout();
            this.pnCredits.SuspendLayout();
            this.pnTier.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbxTier)).BeginInit();
            this.pnStatus.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnJobsContainer
            // 
            this.pnJobsContainer.Controls.Add(this.tblpnJobsContainer);
            this.pnJobsContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnJobsContainer.Location = new System.Drawing.Point(0, 0);
            this.pnJobsContainer.Name = "pnJobsContainer";
            this.pnJobsContainer.Size = new System.Drawing.Size(791, 75);
            this.pnJobsContainer.TabIndex = 2;
            // 
            // tblpnJobsContainer
            // 
            this.tblpnJobsContainer.AutoScroll = true;
            this.tblpnJobsContainer.ColumnCount = 1;
            this.tblpnJobsContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblpnJobsContainer.Controls.Add(this.pnJob, 0, 0);
            this.tblpnJobsContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblpnJobsContainer.Location = new System.Drawing.Point(0, 0);
            this.tblpnJobsContainer.Name = "tblpnJobsContainer";
            this.tblpnJobsContainer.RowCount = 1;
            this.tblpnJobsContainer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 71F));
            this.tblpnJobsContainer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tblpnJobsContainer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tblpnJobsContainer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tblpnJobsContainer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tblpnJobsContainer.Size = new System.Drawing.Size(791, 75);
            this.tblpnJobsContainer.TabIndex = 0;
            // 
            // pnJob
            // 
            this.pnJob.BackColor = System.Drawing.Color.WhiteSmoke;
            this.pnJob.Controls.Add(this.tblpnJob);
            this.pnJob.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnJob.Location = new System.Drawing.Point(3, 3);
            this.pnJob.Name = "pnJob";
            this.pnJob.Size = new System.Drawing.Size(785, 69);
            this.pnJob.TabIndex = 0;
            // 
            // tblpnJob
            // 
            this.tblpnJob.ColumnCount = 5;
            this.tblpnJob.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 26.47059F));
            this.tblpnJob.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.tblpnJob.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 73.52941F));
            this.tblpnJob.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 154F));
            this.tblpnJob.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 99F));
            this.tblpnJob.Controls.Add(this.pnTitle, 2, 0);
            this.tblpnJob.Controls.Add(this.pnCredits, 3, 0);
            this.tblpnJob.Controls.Add(this.pnTier, 0, 0);
            this.tblpnJob.Controls.Add(this.pnStatus, 4, 0);
            this.tblpnJob.Controls.Add(this.panel1, 1, 0);
            this.tblpnJob.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblpnJob.Location = new System.Drawing.Point(0, 0);
            this.tblpnJob.Name = "tblpnJob";
            this.tblpnJob.RowCount = 1;
            this.tblpnJob.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblpnJob.Size = new System.Drawing.Size(785, 69);
            this.tblpnJob.TabIndex = 2;
            // 
            // pnTitle
            // 
            this.pnTitle.Controls.Add(this.lblLanguages);
            this.pnTitle.Controls.Add(this.lblWordCount);
            this.pnTitle.Controls.Add(this.lblTitle);
            this.pnTitle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnTitle.Location = new System.Drawing.Point(202, 3);
            this.pnTitle.Name = "pnTitle";
            this.pnTitle.Size = new System.Drawing.Size(326, 63);
            this.pnTitle.TabIndex = 1;
            // 
            // lblLanguages
            // 
            this.lblLanguages.AutoSize = true;
            this.lblLanguages.Location = new System.Drawing.Point(9, 23);
            this.lblLanguages.Name = "lblLanguages";
            this.lblLanguages.Size = new System.Drawing.Size(99, 13);
            this.lblLanguages.TabIndex = 1;
            this.lblLanguages.Text = "English > Japanese";
            // 
            // lblWordCount
            // 
            this.lblWordCount.AutoSize = true;
            this.lblWordCount.Location = new System.Drawing.Point(10, 39);
            this.lblWordCount.Name = "lblWordCount";
            this.lblWordCount.Size = new System.Drawing.Size(44, 13);
            this.lblWordCount.TabIndex = 1;
            this.lblWordCount.Text = "2 words";
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.ForeColor = System.Drawing.Color.DodgerBlue;
            this.lblTitle.Location = new System.Drawing.Point(10, 5);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(46, 15);
            this.lblTitle.TabIndex = 1;
            this.lblTitle.Text = "My Job";
            // 
            // pnCredits
            // 
            this.pnCredits.Controls.Add(this.lblDate);
            this.pnCredits.Controls.Add(this.lblCredits);
            this.pnCredits.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnCredits.Location = new System.Drawing.Point(534, 3);
            this.pnCredits.Name = "pnCredits";
            this.pnCredits.Size = new System.Drawing.Size(148, 63);
            this.pnCredits.TabIndex = 2;
            // 
            // lblDate
            // 
            this.lblDate.AutoSize = true;
            this.lblDate.Location = new System.Drawing.Point(11, 26);
            this.lblDate.Name = "lblDate";
            this.lblDate.Size = new System.Drawing.Size(91, 13);
            this.lblDate.TabIndex = 1;
            this.lblDate.Text = "Mon Feb 21 2011";
            // 
            // lblCredits
            // 
            this.lblCredits.AutoSize = true;
            this.lblCredits.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCredits.ForeColor = System.Drawing.Color.DodgerBlue;
            this.lblCredits.Location = new System.Drawing.Point(12, 5);
            this.lblCredits.Name = "lblCredits";
            this.lblCredits.Size = new System.Drawing.Size(70, 15);
            this.lblCredits.TabIndex = 1;
            this.lblCredits.Text = "0.80 credits";
            // 
            // pnTier
            // 
            this.pnTier.Controls.Add(this.pbxTier);
            this.pnTier.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnTier.Location = new System.Drawing.Point(3, 3);
            this.pnTier.Name = "pnTier";
            this.pnTier.Size = new System.Drawing.Size(113, 63);
            this.pnTier.TabIndex = 4;
            // 
            // pbxTier
            // 
            this.pbxTier.Location = new System.Drawing.Point(0, 6);
            this.pbxTier.Name = "pbxTier";
            this.pbxTier.Size = new System.Drawing.Size(92, 44);
            this.pbxTier.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pbxTier.TabIndex = 1;
            this.pbxTier.TabStop = false;
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStatus.ForeColor = System.Drawing.Color.DodgerBlue;
            this.lblStatus.Location = new System.Drawing.Point(3, 5);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(71, 15);
            this.lblStatus.TabIndex = 2;
            this.lblStatus.Text = "Reviewable";
            // 
            // pnStatus
            // 
            this.pnStatus.Controls.Add(this.lblStatus);
            this.pnStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnStatus.Location = new System.Drawing.Point(688, 3);
            this.pnStatus.Name = "pnStatus";
            this.pnStatus.Size = new System.Drawing.Size(94, 63);
            this.pnStatus.TabIndex = 5;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.lblIdJobIdValue);
            this.panel1.Controls.Add(this.lblJobIdName);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(122, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(74, 63);
            this.panel1.TabIndex = 6;
            // 
            // lblJobIdName
            // 
            this.lblJobIdName.AutoSize = true;
            this.lblJobIdName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblJobIdName.ForeColor = System.Drawing.Color.DodgerBlue;
            this.lblJobIdName.Location = new System.Drawing.Point(11, 6);
            this.lblJobIdName.Name = "lblJobIdName";
            this.lblJobIdName.Size = new System.Drawing.Size(37, 15);
            this.lblJobIdName.TabIndex = 2;
            this.lblJobIdName.Text = "Job #\r\n";
            // 
            // lblIdJobIdValue
            // 
            this.lblIdJobIdValue.AutoSize = true;
            this.lblIdJobIdValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblIdJobIdValue.ForeColor = System.Drawing.Color.DodgerBlue;
            this.lblIdJobIdValue.Location = new System.Drawing.Point(11, 26);
            this.lblIdJobIdValue.Name = "lblIdJobIdValue";
            this.lblIdJobIdValue.Size = new System.Drawing.Size(42, 15);
            this.lblIdJobIdValue.TabIndex = 2;
            this.lblIdJobIdValue.Text = "10548";
            // 
            // JobViewerSingle
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pnJobsContainer);
            this.Name = "JobViewerSingle";
            this.Size = new System.Drawing.Size(791, 75);
            this.pnJobsContainer.ResumeLayout(false);
            this.tblpnJobsContainer.ResumeLayout(false);
            this.pnJob.ResumeLayout(false);
            this.tblpnJob.ResumeLayout(false);
            this.pnTitle.ResumeLayout(false);
            this.pnTitle.PerformLayout();
            this.pnCredits.ResumeLayout(false);
            this.pnCredits.PerformLayout();
            this.pnTier.ResumeLayout(false);
            this.pnTier.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbxTier)).EndInit();
            this.pnStatus.ResumeLayout(false);
            this.pnStatus.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnJobsContainer;
        private System.Windows.Forms.TableLayoutPanel tblpnJobsContainer;
        private System.Windows.Forms.Panel pnJob;
        private System.Windows.Forms.TableLayoutPanel tblpnJob;
        private System.Windows.Forms.Panel pnTitle;
        private System.Windows.Forms.Label lblLanguages;
        private System.Windows.Forms.Label lblWordCount;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Panel pnCredits;
        private System.Windows.Forms.Label lblDate;
        private System.Windows.Forms.Label lblCredits;
        private System.Windows.Forms.Panel pnTier;
        private System.Windows.Forms.PictureBox pbxTier;
        private System.Windows.Forms.Panel pnStatus;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lblJobIdName;
        private System.Windows.Forms.Label lblIdJobIdValue;
    }
}
