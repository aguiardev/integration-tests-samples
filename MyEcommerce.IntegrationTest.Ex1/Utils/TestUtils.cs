using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace MyEcommerce.IntegrationTest.Utils
{
    public static class TestUtils
    {
        public static bool IsHttpStatusCodeOK(this IActionResult noContentResult)
            => (noContentResult as OkObjectResult).StatusCode == (int)HttpStatusCode.OK;

        public static bool IsHttpStatusCodeNoContent(this IActionResult noContentResult)
            => noContentResult.As<NoContentResult>().StatusCode == (int)HttpStatusCode.NoContent;

        public static bool IsHttpStatusCodeNotFound(this IActionResult noContentResult)
            => noContentResult.As<NotFoundResult>().StatusCode == (int)HttpStatusCode.NotFound;

        public static bool IsHttpStatusCodeCreated(this IActionResult noContentResult)
            => noContentResult.As<CreatedAtActionResult>().StatusCode == (int)HttpStatusCode.Created;
    }
}
