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

namespace MyOutlookAddin
{
    partial class UCTranslationPanel
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
            this.lblYourCredits = new System.Windows.Forms.Label();
            this.lblCredits = new System.Windows.Forms.Label();
            this.pnlFooterContainer = new System.Windows.Forms.Panel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.lblCopyrightNotice = new System.Windows.Forms.Label();
            this.pictureBoxPowered = new System.Windows.Forms.PictureBox();
            this.pictureBoxLogo = new System.Windows.Forms.PictureBox();
            this.btnOverview = new System.Windows.Forms.Button();
            this.btnOrder = new System.Windows.Forms.Button();
            this.btnStart = new System.Windows.Forms.Button();
            this.pnlFooterContainer.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPowered)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLogo)).BeginInit();
            this.SuspendLayout();
            // 
            // lblYourCredits
            // 
            this.lblYourCredits.AutoSize = true;
            this.lblYourCredits.Location = new System.Drawing.Point(11, 36);
            this.lblYourCredits.Margin = new System.Windows.Forms.Padding(3, 0, 3, 5);
            this.lblYourCredits.Name = "lblYourCredits";
            this.lblYourCredits.Size = new System.Drawing.Size(69, 13);
            this.lblYourCredits.TabIndex = 3;
            this.lblYourCredits.Text = "Your credits: ";
            this.lblYourCredits.Visible = false;
            // 
            // lblCredits
            // 
            this.lblCredits.AutoSize = true;
            this.lblCredits.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCredits.Location = new System.Drawing.Point(82, 34);
            this.lblCredits.Name = "lblCredits";
            this.lblCredits.Size = new System.Drawing.Size(16, 17);
            this.lblCredits.TabIndex = 4;
            this.lblCredits.Text = "?";
            this.lblCredits.Visible = false;
            // 
            // pnlFooterContainer
            // 
            this.pnlFooterContainer.Controls.Add(this.tableLayoutPanel1);
            this.pnlFooterContainer.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlFooterContainer.Location = new System.Drawing.Point(8, 138);
            this.pnlFooterContainer.Name = "pnlFooterContainer";
            this.pnlFooterContainer.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.pnlFooterContainer.Size = new System.Drawing.Size(394, 75);
            this.pnlFooterContainer.TabIndex = 8;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.lblCopyrightNotice, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.pictureBoxPowered, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(394, 75);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // lblCopyrightNotice
            // 
            this.lblCopyrightNotice.AutoSize = true;
            this.lblCopyrightNotice.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lblCopyrightNotice.ForeColor = System.Drawing.Color.Teal;
            this.lblCopyrightNotice.Location = new System.Drawing.Point(3, 0);
            this.lblCopyrightNotice.Name = "lblCopyrightNotice";
            this.lblCopyrightNotice.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblCopyrightNotice.Size = new System.Drawing.Size(388, 13);
            this.lblCopyrightNotice.TabIndex = 9;
            this.lblCopyrightNotice.Text = "Joint Copyright (c) 2011 Miguel A. Ramos, Eddy Jiménez. All rights reserved.";
            this.lblCopyrightNotice.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pictureBoxPowered
            // 
            this.pictureBoxPowered.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pictureBoxPowered.Image = global::MyOutlookAddin.Properties.Resources.mygengo_powered;
            this.pictureBoxPowered.Location = new System.Drawing.Point(3, 16);
            this.pictureBoxPowered.Name = "pictureBoxPowered";
            this.pictureBoxPowered.Size = new System.Drawing.Size(388, 31);
            this.pictureBoxPowered.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBoxPowered.TabIndex = 8;
            this.pictureBoxPowered.TabStop = false;
            // 
            // pictureBoxLogo
            // 
            this.pictureBoxLogo.Image = global::MyOutlookAddin.Properties.Resources.logo;
            this.pictureBoxLogo.Location = new System.Drawing.Point(11, 11);
            this.pictureBoxLogo.Margin = new System.Windows.Forms.Padding(3, 3, 3, 10);
            this.pictureBoxLogo.Name = "pictureBoxLogo";
            this.pictureBoxLogo.Size = new System.Drawing.Size(97, 15);
            this.pictureBoxLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBoxLogo.TabIndex = 9;
            this.pictureBoxLogo.TabStop = false;
            // 
            // btnOverview
            // 
            this.btnOverview.Enabled = false;
            this.btnOverview.Image = global::MyOutlookAddin.Properties.Resources.document_view;
            this.btnOverview.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnOverview.Location = new System.Drawing.Point(143, 57);
            this.btnOverview.Name = "btnOverview";
            this.btnOverview.Size = new System.Drawing.Size(60, 56);
            this.btnOverview.TabIndex = 2;
            this.btnOverview.Text = "Overview";
            this.btnOverview.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnOverview.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnOverview.UseVisualStyleBackColor = true;
            // 
            // btnOrder
            // 
            this.btnOrder.Enabled = false;
            this.btnOrder.Image = global::MyOutlookAddin.Properties.Resources.document_edit;
            this.btnOrder.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnOrder.Location = new System.Drawing.Point(77, 57);
            this.btnOrder.Name = "btnOrder";
            this.btnOrder.Size = new System.Drawing.Size(60, 56);
            this.btnOrder.TabIndex = 1;
            this.btnOrder.Text = "Order";
            this.btnOrder.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnOrder.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnOrder.UseVisualStyleBackColor = true;
            // 
            // btnStart
            // 
            this.btnStart.Image = global::MyOutlookAddin.Properties.Resources.stopwatch_run;
            this.btnStart.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnStart.Location = new System.Drawing.Point(11, 57);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(60, 56);
            this.btnStart.TabIndex = 0;
            this.btnStart.Text = "Start";
            this.btnStart.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnStart.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnStart.UseVisualStyleBackColor = true;
            // 
            // UCTranslationPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(223)))), ((int)(((byte)(232)))), ((int)(((byte)(246)))));
            this.Controls.Add(this.pictureBoxLogo);
            this.Controls.Add(this.pnlFooterContainer);
            this.Controls.Add(this.lblCredits);
            this.Controls.Add(this.lblYourCredits);
            this.Controls.Add(this.btnOverview);
            this.Controls.Add(this.btnOrder);
            this.Controls.Add(this.btnStart);
            this.Name = "UCTranslationPanel";
            this.Padding = new System.Windows.Forms.Padding(8);
            this.Size = new System.Drawing.Size(410, 221);
            this.pnlFooterContainer.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPowered)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLogo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnOrder;
        private System.Windows.Forms.Button btnOverview;
        private System.Windows.Forms.Label lblYourCredits;
        private System.Windows.Forms.Label lblCredits;
        private System.Windows.Forms.Panel pnlFooterContainer;
        private System.Windows.Forms.PictureBox pictureBoxPowered;
        private System.Windows.Forms.Label lblCopyrightNotice;
        private System.Windows.Forms.PictureBox pictureBoxLogo;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    }
}
