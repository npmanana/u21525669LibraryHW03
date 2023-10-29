using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LibraryHW03.Models
{
    public class CombinedViewModel
    {
        public IEnumerable<students> Students { get; set; }
        public IEnumerable<books> Books { get; set; }
        public IEnumerable<borrows> Borrows { get; set; }
        public IEnumerable<authors> Authors { get; set; }
        public IEnumerable<types> Types { get; set; }

        // Pagination properties
        public int StudentCurrentPage { get; set; }
        public int StudentTotalPages { get; set; }
        public int BookCurrentPage { get; set; }
        public int BookTotalPages { get; set; }
        public int AuthorCurrentPage { get; set; }
        public int AuthorTotalPages { get; set; }
        public int TypeCurrentPage { get; set; }
        public int TypeTotalPages { get; set; }
        public int BorrowCurrentPage { get; set; }
        public int BorrowTotalPages { get; set; }

        public string BookName { get; set; }
        public int BorrowCount { get; set; }
    }

}
