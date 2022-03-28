using Homework51_ImageSharePassword.Data;
using Homework51_ImageSharePassword.Web.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Homework51_ImageSharePassword.Web.Controllers
{
    public class HomeController : Controller
    {
        private string _connectionString = @"Data Source=.\sqlexpress;Initial Catalog=ImageSharePassword; Integrated Security=true;";

        private readonly IWebHostEnvironment _webHostEnvironment;

        public HomeController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Upload(IFormFile image, string password)
        {
            string fileName = $"{Guid.NewGuid()}-{image.FileName}";
            string filePath = Path.Combine(_webHostEnvironment.WebRootPath, "Uploads", fileName);
            using var fs = new FileStream(filePath, FileMode.CreateNew);
            image.CopyTo(fs);

            var repo = new ImagesRepository(_connectionString);
            int id = repo.Upload(fileName, password);
            var vm = new UploadsViewModel
            {
                ImageId = id,
                Password = password
            };
            return View(vm);
        }
        public IActionResult ViewImage(int id)
        {
            var repo = new ImagesRepository(_connectionString);
            var image = repo.GetImage(id);
            var idList = HttpContext.Session.Get<List<int>>("IdList");

            if (idList == null)
            {
                idList = new List<int>();
            }

            if (idList.Contains(id))
            {
                image.HasAccess = true;
                repo.IncrementView(id);            
            }       

            HttpContext.Session.Set("IdList", idList);

            var vm = new ViewImageViewModel
            {
                Image = image,
                Message = (string)TempData["Message"]
            };

            return View(vm);
        }
        [HttpPost]
        public IActionResult ViewImage(int id, string password)
        {
            var repo = new ImagesRepository(_connectionString);
            var image = repo.GetImage(id);           

            var idList = HttpContext.Session.Get<List<int>>("IdList");

            if (!String.IsNullOrEmpty(password) && password == image.Password)
            {
                if (idList == null)
                {
                    idList = new List<int>();
                }
                idList.Add(id);
                image.HasAccess = true;
                HttpContext.Session.Set("IdList", idList);
            }
            else
            {
                TempData["Message"] = "Incorrect Password";               
            }
       
            return Redirect($"/home/viewimage?id={id}");
        }       
    }
}
