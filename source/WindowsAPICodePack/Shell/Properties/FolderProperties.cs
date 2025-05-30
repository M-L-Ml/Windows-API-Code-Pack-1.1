using System;

namespace Microsoft.WindowsAPICodePack.Shell
{
    /// <summary>Represents the properties of a known folder.</summary>  
    internal struct FolderProperties
    {
        public FolderCategory category;
        public string canonicalName;
        public string description;
        public Guid parentId;
        public string relativePath;
        public string parsingName;
        public string tooltipResourceId;
        public string localizedNameResourceId;
        public string iconResourceId;
        public string security;
        public System.IO.FileAttributes fileAttributes;
        public DefinitionOptions definitionOptions;
        public Guid folderTypeId;
        public string folderType;
        public string path;
        public bool pathExists;
        public RedirectionCapability redirection;
        public string localizedName;
        public string tooltip;
        internal Guid folderId;
    }
}
