namespace Tracker.Core.Data
{
    /// <summary>
    /// Defines basic structure of entity.
    /// </summary>
    /// <typeparam name="TKey"><code>System.Type</code> for database key</typeparam>
    public interface IEntity<TKey>
    {
        TKey Id { get; set; }
        bool IsDeleted { get; set; }
        DateTime CreatedDate { get; set; }
        string CreatedBy { get; set; }
        DateTime? UpdatedDate { get; set; }
        string UpdatedBy { get; set; }
    }
}
