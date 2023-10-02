using DotNetForHtml5;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.JSInterop;
using ServerMgrClient.Browser.Interop;

namespace ServerMgrClient.Browser.Pages
{
    [Route("/")]
    public class Index : ComponentBase
    {
        protected override void BuildRenderTree(RenderTreeBuilder __builder)
        {
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            Cshtml5Initializer.Initialize(new UnmarshalledJavaScriptExecutionHandler(JSRuntime));
            Program.RunApplication();
        }

        [Inject]
        private IJSRuntime JSRuntime { get; set; }
    }
}