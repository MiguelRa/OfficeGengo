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
using Microsoft.Office.Tools.Ribbon;
using System.Windows.Forms;
using MyGengoTranslator.UIForms;

namespace MyGengoWordAddIn
{

    public partial class wGengoRibbon
    {
        private MyGengoTranslator.MyGengoTranslatorClass _myGengoTranslator = null;
        private string _specificFolderName = "wordGengo";
        private string _productName = "WordGengo";
        private string _containerAppName = "Microsoft Word";

        private void wGengoRibbon_Load(object sender, RibbonUIEventArgs e)
        {
            btnStart.Click += new RibbonControlEventHandler(btnStart_Click);
            btnOrder.Click += new RibbonControlEventHandler(btnOrder_Click);
            btnOverview.Click += new RibbonControlEventHandler(btnOverview_Click);
        }        

        public void ShowTranslationForm(int activeTab)
        {
            if (_myGengoTranslator == null)
            {
                _myGengoTranslator = new MyGengoTranslator.MyGengoTranslatorClass(_specificFolderName, _productName, _containerAppName);
                
                // set icon
                _myGengoTranslator.Icon = (System.Drawing.Icon)global::MyGengoWordAddIn.Properties.Resources.icon;

                // set logo image
                _myGengoTranslator.LogoImage = global::MyGengoWordAddIn.Properties.Resources.logo;

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

        #region handle MyGengoTranslator events

        void myGengoTranslator_CreditsChanged(string newCredits)
        {
            lblCredits.Label = newCredits;

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

        #region handle ribbon events

        void btnOverview_Click(object sender, RibbonControlEventArgs e)
        {
            ShowTranslationForm(2);
        }

        void btnStart_Click(object sender, RibbonControlEventArgs e)
        {
            ShowTranslationForm(0);
        }

        void btnOrder_Click(object sender, RibbonControlEventArgs e)
        {
            ShowTranslationForm(1);
        }

        #endregion

        private void wGengoRibbon_Close(object sender, EventArgs e)
        {
            if (_myGengoTranslator != null)
            {
                _myGengoTranslator.Close();               
                
            }
        }
    }
    
}
