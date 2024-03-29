﻿namespace Dragonfly.UmbracoModels.MvcFakes
{
    using System.Collections.Specialized;
    using System.Web;
    using System.Web.Mvc;
    using Microsoft.AspNetCore.Routing;

    public class FakeControllerContext : ControllerContext
    {
        public FakeControllerContext(ControllerBase Controller)
            : this(Controller )
        {
        }

        //public FakeControllerContext(ControllerBase Controller)
        //    : this(Controller, null, null, null, null,  null)
        //{
        //}

        //public FakeControllerContext(ControllerBase Controller)
        //    : this(Controller, null, null, null, null, null)
        //{
        //}


        public FakeControllerContext(ControllerBase Controller, NameValueCollection FormParams) 
            : this(Controller, null, null, FormParams, null, null, null)
        {
        }


        public FakeControllerContext(ControllerBase Controller, NameValueCollection FormParams, NameValueCollection QueryStringParams)
            : this(Controller, null, null, FormParams, QueryStringParams, null, null)
        {
        }



        public FakeControllerContext(ControllerBase Controller, string UserName)
            : this(Controller, UserName, null, null, null, null, null)
        {
        }


        public FakeControllerContext(ControllerBase Controller, string UserName, string[] Roles)
            : this(Controller, UserName, Roles, null, null, null, null)
        {
        }


        public FakeControllerContext
            (
            ControllerBase Controller,
                string UserName,
                string[] Roles,
                NameValueCollection FormParams,
                NameValueCollection QueryStringParams
                //HttpCookieCollection Cookies,
                //SessionStateItemCollection SessionItems
            )
            : base(new FakeHttpContext(new FakePrincipal(new FakeIdentity(UserName), Roles), FormParams, QueryStringParams), new RouteData(), Controller)
        { }
    }
}