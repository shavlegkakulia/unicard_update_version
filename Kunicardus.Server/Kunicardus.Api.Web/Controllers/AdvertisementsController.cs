using Kunicardus.Services.Abstract;
using Kunicardus.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Formatting;
using System.Web.Http;

namespace Kunicardus.Api.Web.Controllers
{
    public class AdvertisementsController : ApiController
    {
        public IAdvertisementService _adService { get; set; }

        public AdvertisementsController(IAdvertisementService billboardService)
        {
            _adService = billboardService;
        }

        [Route("~/api/billboards")]
        [HttpGet]
        public virtual IHttpActionResult GetBilborads([FromUri] GetBillboardsRequest request)
        {
            return Ok(_adService.GetBillboards(request));
        }

        [Route("~/api/advertisements")]
        public virtual IHttpActionResult GetAdvertisements([FromUri] GetAdvertisementsRequest request)
        {
            return Ok(_adService.GetAdvertisements(request));
        }

        [Route("~/api/advertisements/{advertisementId}/save-points/{userId}")]
        [HttpPost]
        public virtual IHttpActionResult SavePoints([FromUri] SaveAdvertisementPointsRequest request)
        {
            return Ok(_adService.SavePoints(request));
        }
    }
}
