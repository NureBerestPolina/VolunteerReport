using VolunteerReport.Infrastructure.Services;

namespace VolunteerReport.Infrastructure.Tests
{
    [TestClass]
    public class PhotoPlagiarismServiceTest
    {
        [TestMethod]
        [DataRow("init.jpg", "contrast.jpg", true)]
        [DataRow("init.jpg", "rotated.jpg", true)]
        [DataRow("init.jpg", "test_folder.jpg", false)]
        [DataRow("init.jpg", "kitty1.jpg", false)]
        [DataRow("init.jpg", "kitty2.jpg", false)]
        [DataRow("init.jpg", "kitty3.jpg", false)]
        [DataRow("init.jpg", "kitty4.jpg", false)]
        public async Task TestMethod(string firstFileName, string secondFileName, bool arePlagiated)
        {
            var service = new PhotoPlagiarizmService();
            var imgDirectory = Path.Combine(Directory.GetCurrentDirectory(), "TestImages");
            var firstFile = Path.Combine(imgDirectory, firstFileName);
            var secondFile = Path.Combine(imgDirectory, secondFileName);
            var expectedResult = arePlagiated;

            var result = await service.IsPhotoPlagiarizedAsync(secondFile, firstFile);

            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        [DataRow("test_folder.jpg", "report_photos", true)]
        public async Task FolderTest(string fileName, string folderName, bool arePlagiated)
        {
            var service = new PhotoPlagiarizmService();
            var imgDirectory = Path.Combine(Directory.GetCurrentDirectory(), "TestImages");
            var firstFile = Path.Combine(imgDirectory, fileName);
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), folderName);
            var expectedResult = arePlagiated;

            var result = await service.IsPhotoPlagiarizedInDirectoryAsync(firstFile, folderPath);

            Assert.AreEqual(expectedResult, result);
        }
    }
}