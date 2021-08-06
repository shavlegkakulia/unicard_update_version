using System;
using SQLite;

namespace Kunicardus.Core.Models.DB
{
    public class VersionsModel : DBModel
    {
        [PrimaryKey]
        public VersionType Type { get; set; }

        public int Version { get; set; }
    }

    public enum VersionType : int
    {
        category
    }
}
