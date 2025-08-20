using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Net;



namespace TaskTrackerClean.API.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("[controller]")]
    [ApiController]
    public class ErrorsController : ControllerBase
    {
        public ProblemDetailsFactory _problemDatailsFactory;

        public ErrorsController(ProblemDetailsFactory problemDetailsFactory)
        {
            _problemDatailsFactory = problemDetailsFactory;
        }

        public ActionResult Errors()
        {
            var exceptionHandler = HttpContext.Features.Get<IExceptionHandlerFeature>();

            if (exceptionHandler == null)
            {
                return new JsonResult(new { errorMessage = "Unkown error has occurred." });
            }


            var problem = new ProblemDetails();
            var exception = exceptionHandler.Error;

            problem.Detail = exception.Message;
            problem.Instance = exceptionHandler.Path;

            problem.Status = exception switch
            {
                KeyNotFoundException => (int)HttpStatusCode.NotFound,
                ArgumentException => (int)HttpStatusCode.BadRequest,
                InvalidOperationException => (int)HttpStatusCode.BadRequest,
                _ => (int)HttpStatusCode.InternalServerError
            };




            return MyProblem(problem);

        }

        [NonAction]
        public ObjectResult MyProblem(ProblemDetails problem)
        {
            var problemDetails = _problemDatailsFactory.CreateProblemDetails(
                HttpContext,
                statusCode: problem.Status ?? (int)HttpStatusCode.InternalServerError,
                title: problem.Title,
                type: problem.Type,
                detail: problem.Detail,
                instance: problem.Instance
            );

            return new ObjectResult(problemDetails)
            {
            };
        }
    }
}