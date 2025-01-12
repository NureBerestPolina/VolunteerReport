using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VolunteerReport.Infrastructure.Services.Interfaces;
using Shipwreck.Phash;
using Shipwreck.Phash.Bitmaps;
using System.Drawing;
using System.Collections.Concurrent;

namespace VolunteerReport.Infrastructure.Services
{
    public class PhotoPlagiarizmService : IPhotoPlagiarismService
    {
        private static readonly ConcurrentDictionary<string, Digest> _digestCache = new ConcurrentDictionary<string, Digest>();

        public async Task<bool> IsPhotoPlagiarizedAsync(string uploadedFilePath, string storedFilePath)
        {
            if (!File.Exists(uploadedFilePath) || !File.Exists(storedFilePath))
            {
                return false; // Either file doesn't exist
            }

            try
            {
                var uploadedHash = CalculateDigestHashFromFile(uploadedFilePath);
                var storedHash = CalculateDigestHashFromFile(storedFilePath);

                return AreSimilar(uploadedHash, storedHash);
            }
            catch
            {
                return false; // Return false if any exception occurs (file issues, etc.)
            }
        }

        public async Task<bool> IsPhotoPlagiarizedInDirectoryAsync(string uploadedFilePath, string storedDirectoryPath)
        {
            if (!File.Exists(uploadedFilePath) || !Directory.Exists(storedDirectoryPath))
            {
                return false; // Either file doesn't exist
            }

            try
            {
                var uploadedHash = CalculateDigestHashFromFile(uploadedFilePath);

                var storedHashes = Directory.EnumerateFiles(storedDirectoryPath)
                    .Select(CalculateDigestHashFromFile);

                return storedHashes.Any(score => AreSimilar(uploadedHash, score));
            }
            catch
            {
                return false; // Return false if any exception occurs (file issues, etc.)
            }
        }

        private static bool AreSimilar(Digest uploadedHash, Digest storedHash)
        {
            var score = ImagePhash.GetCrossCorrelation(uploadedHash, storedHash);

            return score > 0.9;
        }

        private static Digest CalculateDigestHashFromFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new ArgumentException("File does not exist", nameof(filePath));
            }

            //return _digestCache.GetOrAdd(filePath, path =>
            //{
           // using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
           // {
                //using (var uploadedBitmap = (Bitmap)Image.FromStream(fs))
                {

                    using var uploadedBitmap = (Bitmap)System.Drawing.Image.FromFile(filePath);
                    var uploadedImage = uploadedBitmap.ToLuminanceImage();
                    var hash = ImagePhash.ComputeDigest(uploadedImage);
                    return hash;
                }
                //});
        }
    }
}
