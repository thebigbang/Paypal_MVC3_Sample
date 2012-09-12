using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Objects;

namespace Paypal_MVC3_Sample.Models
{
    public enum MethodePaiementEnum
    {
        CarteCredit,
        Cheque,
        SurPlace,
        Paypal //lui qui nous interesse ici :)
    }
    public class PaiementModel
    {
        public List<string> MethodePaiement { get; set; }
        public Addresse AddresseFacturation { get; set; }
        [Required(ErrorMessage = "You must choose a payment Method.")]
        public string MethodePaiementDefinie { get; set; }

        public static PaiementModel Generate()
        {
            return new PaiementModel
                       {
                           AddresseFacturation = new Addresse
                                                     {
                                                         City = "FactureVille",
                                                         Identity = new Identity
                                                                        {
                                                                            Genre = "Madame", Nom = "NomFacture",Prenom = "PrenomFacture",Societe = "SARL FactureCorp"
                                                                        },
                                                                        MainStreet = "Rue Principale de Facture4",
                                                                        SubStreet = "allee de facturette dans le vent",
                                                                        PostCode = "99988"
                                                     },
                                                     MethodePaiement = new List<string>
                                                                           {
                                                                               "CarteCredit",
                                                                               "Cheque",
                                                                               "SurPlace",
                                                                               "Paypal"
                                                                           }
                       };
        }
    }
}