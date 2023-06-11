namespace Demo_API.Models
{
    public class TargetAsset
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public string Owner { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; }
        public string[] Tags { get; set; }
        public int Cpu { get; set; }
        public long Ram { get; set; }
        public int? ParentId { get; set; }
        public bool? IsStartable { get; set; }
        public int? ParentTargetAssetCount { get; set; }
    }
}
