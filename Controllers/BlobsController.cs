using Microsoft.AspNetCore.Mvc;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace AzureStorageAccountDemo.Controllers
{
    public class BlobsController : Controller
    {
        private readonly IConfiguration _configuration;

        public BlobsController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            List<string> filelist = new List<string>();

            // string blobstorageconnectionstring = _configuration.GetConnectionString("p2pdemoblobconn");

            string blobstorageconnectionstring = _configuration.GetValue<string>("p2pdemoblobconn");

            BlobContainerClient blobContainerClient = new BlobContainerClient(blobstorageconnectionstring, "photos");

            foreach (BlobItem item in blobContainerClient.GetBlobs())
            {
                filelist.Add(item.Name);
            }

            return View(filelist);
        }


        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(List<IFormFile> files)
        {
            long size = files.Sum(f => f.Length);

            foreach (var formFile in files)
            {
                if (formFile.Length > 0)
                {
                    string blobName = formFile.FileName;

                    string blobstorageconnectionstring = _configuration.GetConnectionString("blobconn");
                    
                    BlobContainerClient blobContainerClient = new BlobContainerClient(blobstorageconnectionstring, "photos");
                    
                    BlobClient blobClient = blobContainerClient.GetBlobClient(blobName);
                    
                    var filePath = Path.GetTempFileName();

                    using (var stream = System.IO.File.Create(filePath))
                    {
                        await formFile.CopyToAsync(stream);
                    }

                    await blobClient.UploadAsync(filePath);
                }
            }

            return RedirectToAction("Index");
        }

        public async Task<ActionResult> DownloadFile(string fileName)
        {
            string downloadPath = @"D:\AzureAssignment\DownloadedBlob\";

            string blobstorageconnectionstring = _configuration.GetConnectionString("blobconn");

            BlobContainerClient blobContainerClient = new BlobContainerClient(blobstorageconnectionstring, "photos");

            BlobClient blobClient = blobContainerClient.GetBlobClient(fileName);

            await blobClient.DownloadToAsync(downloadPath + fileName);

            return RedirectToAction("Index");
        }
       
        public async Task<IActionResult> Delete(string fileName)
        {
            string blobstorageconnectionstring = _configuration.GetConnectionString("blobconn");

            BlobContainerClient blobContainerClient = new BlobContainerClient(blobstorageconnectionstring, "photos");

            BlobClient blobClient = blobContainerClient.GetBlobClient(fileName);

            await blobClient.DeleteIfExistsAsync(DeleteSnapshotsOption.None);

            return RedirectToAction("Index");
        }
    }
}
