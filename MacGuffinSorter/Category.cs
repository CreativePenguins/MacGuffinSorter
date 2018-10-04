using System;
using System.IO;
using System.Collections.Generic;

namespace MacGuffinSorter
{
    public class Category
    {
        public string folderPath { get; private set; }
        public string folderName { get; private set; }
        public string productNumber { get; private set; }
        public string metadata { get; private set; }
        public List<string> metadataList { get; private set; } = new List<string>();

        public Category()
        {
            folderPath = folderName = productNumber = string.Empty;
        }

        public Category(string folder)
        {
            // Full Path
            folderPath = folder;
            // Folder name
            folderName = Path.GetFileName(folder);
            // Just the product Number
            productNumber = folderName.Split('-')[0];
            // The rest of the data including a leading "-" if it has one
            metadata = folderName.Substring(productNumber.Length);
            // Putting extra data into an array
            var metadataArray = metadata.Split('-');
            // For each item in the array
            for (int i = 0; i < metadataArray.Length; i++)
            {
                // If it's blank, don't do anything
                if (String.IsNullOrWhiteSpace(metadataArray[i]))
                    continue;
                // If it starts with x then the size is two parts
                if (metadataArray[i].ToLower().Trim().StartsWith('x'))
                {
                    metadataList.Add(metadataArray[i] + '-' + metadataArray[i + 1]);
                    i++;
                }
                // Otherwise just dump it in.
                else
                    metadataList.Add(metadataArray[i]);
            }
        }
    }
}
