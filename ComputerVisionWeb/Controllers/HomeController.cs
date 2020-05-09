using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ComputerVisionWeb.Models;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Web.Http.Cors;
using ComputerVisionWeb.Models.Enums;
using ComputerVisionWeb.Helper;
using ComputerVisionWeb.Services;

namespace ComputerVisionWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _config;
        private readonly IImageProcessor _imageProcessor;


        public HomeController(IConfiguration configuration, ILogger<HomeController> logger, IImageProcessor imageProcessor)
        {
            _config = configuration;
            _logger = logger;
            _imageProcessor = imageProcessor;
        }

        public IActionResult Index()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> ConvertToText(PostClass model)
        {
            var imageProcessModel=await _imageProcessor.ReadImageTextAsync(model.File);

            return new JsonResult(imageProcessModel);
      
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

    public class PostClass
    {

        public IFormFile File { get; set; }
    }
}
