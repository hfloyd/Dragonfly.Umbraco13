namespace Dragonfly.UmbracoModels.MvcFakes
{
    using System;
    using System.Web.Mvc;


    public class FakeController : ControllerBase
    {
        public void Execute()
        {
            throw new InvalidOperationException();
        }

        protected override void ExecuteCore()
        {
            throw new InvalidOperationException();
        }
    }
}
