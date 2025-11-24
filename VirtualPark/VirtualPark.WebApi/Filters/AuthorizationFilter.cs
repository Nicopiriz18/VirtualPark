// <copyright file="AuthorizationFilter.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using VirtualPark.Application.Session;
using VirtualPark.Domain;
using VirtualPark.Domain.Enums;

namespace VirtualPark.WebApi.Filters;

public class AuthorizationFilter : Attribute, IAuthorizationFilter
{
    private const string AUTHORIZATIONHEADER = "Authorization";
    private readonly RoleEnum? requiredRole;

    public bool RequireVisitor { get; set; } = false;

    public AuthorizationFilter()
    {
        this.requiredRole = null;
    }

    public AuthorizationFilter(RoleEnum requiredRole)
    {
        this.requiredRole = requiredRole;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var authorizationHeader = context.HttpContext.Request.Headers[AUTHORIZATIONHEADER].ToString();

        if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.StartsWith("Bearer "))
        {
            context.Result = new ObjectResult(new { message = "Unauthorized" })
            {
                StatusCode = (int)HttpStatusCode.Unauthorized,
            };
            return;
        }

        var token = authorizationHeader.Substring("Bearer ".Length).Trim();
        var sessionService = (ISessionService?)context.HttpContext.RequestServices.GetService(typeof(ISessionService));

        if (sessionService == null)
        {
            context.Result = new ObjectResult(new { message = "Unauthorized" })
            {
                StatusCode = (int)HttpStatusCode.Unauthorized,
            };
            return;
        }

        User user;
        try
        {
            user = sessionService.GetByToken(token) ?? throw new KeyNotFoundException();
        }
        catch (KeyNotFoundException)
        {
            context.Result = new ObjectResult(new { message = "Unauthorized" })
            {
                StatusCode = (int)HttpStatusCode.Unauthorized,
            };
            return;
        }

        if (this.requiredRole.HasValue)
        {
            var hasRole = user.Roles.Any(r => r == this.requiredRole.Value);

            if (!hasRole)
            {
                context.Result = new JsonResult(new { message = "Forbidden" })
                {
                    StatusCode = StatusCodes.Status403Forbidden,
                };
                return;
            }
        }

        if (this.RequireVisitor && user is not Visitor)
        {
            context.Result = new JsonResult(new { message = "Forbidden" })
            {
                StatusCode = StatusCodes.Status403Forbidden,
            };
            return;
        }
    }
}
