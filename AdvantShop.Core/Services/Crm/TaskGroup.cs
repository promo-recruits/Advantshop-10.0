using System;

namespace AdvantShop.Core.Services.Crm
{
    public class TaskGroup
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int SortOrder { get; set; }
        public DateTime DateCreated { get; set; }
        public bool Enabled { get; set; }
        public bool IsPrivateComments { get; set; }

        public override bool Equals(object obj)
        {
            var other = obj as TaskGroup;
            if (other == null)
                return false;

            return Id == other.Id &&
                   Name == other.Name &&
                   SortOrder == other.SortOrder &&
                   Enabled == other.Enabled &&
                   DateCreated == other.DateCreated &&
                   IsPrivateComments == other.IsPrivateComments;
        }

        public override int GetHashCode()
        {
            return
                Id.GetHashCode() ^
                (Name ?? "").GetHashCode() ^
                SortOrder.GetHashCode() ^
                DateCreated.GetHashCode() ^
                Enabled.GetHashCode() ^
                IsPrivateComments.GetHashCode();
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
