using System;
using System.Collections.Generic;
using System.Text;

namespace Bookstore_Tycoon.Models
{
    class RollingInfo
    {
        public string Filename { get; set; }
        public string GameName { get; set; }

        public int Customers { get; set; }
        public int SatisfactionRoll0 { get; set; }
        public int SatisfactionRoll1 { get; set; }
        public int SatisfactionRoll2 { get; set; }
        public int SatisfactionRoll3 { get; set; }
        public int SatisfactionRoll4 { get; set; }
        public int SatisfactionRoll5 { get; set; }
        public int BooksSold { get; set; }
        public int BookRoll0 { get; set; }
        public int BookRoll1 { get; set; }
        public int BookRoll2 { get; set; }
        public int BookRoll3 { get; set; }
        public int BookRoll4 { get; set; }
        public int BookRoll5 { get; set; }
    }
}
