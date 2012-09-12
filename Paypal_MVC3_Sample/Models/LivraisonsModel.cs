using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Objects;

namespace Paypal_MVC3_Sample.Models
{
    public class LivraisonsModel
    {
        public List<Fournisseur> Fournisseurs { get; set; }
        public Addresse AddresseLivraison { get; set; }
        [Required(ErrorMessage = "You must choose a shipping method before being able to continue.")]
        public string FournisseurName { get; set; }

        /// <summary>
        /// Generate generic shipping informations
        /// </summary>
        /// <returns></returns>
        public static LivraisonsModel Generate()
        {
            return new LivraisonsModel
                       {
                           Fournisseurs = new List<Fournisseur>
                                              {
                                                  new Fournisseur
                                                      {
                                                          Nom = "Colissimo Sans Assurance",
                                                          PoidsMaxi = 25,
                                                          Prix = new decimal(5.70)
                                                      },
                                                      new Fournisseur
                                                          {
                                                              Nom = "Colissimo Avec Assurance",
                                                              PoidsMaxi = 25,
                                                              Prix = new decimal(10.93)
                                                          },
                                                          new Fournisseur
                                                              {
                                                                  Nom = "Sur Place",
                                                                  PoidsMaxi = -1,
                                                                  Prix = 0
                                                              }
                                              },
                                              AddresseLivraison = new Addresse
                                                                      {
                                                                          City = "TestVille",
                                                                          MainStreet = "Rue Principale n°2",
                                                                          PostCode = "13580",
                                                                          Identity = new Identity
                                                                                         {
                                                                                             Genre = "Monsieur",
                                                                                             Nom = "NomTest",
                                                                                             Prenom = "PrenomTest"
                                                                                         }
                                                                      }
                       };
        }
    }
}