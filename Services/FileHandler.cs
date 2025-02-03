using EStore.Models;

namespace EStore.Services;

public class FileHandler
{
    async Task<string> GenerateFilePathAsync(string fileName, string directory)
    {
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
        var existingFilePath = Path.Combine(directory, fileName);

        if (File.Exists(existingFilePath))
        {
            var newFileName = Path.Combine(existingFilePath + RandomGuid(), directory);
            await GenerateFilePathAsync(newFileName, directory);
        }
        return existingFilePath;
    }
    public  async Task<Response> DeleteFileAsync(string directory, string subDirectory, string fileName)
    {


        return new Response { };
    }
    public async Task<Response> SaveFileAsync(IFormFile file, string directory, string subDirectory)
    {
        Response response = new();
        try
        {
            // Ensure the main directory exists
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // Ensure the subdirectory exists (e.g., "products" or "categories")
            var subDirectoryPath = Path.Combine(directory, subDirectory);
            if (!Directory.Exists(subDirectoryPath))
            {
                Directory.CreateDirectory(subDirectoryPath);
            }

            // Generate unique file name and path
            var filePath = await GenerateFilePathAsync(file.FileName, subDirectoryPath);

            // Save the file to the subdirectory
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            // Generate relative URL dynamically (e.g., "/images/products/image.png")
            var fileUrl = $"{subDirectory}/{Path.GetFileName(filePath)}";

            response.Success = true;
            response.Data = fileUrl; // Store the relative URL
            return response;
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Error = ex.Message;
        }
        return response;
    }



    string RandomGuid()
    {
        string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var random = new Random();
        string randomString = "-" + new string(Enumerable.Repeat(chars, 4)
            .Select(s => s[random.Next(s.Length)]).ToArray());
        return randomString;
    }
}
