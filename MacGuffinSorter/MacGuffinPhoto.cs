using System;
using System.IO;
using System.Collections.Generic;

namespace MacGuffinSorter
{
    /// <summary>
    /// This class describes a photo file object.
    /// </summary>
    public class MacGuffinPhoto
    {
        public string filePath { get; private set; }
        public string fileName { get; private set; }
        public string productNumber { get; private set; }
        public string metadata { get; private set; }
        public List<string> metadataList { get; private set; } = new List<string>();

        public MacGuffinPhoto() {
            filePath =  productNumber = string.Empty;
        }

        public MacGuffinPhoto(string file) {
            // Full path
            filePath = file;
            // File name without extension
            fileName = Path.GetFileNameWithoutExtension(file);
            // Just the product number
            productNumber = fileName.Split('-')[0];
            // The rest of the data including a leading "-"
            metadata = fileName.Substring(productNumber.Length);
            // Putting the data into an array
            var metadataArray = metadata.Split('-');
            // For each item in the array
            for (int i = 0; i < metadataArray.Length; i++){
                // If it is blank, don't do anything.
                if (String.IsNullOrWhiteSpace(metadataArray[i]))
                    continue;
                // If starts with X then the size will need to be from two parts.
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
