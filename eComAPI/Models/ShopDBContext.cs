using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using eComAPI.Data.Entity;

namespace eComAPI.Models
{
    public class html
    {
        private string _content = "";
        public html(string pagecontent)
        {
            _content = pagecontent;
        }

        public override string ToString()
        {
            return _content;
        }
    }
    public class ShopDBContext : IDisposable
    {

        public StoreAP<Product> Products;

        public StoreAP<ProductCollection> ProductCollections;

        public StoreAP<Member> Members;
        
        public ShopDBContext()
        {
            //---------------------------------------------------------
            //--   .Initialize Data Stores.
            //--   .TODO: add Actual DataStore
            //--   .      underlying DB Store should be "Singularity".

            Members = new StoreAP<Member>();
            Products = new StoreAP<Product>();
            ProductCollections = new StoreAP<ProductCollection>();

            //---------------------------------------------------------
        }

        public void DefaultData()
        {
            //Create default admin user login
            Member newUser = new Member();
            newUser.MemberName = "akutra";
            newUser.PasswordHash = "";
            newUser.CreateDate = DateTime.Now;
            //add the user
            Members.Add(newUser);

            //-----------------------------------------
            //- Tests

            //QEntities<Member> qEntities = new QEntities<Member>(Members.ToArray());
            //qEntities.Where(q => q.MemberName.ToLower() == "akutra").Select(q => q);

            //var userEntity = Members.Entry(newUser);
            //int mId = (int)userEntity.Property("MemberId").CurrentValue;

            //userEntity.CurrentValues["PasswordHash"] = "Pa$$word";
            //userEntity.Property("PasswordHash").CurrentValue = "Passw0rd";
            //-----------------------------------------

            //create default collection
            ProductCollection newCollection = new ProductCollection();
            newCollection.Name = "eBooks";
            newCollection.Image = "books_249x166.png";
            //add the collection, return has the autoincrement ID field populated
            ProductCollection cCol = ProductCollections.Add(newCollection);

            //create a default product
            Product newProduct = new Product();
            newProduct.Name = "Top 29 Profit Boosting Tools";
            newProduct.CollectionId = cCol.CollectionId;
            newProduct.defImage = "29Tools_249px.png";
            newProduct.SKU = "EB0001";
            newProduct.Description = "Finally, be able to set up a profitable online business online business in the fastest and easiest way possible. Save yourself a ton of research and confusion with inferior tools & software products.";
            newProduct.Access = @"https://www.akutra-ramses.solutions/profitboostingtools/";
            newProduct.Match = "eBook";
            newProduct.ItemType = "business";
            //add the new product
            Products.Add(newProduct);

            //create a default product
            newProduct = new Product();
            newProduct.Name = "Powers Of The Mind and The Secret to Living";
            newProduct.CollectionId = cCol.CollectionId;
            newProduct.defImage = "Spoon_249px.png";
            newProduct.SKU = "EB0002";
            newProduct.Description = "Follow this simple step-by-step methods to discover amazing ways to actualize you dreams and desires. Save yourself a ton of research and confusion by avoiding other fad logics.";
            newProduct.Access = @"https://www.akutra-ramses.solutions/selfactualization/";
            newProduct.Match = "eBook";
            newProduct.ItemType = "personal development";
            //add the new product
            Products.Add(newProduct);

            //create a default product
            newProduct = new Product();
            newProduct.Name = "Manifesting with The Law of Attraction";
            newProduct.CollectionId = cCol.CollectionId;
            newProduct.defImage = "LOA_249px.png";
            newProduct.SKU = "EB0003";
            newProduct.Description = "Follow these proven Law Of Attraction strategies to manifest your dreams in your life. Save yourself a ton of research and confusion with these proven-to-work strategies.";
            newProduct.Access = @"https://www.akutra-ramses.solutions/manifestvialoa/";
            newProduct.Match = "eBook";
            newProduct.ItemType = "personal development";
            //add the new product
            Products.Add(newProduct);

            //create a default product
            newProduct = new Product();
            newProduct.Name = "Feeling Fuller For Longer";
            newProduct.CollectionId = cCol.CollectionId;
            newProduct.defImage = "FeelingFuller_249px.png";
            newProduct.SKU = "EB0004";
            newProduct.Description = "Follow this simple step-by-step method to avoid overeating and see faster results. Save yourself a ton of research and confusion by avoiding other fad diets.";
            newProduct.Access = @"https://www.akutra-ramses.solutions/weightloss/";
            newProduct.Match = "eBook";
            newProduct.ItemType = "weight loss";
            //add the new product
            Products.Add(newProduct);

            //-----------------------------------------
            //- Tests

            //Products.Where(q => q.Name.ToLower().Contains("the")).Select(q => q.SKU);
            //QEntities<Product> qEntities = new QEntities<Product>(Products.ToArray());
            //qEntities.Where(q => q.Name.ToLower().Contains("the") ).Select(q => q.SKU);
            //-----------------------------------------
        }

        #region Dispose
        private bool disposed = false; // to detect redundant calls

        private void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    //nothing here for the moment...
                }

                disposed = true;
            }
        }

        ~ShopDBContext()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}