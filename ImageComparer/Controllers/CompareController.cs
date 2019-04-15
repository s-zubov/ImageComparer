using System;
using System.Collections.Generic;
using System.Drawing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ImageComparer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DifferencesController : ControllerBase
    {
        private readonly IImageComparerManager _imageComparerManager;

        public DifferencesController(IImageComparerManager imageComparerManager)
        {
            _imageComparerManager = imageComparerManager;
        }

        [HttpPost("s")]
        public IActionResult Process(List<IFormFile> files)
        {
            if (files.Count != 2)
                return BadRequest();
            
            var left = Image.FromStream(files[0].OpenReadStream()) as Bitmap;
            var right = Image.FromStream(files[1].OpenReadStream()) as Bitmap;
            
            return File(ImageHelper.ImageToByteArray(_imageComparerManager.GetImage(_imageComparerManager.Process(left,
                right, 20))), "image/jpeg");
        }

        [HttpPost("task")]
        public ActionResult<string> ProcessInBackground(List<IFormFile> files)
        {
            if (files.Count != 2)
                return BadRequest();
            
            var left = Image.FromStream(files[0].OpenReadStream()) as Bitmap;
            var right = Image.FromStream(files[1].OpenReadStream()) as Bitmap;
            
            return _imageComparerManager.ProcessInBackground(left, right, 20).ToString();
        }

        [HttpGet("task/{guid}/status")]
        public ActionResult<string> Status(string guid)
        {
            return _imageComparerManager.GetState(Guid.Parse(guid)).ToString("G");
        }

        [HttpGet("task/{guid}")]
        public IActionResult Result(string guid)
        {
            return File(ImageHelper.ImageToByteArray(_imageComparerManager.GetImage(Guid.Parse(guid))), "image/jpeg");
        }
    }
}