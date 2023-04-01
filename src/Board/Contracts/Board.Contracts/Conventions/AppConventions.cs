using Board.Contracts.Contexts.Categories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Board.Contracts.Conventions
{
    public static class AppConventions
    {
        /// <response code="200">Запрос выполнен успешно.</response>
        [ApiConventionNameMatch(ApiConventionNameMatchBehavior.Prefix)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status404NotFound)]
        public static void Get([ApiConventionTypeMatch(ApiConventionTypeMatchBehavior.Any)][ApiConventionNameMatch(ApiConventionNameMatchBehavior.Suffix)] object id, 
            CancellationToken cancellation)
        {

        }

        [ApiConventionNameMatch(ApiConventionNameMatchBehavior.Prefix)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public static void Get([ApiConventionTypeMatch(ApiConventionTypeMatchBehavior.Any)] params object[] p)
        {

        }

        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status422UnprocessableEntity)]
        //[ProducesResponseType(typeof(ErrorDto), StatusCodes.Status500InternalServerError)]
        public static void Create([ApiConventionTypeMatch(ApiConventionTypeMatchBehavior.Any)] params object[] p)
        {
            
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status422UnprocessableEntity)]
        public static void Update([ApiConventionTypeMatch(ApiConventionTypeMatchBehavior.Any)] params object[] p)
        {
        }


        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status422UnprocessableEntity)]
        public static void Patch([ApiConventionTypeMatch(ApiConventionTypeMatchBehavior.Any)] params object[] p)
        {
        }

        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status404NotFound)]
        public static void Delete([ApiConventionTypeMatch(ApiConventionTypeMatchBehavior.Any)] params object[] p)
        {
        }

    }
}
