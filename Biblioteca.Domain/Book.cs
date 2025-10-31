using System;

namespace Biblioteca.Domain
{
    public class Book
    {
        public int BookId { get; set; }

        public string Title { get; set; }

        public DateTime PublishDate { get; set; }

        public decimal Price { get; set; }

        public int AuthorId { get; set; }

        public Author Author { get; set; }
    }
}