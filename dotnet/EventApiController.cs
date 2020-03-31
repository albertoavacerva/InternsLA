using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sabio.Models;
using Sabio.Models.Domain;
using Sabio.Models.Requests;
using Sabio.Models.Requests.Event;
using Sabio.Models.Requests.EventParticipant;
using Sabio.Models.Requests.NewFolder;
using Sabio.Services;
using Sabio.Web.Controllers;
using Sabio.Web.Models.Responses;

namespace Sabio.Web.Api.Controllers
{
    [Route("api/events")]
    [ApiController]
    public class EventApiController : BaseApiController
    {
        private IEventService _service = null;
        private IAuthenticationService<int> _authService = null;
        public EventApiController(IEventService service,
            ILogger<EventApiController> logger,
            IAuthenticationService<int> authService) : base(logger)
        {
            _service = service;
            _authService = authService;
        }


        [AllowAnonymous]
        [HttpGet("paginate")]
        public ActionResult<ItemResponse<Paged<Event>>> Paginate(int pageIndex, int pageSize)
        {
            int code = 200;
            BaseResponse response = null;

            try
            {
                Paged<Event> data = _service.Paginate(pageIndex, pageSize);

                if (data == null)
                {
                    code = 404;
                    response = new ErrorResponse("Resource not found.");

                }
                else
                {
                    response = new ItemResponse<Paged<Event>> { Item = data };

                }
            }

            catch (Exception ex)
            {

                code = 500;
                response = new ErrorResponse(ex.Message);
                base.Logger.LogError(ex.ToString());
            }

            return StatusCode(code, response);

        }
        [HttpGet("createdBy/{userId:int}")]
        public ActionResult<ItemsResponse<Event>> CreatedBy(int userId)
        {
            int code = 200;
            BaseResponse response = null;

            try
            {
                List<Event> data = _service.CreatedBy(userId);

                if (data == null)
                {
                    code = 404;
                    response = new ErrorResponse("Resource not found.");

                }
                else
                {
                    response = new ItemsResponse<Event> { Items = data };

                }
            }
            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.Message);
                base.Logger.LogError(ex.ToString());
            }
            return StatusCode(code, response);

        }

        [AllowAnonymous]
        [HttpGet("{id:int}")]
        public ActionResult<ItemResponse<Event>> Get(int id)
        {
            int code = 200;
            BaseResponse response = null;

            try
            {
                Event e = _service.Get(id);
                if (e == null)
                {
                    code = 404;
                    response = new ErrorResponse("Resource not found.");
                }
                else
                {
                    response = new ItemResponse<Event> { Item = e };
                }
            }
            catch (SqlException argEx)
            {
                code = 500;
                response = new ErrorResponse($"SqlException Errors: { argEx.Message}");
            }
            catch (ArgumentException argEx)
            {
                code = 500;
                response = new ErrorResponse($"ArgumentException Errors: { argEx.Message}");
            }
            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse($"Generic Errors: { ex.Message}");
                base.Logger.LogError(ex.ToString());
            }
            return StatusCode(code, response);
        }

        [HttpGet("participant/{id:int}")]
        public ActionResult<ItemResponse<int>> GetByUserId(int id)
        {
            int code = 200;
            BaseResponse response = null;

            try
            {
                List<int> eps = _service.GetByUserId(id);
                if (eps == null)
                {
                    code = 404;
                    response = new ErrorResponse("Resource not found.");
                }
                else
                {
                    response = new ItemsResponse<int> { Items = eps };
                }
            }
            catch (SqlException argEx)
            {
                code = 500;
                response = new ErrorResponse($"SqlException Errors: { argEx.Message}");
            }
            catch (ArgumentException argEx)
            {
                code = 500;
                response = new ErrorResponse($"ArgumentException Errors: { argEx.Message}");
            }
            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse($"Generic Errors: { ex.Message}");
                base.Logger.LogError(ex.ToString());
            }
            return StatusCode(code, response);
        }

        [HttpPost]
        public ActionResult<ItemResponse<int>> Add(EventAddMultiStep model)
        {
            int code = 200;
            BaseResponse response = null;
         
            try
            {
                int userId = _authService.GetCurrentUserId();
                int id = _service.Add(model, userId);

                response = new ItemResponse<int> { Item = id };
            }
            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse($"Generic Errors: { ex.Message}");
                base.Logger.LogError(ex.ToString());
            }

            return StatusCode(code, response);

        }

        [HttpPost("participant")]
        public ActionResult<SuccessResponse> AddParticipant(EventParticipantAddRequest model)
        {
            int code = 200;
            BaseResponse response = null;

            try
            {
                int userId = _authService.GetCurrentUserId();
             _service.AddParticipant(model, userId);

                response = new SuccessResponse();
            }
            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse($"Generic Errors: { ex.Message}");
                base.Logger.LogError(ex.ToString());
            }

            return StatusCode(code, response);

        }

        [HttpPut("{id:int}")]
        public ActionResult<SuccessResponse> Update(EventUpdateMultiStep model)
        {
            int code = 200;
            BaseResponse response = null;

            try
            {
                int userId = _authService.GetCurrentUserId();
                _service.Update(model, userId);

                response = new SuccessResponse();

            }
            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse($"Generic Errors: { ex.Message}");
                base.Logger.LogError(ex.ToString());
            }

            return StatusCode(code, response);

        }
 

       
    }
}