using Prism.Events;

namespace FriendOrganizer.UI.Event
{
    public class AfterDetailDeletedEvent : PubSubEvent<AfterDetailDeletedEventArgs>
    {
        public AfterDetailDeletedEvent()
        {
        }
    }

    public class AfterDetailDeletedEventArgs
    {
        public int Id { get; set; }
        public string ViewModelName { get; set; }
    }
}
