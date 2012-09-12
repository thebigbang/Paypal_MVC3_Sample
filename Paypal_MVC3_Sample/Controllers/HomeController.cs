using System.Linq;
using System.Web.Mvc;
using Payment;
using Paypal_MVC3_Sample.Models;

namespace Paypal_MVC3_Sample.Controllers
{
    /// <summary>
    /// Sample program to learn how to use paypal in Asp.Net mvc3(+)
    /// Session["PanierCommande"] the shopping cart,
    /// Session["InformationLivraison"] shipping Information (address,shipping mode with price),
    /// Session["InformationPaiement"] payment information(payment method choosen, billing address)
    /// </summary>
    public class HomeController : Controller
    {
        /// <returns></returns>
        public ActionResult Index()
        {
            ViewBag.Message = "Welcome to The Paypal MVC3 Sample Program.";
            return View();
        }
        /// <summary>
        /// Panier auto généré
        /// </summary>
        /// <returns></returns>
        public ActionResult ShoppingCart()
        {
            return View(ShoppingCartModel.Generate());
        }

        /// <summary>
        /// résultat post du panier autogénéré
        /// </summary>
        /// <param name="model"></param>
        /// <param name="xPressCheckout"> </param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ShoppingCart(ShoppingCartModel model)
        {
            Session["PanierCommande"] = ShoppingCartModel.Generate(); //on triche mais tant pi, on peut pas aller plus vite ce soir ... :s
            /*     if (xPressCheckout)
                 {
                     return ExpressCheckout();
                 }*/
            return RedirectToAction("Checkout");
        }

        /// <summary>
        /// Not used right now.
        /// A utiliser si le post du shoppingCart donne un résultat "XpressCheckout" de cliqué.
        /// </summary>
        [HttpPost]
        public ActionResult ExpressCheckout()
        {
            ShoppingCartModel shoppingCart = ShoppingCartModel.Generate();

            decimal shoppingcartfinalprice = shoppingCart.ProductToOrders.Sum(productQtt => productQtt.Produits.Prix * productQtt.Quantity);

            Session["payment_amt"] = shoppingcartfinalprice; //on stock le prix final avec fdp

            NVPAPICaller test = new NVPAPICaller();
            string retMsg = "";
            string token = "";

            if (System.Web.HttpContext.Current.Session["payment_amt"] != null)
            {
                string amt = Session["payment_amt"].ToString();
                amt = amt.Replace(',', '.');
                bool ret = test.ShortcutExpressCheckout(amt, ref token, ref retMsg);
                if (ret)
                {
                    Session["token"] = token;
                    Response.Redirect(retMsg);
                }
                else
                {
                    return RedirectToAction("Index", "PaypalErrors", new { ErrorCode = retMsg });
                    //              Response.Redirect("APIError.aspx?" + retMsg);
                }
            }
            else
            {
                return RedirectToAction("Index", "PaypalErrors", new { ErrorCode = "AmtMissing" });
                //          Response.Redirect("APIError.aspx?ErrorCode=AmtMissing");
            }
            Session["ShortcutPaypalCheckout"] = true;
            return View("Index");
        }


        /// <summary>
        /// On procède au choix du mode de livraison et son cout.
        /// </summary>
        /// <returns></returns>
        public ActionResult Checkout()
        {
            return View(LivraisonsModel.Generate());
        }
        /// <summary>
        /// Post du choix de livraison avec adresse, cout, mode de livraison
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Checkout(LivraisonsModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(LivraisonsModel.Generate());
            }
            Session["InformationLivraison"] = model;
            return RedirectToAction("Payment");
        }

        /// <summary>
        /// Page proposant plusieurs mode de paiement... dont paypal
        /// </summary>
        /// <returns></returns>
        public ActionResult Payment()
        {
            return View(PaiementModel.Generate());
        }
        /// <summary>
        /// Post du paiement, avec la partie concernant l'initialisation de paypal
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Payment(PaiementModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(PaiementModel.Generate());
            }
            LivraisonsModel livraisons = (LivraisonsModel)Session["InformationLivraison"];

            ShoppingCartModel shoppingCart = (ShoppingCartModel)Session["PanierCommande"];
            if (shoppingCart == null)
            {
                return View("Error");
            }
            decimal shoppingcartfinalprice = shoppingCart.ProductToOrders.Sum(productQtt => productQtt.Produits.Prix * productQtt.Quantity);

            shoppingcartfinalprice += LivraisonsModel.Generate().Fournisseurs.Single(f => f.Nom == livraisons.FournisseurName).Prix;

            Session["payment_amt"] = shoppingcartfinalprice; //on stock le prix final avec fdp

            Session["InformationPaiement"] = model;
            switch (model.MethodePaiementDefinie)
            {
                case "Cheque":
                    break; //on a choisit le paiement en cheque...
                case "CarteCredit":
                    break;//on a choisit le paiement en CB
                case "Paypal"://Paypal is choosen: the one explained in this sample.
                    {
                        string token = "", returnedMsg = "";
                        if (!Paypal.Instantiation(shoppingcartfinalprice, ref token, ref returnedMsg, livraisons.AddresseLivraison))
                        {
                            //enable to send an error email about paypal. Not used in this sample.
                            //new SendMail.Paypal().Error(returnedMsg + "    erreur survenue au moment de l'instanciation de la transaction");
                            return RedirectToAction("Index", "PaypalErrors", new { ErrorCode = returnedMsg });
                        }
                        Session["token"] = token;
                        Response.Redirect(returnedMsg);
                        return Index();
                    }
                case "SurPlace":
                    break;//on a choisit le paiement sur place.
            }
            return View("Thanks", new ValidationStatus { Status = "Payment Pending" });
        }

        /// <summary>
        /// Page de confirmation de commande. affiche un récap et le bouton confirmation terminant le paiement...
        /// </summary>
        /// <returns></returns>
        public ActionResult Confirmation(string PayerID)
        {
            Session["payerId"] = PayerID;
            #region unusuedXpressCheckout
            /*ZONE à appeler en cas d'express checkout: */
            //note: non utilisé pour le moment. cleanup code et validation fonctionnement avant.
            /*  if ((bool)Session["ShortcutPaypalCheckout"])
              {
                  NVPAPICaller test = new NVPAPICaller();

                  string retMsg = "";
                  string token = "";
                  string payerId = "";
                  string shippingAddress = ""; //parser le string en objet interne
                  token = Session["token"].ToString();

                  bool ret = test.GetShippingDetails(token, ref payerId, ref shippingAddress, ref retMsg);
                  if (ret)
                  {
                      Session["payerId"] = payerId;
                      Response.Write(shippingAddress);
                  }
                  else
                  {
                      Response.Redirect("APIError.aspx?" + retMsg);
                  }
              }*/
            /*fin de zone */
            #endregion
            RecapitulatifCommandeModel model = new RecapitulatifCommandeModel
                                                 {
                                                     Pannier = (ShoppingCartModel)Session["PanierCommande"],
                                                     Livraison = (LivraisonsModel)Session["InformationLivraison"],
                                                     Paiement = (PaiementModel)Session["InformationPaiement"]
                                                 };
            return View(model);
        }
        /// <summary>
        /// Post de confirmation de commande, suite à quoi on stock en db la commande et valide le paiement aupres de paypal.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Confirmation(RecapitulatifCommandeModel model)
        {

            decimal finalPaymentAmount = decimal.Parse(Session["payment_amt"].ToString());
            string token = Session["token"].ToString();
            string payerId = Session["payerId"].ToString();
            string returnedMsg = "";
            if (Paypal.OrderConfirmation(token, payerId, finalPaymentAmount, ref returnedMsg) != "VALIDATED")
            {
                //enable to send email about paypal error, not used in this sample.
                //new SendMail.Paypal().Error(returnedMsg + "    erreur survenue au moment de la confirmation du paiement.");
                return RedirectToAction("Index", "PaypalErrors", new { errorCode = returnedMsg });
            }
            //if we are here everything is ok;
            ValidationStatus v = new ValidationStatus { Status = "Validated" };
            return View("Thanks", v);
        }
    }
}
