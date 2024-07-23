using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata;
using System.Text.Json;
using HtmlAgilityPack;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace CrawlerExample
{
    public class Crawler
    {
        public string Url;
        public string pathToStoreInfo;

        public class Product
        {
            public string? Url { get; set; }
            public string? Image { get; set; }
            public string? Name { get; set; }
            public string? Price { get; set; }

            public Product(String? Url, String? Image, String? Name, String? Price)
            {
                this.Url = Url;
                this.Image = Image;
                this.Name = Name;
                this.Price = Price;
            }

        }

        public Crawler(String Url, String pathToStoreInfo) {
            this.Url = Url;
            this.pathToStoreInfo = pathToStoreInfo;
        }

        public bool checkLastWriteTime(String pathToStoreInfo)
        {
            DateTime lastWriteTime = File.GetLastWriteTime(pathToStoreInfo);
            DateTime currentTime = DateTime.Now;
            return lastWriteTime.Date < currentTime.Date;
        }

        public void startCrawlerTask()
        {
            Thread thread = new Thread(new ThreadStart(this.crawlingWeb));
            thread.IsBackground = true;
            thread.Start();
        }

        private void crawlingWeb()
        {
            while (true)
            {
                Console.WriteLine("Try Crawling at " + DateTime.Now);;
                if (!File.Exists(pathToStoreInfo) || checkLastWriteTime(pathToStoreInfo))
                {
                    List<Product> products = new List<Product>();

                    var chromeOptions = new ChromeOptions();
                    // chromeOptions.AddArguments("headless"); 
                    // chromeOptions.AddArgument("'user-agent=Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/60.0.3112.50 Safari/537.36'");

                    using (var driver = new ChromeDriver(chromeOptions))
                    {

                        driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(2);
                        driver.Navigate().GoToUrl(Url);


                        var HtmlElements = driver.FindElements(By.CssSelector("a.m-card"));
                        Console.WriteLine("HtmlElements: " + HtmlElements.Count);

                        foreach (var HtmlElement in HtmlElements)
                        {
                            products.Add(new Product(
                                HtmlElement.GetAttribute("href"),
                                HtmlElement.FindElement(By.CssSelector("img.img-responsive")).GetAttribute("src"),
                                HtmlElement.FindElement(By.CssSelector("p.m-card__name")).Text,
                                HtmlElement.FindElement(By.CssSelector("p.m-card__price")).Text
                                )
                            );
                        }
                        /*
                        foreach (Product product in products) {
                            Console.WriteLine("Url: " + product.Url + " Image: " + product.Image + " name: " + product.Name
                            + " Price: " + product.Price);
                        }*/
                        if (HtmlElements.Count > 0)
                        {
                            File.WriteAllText(pathToStoreInfo, JsonSerializer.Serialize(products));
                        }
                        else
                        {
                            driver.GetScreenshot().SaveAsFile(pathToStoreInfo + "_debug.png");
                        }
                    }
                }
                Thread.Sleep(2 * 3600 * 1000);
            }

        }
    }
}