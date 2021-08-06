using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kunicardus.Domain
{
    public class UploadedFile
    {
        public int UploadedFileId { get; set; }
        public byte[] Content { get; set; }
        public string ContentType { get; set; }
        public string Title { get; set; }
        public string FileName { get; set; }
        public string Description { get; set; }
        public DateTime UploadDate { get; set; }

        public bool HasFile()
        {
            return Content != null && Content.Any();
        }
    }
}
