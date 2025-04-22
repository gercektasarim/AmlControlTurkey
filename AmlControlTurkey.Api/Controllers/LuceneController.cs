using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using AmlControlTurkey.Core.Models;

namespace AmlControlTurkey.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LuceneController : ControllerBase
    {
        private readonly LuceneSearchService _luceneSearchService;

        public LuceneController(LuceneSearchService luceneSearchService)
        {
            _luceneSearchService = luceneSearchService;
        }

        [HttpGet("search")]
        public IActionResult SearchDocuments([FromQuery] string queryText)
        {
            try
            {
                var results = _luceneSearchService.Search(queryText);
                return Ok(results);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
