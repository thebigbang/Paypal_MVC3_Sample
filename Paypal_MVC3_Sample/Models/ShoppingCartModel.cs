using System.Collections.Generic;
using Objects;

namespace Paypal_MVC3_Sample.Models
{
 
    public class ProductToOrder
    {
        public Produits Produits { get; set; }
        public int Quantity { get; set; }
    }
    public class ShoppingCartModel
    {
        public IEnumerable<ProductToOrder> ProductToOrders { get; set; }
        public static ShoppingCartModel Generate()
        {
            return new ShoppingCartModel
                       {
                           ProductToOrders = new[]{
                           new ProductToOrder
                               {
                                   Produits = new Produits
                                                        {
                                                            Nom = "Livre",
                                                            Prix = new decimal(14.4)
                                                        },
                                   Quantity = 3
                               },
                           new ProductToOrder
                               {
                                   Produits = new Produits
                                                        {
                                                            Nom = "Dessin",
                                                            Prix = new decimal(25)
                                                        },
                                   Quantity = 1
                               },
                           new ProductToOrder
                               {
                                   Produits = new Produits
                                                        {
                                                            Nom = "Petit Bol",
                                                            Prix = new decimal(8.7)
                                                        },
                                   Quantity = 6
                               },
                           new ProductToOrder
                               {
                                   Produits = new Produits
                                                        {
                                                            Nom = "Boite à crayon",
                                                            Prix = new decimal(18)
                                                        },
                                   Quantity = 2
                               }}
                       };
        }


    }
}