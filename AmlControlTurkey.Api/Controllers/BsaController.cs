using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using AmlControlTurkey.Core.Models;

namespace AmlControlTurkey.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BsaController : ControllerBase
    {
        private readonly BsaSearchService _bsaSearchService;

        public BsaController(BsaSearchService bsaSearchService)
        {
            _bsaSearchService = bsaSearchService;
        }

        [HttpGet("search")]
        public IActionResult SearchDocuments([FromQuery] string queryText)
        {
            try
            {
                var results = _bsaSearchService.Search(queryText);
                return Ok(results);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
