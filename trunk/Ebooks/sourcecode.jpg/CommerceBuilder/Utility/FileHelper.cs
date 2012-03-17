using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
//using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using CommerceBuilder.Common;
using CommerceBuilder.Utility.ImageQuantization;
using System.Web;

namespace CommerceBuilder.Utility
{
    /// <summary>
    /// Utility class for file handling
    /// </summary>
    public static class FileHelper
    {
        private const string SafeDirectoryPattern = "[^a-zA-Z0-9\\-_]";
        private const string SafeFilePattern = "[^a-zA-Z0-9\\-_]";
        private const string SafeFilePattern2 = "^[a-zA-Z0-9\\-_]+$";

        /// <summary>
        /// Returns the physical base path for product images.
        /// </summary>
        public static string BaseImagePath
        {
            get
            {
                return AppDomain.CurrentDomain.BaseDirectory + "Assets\\ProductImages\\";
            }
        }

        /// <summary>
        /// Returns the physical base path for digital goods.
        /// </summary>
        public static string BaseDigitalGoodsPath
        {
            get
            {
                return AppDomain.CurrentDomain.BaseDirectory + "App_Data\\DigitalGoods\\";
            }
        }

        /// <summary>
        /// Returns the base url for product images.
        /// </summary>
        public static string BaseImageUrlPath
        {
            get
            {
                return "~/Assets/ProductImages/";
            }
        }

        /// <summary>
        /// Returns the base path on the file sytem for the current application
        /// </summary>
        public static string ApplicationBasePath
        {
            get
            {
                HttpContext context = HttpContext.Current;
                if (context != null)
                {
                    return context.Server.MapPath("~/");
                }
                return AppDomain.CurrentDomain.BaseDirectory;
            }
        }

        /// <summary>
        /// Indicates whether a filename has a valid extension
        /// </summary>
        /// <param name="filename">The file name</param>
        /// <param name="filter">The extension filter (e.g. gif, jpg)</param>
        /// <returns>True if the extension appears in the filter or no filter is set; false otherwise</returns>
        public static bool IsExtensionValid(string filename, string filter)
        {
            if (string.IsNullOrEmpty(filter)) return true;
            if (string.IsNullOrEmpty(filename)) return false;
            string cleanFilter = filter.Replace(" ", "").ToLowerInvariant();
            if (string.IsNullOrEmpty(cleanFilter)) return true;
            string cleanExtension = Path.GetExtension(filename.Replace(" ", "")).ToLowerInvariant();
            if (cleanExtension.StartsWith(".")) cleanExtension = cleanExtension.Substring(1);
            string[] validExtensions = cleanFilter.Split(",".ToCharArray());
            foreach (string ve in validExtensions)
            {
                if (ve == cleanExtension) return true;
            }
            return false;
        }

        /// <summary>
        /// Tests a file to see whether it is an image.
        /// </summary>
        /// <param name="path">The full physical path to the file to test.</param>
        /// <returns>True if the file is recognized as an image, false otherwise.</returns>
        public static bool IsImageFile(string path)
        {
            bool isImage = false;
            System.Drawing.Image testImage = null;
            try
            {
                if (File.Exists(path))
                {
                    testImage = System.Drawing.Image.FromFile(path);
                    isImage = true;
                }
            }
            catch
            {
                //ignore failure
            }
            finally
            {
                if (testImage != null)
                {
                    testImage.Dispose();
                    testImage = null;
                }
            }
            return isImage;
        }

        /// <summary>
        /// Checks a file extension to see whether it is recognized as a text file.
        /// </summary>
        /// <param name="path">The full physical path to the file to test.</param>
        /// <returns>True if the file is recognized as a text file, false otherwise.</returns>
        public static bool IsTextFile(string path)
        {
            string extension = Path.GetExtension(path).ToLowerInvariant();
            return (extension == ".txt" || extension == ".css" || extension == ".skin" || extension == ".js");
        }

        /// <summary>
        /// Gets a string that is suitable for use as a base image name
        /// </summary>
        /// <param name="rawFilename">The string to use as the proposed file name (such as product sku)</param>
        /// <param name="includeExtension">If <b>true</b> file extension is included</param>
        /// <returns>A string that is safe to use in forming a file name, or string.Empty if a file safe name is not possible.</returns>
        public static string GetSafeBaseImageName(string rawFilename, bool includeExtension)
        {
            string extension;
            if (includeExtension)
            {
                extension = Path.GetExtension(rawFilename);
                if (!string.IsNullOrEmpty(extension))
                    rawFilename = rawFilename.Substring(0, rawFilename.Length - extension.Length);
                else extension = ".jpg";
            }
            else extension = string.Empty;
            rawFilename = Regex.Replace(rawFilename, SafeFilePattern, string.Empty);
            if (Regex.Match(rawFilename, SafeFilePattern2).Success) return rawFilename + extension;
            return string.Empty;
        }

        /// <summary>
        /// Determines the best resize dimensions for an image.
        /// </summary>
        /// <param name="originalWidth">Original width of image</param>
        /// <param name="originalHeight">Original height of image</param>
        /// <param name="desiredWidth">Desired width of image</param>
        /// <param name="desiredHeight">Desired height of image</param>
        /// <param name="resizeWidth">The resulting width of the image for use in resizing</param>
        /// <param name="resizeHeight">The resulting height of the image for use in resizing</param>
        /// <remarks>This method always retains aspect ratio</remarks>
        public static void GetResizeInfo(int originalWidth, int originalHeight, int desiredWidth, int desiredHeight, out int resizeWidth, out int resizeHeight)
        {
            FileHelper.GetResizeInfo(originalWidth, originalHeight, desiredWidth, desiredHeight, true, out resizeWidth, out resizeHeight);
        }

        /// <summary>
        /// Determines the best resize dimensions for an image.
        /// </summary>
        /// <param name="originalWidth">Original width of image</param>
        /// <param name="originalHeight">Original height of image</param>
        /// <param name="desiredWidth">Desired width of image</param>
        /// <param name="desiredHeight">Desired height of image</param>
        /// <param name="maintainAspectRatio">Indicates whether resizing should maintain the original aspect ratio of the image</param>
        /// <param name="resizeWidth">The resulting width of the image for use in resizing</param>
        /// <param name="resizeHeight">The resulting height of the image for use in resizing</param>
        public static void GetResizeInfo(int originalWidth, int originalHeight, int desiredWidth, int desiredHeight, bool maintainAspectRatio, out int resizeWidth, out int resizeHeight)
        {
            resizeWidth = desiredWidth;
            resizeHeight = desiredHeight;
            if (maintainAspectRatio)
            {
                Decimal sW = (Decimal)((Decimal)resizeWidth / (Decimal)originalWidth);
                Decimal sH = (Decimal)((Decimal)resizeHeight / (Decimal)originalHeight);
                Decimal isoScalar = Math.Min(sW, sH);
                resizeWidth = (int)Math.Round((isoScalar * originalWidth), 0);
                resizeHeight = (int)Math.Round((isoScalar * originalHeight), 0);
            }
        }

        /// <summary>
        /// Writes an image to the file name specified
        /// </summary>
        /// <param name="originalImage">Image to write</param>
        /// <param name="fileName">File to write to</param>
        public static void WriteImageFile(Image originalImage, string fileName)
        {
            WriteImageFile(originalImage, fileName, originalImage.Width, originalImage.Height, true, 100);
        }

        /// <summary>
        /// Writes an image to the file name specified
        /// </summary>
        /// <param name="originalImage">Image to write</param>
        /// <param name="fileName">File to write to</param>
        /// <param name="quality">Quality of the image in percent</param>
        public static void WriteImageFile(Image originalImage, string fileName, int quality)
        {
            WriteImageFile(originalImage, fileName, originalImage.Width, originalImage.Height, true, quality);
        }

        /// <summary>
        /// Writes an image to the file name specified
        /// </summary>
        /// <param name="originalImage">Image to write</param>
        /// <param name="fileName">File to write to</param>
        /// <param name="desiredWidth">Desired width of the image</param>
        /// <param name="desiredHeight">Desired height of the image</param>
        /// <param name="maintainAspectRatio">If <b>true</b> aspect ratio is maintained while resizing</param>
        /// <param name="quality">Quality of the image in percent</param>
        public static void WriteImageFile(Image originalImage, string fileName, int desiredWidth, int desiredHeight, bool maintainAspectRatio, int quality)
        {
            //GET RESIZE DIMENSIONS
            int resizeWidth, resizeHeight;
            FileHelper.GetResizeInfo(originalImage.Width, originalImage.Height, desiredWidth, desiredHeight, maintainAspectRatio, out resizeWidth, out resizeHeight);
            //GENERATE IMAGE OF APPROPRIATE SIZE
            using (System.Drawing.Image resizedImage = GetResizedImage(originalImage, resizeWidth, resizeHeight))
            {
                if (resizedImage != null)
                {
                    string extension = Path.GetExtension(fileName).ToLowerInvariant();
                    switch (extension)
                    {
                        case ".png":
                            resizedImage.Save(fileName, ImageFormat.Png);
                            break;
                        case ".gif":
                            if (Misc.HasUnmanagedCodePermission())
                            {
                                //QUANTIZE GIF IMAGES
                                OctreeQuantizer quantizer = new OctreeQuantizer(255, 8);
                                using (Image quantized = quantizer.Quantize(resizedImage))
                                {
                                    quantized.Save(fileName, ImageFormat.Gif);
                                    quantized.Dispose();
                                }
                            }
                            else resizedImage.Save(fileName, ImageFormat.Gif);
                            break;
                        case ".bmp":
                            resizedImage.Save(fileName, ImageFormat.Bmp);
                            break;
                        default:
                            SaveJPGWithCompressionSetting(resizedImage, fileName, (long)quality);
                            break;
                    }
                }
            }
        }

        private static ImageCodecInfo FindEncoder(ImageFormat fmt)
        {
            ImageCodecInfo[] infoArray1 = ImageCodecInfo.GetImageEncoders();
            ImageCodecInfo[] infoArray2 = infoArray1;
            for (int num1 = 0; num1 < infoArray2.Length; num1++)
            {
                ImageCodecInfo info1 = infoArray2[num1];
                if (info1.FormatID.Equals(fmt.Guid))
                {
                    return info1;
                }
            }
            return null;
        }

        private static void SaveJPGWithCompressionSetting(System.Drawing.Image img, string filename, long compression)
        {
            //Create a parameter collection
            EncoderParameters codecParameters = new EncoderParameters(1);

            //Fill the only parameter
            codecParameters.Param[0] = new EncoderParameter(Encoder.Quality, compression);

            //Get the codec info
            ImageCodecInfo codecInfo = FindEncoder(ImageFormat.Jpeg);

            //Save the image
            img.Save(filename, codecInfo, codecParameters);
        }

        /// <summary>
        /// Resizes a given image to the specified width and height
        /// </summary>
        /// <param name="imgPhoto">The input image</param>
        /// <param name="Width">The desired width</param>
        /// <param name="Height">The desired height</param>
        /// <returns>The resized image</returns>
        public static Image GetResizedImage(Image imgPhoto, int Width, int Height)
        {
            int sourceWidth = imgPhoto.Width;
            int sourceHeight = imgPhoto.Height;
            int sourceX = 0;
            int sourceY = 0;
            int destX = 0;
            int destY = 0;

            float nPercent = 0;
            float nPercentW = 0;
            float nPercentH = 0;

            nPercentW = ((float)Width / (float)sourceWidth);
            nPercentH = ((float)Height / (float)sourceHeight);
            if (nPercentH < nPercentW)
            {
                nPercent = nPercentH;
                destX = System.Convert.ToInt16((Width -
                              (sourceWidth * nPercent)) / 2);
            }
            else
            {
                nPercent = nPercentW;
                destY = System.Convert.ToInt16((Height -
                              (sourceHeight * nPercent)) / 2);
            }

            int destWidth = (int)(sourceWidth * nPercent);
            int destHeight = (int)(sourceHeight * nPercent);

            Bitmap bmPhoto = new Bitmap(Width, Height,
                              PixelFormat.Format24bppRgb);
            bmPhoto.SetResolution(imgPhoto.HorizontalResolution,
                             imgPhoto.VerticalResolution);

            Graphics grPhoto = Graphics.FromImage(bmPhoto);
            grPhoto.Clear(Color.White);
            grPhoto.InterpolationMode =
                    InterpolationMode.HighQualityBicubic;

            grPhoto.DrawImage(imgPhoto,
                new Rectangle(destX, destY, destWidth, destHeight),
                new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight),
                GraphicsUnit.Pixel);

            grPhoto.Dispose();
            return bmPhoto;
        }

        /// <summary>
        /// Reads all data from the given file in a text string
        /// </summary>
        /// <param name="path">The file to read</param>
        /// <returns>File data</returns>
        public static string ReadText(string path)
        {
            string text;
            using (StreamReader sr = File.OpenText(path))
            {
                text = sr.ReadToEnd();
            }
            return text;
        }

        /// <summary>
        /// Records a textual byte-by-byte representation of the given data to the file specified
        /// </summary>
        /// <param name="filepath">The file to write to</param>
        /// <param name="value">The data to write</param>
        public static void WriteBytes(string filepath, string value)
        {
            FileHelper.WriteBytes(filepath, System.Text.Encoding.GetEncoding("iso-8859-1").GetBytes(value));
        }

        //TODO: WHAT ARE THESE METHODS?  THE NAME MISLEAD ME TO THINKING THEY WERE SOMETHING ELSE
        /// <summary>
        /// Records a textual byte-by-byte representation of the given data to the file specified
        /// </summary>
        /// <param name="filepath">The file to write to</param>
        /// <param name="value">The data to write</param>
        public static void WriteBytes(string filepath, byte[] value)
        {
            using (StreamWriter sw = File.CreateText(filepath))
            {
                foreach (byte ch in value)
                {
                    sw.WriteLine(ch.ToString() + " " + Convert.ToString(ch));
                }
            }
        }

        /// <summary>
        /// Copies the contents of the source directory to the target
        /// </summary>
        /// <param name="source">Directory to copy from</param>
        /// <param name="target">Directory to copy to; it will be created if it does not exist</param>
        public static void CopyDirectory(string source, string target)
        {
            DirectoryInfo sourceDir = new DirectoryInfo(source);
            DirectoryInfo targetDir = new DirectoryInfo(target);
            //verify target exists
            if (!targetDir.Exists) targetDir.Create();
            //copy sub directories
            DirectoryInfo[] sourceSubs = sourceDir.GetDirectories();
            foreach (DirectoryInfo subDir in sourceSubs)
            {
                if (!subDir.Name.StartsWith("."))
                {
                    CopyDirectory(subDir.FullName, Path.Combine(targetDir.FullName, subDir.Name));
                }
            }
            //copy files
            FileInfo[] sourceFiles = sourceDir.GetFiles();
            foreach (FileInfo fi in sourceFiles)
            {
                File.Copy(fi.FullName, Path.Combine(targetDir.FullName, fi.Name), true);
            }
        }

        /// <summary>
        /// Tests for the ability to open an existing file for write access.
        /// </summary>
        /// <param name="fileName">Full physical path of the file to attempt to open.</param>
        /// <returns>Exception that occurred during the test, or null if the test succeeded.</returns>
        /// <remarks>The contents of the file are not changed by this method.</remarks>
        public static Exception CanWriteExistingFile(string fileName)
        {
            // THE FILE MUST EXIST TO TEST WRITE ACCESS
            if (!File.Exists(fileName))
            {
                throw new ArgumentException("File does not exist.");
            }

            // ATTEMPT TO OPEN FILE WITH WRITE ACCESS, RETURN ANY EXCEPTION
            try
            {
                using (FileStream fs = File.OpenWrite(fileName))
                {
                    fs.Close();
                }
            }
            catch (Exception ex)
            {
                return ex;
            }

            // NO EXCEPTIONS, THE METHOD WAS SUCCESSFUL
            return null;
        }

        /// <summary>
        /// Tests for the ability to open an existing file for read access.
        /// </summary>
        /// <param name="fileName">Full physical path of the file to attempt to open.</param>
        /// <returns>Exception that occurred during the test, or null if the test succeeded.</returns>
        public static Exception CanReadExistingFile(string fileName)
        {
            // THE FILE MUST EXIST TO TEST READ ACCESS
            if (!File.Exists(fileName))
            {
                throw new ArgumentException("File does not exist.");
            }

            // ATTEMPT TO OPEN FILE WITH READ ACCESS, RETURN ANY EXCEPTION
            try
            {
                using (FileStream fs = File.OpenRead(fileName))
                {
                    fs.Close();
                }
            }
            catch (Exception ex)
            {
                return ex;
            }

            // NO EXCEPTIONS, THE METHOD WAS SUCCESSFUL
            return null;
        }

        /// <summary>
        /// Tests for the ability to create (write) the specified file
        /// </summary>
        /// <param name="fileName">Full physical path of the file to attempt to create.</param>
        /// <param name="fileContent">Text content to write to the file.</param>
        /// <returns>Exception that occurred during the test, or null if the test succeeded.</returns>
        /// <remarks>The the given file path already exists, this method will attempt to overwrite it.</remarks>
        public static Exception CanCreateFile(string fileName, string fileContent)
        {
            // ATTEMPT TO WRITE FILE, RETURN ANY EXCEPTION
            try
            {
                File.WriteAllText(fileName, fileContent);
            }
            catch (Exception ex)
            {
                return ex;
            }

            // NO EXCEPTIONS, THE METHOD WAS SUCCESSFUL
            return null;
        }

        /// <summary>
        /// Tests for the ability to create (write) the specified file
        /// </summary>
        /// <param name="fileName">Full physical path of the file to attempt to create.</param>
        /// <param name="fileContent">Text content to write to the file.</param>
        /// <param name="createDirectoryStructure">Whether to try to create directory structure if it does not exist?</param>
        /// <returns>Exception that occurred during the test, or null if the test succeeded.</returns>
        /// <remarks>The the given file path already exists, this method will attempt to overwrite it.</remarks>        
        public static Exception CanCreateFile(string fileName, string fileContent, bool createDirectoryStructure)
        {
            string dirName = Path.GetDirectoryName(fileName);
            if (createDirectoryStructure && !Directory.Exists(dirName))
            {
                try
                {
                    Directory.CreateDirectory(dirName);
                }
                catch (Exception ex)
                {
                    return ex;
                }
            }
            return CanCreateFile(fileName, fileContent);
        }


        /// <summary>
        /// Tests for the ability to delete the specified file.  Warning: If this method succeeds
        /// the specified file is <b>DELETED</b>.
        /// </summary>
        /// <param name="fileName">Full physical path of the file to attempt to delete</param>
        /// <returns>Exception that occurred during the test, or null if the test succeeded.</returns>
        public static Exception CanDeleteFile(string fileName)
        {
            if (!File.Exists(fileName))
            {
                throw new ArgumentException("File does not exist.");
            }

            // ATTEMPT THE DELETE, RETURN ANY EXCEPTION 
            try
            {
                File.Delete(fileName);
            }
            catch (Exception ex)
            {
                return ex;
            }

            // NO EXCEPTIONS, THE METHOD WAS SUCCESSFUL
            return null;
        }

        /// <summary>
        /// Gets a human readble number from the file size
        /// </summary>
        /// <param name="fileSize">The number of bytes in the file</param>
        /// <returns>The file size expressed in bytes, kb, or mb.</returns>
        public static string FormatFileSize(long fileSize)
        {
            if (fileSize < 1000) return fileSize.ToString() + "B";
            LSDecimal tempSize = fileSize / 1000;
            if (tempSize < 1000) return string.Format("{0:0.#}KB", tempSize);
            tempSize = tempSize / 1000;
            return string.Format("{0:F1}MB", tempSize);
        }


        /// <summary>
        /// Gets a unique file name based on the given file name.
        /// If a file with same name already exists in the given path 
        /// it will add numbers to the name until a unique name is found
        /// </summary>
        /// <param name="FileNameWithPath">File name with fully qualified path</param>
        /// <returns>Unique file name</returns>
        public static string GetUniqueFileName(string FileNameWithPath)
        {
            int count = 0;
            string Name = "";
            if (System.IO.File.Exists(FileNameWithPath))
            {
                System.IO.FileInfo f = new System.IO.FileInfo(FileNameWithPath);
                if (!string.IsNullOrEmpty(f.Extension))
                {
                    Name = f.FullName.Substring(0, f.FullName.LastIndexOf('.'));
                }
                else
                {
                    Name = f.FullName;
                }

                while (File.Exists(FileNameWithPath))
                {
                    count++;
                    FileNameWithPath = Name + count.ToString() + f.Extension;
                }
            }
            return FileNameWithPath;
        }

    }
}