// <copyright file="ExceptionFilter.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using VirtualPark.Domain.Exceptions;

namespace VirtualPark.WebApi.Filters;
public class ExceptionFilter : IExceptionFilter
{
    private readonly Dictionary<Type, Func<Exception, IActionResult>> handlers;

    public ExceptionFilter()
    {
        this.handlers = new Dictionary<Type, Func<Exception, IActionResult>>
        {
            [typeof(ArgumentException)] = ex => new ObjectResult(new
            {
                Error = ex.Message, // da más contexto que un genérico
            })
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
            },

            [typeof(KeyNotFoundException)] = ex => new ObjectResult(new
            {
                Error = ex.Message,
            })
            {
                StatusCode = (int)HttpStatusCode.NotFound,
            },

            [typeof(InvalidOperationException)] = ex => new ObjectResult(new
            {
                Error = ex.Message,
            })
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
            },
            [typeof(DuplicateEmailException)] = ex => new ObjectResult(new { Error = ex.Message })
            {
                StatusCode = (int)HttpStatusCode.Conflict,
            },
            [typeof(InvalidCredentialsException)] = ex => new ObjectResult(new { Error = ex.Message })
            {
                StatusCode = (int)HttpStatusCode.Unauthorized,
            },
        };
    }

    public void OnException(ExceptionContext context)
    {
        if (this.handlers.TryGetValue(context.Exception.GetType(), out var handler))
        {
            context.Result = handler(context.Exception);
        }
        else if (context.Exception is DbUpdateException dbEx)
        {
            context.Result = new ObjectResult(new
            {
                Error = "Database update error",
                Details = dbEx.InnerException?.Message,
            })
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
            };
        }
        else
        {
            // Fallback para excepciones no contempladas
            context.Result = new ObjectResult(new
            {
                Error = "An unexpected error occurred.",
                ExceptionType = context.Exception.GetType().Name,
            })
            {
                StatusCode = (int)HttpStatusCode.InternalServerError,
            };
        }

        context.ExceptionHandled = true;
    }
}
