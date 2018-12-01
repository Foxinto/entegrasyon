
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Security.Cryptography;

namespace ErsPos
{

    class SslCommerz : BankPos
    {

        /*Gateway URL: https://sandbox.sslcommerz.com/gwprocess/v3/process.php

         Validation API: https://sandbox.sslcommerz.com/validator/api/validationserverAPI.php

         Transaction Status Check API: https://sandbox.sslcommerz.com/validator/api/merchantTransIDvalidationAPI.php

   */

        public string GatewayUrl()  //Dökümandan aldığımız url leri tanımlıyoruz.
        {
            return "https://sandbox.sslcommerz.com/gwprocess/v3/process.php";
        }

        public string ValidationUrl()
        {
            return "https://sandbox.sslcommerz.com/validator/api/validationserverAPI.php";
        }

        public string TransactionUrl()
        {
            return "https://sandbox.sslcommerz.com/validator/api/merchantTransIDvalidationAPI.php";
        }

        public override string CallbackVariableName
        {
            get { return "tran_id"; }
        }
        public override bool MakePayment(PostingData data) //Ödeme işlemini yaptığımız kısım.
        {
            try
            {
                NoneSecure(data);
            }
            finally
            {
                base.MakePayment(data); //en sonunda otomatik log için
            }
            
            return true;
        }

        private void NoneSecure(PostingData payment) 
        {

            string pamount = (Convert.ToDouble(payment.Amount.ToString("###0.00"))).ToString(); //bizden double formatta istemediği için *100 yapmadık.

            NameValueCollection collection = new NameValueCollection(); //dataları giriyoruz.

            
            collection.Add("total_amount", pamount);
            collection.Add("store_id", this.ClientId);
            collection.Add("tran_id", payment.TransactionId.ToString());
            collection.Add("success_url", this.CallbackURL);
            collection.Add("cancel_url", this.CallbackURL);
            collection.Add("fail_url", this.CallbackURL);
            collection.Add("cus_name", payment.CardHolderName);
            collection.Add("cus_email", payment.customEmail);                       
            collection.Add("cus_phone", payment.CardPhoneNumber);
            collection.Add("currency", payment.Currency.ToString());

            payment.PostResult.Result = TransactionResult.InProgress;

            payment.PostResult.Form = this.PreparePOSTForm(this.GatewayUrl(), collection);//burdaki dataları gatewayurl ye form request atıyoruz.
        }

        public override void CheckResponse(PostingData payment, string formValues)//bu bölümde ise attığımız requestlerin responslarını check ediyoruz. 
        {
            try
            {

                NameValueCollection RequestForm = System.Web.HttpUtility.ParseQueryString(formValues);

                String mdstatus = RequestForm.Get("status");
                String verifySign = RequestForm.Get("verify_sign");
                String verifyKey = RequestForm.Get("verify_key");
                String storePassword = this.Password3D;
                //String[] verifyData = verifyKey.Split(',');
                //String data = null;

                //if (verifyKey != null)
                //{
                //    foreach (var item in verifyData)
                //    {
                //        if(RequestForm.Get(item) != null && RequestForm.Get(item) != "")
                //            data += item + "="+ RequestForm.Get(item) + "&";
                //    }
                //}


                //SHA1 sha = new SHA1CryptoServiceProvider();
                //string XHashedPassword = Convert.ToBase64String(sha.ComputeHash(Encoding.UTF8.GetBytes(storePassword)));
                //string hashstr = data;
                //byte[] hashbytes = System.Text.Encoding.GetEncoding("ISO-8859-9").GetBytes(hashstr);
                //byte[] inputbytes = sha.ComputeHash(hashbytes);
                //String hash = Convert.ToBase64String(inputbytes);
                //string XHashData = hash;
                
                //if(XHashData == verifySign)
                //{
                //    payment.PostResult.Result = TransactionResult.InProgress;
                //    payment.PostResult.ErrorMessages.Add("Hash validation success.");

                foreach (var item in RequestForm)
                {
                    payment.PostResult.AddStatus(item.ToString(), RequestForm.Get(item.ToString()));
                }

                if (mdstatus == "VALID")//burda bize dönen response valid ise işlem onaylanmıştır.
                {
                                                              
                    var store_id = RequestForm.Get("store_id");
                    var val_id = RequestForm.Get("val_id");
                    var store_passwd = "test_cholotravel@ssl";

                    var query = @"?val_id=" + val_id + "&store_id=" + store_id + "&store_passwd=" + store_passwd + "&v=1&format=json";

                    try
                    {
                        HttpWebRequest webRequest = (HttpWebRequest)HttpWebRequest.Create(this.ValidationUrl() + query);//Vpos adresi
                        webRequest.Method = "POST";
                        webRequest.ContentType = "application/x-www-form-urlencoded";
                        webRequest.KeepAlive = false;
                        string responseFromServer = "";

                        using (WebResponse webResponse = webRequest.GetResponse())
                        {
                            using (StreamReader reader = new StreamReader(webResponse.GetResponseStream()))
                            {
                                responseFromServer = reader.ReadToEnd();
                                reader.Close();
                            }
                            webResponse.Close();
                        }
                        JObject json = JObject.Parse(responseFromServer);
                        payment.PostResult.Form = json.ToString();

                        var status = json.GetValue("status");
                        var APIConnect = json.GetValue("APIConnect");
                        var riskSeviyesi = json.GetValue("risk_level");
                        var riskTitle = json.GetValue("risk_title");

                        if (APIConnect.ToString() == "DONE")
                        {
                            if (status.ToString() == "VALID" || status.ToString() == "VALIDATED")
                            {
                                payment.PostResult.Result = TransactionResult.Approved;
                                payment.PostResult.Message = "Risk Seviyesi : " + riskSeviyesi.ToString() + " " + riskTitle.ToString();
                            }
                            else
                            {
                                payment.PostResult.Result = TransactionResult.Error;
                                payment.PostResult.Message = "Risk Seviyesi : " + riskSeviyesi.ToString() + " " + riskTitle.ToString();
                                payment.PostResult.ErrorMessages.Add("Payment is not succsesfuly");
                            }
                        }
                        else
                        {
                            payment.PostResult.Result = TransactionResult.Error;
                            payment.PostResult.Message = "Risk Seviyesi : " + riskSeviyesi.ToString() + " " + riskTitle.ToString();
                            payment.PostResult.ErrorMessages.Add("Payment is not succsesfuly");
                        }
                            
                    }
                    catch (Exception ex)
                    {

                        throw ex;
                    }
                   
                    payment.PostResult.AddStatus("risk_level", RequestForm.Get("risk_level"));
                    payment.PostResult.AddStatus("tran_id", RequestForm.Get("tran_id"));
                    payment.PostResult.AddStatus("valid_id", RequestForm.Get("valid_id"));
                    payment.PostResult.AddStatus("store_id", RequestForm.Get("store_id"));
                }
                else if (mdstatus == "FAILED")//burda bize dönen response failed ise işlemde hatalar var.hatalar bize hata mesajı olarak yansır.
                {
                    if (RequestForm.Get("error") != "")
                    {
                        payment.PostResult.AddError(RequestForm.Get("error"));
                    }
                    payment.PostResult.Result = TransactionResult.Error;

                }
                else if (mdstatus == "CANCELLED")
                {

                    payment.PostResult.Result = TransactionResult.Error;

                    payment.PostResult.Message = "Customer canceled";
                }
                //}
                //else
                //{
                //    payment.PostResult.Result = TransactionResult.Error;
                //    payment.PostResult.ErrorMessages.Add("Hash validation failed.");
                //}
            }             
            finally
            {
                base.CheckResponse(payment, formValues); //en sonunda otomatik log için
            }

        }
        
    }
}

  









