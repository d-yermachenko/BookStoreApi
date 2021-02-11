using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStoreApi {
    public static class ControllerExtesions {
        public static string GetControllerActionNames(this ControllerBase ctrl) {
            var controller = ctrl.ControllerContext.ActionDescriptor.ControllerName;
            var action = ctrl.ControllerContext.ActionDescriptor.ActionName;

            return $"{controller} - {action}";
        }

        public static IActionResult InternalError(this ControllerBase ctrl, ILogger logger, string message) {
            logger.LogError(message);
            return ctrl.StatusCode(500, "Something went wrong. Please contact the Administrator");
        }

    }
}
