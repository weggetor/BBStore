using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Web;

namespace Bitboxx.DNNModules.BBStore.Providers.Payment
{
    public class PaypalApi
    {
        public const string SandboxUrl = "https://api-3t.sandbox.paypal.com/nvp";
        public const string LiveUrl = "https://api-3t.paypal.com/nvp";
        public const int Version = 93;
        public static bool UseSandbox = true;

        public static string SetExpressCheckout(PaypalUserCredentials user, PaypalPaymentDetails paymentRequest, List<PaypalPaymentDetailsItem> items, PaypalSetExpressCheckoutParameters param)
        {
           
            string postData = "";

            postData += user.ToPostData();
            postData += HttpUtility.UrlEncode("METHOD") + "=SetExpressCheckout&";
            postData += HttpUtility.UrlEncode("VERSION") + "=" + Version.ToString() + "&";
            postData += paymentRequest.ToPostData();
            int index = 0;
            foreach (PaypalPaymentDetailsItem item in items)
            {
                postData += item.ToPostData(index);
                index++;
                if (index > 9)
                    break;
            }
            postData += param.ToPostData();
            
            return SendRequest(postData);
        }

        public static string GetExpressCheckoutDetails(PaypalUserCredentials user, string token)
        {
            string postData = "";

            postData += user.ToPostData();
            postData += HttpUtility.UrlEncode("METHOD") + "=GetExpressCheckoutDetails&";
            postData += HttpUtility.UrlEncode("VERSION") + "=" + Version.ToString() + "&";
            postData += HttpUtility.UrlEncode("TOKEN") + "=" + token;

            return SendRequest(postData);
        }

        public static string DoExpressCheckoutPayment(PaypalUserCredentials user, PaypalPaymentDetails paymentRequest, string token, string payerId)
        {
            int version = 93;
            string postData = "";

            postData += user.ToPostData();
            postData += HttpUtility.UrlEncode("METHOD") + "=DoExpressCheckoutPayment&";
            postData += HttpUtility.UrlEncode("VERSION") + "=" + Version.ToString() + "&";
            postData += HttpUtility.UrlEncode("TOKEN") + "=" + token + "&";
            postData += HttpUtility.UrlEncode("PAYERID") + "=" + payerId +"&";
            postData += paymentRequest.ToPostData();

            return SendRequest(postData);
        }

        private static string SendRequest(string postData)
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
            ServicePointManager.DefaultConnectionLimit = 9999;

            string url = UseSandbox ? SandboxUrl : LiveUrl;

            HttpWebRequest myHttpWebRequest = null; //Declare an HTTP-specific implementation of the WebRequest class.
            HttpWebResponse myHttpWebResponse = null; //Declare an HTTP-specific implementation of the WebResponse class

            //Create Request
            myHttpWebRequest = (HttpWebRequest) HttpWebRequest.Create(url);
            myHttpWebRequest.Method = "POST";

            byte[] data = Encoding.ASCII.GetBytes(postData);

            myHttpWebRequest.ContentType = "application/x-www-form-urlencoded";
            myHttpWebRequest.ContentLength = data.Length;

            Stream requestStream = myHttpWebRequest.GetRequestStream();
            requestStream.Write(data, 0, data.Length);
            requestStream.Close();

            //Get Response
            myHttpWebResponse = (HttpWebResponse) myHttpWebRequest.GetResponse();

            String responseString;
            using (Stream stream = myHttpWebResponse.GetResponseStream())
            {
                StreamReader reader = new StreamReader(stream, Encoding.UTF8);

                responseString = reader.ReadToEnd();
            }
            return responseString;
        }
    }

    #region enums
    public enum PaypalAckType
    {
        Success,
        Failure,
        SuccessWithWarning,
    }
    
    public enum PaypalCheckoutStatusType
    {
        PaymentActionNotInitiated,
        PaymentActionFailed,
        PaymentActionProgress,
        PaymentActionCompleted
    }

    public enum PaypalPayerStatus
    {
        Verified,
        Unverified
    }

    public enum PaypalAddressStatus
    {
        None,
        Confirmed,
        Unconfirmed
    }

    public enum PaypalItemCategory
    {
        Digital,
        Physical
    }

    public enum PaypalPaymentStatus
    {
        None, // – No status.
        Canceled, //-Reversal – A reversal has been canceled; for example, when you win a dispute and the funds for the reversal have been returned to you.
        Completed, //– The payment has been completed, and the funds have been added successfully to your account balance.
        Denied, // – You denied the payment. This happens only if the payment was previously pending because of possible reasons described for the PendingReason element.
        Expired, //– the authorization period for this payment has been reached.
        Failed, //– The payment has failed. This happens only if the payment was made from your buyer's bank account.
        InProgress, //– The transaction has not terminated, e.g. an authorization may be awaiting completion.
        PartiallyRefunded, //– The payment has been partially refunded.
        Pending, // – The payment is pending. See the PendingReason field for more information.
        Refunded, // – You refunded the payment.
        Reversed, // – A payment was reversed due to a chargeback or other type of reversal. The funds have been removed from your account balance and returned to the buyer. The reason for the reversal is specified in the ReasonCode element.
        Processed, // – A payment has been accepted.
        Voided, // – An authorization for this transaction has been voided.
        CompletedFundsHeld, // – The payment has been completed, and the funds have been added successfully to your pending balance.
    }
    public enum PaypalPendingReason
    {
        None, // – No pending reason.
        Address, // – The payment is pending because your buyer did not include a confirmed shipping address and your Payment Receiving Preferences is set such that you want to manually accept or deny each of these payments. To change your preference, go to the Preferences section of your Profile.
        Authorization, // – The payment is pending because it has been authorized but not settled. You must capture the funds first.
        ECheck, // – The payment is pending because it was made by an eCheck that has not yet cleared.
        Intl, // – The payment is pending because you hold a non-U.S. account and do not have a withdrawal mechanism. You must manually accept or deny this payment from your Account Overview.
        MultiCurrency, // – You do not have a balance in the currency sent, and you do not have your Payment Receiving Preferences set to automatically convert and accept this payment. You must manually accept or deny this payment.
        Order, // – The payment is pending because it is part of an order that has been authorized but not settled.
        PaymentReview, // – The payment is pending while it is being reviewed by PayPal for risk.
        RegulatoryReview, // - The payment is pending while we make sure it meets regulatory requirements. You will be contacted again in 24-72 hours with the outcome of the review.
        Unilateral, // – The payment is pending because it was made to an email address that is not yet registered or confirmed.
        Verify, // – The payment is pending because you are not yet verified. You must verify your account before you can accept this payment.
        Other, // – The payment is pending for a reason other than those listed above. For more information, contact PayPal customer service.
    }

    public enum PaypalReasonCode
    {
        None, // – No reason code.
        Chargeback, // – A reversal has occurred on this transaction due to a chargeback by your buyer.
        Guarantee, // – A reversal has occurred on this transaction due to your buyer triggering a money-back guarantee.
        BuyerComplaint, // – A reversal has occurred on this transaction due to a complaint about the transaction from your buyer.
        Refund, // – A reversal has occurred on this transaction because you have given the buyer a refund.
        Other, // – A reversal has occurred on this transaction due to a reason not listed above.
    }

    public enum PaypalProtectionEligibility
    {
        None,
        Eligible, // – Merchant is protected by PayPal's Seller Protection Policy for Unauthorized Payments and Item Not Received.
        PartiallyEligible, // – Merchant is protected by PayPal's Seller Protection Policy for Item Not Received.
        Ineligible, // – Merchant is not protected under the Seller Protection Policy.
    }
    
    public enum PaypalProtectionEligibilityType
    {
        None,
        Eligible, // – Merchant is protected by PayPal's Seller Protection Policy for both Unauthorized Payment and Item Not Received
        ItemNotReceivedEligible, // – Merchant is protected by PayPal's Seller Protection Policy for Item Not Received
        UnauthorizedPaymentEligible, // – Merchant is protected by PayPal's Seller Protection Policy for Unauthorized Payment
        Ineligible, // – Merchant is not protected under the Seller Protection Policy
    }

    #endregion

    public class PaypalExpressCheckoutStatus
    {
        public PaypalExpressCheckoutStatus()
        {
            
        }
        public PaypalExpressCheckoutStatus(string response)
        {
            string[] responseTokens = response.Split('&');
            foreach (string responseToken in responseTokens)
            {
                string[] nameValues = responseToken.Split('=');
                switch (nameValues[0])
                {
                    case "CHECKOUTSTATUS":
                        this.CheckOutStatus = (PaypalCheckoutStatusType)Enum.Parse(typeof(PaypalCheckoutStatusType), HttpUtility.UrlDecode(nameValues[1]), true);
                        break;
                }
            }
        }
        public PaypalCheckoutStatusType CheckOutStatus { get; set; }
    }

    public class PaypalCommonResponse
    {
        public PaypalCommonResponse()
        {
            Errors = new List<PaypalError>();
        }
        public PaypalCommonResponse(string response)
        {
            Errors = new List<PaypalError>();

            int errorNum = -1;
            for (int i = 9; i > -1; i--)
            {
                if (response.Contains("L_ERRORCODE" + i.ToString()))
                {
                    errorNum = i;
                    break;
                }
            }
            PaypalError[] ppErrors = new PaypalError[0];
            if (errorNum > -1)
            {
                ppErrors = new PaypalError[errorNum + 1];
            }
            string[] responseTokens = response.Split('&');
            foreach (string responseToken in responseTokens)
            {
                string[] nameValues = responseToken.Split('=');
                switch (nameValues[0])
                {
                    case "TOKEN":
                        this.Token = HttpUtility.UrlDecode(nameValues[1]);
                        break;
                    case "TIMESTAMP":
                        this.Timestamp = DateTime.Parse(HttpUtility.UrlDecode(nameValues[1]));
                        break;
                    case "CORRELATIONID":
                        this.CorrelationId = HttpUtility.UrlDecode(nameValues[1]);
                        break;
                    case "ACK":
                        this.Ack = (PaypalAckType)Enum.Parse(typeof(PaypalAckType),HttpUtility.UrlDecode(nameValues[1]));
                        break;
                    case "VERSION":
                        this.Version = Convert.ToInt32(HttpUtility.UrlDecode(nameValues[1]));
                        break;
                    case "BUILD":
                        this.Build = Convert.ToInt32(HttpUtility.UrlDecode(nameValues[1]));
                        break;
                    default:
                        if (errorNum > -1)
                        {
                            string key = nameValues[0];
                            string value = HttpUtility.UrlDecode(nameValues[1]);
                            int index = Convert.ToInt32(key.Substring(key.Length - 1, 1));
                            if (ppErrors[index] == null)
                                ppErrors[index] = new PaypalError();

                            key = key.Substring(0, key.Length - 1);
                            switch (key)
                            {
                                case "L_ERRORCODE":
                                    ppErrors[index].ErrorNo = Convert.ToInt32(value);
                                    break;
                                case "L_SHORTMESSAGE":
                                    ppErrors[index].ShortMessage = value;
                                    break;
                                case "L_LONGMESSAGE":
                                    ppErrors[index].LongMessage = value;
                                    break;
                                case "L_SEVERITYCODE":
                                    ppErrors[index].Severity = value;
                                    break;
                            }
                        }
                        break;
                }
            }
            foreach (PaypalError paypalError in ppErrors)
            {
                this.Errors.Add(paypalError);
            }
        }

        public string Token { get; set; }
        public DateTime Timestamp { get; set; }
        public string CorrelationId { get; set; }
        public PaypalAckType Ack { get; set; }
        public int Version { get; set; }
        public int Build { get; set; }
        public List<PaypalError> Errors { get; set; }
    }

    public class PaypalPayerInfo
    {
        public PaypalPayerInfo(){}
        public PaypalPayerInfo(string response)
        {
             string[] responseTokens = response.Split('&');
            foreach (string responseToken in responseTokens)
            {
                string[] nameValues = responseToken.Split('=');
                switch (nameValues[0])
                {
                    case "EMAIL":
                        this.Email = HttpUtility.UrlDecode(nameValues[1]);
                        break;
                    case "PAYERID":
                        this.PayerId = HttpUtility.UrlDecode(nameValues[1]);
                        break;
                    case "PAYERSTATUS":
                        this.PayerStatus = (PaypalPayerStatus)Enum.Parse(typeof(PaypalPayerStatus), HttpUtility.UrlDecode(nameValues[1]),true);
                        break;
                    case "COUNTRYCODE":
                        this.CountryCode = HttpUtility.UrlDecode(nameValues[1]);
                        break;
                    case "BUSINESS":
                        this.Business = HttpUtility.UrlDecode(nameValues[1]);
                        break;
                    case "FIRSTNAME":
                        this.Firstname = HttpUtility.UrlDecode(nameValues[1]);
                        break;
                    case "MIDDLENAME":
                        this.Middlename = HttpUtility.UrlDecode(nameValues[1]);
                        break;
                    case "LASTNAME":
                        this.Lastname = HttpUtility.UrlDecode(nameValues[1]);
                        break;
                    case "SUFFIX":
                        this.Suffix = HttpUtility.UrlDecode(nameValues[1]);
                        break;
                    case "PAYMENTREQUEST_0_SHIPTONAME":
                        this.ShipToName = HttpUtility.UrlDecode(nameValues[1]);
                        break;
                    case "PAYMENTREQUEST_0_SHIPTOSTREET":
                        this.ShipToStreet = HttpUtility.UrlDecode(nameValues[1]);
                        break;
                    case "PAYMENTREQUEST_0_SHIPTOSTREET2":
                        this.ShipToStreet2 = HttpUtility.UrlDecode(nameValues[1]);
                        break;
                    case "PAYMENTREQUEST_0_SHIPTOCITY":
                        this.ShipToCity = HttpUtility.UrlDecode(nameValues[1]);
                        break;
                    case "PAYMENTREQUEST_0_SHIPTOSTATE":
                        this.ShipToState = HttpUtility.UrlDecode(nameValues[1]);
                        break;
                    case "PAYMENTREQUEST_0_SHIPTOZIP":
                        this.ShipToZip = HttpUtility.UrlDecode(nameValues[1]);
                        break;
                    case "PAYMENTREQUEST_0_SHIPTOCOUNTRYCODE":
                        this.ShipToCountryCode = HttpUtility.UrlDecode(nameValues[1]);
                        break;
                    case "PAYMENTREQUEST_0_SHIPTOPHONENUM":
                        this.ShipToPhoneNum = HttpUtility.UrlDecode(nameValues[1]);
                        break;
                    case "PAYMENTREQUEST_0_ADDRESSSTATUS":
                        this.AddressStatus = (PaypalAddressStatus)Enum.Parse(typeof(PaypalAddressStatus), HttpUtility.UrlDecode(nameValues[1]), true);
                        break;
                }
            }
        }

        public string Email { get; set; }
        public string PayerId { get; set; }
        public PaypalPayerStatus  PayerStatus { get; set; }
        public string CountryCode { get; set; }
        public string Business { get; set; }
        public string Firstname { get; set; }
        public string Middlename { get; set; }
        public string Lastname { get; set; }
        public string Suffix { get; set; }
        public string ShipToName { get; set; }
        public string ShipToStreet { get; set; }
        public string ShipToStreet2 { get; set; }
        public string ShipToCity { get; set; }
        public string ShipToState { get; set; }
        public string ShipToZip { get; set; }
        public string ShipToCountryCode { get; set; }
        public string ShipToPhoneNum { get; set; }
        public PaypalAddressStatus AddressStatus { get; set; }
    }

    public class PaypalPaymentDetails
    {
        public PaypalPaymentDetails()
        {
            PaymentAction = "SALE";
        }

        public Decimal Amt { get; set; }
        public string CurrencyCode { get; set; }
        public string Desc { get; set; }
        public string Custom { get; set; }
        public Decimal TaxAmt { get; set; }
        public Decimal ItemAmt { get; set; }
        public Decimal HandlingAmt { get; set; }
        public Decimal ShippingAmt { get; set; }
        public Decimal InsuranceAmt { get; set; }
        public Decimal ShipDiscAmt { get; set; }
        public string InvNum { get; set; }
        public string InsuranceOptionOffered { get; set; }
        public string NotifyUrl { get; set; }
        public string NoteText { get; set; }
        public string TransactionId { get; set; }
        public string AllowedPaymentMethod { get; set; }
        public string PaymentRequestId { get; set; }
        public string PaymentAction { get; set; }

        public string ToPostData()
        {
            string postData = "";
            postData += HttpUtility.UrlEncode("PAYMENTREQUEST_0_PAYMENTACTION") + "=" + HttpUtility.UrlEncode(PaymentAction) + "&";
            if (Amt != 0) postData += HttpUtility.UrlEncode("PAYMENTREQUEST_0_AMT") + "=" + Amt.ToString("F", CultureInfo.InvariantCulture) + "&";
            if (CurrencyCode != string.Empty) postData += HttpUtility.UrlEncode("PAYMENTREQUEST_0_CURRENCYCODE") + "=" + HttpUtility.UrlEncode(CurrencyCode) + "&";
            if (Desc != string.Empty) postData += HttpUtility.UrlEncode("PAYMENTREQUEST_0_DESC") + "=" + HttpUtility.UrlEncode(Desc) + "&";
            if (ItemAmt != 0) postData += HttpUtility.UrlEncode("PAYMENTREQUEST_0_ITEMAMT") + "=" + ItemAmt.ToString("F", CultureInfo.InvariantCulture) + "&";
            if (TaxAmt!= 0) postData += HttpUtility.UrlEncode("PAYMENTREQUEST_0_TAXAMT") + "=" + TaxAmt.ToString("F", CultureInfo.InvariantCulture) + "&";
            if (HandlingAmt != 0) postData += HttpUtility.UrlEncode("PAYMENTREQUEST_0_HANDLINGAMT") + "=" + HandlingAmt.ToString("F", CultureInfo.InvariantCulture) + "&";
            if (ShippingAmt != 0) postData += HttpUtility.UrlEncode("PAYMENTREQUEST_0_SHIPPINGAMT") + "=" + ShippingAmt.ToString("F", CultureInfo.InvariantCulture) + "&";
            if (InvNum != string.Empty) postData += HttpUtility.UrlEncode("PAYMENTREQUEST_0_INVNUM") + "=" + HttpUtility.UrlEncode(InvNum) + "&";
            return postData;
        }
    }

    public class PaypalPaymentDetailsItem
    {
        public string Name { get; set; }
        public string Desc { get; set; }
        public Decimal Amt { get; set; }
        public int Number { get; set; }
        public int Qty { get; set; }
        public Decimal TaxAmt { get; set; }
        public int ItemWeightValue { get; set; }
        public string ItemWeightUnit { get; set; }
        public int ItemLengthValue { get; set; }
        public string ItemLengthUnit { get; set; }
        public int ItemWidthValue { get; set; }
        public string ItemWidthtUnit { get; set; }
        public int ItemHeightValue { get; set; }
        public string ItemHeightUnit { get; set; }
        public PaypalItemCategory ItemCategory { get; set; }
        public string ToPostData(int index)
        {
            string postData = "";
            if (Name != null) postData += HttpUtility.UrlEncode("L_PAYMENTREQUEST_0_NAME" + index.ToString()) + "=" + HttpUtility.UrlEncode(Name) + "&";
            if (Desc != null) postData += HttpUtility.UrlEncode("L_PAYMENTREQUEST_0_DESC" + index.ToString()) + "=" + HttpUtility.UrlEncode(Desc) + "&";
            if (Amt != 0) postData += HttpUtility.UrlEncode("L_PAYMENTREQUEST_0_AMT" + index.ToString()) + "=" + Amt.ToString("F", CultureInfo.InvariantCulture) + "&";
            if (TaxAmt != 0) postData += HttpUtility.UrlEncode("L_PAYMENTREQUEST_0_TAXAMT" + index.ToString()) + "=" + TaxAmt.ToString("F", CultureInfo.InvariantCulture) + "&";
            if (Number != 0) postData += HttpUtility.UrlEncode("L_PAYMENTREQUEST_0_NUMBER" + index.ToString()) + "=" + Number.ToString(CultureInfo.InvariantCulture) + "&";
            if (Qty != 0) postData += HttpUtility.UrlEncode("L_PAYMENTREQUEST_0_QTY" + index.ToString()) + "=" + Qty.ToString(CultureInfo.InvariantCulture) + "&";
            return postData;
        }
    }

    public class PaypalUserCredentials
    {
        public PaypalUserCredentials()
        {
        }

        public PaypalUserCredentials(string user, string password, string signature)
        {
            User = user;
            Password = password;
            Signature = signature;
        }

        public string User { get; set; }
        public string Password { get; set; }
        public string Signature { get; set; }

        public string ToPostData()
        {
            string postData = "";
            postData += HttpUtility.UrlEncode("USER") + "=" + HttpUtility.UrlEncode(this.User) + "&";
            postData += HttpUtility.UrlEncode("PWD") + "=" + HttpUtility.UrlEncode(this.Password) + "&";
            postData += HttpUtility.UrlEncode("SIGNATURE") + "=" + HttpUtility.UrlEncode(this.Signature) + "&";
            return postData;
        }
    }

    public class PaypalError
    {
        public PaypalError()
        {
        }

        public PaypalError(string shortMessage, string longMessage, string severity, int errorNo)
        {
            ShortMessage = shortMessage;
            LongMessage = longMessage;
            Severity = severity;
            ErrorNo = errorNo;
        }

        public string ShortMessage { get; set; }
        public string LongMessage { get; set; }
        public string Severity { get; set; }
        public int ErrorNo { get; set; }
    }

    public class PaypalSetExpressCheckoutParameters
    {
        public PaypalSetExpressCheckoutParameters()
        {
        }
        
        public PaypalSetExpressCheckoutParameters(string returnUrl, string cancelUrl, int noShipping, int allowNote)
        {
            ReturnUrl = returnUrl;
            CancelUrl = cancelUrl;
            NoShipping = noShipping;
            AllowNote = allowNote;
        }

        public string ReturnUrl { get; set; }
        public string CancelUrl { get; set; }
        public int NoShipping { get; set; }
        public int AllowNote { get; set; }

        public string ToPostData()
        {
            string postData = "";
            postData += HttpUtility.UrlEncode("RETURNURL") + "=" + HttpUtility.UrlEncode(this.ReturnUrl) + "&";
            postData += HttpUtility.UrlEncode("CANCELURL") + "=" + HttpUtility.UrlEncode(this.CancelUrl) + "&";
            postData += HttpUtility.UrlEncode("NOSHIPPING") + "=" + this.NoShipping.ToString() + "&";
            postData += HttpUtility.UrlEncode("ALLOWNOTE") + "=" + this.AllowNote.ToString() + "&";
            return postData;
        }
    }

    public class PaypalPaymentResult
    {
        public PaypalPaymentResult()
        {
        }
        
        public PaypalPaymentResult(string response)
        {
            string[] responseTokens = response.Split('&');
            foreach (string responseToken in responseTokens)
            {
                string[] nameValues = responseToken.Split('=');
                switch (nameValues[0])
                {
                    case "INSURANCEOPTIONSELECTED":
                        this.InsuranceOptionSelected = Convert.ToBoolean( HttpUtility.UrlDecode(nameValues[1]));
                        break;
                    case "SHIPPINGOPTIONISDEFAULT":
                        this.ShippingOptionIsDefault = Convert.ToBoolean( HttpUtility.UrlDecode(nameValues[1]));
                        break;
                    case "PAYMENTINFO_0_TRANSACTIONID":
                        this.TransactionId = HttpUtility.UrlDecode(nameValues[1]);
                        break;
                    case "PAYMENTINFO_0_TRANSACTIONTYPE":
                        this.TransactionType = HttpUtility.UrlDecode(nameValues[1]);
                        break;
                    case "PAYMENTINFO_0_PAYMENTTYPE":
                        this.PaymentType = HttpUtility.UrlDecode(nameValues[1]);
                        break;
                    case "PAYMENTINFO_0_ORDERTIME":
                        this.OrderTime = DateTime.Parse(HttpUtility.UrlDecode(nameValues[1]));
                        break;
                    case "PAYMENTINFO_0_AMT":
                        this.Amt = Decimal.Parse(HttpUtility.UrlDecode(nameValues[1]),CultureInfo.InvariantCulture);
                        break;
                     case "PAYMENTINFO_0_FEEAMT":
                        this.FeeAmt = Decimal.Parse(HttpUtility.UrlDecode(nameValues[1]),CultureInfo.InvariantCulture);
                        break;
                     case "PAYMENTINFO_0_TAXAMT":
                        this.TaxAmt = Decimal.Parse(HttpUtility.UrlDecode(nameValues[1]),CultureInfo.InvariantCulture);
                        break;
                    case "PAYMENTINFO_0_CURRENCYCODE":
                        this.CurrencyCode = HttpUtility.UrlDecode(nameValues[1]);
                        break;
                    case "PAYMENTINFO_0_PAYMENTSTATUS":
                        this.PaymentStatus = (PaypalPaymentStatus)Enum.Parse(typeof(PaypalPaymentStatus), HttpUtility.UrlDecode(nameValues[1]).Replace("-",""), true);
                        break;
                    case "PAYMENTINFO_0_PENDINGREASON":
                        this.PendingReason = (PaypalPendingReason)Enum.Parse(typeof(PaypalPendingReason), HttpUtility.UrlDecode(nameValues[1]).Replace("-",""), true);
                        break;
                    case "PAYMENTINFO_0_REASONCODE":
                        this.ReasonCode = (PaypalReasonCode)Enum.Parse(typeof(PaypalReasonCode), HttpUtility.UrlDecode(nameValues[1]).Replace("-",""), true);
                        break;
                    case "PAYMENTINFO_0_PROTECTIONELIGIBILITY":
                        this.ProtectionEgilibility = (PaypalProtectionEligibility)Enum.Parse(typeof(PaypalProtectionEligibility), HttpUtility.UrlDecode(nameValues[1]).Replace("-",""), true);
                        break;
                    case "PAYMENTINFO_0_PROTECTIONELIGIBILITYTYPE":
                        this.ProtectionEgilibilityType = (PaypalProtectionEligibilityType)Enum.Parse(typeof(PaypalProtectionEligibilityType), HttpUtility.UrlDecode(nameValues[1]).Replace("-",""), true);
                        break;
                    case "PAYMENTINFO_0_SECUREMERCHANTACCOUNTID":
                        this.SecureMerchantAccountId = HttpUtility.UrlDecode(nameValues[1]);
                        break;
                    case "PAYMENTINFO_0_ERRORCODE":
                        this.ErrorCode = Convert.ToInt32(HttpUtility.UrlDecode(nameValues[1]));
                        break;
                    case "PAYMENTINFO_0_ACK":
                        this.Ack = (PaypalAckType)Enum.Parse(typeof(PaypalAckType), HttpUtility.UrlDecode(nameValues[1]).Replace("-",""), true);
                        break;

                }
            }
        }

        public bool InsuranceOptionSelected { get; set; }
        public bool ShippingOptionIsDefault { get; set; }
        public string TransactionId { get; set; }
        public string TransactionType { get; set; }
        public string PaymentType { get; set; }
        public DateTime OrderTime { get; set; }
        public Decimal Amt { get; set; }
        public Decimal FeeAmt { get; set; }
        public Decimal TaxAmt { get; set; }
        public string CurrencyCode { get; set; }
        public PaypalPaymentStatus PaymentStatus { get; set; }
        public PaypalPendingReason PendingReason { get; set; }
        public PaypalReasonCode ReasonCode  { get; set; }
        public PaypalProtectionEligibility ProtectionEgilibility { get; set; }
        public PaypalProtectionEligibilityType ProtectionEgilibilityType { get; set; }
        public string SecureMerchantAccountId { get; set; }
        public int ErrorCode { get; set; }
        public PaypalAckType Ack { get; set; }
    }
}
