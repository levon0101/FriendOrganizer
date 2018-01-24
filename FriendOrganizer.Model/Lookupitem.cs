using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriendOrganizer.Model
{
    public class Lookupitem
    {
        public int Id { get; set; }

        public string DisplayMember { get; set; }

    }
    public class NullLookupItem:Lookupitem
    {
        public new int? Id { get { return null; } set { } }
    }
}
