using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MacGuffinSorter
{
    class Program
    {
        static void Main(string[] args)
        {

            // Ask user for the location of the folder in which the pictures are stored.
            Console.Write("Enter fully qualified path to folder containing photos: ");
            string folderPath = Console.ReadLine();

            // Making sure the folder acutally exists.
            while (!Directory.Exists(folderPath)) {
                Console.WriteLine("Location given does not exist.");
                Console.Write("Please enter a valid location: ");
                folderPath = Console.ReadLine();
            }

            // Ask user for destination 
            Console.Write("Enter fully qualified path to foler containing category destinations: ");
            string destinationPath = Console.ReadLine();

            // Making sure that directory exists
            while (!Directory.Exists(destinationPath))
            {
                Console.WriteLine("Location given does not exist.");
                Console.Write("Please enter a valid location: ");
                destinationPath = Console.ReadLine();
            }

            // Put all files in an enumerable list.
            IEnumerable<string> files = Directory.EnumerateFiles(folderPath);
            IEnumerable<string> folders = Directory.EnumerateDirectories(destinationPath);

            // List to store object information
            IList<MacGuffinPhoto> photos = new List<MacGuffinPhoto>();
            IList<Category> categories = new List<Category>();

            // Go through each file and push it to the list.
            foreach(string f in files){
                photos.Add(new MacGuffinPhoto(f));
            }

            // Go through each folder and push it to list.
            foreach(string c in folders){
                categories.Add(new Category(c));
            }

            // Creating an enum to cache possible categories for faster lookup will be based on the product number.
            IEnumerable<Category> possibleCategories = Enumerable.Empty<Category>();

            /* For every photo: grab the possible categories based on the product number,
             * then compare the metadata properties, counting how many match. Take the category
             * that has the higest amount of matches and add it as the destination, repeating until
             * all possible categories have been gone through. If no category matches the default 
             * will be used, which is just the folder with no metadata properties.
             */
            foreach(MacGuffinPhoto photo in photos){
                // This will be the destination path
                string destination = String.Empty;
                // This is the default destination -- no metadata
                string defaultDestination = String.Empty;
                // Counter to keep track of the highest number of matches
                int destCount = 0;
                // This is grabbing the categories that are possible based on the product number
                possibleCategories = from c in categories where c.productNumber.ToLower().Trim() == photo.productNumber.ToLower().Trim() select c;
                // If there's only one possible category then just move it. Don't bother with the rest.
                if(possibleCategories.Count() == 1){
                    defaultDestination = possibleCategories.First().folderPath;
                    File.Move(photo.filePath, defaultDestination + Path.DirectorySeparatorChar + photo.fileName + ".jpg");
                    continue;
                }
                // Looping through each possible category
                foreach(Category c in possibleCategories){
                    if(c.metadataList.Count == 0){
                        // Sets the default destination based on how many metadata tags it has.
                        defaultDestination = c.folderPath;
                    }
                    // If the photo doens't have any metadata then it stops here so extra code isn't needed.
                    if (photo.metadataList.Count == 0)
                        continue;
                    // Setting a counter to 0 to compare to the destination counter
                    int count = 0;
                    // Taking every metadata tag and comparing it to the possible metadata tags in the category.
                    foreach(string x in photo.metadataList){
                        foreach(string y in c.metadataList){
                            if (x.ToLower().Trim() == y.ToLower().Trim()){
                                // If there's a match we add one to the counter
                                count++;
                            }
                        }
                    }
                    // If the counter has a higher match than the default or previous categories
                    // and the photo has more or equal to the tags of the category
                    if (count > destCount && photo.metadataList.Count >= c.metadataList.Count){
                        if (c.metadataList.Count > 1)
                        {
                            if (count == c.metadataList.Count)
                            {
                                // This if for when there is more than one tag.
                                // Making sure that all tags match.
                                destCount = count;
                                destination = c.folderPath;
                            }
                            else
                            {
                                continue;
                            }
                        }
                        // This will occur when there is only one tag
                        else
                        {
                            destCount = count;
                            destination = c.folderPath;
                        }
                    }
                }
                // I don't set the destination path if nothing matches, so we just use the default.
                // However if there is a destination path chosen based on the number of matches it gets then we move the file accordingly.
                if(!String.IsNullOrWhiteSpace(destination)){
                    File.Move(photo.filePath, destination + Path.DirectorySeparatorChar + photo.fileName + ".jpg");
                } else {
                    File.Move(photo.filePath, defaultDestination + Path.DirectorySeparatorChar + photo.fileName + ".jpg");
                }
            }
        }
    }
}
