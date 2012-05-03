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
using System.Configuration;
using System.Net;
using System.Threading;
using System.Windows.Forms;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.ComponentModel;
using MyGengoTranslator.Classes;

namespace MyGengoTranslator
{
    public class MyGengoClientWrapper
    {
        #region Atributes

        private static MyGengoClientWrapper _myGengoClientWrapper = null;
        private bool _modeOffline = Convert.ToBoolean(ConfigurationManager.AppSettings["wGengo_modeOffline"]);

        private string _privateKey = string.Empty;
        private string _publicKey = string.Empty;
        private string _specificFolderName = string.Empty;

        #endregion

        #region Properties.Resources

        public static MyGengoClientWrapper Instance
        {
            get
            {
                if (_myGengoClientWrapper == null)
                    _myGengoClientWrapper = new MyGengoClientWrapper();

                return _myGengoClientWrapper;
            }
            set
            {
                if (value != null)
                    _myGengoClientWrapper = value;
            }
        }

        #endregion

        #region Constructor

        public MyGengoClientWrapper()
        {
        }

        #endregion

        #region Methods

        #region Common

        /// <summary>
        /// delegate que representa un método de tipo void que recibe como parámetro un ErrorInfo
        /// </summary>
        /// <param name="errorInfo"></param>
        public delegate void ErrorCallBackDelegate(ErrorInfo errorInfo);

        /// <summary>
        /// Referencia a un método con la firma al delegado declarado anteriormente. 
        /// que se utiliza para llamar a dicho metodo cuando ocurrió algun error en la llamada a la API
        /// </summary>
        private ErrorCallBackDelegate methodErrorCallback;

        /// <summary>
        /// Método que chequea si el resultado de la llamada asíncrona a la API falló
        /// </summary>
        /// <param name="asyncResult">Resultado de la llamada asícrona</param>
        /// <returns>Una intacia de la clase ErrorInfo con información del error. Null en caso de que no haya fallado</returns>
        private ErrorInfo CheckForError(string result)
        {
            // check for error in api response, throw Exception if positive
            ErrorInfo errorInfo = null;

            // parse response and fill errorInfo...
            //response = "{\"opstat\":\"error\",\"err\":{\"code\":1150,\"msg\":\"'api_key' is a required field\"}}"; 
            object resultJson = JsonConvert.DeserializeObject(result);
            string responseType = (((Newtonsoft.Json.Linq.JObject)resultJson).Property("opstat")).Value.ToString();
            responseType = responseType.Replace("\"", "");

            if (responseType.Equals("error"))
            {
                JObject errObject = (JObject)(((Newtonsoft.Json.Linq.JObject)resultJson).Property("err")).Value;
                string errorMessage = errObject.Property("msg").Value.ToString();
                string errorCode = errObject.Property("code").Value.ToString();
                errorMessage = errorMessage.Replace("\"", "");

                errorInfo = new ErrorInfo(ErrorType.ApiError, errorMessage, errorCode);
            }

            return errorInfo;
        }

        /// <summary>
        /// Método utilizado para parsear la respuesta JSon de la api a un objeto del dominio del wrapper
        /// </summary>
        /// <param name="result">respuesta en formato JSon de la API</param>
        /// <param name="apiMethod">Enumerado que representa el método en cuestión</param>
        /// <returns></returns>
        private object ConvertResultToObject(string result, ApiMethod apiMethod)
        {
            object objectResult = null;

            switch (apiMethod)
            {
                case ApiMethod.GetAccountBalance:
                    {
                        //{"opstat": "ok","response": {"credits": "1003.60"}}
                        object resultJson = JsonConvert.DeserializeObject(result);
                        JObject responseJson = (JObject)(((Newtonsoft.Json.Linq.JObject)resultJson).Property("response")).Value;
                        string creditsStr = responseJson.Property("credits").Value.ToString();
                        creditsStr = creditsStr.Replace("\"", "");
                        objectResult = Convert.ToSingle(creditsStr);

                        break;
                    }
                case ApiMethod.GetMyJobs:
                    {
                        //{"opstat":"ok","response":[{
                        //                          "job_id": "77777",
                        //                          "slug": "adfadfdsfaf.",
                        //                          "body_src": "Un texto",
                        //                          "lc_src": "en",
                        //                          "lc_tgt": "ja",
                        //                          "unit_count": "2",
                        //                          "tier": "machine",
                        //                          "credits": "0.80",
                        //                          "status": "available",
                        //                          "eta": "",
                        //                          "ctime": 1315423968,
                        //                          "callback_url": "",
                        //                          "auto_approve": "0",
                        //                          "custom_data": ""
                        //                          },{
                        //                              //otro job
                        //                          }]}

                        object resultJson = JsonConvert.DeserializeObject(result);
                        //JObject responseJson = (JObject)(((Newtonsoft.Json.Linq.JObject)resultJson).Property("response")).Value;
                        JArray array = (JArray)(((Newtonsoft.Json.Linq.JObject)resultJson).Property("response")).Value;

                        List<Job> jobsList = new List<Job>();
                        Job jobClient = null;
                        foreach (JObject job in array)
                        {
                            //NO se muestra un job con este estado!!! (17-3-2012). Razón: una vex z que se ordena un trabajo con tier = ultra,
                            //La API devuelve doble dicho trabajo, la copia vuelve con tier = ultra_proofread y no nos interesa motrar esto.
                            string tier = job.Property("tier").Value.ToString().Replace("\"", "");
                            if (tier == Properties.Resources.TIER_ULTRA_PROOFREAD)
                                continue;

                            jobClient = new Job();
                            jobClient.Job_Id = job.Property("job_id").Value.ToString().Replace("\"", "");
                            jobClient.Slug = job.Property("slug").Value.ToString().Replace("\"", "");
                            jobClient.Body_src = job.Property("body_src").Value.ToString().Replace("\"", "");
                            jobClient.Lc_src = job.Property("lc_src").Value.ToString().Replace("\"", "");
                            jobClient.Lc_tgt = job.Property("lc_tgt").Value.ToString().Replace("\"", "");
                            jobClient.Unit_count = Convert.ToInt32(job.Property("unit_count").Value.ToString().Replace("\"", ""));
                            jobClient.Tier = job.Property("tier").Value.ToString().Replace("\"", "");
                            string credits_str = job.Property("credits").Value.ToString().Replace("\"", "");
                            jobClient.Credits = Convert.ToSingle(credits_str);
                            //jobClient.Credits = (float)Convert.ToDouble(credits_str);
                            //jobClient.Credits = float.Parse(credits_str, System.Globalization.NumberStyles.Currency);
                            //jobClient.Credits = float.Parse(credits_str, System.Globalization.NumberStyles.Currency);                            
                            jobClient.Status = job.Property("status").Value.ToString().Replace("\"", "");
                            jobClient.Eta = job.Property("eta").Value.ToString().Replace("\"", "");

                            //TODO: Buscar mecasimo para convertir a una fecha váldia!!!
                            string ctimeStr = job.Property("ctime").Value.ToString().Replace("\"", "");
                            jobClient.Ctime = Utils.ConvertFromUnixTimeStamp(Convert.ToDouble(ctimeStr));
                            //jobClient.Custom_data = job.Property("custom_data").Value.ToString();

                            if (job.Property("captcha_url") != null && job.Property("captcha_url").Value != null)
                                jobClient.CaptchaURL = job.Property("captcha_url").Value.ToString().Replace("\"", "");

                            jobsList.Add(jobClient);
                        }

                        objectResult = jobsList;
                        break;
                    }
                case ApiMethod.GetLanguages:
                    {
                        //{"opstat":"ok","response":[{
                        //                          "language": "English",
                        //                          "localized_name": "English",
                        //                          "lc": "en",
                        //                          "unit_type": "word"
                        //                          },{
                        //                              //otro language
                        //                          }]}

                        object resultJson = JsonConvert.DeserializeObject(result);
                        //JObject responseJson = (JObject)(((Newtonsoft.Json.Linq.JObject)resultJson).Property("response")).Value;
                        JArray array = (JArray)(((Newtonsoft.Json.Linq.JObject)resultJson).Property("response")).Value;

                        List<Language> languageList = new List<Language>();
                        Language language = null;
                        foreach (JObject job in array)
                        {
                            language = new Language();
                            language.LanguageName = job.Property("language").Value.ToString().Replace("\"", "");
                            language.LocalizedName = job.Property("localized_name").Value.ToString().Replace("\"", "");
                            language.Lc = job.Property("lc").Value.ToString().Replace("\"", "");
                            language.UnitType = job.Property("unit_type").Value.ToString().Replace("\"", "");
                            languageList.Add(language);
                        }

                        languageList.Insert(0, new Language() { LanguageName = Properties.Resources.NotSpecified, Lc = string.Empty });
                        objectResult = languageList;
                        break;
                    }
                case ApiMethod.GetLanguagePairs:
                    {
                        //{"opstat":"ok","response":[{
                        //                          "lc_src": "de",
                        //                          "lc_tgt": "en",
                        //                          "tier": "machine",
                        //                          "unit_price": "0.0000"
                        //                          },{
                        //                              //otro language pair
                        //                          }]}

                        object resultJson = JsonConvert.DeserializeObject(result);
                        //JObject responseJson = (JObject)(((Newtonsoft.Json.Linq.JObject)resultJson).Property("response")).Value;
                        JArray array = (JArray)(((Newtonsoft.Json.Linq.JObject)resultJson).Property("response")).Value;

                        List<LanguagePair> languageList = new List<LanguagePair>();
                        LanguagePair languagePair = null;
                        foreach (JObject job in array)
                        {
                            languagePair = new LanguagePair();
                            languagePair.Lc_src = job.Property("lc_src").Value.ToString().Replace("\"", "");
                            languagePair.Lc_tgt = job.Property("lc_tgt").Value.ToString().Replace("\"", "");
                            languagePair.Tier = job.Property("tier").Value.ToString().Replace("\"", "");
                            string unitPrice = job.Property("unit_price").Value.ToString().Replace("\"", "");
                            languagePair.Unit_price = Convert.ToSingle(unitPrice);
                            languageList.Add(languagePair);
                        }

                        objectResult = languageList;
                        break;
                    }
                case ApiMethod.TranslateJob:
                    {
                        //                        {
                        //  "opstat": "ok",
                        //  "response": {
                        //    "jobs": [
                        //      {
                        //        "job1": {
                        //          "job_id": "15723",
                        //          "slug": "Un texto para traducir",
                        //          "body_src": "Un texto",
                        //          "lc_src": "en",
                        //          "lc_tgt": "es",
                        //          "unit_count": "2",
                        //          "tier": "ultra_pro",
                        //          "credits": "0.40",
                        //          "status": "available",
                        //          "eta": "",
                        //          "ctime": 1297743137,
                        //          "callback_url": "",
                        //          "auto_approve": "0",
                        //          "custom_data": "",
                        //          "body_tgt": "Un Texto",
                        //          "mt": 1
                        //        }
                        //      }
                        //    ]
                        //  }
                        //}

                        object resultJson = JsonConvert.DeserializeObject(result);
                        JObject resposeObject = (JObject)(((Newtonsoft.Json.Linq.JObject)resultJson).Property("response")).Value;
                        JArray arrayJobs = (JArray)resposeObject.Property("jobs").Value;
                        JObject job = (JObject)((JObject)arrayJobs[0]).Property("job1").Value;

                        //JArray array = (JArray)(((Newtonsoft.Json.Linq.JObject)resultJson).Property("response")).Value;

                        Job jobClient = null;

                        jobClient = new Job();
                        jobClient.Job_Id = job.Property("job_id").Value.ToString().Replace("\"", "");
                        jobClient.Slug = job.Property("slug").Value.ToString().Replace("\"", "");
                        jobClient.Body_src = job.Property("body_src").Value.ToString().Replace("\"", "");
                        jobClient.Lc_src = job.Property("lc_src").Value.ToString().Replace("\"", "");
                        jobClient.Lc_tgt = job.Property("lc_tgt").Value.ToString().Replace("\"", "");
                        jobClient.Unit_count = Convert.ToInt32(job.Property("unit_count").Value.ToString().Replace("\"", ""));
                        jobClient.Tier = job.Property("tier").Value.ToString().Replace("\"", "");
                        jobClient.Credits = Convert.ToSingle(job.Property("credits").Value.ToString().Replace("\"", ""));
                        jobClient.Status = job.Property("status").Value.ToString().Replace("\"", "");
                        jobClient.Eta = job.Property("eta").Value.ToString().Replace("\"", "");

                        //TODO: Buscar mecasimo para convertir a una fecha váldia!!!
                        string ctimeStr = job.Property("ctime").Value.ToString().Replace("\"", "");
                        jobClient.Ctime = Utils.ConvertFromUnixTimeStamp(Convert.ToDouble(ctimeStr));
                        //jobClient.Custom_data = job.Property("custom_data").Value.ToString();

                        objectResult = jobClient;
                        break;
                    }
                case ApiMethod.CancelJob:
                    {
                        //{"opstat": "ok","response": {}}
                        object resultJson = JsonConvert.DeserializeObject(result);
                        string responseJson = (string)(((Newtonsoft.Json.Linq.JObject)resultJson).Property("opstat")).Value.ToString();
                        bool canceldOK = responseJson == "ok";
                        objectResult = canceldOK;

                        break;
                    }
                case ApiMethod.ViewJob:
                    {
                        //                        {
                        //  "opstat": "ok",
                        //  "response": {
                        //    "job": {
                        //      "job_id": "16673",
                        //      "slug": "aplicaci\\u00f3n",
                        //      "body_src": "Un cami\\u00f3n. Confronted with this situation, most IT professionals chose a middle ground, which forced information workers to make critical security decisions. If a document contained ActiveX controls or macros from an unknown source, users were asked whether they wanted to enable the ActiveX controls or the macros. Users were not allowed to access the document until they answered the question. Although this was not a perfect solution, it did provide a mechanism for mitigating security threats without intruding too much on productivity. The main problem was that most users, when confronted with a security warning, dismissed the warning so they could access the document and get their work done. This was acceptable for low-risk internal documents that did not likely contain malicious content, but it was not acceptable for high-risk external documents that passed through the Internet and could contain malicious content. Unfortunately, users did not usually distinguish between high-risk and low-risk files and treated both files the same way — that is, they accepted the risk and enabled the ActiveX controls and macros.",
                        //      "lc_src": "en",
                        //      "lc_tgt": "es",
                        //      "unit_count": "8",
                        //      "tier": "pro",
                        //      "credits": "0.80",
                        //      "status": "approved",
                        //      "eta": "",
                        //      "ctime": 1298861619,
                        //      "callback_url": "",
                        //      "auto_approve": "0",
                        //      "custom_data": "",
                        //      "body_tgt": "Una aplicaci\\u00f3n. Confronted with this situation, most IT professionals chose a middle ground, which forced information workers to make critical security decisions. If a document contained ActiveX controls or macros from an unknown source, users were asked whether they wanted to enable the ActiveX controls or the macros. Users were not allowed to access the document until they answered the question. Although this was not a perfect solution, it did provide a mechanism for mitigating security threats without intruding too much on productivity. The main problem was that most users, when confronted with a security warning, dismissed the warning so they could access the document and get their work done. This was acceptable for low-risk internal documents that did not likely contain malicious content, but it was not acceptable for high-risk external documents that passed through the Internet and could contain malicious content. Unfortunately, users did not usually distinguish between high-risk and low-risk files and treated both files the same way — that is, they accepted the risk and enabled the ActiveX controls and macros. Confronted with this situation, most IT professionals chose a middle ground, which forced information workers to make critical security decisions. If a document contained ActiveX controls or macros from an unknown source, users were asked whether they wanted to enable the ActiveX controls or the macros. Users were not allowed to access the document until they answered the question. Although this was not a perfect solution, it did provide a mechanism for mitigating security threats without intruding too much on productivity. The main problem was that most users, when confronted with a security warning, dismissed the warning so they could access the document and get their work done. This was acceptable for low-risk internal documents that did not likely contain malicious content, but it was not acceptable for high-risk external documents that passed through the Internet and could contain malicious content. Unfortunately, users did not usually distinguish between high-risk and low-risk files and treated both files the same way — that is, they accepted the risk and enabled the ActiveX controls and macros."
                        //    }
                        //  }
                        //}

                        object resultJson = JsonConvert.DeserializeObject(result);
                        JObject resposeObject = (JObject)(((Newtonsoft.Json.Linq.JObject)resultJson).Property("response")).Value;
                        JObject job = (JObject)((JObject)resposeObject).Property("job").Value;

                        Job jobClient = null;

                        jobClient = new Job();
                        jobClient.Job_Id = job.Property("job_id").Value.ToString().Replace("\"", "");
                        jobClient.Slug = job.Property("slug").Value.ToString().Replace("\"", "");
                        jobClient.Body_src = job.Property("body_src").Value.ToString().Replace("\"", "");
                        if (job.Property("body_tgt") != null && job.Property("body_tgt").Value != null)
                            jobClient.Body_tgt = job.Property("body_tgt").Value.ToString().Replace("\"", "");
                        jobClient.Lc_src = job.Property("lc_src").Value.ToString().Replace("\"", "");
                        jobClient.Lc_tgt = job.Property("lc_tgt").Value.ToString().Replace("\"", "");
                        jobClient.Unit_count = Convert.ToInt32(job.Property("unit_count").Value.ToString().Replace("\"", ""));
                        jobClient.Tier = job.Property("tier").Value.ToString().Replace("\"", "");
                        jobClient.Credits = Convert.ToSingle(job.Property("credits").Value.ToString().Replace("\"", ""));
                        jobClient.Status = job.Property("status").Value.ToString().Replace("\"", "");
                        jobClient.Eta = job.Property("eta").Value.ToString().Replace("\"", "");
                        if (job.Property("captcha_url") != null && job.Property("captcha_url").Value != null)
                            jobClient.CaptchaURL = job.Property("captcha_url").Value.ToString().Replace("\"", "");

                        //TODO: Buscar mecasimo para convertir a una fecha váldia!!!
                        string ctimeStr = job.Property("ctime").Value.ToString().Replace("\"", "");
                        jobClient.Ctime = Utils.ConvertFromUnixTimeStamp(Convert.ToDouble(ctimeStr));
                        //jobClient.Custom_data = job.Property("custom_data").Value.ToString();

                        objectResult = jobClient;
                        break;
                    }
                case ApiMethod.ReviewJob:
                    {
                        // {
                        //  "opstat": "ok",
                        //  "response": {
                        //    "job_id": "16613",	
                        //    "body_src": " aplicaci\\u00f3n. Developing mobile applications used to be an arcane activity pursued by highly specialized developers, but no more. The surge in popularity of Android devices, BlackBerries, and iPhones has application development professionals gearing up to incorporate mobile development into mainstream development processes. The first step in taking mobile development mainstream is defining your strategy. Learn from your peers in consumer product strategy by applying Forrester's POST method to your mobile development efforts. Begin by understanding what types of mobile users you need to support. Next, determine your objectives, and then build a strategy based on your desired offering and level of corporate commitment to mobile. Once you have completed these three steps, then — and only then — should you choose from among the six mobile development styles at your disposal and the vendors that offer mobile platforms and tools that can aid your efforts.",
                        //    "imagePreview": "~/_wpresources/MiguelRa.spGengo/spGengo/asp_mock/preview/jobPreview16613.jpg"
                        //  }
                        //}

                        object resultJson = JsonConvert.DeserializeObject(result);
                        JObject resposeObject = (JObject)(((Newtonsoft.Json.Linq.JObject)resultJson).Property("response")).Value;
                        //JObject job = (JObject)((JObject)resposeObject).Property("job").Value;

                        Job jobClient = null;

                        jobClient = new Job();
                        jobClient.Job_Id = resposeObject.Property("job_id").Value.ToString().Replace("\"", "");

                        jobClient.Body_src = resposeObject.Property("body_src").Value.ToString().Replace("\"", "");
                        jobClient.PreviewImage = resposeObject.Property("imagePreview").Value.ToString().Replace("\"", "");
                        
                        if (resposeObject.Property("captcha_url") != null && resposeObject.Property("captcha_url").Value != null)
                            jobClient.CaptchaURL = resposeObject.Property("captcha_url").Value.ToString().Replace("\"", "");

                        objectResult = jobClient;

                        break;
                    }
                case ApiMethod.ApproveJob:
                    {
                        //{"opstat": "ok","response": {}}
                        object resultJson = JsonConvert.DeserializeObject(result);
                        string responseJson = (string)(((Newtonsoft.Json.Linq.JObject)resultJson).Property("opstat")).Value.ToString();
                        bool canceldOK = responseJson == "ok";
                        objectResult = canceldOK;

                        break;
                    }
                case ApiMethod.CorrectJob:
                    {
                        //{"opstat": "ok","response": {}}
                        object resultJson = JsonConvert.DeserializeObject(result);
                        string responseJson = (string)(((Newtonsoft.Json.Linq.JObject)resultJson).Property("opstat")).Value.ToString();
                        bool canceldOK = responseJson == "ok";
                        objectResult = canceldOK;

                        break;
                    }
                case ApiMethod.RejectJob:
                    {
                        //{"opstat": "ok","response": {}}
                        object resultJson = JsonConvert.DeserializeObject(result);
                        string responseJson = (string)(((Newtonsoft.Json.Linq.JObject)resultJson).Property("opstat")).Value.ToString();
                        bool canceldOK = responseJson == "ok";
                        objectResult = canceldOK;

                        break;
                    }
                default:
                    break;
            }

            return objectResult;
        }

        private string GetFolderPathPreviewImage()
        {
            string specificFolder = string.Format(Properties.Resources.AppDirectory, _specificFolderName);
            string folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + specificFolder + "Previews";
            if (!System.IO.Directory.Exists(folder))
                System.IO.Directory.CreateDirectory(folder);

            return folder;          
        }

        #endregion

        #region GetAccountBalance

        /// <summary>
        /// delegate que representa un método de tipo void que recibe como parámetro un float.
        /// </summary>
        /// <param name="result"></param>
        public delegate void GetAccountBalanceCallBackDelegate(float result);


        /// <summary>
        /// Referencia a un método con la firma al delegado declarado anteriormente. 
        /// que se utiliza para invocar a dicho método cuando la llamada a la API finalizó exitosamente
        /// </summary>
        private GetAccountBalanceCallBackDelegate getAccountBalanceSuccessCallback;

        //public delegate string AsyncMethodCaller(string jobId);

        /// <summary>
        /// Method that is called by wrapper client code used to make de async call to the API
        /// </summary>
        /// <param name="pSuccessCallback">Reference to a method that will be invoked when the async API callBack finish successfully</param>
        /// <param name="pErrorCallback">Reference to a method that will be invoked when the async API callBack retunrs an error</param>
        public void GetAccountBalance(GetAccountBalanceCallBackDelegate pSuccessCallback, ErrorCallBackDelegate pErrorCallback)
        {
            try
            {
                // set callback handlers
                getAccountBalanceSuccessCallback = pSuccessCallback;
                methodErrorCallback = pErrorCallback;

                // create ayns caller object
                GetAccountBalanceMethodCaller caller = null;
                if (!_modeOffline)
                    caller = new GetAccountBalanceMethodCaller(myGengoClient.myGengoClient.Account_BalanceRaw);
                else
                    caller = new GetAccountBalanceMethodCaller(TestMethodOffLineData);

                // call async
                IAsyncResult result = caller.BeginInvoke(new AsyncCallback(GetAccountBalanceCallBack), caller);

            }
            catch (Exception ex)
            {
                ErrorInfo error = new ErrorInfo(ErrorType.AppError, ex.Message);
                methodErrorCallback.Invoke(error);
            }
        }

        /// <summary>
        /// delegate que representa un método que devuelve un string y no recibe ningún paraámetro 
        /// </summary>
        /// <returns></returns>
        public delegate string GetAccountBalanceMethodCaller();

        // async callback method

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ar"></param>
        private void GetAccountBalanceCallBack(IAsyncResult ar)
        {
            try
            {
                // get result
                GetAccountBalanceMethodCaller caller = (GetAccountBalanceMethodCaller)ar.AsyncState;

                // look for error in response
                string returnValue = caller.EndInvoke(ar);
                ErrorInfo error = CheckForError(returnValue);
                if (error != null)
                {
                    methodErrorCallback.Invoke(error);
                    return;
                }

                // map result to object
                //string returnValue = caller.EndInvoke(ar);
                float objectResult = (float)ConvertResultToObject(returnValue, ApiMethod.GetAccountBalance);

                // call success callback method
                getAccountBalanceSuccessCallback.Invoke(objectResult);
            }
            catch (Exception ex)
            {
                ErrorInfo error = new ErrorInfo(ErrorType.AppError, ex.Message);
                methodErrorCallback.Invoke(error);
            }

        }

        // async callback method offline
        private string TestMethodOffLineData()
        {
            string response = string.Empty;
            // debug?
            bool debug = Convert.ToBoolean(ConfigurationManager.AppSettings["wGengo_debug"]);

            try
            {
                // fake server error
                //throw new NullReferenceException();            

                // fake delay
                int delay = int.Parse(ConfigurationManager.AppSettings["wGengo_delay"]);
                Thread.Sleep(delay);

                // fake response
                // * a fake response is in "[pagename].aspx.txt" file. 
                string parentFolder = AppDomain.CurrentDomain.BaseDirectory;
                string responsePath = parentFolder + "OfflineData\\" + Enum.GetName(typeof(ApiMethod), ApiMethod.GetAccountBalance) + ".txt";
                TextReader textReader = new StreamReader(responsePath);
                response = string.Empty;
                response = textReader.ReadToEnd();
                textReader.Close();

                // fake api error
                //response = "{\"opstat\":\"error\",\"err\":{\"code\":1150,\"msg\":\"\\\"api_key\\\" is a required field\"}}";
                //response = "{\"opstat\":\"error\",\"err\":{\"code\":1150,\"msg\":\"api_key is a required field\"}}";          

                // unescape response
                //response = response.Replace("\\\\", "-");
                response = response.Replace("\\", "");

                // return response
                return response;
            }
            catch (Exception ex)
            {
                // return server error
                string debugInfo = string.Empty;
                if (debug)
                {
                    debugInfo = "Message: " + ex.Message + "<br />" + "Type: " + ex.GetType().Name + "<br />" + "Source: " + ex.Source;
                }
                response = "{\"opstat\" : \"serverError\", \"response\" : \"" + debugInfo + "\"}";

                // unescape response
                response = response.Replace("\\", "");

                // return
                return response;

            }
        }

        #endregion

        #region GetMyJobs

        /// <summary>
        /// delegate que representa un método de tipo void que recibe como parámetro un float.
        /// </summary>
        /// <param name="result"></param>
        public delegate void GetMyJobsCallBackDelegate(List<Job> jobsList);


        /// <summary>
        /// Referencia a un método con la firma al delegado declarado anteriormente. 
        /// que se utiliza para invocar a dicho método cuando la llamada a la API finalizó exitosamente
        /// </summary>
        private GetMyJobsCallBackDelegate getMyJobsSuccessCallback;

        /// <summary>
        /// delegate que representa un método que devuelve un string y no recibe ningún paraámetro 
        /// </summary>
        /// <returns></returns>
        public delegate string GetMyJobsMethodCaller(string pStatus, string pTimespan, string pCount, bool pLazyLoad);

        /// <summary>
        /// Method that is called by wrapper client code used to make de async call to the API
        /// </summary>
        /// <param name="pSuccessCallback">Reference to a method that will be invoked when the async API callBack finish successfully</param>
        /// <param name="pErrorCallback">Reference to a method that will be invoked when the async API callBack retunrs an error</param>
        public void GetMyJobs(GetMyJobsCallBackDelegate pSuccessCallback, ErrorCallBackDelegate pErrorCallback, string pStatus, string pTimespan, string pCount, bool pLazyLoad)
        {
            try
            {
                // set callback handlers
                getMyJobsSuccessCallback = pSuccessCallback;
                methodErrorCallback = pErrorCallback;

                // create async caller object
                GetMyJobsMethodCaller caller = null;
                if (!_modeOffline)
                    caller = new GetMyJobsMethodCaller(myGengoClient.myGengoClient.GetMyJobs);                    
                else
                    caller = new GetMyJobsMethodCaller(GetMyJobsOffLineData);

                string jobCount = ConfigurationManager.AppSettings["wGengo_jobCount"];
                IAsyncResult result = caller.BeginInvoke(pStatus, pTimespan, jobCount, pLazyLoad, new AsyncCallback(GetMyJobsCallBack), caller);
            }
            catch (Exception ex)
            {
                ErrorInfo error = new ErrorInfo(ErrorType.AppError, ex.Message);
                methodErrorCallback.Invoke(error);
            }

        }

        // async callback method

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ar"></param>
        private void GetMyJobsCallBack(IAsyncResult ar)
        {
            try
            {
                // get result
                GetMyJobsMethodCaller caller = (GetMyJobsMethodCaller)ar.AsyncState;

                // look for error in response
                string returnValue = caller.EndInvoke(ar);
                ErrorInfo error = CheckForError(returnValue);
                if (error != null)
                {
                    methodErrorCallback.Invoke(error);
                    return;
                }

                // map result to object

                List<Job> objectResult = (List<Job>)ConvertResultToObject(returnValue, ApiMethod.GetMyJobs);

                // call success callback method
                getMyJobsSuccessCallback.Invoke(objectResult);
            }
            catch (Exception ex)
            {
                ErrorInfo error = new ErrorInfo(ErrorType.AppError, ex.Message);
                methodErrorCallback.Invoke(error);
            }

        }

        // async callback method offline
        private string GetMyJobsOffLineData(string pStatus, string pTimespan, string pCount, bool pLazyLoad)
        {
            string response = string.Empty;
            // debug?
            bool debug = Convert.ToBoolean(ConfigurationManager.AppSettings["wGengo_debug"]);

            try
            {
                // fake server error
                //throw new NullReferenceException();            

                // fake delay
                int delay = int.Parse(ConfigurationManager.AppSettings["wGengo_delay"]);
                Thread.Sleep(delay);

                // fake response
                // * a fake response is in "[pagename].aspx.txt" file. 
                string parentFolder = AppDomain.CurrentDomain.BaseDirectory;
                string responsePath = parentFolder + "OfflineData\\" + Enum.GetName(typeof(ApiMethod), ApiMethod.GetMyJobs) + ".txt";
                TextReader textReader = new StreamReader(responsePath);
                response = string.Empty;
                response = textReader.ReadToEnd();
                textReader.Close();

                // fake api error
                //response = "{\"opstat\":\"error\",\"err\":{\"code\":1150,\"msg\":\"\\\"api_key\\\" is a required field\"}}";
                //response = "{\"opstat\":\"error\",\"err\":{\"code\":1150,\"msg\":\"'api_key' is a required field\"}}";          

                // unescape response
                //response = response.Replace("\\\\", "-");
                response = response.Replace("\\", "");

                // return response
                return response;
            }
            catch (Exception ex)
            {
                // return server error
                string debugInfo = string.Empty;
                if (debug)
                {
                    debugInfo = "Message: " + ex.Message + "<br />" + "Type: " + ex.GetType().Name + "<br />" + "Source: " + ex.Source;
                }
                response = "{\"opstat\" : \"serverError\", \"response\" : \"" + debugInfo + "\"}";

                // unescape response
                response = response.Replace("\\", "");

                // return
                return response;

            }

        }

        #endregion

        #region GetLanguages

        /// <summary>
        /// delegate que representa un método de tipo void que recibe como parámetro un float.
        /// </summary>
        /// <param name="result"></param>
        public delegate void GetLanguagesCallBackDelegate(List<Language> languageList);


        /// <summary>
        /// Referencia a un método con la firma al delegado declarado anteriormente. 
        /// que se utiliza para invocar a dicho método cuando la llamada a la API finalizó exitosamente
        /// </summary>
        private GetLanguagesCallBackDelegate getLanguagesSuccessCallback;

        /// <summary>
        /// delegate que representa un método que devuelve un string y no recibe ningún paraámetro 
        /// </summary>
        /// <returns></returns>
        public delegate string GetLanguagesMethodCaller();

        /// <summary>
        /// Method that is called by wrapper client code used to make de async call to the API
        /// </summary>
        /// <param name="pSuccessCallback">Reference to a method that will be invoked when the async API callBack finish successfully</param>
        /// <param name="pErrorCallback">Reference to a method that will be invoked when the async API callBack retunrs an error</param>
        public void GetLanguages(GetLanguagesCallBackDelegate pSuccessCallback, ErrorCallBackDelegate pErrorCallback)
        {
            try
            {
                // set callback handlers
                getLanguagesSuccessCallback = pSuccessCallback;
                methodErrorCallback = pErrorCallback;

                // create async caller object
                GetLanguagesMethodCaller caller = null;
                if (!_modeOffline)
                    caller = new GetLanguagesMethodCaller(myGengoClient.myGengoClient.Translate_Service_LanguageRaw);
                else
                    caller = new GetLanguagesMethodCaller(GetLanguagesOffLineData);

                string jobCount = ConfigurationManager.AppSettings["wGengo_jobCount"];
                IAsyncResult result = caller.BeginInvoke(new AsyncCallback(GetLanguagesCallBack), caller);
            }
            catch (Exception ex)
            {
                ErrorInfo error = new ErrorInfo(ErrorType.AppError, ex.Message);
                methodErrorCallback.Invoke(error);
            }

        }

        // async callback method

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ar"></param>
        private void GetLanguagesCallBack(IAsyncResult ar)
        {
            try
            {
                // get result
                GetLanguagesMethodCaller caller = (GetLanguagesMethodCaller)ar.AsyncState;

                // look for error in response
                string returnValue = caller.EndInvoke(ar);
                ErrorInfo error = CheckForError(returnValue);
                if (error != null)
                {
                    methodErrorCallback.Invoke(error);
                    return;
                }

                // map result to object            
                List<Language> objectResult = (List<Language>)ConvertResultToObject(returnValue, ApiMethod.GetLanguages);

                // call success callback method
                getLanguagesSuccessCallback.Invoke(objectResult);
            }
            catch (Exception ex)
            {
                ErrorInfo error = new ErrorInfo(ErrorType.AppError, ex.Message);
                methodErrorCallback.Invoke(error);
            }

        }

        // async callback method offline
        private string GetLanguagesOffLineData()
        {
            string response = string.Empty;
            // debug?
            bool debug = Convert.ToBoolean(ConfigurationManager.AppSettings["wGengo_debug"]);

            try
            {
                // fake server error
                //throw new NullReferenceException();            

                // fake delay
                int delay = int.Parse(ConfigurationManager.AppSettings["wGengo_delay"]);
                Thread.Sleep(delay);

                // fake response
                // * a fake response is in "[pagename].aspx.txt" file. 
                string parentFolder = AppDomain.CurrentDomain.BaseDirectory;
                string responsePath = parentFolder + "OfflineData\\" + Enum.GetName(typeof(ApiMethod), ApiMethod.GetLanguages) + ".txt";
                TextReader textReader = new StreamReader(responsePath);
                response = string.Empty;
                response = textReader.ReadToEnd();
                textReader.Close();

                // fake api error
                //response = "{\"opstat\":\"error\",\"err\":{\"code\":1150,\"msg\":\"\\\"api_key\\\" is a required field\"}}";
                //response = "{\"opstat\":\"error\",\"err\":{\"code\":1150,\"msg\":\"'api_key' is a required field\"}}";          

                // unescape response
                //response = response.Replace("\\\\", "-");
                response = response.Replace("\\", "");

                // return response
                return response;
            }
            catch (Exception ex)
            {
                // return server error
                string debugInfo = string.Empty;
                if (debug)
                {
                    debugInfo = "Message: " + ex.Message + "<br />" + "Type: " + ex.GetType().Name + "<br />" + "Source: " + ex.Source;
                }
                response = "{\"opstat\" : \"serverError\", \"response\" : \"" + debugInfo + "\"}";

                // unescape response
                response = response.Replace("\\", "");

                // return
                return response;

            }

        }

        #endregion

        #region GetLanguages Pairs

        /// <summary>
        /// delegate que representa un método de tipo void que recibe como parámetro un float.
        /// </summary>
        /// <param name="result"></param>
        public delegate void GetLanguagePairsCallBackDelegate(List<LanguagePair> languagePairs);


        /// <summary>
        /// Referencia a un método con la firma al delegado declarado anteriormente. 
        /// que se utiliza para invocar a dicho método cuando la llamada a la API finalizó exitosamente
        /// </summary>
        private GetLanguagePairsCallBackDelegate getLanguagePairsSuccessCallback;

        /// <summary>
        /// delegate que representa un método que devuelve un string y no recibe ningún paraámetro 
        /// </summary>
        /// <returns></returns>
        public delegate string GetLanguagePairsMethodCaller();

        /// <summary>
        /// Method that is called by wrapper client code used to make de async call to the API
        /// </summary>
        /// <param name="pSuccessCallback">Reference to a method that will be invoked when the async API callBack finish successfully</param>
        /// <param name="pErrorCallback">Reference to a method that will be invoked when the async API callBack retunrs an error</param>
        public void GetLanguagePairs(GetLanguagePairsCallBackDelegate pSuccessCallback, ErrorCallBackDelegate pErrorCallback)
        {
            try
            {
                // set callback handlers
                getLanguagePairsSuccessCallback = pSuccessCallback;
                methodErrorCallback = pErrorCallback;

                // create async caller object
                GetLanguagePairsMethodCaller caller = null;
                if (!_modeOffline)
                    caller = new GetLanguagePairsMethodCaller(myGengoClient.myGengoClient.Translate_Service_LanguagePairs);
                else
                    caller = new GetLanguagePairsMethodCaller(GetLanguagePairsOffLineData);

                string jobCount = ConfigurationManager.AppSettings["wGengo_jobCount"];
                IAsyncResult result = caller.BeginInvoke(new AsyncCallback(GetLanguagePairsCallBack), caller);
            }
            catch (Exception ex)
            {
                ErrorInfo error = new ErrorInfo(ErrorType.AppError, ex.Message);
                methodErrorCallback.Invoke(error);
            }

        }

        // async callback method

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ar"></param>
        private void GetLanguagePairsCallBack(IAsyncResult ar)
        {
            try
            {
                // get result
                GetLanguagePairsMethodCaller caller = (GetLanguagePairsMethodCaller)ar.AsyncState;

                // look for error in response
                string returnValue = caller.EndInvoke(ar);
                ErrorInfo error = CheckForError(returnValue);
                if (error != null)
                {
                    methodErrorCallback.Invoke(error);
                    return;
                }

                // map result to object            
                List<LanguagePair> objectResult = (List<LanguagePair>)ConvertResultToObject(returnValue, ApiMethod.GetLanguagePairs);

                // call success callback method
                getLanguagePairsSuccessCallback.Invoke(objectResult);
            }
            catch (Exception ex)
            {
                ErrorInfo error = new ErrorInfo(ErrorType.AppError, ex.Message);
                methodErrorCallback.Invoke(error);
            }

        }

        // async callback method offline
        private string GetLanguagePairsOffLineData()
        {
            string response = string.Empty;
            // debug?
            bool debug = Convert.ToBoolean(ConfigurationManager.AppSettings["wGengo_debug"]);

            try
            {
                // fake server error
                //throw new NullReferenceException();            

                // fake delay
                int delay = int.Parse(ConfigurationManager.AppSettings["wGengo_delay"]);
                Thread.Sleep(delay);

                // fake response
                // * a fake response is in "[pagename].aspx.txt" file. 
                string parentFolder = AppDomain.CurrentDomain.BaseDirectory;
                string responsePath = parentFolder + "OfflineData\\" + Enum.GetName(typeof(ApiMethod), ApiMethod.GetLanguagePairs) + ".txt";
                TextReader textReader = new StreamReader(responsePath);
                response = string.Empty;
                response = textReader.ReadToEnd();
                textReader.Close();

                // fake api error
                //response = "{\"opstat\":\"error\",\"err\":{\"code\":1150,\"msg\":\"\\\"api_key\\\" is a required field\"}}";
                //response = "{\"opstat\":\"error\",\"err\":{\"code\":1150,\"msg\":\"'api_key' is a required field\"}}";          

                // unescape response
                //response = response.Replace("\\\\", "-");
                response = response.Replace("\\", "");

                // return response
                return response;
            }
            catch (Exception ex)
            {
                // return server error
                string debugInfo = string.Empty;
                if (debug)
                {
                    debugInfo = "Message: " + ex.Message + "<br />" + "Type: " + ex.GetType().Name + "<br />" + "Source: " + ex.Source;
                }
                response = "{\"opstat\" : \"serverError\", \"response\" : \"" + debugInfo + "\"}";

                // unescape response
                response = response.Replace("\\", "");

                // return
                return response;

            }

        }

        #endregion

        #region Translate job

        /// <summary>
        /// delegate que representa un método de tipo void que recibe como parámetro un float.
        /// </summary>
        /// <param name="result"></param>
        public delegate void TranslateJobCallBackDelegate(Job job);


        /// <summary>
        /// Referencia a un método con la firma al delegado declarado anteriormente. 
        /// que se utiliza para invocar a dicho método cuando la llamada a la API finalizó exitosamente
        /// </summary>
        private TranslateJobCallBackDelegate translateJobSuccessCallback;

        /// <summary>
        /// delegate que representa un método que devuelve un string y no recibe ningún paraámetro 
        /// </summary>
        /// <returns></returns>
        public delegate string TranslateJobMethodCaller(string pType, string pSlug, string pBodySrc, string pLc_src, string pLc_tgt, string pTier, string pAutoApprove, string pCustomData, string pComment);

        /// <summary>
        /// Method that is called by wrapper client code used to make de async call to the API
        /// </summary>
        /// <param name="pSuccessCallback">Reference to a method that will be invoked when the async API callBack finish successfully</param>
        /// <param name="pErrorCallback">Reference to a method that will be invoked when the async API callBack retunrs an error</param>
        public void TranslateJob(TranslateJobCallBackDelegate pSuccessCallback, ErrorCallBackDelegate pErrorCallback, string pType, string pSlug, string pBodySrc, string pLc_src, string pLc_tgt, string pTier, string pAutoApprove, string pCustomData, string pComment)
        {
            try
            {
                // set callback handlers
                translateJobSuccessCallback = pSuccessCallback;
                methodErrorCallback = pErrorCallback;

                // create async caller object
                TranslateJobMethodCaller caller = null;
                if (!_modeOffline)
                    caller = new TranslateJobMethodCaller(myGengoClient.myGengoClient.Translate_JobsRaw);
                else
                    caller = new TranslateJobMethodCaller(TranslateJobOffLineData);

                IAsyncResult result = caller.BeginInvoke(pType, pSlug, pBodySrc, pLc_src, pLc_tgt, pTier, pAutoApprove, pCustomData, pComment, new AsyncCallback(TranslateJobCallBack), caller);
            }
            catch (Exception ex)
            {
                ErrorInfo error = new ErrorInfo(ErrorType.AppError, ex.Message);
                methodErrorCallback.Invoke(error);
            }
        }

        // async callback method

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ar"></param>
        private void TranslateJobCallBack(IAsyncResult ar)
        {
            try
            {
                // get result
                TranslateJobMethodCaller caller = (TranslateJobMethodCaller)ar.AsyncState;

                // look for error in response
                string returnValue = caller.EndInvoke(ar);
                ErrorInfo error = CheckForError(returnValue);
                if (error != null)
                {
                    methodErrorCallback.Invoke(error);
                    return;
                }

                // map result to object

                Job objectResult = (Job)ConvertResultToObject(returnValue, ApiMethod.TranslateJob);

                // call success callback method
                translateJobSuccessCallback.Invoke(objectResult);
            }
            catch (Exception ex)
            {
                ErrorInfo error = new ErrorInfo(ErrorType.AppError, ex.Message);
                methodErrorCallback.Invoke(error);
            }
        }

        // async callback method offline
        private string TranslateJobOffLineData(string pType, string pSlug, string pBodySrc, string pLc_src, string pLc_tgt, string pTier, string pAutoApprove, string pCustomData, string pComment)
        {
            string response = string.Empty;
            // debug?
            bool debug = Convert.ToBoolean(ConfigurationManager.AppSettings["wGengo_debug"]);

            try
            {
                // fake server error
                //throw new NullReferenceException();            

                // fake delay
                int delay = int.Parse(ConfigurationManager.AppSettings["wGengo_delay"]);
                Thread.Sleep(delay);

                // fake response
                // * a fake response is in "[pagename].aspx.txt" file. 
                string parentFolder = AppDomain.CurrentDomain.BaseDirectory;
                string responsePath = parentFolder + "OfflineData\\" + Enum.GetName(typeof(ApiMethod), ApiMethod.TranslateJob) + ".txt";
                TextReader textReader = new StreamReader(responsePath);
                response = string.Empty;
                response = textReader.ReadToEnd();
                textReader.Close();

                // fake api error
                //response = "{\"opstat\":\"error\",\"err\":{\"code\":1150,\"msg\":\"\\\"api_key\\\" is a required field\"}}";
                //response = "{\"opstat\":\"error\",\"err\":{\"code\":1150,\"msg\":\"'api_key' is a required field\"}}";          

                // unescape response
                //response = response.Replace("\\\\", "-");
                response = response.Replace("\\", "");

                // return response
                return response;
            }
            catch (Exception ex)
            {
                // return server error
                string debugInfo = string.Empty;
                if (debug)
                {
                    debugInfo = "Message: " + ex.Message + "<br />" + "Type: " + ex.GetType().Name + "<br />" + "Source: " + ex.Source;
                }
                response = "{\"opstat\" : \"serverError\", \"response\" : \"" + debugInfo + "\"}";

                // unescape response
                response = response.Replace("\\", "");

                // return
                return response;

            }

        }

        #endregion

        #region Cancel job

        /// <summary>
        /// delegate que representa un método de tipo void que recibe como parámetro un float.
        /// </summary>
        /// <param name="result"></param>
        public delegate void CancelJobCallBackDelegate(bool pJobCanceled);


        /// <summary>
        /// Referencia a un método con la firma al delegado declarado anteriormente. 
        /// que se utiliza para invocar a dicho método cuando la llamada a la API finalizó exitosamente
        /// </summary>
        private CancelJobCallBackDelegate cancelJobSuccessCallback;

        /// <summary>
        /// delegate que representa un método que devuelve un string y no recibe ningún paraámetro 
        /// </summary>
        /// <returns></returns>
        public delegate string CancelJobMethodCaller(string pJob_Id);

        /// <summary>
        /// Method that is called by wrapper client code used to make de async call to the API
        /// </summary>
        /// <param name="pSuccessCallback">Reference to a method that will be invoked when the async API callBack finish successfully</param>
        /// <param name="pErrorCallback">Reference to a method that will be invoked when the async API callBack retunrs an error</param>
        public void CancelJob(CancelJobCallBackDelegate pSuccessCallback, ErrorCallBackDelegate pErrorCallback, string pJob_Id)
        {
            try
            {
                // set callback handlers
                cancelJobSuccessCallback = pSuccessCallback;
                methodErrorCallback = pErrorCallback;

                // create async caller object
                CancelJobMethodCaller caller = null;
                if (!_modeOffline)
                    caller = new CancelJobMethodCaller(myGengoClient.myGengoClient.TranslateJobCancel);
                else
                    caller = new CancelJobMethodCaller(CancelJobOffLineData);

                IAsyncResult result = caller.BeginInvoke(pJob_Id, new AsyncCallback(CancelJobCallBack), caller);
            }
            catch (Exception ex)
            {
                ErrorInfo error = new ErrorInfo(ErrorType.AppError, ex.Message);
                methodErrorCallback.Invoke(error);
            }
        }

        // async callback method

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ar"></param>
        private void CancelJobCallBack(IAsyncResult ar)
        {
            try
            {
                // get result
                CancelJobMethodCaller caller = (CancelJobMethodCaller)ar.AsyncState;

                // look for error in response
                string returnValue = caller.EndInvoke(ar);
                ErrorInfo error = CheckForError(returnValue);
                if (error != null)
                {
                    methodErrorCallback.Invoke(error);
                    return;
                }

                // map result to object

                bool objectResult = (bool)ConvertResultToObject(returnValue, ApiMethod.CancelJob);

                // call success callback method
                cancelJobSuccessCallback.Invoke(objectResult);
            }
            catch (Exception ex)
            {
                ErrorInfo error = new ErrorInfo(ErrorType.AppError, ex.Message);
                methodErrorCallback.Invoke(error);
            }
        }

        // async callback method offline
        private string CancelJobOffLineData(string pJob_Id)
        {
            string response = string.Empty;
            // debug?
            bool debug = Convert.ToBoolean(ConfigurationManager.AppSettings["wGengo_debug"]);

            try
            {
                // fake server error
                //throw new NullReferenceException();            

                // fake delay
                int delay = int.Parse(ConfigurationManager.AppSettings["wGengo_delay"]);
                Thread.Sleep(delay);

                // fake response
                // * a fake response is in "[pagename].aspx.txt" file. 
                string parentFolder = AppDomain.CurrentDomain.BaseDirectory;
                string responsePath = parentFolder + "OfflineData\\" + Enum.GetName(typeof(ApiMethod), ApiMethod.CancelJob) + ".txt";
                TextReader textReader = new StreamReader(responsePath);
                response = string.Empty;
                response = textReader.ReadToEnd();
                textReader.Close();

                // fake api error
                //response = "{\"opstat\":\"error\",\"err\":{\"code\":1150,\"msg\":\"\\\"api_key\\\" is a required field\"}}";
                //response = "{\"opstat\":\"error\",\"err\":{\"code\":1150,\"msg\":\"'api_key' is a required field\"}}";          

                // unescape response
                //response = response.Replace("\\\\", "-");
                response = response.Replace("\\", "");

                // return response
                return response;
            }
            catch (Exception ex)
            {
                // return server error
                string debugInfo = string.Empty;
                if (debug)
                {
                    debugInfo = "Message: " + ex.Message + "<br />" + "Type: " + ex.GetType().Name + "<br />" + "Source: " + ex.Source;
                }
                response = "{\"opstat\" : \"serverError\", \"response\" : \"" + debugInfo + "\"}";

                // unescape response
                response = response.Replace("\\", "");

                // return
                return response;

            }

        }

        #endregion

        #region View job

        /// <summary>
        /// delegate que representa un método de tipo void que recibe como parámetro un float.
        /// </summary>
        /// <param name="result"></param>
        public delegate void ViewJobCallBackDelegate(Job pJob);


        /// <summary>
        /// Referencia a un método con la firma al delegado declarado anteriormente. 
        /// que se utiliza para invocar a dicho método cuando la llamada a la API finalizó exitosamente
        /// </summary>
        private ViewJobCallBackDelegate viewJobSuccessCallback;

        /// <summary>
        /// delegate que representa un método que devuelve un string y no recibe ningún paraámetro 
        /// </summary>
        /// <returns></returns>
        public delegate string ViewJobMethodCaller(string pJob_Id, string pPre_mt, bool pHTMLEncode);

        /// <summary>
        /// Method that is called by wrapper client code used to make de async call to the API
        /// </summary>
        /// <param name="pSuccessCallback">Reference to a method that will be invoked when the async API callBack finish successfully</param>
        /// <param name="pErrorCallback">Reference to a method that will be invoked when the async API callBack retunrs an error</param>
        public void ViewJob(ViewJobCallBackDelegate pSuccessCallback, ErrorCallBackDelegate pErrorCallback, string pJob_Id, string pPre_mt)
        {
            try
            {
                // set callback handlers
                viewJobSuccessCallback = pSuccessCallback;
                methodErrorCallback = pErrorCallback;

                // create async caller object
                ViewJobMethodCaller caller = null;
                if (!_modeOffline)
                    caller = new ViewJobMethodCaller(myGengoClient.myGengoClient.TranslateJobsGetByIdRaw);
                else
                    caller = new ViewJobMethodCaller(ViewJobOffLineData);

                bool pHTMLEncode = false;
                IAsyncResult result = caller.BeginInvoke(pJob_Id, pPre_mt, pHTMLEncode, new AsyncCallback(ViewJobCallBack), caller);
            }
            catch (Exception ex)
            {
                ErrorInfo error = new ErrorInfo(ErrorType.AppError, ex.Message);
                methodErrorCallback.Invoke(error);
            }
        }

        // async callback method

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ar"></param>
        private void ViewJobCallBack(IAsyncResult ar)
        {
            // get result
            ViewJobMethodCaller caller = (ViewJobMethodCaller)ar.AsyncState;

            // look for error in response
            string returnValue = caller.EndInvoke(ar);
            ErrorInfo error = CheckForError(returnValue);
            if (error != null)
            {
                methodErrorCallback.Invoke(error);
                return;
            }

            // map result to object

            Job objectResult = (Job)ConvertResultToObject(returnValue, ApiMethod.ViewJob);

            // call success callback method
            viewJobSuccessCallback.Invoke(objectResult);

        }

        // async callback method offline
        private string ViewJobOffLineData(string pJob_Id, string pPre_mt, bool pHTMLEncode)
        {
            string response = string.Empty;
            // debug?
            bool debug = Convert.ToBoolean(ConfigurationManager.AppSettings["wGengo_debug"]);

            try
            {
                // fake server error
                //throw new NullReferenceException();            

                // fake delay
                int delay = int.Parse(ConfigurationManager.AppSettings["wGengo_delay"]);
                Thread.Sleep(delay);

                // fake response
                // * a fake response is in "[pagename].aspx.txt" file. 
                string parentFolder = AppDomain.CurrentDomain.BaseDirectory;
                string responsePath = parentFolder + "OfflineData\\" + Enum.GetName(typeof(ApiMethod), ApiMethod.ViewJob) + ".txt";
                TextReader textReader = new StreamReader(responsePath);
                response = string.Empty;
                response = textReader.ReadToEnd();
                textReader.Close();

                // fake api error
                //response = "{\"opstat\":\"error\",\"err\":{\"code\":1150,\"msg\":\"\\\"api_key\\\" is a required field\"}}";
                //response = "{\"opstat\":\"error\",\"err\":{\"code\":1150,\"msg\":\"'api_key' is a required field\"}}";          

                // unescape response
                //response = response.Replace("\\\\", "-");
                response = response.Replace("\\", "");

                // return response
                return response;
            }
            catch (Exception ex)
            {
                // return server error
                string debugInfo = string.Empty;
                if (debug)
                {
                    debugInfo = "Message: " + ex.Message + "<br />" + "Type: " + ex.GetType().Name + "<br />" + "Source: " + ex.Source;
                }
                response = "{\"opstat\" : \"serverError\", \"response\" : \"" + debugInfo + "\"}";

                // unescape response
                response = response.Replace("\\", "");

                // return
                return response;
            }
        }

        #endregion

        #region Review job

        /// <summary>
        /// delegate que representa un método de tipo void que recibe como parámetro un float.
        /// </summary>
        /// <param name="result"></param>
        public delegate void ReviewJobCallBackDelegate(Job pJob);


        /// <summary>
        /// Referencia a un método con la firma al delegado declarado anteriormente. 
        /// que se utiliza para invocar a dicho método cuando la llamada a la API finalizó exitosamente
        /// </summary>
        private ReviewJobCallBackDelegate reviewJobSuccessCallback;

        /// <summary>
        /// delegate que representa un método que devuelve un string y no recibe ningún paraámetro 
        /// </summary>
        /// <returns></returns>
        public delegate string ReviewJobMethodCaller(string pJob_Id, string pImagePath, string pImagePathPrefix);

        /// <summary>
        /// Method that is called by wrapper client code used to make de async call to the API
        /// </summary>
        /// <param name="pSuccessCallback">Reference to a method that will be invoked when the async API callBack finish successfully</param>
        /// <param name="pErrorCallback">Reference to a method that will be invoked when the async API callBack retunrs an error</param>
        public void ReviewJob(ReviewJobCallBackDelegate pSuccessCallback, ErrorCallBackDelegate pErrorCallback, string pJob_Id)
        {
            try
            {
                // set callback handlers
                reviewJobSuccessCallback = pSuccessCallback;
                methodErrorCallback = pErrorCallback;

                // create async caller object
                ReviewJobMethodCaller caller = null;
                if (!_modeOffline)
                    caller = new ReviewJobMethodCaller(myGengoClient.myGengoClient.GetJobPreview);
                else
                    caller = new ReviewJobMethodCaller(ReviewJobOffLineData);

                //string specificFolder = string.Format(Properties.Resources.AppDirectory, _specificFolderName);
                //string folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + specificFolder + "Previews";
                //if (!System.IO.Directory.Exists(folder))
                //    System.IO.Directory.CreateDirectory(folder);

                string folder = GetFolderPathPreviewImage();
                string impagePath = string.Format("{0}/preview{1}.jpg", folder, pJob_Id);
                string imagePathPrefix = "";

                IAsyncResult result = caller.BeginInvoke(pJob_Id, impagePath, imagePathPrefix, new AsyncCallback(ReviewJobCallBack), caller);
            }
            catch (Exception ex)
            {
                ErrorInfo error = new ErrorInfo(ErrorType.AppError, ex.Message);
                methodErrorCallback.Invoke(error);
            }
        }

        // async callback method

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ar"></param>
        private void ReviewJobCallBack(IAsyncResult ar)
        {
            try
            {
                // get result
                ReviewJobMethodCaller caller = (ReviewJobMethodCaller)ar.AsyncState;

                // look for error in response
                string returnValue = caller.EndInvoke(ar);
                ErrorInfo error = CheckForError(returnValue);
                if (error != null)
                {
                    methodErrorCallback.Invoke(error);
                    return;
                }

                // map result to object
                Job objectResult = (Job)ConvertResultToObject(returnValue, ApiMethod.ReviewJob);

                string imagePath;
                string previewfolder;
                string captchaPath;
                string captchaFolder;

                if (!_modeOffline)
                {
                    //string folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + Properties.Resources.AppDirectory + "Previews";
                    //if (!System.IO.Directory.Exists(folder))
                    //    System.IO.Directory.CreateDirectory(folder);

                    string folder = GetFolderPathPreviewImage();
                    imagePath = string.Format("{0}/preview{1}.jpg", folder, objectResult.Job_Id);                  

                }
                else
                {
                    string parentFolder = AppDomain.CurrentDomain.BaseDirectory + "OfflineData\\";
                    previewfolder = parentFolder + "Previews";
                    imagePath = string.Format("{0}/previewMock.jpg", previewfolder);

                    captchaFolder = parentFolder + "Reject";
                    captchaPath = string.Format("{0}/captchaMock.jpg", captchaFolder);
                    objectResult.CaptchaURL = captchaPath;
                }
                objectResult.PreviewImage = imagePath;

                    // call success callback method
                    reviewJobSuccessCallback.Invoke(objectResult);
            }
            catch (Exception ex)
            {
                ErrorInfo error = new ErrorInfo(ErrorType.AppError, ex.Message);
                methodErrorCallback.Invoke(error);
            }
        }

        // async callback method offline
        private string ReviewJobOffLineData(string pJob_Id, string pImagePath, string pImagePathPrefix)
        {
            string response = string.Empty;
            // debug?
            bool debug = Convert.ToBoolean(ConfigurationManager.AppSettings["wGengo_debug"]);

            try
            {
                // fake server error
                //throw new NullReferenceException();            

                // fake delay
                int delay = int.Parse(ConfigurationManager.AppSettings["wGengo_delay"]);
                Thread.Sleep(delay);

                // fake response
                // * a fake response is in "[pagename].aspx.txt" file. 
                string parentFolder = AppDomain.CurrentDomain.BaseDirectory;
                string responsePath = parentFolder + "OfflineData\\" + Enum.GetName(typeof(ApiMethod), ApiMethod.ReviewJob) + ".txt";
                TextReader textReader = new StreamReader(responsePath);
                response = string.Empty;
                response = textReader.ReadToEnd();
                textReader.Close();

                // fake api error
                //response = "{\"opstat\":\"error\",\"err\":{\"code\":1150,\"msg\":\"\\\"api_key\\\" is a required field\"}}";
                //response = "{\"opstat\":\"error\",\"err\":{\"code\":1150,\"msg\":\"'api_key' is a required field\"}}";          

                // unescape response
                //response = response.Replace("\\\\", "-");
                response = response.Replace("\\", "");

                // return response
                return response;
            }
            catch (Exception ex)
            {
                // return server error
                string debugInfo = string.Empty;
                if (debug)
                {
                    debugInfo = "Message: " + ex.Message + "<br />" + "Type: " + ex.GetType().Name + "<br />" + "Source: " + ex.Source;
                }
                response = "{\"opstat\" : \"serverError\", \"response\" : \"" + debugInfo + "\"}";

                // unescape response
                response = response.Replace("\\", "");

                // return
                return response;
            }
        }

        #endregion

        #region Approve job

        /// <summary>
        /// delegate que representa un método de tipo void que recibe como parámetro un float.
        /// </summary>
        /// <param name="result"></param>
        public delegate void ApproveJobCallBackDelegate(bool jobApproved);


        /// <summary>
        /// Referencia a un método con la firma al delegado declarado anteriormente. 
        /// que se utiliza para invocar a dicho método cuando la llamada a la API finalizó exitosamente
        /// </summary>
        private ApproveJobCallBackDelegate approveJobSuccessCallback;

        /// <summary>
        /// delegate que representa un método que devuelve un string y no recibe ningún paraámetro 
        /// </summary>
        /// <returns></returns>
        public delegate string ApproveJobMethodCaller(string pJob_Id, string pRating);

        /// <summary>
        /// Method that is called by wrapper client code used to make de async call to the API
        /// </summary>
        /// <param name="pSuccessCallback">Reference to a method that will be invoked when the async API callBack finish successfully</param>
        /// <param name="pErrorCallback">Reference to a method that will be invoked when the async API callBack retunrs an error</param>
        public void ApproveJob(ApproveJobCallBackDelegate pSuccessCallback, ErrorCallBackDelegate pErrorCallback, string pJob_Id, string pRating)
        {
            try
            {
                // set callback handlers
                approveJobSuccessCallback = pSuccessCallback;
                methodErrorCallback = pErrorCallback;

                // create async caller object
                ApproveJobMethodCaller caller = null;
                if (!_modeOffline)
                    caller = new ApproveJobMethodCaller(myGengoClient.myGengoClient.TranslateJobApprove);
                else
                    caller = new ApproveJobMethodCaller(ApproveJobOffLineData);

                IAsyncResult result = caller.BeginInvoke(pJob_Id, pRating, new AsyncCallback(ApproveJobCallBack), caller);
            }
            catch (Exception ex)
            {
                ErrorInfo error = new ErrorInfo(ErrorType.AppError, ex.Message);
                methodErrorCallback.Invoke(error);
            }
        }

        // async callback method

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ar"></param>
        private void ApproveJobCallBack(IAsyncResult ar)
        {
            try
            {
                // get result
                ApproveJobMethodCaller caller = (ApproveJobMethodCaller)ar.AsyncState;

                // look for error in response
                string returnValue = caller.EndInvoke(ar);
                ErrorInfo error = CheckForError(returnValue);
                if (error != null)
                {
                    methodErrorCallback.Invoke(error);
                    return;
                }

                // map result to object

                bool objectResult = (bool)ConvertResultToObject(returnValue, ApiMethod.ApproveJob);

                // call success callback method
                approveJobSuccessCallback.Invoke(objectResult);
            }
            catch (Exception ex)
            {
                ErrorInfo error = new ErrorInfo(ErrorType.AppError, ex.Message);
                methodErrorCallback.Invoke(error);
            }
        }

        // async callback method offline
        private string ApproveJobOffLineData(string pJob_Id, string pRating)
        {
            string response = string.Empty;
            // debug?
            bool debug = Convert.ToBoolean(ConfigurationManager.AppSettings["wGengo_debug"]);

            try
            {
                // fake server error
                //throw new NullReferenceException();            

                // fake delay
                int delay = int.Parse(ConfigurationManager.AppSettings["wGengo_delay"]);
                Thread.Sleep(delay);

                // fake response
                // * a fake response is in "[pagename].aspx.txt" file. 
                string parentFolder = AppDomain.CurrentDomain.BaseDirectory;
                string responsePath = parentFolder + "OfflineData\\" + Enum.GetName(typeof(ApiMethod), ApiMethod.ApproveJob) + ".txt";
                TextReader textReader = new StreamReader(responsePath);
                response = string.Empty;
                response = textReader.ReadToEnd();
                textReader.Close();

                // fake api error
                //response = "{\"opstat\":\"error\",\"err\":{\"code\":1150,\"msg\":\"\\\"api_key\\\" is a required field\"}}";
                //response = "{\"opstat\":\"error\",\"err\":{\"code\":1150,\"msg\":\"'api_key' is a required field\"}}";          

                // unescape response
                //response = response.Replace("\\\\", "-");
                response = response.Replace("\\", "");

                // return response
                return response;
            }
            catch (Exception ex)
            {
                // return server error
                string debugInfo = string.Empty;
                if (debug)
                {
                    debugInfo = "Message: " + ex.Message + "<br />" + "Type: " + ex.GetType().Name + "<br />" + "Source: " + ex.Source;
                }
                response = "{\"opstat\" : \"serverError\", \"response\" : \"" + debugInfo + "\"}";

                // unescape response
                response = response.Replace("\\", "");

                // return
                return response;
            }
        }

        #endregion

        #region Correct job

        /// <summary>
        /// delegate que representa un método de tipo void que recibe como parámetro un float.
        /// </summary>
        /// <param name="result"></param>
        public delegate void CorrectJobCallBackDelegate(bool requestAcepted);


        /// <summary>
        /// Referencia a un método con la firma al delegado declarado anteriormente. 
        /// que se utiliza para invocar a dicho método cuando la llamada a la API finalizó exitosamente
        /// </summary>
        private CorrectJobCallBackDelegate correctJobSuccessCallback;

        /// <summary>
        /// delegate que representa un método que devuelve un string y no recibe ningún paraámetro 
        /// </summary>
        /// <returns></returns>
        public delegate string CorrectJobMethodCaller(string pJob_Id, string pRequestCorrection);

        /// <summary>
        /// Method that is called by wrapper client code used to make de async call to the API
        /// </summary>
        /// <param name="pSuccessCallback">Reference to a method that will be invoked when the async API callBack finish successfully</param>
        /// <param name="pErrorCallback">Reference to a method that will be invoked when the async API callBack retunrs an error</param>
        public void CorrectJob(CorrectJobCallBackDelegate pSuccessCallback, ErrorCallBackDelegate pErrorCallback, string pJob_Id, string pRequestCorrection)
        {
            try
            {
                // set callback handlers
                correctJobSuccessCallback = pSuccessCallback;
                methodErrorCallback = pErrorCallback;

                // create async caller object
                CorrectJobMethodCaller caller = null;
                if (!_modeOffline)
                    caller = new CorrectJobMethodCaller(myGengoClient.myGengoClient.RequestCorrection);
                else
                    caller = new CorrectJobMethodCaller(CorrectJobOffLineData);

                IAsyncResult result = caller.BeginInvoke(pJob_Id, pRequestCorrection, new AsyncCallback(CorrectJobCallBack), caller);
            }
            catch (Exception ex)
            {
                ErrorInfo error = new ErrorInfo(ErrorType.AppError, ex.Message);
                methodErrorCallback.Invoke(error);
            }
        }

        // async callback method

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ar"></param>
        private void CorrectJobCallBack(IAsyncResult ar)
        {
            try
            {
                // get result
                CorrectJobMethodCaller caller = (CorrectJobMethodCaller)ar.AsyncState;

                // look for error in response
                string returnValue = caller.EndInvoke(ar);
                ErrorInfo error = CheckForError(returnValue);
                if (error != null)
                {
                    methodErrorCallback.Invoke(error);
                    return;
                }
                // map result to object

                bool objectResult = (bool)ConvertResultToObject(returnValue, ApiMethod.CorrectJob);

                // call success callback method
                correctJobSuccessCallback.Invoke(objectResult);
            }
            catch (Exception ex)
            {
                ErrorInfo error = new ErrorInfo(ErrorType.AppError, ex.Message);
                methodErrorCallback.Invoke(error);
            }
        }

        // async callback method offline
        private string CorrectJobOffLineData(string pJob_Id, string pRating)
        {
            string response = string.Empty;
            // debug?
            bool debug = Convert.ToBoolean(ConfigurationManager.AppSettings["wGengo_debug"]);

            try
            {
                // fake server error
                //throw new NullReferenceException();            

                // fake delay
                int delay = int.Parse(ConfigurationManager.AppSettings["wGengo_delay"]);
                Thread.Sleep(delay);

                // fake response
                // * a fake response is in "[pagename].aspx.txt" file. 
                string parentFolder = AppDomain.CurrentDomain.BaseDirectory;
                string responsePath = parentFolder + "OfflineData\\" + Enum.GetName(typeof(ApiMethod), ApiMethod.CorrectJob) + ".txt";
                TextReader textReader = new StreamReader(responsePath);
                response = string.Empty;
                response = textReader.ReadToEnd();
                textReader.Close();

                // fake api error
                //response = "{\"opstat\":\"error\",\"err\":{\"code\":1150,\"msg\":\"\\\"api_key\\\" is a required field\"}}";
                //response = "{\"opstat\":\"error\",\"err\":{\"code\":1150,\"msg\":\"'api_key' is a required field\"}}";          

                // unescape response
                //response = response.Replace("\\\\", "-");
                response = response.Replace("\\", "");

                // return response
                return response;
            }
            catch (Exception ex)
            {
                // return server error
                string debugInfo = string.Empty;
                if (debug)
                {
                    debugInfo = "Message: " + ex.Message + "<br />" + "Type: " + ex.GetType().Name + "<br />" + "Source: " + ex.Source;
                }
                response = "{\"opstat\" : \"serverError\", \"response\" : \"" + debugInfo + "\"}";

                // unescape response
                response = response.Replace("\\", "");

                // return
                return response;
            }
        }

        #endregion

        #region Reject job

        /// <summary>
        /// delegate que representa un método de tipo void que recibe como parámetro un float.
        /// </summary>
        /// <param name="result"></param>
        public delegate void RejectJobCallBackDelegate(bool jobRejected);


        /// <summary>
        /// Referencia a un método con la firma al delegado declarado anteriormente. 
        /// que se utiliza para invocar a dicho método cuando la llamada a la API finalizó exitosamente
        /// </summary>
        private RejectJobCallBackDelegate rejectJobSuccessCallback;

        /// <summary>
        /// delegate que representa un método que devuelve un string y no recibe ningún paraámetro 
        /// </summary>
        /// <returns></returns>
        public delegate string RejectJobMethodCaller(string pJob_Id, string pReason, string pComment, string pCaptcha, string pFollowUp);

        /// <summary>
        /// Method that is called by wrapper client code used to make de async call to the API
        /// </summary>
        /// <param name="pSuccessCallback">Reference to a method that will be invoked when the async API callBack finish successfully</param>
        /// <param name="pErrorCallback">Reference to a method that will be invoked when the async API callBack retunrs an error</param>
        public void RejectJob(RejectJobCallBackDelegate pSuccessCallback, ErrorCallBackDelegate pErrorCallback, string pJob_Id, string pReason, string pComment, string pCaptcha, string pFollowUp)
        {
            try
            {
                // set callback handlers
                rejectJobSuccessCallback = pSuccessCallback;
                methodErrorCallback = pErrorCallback;

                // create async caller object
                RejectJobMethodCaller caller = null;
                if (!_modeOffline)
                    caller = new RejectJobMethodCaller(myGengoClient.myGengoClient.RejectJob);
                else
                    caller = new RejectJobMethodCaller(RejectJobOffLineData);

                IAsyncResult result = caller.BeginInvoke(pJob_Id, pReason, pComment, pCaptcha, pFollowUp, new AsyncCallback(RejectJobCallBack), caller);
            }
            catch (Exception ex)
            {
                ErrorInfo error = new ErrorInfo(ErrorType.AppError, ex.Message);
                methodErrorCallback.Invoke(error);
            }
        }

        // async callback method

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ar"></param>
        private void RejectJobCallBack(IAsyncResult ar)
        {
            try
            {
                // get result
                RejectJobMethodCaller caller = (RejectJobMethodCaller)ar.AsyncState;

                // look for error in response
                string returnValue = caller.EndInvoke(ar);
                ErrorInfo error = CheckForError(returnValue);
                if (error != null)
                {
                    methodErrorCallback.Invoke(error);
                    return;
                }

                // map result to object

                bool objectResult = (bool)ConvertResultToObject(returnValue, ApiMethod.RejectJob);

                // call success callback method
                rejectJobSuccessCallback.Invoke(objectResult);
            }
            catch (Exception ex)
            {
                ErrorInfo error = new ErrorInfo(ErrorType.AppError, ex.Message);
                methodErrorCallback.Invoke(error);
            }
        }

        // async callback method offline
        private string RejectJobOffLineData(string pJob_Id, string pReason, string pComment, string pCaptcha, string pFollowUp)
        {
            string response = string.Empty;
            // debug?
            bool debug = Convert.ToBoolean(ConfigurationManager.AppSettings["wGengo_debug"]);

            try
            {
                // fake server error
                //throw new NullReferenceException();            

                // fake delay
                int delay = int.Parse(ConfigurationManager.AppSettings["wGengo_delay"]);
                Thread.Sleep(delay);

                // fake response
                // * a fake response is in "[pagename].aspx.txt" file. 
                string parentFolder = AppDomain.CurrentDomain.BaseDirectory;
                string responsePath = parentFolder + "OfflineData\\" + Enum.GetName(typeof(ApiMethod), ApiMethod.CorrectJob) + ".txt";
                TextReader textReader = new StreamReader(responsePath);
                response = string.Empty;
                response = textReader.ReadToEnd();
                textReader.Close();

                // fake api error
                //response = "{\"opstat\":\"error\",\"err\":{\"code\":1150,\"msg\":\"\\\"api_key\\\" is a required field\"}}";
                //response = "{\"opstat\":\"error\",\"err\":{\"code\":1150,\"msg\":\"'api_key' is a required field\"}}";          

                // unescape response
                //response = response.Replace("\\\\", "-");
                response = response.Replace("\\", "");

                // return response
                return response;
            }
            catch (Exception ex)
            {
                // return server error
                string debugInfo = string.Empty;
                if (debug)
                {
                    debugInfo = "Message: " + ex.Message + "<br />" + "Type: " + ex.GetType().Name + "<br />" + "Source: " + ex.Source;
                }
                response = "{\"opstat\" : \"serverError\", \"response\" : \"" + debugInfo + "\"}";

                // unescape response
                response = response.Replace("\\", "");

                // return
                return response;
            }
        }

        #endregion

        #region Initialize

        public bool Initialize(string pPublicKey, string pPrivateKey, string specificFolderName)
        {
            try
            {
                if (string.IsNullOrEmpty(pPrivateKey) || string.IsNullOrEmpty(pPublicKey))
                    return false;

                _specificFolderName = specificFolderName;                
                _privateKey = pPrivateKey;
                _publicKey = pPublicKey;

                string response = string.Empty;
                bool debug = false;

                debug = Convert.ToBoolean(ConfigurationManager.AppSettings["wGengo_debug"]);
                string apiUrl = ConfigurationManager.AppSettings["wGengo_apiUrl"];
                string responseFormat = ConfigurationManager.AppSettings["wGengo_responseFormat"];
                bool useProxy = Convert.ToBoolean(ConfigurationManager.AppSettings["wGengo_useProxy"]);
                string proxyHost = ConfigurationManager.AppSettings["wGengo_proxyHost"];
                int proxyPort = int.Parse(ConfigurationManager.AppSettings["wGengo_proxyPort"]);
                string proxyUsername = ConfigurationManager.AppSettings["wGengo_proxyUsername"];
                string proxyPassword = ConfigurationManager.AppSettings["wGengo_proxyPassword"];

                // initialize api client
                WebProxy proxy = null;
                if (useProxy)
                {
                    proxy = new WebProxy(proxyHost, proxyPort);
                    proxy.Credentials = new NetworkCredential(proxyUsername, proxyPassword);
                }

                myGengoClient.myGengoClient.initialize(apiUrl, _publicKey, _privateKey, responseFormat, proxy);

                return true;
            }
            catch (Exception)
            {
                throw new Exception(Properties.Resources.ErrorInitializinClientWrapper);
            }
        }

        #endregion

        #endregion
    }
}
