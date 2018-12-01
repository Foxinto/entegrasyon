using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace ErsPos
{
    public class IPARA : BankPos
    {
     

        public override string CallbackVariableName
        {
            get { return "orderId"; }
        }

        public override string PostUrl()
        {
            return "https://api.ipara.com";
        }

        //public override string ThreeDUrl()
        //{
        //    return "https://www.ipara.com/3dgate";
        //}

       
        
        public override bool MakePayment(PostingData data)
        {
            try
            {

                if (this.StoreType == ThreeDType.NoneSecure)
                    iParaNoneSecure(data);
                else if (this.StoreType == ThreeDType.ThreeD)
                    iParaThreeD(data);
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

                iParaNoneSecure(data);
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

                iParaNoneSecure(data);
            }
            finally
            {
                base.RefundPayment(data);

                this.StoreType = st;

                this.TransType = tt;
            }

            return true;
        }

        private void iParaThreeD(PostingData payment)
        {
            var threeDResponse = new XmlDocument();
            var sonucResponse = new XmlDocument();

            string transType = "";
            if (this.TransType == TransactionBaseType.PreAuth) transType = "PreAuth";
            else if (this.TransType == TransactionBaseType.Sale) transType = "Sale";
            else if (this.TransType == TransactionBaseType.PostAuth) transType = "PostAuth";
            else if (this.TransType == TransactionBaseType.Refund) transType = "Credit";
            else if (this.TransType == TransactionBaseType.Void) transType = "Void";

            string mode = "T";
            string orderId = payment.TransactionId;
            string cardOwnerName = payment.CardHolderName;
            string cardNumber = payment.CardNo;
            string cardExpireMonth = "00".Substring(0, 2 - payment.ExpMonth.Length) + payment.ExpMonth;
            string cardExpireYear = payment.ExpYear.Length == 2 ? payment.ExpYear.ToString() : payment.ExpYear.Substring(2, 2); 
            string cardCvc = payment.CVV2;
            string userId = "";
            string cardId = "";
            string installment = payment.Installment;
            string amount = (Convert.ToDouble(payment.Amount.ToString("###0.00")) * 100).ToString(); 
            string purchaserName = this.AccountName;
            string purchaserSurname = this.AccountName;
            string purchaserEmail = this.email;
            string successUrl = this.CallbackURL;/*.Contains("?") ? this.CallbackURL3D.Split('?')[0] : this.CallbackURL3D;*/
            string failureUrl = this.CallbackURL;/*.Contains("?") ? this.CallbackURL3D.Split('?')[0] : this.CallbackURL3D;*/
            string transactionDate = DateTime.Now.ToString("yyyy-MM-ddHH:mm:ss");
            string version = "1.0";
            string privateKey = this.Password3D;
            string publicKey = this.Password;
            
            

            this.HotelTransactionId = payment.TransactionId;
                       

            NameValueCollection collection = new NameValueCollection();

           
            SHA1 sha = new SHA1CryptoServiceProvider();
            var token = privateKey + orderId + amount + mode + cardOwnerName + cardNumber + cardExpireMonth + cardExpireYear + cardCvc + userId + cardId + purchaserName + purchaserSurname + purchaserEmail + transactionDate;
            token = Convert.ToBase64String(sha.ComputeHash(Encoding.UTF8.GetBytes(token)));
            token = publicKey + ':' + token;

            //SHA1 sha = new SHA1CryptoServiceProvider();
            //string HashData = CodeHelper.GetSHA1(this.Password3D + payment.TransactionId + pamount + "T" + payment.CardHolderName + payment.CardNo + payment.ExpMonth + payment.ExpYear + payment.CVV2 + "" + "" + payment.CardHolderName + payment.CardHolderName + this.email + "");
            //string XHashedPassword = Convert.ToBase64String(sha.ComputeHash(Encoding.UTF8.GetBytes(HashData)));
            //string token = this.Password + ':' + HashData;

            collection.Add("mode", "T");
            collection.Add("orderId", orderId);
            collection.Add("cardOwnerName", cardOwnerName);
            collection.Add("cardNumber", cardNumber);
            collection.Add("cardExpireMonth", cardExpireMonth);
            collection.Add("cardExpireYear", cardExpireYear);
            collection.Add("cardCvc", cardCvc);
            collection.Add("userId", userId);
            collection.Add("cardId", cardId);
            collection.Add("installment", installment);
            collection.Add("amount", amount);
            collection.Add("echo", "");
            collection.Add("purchaserName", purchaserName);
            collection.Add("purchaserSurname", purchaserSurname);
            collection.Add("purchaserEmail", purchaserEmail);
            collection.Add("successUrl", successUrl);
            collection.Add("failureUrl", failureUrl);
            collection.Add("transactionDate", transactionDate);
            collection.Add("version", version);
            collection.Add("token", token);


            payment.PostResult.Result = TransactionResult.InProgress;

            //var uri = this.CallbackURL.Split('?')[0].ToString().Split('/');
            //var urlPost = uri[0] + "//" + uri[1] + uri[2];

            //string headerString = "<meta name=\"referrer\" content="+urlPost+" >";

            payment.PostResult.Form = this.PreparePOSTForm("https://www.ipara.com/3dgate", collection);

            
        }

        private void iParaNoneSecure(PostingData payment)
        {

        }

        public override void CheckResponse(PostingData payment, string formValues)
        {
            NameValueCollection RequestForm = System.Web.HttpUtility.ParseQueryString(formValues);


           

            var result = RequestForm.Get("result");
            var errorMessage = RequestForm.Get("errorMessage");
            var errorCode = RequestForm.Get("errorCode");
            var publicKey = RequestForm.Get("publicKey");
            var echo = RequestForm.Get("echo");
            var transactionDate = RequestForm.Get("transactionDate");
            var mode = RequestForm.Get("mode");
            var orderId = RequestForm.Get("orderId");
            var amount = RequestForm.Get("amount");
            var hash = RequestForm.Get("hash");
            var threeDSecureCode = RequestForm.Get("threeDSecureCode");
            var threeD = "true";

            string privateKey = this.Password3D;
            string version = "1.0";

            SHA1 sha = new SHA1CryptoServiceProvider();
            var token = privateKey + orderId + amount + mode + threeDSecureCode + transactionDate;
            token = Convert.ToBase64String(sha.ComputeHash(Encoding.UTF8.GetBytes(token)));
            token = publicKey + ':' + token;

            //SHA1 sha = new SHA1CryptoServiceProvider();
            //string hashstr = orderId + result + amount + mode + errorCode + errorMessage + transactionDate + publicKey + privateKey;
            //var Hashedstr = Convert.ToBase64String(sha.ComputeHash(Encoding.UTF8.GetBytes(hashstr)));
            //var token = publicKey + ':' + Hashedstr;


            var resdata = "";


            if (result != null && result != "1")
                payment.PostResult.AddError(errorMessage);

            if (result != null)
                payment.PostResult.ApprCode = result;
            else
                payment.PostResult.ApprCode = RequestForm.Get("result");


            string auth = "<auth>"; /*"<?xml version = \"1.0\" encoding =\"UTF-8\" transactionDate=\"" + transactionDate.ToString() + "\"token =\"" + token.ToString() + "\"?>";*/
            auth = auth + "<mode>" + mode + "</mode>";
            auth = auth + "<threeD>" + threeD + "</threeD>";
            auth = auth + "<orderId>" + orderId + "</orderId>";
            auth = auth + "<amount>" + amount + "</amount>";
            auth = auth + "<echo>" + echo + "</echo>";
            auth = auth + "<threeDSecureCode>" + threeDSecureCode + "</threeDSecureCode>";
            //auth = auth + "<purchaser>purchaser</purchaser>";
            auth = auth + "<products>";
            auth = auth + "<product>";
            auth = auth + "<productCode>Product Code 1</productCode>";
            auth = auth + "<productName>Product Name 1</productName>";
            auth = auth + "<quantity>1</quantity>";
            auth = auth + "<price>1500</price>";
            auth = auth + "</product>"; 
            auth = auth + "</products>";
            auth = auth + "</auth>";


            //var uri = this.CallbackURL.Split('?')[0].ToString().Split('/');
            //var urlPost = uri[0] + "//" + uri[1] + uri[2];


            resdata = SendRequest(auth,transactionDate,version,token, "https://api.ipara.com/rest/payment/auth");

            var sonucResponse = new XmlDocument();
            sonucResponse.LoadXml(resdata);

            var dataResponseCode = sonucResponse.SelectSingleNode("authResponse/result").InnerText;

            if(dataResponseCode == "1")
            {
                payment.PostResult.Result = TransactionResult.Approved;
            }


            base.CheckResponse(payment, formValues);
        }



        public string SendRequest(string auth, string transactionDate, string version, string token, string postAcsUrl)
        {
            

            byte[] dataStream = Encoding.UTF8.GetBytes(auth);
            HttpWebRequest webRequest = (HttpWebRequest)HttpWebRequest.Create(postAcsUrl);
            webRequest.Method = "POST";

            //if (urlPost.ToString() != "")
            //    webRequest.Headers["referrer"] = urlPost;
            if (transactionDate.ToString() != "")
                webRequest.Headers["transactionDate"] = transactionDate;
            if (version.ToString() != "")
                webRequest.Headers["version"] = version;
            if (token.ToString() != "")
                webRequest.Headers["token"] = token;
            webRequest.ContentType = "application/xml";
            webRequest.ContentLength = dataStream.Length;
            webRequest.KeepAlive = false;
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

        

      

        
    }
}
