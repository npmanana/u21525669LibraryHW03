using LibraryHW03.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using PagedList.EntityFramework;
using System.Data.Entity.Core.Metadata.Edm;
using Newtonsoft.Json;

namespace LibraryHW03.Controllers
{
    public class HomeController : Controller
    {
        LibraryEntities db = new LibraryEntities();
        public async Task<ActionResult> HomeScreenIndex(int? studentPage, int? bookPage)
        {
            int pageSize = 10; // Number of items to display per page

            // Calculate the current page for students and books separately
            int studentPageNumber = studentPage ?? 1;
            int bookPageNumber = bookPage ?? 1;

            // Fetch paginated students and books
            var studentsData = await db.students.OrderBy(s => s.studentId)
           .ToPagedListAsync(studentPageNumber, pageSize);



            var booksData = db.books.Include(b => b.authors).Include(b => b.types)
                .OrderBy(b => b.bookId)
                .ToPagedList(bookPageNumber, pageSize);


            var viewModel = new CombinedViewModel
            {
                Students = studentsData,
                Books = booksData.ToList(), // Convert to a list
                StudentCurrentPage = studentPageNumber, // Include current page numbers in the view model
                BookCurrentPage = bookPageNumber,
                StudentTotalPages = (int)Math.Ceiling((double)db.students.Count() / pageSize), // Calculate total pages for students
                BookTotalPages = (int)Math.Ceiling((double)db.books.Count() / pageSize) // Calculate total pages for books
            };

            return View(viewModel);
        }




        public async Task<ActionResult> MaintainScreenIndex(int? authorPage, int? typePage, int? borrowPage)
        {
            int pageSize = 10; // Number of items to display per page

            int authorPageNumber = authorPage ?? 1;
            int typePageNumber = typePage ?? 1;
            int borrowPageNumber = borrowPage ?? 1;

            var authorsData = db.authors.OrderBy(a => a.authorId)
                .ToPagedList(authorPageNumber, pageSize);

            var typesData = db.types.OrderBy(t => t.typeId)
                .ToPagedList(typePageNumber, pageSize);

            var borrowsData = db.borrows.OrderBy(b => b.borrowId)
                .ToPagedList(borrowPageNumber, pageSize);

            var viewModel = new CombinedViewModel
            {
                Authors = authorsData,
                Types = typesData,
                Borrows = borrowsData,
                AuthorCurrentPage = authorPageNumber,
                TypeCurrentPage = typePageNumber,
                BorrowCurrentPage = borrowPageNumber,
                AuthorTotalPages = (int)Math.Ceiling((double)db.authors.Count() / pageSize),
                TypeTotalPages = (int)Math.Ceiling((double)db.types.Count() / pageSize),
                BorrowTotalPages = (int)Math.Ceiling((double)db.borrows.Count() / pageSize)
            };

            return View(viewModel);
        }



        public ActionResult PopularBooksReport()
        {
            var popularBooks = db.borrows
                .GroupBy(b => b.bookId)
                .Select(g => new
                {
                    BookId = g.Key,
                    BorrowCount = g.Count(),
                    BookName = g.FirstOrDefault().books.name
                })
                .OrderByDescending(b => b.BorrowCount)
                .Take(10);

            var popularBooksJson = JsonConvert.SerializeObject(popularBooks);

            ViewBag.PopularBooksData = popularBooksJson;


            return View();
        }




        ///  B O O K S
        public ActionResult BooksIndex()
        {
            var books = db.books.Include(b => b.authors).Include(b => b.types);
            return View(books.ToList());
        }

        // GET: books/Details/5
        public ActionResult BooksDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            books books = db.books.Find(id);
            if (books == null)
            {
                return HttpNotFound();
            }
            return View(books);
        }

        // GET: books/Create
        public ActionResult BooksCreate()
        {
            ViewBag.authorId = new SelectList(db.authors, "authorId", "name");
            ViewBag.typeId = new SelectList(db.types, "typeId", "name");
            return View();
        }

        // POST: books/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult BooksCreate([Bind(Include = "bookId,name,pagecount,point,authorId,typeId")] books books)
        {
            if (ModelState.IsValid)
            {
                db.books.Add(books);
                db.SaveChanges();
                return RedirectToAction("BooksIndex");
            }

            ViewBag.authorId = new SelectList(db.authors, "authorId", "name", books.authorId);
            ViewBag.typeId = new SelectList(db.types, "typeId", "name", books.typeId);
            return View(books);
        }

        // GET: books/Edit/5
        public ActionResult BooksEdit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            books books = db.books.Find(id);
            if (books == null)
            {
                return HttpNotFound();
            }
            ViewBag.authorId = new SelectList(db.authors, "authorId", "name", books.authorId);
            ViewBag.typeId = new SelectList(db.types, "typeId", "name", books.typeId);
            return View(books);
        }

        // POST: books/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult BooksEdit([Bind(Include = "bookId,name,pagecount,point,authorId,typeId")] books books)
        {
            if (ModelState.IsValid)
            {
                db.Entry(books).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("BooksIndex");
            }
            ViewBag.authorId = new SelectList(db.authors, "authorId", "name", books.authorId);
            ViewBag.typeId = new SelectList(db.types, "typeId", "name", books.typeId);
            return View(books);
        }

        // GET: books/Delete/5
        public ActionResult BooksDelete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            books books = db.books.Find(id);
            if (books == null)
            {
                return HttpNotFound();
            }
            return View(books);
        }

        // POST: books/Delete/5
        [HttpPost, ActionName("BooksDelete")]
        [ValidateAntiForgeryToken]
        public ActionResult BooksDeleteConfirmed(int id)
        {
            books books = db.books.Find(id);
            db.books.Remove(books);
            db.SaveChanges();
            return RedirectToAction("BooksIndex");
        }



        ///  Students
        public ActionResult StudentsIndex()
        {
            return View(db.students.ToList());
        }

        // GET: students/Details/5
        public ActionResult StudentsDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            students students = db.students.Find(id);
            if (students == null)
            {
                return HttpNotFound();
            }
            return View(students);
        }

        // GET: students/Create
        public ActionResult StudentsCreate()
        {
            return View();
        }

        // POST: students/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult StudentsCreate([Bind(Include = "studentId,name,surname,birthdate,gender,class,point")] students students)
        {
            if (ModelState.IsValid)
            {
                db.students.Add(students);
                db.SaveChanges();
                return RedirectToAction("StudentsIndex");
            }

            return View(students);
        }

        // GET: students/Edit/5
        public ActionResult StudentsEdit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            students students = db.students.Find(id);
            if (students == null)
            {
                return HttpNotFound();
            }
            return View(students);
        }

        // POST: students/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult StudentsEdit([Bind(Include = "studentId,name,surname,birthdate,gender,class,point")] students students)
        {
            if (ModelState.IsValid)
            {
                db.Entry(students).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("StudentsIndex");
            }
            return View(students);
        }

        // GET: students/Delete/5
        public ActionResult StudentsDelete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            students students = db.students.Find(id);
            if (students == null)
            {
                return HttpNotFound();
            }
            return View(students);
        }

        // POST: students/Delete/5
        [HttpPost, ActionName("StudentsDelete")]
        [ValidateAntiForgeryToken]
        public ActionResult StudentsDeleteConfirmed(int id)
        {
            students students = db.students.Find(id);
            db.students.Remove(students);
            db.SaveChanges();
            return RedirectToAction("StudentsIndex");
        }




        ////////   Borrows
        public ActionResult BorrowsIndex()
        {
            var borrows = db.borrows.Include(b => b.books).Include(b => b.students);
            return View(borrows.ToList());
        }

        // GET: borrows/Details/5
        public ActionResult BorrowsDetails(int? id)
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
        public ActionResult BorrowsCreate()
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
        public ActionResult BorrowsCreate([Bind(Include = "borrowId,studentId,bookId,takenDate,broughtDate")] borrows borrows)
        {
            if (ModelState.IsValid)
            {
                db.borrows.Add(borrows);
                db.SaveChanges();
                return RedirectToAction("BorrowsIndex");
            }

            ViewBag.bookId = new SelectList(db.books, "bookId", "name", borrows.bookId);
            ViewBag.studentId = new SelectList(db.students, "studentId", "name", borrows.studentId);
            return View(borrows);
        }

        // GET: borrows/Edit/5
        public ActionResult BorrowsEdit(int? id)
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
        public ActionResult BorrowsEdit([Bind(Include = "borrowId,studentId,bookId,takenDate,broughtDate")] borrows borrows)
        {
            if (ModelState.IsValid)
            {
                db.Entry(borrows).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("BorrowsIndex");
            }
            ViewBag.bookId = new SelectList(db.books, "bookId", "name", borrows.bookId);
            ViewBag.studentId = new SelectList(db.students, "studentId", "name", borrows.studentId);
            return View(borrows);
        }

        // GET: borrows/Delete/5
        public ActionResult BorrowsDelete(int? id)
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
        [HttpPost, ActionName("BorrowsDelete")]
        [ValidateAntiForgeryToken]
        public ActionResult BorrowsDeleteConfirmed(int id)
        {
            borrows borrows = db.borrows.Find(id);
            db.borrows.Remove(borrows);
            db.SaveChanges();
            return RedirectToAction("BorrowsIndex");
        }







        /////////////////



        /// 



        public ActionResult AuthorsIndex()
        {
            return View(db.authors.ToList());
        }

        // GET: authors/Details/5
        public ActionResult AuthorsDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            authors authors = db.authors.Find(id);
            if (authors == null)
            {
                return HttpNotFound();
            }
            return View(authors);
        }

        // GET: authors/Create
        public ActionResult AuthorsCreate()
        {
            return View();
        }

        // POST: authors/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AuthorsCreate([Bind(Include = "authorId,name,surname")] authors authors)
        {
            if (ModelState.IsValid)
            {
                db.authors.Add(authors);
                db.SaveChanges();
                return RedirectToAction("AuthorsIndex");
            }

            return View(authors);
        }

        // GET: authors/Edit/5
        public ActionResult AuthorsEdit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            authors authors = db.authors.Find(id);
            if (authors == null)
            {
                return HttpNotFound();
            }
            return View(authors);
        }

        // POST: authors/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AuthorsEdit([Bind(Include = "authorId,name,surname")] authors authors)
        {
            if (ModelState.IsValid)
            {
                db.Entry(authors).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("AuthorsIndex");
            }
            return View(authors);
        }

        // GET: authors/Delete/5
        public ActionResult AuthorsDelete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            authors authors = db.authors.Find(id);
            if (authors == null)
            {
                return HttpNotFound();
            }
            return View(authors);
        }

        // POST: authors/Delete/5
        [HttpPost, ActionName("AuthorsDelete")]
        [ValidateAntiForgeryToken]
        public ActionResult AuthorsDeleteConfirmed(int id)
        {
            authors authors = db.authors.Find(id);
            db.authors.Remove(authors);
            db.SaveChanges();
            return RedirectToAction("AuthorsIndex");
        }





        /////////

        public ActionResult TypesIndex()
        {
            return View(db.types.ToList());
        }

        // GET: types/Details/5
        public ActionResult TypesDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            types types = db.types.Find(id);
            if (types == null)
            {
                return HttpNotFound();
            }
            return View(types);
        }

        // GET: types/Create
        public ActionResult TypesCreate()
        {
            return View();
        }

        // POST: types/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TypesCreate([Bind(Include = "typeId,name")] types types)
        {
            if (ModelState.IsValid)
            {
                db.types.Add(types);
                db.SaveChanges();
                return RedirectToAction("TypesIndex");
            }

            return View(types);
        }

        // GET: types/Edit/5
        public ActionResult TypesEdit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            types types = db.types.Find(id);
            if (types == null)
            {
                return HttpNotFound();
            }
            return View(types);
        }

        // POST: types/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TypesEdit([Bind(Include = "typeId,name")] types types)
        {
            if (ModelState.IsValid)
            {
                db.Entry(types).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("TypesIndex");
            }
            return View(types);
        }

        // GET: types/Delete/5
        public ActionResult TypesDelete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            types types = db.types.Find(id);
            if (types == null)
            {
                return HttpNotFound();
            }
            return View(types);
        }

        // POST: types/Delete/5
        [HttpPost, ActionName("TypesDelete")]
        [ValidateAntiForgeryToken]
        public ActionResult TypesDeleteConfirmed(int id)
        {
            types types = db.types.Find(id);
            db.types.Remove(types);
            db.SaveChanges();
            return RedirectToAction("TypesIndex");
        }
    }
}