using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace proyecto.Models
{
    public class FileListingViewModel
    {
        public List<FileInformation> Files { get; set; }
        public string CKEditorFuncNum { get; set; }
    }


    public class FileInformation
    {
        public string FileName { get; set; }
    }
}