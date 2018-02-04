﻿using Prism.Events;

namespace FriendOrganizer.UI.Event
{
    public class AfteeDetailSaveEvent:PubSubEvent<AfterDetailSaveEventArgs >
    {
    }

    public class AfterDetailSaveEventArgs
    {
        public int Id { get; set; }
        public string DisplayMember { get; set; }
        public string ViewModelName { get; set; }

    }
}
