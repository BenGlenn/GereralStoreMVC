using GeneralStore.MVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GeneralStore.MVC.Controllers
{
    public class TransactionController : Controller
    {
        // Add the application DB Context (Link to the Database)
        private ApplicationDbContext _db = new ApplicationDbContext();

        // GET: Transaction
        public ActionResult Index()
        {
            List<Transaction> transactionList = _db.Transactions.ToList();
            List<Transaction> orderedList = transactionList.OrderBy(prod => prod.ProductID).ToList();
            return View(orderedList);
        }
        // Get: Transaction 
        public ActionResult Create()
        {
            var customers = new SelectList(_db.Customers.ToList(), "CustomerID", "FullName");
            ViewBag.Customers = customers;
            var product = new SelectList(_db.Products.ToList(), "ProductID", "Name");
            ViewBag.Products = product;
            return View();
        }

        // Post: Transaction

        [HttpPost]
        public ActionResult Create(Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                Customer customer = _db.Customers.Find(transaction.CustomerID);
                if (customer == null)
                    return HttpNotFound();
                Product product = _db.Products.Find(transaction.ProductID);
                if (product == null)
                    return HttpNotFound();

                _db.Transactions.Add(transaction);
                product.InventoryCount -= transaction.ItemCount;
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(transaction);
        }

        // GET : Delete
        // Transaction/Delete/{id}
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }
            Transaction transaction = _db.Transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            return View(transaction);
        }
        // POST : Delete
        // Transaction/Delete/{id}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            Transaction transaction = _db.Transactions.Find(id);
            Product product = _db.Products.Find(transaction.ProductID);
            _db.Transactions.Remove(transaction);
            product.InventoryCount += transaction.ItemCount;
            _db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}