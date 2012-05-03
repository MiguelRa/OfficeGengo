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
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.IO;
using System.Xml.Serialization;
using System.Text.RegularExpressions;
using MiguelRa.Util;
using System.Diagnostics;
using MyGengoTranslator.Classes;
using System.Configuration;

namespace MyGengoTranslator.UIForms
{
    public delegate void TabVisibleChangedEventHandler(object sender, bool isVisible);
    public delegate void CreditsChangedEventHandler(string newCredits);

    public partial class FormGengoTranslator : Form
    {
        #region Attributes

        private Color _btnTierSelectedColor = Color.LightBlue;
        private MyGengoClientWrapper _clientWrapper = null;
        private List<Language> _languageList = null;
        private List<LanguagePair> _languagePairs = null;
        private AppState _appState;
        //private string _appFullPath = string.Empty; //Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + Properties.Resources.AppDirectory;
        private List<LanguagePair> _selectedLanguagePairs = null;
        private Language _languageFrom = null;
        private float _unitPrice = -1;
        private bool _tierClicked = false;
        int _wordCount = 0;
        private string _selectedTier = string.Empty;
        private List<Job> _jobList = null;
        private Job _selectedJob = null;
        private string _selectedRating = string.Empty;
        private bool _captchaLoaded = false;
        private bool _refreshing = false;
        private ErrorInfo _errorInfo = null;
        private string _specificFolderName = string.Empty;

        #region Delegates for UI stuff

        private delegate void UpdateCredistsCallBack(float pCredits);
        private delegate void UpdateJobsCallBack(List<Job> pJobsList);
        private delegate void UpdateLanguagesCallBack(List<Language> pLanguageList);
        private delegate void ShowNotificationCallBack(string message, ENotificationType notificationType);
        private delegate void ShowTabPagesCallBack(List<TabPage> tabPages);
        private delegate void ConfigureUICallBack();
        private delegate void ShowJobDataTabPageViewCallBack(Job pJob);
        private delegate void ShowJobDataTabPageReviewCallBack(Job pJob);
        private delegate void HideLoadingCallBack();

        #endregion

        #region Public events

        public event TabVisibleChangedEventHandler OrderTabVisibleChanged;
        public event TabVisibleChangedEventHandler OverviewTabVisibleChanged;
        public event CreditsChangedEventHandler CreditsChanged;

        #endregion

        #endregion

        #region Properties

        public Image LogoImage
        {
            get
            {
                if (pictureBoxLogo != null)
                    return pictureBoxLogo.Image;
                else
                    return null;
            }
            set
            {
                if (pictureBoxLogo != null)
                    pictureBoxLogo.Image = value;
            }
        }

        #endregion

        #region Constructor

        public FormGengoTranslator(string specificFolderName, string appName, string containerName)
        {
            InitializeComponent();

            _clientWrapper = new MyGengoClientWrapper();
            _specificFolderName = specificFolderName;

            InitializingJobViewer();

            _appState = AppState.BeforeLogin;
            ConfigureUI();

            Session session = GetSession();
            if (session != null)
            {
                textBoxPrivateKey.Text = session.PrivateKey;
                textboxPublicKey.Text = session.PublicKey;
                LogIn();
            }

            this.Text = string.Format(this.Text, appName, containerName);


        }

        #endregion

        #region Methods

        #region UIBehavior

        private void ResetTierButtons()
        {
            btnTierStandard.BackColor = Color.Transparent;
            btnTierProfessional.BackColor = Color.Transparent;
            btnTierUltra.BackColor = Color.Transparent;
            btnTierMachine.BackColor = Color.Transparent;
        }

        private void ResetRatingButtons()
        {
            btnRating1.BackColor = Color.Transparent;
            btnRating2.BackColor = Color.Transparent;
            btnRating3.BackColor = Color.Transparent;
            btnRating4.BackColor = Color.Transparent;
            btnRating5.BackColor = Color.Transparent;
        }

        private void ShowLoading()
        {
            pbxLoadingStart.Visible = true;
            pbxLoadingOrder.Visible = true;
            pbxLoadingOverview.Visible = true;
            pbxLoadingView.Visible = true;
            pbxLoadingReview.Visible = true;
            pbxLoadingReject.Visible = true;
            pbxLoadingCorrect.Visible = true;

            HideNotification();
        }

        private void HideLoading()
        {
            if (pbxLoadingStart.InvokeRequired)
            {
                HideLoadingCallBack d = new HideLoadingCallBack(HideLoading);
                this.Invoke(d, null);
            }
            else
            {
                pbxLoadingStart.Visible = false;
                pbxLoadingOrder.Visible = false;
                pbxLoadingOverview.Visible = false;
                pbxLoadingView.Visible = false;
                pbxLoadingReview.Visible = false;
                pbxLoadingReject.Visible = false;
                pbxLoadingCorrect.Visible = false;
            }
        }

        private void ShowNotification(string message, ENotificationType notificationType)
        {
            if (lblNotificationStart.InvokeRequired)
            {
                ShowNotificationCallBack d = new ShowNotificationCallBack(ShowNotification);
                this.Invoke(d, new object[] { message, notificationType });
            }
            else
            {
                // hide loading
                HideLoading();

                // show success message only if it is activated in configuration.
                bool showSuccessMessage = Convert.ToBoolean(ConfigurationManager.AppSettings["wGengo_showSuccessMsg"]);
                if (notificationType == ENotificationType.Success && !showSuccessMessage)
                    return;

                lblNotificationStart.Text = message;
                lblNotificationOrder.Text = message;
                lblNotificationOverview.Text = message;
                lblNotificationView.Text = message;
                lblNotificationReview.Text = message;
                lblNotificationCorrect.Text = message;
                lblNotificationReject.Text = message;

                Color color = (notificationType == ENotificationType.Success) ? System.Drawing.Color.Green : System.Drawing.Color.Red;
                lblNotificationStart.ForeColor = color;
                lblNotificationOrder.ForeColor = color;
                lblNotificationOverview.ForeColor = color;
                lblNotificationView.ForeColor = color;
                lblNotificationReview.ForeColor = color;
                lblNotificationCorrect.ForeColor = color;
                lblNotificationReject.ForeColor = color;

                lblNotificationStart.Visible = true;
                lblNotificationOrder.Visible = true;
                lblNotificationOverview.Visible = true;
                lblNotificationView.Visible = true;
                lblNotificationReview.Visible = true;
                lblNotificationCorrect.Visible = true;
                lblNotificationReject.Visible = true;
            }

        }

        private void HideNotification()
        {
            lblNotificationStart.Visible = false;
            lblNotificationOrder.Visible = false;
            lblNotificationOverview.Visible = false;
            lblNotificationView.Visible = false;
            lblNotificationReview.Visible = false;
            lblNotificationCorrect.Visible = false;
            lblNotificationReject.Visible = false;
        }

        private void InitializingJobViewer()
        {
            MyJobDataProvider jobDataProvider = new MyJobDataProvider();

            // tab Overview: initilize jobsViewer control
            jobsViewer1.JobDataProvider = jobDataProvider;
            jobsViewer1.ViewClick += new JobsViewer.JobsViewer.JobViewerActionHandler(jobsViewer1_ViewClick);
            jobsViewer1.ReviewClick += new JobsViewer.JobsViewer.JobViewerActionHandler(jobsViewer1_ReviewClick);
            jobsViewer1.CancelClick += new JobsViewer.JobsViewer.JobViewerActionHandler(jobsViewer1_CancelClick);

            // tab View: initilize jobViewerSingle control
            jobViewerSingleView.JobDataProvider = jobDataProvider;

            // tab View: initilize jobViewerSingle control
            jobViewerSingleView.JobDataProvider = jobDataProvider;

            // tab Review: initilize jobViewerSingle control
            jobViewerSingleReview.JobDataProvider = jobDataProvider;

            // tab Reject: initilize jobViewerSingle control
            jobViewerSingleRejectJob.JobDataProvider = jobDataProvider;

            // tab RequestCorrection: initilize jobViewerSingle control
            jobViewerSingleCorrectJob.JobDataProvider = jobDataProvider;
        }

        private void ShowTabPages(List<TabPage> tabPages)
        {
            if (tabControlFuncionalities.InvokeRequired)
            {
                ShowTabPagesCallBack d = new ShowTabPagesCallBack(ShowTabPages);
                this.Invoke(d, new object[] { tabPages });
            }
            else
            {
                // if tabContron doesn't contain it already, add the tabPage.
                foreach (var tabPage in tabPages)
                {
                    if (!tabControlFuncionalities.TabPages.Contains(tabPage))
                    {
                        tabControlFuncionalities.TabPages.Add(tabPage);
                    }
                }
            }
        }

        private void HideTabPages()
        {
            tabControlFuncionalities.TabPages.Remove(tabPageOrder);
            tabControlFuncionalities.TabPages.Remove(tabPageOverview);
            tabControlFuncionalities.TabPages.Remove(tabPageView);
            tabControlFuncionalities.TabPages.Remove(tabPageReview);
            tabControlFuncionalities.TabPages.Remove(tabPageReject);
            tabControlFuncionalities.TabPages.Remove(tabPageRequestCorrection);
        }

        private void ConfigureUI()
        {
            if (tabControlFuncionalities.InvokeRequired)
            {
                ConfigureUICallBack d = new ConfigureUICallBack(ConfigureUI);
                this.Invoke(d, null);
            }
            else
            {
                switch (_appState)
                {
                    case AppState.BeforeLogin:
                        {
                            HideTabPages();
                            btnClear.Enabled = textboxPublicKey.Enabled = textBoxPrivateKey.Enabled = true;
                            textboxPublicKey.Text = textBoxPrivateKey.Text = string.Empty;
                            lblFieldEmptyPublicKey.Visible = lblFieldEmptyPrivateKey.Visible = false;
                            btnOrder.Enabled = _tierClicked = false;
                            _wordCount = 0;
                            btnLogIn.Text = Properties.Resources.LogIn;
                            lbCredits.Text = "?";
                            ShowCredits(false);
                            btnRefresh.Enabled = false;

                            break;
                        }
                    case AppState.Logging:
                        {
                            ShowLoading();
                            //Deshabiltando controles visuales
                            btnLogIn.Enabled = btnClear.Enabled = textboxPublicKey.Enabled = textBoxPrivateKey.Enabled = false;
                            break;
                        }
                    case AppState.AfterLogin:
                        {
                            ShowTabPages(new List<TabPage>() { tabPageOrder, tabPageOverview });
                            tabControlFuncionalities.SelectedTab = tabPageOverview;

                            btnLogIn.Enabled = true;
                            btnLogIn.Text = Properties.Resources.LogOf;
                            btnRefresh.Enabled = true;
                            break;

                        }
                    case AppState.ErrorLogin:
                        {
                            _appState = AppState.BeforeLogin;
                            ConfigureUI();

                            break;
                        }
                    case AppState.Ordering:
                        {
                            #region Deshabiltando controles visuales

                            //Tab start
                            btnLogIn.Enabled = false;

                            //Tab Order
                            panelOrder1.Enabled = panelOrder2.Enabled = false;

                            //Tab Overview
                            jobsViewer1.Enabled = false;

                            //Tab View
                            panelView.Enabled = false;

                            //Tab Review
                            panelReview.Enabled = false;

                            //Tab Reject
                            panelReject.Enabled = false;

                            //Tab Correct
                            panelCorrect.Enabled = false;

                            #endregion

                            HideNotification();
                            ShowLoading();
                            break;
                        }
                    case AppState.AfterOrder:
                        {
                            #region Habiltando controles visuales

                            //Tab start
                            btnLogIn.Enabled = true;

                            //Tab Order
                            panelOrder1.Enabled = panelOrder2.Enabled = true;

                            //Tab Overview
                            jobsViewer1.Enabled = true;

                            //Tab View
                            panelView.Enabled = true;

                            //Tab Review
                            panelReview.Enabled = true;

                            //Tab Reject
                            panelReject.Enabled = true;

                            //Tab Correct
                            panelCorrect.Enabled = true;

                            //Tab Order
                            panelOrder1.Enabled = panelOrder2.Enabled = true;

                            #endregion

                            ShowNotification(Properties.Resources.TEXT_INFO_ORDER_OK, ENotificationType.Success);
                            ClearJobTab();
                            break;
                        }
                    case AppState.ErrorOrdering:
                        {
                            #region Habiltando controles visuales

                            //Tab start
                            btnLogIn.Enabled = true;

                            //Tab Order
                            panelOrder1.Enabled = panelOrder2.Enabled = true;

                            //Tab Overview
                            jobsViewer1.Enabled = true;

                            //Tab View
                            panelView.Enabled = true;

                            //Tab Review
                            panelReview.Enabled = true;

                            //Tab Reject
                            panelReject.Enabled = true;

                            //Tab Correct
                            panelCorrect.Enabled = true;

                            //Tab Order
                            panelOrder1.Enabled = panelOrder2.Enabled = true;

                            #endregion

                            ClearJobTab();

                            break;
                        }
                    case AppState.Canceling:
                        {
                            #region Deshabiltando controles visuales

                            //Tab start
                            btnLogIn.Enabled = false;

                            //Tab Order
                            panelOrder1.Enabled = panelOrder2.Enabled = false;

                            //Tab Overview
                            jobsViewer1.Enabled = false;

                            //Tab View
                            panelView.Enabled = false;

                            //Tab Review
                            panelReview.Enabled = false;

                            //Tab Reject
                            panelReject.Enabled = false;

                            //Tab Correct
                            panelCorrect.Enabled = false;

                            #endregion

                            HideNotification();
                            ShowLoading();
                            break;
                        }
                    case AppState.AfterCancel:
                        {
                            #region Habiltando controles visuales

                            //Tab start
                            btnLogIn.Enabled = true;

                            //Tab Order
                            panelOrder1.Enabled = panelOrder2.Enabled = true;

                            //Tab Overview
                            jobsViewer1.Enabled = true;

                            //Tab View
                            panelView.Enabled = true;

                            //Tab Review
                            panelReview.Enabled = true;

                            //Tab Reject
                            panelReject.Enabled = true;

                            //Tab Correct
                            panelCorrect.Enabled = true;

                            #endregion

                            ShowNotification(Properties.Resources.TEXT_INFO_CANCEL_OK, ENotificationType.Success);

                            //
                            UpdateJobState();

                            _selectedJob = null;

                            break;
                        }
                    case AppState.ErrorCanceling:
                        {
                            #region Habiltando controles visuales

                            //Tab start
                            btnLogIn.Enabled = true;

                            //Tab Order
                            panelOrder1.Enabled = panelOrder2.Enabled = true;

                            //Tab Overview
                            jobsViewer1.Enabled = true;

                            //Tab View
                            panelView.Enabled = true;

                            //Tab Review
                            panelReview.Enabled = true;

                            //Tab Reject
                            panelReject.Enabled = true;

                            //Tab Correct
                            panelCorrect.Enabled = true;

                            #endregion

                            _selectedJob = null;

                            break;
                        }
                    case AppState.Viewing:
                        {
                            #region Deshabiltando controles visuales

                            //Tab start
                            btnLogIn.Enabled = false;

                            //Tab Order
                            panelOrder1.Enabled = panelOrder2.Enabled = false;

                            //Tab Overview
                            jobsViewer1.Enabled = false;

                            //Tab View
                            panelView.Enabled = false;

                            //Tab Review
                            tabControlFuncionalities.TabPages.Remove(tabPageReview);

                            //Tab Reject
                            tabControlFuncionalities.TabPages.Remove(tabPageReject);

                            //Tab Correct
                            tabControlFuncionalities.TabPages.Remove(tabPageRequestCorrection);

                            #endregion

                            HideNotification();
                            ShowLoading();
                            break;
                        }
                    case AppState.AfterView:
                        {
                            #region Habiltando controles visuales

                            //Tab start
                            btnLogIn.Enabled = true;

                            //Tab Order
                            panelOrder1.Enabled = panelOrder2.Enabled = true;

                            //Tab Overview
                            jobsViewer1.Enabled = true;

                            //Tab View
                            panelView.Enabled = true;
                            if (!tabControlFuncionalities.TabPages.Contains(tabPageView))
                                tabControlFuncionalities.TabPages.Add(tabPageView);

                            tabControlFuncionalities.SelectedTab = tabPageView;

                            #endregion

                            ShowNotification(Properties.Resources.TEXT_INFO_VIEW_OK, ENotificationType.Success);

                            break;
                        }
                    case AppState.ErrorViewing:
                        {
                            #region Habiltando controles visuales

                            //Tab start
                            btnLogIn.Enabled = true;

                            //Tab Order
                            panelOrder1.Enabled = panelOrder2.Enabled = true;

                            //Tab Overview
                            jobsViewer1.Enabled = true;

                            #endregion

                            break;
                        }
                    case AppState.Reviewing:
                        {
                            #region Deshabiltando controles visuales

                            //Tab start
                            btnLogIn.Enabled = false;

                            //Tab Order
                            panelOrder1.Enabled = panelOrder2.Enabled = false;

                            //Tab Overview
                            jobsViewer1.Enabled = false;

                            //Tab View
                            tabControlFuncionalities.TabPages.Remove(tabPageView);

                            //Tab Reject
                            tabControlFuncionalities.TabPages.Remove(tabPageReject);

                            //Tab Correct
                            tabControlFuncionalities.TabPages.Remove(tabPageRequestCorrection);

                            #endregion

                            HideNotification();
                            ShowLoading();
                            break;
                        }
                    case AppState.AfterReview:
                        {
                            #region Habiltando controles visuales

                            //Tab start
                            btnLogIn.Enabled = true;

                            //Tab Order
                            panelOrder1.Enabled = panelOrder2.Enabled = true;

                            //Tab Overview
                            jobsViewer1.Enabled = true;

                            //Tab View
                            panelView.Enabled = true;
                            if (!tabControlFuncionalities.TabPages.Contains(tabPageReview))
                                tabControlFuncionalities.TabPages.Add(tabPageReview);

                            //Tab review
                            btnRating1.Enabled = btnRating2.Enabled = btnRating3.Enabled = btnRating4.Enabled = btnRating5.Enabled =
                                btnShowRejectForm.Enabled = btnShowCorrectForm.Enabled = btnApprove.Enabled = true;
                            btnApprove.Text = "Approve";

                            tabControlFuncionalities.SelectedTab = tabPageReview;

                            #endregion

                            ShowNotification(Properties.Resources.TEXT_INFO_REVIEW_OK, ENotificationType.Success);

                            break;
                        }
                    case AppState.ErrorReviewing:
                        {
                            #region Habiltando controles visuales

                            //Tab start
                            btnLogIn.Enabled = true;

                            //Tab Order
                            panelOrder1.Enabled = panelOrder2.Enabled = true;

                            //Tab Overview
                            jobsViewer1.Enabled = true;

                            #endregion

                            break;
                        }
                    case AppState.Approving:
                        {
                            #region Deshabiltando controles visuales

                            //Tab start
                            btnLogIn.Enabled = false;

                            //Tab Order
                            panelOrder1.Enabled = panelOrder2.Enabled = false;

                            //Tab Overview
                            jobsViewer1.Enabled = false;

                            //panelReview
                            //panelReview.Enabled = false;
                            btnRating1.Enabled = btnRating2.Enabled = btnRating3.Enabled = btnRating4.Enabled = btnRating5.Enabled =
                                btnShowRejectForm.Enabled = btnShowCorrectForm.Enabled = btnApprove.Enabled = false;

                            //Tab Reject
                            tabControlFuncionalities.TabPages.Remove(tabPageReject);

                            //Tab Correct
                            tabControlFuncionalities.TabPages.Remove(tabPageRequestCorrection);

                            #endregion

                            HideNotification();
                            ShowLoading();
                            break;
                        }
                    case AppState.AfterApprove:
                        {
                            #region Habiltando controles visuales

                            //Tab start
                            btnLogIn.Enabled = true;

                            //Tab Order
                            panelOrder1.Enabled = panelOrder2.Enabled = true;

                            //Tab Overview
                            jobsViewer1.Enabled = true;

                            //panelReview
                            panelReview.Enabled = true;

                            //tab page Review
                            btnApprove.Enabled = true;
                            btnApprove.Text = "Done";

                            #endregion

                            ShowNotification(Properties.Resources.TEXT_INFO_APPROVED_OK, ENotificationType.Success);

                            btnShowRejectForm.Enabled = btnShowCorrectForm.Enabled = false;
                            _selectedRating = string.Empty;

                            UpdateJobState();

                            break;
                        }
                    case AppState.ErrorApproving:
                        {
                            #region Habiltando controles visuales

                            //Tab start
                            btnLogIn.Enabled = true;

                            //Tab Order
                            panelOrder1.Enabled = panelOrder2.Enabled = true;

                            //Tab Overview
                            jobsViewer1.Enabled = true;

                            //panelReview
                            panelReview.Enabled = true;

                            //tab page Review
                            btnRating1.Enabled = btnRating2.Enabled = btnRating3.Enabled = btnRating4.Enabled = btnRating5.Enabled =
                                btnShowRejectForm.Enabled = btnShowCorrectForm.Enabled = btnApprove.Enabled = true;
                            btnApprove.Text = "Approve";

                            #endregion

                            break;
                        }
                    case AppState.BeforeRequestCorrection:
                        {
                            #region Inicializando contoles visuales

                            // Eliminando tabPageReject
                            if (tabControlFuncionalities.TabPages.Contains(tabPageReject))
                                tabControlFuncionalities.TabPages.Remove(tabPageReject);

                            // Añadiendo tabPageRequestCorrection
                            if (!tabControlFuncionalities.TabPages.Contains(tabPageRequestCorrection))
                                tabControlFuncionalities.TabPages.Add(tabPageRequestCorrection);
                            tabControlFuncionalities.SelectedTab = tabPageRequestCorrection;

                            // Mostrando el Job en el job viewer
                            jobViewerSingleCorrectJob.LoadJob(_selectedJob);

                            lblFieldEmptyFormalCorrection.Visible = false;
                            textBoxCorrectionRequest.ReadOnly = false;
                            textBoxCorrectionRequest.Text = string.Empty;
                            btnRequestCorrection.Enabled = false;
                            btnRequestCorrection.Text = "Request correction";

                            #endregion

                            HideNotification();

                            break;
                        }
                    case AppState.RequestingCorrection:
                        {
                            #region Deshabiltando controles visuales

                            //Tab start
                            btnLogIn.Enabled = false;

                            //Tab Order
                            panelOrder1.Enabled = panelOrder2.Enabled = false;

                            //Tab Overview
                            jobsViewer1.Enabled = false;

                            //panelReview
                            //panelReview.Enabled = false;
                            btnRating1.Enabled = btnRating2.Enabled = btnRating3.Enabled = btnRating4.Enabled = btnRating5.Enabled =
                                btnShowRejectForm.Enabled = btnShowCorrectForm.Enabled = btnApprove.Enabled = false;

                            //Tab Correct
                            textBoxCorrectionRequest.ReadOnly = true;
                            btnRequestCorrection.Enabled = false;

                            #endregion

                            HideNotification();
                            ShowLoading();

                            break;
                        }
                    case AppState.AfterRequestCorrection:
                        {
                            #region Habilitando controles visuales

                            //Tab start
                            btnLogIn.Enabled = true;

                            //Tab Order
                            panelOrder1.Enabled = panelOrder2.Enabled = true;

                            //Tab Overview
                            jobsViewer1.Enabled = true;

                            //panelReview
                            //panelReview.Enabled = false;
                            //btnRating1.Enabled = btnRating2.Enabled = btnRating3.Enabled = btnRating4.Enabled = btnRating5.Enabled = true;

                            btnRequestCorrection.Enabled = true;
                            btnRequestCorrection.Text = "Done";

                            btnApprove.Enabled = true;
                            btnApprove.Text = "Done";

                            #endregion

                            ShowNotification(Properties.Resources.TEXT_INFO_REQCORRECTION_OK, ENotificationType.Success);

                            //Actualizndo el estado del job en el list view
                            jobViewerSingleCorrectJob.LoadJob(_selectedJob);
                            jobViewerSingleReview.LoadJob(_selectedJob);
                            UpdateJobState();

                            break;
                        }
                    case AppState.ErrorRequestingCorrection:
                        {

                            #region Inicializando contoles visuales

                            textBoxCorrectionRequest.ReadOnly = false;
                            btnRequestCorrection.Enabled = true;

                            #endregion

                            #region Habilitando controles visuales

                            //Tab start
                            btnLogIn.Enabled = true;

                            //Tab Order
                            panelOrder1.Enabled = panelOrder2.Enabled = true;

                            //Tab Overview
                            jobsViewer1.Enabled = true;

                            btnRating1.Enabled = btnRating2.Enabled = btnRating3.Enabled = btnRating4.Enabled = btnRating5.Enabled =
                               btnShowRejectForm.Enabled = btnShowCorrectForm.Enabled = btnApprove.Enabled = true;

                            #endregion

                            break;
                        }
                    case AppState.BeforeReject:
                        {
                            #region Inicializando controles visuales

                            pictureBoxLoadingCaptcha.Visible = lbLoadingCaptcha.Visible = false;

                            // Eliminando tabPageRequestCorrection
                            if (tabControlFuncionalities.TabPages.Contains(tabPageRequestCorrection))
                                tabControlFuncionalities.TabPages.Remove(tabPageRequestCorrection);

                            //Incializando el captcha

                            UpdateCaptchaImage();

                            // Añadiendo tabPageRequestCorrection
                            if (!tabControlFuncionalities.TabPages.Contains(tabPageReject))
                                tabControlFuncionalities.TabPages.Add(tabPageReject);
                            tabControlFuncionalities.SelectedTab = tabPageReject;

                            // Mostrando el Job en el job viewer
                            jobViewerSingleRejectJob.LoadJob(_selectedJob);

                            lblFieldEmptyFeedback.Visible = false;
                            textBoxFeedBack.ReadOnly = false;
                            textBoxFeedBack.Text = string.Empty;

                            lblFieldEmptyCaptcha.Visible = false;
                            textBoxCaptcha.ReadOnly = false;
                            textBoxCaptcha.Text = string.Empty;
                            textBoxCaptcha.Enabled = false;

                            panelFollowUp.Enabled = true;
                            panelReason.Enabled = true;
                            btnReject.Enabled = false;
                            btnReject.Text = "Reject";

                            _captchaLoaded = false;

                            radioBtnFollowUpAnotherTranslator.Checked = radioBtnReasonQuality.Checked = true;

                            #endregion

                            ShowLoading();
                            HideNotification();

                            break;
                        }
                    case AppState.Rejecting:
                        {
                            #region Deshabiltando controles visuales

                            //Tab start
                            btnLogIn.Enabled = false;

                            //Tab Order
                            panelOrder1.Enabled = panelOrder2.Enabled = false;

                            //Tab Overview
                            jobsViewer1.Enabled = false;

                            //panelReview
                            //panelReview.Enabled = false;
                            btnRating1.Enabled = btnRating2.Enabled = btnRating3.Enabled = btnRating4.Enabled = btnRating5.Enabled =
                                btnShowRejectForm.Enabled = btnShowCorrectForm.Enabled = btnApprove.Enabled = false;

                            //Tab Reject
                            textBoxFeedBack.ReadOnly = true;
                            textBoxCaptcha.ReadOnly = true;

                            panelFollowUp.Enabled = false;
                            panelReason.Enabled = false;
                            btnReject.Enabled = false;

                            #endregion

                            HideNotification();
                            ShowLoading();

                            break;
                        }
                    case AppState.AfterReject:
                        {
                            #region Habilitando controles visuales

                            //Tab start
                            btnLogIn.Enabled = true;

                            //Tab Order
                            panelOrder1.Enabled = panelOrder2.Enabled = true;

                            //Tab Overview
                            jobsViewer1.Enabled = true;

                            //panelReview
                            //panelReview.Enabled = false;
                            //btnRating1.Enabled = btnRating2.Enabled = btnRating3.Enabled = btnRating4.Enabled = btnRating5.Enabled = true;

                            btnReject.Enabled = true;
                            btnReject.Text = "Done";

                            btnApprove.Enabled = true;
                            btnApprove.Text = "Done";

                            #endregion

                            ShowNotification(Properties.Resources.TEXT_INFO_REJECT_OK, ENotificationType.Success);

                            //Actualizndo el estado del job en el list view
                            jobViewerSingleRejectJob.LoadJob(_selectedJob);
                            jobViewerSingleReview.LoadJob(_selectedJob);
                            UpdateJobState();

                            break;
                        }
                    case AppState.ErrorRejecting:
                        {
                            if (_errorInfo.ErrorCode == EErrorCode.REJECT_WRONG_CAPTCHA)
                            {
                                pictureBoxCaptcha.ErrorImage = null;
                                pictureBoxCaptcha.InitialImage = null;
                                pictureBoxCaptcha.Image = null;
                                pictureBoxCaptcha.ImageLocation = string.Empty;
                                
                                pictureBoxCaptcha.Refresh();
                                pictureBoxLoadingCaptcha.Visible = lbLoadingCaptcha.Visible = true;
                                textBoxCaptcha.Text = string.Empty;

                                _clientWrapper.ViewJob(ViewJobWrongCaptcha_CallBack, Generic_ErrorCallBack, _selectedJob.Job_Id, string.Empty);                            
                                return;
                            }

                            #region Inicializando controles visuales

                            textBoxFeedBack.ReadOnly = false;

                            textBoxCaptcha.ReadOnly = false;

                            panelFollowUp.Enabled = true;
                            panelReason.Enabled = true;
                            btnReject.Enabled = true;
                            btnReject.Text = "Reject";

                            #endregion

                            #region Habilitando controles visuales

                            //Tab start
                            btnLogIn.Enabled = true;

                            //Tab Order
                            panelOrder1.Enabled = panelOrder2.Enabled = true;

                            //Tab Overview
                            jobsViewer1.Enabled = true;

                            btnRating1.Enabled = btnRating2.Enabled = btnRating3.Enabled = btnRating4.Enabled = btnRating5.Enabled =
                              btnShowRejectForm.Enabled = btnShowCorrectForm.Enabled = btnApprove.Enabled = true;

                            #endregion

                            break;
                        }
                    default:
                        break;
                }
            }
        }

        private delegate void ViewJobWrongCaptcha_CallBackCallBack(Job pJob);

        private void ViewJobWrongCaptcha_CallBack(Job pJob)
        {
            if (textBoxFeedBack.InvokeRequired)
            {
                ViewJobWrongCaptcha_CallBackCallBack d = new ViewJobWrongCaptcha_CallBackCallBack(ViewJobWrongCaptcha_CallBack);
                this.Invoke(d, pJob);
            }
            else
            {
                _selectedJob = pJob;

                //Incializando el captcha
                UpdateCaptchaImage();

                jobViewerSingleRejectJob.LoadJob(_selectedJob);
                jobViewerSingleReview.LoadJob(_selectedJob);

                UpdateJobState();

                #region Inicializando controles visuales

                textBoxFeedBack.ReadOnly = false;

                textBoxCaptcha.ReadOnly = false;

                panelFollowUp.Enabled = true;
                panelReason.Enabled = true;
                btnReject.Enabled = true;
                btnReject.Text = "Reject";

                #endregion

                #region Habilitando controles visuales

                //Tab start
                btnLogIn.Enabled = true;

                //Tab Order
                panelOrder1.Enabled = panelOrder2.Enabled = true;

                //Tab Overview
                jobsViewer1.Enabled = true;

                btnRating1.Enabled = btnRating2.Enabled = btnRating3.Enabled = btnRating4.Enabled = btnRating5.Enabled =
                  btnShowRejectForm.Enabled = btnShowCorrectForm.Enabled = btnApprove.Enabled = true;

                #endregion
            }
        }

        private delegate void UpdateCaptchaImageCallBack();

        private void UpdateCaptchaImage()
        {
            if (panelPictureBox.InvokeRequired)
            {
                UpdateCaptchaImageCallBack d = new UpdateCaptchaImageCallBack(UpdateCaptchaImage);
                this.Invoke(d, null);
            }
            else
            {
                panelPictureBox.Controls.Clear();

                PictureBox myPictureBoxCaptcha = new PictureBox();

                myPictureBoxCaptcha.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
                myPictureBoxCaptcha.Dock = System.Windows.Forms.DockStyle.Fill;
                pictureBoxLoadingCaptcha.Visible = true;
                lbLoadingCaptcha.Visible = true;
                //myPictureBoxCaptcha.InitialImage = global::MyGengoTranslator.Properties.Resources.loading_balls;
                myPictureBoxCaptcha.Location = new System.Drawing.Point(0, 0);
                myPictureBoxCaptcha.ErrorImage = null;
                myPictureBoxCaptcha.InitialImage = null;
                myPictureBoxCaptcha.Margin = new System.Windows.Forms.Padding(3, 3, 3, 10);
                myPictureBoxCaptcha.Name = "pictureBoxCaptcha";
                myPictureBoxCaptcha.Size = new System.Drawing.Size(120, 40);
                myPictureBoxCaptcha.TabIndex = 71;
                myPictureBoxCaptcha.TabStop = false;
                myPictureBoxCaptcha.LoadCompleted += new System.ComponentModel.AsyncCompletedEventHandler(this.pictureBoxCaptcha_LoadCompleted);
                panelPictureBox.Controls.Add(myPictureBoxCaptcha);

                myPictureBoxCaptcha.ImageLocation = _selectedJob.CaptchaURL;
                textBoxCaptcha.Focus();
            }
        }
        
        private void SetOrderPrice()
        {
            float price = 0;
            string priceStrFormatted = string.Empty;

            if (_unitPrice == -1)
            {
                priceStrFormatted = Utils.ConvertToCurrencyString(price.ToString());
                labelAmmount.Text = "$" + priceStrFormatted;
            }
            else
            {
                price = _wordCount * _unitPrice;
                priceStrFormatted = Utils.ConvertToCurrencyString(price.ToString());
                labelAmmount.Text = "$" + priceStrFormatted;
            }

            labelAmmount.Tag = price;
        }

        private void ClearJobTab()
        {
            textBoxTexToTranslate.Text = textBoxJobTitle.Text = textBoxComment.Text = string.Empty;
            comboBoxLanguageFrom.SelectedIndex = 0;
        }

        private void UpdateJobState()
        {
            jobsViewer1.UpdateJob(_selectedJob);
        }

        private void ShowCredits(bool show)
        {
            if (show)
            {
                lbCredits.Visible = true;
                lbIndicaCredits.Visible = true;
            }
            else
            {
                lbCredits.Visible = false;
                lbIndicaCredits.Visible = false;
            }
        }

        #endregion

        #region CallBacks

        public void Generic_ErrorCallBack(ErrorInfo pErrorInfo)
        {
            _errorInfo = pErrorInfo;
            
            string errorMessage;
            if (pErrorInfo.ErrorType == ErrorType.ApiError)
            {
                
                if (pErrorInfo.ErrorCode == EErrorCode.REJECT_WRONG_CAPTCHA)
                    errorMessage = string.Format("{0}: {1}", "API Error", "Invalid captcha. Please try again.");
                else
                    errorMessage = string.Format("{0}: {1}", "API Error", pErrorInfo.ErrorMessage);
            }
            else
                errorMessage = string.Format("{0}: {1}", "App Error", pErrorInfo.ErrorMessage);           

            ShowNotification(errorMessage, ENotificationType.Error);
            _refreshing = false;
            GoErrorState();
        }

        public void GetAccountBalance_SuccessCallback(float accountBalance)
        {
            UpdateCredists(accountBalance);
            if (_clientWrapper == null)
                return;

            _clientWrapper.GetMyJobs(GetMyJobs_SuccessCallback, Generic_ErrorCallBack,
                string.Empty, string.Empty, string.Empty, false);
        }

        public void GetMyJobs_SuccessCallback(List<Job> jobList)
        {
            _jobList = jobList;
            //UpdateJobs(jobList);

            if (_clientWrapper == null)
                return;

            _clientWrapper.GetLanguagePairs(GetLanguagePairs_SuccessCallback, Generic_ErrorCallBack);
        }

        public void GetLanguages_SuccessCallback(List<Language> languageList)
        {
            _languageList = languageList;
            Utils.LanguageList = _languageList;
            UpdateLanguages(languageList);

            if (_refreshing)
                ShowNotification(Properties.Resources.TEXT_INFO_REFRESH_OK, ENotificationType.Success);
            else
                ShowNotification(Properties.Resources.LoginSuccess, ENotificationType.Success);

            _refreshing = false;

            SaveSession();

            UpdateJobs(_jobList);

            _appState = AppState.AfterLogin;
            ConfigureUI();
        }

        public void GetLanguagePairs_SuccessCallback(List<LanguagePair> languagePairs)
        {
            _languagePairs = languagePairs;
            if (_clientWrapper == null)
                return;

            _clientWrapper.GetLanguages(GetLanguages_SuccessCallback, Generic_ErrorCallBack);
        }

        private void TransalteJob_SuccessCallback(Job pJob)
        {
            if (_jobList == null)
                _jobList = new List<Job>();
            _jobList.Insert(0, pJob);

            _clientWrapper.GetAccountBalance(GetAccountBalanceOrdered_SuccessCallback, Generic_ErrorCallBack);
        }

        private void CancelJob_SuccessCallback(bool pJobCanceled)
        {
            _clientWrapper.GetAccountBalance(GetAccountBalanceJobCanceled_SuccessCallback, Generic_ErrorCallBack);
        }

        private void GetAccountBalanceJobCanceled_SuccessCallback(float accountBalance)
        {
            UpdateCredists(accountBalance);

            _appState = AppState.AfterCancel;
            _selectedJob.Status = EJobStatus.JOB_STATUS_CANCELLED;

            ConfigureUI();
        }

        private void ViewJob_SuccessCallback(Job pJob)
        {
            _appState = AppState.AfterView;
            ConfigureUI();

            ShowJobDataTabPageView(pJob);
        }

        private void ViewJobApprove_SuccessCallback(Job pJob)
        {
            _selectedJob = pJob;
            _appState = AppState.AfterApprove;
            ConfigureUI();

            ShowJobDataTabPageReview(_selectedJob);
        }

        private void GetAccountBalanceOrdered_SuccessCallback(float accountBalance)
        {
            UpdateCredists(accountBalance);

            UpdateJobs(_jobList);

            _appState = AppState.AfterOrder;
            ConfigureUI();
        }

        private void ViewJobRequestCorrection_SuccessCallback(Job pJob)
        {
            _selectedJob = pJob;

            _appState = AppState.AfterRequestCorrection;
            ConfigureUI();
        }

        private void ReviewJob_SuccessCallback(Job pJob)
        {
            _appState = AppState.AfterReview;
            ConfigureUI();

            _selectedJob.Body_src = pJob.Body_src;
            _selectedJob.PreviewImage = pJob.PreviewImage;
            if(!string.IsNullOrEmpty(pJob.CaptchaURL))
                _selectedJob.CaptchaURL = pJob.CaptchaURL;

            ShowJobDataTabPageReview(_selectedJob);
        }

        private void ApproveJob_SuccessCallback(bool jobApproved)
        {
            if (_clientWrapper == null)
                return;

            _clientWrapper.ViewJob(ViewJobApprove_SuccessCallback, Generic_ErrorCallBack, _selectedJob.Job_Id, string.Empty);
        }

        private void CorrectJob_SuccessCallback(bool requestAcepted)
        {
            if (_clientWrapper == null)
                return;

            _clientWrapper.ViewJob(ViewJobRequestCorrection_SuccessCallback, Generic_ErrorCallBack, _selectedJob.Job_Id, string.Empty);
        }

        private void RejectJob_SuccessCallback(bool jobRejected)
        {
            if (_clientWrapper == null)
                return;

            _clientWrapper.ViewJob(ViewJobReject_SuccessCallback, Generic_ErrorCallBack, _selectedJob.Job_Id, string.Empty);
        }

        private void ViewJobReject_SuccessCallback(Job pJob)
        {
            _selectedJob = pJob;

            _appState = AppState.AfterReject;
            ConfigureUI();
        }

        private void ShowJobDataTabPageReview(Job pJob)
        {
            if (tabControlFuncionalities.InvokeRequired)
            {
                ShowJobDataTabPageReviewCallBack d = new ShowJobDataTabPageReviewCallBack(ShowJobDataTabPageReview);
                this.Invoke(d, new object[] { pJob });
            }
            else
            {
                jobViewerSingleReview.LoadJob(pJob);

                lbOriginalTextReview.Text = "Original text " + "[" + GetLanguageStr(_selectedJob.Lc_src) + "]";
                textBoxOriginalTextReview.Text = pJob.Body_src;
                //textBoxOriginalTextReview.Enabled = false;
                textBoxOriginalTextReview.ReadOnly = true;

                if (_selectedJob.Status == EJobStatus.JOB_STATUS_APPROVED)
                {
                    textBoxTranslatedTextReview.Visible = true;
                    textBoxTranslatedTextReview.Text = pJob.Body_tgt;
                    pictureBoxTranslatedTextReview.Visible = false;
                }
                else
                {
                    textBoxTranslatedTextReview.Visible = false;
                    pictureBoxTranslatedTextReview.Visible = true;
                }


                lbTranslatedTextReview.Text = "Translated text " + "[" + GetLanguageStr(_selectedJob.Lc_tgt) + "]";

                //string folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + Properties.Resources.AppDirectory + "Previews";
                //string impagePath = string.Format("{0}/{1}", folder, pJob.PreviewImage);

                string impagePath = pJob.PreviewImage;
                pictureBoxTranslatedTextReview.ImageLocation = impagePath;
            }
        }

        private void ShowJobDataTabPageView(Job pJob)
        {
            if (tabControlFuncionalities.InvokeRequired)
            {
                ShowJobDataTabPageViewCallBack d = new ShowJobDataTabPageViewCallBack(ShowJobDataTabPageView);
                this.Invoke(d, new object[] { pJob });
            }
            else
            {
                jobViewerSingleView.LoadJob(pJob);

                lbOriginalTextView.Text = "Original text " + "[" + GetLanguageStr(pJob.Lc_src) + "]";
                textBoxOriginalTextView.Text = pJob.Body_src;
                //textBoxOriginalTextView.Enabled = false;
                textBoxOriginalTextView.ReadOnly = true;
                lbTranslatedTextView.Text = "Translated text " + "[" + GetLanguageStr(pJob.Lc_tgt) + "]";
                textBoxTranslatedTextView.Text = pJob.Body_tgt;
                //textBoxTranslatedTextView.Enabled = false;
                textBoxTranslatedTextView.ReadOnly = true;

                pictureBoxMtView.Visible = lbMtTexView.Visible = pJob.Status != EJobStatus.JOB_STATUS_APPROVED;

            }
        }

        private string GetLanguageStr(string pLc)
        {
            return _languageList.Where(l => l.Lc == pLc).FirstOrDefault().LanguageName;
        }

        private void UpdateLanguages(List<Language> languageList)
        {
            if (lbCredits.InvokeRequired)
            {
                UpdateLanguagesCallBack d = new UpdateLanguagesCallBack(UpdateLanguages);
                this.Invoke(d, new object[] { languageList });
            }
            else
            {
                comboBoxLanguageFrom.DataSource = languageList;
                comboBoxLanguageFrom.ValueMember = "Lc";
                comboBoxLanguageFrom.DisplayMember = "LanguageName";
            }
        }

        private void UpdateJobs(List<Job> jobList)
        {
            if (lbCredits.InvokeRequired)
            {
                UpdateJobsCallBack d = new UpdateJobsCallBack(UpdateJobs);
                this.Invoke(d, new object[] { jobList });
            }
            else
            {
                jobsViewer1.LoadJobs(jobList.ToArray());
                jobsViewer1.Refresh();
            }
        }

        private void UpdateCredists(float result)
        {
            if (lbCredits.InvokeRequired)
            {
                UpdateCredistsCallBack d = new UpdateCredistsCallBack(UpdateCredists);
                this.Invoke(d, new object[] { result });
            }
            else
            {
                string creditsFormatted = Utils.ConvertToCurrencyString(result.ToString());
                lbCredits.Text = creditsFormatted;
                ShowCredits(true);
            }
        }

        #endregion

        private void SaveSession()
        {
            //DirectoryInfo directoryInfo = null;

            //if (!Directory.Exists(_appFullPath))
            //    directoryInfo = System.IO.Directory.CreateDirectory(_appFullPath);

            Session session = new Session();
            session.PublicKey = textboxPublicKey.Text;
            session.PrivateKey = textBoxPrivateKey.Text;           

            //string filePath = _appFullPath + "Session.xml";
            string appFullPath = GetFolderPathData();
            string filePath = appFullPath + "Session.xml";

            XmlDocument doc = Serialize(Encoding.UTF8, session);
            doc.Save(filePath);
        }

        private Session GetSession()
        {
            try
            {
                XmlDocument xmlDocument = new XmlDocument();

                //string filePath = _appFullPath + "Session.xml";
                string appFullPath = GetFolderPathData();
                string filePath = appFullPath + "Session.xml";
                xmlDocument.Load(filePath);

                Session session = (Session)Deserialize(Encoding.UTF8, xmlDocument, typeof(Session));

                return session;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private void ClearSession()
        {
            try
            {
                //string filePath = _appFullPath + "Session.xml";
                string appFullPath = GetFolderPathData();
                string filePath = appFullPath + "Session.xml";
                System.IO.File.Delete(filePath);
            }
            catch (Exception)
            {
                //Por si el fichero no existe
            }
        }

        private void LogOff()
        {
            ClearSession();

            _appState = AppState.BeforeLogin;
            ConfigureUI();

            ShowNotification(Properties.Resources.LogOfSuccess, ENotificationType.Success);
        }

        private void LogIn()
        {
            _appState = AppState.Logging;
            ConfigureUI();

            _clientWrapper.Initialize(textboxPublicKey.Text, textBoxPrivateKey.Text, _specificFolderName);
            _clientWrapper.GetAccountBalance(GetAccountBalance_SuccessCallback, Generic_ErrorCallBack);
        }

        public static int CountWords(string s)
        {
            MatchCollection collection = Regex.Matches(s, @"[\S]+");
            return collection.Count;
        }

        private bool IsValidLanguage(ComboBox comboBoxLanguage)
        {
            if (comboBoxLanguage == null)
                return false;

            if (comboBoxLanguage.Items.Count == 0)
                return false;

            object selectedValue = comboBoxLanguage.SelectedValue;
            if (selectedValue == null)
                return false;

            if (selectedValue.GetType() == typeof(Language))
                return false;
            else if (selectedValue.GetType() == typeof(string) && !string.IsNullOrEmpty((string)selectedValue))
                return true;
            else
                return false;
        }

        public void ShowDialog(int activeTab)
        {
            tabControlFuncionalities.SelectedIndex = activeTab;
            this.ShowDialog();
        }

        private void GoErrorState()
        {
            switch (_appState)
            {
                case AppState.Logging:
                    {
                        _appState = AppState.ErrorLogin;
                        break;
                    }

                case AppState.Ordering:
                    {
                        _appState = AppState.ErrorOrdering;
                        break;
                    }
                case AppState.Canceling:
                    {
                        _appState = AppState.ErrorCanceling;
                        break;
                    }
                case AppState.Viewing:
                    {
                        _appState = AppState.ErrorViewing;
                        break;
                    }
                case AppState.Reviewing:
                    {
                        _appState = AppState.ErrorReviewing;
                        break;
                    }
                case AppState.Approving:
                    {
                        _appState = AppState.ErrorApproving;
                        break;
                    }
                case AppState.RequestingCorrection:
                    {
                        _appState = AppState.ErrorRequestingCorrection;
                        break;
                    }
                case AppState.Rejecting:
                    {
                        _appState = AppState.ErrorRejecting;
                        break;
                    }
            }
            ConfigureUI();
        }

        public void DisposeResources()
        {             //Limpiar la carpeta Preview
            string folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + Properties.Resources.AppDirectory + "//Previews";
            DirectoryInfo dir = new DirectoryInfo(folder);
            if (dir != null && dir.Exists)
                dir.Delete(true);

        }

        #region XML work (Serialization)

        private static XmlDocument Serialize(Encoding encoding, object obj, string rootName = null)
        {
            return Serialize(encoding, obj, obj.GetType(), rootName);
        }

        private static XmlDocument Serialize(Encoding encoding, object obj, Type type, string rootName = null)
        {
            XmlSerializer xmlSerializer = null;
            if (string.IsNullOrEmpty(rootName))
                xmlSerializer = new XmlSerializer(type);
            else
            {
                XmlRootAttribute root = new XmlRootAttribute(rootName);
                xmlSerializer = new XmlSerializer(type, root);
            }

            RWriter stringWriter = new RWriter(encoding);

            System.Xml.XmlWriterSettings writerSettings = new System.Xml.XmlWriterSettings();
            writerSettings.Encoding = encoding;
            writerSettings.Indent = true;

            XmlDocument serialized = null;
            System.Xml.XmlWriter writer = System.Xml.XmlWriter.Create(stringWriter, writerSettings);
            try
            {
                xmlSerializer.Serialize(writer, obj);

                serialized = new XmlDocument();
                serialized.LoadXml(stringWriter.ToString());
            }
            catch (Exception) { }

            writer.Close();
            stringWriter.Close();

            return serialized;
        }

        private static object Deserialize(Encoding encoding, XmlDocument serializedObject, Type objectType, string rootName = null)
        {
            XmlSerializer xmlSerializer = null;
            if (string.IsNullOrEmpty(rootName))
                xmlSerializer = new XmlSerializer(objectType);
            else
            {
                XmlRootAttribute root = new XmlRootAttribute(rootName);
                xmlSerializer = new XmlSerializer(objectType, root);
            }

            StringReader stringReader = new StringReader(serializedObject.OuterXml);

            object obj = null;
            try
            {
                obj = xmlSerializer.Deserialize(stringReader);

            }
            catch (Exception) { }

            stringReader.Close();

            return obj;
        }


        #endregion

        #endregion

        #region Events

        private void btnLogIn_Click(object sender, EventArgs e)
        {
            #region Validations

            bool error = false;
            if (textboxPublicKey.Text == string.Empty)
            {
                error = true;
                lblFieldEmptyPublicKey.Visible = true;
            }
            else
                lblFieldEmptyPublicKey.Visible = false;

            if (textBoxPrivateKey.Text == string.Empty)
            {
                error = true;
                lblFieldEmptyPrivateKey.Visible = true;
            }
            else
                lblFieldEmptyPrivateKey.Visible = false;

            if (error)
                return;

            #endregion

            Button button = sender as Button;
            if (button.Text == Properties.Resources.LogIn)
                LogIn();
            else
                LogOff();
        }

        void jobsViewer1_ViewClick(object sender, object job)
        {
            if (job == null)
                return;

            _selectedJob = job as Job;

            _appState = AppState.Viewing;
            ConfigureUI();

            string pre_mt = string.Empty;
            if (_selectedJob.Status != EJobStatus.JOB_STATUS_APPROVED)
            {
                pre_mt = "1";
            }

            _clientWrapper.ViewJob(ViewJob_SuccessCallback, Generic_ErrorCallBack, _selectedJob.Job_Id, pre_mt);
        }

        void jobsViewer1_ReviewClick(object sender, object job)
        {
            if (job == null)
                return;

            _selectedJob = job as Job;

            _appState = AppState.Reviewing;
            ConfigureUI();

            _clientWrapper.ReviewJob(ReviewJob_SuccessCallback, Generic_ErrorCallBack, _selectedJob.Job_Id);
        }

        void jobsViewer1_CancelClick(object sender, object job)
        {
            if (job == null)
                return;

            _selectedJob = job as Job;

            _appState = AppState.Canceling;
            ConfigureUI();

            _clientWrapper.CancelJob(CancelJob_SuccessCallback, Generic_ErrorCallBack, ((Job)job).Job_Id);
        }

        private void btnTierStandard_Click(object sender, EventArgs e)
        {
            SetTierButtonValues(btnTierStandard);
            SetOrderPrice();
            btnOrder.Enabled = IsValidJob();
            _selectedTier = Properties.Resources.TIER_STANDARD;
        }

        private void SetTierButtonValues(Button button)
        {
            if (button == null)
                return;

            ResetTierButtons();
            button.BackColor = _btnTierSelectedColor;
            _tierClicked = true;

            if (button.Tag == null)
            {
                _unitPrice = -1;
                return;
            }

            OrderPriceFactor orderPriceFactor = (OrderPriceFactor)button.Tag;
            _unitPrice = orderPriceFactor.UnitPrice;
        }

        private void btnTierProfessional_Click(object sender, EventArgs e)
        {
            SetTierButtonValues(btnTierProfessional);
            SetOrderPrice();
            btnOrder.Enabled = IsValidJob();
            _selectedTier = Properties.Resources.TIER_PROFESSIONAL;
        }

        private void btnTierUltra_Click(object sender, EventArgs e)
        {
            SetTierButtonValues(btnTierUltra);
            SetOrderPrice();
            btnOrder.Enabled = IsValidJob();
            _selectedTier = Properties.Resources.TIER_ULTRA;
        }

        private void btnTierMachine_Click(object sender, EventArgs e)
        {
            SetTierButtonValues(btnTierMachine);
            _unitPrice = -1;
            SetOrderPrice();
            _selectedTier = Properties.Resources.TIER_MACHINE;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // tab Overview: initilize jobsViewer control
            jobsViewer1.JobDataProvider = new MyJobDataProvider();
            jobsViewer1.ViewClick += new JobsViewer.JobsViewer.JobViewerActionHandler(jobsViewer1_ViewClick);
            jobsViewer1.ReviewClick += new JobsViewer.JobsViewer.JobViewerActionHandler(jobsViewer1_ReviewClick);
            jobsViewer1.CancelClick += new JobsViewer.JobsViewer.JobViewerActionHandler(jobsViewer1_CancelClick);

            // tab Overview: load jobs
            object[] jobList = { "object1", "object2", "object3", "object4", "object5", "object6" };
            jobsViewer1.LoadJobs(jobList);

            // tab View: initilize jobViewerSingle control
            jobViewerSingleView.JobDataProvider = new MyJobDataProvider();

            // tab View: load job
            object job = "object1";
            jobViewerSingleView.LoadJob(job);
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            HideNotification();

            _appState = AppState.BeforeLogin;
            ConfigureUI();
        }

        private void comboBoxLanguageFrom_SelectedIndexChanged(object sender, EventArgs e)
        {
            _languageFrom = null;
            object selectedValue = ((ComboBox)sender).SelectedValue;
            Language selectedLanguage = null;

            if (selectedValue == null)
                return;

            if (selectedValue.GetType() == typeof(Language))
            {
                selectedLanguage = (Language)((ComboBox)sender).SelectedValue;
            }
            else if (selectedValue.GetType() == typeof(string))
            {
                selectedLanguage = _languageList.Where(l => l.Lc == (string)selectedValue).FirstOrDefault();
            }

            if (selectedLanguage == null)
                return;

            List<Language> languageListTo = new List<Language>();
            if (selectedLanguage.LanguageName == Properties.Resources.NotSpecified)
            {
                languageListTo.Add(selectedLanguage);
                lbTotalUnits.Text = CountWords(textBoxTexToTranslate.Text).ToString();
                labelAmmount.Text = "$" + Utils.ConvertToCurrencyString("0");
                _tierClicked = false;
                _unitPrice = -1;
                ResetTierButtons();
                _selectedTier = string.Empty;
            }
            else
            {
                _languageFrom = selectedLanguage;

                string unitType = selectedLanguage.UnitType.ToString();
                if (unitType == Properties.Resources.TIER_UNIT_TYPE_CHARACTER)
                    unitType = Properties.Resources.TIER_UNIT_TYPE_CHAR;

                lbTotalUnits.Text = CountWords(textBoxTexToTranslate.Text).ToString() + " " + unitType + "s";

                List<LanguagePair> languagePairs = _languagePairs.Where(l => l.Lc_src == selectedLanguage.Lc).ToList();
                foreach (var languagePair in languagePairs)
                {
                    Language language = _languageList.Where(l => l.Lc == languagePair.Lc_tgt).FirstOrDefault();
                    if (!languageListTo.Contains(language))
                        languageListTo.Add(language);
                }
            }

            comboBoxLanguageTo.DataSource = languageListTo;
            comboBoxLanguageTo.ValueMember = "Lc";
            comboBoxLanguageTo.DisplayMember = "LanguageName";
            //comboBoxLanguageTo_SelectedIndexChanged(comboBoxLanguageTo, null);
        }

        private void textboxPublicKey_TextChanged(object sender, EventArgs e)
        {
            btnClear.Enabled = (!string.IsNullOrEmpty(textboxPublicKey.Text) || !string.IsNullOrEmpty(textBoxPrivateKey.Text));

            btnLogIn.Enabled = (!string.IsNullOrEmpty(textboxPublicKey.Text) && !string.IsNullOrEmpty(textBoxPrivateKey.Text));
        }

        private void tabControlFuncionalities_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControlFuncionalities.SelectedTab == tabPageOrder)
                if (comboBoxLanguageFrom.Items.Count > 0 && comboBoxLanguageFrom.SelectedIndex == 0)
                    comboBoxLanguageFrom_SelectedIndexChanged(comboBoxLanguageFrom, null);
        }

        private void comboBoxLanguageTo_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnOrder.Enabled = IsValidJob();

            if (_languagePairs == null || _languagePairs.Count == 0)
            {
                _selectedLanguagePairs = null;
                return;
            }

            string lcFrom = comboBoxLanguageFrom.SelectedValue as string;
            string lcTo = comboBoxLanguageTo.SelectedValue as string;

            if (lcFrom == null || lcTo == null)
            {
                _selectedLanguagePairs = null;
                return;
            }

            Language languangeFrom = _languageList.Where(l => l.Lc == lcFrom).FirstOrDefault();
            if (languangeFrom == null)
                return;

            _selectedLanguagePairs = _languagePairs.Where(l => l.Lc_src == lcFrom && l.Lc_tgt == lcTo).ToList();

            btnTierStandard.Enabled = btnTierProfessional.Enabled =
                btnTierUltra.Enabled = btnTierMachine.Enabled = false;
            btnTierStandard.Tag = btnTierProfessional.Tag =
                btnTierUltra.Tag = btnTierMachine.Tag = null;

            string priceFormatted = string.Empty;
            if (_selectedLanguagePairs == null || _selectedLanguagePairs.Count == 0)
            {
                btnTierStandard.Text = Properties.Resources.BtnTierStardard;
                btnTierProfessional.Text = Properties.Resources.BtnTierProfessional;
                btnTierUltra.Text = Properties.Resources.BtnTierUltra;
                btnTierMachine.Text = Properties.Resources.BtnTierMachine;
            }
            else
            {
                foreach (var languagePairs in _selectedLanguagePairs)
                {
                    string unitType = languangeFrom.UnitType.ToString();
                    if (unitType == Properties.Resources.TIER_UNIT_TYPE_CHARACTER)
                        unitType = Properties.Resources.TIER_UNIT_TYPE_CHAR;

                    if (languagePairs.Tier == Properties.Resources.TIER_STANDARD)
                    {
                        btnTierStandard.Enabled = true;
                        priceFormatted = Utils.ConvertToCurrencyString(languagePairs.Unit_price.ToString());
                        btnTierStandard.Text = Properties.Resources.BtnTierStardard + "\n" + "$" +
                            priceFormatted + "/" + unitType;
                        btnTierStandard.Tag = new OrderPriceFactor() { UnitPrice = languagePairs.Unit_price, UnitType = unitType };
                    }
                    else if (languagePairs.Tier == Properties.Resources.TIER_PROFESSIONAL)
                    {
                        btnTierProfessional.Enabled = true;
                        priceFormatted = Utils.ConvertToCurrencyString(languagePairs.Unit_price.ToString());
                        btnTierProfessional.Text = Properties.Resources.BtnTierProfessional + "\n" + "$" +
                           priceFormatted + "/" + unitType;
                        btnTierProfessional.Tag = new OrderPriceFactor() { UnitPrice = languagePairs.Unit_price, UnitType = unitType };
                    }
                    else if (languagePairs.Tier == Properties.Resources.TIER_ULTRA)
                    {
                        btnTierUltra.Enabled = true;
                        priceFormatted = Utils.ConvertToCurrencyString(languagePairs.Unit_price.ToString());
                        btnTierUltra.Text = Properties.Resources.BtnTierUltra + "\n" + "$" +
                            priceFormatted + "/" + unitType;
                        btnTierUltra.Tag = new OrderPriceFactor() { UnitPrice = languagePairs.Unit_price, UnitType = unitType };
                    }
                    else if (languagePairs.Tier == Properties.Resources.TIER_MACHINE)
                    {
                        btnTierMachine.Enabled = true;                        
                        btnTierMachine.Tag = new OrderPriceFactor() { UnitPrice = languagePairs.Unit_price, UnitType = unitType };
                    }
                }
            }
        }

        private void textBoxTexToTranslate_TextChanged(object sender, EventArgs e)
        {
            _wordCount = CountWords(textBoxTexToTranslate.Text);

            if (_languageFrom != null)
            {
                string unitType = _languageFrom.UnitType.ToString();
                if (unitType == Properties.Resources.TIER_UNIT_TYPE_CHARACTER)
                    unitType = Properties.Resources.TIER_UNIT_TYPE_CHAR;
                lbTotalUnits.Text = _wordCount.ToString() + " " + unitType + "s";
            }
            else
                lbTotalUnits.Text = _wordCount.ToString();

            SetOrderPrice();
            btnOrder.Enabled = IsValidJob();
        }

        private bool IsValidJob()
        {
            bool IsValidLanguageFrom = IsValidLanguage(comboBoxLanguageFrom);
            bool IsValidLanguageTo = IsValidLanguage(comboBoxLanguageTo);

            return _wordCount > 0 && !string.IsNullOrEmpty(textBoxJobTitle.Text) && IsValidLanguageFrom && IsValidLanguageTo &&
                _tierClicked;
        }

        private void textBoxJobTitle_TextChanged(object sender, EventArgs e)
        {
            btnOrder.Enabled = IsValidJob();
        }

        private void buttonOrder_Click(object sender, EventArgs e)
        {
            _appState = AppState.Ordering;
            ConfigureUI();

            string pType = "text";
            string pSlug = textBoxJobTitle.Text.Replace("\"", "'");
            string pBodySrc = textBoxTexToTranslate.Text.Replace("\"", "'"); ;
            string pLc_src = (string)comboBoxLanguageFrom.SelectedValue;
            string pLc_tgt = (string)comboBoxLanguageTo.SelectedValue;
            string pTier = _selectedTier;
            string pAutoApprove = "0";
            string pCustomData = string.Empty;
            string pComment = textBoxComment.Text;

            _clientWrapper.TranslateJob(TransalteJob_SuccessCallback, Generic_ErrorCallBack, pType, pSlug, pBodySrc, pLc_src, pLc_tgt, pTier, pAutoApprove, pCustomData, pComment);
        }

        private void linkLabelGetYourKeys_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo("iexplore");
            startInfo.WindowStyle = ProcessWindowStyle.Normal;
            startInfo.Arguments = Properties.Resources.LINK_GET_YOUR_KEYS;

            Process.Start(startInfo);
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo("iexplore");
            startInfo.WindowStyle = ProcessWindowStyle.Normal;
            startInfo.Arguments = Properties.Resources.LINK_WHAT_MEAN_TIERS;

            Process.Start(startInfo);
        }

        private void btnShowRejectForm_Click(object sender, EventArgs e)
        {
            if (_selectedJob == null)
                return;

            _appState = AppState.BeforeReject;
            ConfigureUI();
        }

        private void btnShowCorrectForm_Click(object sender, EventArgs e)
        {
            if (_selectedJob == null)
                return;

            _appState = AppState.BeforeRequestCorrection;
            ConfigureUI();
        }

        private void SetRatingButtonValues(Button button)
        {
            if (button == null)
                return;

            ResetRatingButtons();
            button.BackColor = _btnTierSelectedColor;

            _selectedRating = button.Text;
        }

        private void btnRating_Click(object sender, EventArgs e)
        {
            SetRatingButtonValues((Button)sender);
        }

        private void btnDoneView_Click(object sender, EventArgs e)
        {
            _selectedJob = null;
            tabControlFuncionalities.SelectedTab = tabPageOverview;
            tabControlFuncionalities.TabPages.Remove(tabPageView);
        }

        private void btnApprove_Click(object sender, EventArgs e)
        {
            if (btnApprove.Text == "Done")
            {
                if (tabControlFuncionalities.TabPages.Contains(tabPageReject))
                    tabControlFuncionalities.TabPages.Remove(tabPageReject);

                if (tabControlFuncionalities.TabPages.Contains(tabPageRequestCorrection))
                    tabControlFuncionalities.TabPages.Remove(tabPageRequestCorrection);

                tabControlFuncionalities.TabPages.Remove(tabPageReview);
                tabControlFuncionalities.SelectedTab = tabPageOverview;

                return;
            }

            _appState = AppState.Approving;
            ConfigureUI();

            _clientWrapper.ApproveJob(ApproveJob_SuccessCallback, Generic_ErrorCallBack, _selectedJob.Job_Id, _selectedRating);

            _selectedRating = string.Empty;
        }

        private void btnRequestCorrection_Click(object sender, EventArgs e)
        {
            if (btnRequestCorrection.Text == "Done")
            {
                tabControlFuncionalities.TabPages.Remove(tabPageRequestCorrection);
                tabControlFuncionalities.SelectedTab = tabPageReview;

                return;
            }

            #region Validations

            bool error = false;
            if (string.IsNullOrEmpty(textBoxCorrectionRequest.Text))
            {
                error = true;
                lblFieldEmptyFormalCorrection.Visible = true;
            }
            else
                lblFieldEmptyFormalCorrection.Visible = false;

            if (error)
                return;

            #endregion

            _appState = AppState.RequestingCorrection;
            ConfigureUI();

            _clientWrapper.CorrectJob(CorrectJob_SuccessCallback, Generic_ErrorCallBack, _selectedJob.Job_Id, textBoxCorrectionRequest.Text);
        }

        private void textBoxCorrectionRequest_TextChanged(object sender, EventArgs e)
        {
            btnRequestCorrection.Enabled = !string.IsNullOrEmpty(textBoxCorrectionRequest.Text.Trim());
        }

        private void textBoxFeedBack_TextChanged(object sender, EventArgs e)
        {
            btnReject.Enabled = (!string.IsNullOrEmpty(textBoxFeedBack.Text.Trim()) && !string.IsNullOrEmpty(textBoxCaptcha.Text.Trim()) && _captchaLoaded);
        }

        private void btnReject_Click(object sender, EventArgs e)
        {
            if (btnReject.Text == "Done")
            {
                tabControlFuncionalities.TabPages.Remove(tabPageReject);
                tabControlFuncionalities.SelectedTab = tabPageReview;

                return;
            }

            #region Validations

            bool error = false;
            if (string.IsNullOrEmpty(textBoxFeedBack.Text))
            {
                error = true;
                lblFieldEmptyFeedback.Visible = true;
            }
            else
                lblFieldEmptyFeedback.Visible = false;

            if (string.IsNullOrEmpty(textBoxCaptcha.Text))
            {
                error = true;
                lblFieldEmptyCaptcha.Visible = true;
            }
            else
                lblFieldEmptyCaptcha.Visible = false;

            if (error)
                return;

            #endregion

            _appState = AppState.Rejecting;
            ConfigureUI();

            string followUp = string.Empty;
            if (radioBtnFollowUpCancel.Checked)
                followUp = Properties.Resources.JOB_REJECT_FOLLOWUP_CANCEL;
            else
                followUp = Properties.Resources.JOB_REJECT_FOLLOWUP_REQUEUE;

            string reason = string.Empty;
            if (radioBtnReasonQuality.Checked)
                reason = Properties.Resources.JOB_REJECT_REASON_QUALITY;
            else if (radioBtnReasonIncomplete.Checked)
                reason = Properties.Resources.JOB_REJECT_REASON_INCOMPLETE;
            else if (radioBtnReasonOther.Checked)
                reason = Properties.Resources.JOB_REJECT_REASON_OTHER;

            _clientWrapper.RejectJob(RejectJob_SuccessCallback, Generic_ErrorCallBack, _selectedJob.Job_Id, reason, textBoxFeedBack.Text, textBoxCaptcha.Text, followUp);
        }

        private void pictureBoxCaptcha_LoadCompleted(object sender, AsyncCompletedEventArgs e)
        {
            pictureBoxLoadingCaptcha.Visible = false;
            lbLoadingCaptcha.Visible = false;

            if (e.Error != null)
            {
                ShowNotification("App error: Error loading captcha image", ENotificationType.Error);
                return;
            }

            textBoxCaptcha.Enabled = _captchaLoaded = true;
            HideLoading();
        }

        private void tabControlFuncionalities_ControlAdded(object sender, ControlEventArgs e)
        {
            if (e.Control.Equals(tabPageOrder))
                OrderTabVisibleChanged(sender, true);
            else if (e.Control.Equals(tabPageOverview))
                OverviewTabVisibleChanged(sender, true);
        }

        void tabControlFuncionalities_ControlRemoved(object sender, ControlEventArgs e)
        {
            if (e.Control.Equals(tabPageOrder) && OrderTabVisibleChanged != null)
                OrderTabVisibleChanged(sender, false);
            else if (e.Control.Equals(tabPageOverview) && OverviewTabVisibleChanged != null)
                OverviewTabVisibleChanged(sender, false);
        }

        private void lbIndicaCredits_TextChanged(object sender, EventArgs e)
        {
            Label label = (Label)sender;
            CreditsChanged(label.Text);
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            _refreshing = true;

            _appState = AppState.BeforeLogin;
            ConfigureUI();

            _languageList = null;
            _languagePairs = null;
            _selectedLanguagePairs = null;
            _languageFrom = null;
            _unitPrice = -1;
            _tierClicked = false;
            _wordCount = 0;
            _selectedTier = string.Empty;
            _jobList = null;
            _selectedJob = null;
            _selectedRating = string.Empty;
            _captchaLoaded = false;

            tabControlFuncionalities.TabPages.Remove(tabPageReject);
            tabControlFuncionalities.TabPages.Remove(tabPageRequestCorrection);
            tabControlFuncionalities.TabPages.Remove(tabPageReview);
            tabControlFuncionalities.TabPages.Remove(tabPageView);

            Session session = GetSession();
            if (session != null)
            {
                textBoxPrivateKey.Text = session.PrivateKey;
                textboxPublicKey.Text = session.PublicKey;
                LogIn();
            }
        }

        #endregion

        private string GetFolderPathData()
        {
            string specificFolder = string.Format(Properties.Resources.AppDirectory, _specificFolderName);
            string folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + specificFolder;
            if (!System.IO.Directory.Exists(folder))
                System.IO.Directory.CreateDirectory(folder);

            return folder;
        }
    }
}

