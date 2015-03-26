# Ghost Crawler

Ghost Crawler is a .NET library based on AngleSharp that gives you the ability to extract data from the websites.

Documentation
==========

In the most simple case you only need to extract a simple data:

```c#

using (var xCrawler = new XWebCrawler())
{
    xCrawler.Navigate().GoToUrl(new Uri("https://www.nuget.org/"));             
    var version = xCrawler.GetElementByQuery(".release>h2").GetStringValue();
}

```
But if you need something more complex you could use:

```c#
using (var xCrawler = new XWebCrawler())
{
    xCrawler.Navigate().GoToUrl(new Uri("https://www.google.com.co/search?q=web+crawler"));              

    var selector = xCrawler.DataSelector();
    
    var definitions = new DataColumnDefinitionCollection();
    definitions.Add("Website Title", ".r>a", true);
    definitions.Add("Location", "._Rm", true);

    selector.SetTable("MainData", new DataTableSetting(definitions) { Scope = "#search", RepeatablePattern = "li.g" });
    selector.FillData("MainData");

    var dataSet = selector.GetDataSet();
    System.IO.File.WriteAllBytes(@"E:\GoogleSiteList.csv", dataSet.Tables["MainData"].ToCSV());
}
```
