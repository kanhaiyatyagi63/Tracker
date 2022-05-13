namespace Tracker.Core.Utilities
{
    public class SelectListItem<TKey>
    {
        public TKey Value { get; set; }
        public string Text { get; set; }
        public bool IsSelected { get; set; }
    }
}
