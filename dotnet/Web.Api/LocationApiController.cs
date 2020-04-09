using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sabio.Models;
using Sabio.Models.Domain;
using Sabio.Models.Requests.Location;
using Sabio.Services;
using Sabio.Web.Controllers;
using Sabio.Web.Models.Responses;

namespace Sabio.Web.Api.Controllers
{
    [Route("api/locations")]
    [ApiController]
    public class LocationApiController : BaseApiController
    {
        private IAuthenticationService<int> _authService = null;
        private ILocationService _service = null;

        public LocationApiController(ILocationService service
           , ILogger<LocationApiController> logger
           , IAuthenticationService<int> authService) : base(logger)
        {
            _service = service;
            _authService = authService;
        }

        [HttpGet("{id:int}")]
        public ActionResult<ItemResponse<Location>> GetById(int id)
        {
            int iCode = 200;
            BaseResponse response = null;
            try
            {
                Location location = _service.Get(id);
                if (location == null)
                {
                    iCode = 404;
                    response = new ErrorResponse("Resource not found.");
                }
                else
                {
                    response = new ItemResponse<Location> { Item = location };
                }
            }
            catch (Exception ex)
            {
                iCode = 500;

                base.Logger.LogError(ex.ToString());
                response = new ErrorResponse($"Argument Exception Error: {ex.Message}");
            }

            return StatusCode(iCode, response);
        }


        [HttpGet("paginate")]
        public ActionResult<ItemResponse<Paged<Location>>> GetPaged(int pageIndex, int pageSize)
        {
            int code = 200;
            BaseResponse result = null;
            try
            {
                Paged<Location> page = _service.Paginate(pageIndex, pageSize);

                if (page == null)
                {
                    code = 404;
                    result = new ErrorResponse("Resource not found.");
                }
                else
                {
                    ItemResponse<Paged<Location>> response = new ItemResponse<Paged<Location>>();
                    result = new ItemResponse<Paged<Location>> { Item = page };
                }
            }
            catch (Exception ex)
            {
                code = 500;
                result = new ErrorResponse(ex.Message);
                base.Logger.LogError(ex.ToString());
            }
            return StatusCode(code, result);

        }

        [HttpGet("search")]
        public ActionResult<ItemsResponse<Paged<Location>>> SearchLocation(int pageIndex, int pageSize, string search)
        {
            int code = 200;
            BaseResponse result = null;
            try
            {
                Paged<Location> page = _service.SearchLocation(pageIndex, pageSize, search);

                if (page == null)
                {
                    code = 404;
                    result = new ErrorResponse("Resource not found.");
                }
                else
                {
                    result = new ItemResponse<Paged<Location>> { Item = page };
                }
            }
            catch (Exception ex)
            {
                code = 500;
                result = new ErrorResponse(ex.Message);
                base.Logger.LogError(ex.ToString());
            }
            return StatusCode(code, result);

        }

        [HttpGet("search/raidus")]
        public ActionResult<ItemResponse<Paged<Location>>> SearchByRadiusPaginate(LocationSearchRequest model)
        {
            int code = 200;
            BaseResponse result = null;
            try
            {
                Paged<Location> page = _service.SearchByRadiusPaginate(model);

                if (page == null)
                {
                    code = 404;
                    result = new ErrorResponse("Resource not found.");
                }
                else
                {
                    ItemResponse<Paged<Location>> response = new ItemResponse<Paged<Location>>();
                    result = new ItemResponse<Paged<Location>> { Item = page };
                }
            }
            catch (Exception ex)
            {
                code = 500;
                result = new ErrorResponse(ex.Message);
                base.Logger.LogError(ex.ToString());
            }
            return StatusCode(code, result);

        }
        [HttpPost]
        public ActionResult<ItemResponse<int>> Create(LocationAddRequest model)
        {
            ObjectResult result = null;

            try
            {
                int userId = _authService.GetCurrentUserId();

                int id = _service.Add(model, userId);

                ItemResponse<int> response = new ItemResponse<int>();
                response.Item = id;

                result = StatusCode(200, response);
            }
            catch (Exception ex)
            {
                ErrorResponse response = new ErrorResponse(ex.Message);
                result = StatusCode(500, response);
            }

            return result;
        }

        [HttpPut("{id:int}")]
        public ActionResult<SuccessResponse> Update(LocationUpdateRequest model)
        {
            ObjectResult result = null;

            try
            {
                int userId = _authService.GetCurrentUserId();
                _service.Update(model, userId);

                SuccessResponse response = new SuccessResponse();

                result = StatusCode(200, response);
            }
            catch (Exception ex)
            {
                ErrorResponse response = new ErrorResponse(ex.Message);
                result = StatusCode(500, response);
            }
            return result;
        }

        [HttpDelete("{id:int}")]
        public ActionResult<SuccessResponse> Delete(int id)
        {
            int code = 200;
            ObjectResult result = null;

            try
            {
                _service.Delete(id);
                SuccessResponse response = new SuccessResponse();
                result = StatusCode(200, response);
            }
            catch (Exception ex)
            {
                ErrorResponse response = new ErrorResponse(ex.Message);
                result = StatusCode(500, response);
            }

            return StatusCode(code, result);
        }
    }
}

