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

namespace MyGengoTranslator
{
    public enum ErrorType { AppError, ApiError };

    public class ErrorInfo
    {
        private ErrorType _errorType;
        private string _errorCode;   
        private string _errorMessage;

        public ErrorType ErrorType
        {
            get { return _errorType; }
            set { _errorType = value; }
        }

        public string ErrorMessage
        {
            get { return _errorMessage; }
            set { _errorMessage = value; }
        }

        public string ErrorCode
        {
            get { return _errorCode; }
            set { _errorCode = value; }
        }

        public ErrorInfo()
        { }

        public ErrorInfo(ErrorType pErrorType, string pErrorMessage)
        {
            _errorType = pErrorType;
            _errorMessage = pErrorMessage;            
        }

        public ErrorInfo(ErrorType pErrorType, string pErrorMessage, string pErrorCode)
        {
            _errorType = pErrorType;
            _errorMessage = pErrorMessage;
            _errorCode = pErrorCode;
        }
    }
}
