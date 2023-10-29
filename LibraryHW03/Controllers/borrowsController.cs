using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using LibraryHW03.Models;

namespace LibraryHW03.Controllers
{
    public class borrowsController : Controller
    {
        private LibraryEntities db = new LibraryEntities();

        // GET: borrows
        public ActionResult Index()
        {
            var borrows = db.borrows.Include(b => b.books).Include(b => b.students);
            return View(borrows.ToList());
        }

        // GET: borrows/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            borrows borrows = db.borrows.Find(id);
            if (borrows == null)
            {
                return HttpNotFound();
            }
            return View(borrows);
        }

        // GET: borrows/Create
        public ActionResult Create()
        {
            ViewBag.bookId = new SelectList(db.books, "bookId", "name");
            ViewBag.studentId = new SelectList(db.students, "studentId", "name");
            return View();
        }

        // POST: borrows/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "borrowId,studentId,bookId,takenDate,broughtDate")] borrows borrows)
        {
            if (ModelState.IsValid)
            {
                db.borrows.Add(borrows);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.bookId = new SelectList(db.books, "bookId", "name", borrows.bookId);
            ViewBag.studentId = new SelectList(db.students, "studentId", "name", borrows.studentId);
            return View(borrows);
        }

        // GET: borrows/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            borrows borrows = db.borrows.Find(id);
            if (borrows == null)
            {
                return HttpNotFound();
            }
            ViewBag.bookId = new SelectList(db.books, "bookId", "name", borrows.bookId);
            ViewBag.studentId = new SelectList(db.students, "studentId", "name", borrows.studentId);
            return View(borrows);
        }

        // POST: borrows/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "borrowId,studentId,bookId,takenDate,broughtDate")] borrows borrows)
        {
            if (ModelState.IsValid)
            {
                db.Entry(borrows).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.bookId = new SelectList(db.books, "bookId", "name", borrows.bookId);
            ViewBag.studentId = new SelectList(db.students, "studentId", "name", borrows.studentId);
            return View(borrows);
        }

        // GET: borrows/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            borrows borrows = db.borrows.Find(id);
            if (borrows == null)
            {
                return HttpNotFound();
            }
            return View(borrows);
        }

        // POST: borrows/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            borrows borrows = db.borrows.Find(id);
            db.borrows.Remove(borrows);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
