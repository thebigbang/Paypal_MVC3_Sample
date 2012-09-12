using System.Globalization;
using Objects;

namespace Payment
{
    /// <summary>
    /// Class enabling paypal transactions.
    /// Everyrhing is for express checkout right now.
    /// </summary>
    public class Paypal
    {
        /// <summary>
        /// Enable the order creation on paypal's side and to log in the customer on it's paypal account.
        /// Get mandatory informations about customer and it's paypal account.
        /// </summary>
        /// <param name="orderAmount">amount of the order, let paypal know a one point the amount to pay.</param>
        /// <param name="token">internal paypal/shop transaction id</param>
        /// <param name="returnedMsg">Error message send by paypal</param>
        /// <param name="shippingAddress">Shipping Address: optionnal field, but usefull for the customer.</param>
        /// <returns></returns>
        public static bool Instantiation(decimal orderAmount, ref string token, ref string returnedMsg, Addresse shippingAddress = null)
        {
            NVPAPICaller test = new NVPAPICaller();

            bool ret = false;
            if (orderAmount > 0)
            {
                string amt = orderAmount.ToString(CultureInfo.InvariantCulture).Replace(',', '.');
                //Optional Shipping Address entered on the merchant site
                string shipToName = "";
                if (shippingAddress != null)
                {
                    if (!string.IsNullOrEmpty(shippingAddress.Identity.Societe))
                    {
                        shipToName = shippingAddress.Identity.Societe + " ";
                    }
                    shipToName += shippingAddress.Identity.Prenom + " " +
                                  shippingAddress.Identity.Nom;
                    string shipToStreet = shippingAddress.MainStreet;
                    string shipToStreet2 = shippingAddress.SubStreet;
                    string shipToCity = shippingAddress.City;
                    string shipToState = ""; //parceque france
                    string shipToZip = shippingAddress.PostCode;
                    //postcode et pas zip parceque france
                    string shipToCountryCode = "FR";
                    //ref: https://cms.paypal.com/us/cgi-bin/?cmd=_render-content&content_ID=developer/e_howto_api_nvp_country_codes

                    ret = test.MarkExpressCheckout(amt, shipToName, shipToStreet, shipToStreet2,
                                                        shipToCity, shipToState, shipToZip, shipToCountryCode,
                                                        ref token, ref returnedMsg);
                }
                else
                {
                    ret = test.MarkExpressCheckout(amt, "", "", "", "", "", "", "", ref token, ref returnedMsg);
                }
                return ret;/*
                            if (ret)
                            {
                             //   retToken = token;
                                return ret;
                                Session["token"] = token;

                                Response.Redirect(retMsg);
                            }
                            else
                            {
                                return RedirectToAction("Index", "PaypalErrors", new { ErrorCode = returnedMsg });
                                //                    Response.Redirect("APIError.aspx?" + retMsg);
                            }*/
            }
            returnedMsg =
                "Une erreur est survenue lors du calcul de la somme de votre panier. Merci de renouveler l'opération.";
            return ret;
        }
        /// <summary>
        /// Permet de confirmer la commande auprès de Paypal et de débiter le compte du client.
        /// </summary>
        /// <param name="token">id transaction-paypal</param>
        /// <param name="payerId">id du compte du client</param>
        /// <param name="finalAmount">somme finale, au cas ou le panier aurait été mis à jour entre temps ou modifié frauduleusement.</param>
        /// <param name="returnedMsg">message de retour, "" si pas d'erreur. (à gérer coté controller)</param>
        /// <returns>renvoi validated si ok et nonvalidated si erreur.</returns>
        public static string OrderConfirmation(string token, string payerId, decimal finalAmount, ref string returnedMsg)
        {
            /*DEBUT PAYPAL */
            NVPAPICaller test = new NVPAPICaller();

            string retMsg = "";
            NVPCodec decoder = new NVPCodec();
            string finalAmountStr = finalAmount.ToString(CultureInfo.InvariantCulture).Replace(',', '.');
            bool ret = test.ConfirmPayment(finalAmountStr, token, payerId, ref decoder, ref retMsg);
            if (ret)
            {
                // Unique transaction ID of the payment. Note:  If the PaymentAction of the request was Authorization or Order, this value is your AuthorizationID for use with the Authorization & Capture APIs. 
                string transactionId = decoder["TRANSACTIONID"];

                // The type of transaction Possible values: l  cart l  express-checkout 
                string transactionType = decoder["TRANSACTIONTYPE"];

                // Indicates whether the payment is instant or delayed. Possible values: l  none l  echeck l  instant 
                string paymentType = decoder["PAYMENTTYPE"];

                // Time/date stamp of payment
                string orderTime = decoder["ORDERTIME"];

                // The final amount charged, including any shipping and taxes from your Merchant Profile.
                string amt = decoder["AMT"];

                // A three-character currency code for one of the currencies listed in PayPay-Supported Transactional Currencies. Default: USD.    
                string currencyCode = decoder["CURRENCYCODE"];

                // PayPal fee amount charged for the transaction    
                string feeAmt = decoder["FEEAMT"];

                // Amount deposited in your PayPal account after a currency conversion.    
                string settleAmt = decoder["SETTLEAMT"];

                // Tax charged on the transaction.    
                string taxAmt = decoder["TAXAMT"];

                //' Exchange rate if a currency conversion occurred. Relevant only if your are billing in their non-primary currency. If 
                string exchangeRate = decoder["EXCHANGERATE"];

                //enable to send a mail easily. Not used in that sample.
                //new SendMail.Paypal().TransactionDone(transactionId, transactionType, paymentType, orderTime, amt, currencyCode, feeAmt, settleAmt, taxAmt, exchangeRate);

                return "VALIDATED";
            }
            returnedMsg = retMsg;
            return "NonValidated";
            //return RedirectToAction("Index", "PaypalErrors", new { ErrorCode = retMsg });
        }
    }
}
