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

namespace MyGengoWordAddIn
{
    partial class wGengoRibbon : Microsoft.Office.Tools.Ribbon.RibbonBase
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        public wGengoRibbon()
            : base(Globals.Factory.GetRibbonFactory())
        {
            InitializeComponent();
        }

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
            this.tabwGengo = this.Factory.CreateRibbonTab();
            this.groupMainFuncionalities = this.Factory.CreateRibbonGroup();
            this.box1 = this.Factory.CreateRibbonBox();
            this.lblYourCredits = this.Factory.CreateRibbonLabel();
            this.lblCredits = this.Factory.CreateRibbonLabel();
            this.separator4 = this.Factory.CreateRibbonSeparator();
            this.btnStart = this.Factory.CreateRibbonButton();
            this.separator2 = this.Factory.CreateRibbonSeparator();
            this.btnOrder = this.Factory.CreateRibbonButton();
            this.separator1 = this.Factory.CreateRibbonSeparator();
            this.btnOverview = this.Factory.CreateRibbonButton();
            this.tabwGengo.SuspendLayout();
            this.groupMainFuncionalities.SuspendLayout();
            this.box1.SuspendLayout();
            // 
            // tabwGengo
            // 
            this.tabwGengo.ControlId.ControlIdType = Microsoft.Office.Tools.Ribbon.RibbonControlIdType.Office;
            this.tabwGengo.Groups.Add(this.groupMainFuncionalities);
            this.tabwGengo.Label = "WordGengo";
            this.tabwGengo.Name = "tabwGengo";
            // 
            // groupMainFuncionalities
            // 
            this.groupMainFuncionalities.Items.Add(this.box1);
            this.groupMainFuncionalities.Items.Add(this.lblCredits);
            this.groupMainFuncionalities.Items.Add(this.separator4);
            this.groupMainFuncionalities.Items.Add(this.btnStart);
            this.groupMainFuncionalities.Items.Add(this.separator2);
            this.groupMainFuncionalities.Items.Add(this.btnOrder);
            this.groupMainFuncionalities.Items.Add(this.separator1);
            this.groupMainFuncionalities.Items.Add(this.btnOverview);
            this.groupMainFuncionalities.Label = "Translation by myGengo";
            this.groupMainFuncionalities.Name = "groupMainFuncionalities";
            // 
            // box1
            // 
            this.box1.Items.Add(this.lblYourCredits);
            this.box1.Name = "box1";
            // 
            // lblYourCredits
            // 
            this.lblYourCredits.Label = "Credits:";
            this.lblYourCredits.Name = "lblYourCredits";
            this.lblYourCredits.Visible = false;
            // 
            // lblCredits
            // 
            this.lblCredits.Label = "?";
            this.lblCredits.Name = "lblCredits";
            this.lblCredits.Visible = false;
            // 
            // separator4
            // 
            this.separator4.Name = "separator4";
            // 
            // btnStart
            // 
            this.btnStart.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.btnStart.Image = global::MyGengoWordAddIn.Properties.Resources.stopwatch_run;
            this.btnStart.Label = "Start";
            this.btnStart.Name = "btnStart";
            this.btnStart.ScreenTip = "Log in/ Log off";
            this.btnStart.ShowImage = true;
            // 
            // separator2
            // 
            this.separator2.Name = "separator2";
            // 
            // btnOrder
            // 
            this.btnOrder.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.btnOrder.Enabled = false;
            this.btnOrder.Image = global::MyGengoWordAddIn.Properties.Resources.document_edit;
            this.btnOrder.Label = "Order";
            this.btnOrder.Name = "btnOrder";
            this.btnOrder.ScreenTip = "New translation";
            this.btnOrder.ShowImage = true;
            // 
            // separator1
            // 
            this.separator1.Name = "separator1";
            // 
            // btnOverview
            // 
            this.btnOverview.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.btnOverview.Enabled = false;
            this.btnOverview.Image = global::MyGengoWordAddIn.Properties.Resources.document_view;
            this.btnOverview.Label = "Overview";
            this.btnOverview.Name = "btnOverview";
            this.btnOverview.ScreenTip = "Your translations";
            this.btnOverview.ShowImage = true;
            // 
            // wGengoRibbon
            // 
            this.Name = "wGengoRibbon";
            this.RibbonType = "Microsoft.Word.Document";
            this.Tabs.Add(this.tabwGengo);
            this.Close += new System.EventHandler(this.wGengoRibbon_Close);
            this.Load += new Microsoft.Office.Tools.Ribbon.RibbonUIEventHandler(this.wGengoRibbon_Load);
            this.tabwGengo.ResumeLayout(false);
            this.tabwGengo.PerformLayout();
            this.groupMainFuncionalities.ResumeLayout(false);
            this.groupMainFuncionalities.PerformLayout();
            this.box1.ResumeLayout(false);
            this.box1.PerformLayout();

        }

        #endregion

        internal Microsoft.Office.Tools.Ribbon.RibbonTab tabwGengo;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup groupMainFuncionalities;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnStart;
        internal Microsoft.Office.Tools.Ribbon.RibbonSeparator separator1;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnOrder;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnOverview;
        internal Microsoft.Office.Tools.Ribbon.RibbonLabel lblYourCredits;
        public Microsoft.Office.Tools.Ribbon.RibbonLabel lblCredits;
        internal Microsoft.Office.Tools.Ribbon.RibbonSeparator separator4;
        internal Microsoft.Office.Tools.Ribbon.RibbonSeparator separator2;
        internal Microsoft.Office.Tools.Ribbon.RibbonBox box1;
    }

    partial class ThisRibbonCollection
    {
        public wGengoRibbon wGengoRibbon
        {
            get { return this.GetRibbon<wGengoRibbon>(); }
        }

        public wGengoRibbon GetRibbon<T1>()
        {
            throw new System.NotImplementedException();
        }
    }
}
