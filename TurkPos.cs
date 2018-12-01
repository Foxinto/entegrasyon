using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.IO.Compression;
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

    public class TurkPos : BankPos

    {


        public override string CallbackVariableName
        {
            get { return "Siparis_ID"; }
        }

        public override bool MakePayment(PostingData data)
        {
            try
            {

                if (this.StoreType == ThreeDType.NoneSecure)
                    TurkPosNoneSecure(data);
                else if (this.StoreType == ThreeDType.ThreeD)
                    TurkPosThreeD(data);
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

                TurkPosNoneSecure(data);
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

                TurkPosNoneSecure(data);
            }
            finally
            {
                base.RefundPayment(data);

                this.StoreType = st;

                this.TransType = tt;
            }

            return true;
        }



        private void TurkPosThreeD(PostingData payment)
        {

           
            var comResponse = new XmlDocument();
            var hahResponse = new XmlDocument();

          
            string pamount = payment.Amount.ToString("N2");


            string posname = this.ExtraParams;
            string CLIENT_CODE = this.TerminalId;
            string CLIENT_USERNAME = this.AccountName;
            string CLIENT_PASSWORD = this.Password;
            string SanalPOS_ID = this.ClientId;
            string GUID = this.Password3D;
            string KK_Sahibi = payment.CardHolderName;
            string KK_No = payment.CardNo;
            string KK_SK_Ay = "00".Substring(0, 2 - payment.ExpMonth.Length) + payment.ExpMonth;
            string KK_SK_Yil = payment.ExpYear;
            string KK_CVC = payment.CVV2.ToString();
            string KK_Sahibi_GSM = payment.CardPhoneNumber;
            string Hata_URL = this.CallbackURL.Split('?')[0];
            string Basarili_URL = this.CallbackURL.Split('?')[0];
            string Siparis_ID = payment.TransactionId;
            string Siparis_Aciklama = "";
            string Taksit = payment.Installment; /* "0".Substring(0, 2-payment.Installment.Length) + payment.Installment;*/
            //string Islem_Hash = HashData;
            string Islem_ID = "";
            string IPAdr = this.UserIPAddress;
            string Ref_URL = this.CallbackURL.Split('?')[0];
            string Data1 = "";
            string Data2 = "";
            string Data3 = "";
            string Data4 = "";
            string Data5 = "";

            //string postorandata = "<TurkPosRequest>";
            //postorandata = postorandata + "<Name>TP_Ozel_Oran_Liste</Name>";
            //postorandata = postorandata + "<CLIENT_CODE>" + CLIENT_CODE + "</CLIENT_CODE>";
            //postorandata = postorandata + "<CLIENT_USERNAME>" + CLIENT_USERNAME + "</CLIENT_USERNAME>";
            //postorandata = postorandata + "<CLIENT_PASSWORD>" + CLIENT_PASSWORD + "</CLIENT_PASSWORD>";
            //postorandata = postorandata + "<GUID>" + GUID + "</GUID>";
            //postorandata = postorandata + "</TurkPosRequest>";

            //postorandata = "strXml=" + postorandata;

            //var rdata = SendRequest(postorandata, "http://mobilws.ew.com.tr/turkpos.xml/service_turkpos_prod.asmx/TP_Ozel_Oran_Liste");

            //comResponse.LoadXml(rdata);
            //XmlNodeList commOran = comResponse.SelectNodes("TurkPosResponse/DT_Ozel_Oranlar");

            //OzelOranModel fields = null;

            //var instaTak = "MO_0" + Taksit;
            //OzelOranModel.OzelOran posINFO;
            //OzelOranModel.OzelOran posChoose = null;

            //foreach (XmlNode no in commOran)
            //{
            //    fields = new OzelOranModel
            //    {
            //        List = new OzelOranModel.OzelOran[] {
            //            new OzelOranModel.OzelOran{
            //                SanalPOSID = no["SanalPOS_ID"].InnerText,
            //                OzelOranID = no["Ozel_Oran_ID"].InnerText,
            //                Kredi_KartiBanka = no["Kredi_Karti_Banka"].InnerText,
            //                GUID = no["GUID"].InnerText,
            //                MO_01 = no["MO_01"].InnerText,
            //                MO_02 = no["MO_02"].InnerText,
            //                MO_03 = no["MO_03"].InnerText,
            //                MO_04 = no["MO_04"].InnerText,
            //                MO_05 = no["MO_05"].InnerText,
            //                MO_06 = no["MO_06"].InnerText,
            //                MO_07 = no["MO_07"].InnerText,
            //                MO_08 = no["MO_08"].InnerText,
            //                MO_09 = no["MO_09"].InnerText,
            //                MO_10 = no["MO_10"].InnerText,
            //                MO_11 = no["MO_11"].InnerText,
            //                MO_12 = no["MO_12"].InnerText,
            //                SeciliKomisyon = no["" + instaTak].InnerText,
            //            },
            //        }
            //    };

            //    posINFO = fields.List.Where(x => x.SanalPOSID == SanalPOS_ID && x.Kredi_KartiBanka == posname).FirstOrDefault();

            //    if (posINFO != null)
            //        posChoose = posINFO;
            //}

            //if (posChoose != null)
            //    posINFO = posChoose;
            //else
            //    posINFO = fields.List.Where(x => x.SanalPOSID == SanalPOS_ID && x.Kredi_KartiBanka == posname).FirstOrDefault();
            string Islem_Tutar = pamount.ToString();
            string Toplam_Tutar = pamount.ToString();
            double komAmount = 0.00;
            double komOran = payment.InstallmentCom;

            //double komOran = posINFO.SeciliKomisyon != "" && posINFO.SeciliKomisyon != null ? double.Parse(posINFO.SeciliKomisyon.Replace(".",",")) : 0;

            if (komOran != 0 && komOran > 0)
            {
                //komAmount =(Convert.ToDouble((Convert.ToDouble(pamount) + (Convert.ToDouble(pamount) * (komOran / 100))).ToString("N2")));
                //Toplam_Tutar = komAmount.ToString();

                komAmount = (Convert.ToDouble(((Convert.ToDouble(pamount) * 100.0) / (100 + komOran)).ToString("N2")));
                Islem_Tutar = komAmount.ToString("N2");
            }

            var rdata = "";
            string Islem_Hash = "<TurkPosRequest>";
            Islem_Hash = Islem_Hash + "<Data>";
            Islem_Hash = Islem_Hash + CLIENT_CODE + GUID + SanalPOS_ID + Taksit + Islem_Tutar + Toplam_Tutar + Siparis_ID + Hata_URL + Basarili_URL;
            Islem_Hash = Islem_Hash + "</Data>";
            Islem_Hash = Islem_Hash + "</TurkPosRequest>";

            Islem_Hash = "strXml=" + Islem_Hash;

            rdata += SendRequest(Islem_Hash, "http://mobilws.ew.com.tr/turkpos.xml/service_turkpos_prod.asmx/SHA2B64");

            hahResponse.LoadXml(rdata);
            var hashData = hahResponse.SelectSingleNode("TurkPosResponse/Sonuc").InnerText;

            string postdata = "<TurkPosRequest>";
            postdata = postdata + "<CLIENT_CODE>" + CLIENT_CODE + "</CLIENT_CODE>";
            postdata = postdata + "<CLIENT_USERNAME>" + CLIENT_USERNAME + "</CLIENT_USERNAME>";
            postdata = postdata + "<CLIENT_PASSWORD>" + CLIENT_PASSWORD + "</CLIENT_PASSWORD>";
            postdata = postdata + "<SanalPOS_ID>" + SanalPOS_ID + "</SanalPOS_ID>";
            postdata = postdata + "<GUID>" + GUID + "</GUID>";
            postdata = postdata + "<KK_Sahibi>" + KK_Sahibi + "</KK_Sahibi>";
            postdata = postdata + "<KK_No>" + KK_No + "</KK_No>";
            postdata = postdata + "<KK_SK_Ay>" + KK_SK_Ay + "</KK_SK_Ay>";
            postdata = postdata + "<KK_SK_Yil>" + KK_SK_Yil + "</KK_SK_Yil>";
            postdata = postdata + "<KK_CVC>" + KK_CVC + "</KK_CVC>";
            postdata = postdata + "<KK_Sahibi_GSM>05385446050</KK_Sahibi_GSM>";
            postdata = postdata + "<Hata_URL>" + Hata_URL + "</Hata_URL>";
            postdata = postdata + "<Basarili_URL>" + Basarili_URL + "</Basarili_URL>";
            postdata = postdata + "<Siparis_ID>" + Siparis_ID + "</Siparis_ID>";
            postdata = postdata + "<Siparis_Aciklama>" + Siparis_Aciklama + "</Siparis_Aciklama>";
            postdata = postdata + "<Taksit>" + Taksit + "</Taksit>";
            postdata = postdata + "<Islem_Tutar>" + Islem_Tutar + "</Islem_Tutar>";
            postdata = postdata + "<Toplam_Tutar>" + Toplam_Tutar + "</Toplam_Tutar>";
            postdata = postdata + "<Islem_Hash>" + hashData + "</Islem_Hash>";
            postdata = postdata + "<Islem_ID>" + Islem_ID + "</Islem_ID>";
            postdata = postdata + "<IPAdr>" + IPAdr + "</IPAdr>";
            postdata = postdata + "<Ref_URL>" + Ref_URL + "</Ref_URL>";
            postdata = postdata + "<Data1>" + Data1 + "</Data1>";
            postdata = postdata + "<Data2>" + Data2 + "</Data2>";
            postdata = postdata + "<Data3>" + Data3 + "</Data3>";
            postdata = postdata + "<Data4>" + Data4 + "</Data4>";
            postdata = postdata + "<Data5>" + Data5 + "</Data5>";
            postdata = postdata + "</TurkPosRequest>";

            postdata = "strXml=" + postdata;

            rdata = SendRequest(postdata, "http://mobilws.ew.com.tr/turkpos.xml/service_turkpos_prod.asmx/TP_Islem_Odeme");

            var sonucResponse = new XmlDocument();
            sonucResponse.LoadXml(rdata);

            var dataResponseCode = sonucResponse.SelectSingleNode("TurkPosResponse/Sonuc").InnerText;
            var dataResponseText = sonucResponse.SelectSingleNode("TurkPosResponse/Sonuc_Str").InnerText;

            if (dataResponseCode == "1")
            {
                var dataResponseUrl = sonucResponse.SelectSingleNode("TurkPosResponse/UCD_URL").InnerText;
                payment.PostResult.Result = TransactionResult.InProgress;
                payment.PostResult.Form = this.PreparePOSTForm(dataResponseUrl, null);
            }
            else
            {
                payment.PostResult.Form = "Error Code :" + dataResponseCode +" / Error Explain:" + dataResponseText;
                payment.PostResult.Result = TransactionResult.Error;
            }
        }

        private void TurkPosNoneSecure(PostingData payment)
        {
        }

        public string SendRequest(string postorandata, string postAcsUrl)
        {
            WebRequest request = WebRequest.Create(postAcsUrl);
            //HttpWebRequest request = (HttpWebRequest)WebRequest.Create(postAcsUrl);
            byte[] strXml;
            strXml = System.Text.Encoding.UTF8.GetBytes(postorandata);
            request.Method = "POST";
            //request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8";
            request.Headers["Accept-Encoding"] = "gzip, deflate";
            request.Headers["Accept-Language"] = "tr-TR,tr;q=0.9,en-US;q=0.8,en;q=0.7";
            request.Headers["Cache-Control"] = "max-age=0";
            request.Headers["Origin"] = "http://mobilws.ew.com.tr";
            //request.Host = "mobilws.ew.com.tr";
            //request.Referer = "http://mobilws.ew.com.tr/turkpos.xml/service_turkpos_prod.asmx?op=TP_Ozel_Oran_Liste";
            request.ContentType = "application/x-www-form-urlencoded";
            //request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/62.0.3202.94 Safari/537.36";
            //request.KeepAlive = true;
            request.ContentLength = strXml.Length;
            Stream requestStream = request.GetRequestStream();
            requestStream.Write(strXml, 0, strXml.Length);
            requestStream.Close();
            WebResponse response;
            

            response = (WebResponse)request.GetResponse();

            Stream responseStream = response.GetResponseStream();

            if (response.Headers.Get("Content-Encoding") == "gzip")
                responseStream = new GZipStream(responseStream, CompressionMode.Decompress);

            if (response.Headers.Get("Content-Encoding") == "deflate")
                responseStream = new DeflateStream(responseStream, CompressionMode.Decompress);

            string responseStr = new StreamReader(responseStream).ReadToEnd();
            return responseStr;
        }

        public override void CheckResponse(PostingData payment, string formValues)
        {
            try
            {
                NameValueCollection RequestForm = System.Web.HttpUtility.ParseQueryString(formValues);
                
                if(RequestForm != null)
                {
                    var dataResponseCode = RequestForm.Get("TURKPOS_RETVAL_Sonuc");
                    var dataResponseText = RequestForm.Get("TURKPOS_RETVAL_Sonuc_Str");
                    var dataResponseGuid = RequestForm.Get("TURKPOS_RETVAL_GUID");
                    var dataResponseİslmTarihi = RequestForm.Get("TURKPOS_RETVAL_Islem_Tarih");
                    var dataResponseDekontId = RequestForm.Get("TURKPOS_RETVAL_Dekont_ID");
                    var dataResponseTahsilatTutari = RequestForm.Get("TURKPOS_RETVAL_Tahsilat_Tutari");
                    var dataResponseOdemeTutari = RequestForm.Get("TURKPOS_RETVAL_Odeme_Tutari");
                    var dataResponseSiparisId = RequestForm.Get("TURKPOS_RETVAL_Siparis_ID");
                    var dataResponseIslemId = RequestForm.Get("TURKPOS_RETVAL_Islem_ID");
                    var dataResponseExdata = RequestForm.Get("TURKPOS_RETVAL_Ext_Data");

                    int resCode = 0,decId= 0;
                
                    if (dataResponseCode != null)
                        resCode = Int32.Parse(dataResponseCode);

                    decId = dataResponseDekontId != "" ? Int32.Parse(dataResponseDekontId) : 0;

                    if (resCode < 0 && decId == 0)
                    {
                        payment.PostResult.Result = TransactionResult.Error;
                        payment.PostResult.Message = formValues;
                        payment.PostResult.ErrorMessages.Add(dataResponseText);
                    }
                    else if(decId > 0)
                    {
                        payment.PostResult.Result = TransactionResult.Approved;
                        payment.PostResult.Message = dataResponseText;
                    }
                       
                }
            }
            finally
            {
                base.CheckResponse(payment, formValues); 
            }
        }

        public partial class OzelOranModel
        {
            private OzelOran[] OzelOranList;

            [System.Xml.Serialization.XmlElementAttribute("List")]
            public OzelOran[] List
            {
                get
                {
                    return this.OzelOranList;
                }
                set
                {
                    this.OzelOranList = value;
                }
            }
            [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
            public partial class OzelOran
            {
                public string OzelOranID { get; set; }
                public string GUID { get; set; }
                public string SanalPOSID { get; set; }
                public string Kredi_KartiBanka { get; set; }
                public string MO_01 { get; set; }
                public string MO_02 { get; set; }
                public string MO_03 { get; set; }
                public string MO_04 { get; set; }
                public string MO_05 { get; set; }
                public string MO_06 { get; set; }
                public string MO_07 { get; set; }
                public string MO_08 { get; set; }
                public string MO_09 { get; set; }
                public string MO_10 { get; set; }
                public string MO_11 { get; set; }
                public string MO_12 { get; set; }
                public string SeciliKomisyon { get; set; }
            }
        }
    }

}



