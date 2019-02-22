using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DemoCrm.Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DemoCrm.Api.Controllers
{
    [Route("api")]
    [ApiController]
    public class RootController : ControllerBase
    {
        private IUrlHelper _urlHelper;

        public RootController(IUrlHelper urlHelper)
        {
            _urlHelper = urlHelper;
        }

        [HttpGet(Name = "GetRoot")]
        public IActionResult GetRoot([FromHeader(Name = "Accept")] string mediaType)
        {
            if (mediaType == "application/vnd.onesoft.hateoas+json")
            {
                var links = new List<HypermediaLink>();

                links.Add(new HypermediaLink(
                    _urlHelper.Link("GetRoot", new { }),
                    "self",
                    "GET"));

                links.Add(new HypermediaLink(
                    _urlHelper.Link("GetCrmUsers", new { }),
                    "self",
                    "GET"));

                links.Add(new HypermediaLink(
                    _urlHelper.Link("CreateCrmUsers", new { }),
                    "self",
                    "POST"));

                return Ok(links);
            }

            return NoContent();
        }
    }
}