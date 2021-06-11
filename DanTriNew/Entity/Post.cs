using System;
using System.Collections.Generic;
using System.Text;

namespace DanTriNew.Entity
{
    class Post
    {
        public long Id { get; set; }
        public String Title { get; set; }
        public String Content { get; set; }
        public int CategoryID { get; set; }
        public int Page { get; set; }
    }
}
