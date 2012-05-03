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
using MyGengoTranslator.UIForms;
using System.Drawing;

namespace MyGengoTranslator
{
    public class MyGengoTranslatorClass
    {
        private FormGengoTranslator _formGengoTranslator = null;
        private string _specificFolderName = string.Empty;
        private string _appName = string.Empty;
        private string _containerName = string.Empty;

        private FormGengoTranslator GetFormGengoTranslator()
        {
            if (_formGengoTranslator == null)
            {
                _formGengoTranslator = new FormGengoTranslator(_specificFolderName, _appName, _containerName);
                _formGengoTranslator.OrderTabVisibleChanged +=
                    new TabVisibleChangedEventHandler(frmGengoTranslator_OrderTabVisibleChanged);
                _formGengoTranslator.OverviewTabVisibleChanged +=
                    new TabVisibleChangedEventHandler(frmGengoTranslator_OverviewTabVisibleChanged);
                _formGengoTranslator.CreditsChanged +=
                    new CreditsChangedEventHandler(frmGengoTranslator_CreditsChanged);
            }

            return _formGengoTranslator;
        }

        #region constructors

        public MyGengoTranslatorClass(string specificFolderName, string appName, string containerName)
        {
            _specificFolderName = specificFolderName;
            _appName = appName;
            _containerName = containerName;
        }

        #endregion

        #region properties

        public Icon Icon
        {
            get
            {
                return GetFormGengoTranslator().Icon;
            }
            set
            {
                GetFormGengoTranslator().Icon = value;
            }
        }

        public Image LogoImage
        {
            get
            {
                return GetFormGengoTranslator().LogoImage;
            }
            set
            {
                GetFormGengoTranslator().LogoImage = value;
            }
        }

        #endregion

        #region public methods

        public void Show(int activeTab)
        {
            GetFormGengoTranslator().ShowDialog(activeTab);
        }

        public void Close()
        {
            if (_formGengoTranslator != null)
            {
                _formGengoTranslator.DisposeResources();
                _formGengoTranslator.Dispose();
            }
        }

        #endregion

        #region public events

        public event TabVisibleChangedEventHandler OrderTabVisibleChanged;
        public event TabVisibleChangedEventHandler OverviewTabVisibleChanged;
        public event CreditsChangedEventHandler CreditsChanged;

        #endregion

        #region handlers formTranslation events

        void frmGengoTranslator_CreditsChanged(string newCredits)
        {
            CreditsChanged(newCredits);
        }

        public void frmGengoTranslator_OverviewTabVisibleChanged(object sender, bool isVisible)
        {
            OverviewTabVisibleChanged(sender, isVisible);
        }

        public void frmGengoTranslator_OrderTabVisibleChanged(object sender, bool isVisible)
        {
            OrderTabVisibleChanged(sender, isVisible);
        }

        #endregion

    }
}
