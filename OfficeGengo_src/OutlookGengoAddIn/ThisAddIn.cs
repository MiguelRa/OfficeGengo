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
using System.Xml.Linq;
using Outlook = Microsoft.Office.Interop.Outlook;
using Office = Microsoft.Office.Core;
using System.Windows.Forms;

namespace MyOutlookAddin
{
    public partial class ThisAddIn
    {
        #region properties

        // fields for custom task pane
        private UCTranslationPanel ucTranslationPanel;
        private Microsoft.Office.Tools.CustomTaskPane tpTranslationPanel;
        private string _taskPaneTitle = "OutlookGengo";
        private string _viewMenuItemText = "OutlookGengo panel";

        // menu item that shows/hide the task pane
        private Office.CommandBarButton miToogleTranslationPanel;

        // constants
        private int widthTaskPaneDocked = 300;
        private int widthTaskPaneFloating = 300;
        private int heightTaskPaneFloating = 500;

        // property that exposes myCustomTaskPane
        public Microsoft.Office.Tools.CustomTaskPane TranslationPanel
        {
            get
            {
                return tpTranslationPanel;
            }
        }

        #endregion

        #region addin event handlers

        // addin startup event handler. Here we initialize and show the task pane.
        private void ThisAddIn_Startup(object sender, System.EventArgs e)
        {
            // create and show myCustomTaskPane
            ucTranslationPanel = new UCTranslationPanel();
            tpTranslationPanel = this.CustomTaskPanes.Add(ucTranslationPanel, _taskPaneTitle);
            tpTranslationPanel.DockPosition = Office.MsoCTPDockPosition.msoCTPDockPositionRight;
            tpTranslationPanel.DockPositionRestrict = Office.MsoCTPDockPositionRestrict.msoCTPDockPositionRestrictNoHorizontal;
            tpTranslationPanel.Width = widthTaskPaneDocked;
            tpTranslationPanel.Visible = true;
            tpTranslationPanel.VisibleChanged += new EventHandler(taskPaneValue_VisibleChanged);
            tpTranslationPanel.DockPositionChanged += new EventHandler(myCustomTaskPane_DockPositionChanged);            

            // add menu item to View menu
            // * get View Menu
            string viewMenuID = "30004";
            Office.CommandBarPopup viewMenu = (Office.CommandBarPopup)this.Application.ActiveExplorer().
                                                CommandBars.ActiveMenuBar.FindControl(Office.MsoControlType.msoControlPopup,
                                                                viewMenuID, Type.Missing, true, true);

            // * add menu item
            miToogleTranslationPanel = (Office.CommandBarButton)viewMenu.Controls.Add(Office.MsoControlType.msoControlButton, missing,
                                                                            missing, 1, true);
            //miToogleTranslationPanel.Style = Office.MsoButtonStyle.msoButtonIconAndCaption;
            miToogleTranslationPanel.Caption = _viewMenuItemText;
            miToogleTranslationPanel.FaceId = 65;
            miToogleTranslationPanel.Tag = "c123";
            miToogleTranslationPanel.State = Office.MsoButtonState.msoButtonDown;
            miToogleTranslationPanel.Click += new Office._CommandBarButtonEvents_ClickEventHandler(viewTaskPane_Click);

        }

        private void ThisAddIn_Shutdown(object sender, System.EventArgs e)
        {
            if (ucTranslationPanel != null)
            {
                ucTranslationPanel.Close();
            }           
        }

        #endregion

        #region controls event handlers

        // event handler for taskpane's VisibleChanged event. Here we update the associated menu item state.
        private void taskPaneValue_VisibleChanged(object sender, System.EventArgs e)
        {
            if (tpTranslationPanel.Visible)
            {
                //MessageBox.Show("reset position of task pane");
                //miToogleTranslationPanel.Style = Office.MsoButtonStyle.msoButtonIconAndCaption;
                miToogleTranslationPanel.State = Office.MsoButtonState.msoButtonDown;
                Microsoft.Office.Tools.CustomTaskPane taskPane = sender as Microsoft.Office.Tools.CustomTaskPane;
                taskPane.DockPosition = Office.MsoCTPDockPosition.msoCTPDockPositionRight;
                taskPane.Width = widthTaskPaneDocked;

            }
            else
                //miToogleTranslationPanel.Style = Office.MsoButtonStyle.msoButtonCaption;
                miToogleTranslationPanel.State = Office.MsoButtonState.msoButtonUp;

        }

        // event handler for miViewTaskPane's click event. Here we show/hide the Translation panel.
        void viewTaskPane_Click(Office.CommandBarButton Ctrl, ref bool CancelDefault)
        {
            tpTranslationPanel.Visible = !tpTranslationPanel.Visible;
        }

        // event handler for task pane's dockPositionChanged event. 
        //Here we resize the task pane depending on its dock position.
        void myCustomTaskPane_DockPositionChanged(object sender, EventArgs e)
        {
            Microsoft.Office.Tools.CustomTaskPane taskPane = sender as Microsoft.Office.Tools.CustomTaskPane;

            if (taskPane != null)
            {
                if (taskPane.DockPosition == Office.MsoCTPDockPosition.msoCTPDockPositionFloating)
                {
                    //MessageBox.Show("Floating");
                    taskPane.Height = heightTaskPaneFloating;
                    taskPane.Width = widthTaskPaneFloating;
                }
                else if (taskPane.DockPosition == Office.MsoCTPDockPosition.msoCTPDockPositionLeft ||
                    taskPane.DockPosition == Office.MsoCTPDockPosition.msoCTPDockPositionRight)
                {
                    taskPane.Width = widthTaskPaneDocked;
                }
            }

        }

        #endregion

        #region VSTO generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InternalStartup()
        {
            this.Startup += new System.EventHandler(ThisAddIn_Startup);
            this.Shutdown += new System.EventHandler(ThisAddIn_Shutdown);
        }

        #endregion        

    }
}
