using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Xml;
using Newtonsoft.Json;
using Iyzipay.Model;
using Iyzipay.Request;


namespace ErsPos
{
    public class Iyzico : BankPos
    {
        


        public override string CallbackVariableName
        {
            get { return "conversationId"; }
        }

        public override string PostUrl()
        {
            return "https://api.iyzipay.com/payment/3dsecure/initialize";
        }

        public override bool MakePayment(PostingData data)
        {

            try
            {

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                if (this.StoreType == ThreeDType.NoneSecure)
                    IyzicoNoneSecure(data);
                else if (this.StoreType == ThreeDType.ThreeD)
                    IyzicoThreeD(data);
            }
            finally
            {
                base.MakePayment(data);
            }

            return true;
        }
        public override bool VoidPayment(PostingData data)
        {
            var st = this.StoreType;

            var tt = this.TransType;

            try
            {
                this.StoreType = ThreeDType.NoneSecure;

                this.TransType = TransactionBaseType.Void;

                IyzicoNoneSecure(data);
            }
            finally
            {
                base.VoidPayment(data);

                this.StoreType = st;

                this.TransType = tt;
            }

            return true;
        }

        public override bool RefundPayment(PostingData data)
        {
            var st = this.StoreType;

            var tt = this.TransType;

            try
            {
                this.StoreType = ThreeDType.NoneSecure;

                this.TransType = TransactionBaseType.Refund;

                IyzicoNoneSecure(data);
            }
            finally
            {
                base.RefundPayment(data);

                this.StoreType = st;

                this.TransType = tt;
            }

            return true;
        }


        private void IyzicoNoneSecure(PostingData payment)
        {

        }


        private void IyzicoThreeD(PostingData payment)
        {



            string conversationId = payment.TransactionId;
            string price = payment.Amount.ToString("###0.0").Replace(",", ".");
            string paidPrice = payment.Amount.ToString("###0.0").Replace(",", ".");
            string currency = GetCurrencyCode(payment.Currency);
            string installment = payment.Installment;
            string callbackUrl = this.CallbackURL.Contains("&") ? this.CallbackURL.Split('&')[0].Replace("&", ".") : this.CallbackURL;
            string cardNumber = payment.CardNo;
            string expireYear = payment.ExpYear;
            string expireMonth = payment.ExpMonth;
            string cvc = payment.CVV2;
            string cardHolderName = payment.CardHolderName;
            string id = "";
            string name = this.AccountName;
            string surname = this.AccountName;
            string identityNumber = "";
            string city = payment.billingCity;
            string country = payment.billingCountry;
            string email = payment.customEmail;
            string ip = this.UserIPAddress;
            string registrationAddress = payment.billingAddress;
            string contactName = payment.postViewName;
            string address = payment.billingAddress;
            string itemType = "";
            string category1 = "";
            double price1 = payment.Amount / 2;
            string price11 = price1.ToString("###0.0").Replace(",", ".");




            string chars = "abcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[13];
            var r = new Random();

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[r.Next(chars.Length)];
            }
            string xiyzirnd = new String(stringChars);


            string ApiKey = this.Password;
            string secretkey = this.Password3D;


            //string auth = "<?xml version=\"1.0\" encoding=\"UTF-8\" ?>";
            string auth = "<root>";
            auth = auth + "<locale>tr</locale>";
            auth = auth + "<conversationId>" + conversationId + "</conversationId>";
            auth = auth + "<price>" + price + "</price>";
            auth = auth + "<paidPrice>" + paidPrice + "</paidPrice>";
            auth = auth + "<currency>TRY</currency>";
            auth = auth + "<installment>" + installment + "</installment>";
            auth = auth + "<paymentChannel>WEB</paymentChannel>";
            auth = auth + "<basketId>B67832</basketId>";
            auth = auth + "<paymentGroup>PRODUCT</paymentGroup>";
            auth = auth + "<callbackUrl>" + callbackUrl + "</callbackUrl>";
            auth = auth + "<paymentCard>";
            auth = auth + "<cardHolderName>" + cardHolderName + "</cardHolderName>";
            auth = auth + "<cardNumber>" + cardNumber + "</cardNumber>";
            auth = auth + "<expireYear>" + expireYear + "</expireYear>";
            auth = auth + "<expireMonth>" + expireMonth + "</expireMonth>";
            auth = auth + "<cvc>" + cvc + "</cvc>";
            auth = auth + "</paymentCard>";
            auth = auth + "<buyer>";
            auth = auth + "<id>BY789</id>";
            auth = auth + "<name>John</name>";
            auth = auth + "<surname>Doe</surname>";
            auth = auth + "<identityNumber>74300864791</identityNumber>";
            auth = auth + "<email>deneme@edeneme.com</email>";
            auth = auth + "<gsmNumber>+905350000000</gsmNumber>";
            auth = auth + "<registrationDate>2013-04-21 15:12:09</registrationDate>";
            auth = auth + "<lastLoginDate>2015-10-05 12:43:35</lastLoginDate>";
            auth = auth + "<registrationAddress>Nidakule Göztepe, Merdivenköy Mah. Bora Sok. No:1</registrationAddress>";
            auth = auth + "<city>Istanbul</city>";
            auth = auth + "<country>Turkey</country>";
            auth = auth + "<zipCode>34732</zipCode>";
            auth = auth + "<ip>85.34.78.112</ip>";
            auth = auth + "</buyer>";
            auth = auth + "<shippingAddress>";
            auth = auth + "<address>Nidakule Göztepe, Merdivenköy Mah. Bora Sok. No:1</address>";
            auth = auth + "<zipCode>34742</zipCode>";
            auth = auth + "<contactName>Jane Doe</contactName>";
            auth = auth + "<city>Istanbul</city>";
            auth = auth + "<country>Turkey</country>";
            auth = auth + "</shippingAddress>";
            auth = auth + "<billingAddress>";
            auth = auth + "<address>Nidakule Göztepe, Merdivenköy Mah. Bora Sok. No:1</address>";
            auth = auth + "<zipCode>34742</zipCode>";
            auth = auth + "<contactName>Jane Doe</contactName>";
            auth = auth + "<city>Istanbul</city>";
            auth = auth + "<country>Turkey</country>";
            auth = auth + "</billingAddress>";
            auth = auth + "<basketItems>";
            auth = auth + "<id>BI101</id>";
            auth = auth + "<price>" + price11 + "</price>";
            auth = auth + "<name>Binocular</name>";
            auth = auth + "<category1>Collectibles</category1>";
            auth = auth + "<category2>Accessories</category2>";
            auth = auth + "<itemType>PHYSICAL</itemType>";
            auth = auth + "</basketItems>";
            auth = auth + "<basketItems>";
            auth = auth + "<id>BI102</id>";
            auth = auth + "<price>" + price11 + "</price>";
            auth = auth + "<name>Game code</name>";
            auth = auth + "<category1>Game</category1>";
            auth = auth + "<category2>Online Game Items</category2>";
            auth = auth + "<itemType>VIRTUAL</itemType>";
            auth = auth + "</basketItems>";
            //auth = auth + "<basketItems>";
            //auth = auth + "<id>BI103</id>";
            //auth = auth + "<price>"+price11+"</price>";
            //auth = auth + "<name>Usb</name>";
            //auth = auth + "<category1>Electronics</category1>";
            //auth = auth + "<category2>Usb / Cable</category2>";
            //auth = auth + "<itemType>PHYSICAL</itemType>";
            //auth = auth + "</basketItems>";
            auth = auth + "</root>";

            

            var xml = new XmlDocument();
            xml.LoadXml(auth);
            string jsonText = ((string)JsonConvert.SerializeXmlNode(xml)).Substring(8, (((string)JsonConvert.SerializeXmlNode(xml)).Substring(8).Length - 1));


            //string pki = "[locale=tr,conversationId=123456789,price=1.0,paidPrice=1.2,installment=1,paymentChannel=WEB,basketId=B67832,paymentGroup=PRODUCT,paymentCard=[cardHolderName=John Doe,cardNumber=5528790000000008,expireYear=2030,expireMonth=12,cvc=123],buyer=[id=BY789,name=John,surname=Doe,identityNumber=74300864791,email=email@email.com,gsmNumber=+905350000000,registrationDate=2013-04-21 15:12:09,lastLoginDate=2015-10-05 12:43:35,registrationAddress=Nidakule Göztepe, Merdivenköy Mah. Bora Sok. No:1,city=Istanbul,country=Turkey,zipCode=34732,ip=85.34.78.112],shippingAddress=[address=Nidakule Göztepe, Merdivenköy Mah. Bora Sok. No:1,zipCode=34742,contactName=Jane Doe,city=Istanbul,country=Turkey],billingAddress=[address=Nidakule Göztepe, Merdivenköy Mah. Bora Sok. No:1,zipCode=34742,contactName=Jane Doe,city=Istanbul,country=Turkey],basketItems=[[id=BI101,price=0.3,name=Binocular,category1=Collectibles,category2=Accessories,itemType=PHYSICAL], [id=BI102,price=0.5,name=Game code,category1=Game,category2=Online Game Items,itemType=VIRTUAL], [id=BI103,price=0.2,name=Usb,category1=Electronics,category2=Usb / Cable,itemType=PHYSICAL]],currency=TRY,callbackUrl=https://www.iyzico.com/deneme]";


            string pki = "[locale=tr,conversationId=" + conversationId + ",price=" + price + ",paidPrice=" + paidPrice + ",installment=" + installment + ",paymentChannel=WEB,basketId=B67832,paymentGroup=PRODUCT,paymentCard=[cardHolderName=" + cardHolderName + ",cardNumber=" + cardNumber + ",expireYear=" + expireYear + ",expireMonth=" + expireMonth + ",cvc=" + cvc + "],buyer=[id=BY789,name=John,surname=Doe,identityNumber=74300864791,email=deneme@edeneme.com,gsmNumber=+905350000000,registrationDate=2013-04-21 15:12:09,lastLoginDate=2015-10-05 12:43:35,registrationAddress=Nidakule Göztepe, Merdivenköy Mah. Bora Sok. No:1,city=Istanbul,country=Turkey,zipCode=34732,ip=85.34.78.112],shippingAddress=[address=Nidakule Göztepe, Merdivenköy Mah. Bora Sok. No:1,zipCode=34742,contactName=Jane Doe,city=Istanbul,country=Turkey],billingAddress=[address=Nidakule Göztepe, Merdivenköy Mah. Bora Sok. No:1,zipCode=34742,contactName=Jane Doe,city=Istanbul,country=Turkey],basketItems=[[id=BI101,price=" + price11 + ",name=Binocular,category1=Collectibles,category2=Accessories,itemType=PHYSICAL], [id=BI102,price=" + price11 + ",name=Game code,category1=Game,category2=Online Game Items,itemType=VIRTUAL]],currency=TRY,callbackUrl=" + callbackUrl + "]";


            SHA1 sha = new SHA1CryptoServiceProvider();
            var hashdegeri = ApiKey + xiyzirnd + secretkey + pki;
            hashdegeri = Convert.ToBase64String(sha.ComputeHash(Encoding.UTF8.GetBytes(hashdegeri)));
            var Authorization = "IYZWS" + " " + ApiKey + ':' + hashdegeri;

            var resdata = SendRequest(Authorization, xiyzirnd, ApiKey, jsonText, "https://api.iyzipay.com/payment/3dsecure/initialize");


            payment.PostResult.Form = resdata;
            string Base64Decode = resdata;
            var reponseForm = JsonConvert.DeserializeObject<createPage>(resdata);
            var thredHtml = reponseForm.threeDSHtmlContent;
            if (thredHtml != null)
            {
                var base64EncodedBytes = System.Convert.FromBase64String(thredHtml);
                var decode = System.Text.Encoding.UTF8.GetString(base64EncodedBytes);

                payment.PostResult.Form = decode;
                payment.PostResult.Result = TransactionResult.InProgress;
            }
            else
            {
                payment.PostResult.Result = TransactionResult.Error;
            }


        }

        public override void CheckResponse(PostingData payment, string formValues)
        {

            this.HotelTransactionId = payment.TransactionId;

            NameValueCollection RequestForm = System.Web.HttpUtility.ParseQueryString(formValues);

            var status = RequestForm.Get("status");
            var paymentId = RequestForm.Get("paymentId");
            var conversationData = RequestForm.Get("conversationData");
            var conversationId = RequestForm.Get("conversationId");
            var mdStatus = RequestForm.Get("mdStatus");


            string err = "";
            string succesful = "";
            switch (mdStatus)
            {
                case "1":
                    succesful = "Başarılı";
                    payment.PostResult.Result = TransactionResult.InProgress;
                    break;
                case "0":
                    err = "	3-D Secure imzası geçersiz veya doğrulama";
                    payment.PostResult.AddError(mdStatus + ":" + err);
                    payment.PostResult.Result = TransactionResult.Error;
                    break;
                case "2":
                    err = "	Kart sahibi veya bankası sisteme kayıtlı değil";
                    payment.PostResult.AddError(mdStatus + ":" + err);
                    payment.PostResult.Result = TransactionResult.Error;
                    break;
                case "3":
                    err = "	Kartın bankası sisteme kayıtlı değil";
                    payment.PostResult.AddError(mdStatus + ":" + err);
                    payment.PostResult.Result = TransactionResult.Error;
                    break;
                case "4":
                    err = "Doğrulama denemesi, kart sahibi sisteme daha sonra kayıt olmayı seçmiş";
                    payment.PostResult.AddError(mdStatus + ":" + err);
                    payment.PostResult.Result = TransactionResult.Error;
                    break;
                case "5":
                    err = "	Doğrulama yapılamıyor";
                    payment.PostResult.AddError(mdStatus + ":" + err);
                    payment.PostResult.Result = TransactionResult.Error;
                    break;
                case "6":
                    err = "3-D Secure hatası";
                    payment.PostResult.AddError(mdStatus + ":" + err);
                    payment.PostResult.Result = TransactionResult.Error;
                    break;
                case "7":
                    err = "	Sistem hatası";
                    payment.PostResult.AddError(mdStatus + ":" + err);
                    payment.PostResult.Result = TransactionResult.Error;
                    break;
                case "8":
                    err = "	Bilinmeyen kart no";
                    payment.PostResult.AddError(mdStatus + ":" + err);
                    payment.PostResult.Result = TransactionResult.Error;
                    break;
            }

      

            conversationId = payment.TransactionId;
            string price = payment.Amount.ToString("###0.0").Replace(",", ".");
            string paidPrice = payment.Amount.ToString("###0.0").Replace(",", ".");
            string currency = GetCurrencyCode(payment.Currency);
            string installment = payment.Installment;
            string callbackUrl = this.CallbackURL.Contains("&") ? this.CallbackURL.Split('&')[0].Replace("&", ".") : this.CallbackURL;
            string cardNumber = payment.CardNo;
            string expireYear = payment.ExpYear;
            string expireMonth = payment.ExpMonth;
            string cvc = payment.CVV2;
            string cardHolderName = payment.CardHolderName;
            string id = "";
            string name = this.AccountName;
            string surname = this.AccountName;
            string identityNumber = "";
            string city = payment.billingCity;
            string country = payment.billingCountry;
            string email = payment.customEmail;
            string ip = this.UserIPAddress;
            string registrationAddress = payment.billingAddress;
            string contactName = payment.postViewName;
            string address = payment.billingAddress;
            string itemType = "";
            string category1 = "";
            double price1 = payment.Amount / 2;
            string price11 = price1.ToString("###0.0").Replace(",", ".");


            string chars = "abcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[13];
            var r = new Random();

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[r.Next(chars.Length)];
            }
            string xiyzirnd = new String(stringChars);


            string ApiKey = this.Password;
            string secretkey = this.Password3D;


            string auth3d = "<root>";
            auth3d = auth3d + "<locale>tr</locale>";
            auth3d = auth3d + "<conversationId>" + conversationId + "</conversationId>";
            auth3d = auth3d + "<paymentId>" + paymentId + "</paymentId>";
            auth3d = auth3d + "</root>";

            var xml = new XmlDocument();
            xml.LoadXml(auth3d);
            string jsonText2 = ((string)JsonConvert.SerializeXmlNode(xml)).Substring(8, (((string)JsonConvert.SerializeXmlNode(xml)).Substring(8).Length - 1));


            string pki = "[locale=tr,conversationId=" + conversationId + ",paymentId=" + paymentId + "]";


            SHA1 sha = new SHA1CryptoServiceProvider();
            var hashdegeri = ApiKey + xiyzirnd + secretkey + pki;
            hashdegeri = Convert.ToBase64String(sha.ComputeHash(Encoding.UTF8.GetBytes(hashdegeri)));
            var Authorization = "IYZWS" + " " + ApiKey + ':' + hashdegeri;


            var rresdata = SendPaymentRequest(Authorization, xiyzirnd, jsonText2, "https://api.iyzipay.com/payment/3dsecure/auth");


            //payment.PostResult.Form = rresdata;
            //var dataResponse = rresdata.Contains("succes");

            //if (dataResponse == true && mdStatus == "1")
            //{
            //    payment.PostResult.Result = TransactionResult.Approved;

            //}
            //else
            //{

            //    payment.PostResult.Result = TransactionResult.Error;
            //}

            
            payment.PostResult.Form = rresdata;

             RequestForm = System.Web.HttpUtility.ParseQueryString(formValues);

            if (rresdata.Contains("succes"))
            {
                payment.PostResult.Result = TransactionResult.Approved;
            }

            else
            {
                payment.PostResult.Result = TransactionResult.Error;
            }


            base.CheckResponse(payment, formValues);

        }

        

        public string SendRequest(string Authorization, string xiyzirnd, string ApiKey, string jsonText, string postAcsUrl)
        {

            byte[] dataStream = Encoding.UTF8.GetBytes(jsonText);
            HttpWebRequest webRequest = (HttpWebRequest)HttpWebRequest.Create(postAcsUrl);
            webRequest.Method = "POST";
            //webRequest.Host = "api.iyzipay.com";
            //webRequest.Accept = "application/json";
            webRequest.ContentType = "application/json";
            if (Authorization.ToString() != "")
                webRequest.Headers["Authorization"] = Authorization;
            if (xiyzirnd.ToString() != "")
                webRequest.Headers["x-iyzi-rnd"] = xiyzirnd;
            //webRequest.Headers["options.ApiKey"] = ApiKey;
            webRequest.ContentLength = dataStream.Length;
            //webRequest.KeepAlive = false;
            string responseFromServer = "";

            using (Stream newStream = webRequest.GetRequestStream())
            {
                newStream.Write(dataStream, 0, dataStream.Length);
                newStream.Close();
            }

            using (WebResponse webResponse = webRequest.GetResponse())
            {
                using (StreamReader reader = new StreamReader(webResponse.GetResponseStream()))
                {
                    responseFromServer = reader.ReadToEnd();
                    reader.Close();
                }

                webResponse.Close();
            }

            if (string.IsNullOrEmpty(responseFromServer))
            {
                return "";
            }

            return responseFromServer;
        }


        public string SendPaymentRequest(string Authorization, string xiyzirnd, string jsonText2, string postAcsUrl)
        {

            byte[] dataStream = Encoding.UTF8.GetBytes(jsonText2);
            HttpWebRequest webRequest = (HttpWebRequest)HttpWebRequest.Create(postAcsUrl);
            webRequest.Method = "POST";           
            webRequest.ContentType = "application/json";
            if (Authorization.ToString() != "")
                webRequest.Headers["Authorization"] = Authorization;
            if (xiyzirnd.ToString() != "")
                webRequest.Headers["x-iyzi-rnd"] = xiyzirnd;
            //webRequest.Headers["options.ApiKey"] = ApiKey;
            webRequest.ContentLength = dataStream.Length;
            //webRequest.KeepAlive = false;
            string responseFromServer = "";

            using (Stream newStream = webRequest.GetRequestStream())
            {
                newStream.Write(dataStream, 0, dataStream.Length);
                newStream.Close();
            }

            using (WebResponse webResponse = webRequest.GetResponse())
            {
                using (StreamReader reader = new StreamReader(webResponse.GetResponseStream()))
                {
                    responseFromServer = reader.ReadToEnd();
                    reader.Close();
                }

                webResponse.Close();
            }

            if (string.IsNullOrEmpty(responseFromServer))
            {
                return "";
            }

            return responseFromServer;
        }


        public class createPage
        {
            public string status { get; set; }
            public string locale { get; set; }
            public string systemTime { get; set; }
            public string conversationId { get; set; }
            public string threeDSHtmlContent { get; set; }
            public string errorCode { get; set; }
            public string errorMessage { get; set; }
            public string errorGroup { get; set; }

        }

       
    }
}
