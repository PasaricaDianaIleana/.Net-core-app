using Swashbuckle.Swagger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Description;

namespace TRMDataManager.App_Start
{
    public class AuthTokenOperation : IDocumentFilter
    {
        public void Apply(SwaggerDocument swaggerDoc, SchemaRegistry schemaRegistry, IApiExplorer apiExplorer)
        {
            //add a new path(route) in swagger
            swaggerDoc.paths.Add("/token", new PathItem
            {
                //command
                post = new Operation 
                { 
                    // added in auth category
                   tags =  new List<string> { "Auth"},
                   //data type
                   consumes = new List<string>
                   {
                       //the body
                       "application/x-www-form-urlencoded"
                   },
                   //parameters def
                   parameters  = new List<Parameter>
                   {
                       new Parameter
                       {
                           type = "string",
                           name = "grant_type",
                           required = true,
                           @in = "formData",
                           @default = "password"
                       },
                            new Parameter
                       {
                           type = "string",
                           name = "username",
                           required = false,
                           @in = "formData"
                       },
                                 new Parameter
                       {
                           type = "string",
                           name = "password",
                           required = false,
                           @in = "formData"
                       }
                   }

                }
            });
        }
    }
}