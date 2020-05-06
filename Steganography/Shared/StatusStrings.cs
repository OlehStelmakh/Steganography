using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Presentation;
using DocumentFormat.OpenXml.Wordprocessing;

namespace Steganography.Shared
{
    class StatusStrings
    {
        public static string ImageAdding = "Image in processing.";
        public static string ImageAdded = "Image has been successfully added!";
        public static string OpeFileDialogImage = "Choose your image for encryption.";
        public static string ConvertingImage = "The image is being converted.";

        public static string WorkDone = "Done!";

        public static string KeyFileAdded = "Key file has been added";
        public static string ReadingKeyFile = "Reading of key file in progress";

        public static string Encryption = "Process of encryption has been started.";
        public static string CreatingHashes = "Generating hashes info for each symbol.";
        public static string SavingData = "Saving data into file.";

        public static string OperationStopped = "Operation has been stopped";

        public static string ReadingTextFromFile = "Reading text from file";
        public static string TextRead = "Text has been read and added";

        public static string DataSaved = "Data has been saved";
        public static string StartedParsing = "Parsing of data in progress";
        public static string DataDecoded = "Data has been successfully encoded.";
        public static string StartingDecoding = "Decoding in progress";

        public static string ErrorDecrypting = "An error occurred during the decrypting!";
        public static string MissingFile = "Required files missed";
    }
}
