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
using MyGengoTranslator.UIForms;

namespace MyOutlookAddin
{
    public partial class UCTranslationPanel : UserControl
    {
        private MyGengoTranslator.MyGengoTranslatorClass _myGengoTranslator = null;
        private string _specificFolderName = "outlookGengo";
        private string _productName = "OutlookGengo";
        private string _containerAppName = "Microsoft Outlook";

        public UCTranslationPanel()
        {
            InitializeComponent();
            
            btnStart.Click += new System.EventHandler(btnStart_Click);
            btnOrder.Click += new System.EventHandler(btnOrder_Click);
            btnOverview.Click += new System.EventHandler(btnOverview_Click);
        }

        public void ShowTranslationForm(int activeTab)
        {
            if (_myGengoTranslator == null)
            {
                _myGengoTranslator = new MyGengoTranslator.MyGengoTranslatorClass(_specificFolderName, _productName, _containerAppName);

                // set icon
                _myGengoTranslator.Icon = (System.Drawing.Icon)global::MyOutlookAddin.Properties.Resources.icon;

                // set logo image
                _myGengoTranslator.LogoImage = global::MyOutlookAddin.Properties.Resources.logo;

                // set event handlers                
                _myGengoTranslator.OrderTabVisibleChanged +=
                    new TabVisibleChangedEventHandler(myGengoTranslator_OrderTabVisibleChanged);
                _myGengoTranslator.OverviewTabVisibleChanged +=
                    new TabVisibleChangedEventHandler(myGengoTranslator_OverviewTabVisibleChanged);
                _myGengoTranslator.CreditsChanged +=
                    new CreditsChangedEventHandler(myGengoTranslator_CreditsChanged);
            }

            _myGengoTranslator.Show(activeTab);
        }

        public void Close()
        {
            if (_myGengoTranslator != null)
            {
                _myGengoTranslator.Close();

            }
        }

        #region handle task pane events

        void btnOverview_Click(object sender, EventArgs e)
        {
            ShowTranslationForm(2);
        }

        void btnStart_Click(object sender, EventArgs e)
        {
            ShowTranslationForm(0);
        }

        void btnOrder_Click(object sender, EventArgs e)
        {
            ShowTranslationForm(1);
        }

        #endregion

        #region handle MyGengoTranslator events

        void myGengoTranslator_CreditsChanged(string newCredits)
        {
            lblCredits.Text = newCredits;

            if (newCredits.Equals("?"))
            {
                lblCredits.Visible = false;
                lblYourCredits.Visible = false;
            }
            else
            {
                lblCredits.Visible = true;
                lblYourCredits.Visible = true;
            }
        }

        public void myGengoTranslator_OverviewTabVisibleChanged(object sender, bool isVisible)
        {
            btnOverview.Enabled = isVisible;
        }

        public void myGengoTranslator_OrderTabVisibleChanged(object sender, bool isVisible)
        {
            btnOrder.Enabled = isVisible;
        }

        #endregion
        
    }
}
