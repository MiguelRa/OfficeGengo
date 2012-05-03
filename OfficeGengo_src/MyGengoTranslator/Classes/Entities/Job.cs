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

namespace MyGengoTranslator.Classes
{
    public class Job
    {
        private string _job_Id;
        public string Job_Id
        {
            get { return _job_Id; }
            set { _job_Id = value; }
        }

        private string _slug;
        public string Slug
        {
            get { return _slug; }
            set { _slug = value; }
        }

        private string _body_src;
        public string Body_src
        {
            get { return _body_src; }
            set { _body_src = value; }
        }

        private string _previewImage;
        public string PreviewImage
        {
            get { return _previewImage; }
            set { _previewImage = value; }
        }
       
        private string _body_tgt;
        public string Body_tgt
        {
            get { return _body_tgt; }
            set { _body_tgt = value; }
        }

        private string _lc_src;
        public string Lc_src
        {
            get { return _lc_src; }
            set { _lc_src = value; }
        }

        private string _lc_tgt;
        public string Lc_tgt
        {
            get { return _lc_tgt; }
            set { _lc_tgt = value; }
        }

        private int _unit_count;
        public int Unit_count
        {
            get { return _unit_count; }
            set { _unit_count = value; }
        }
        
        private string _tier;
        public string Tier
        {
            get { return _tier; }
            set { _tier = value; }
        }

        private float _credits;
        public float Credits
        {
            get { return _credits; }
            set { _credits = value; }
        }

        private string _status;
        public string Status
        {
            get { return _status; }
            set { _status = value; }
        }

        private string _eta;
        public string Eta
        {
            get { return _eta; }
            set { _eta = value; }
        }
        
        private DateTime _ctime;
        public DateTime Ctime
        {
            get { return _ctime; }
            set { _ctime = value; }
        }
        
        private string _callback_url;
        public string Callback_url
        {
            get { return _callback_url; }
            set { _callback_url = value; }
        }
        
        private bool _auto_approve;
        public bool Auto_approve
        {
            get { return _auto_approve; }
            set { _auto_approve = value; }
        }
        
        private string custom_data;
        public string Custom_data
        {
            get { return custom_data; }
            set { custom_data = value; }
        }

        private string _captchaURL;

        public string CaptchaURL
        {
            get { return _captchaURL; }
            set { _captchaURL = value; }
        }

        public Job() { }
    }
}
