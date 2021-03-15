using BookStoreApi.Data.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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

        public static ActionsAvalaibility GetActionsAvalaibility(this ControllerBase controller, string ownerId, string userId, bool currentUserAdmin) {
            ActionsAvalaibility actionsAvalaibility = new ActionsAvalaibility();
            actionsAvalaibility.CanBeViewed = !String.IsNullOrEmpty(ownerId);
            if ((!String.IsNullOrWhiteSpace(ownerId) && String.Equals(ownerId, userId))
                || currentUserAdmin
                ) {
                actionsAvalaibility.CanBeEdited = true;
                actionsAvalaibility.CanBeDeleted = true;
            }

            return actionsAvalaibility;
        }

        public static string GetCurrentUserEmail(this ControllerBase controller) {
            var nameClaim = controller.Request.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
            return nameClaim?.Value ?? String.Empty;
        }

    }
}
