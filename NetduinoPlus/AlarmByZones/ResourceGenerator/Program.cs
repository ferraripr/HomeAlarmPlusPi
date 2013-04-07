using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ResourceGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            ResourceGenerator.Htm.About aboutHtml = new Htm.About();
            aboutHtml.CreateAboutHtml();
            aboutHtml.CreateAboutHtmlMobile();
        }
    }
}
