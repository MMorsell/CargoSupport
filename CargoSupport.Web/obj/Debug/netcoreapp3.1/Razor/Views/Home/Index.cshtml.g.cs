#pragma checksum "C:\Users\Eraza\Source\Repos\MMorsell\CargoSupport\CargoSupport.Web\Views\Home\Index.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "29be84bcf1cec148decd4cd4fc85789ebd854839"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Home_Index), @"mvc.1.0.view", @"/Views/Home/Index.cshtml")]
namespace AspNetCore
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
#nullable restore
#line 1 "C:\Users\Eraza\Source\Repos\MMorsell\CargoSupport\CargoSupport.Web\Views\_ViewImports.cshtml"
using CargoSupport.Web;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "C:\Users\Eraza\Source\Repos\MMorsell\CargoSupport\CargoSupport.Web\Views\_ViewImports.cshtml"
using CargoSupport.Web.Models;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"29be84bcf1cec148decd4cd4fc85789ebd854839", @"/Views/Home/Index.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"08a46639539cb1b6b287bbef5306f5e2c5fc89f7", @"/Views/_ViewImports.cshtml")]
    public class Views_Home_Index : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<List<CargoSupport.Models.PinRouteModel>>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
#nullable restore
#line 2 "C:\Users\Eraza\Source\Repos\MMorsell\CargoSupport\CargoSupport.Web\Views\Home\Index.cshtml"
  
    ViewData["Title"] = "Home Page";

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n<div class=\"text-center\">\r\n    <h1>Hello from ag-grid!</h1>\r\n    <div id=\"myGrid\" style=\"height: 100vw; width: 100vw;\" class=\"ag-theme-material\"></div>\r\n</div>\r\n\r\n");
            DefineSection("Scripts", async() => {
                WriteLiteral("\r\n    <script type=\"text/javascript\" charset=\"utf-8\">\r\n\r\n        var allData = ");
#nullable restore
#line 15 "C:\Users\Eraza\Source\Repos\MMorsell\CargoSupport\CargoSupport.Web\Views\Home\Index.cshtml"
                 Write(Html.Raw(ViewBag.DataTable));

#line default
#line hidden
#nullable disable
                WriteLiteral(@"
        // specify the columns
        let columnDefs = [
            { headerName: ""Id"", field: ""Id"", sortable: true, filter: true, checkboxSelection: true },
            { headerName: ""RouteName"", field: ""RouteName"", sortable: true, filter: true },
            { headerName: ""Driver"", field: ""Driver"", sortable: true, filter: true },
            { headerName: ""EstimatedRouteStart"", field: ""EstimatedRouteStart"", sortable: true, filter: true },
            { headerName: ""EstimatedRouteEnd"", field: ""EstimatedRouteEnd"", sortable: true, filter: true },
            { headerName: ""LoadingIsDone"", field: ""LoadingIsDone"", sortable: true, filter: true },
            { headerName: ""NumberOfColdBoxes"", field: ""NumberOfColdBoxes"", sortable: true, filter: true },
            { headerName: ""NumberOfFrozenBoxes"", field: ""NumberOfFrozenBoxes"", sortable: true, filter: true },
            { headerName: ""PreRideAnnotation"", field: ""PreRideAnnotation"", sortable: true, filter: true },
            { headerName: ""PostRi");
                WriteLiteral(@"deAnnotation"", field: ""PostRideAnnotation"", sortable: true, filter: true },
        ];

        // let the grid know which columns and what data to use
        let gridOptions = {
            columnDefs: columnDefs,
            rowData: allData
        };

        // lookup the container we want the Grid to use
        var eGridDiv = document.querySelector('#myGrid');

        // create the grid passing in the div to use together with the columns & data we want to use
        new agGrid.Grid(eGridDiv, gridOptions);
    </script>
");
            }
            );
        }
        #pragma warning restore 1998
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<List<CargoSupport.Models.PinRouteModel>> Html { get; private set; }
    }
}
#pragma warning restore 1591
