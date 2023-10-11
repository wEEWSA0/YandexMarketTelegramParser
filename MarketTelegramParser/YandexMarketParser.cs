using HtmlAgilityPack;

using OpenQA.Selenium.Chrome;

namespace MarketTelegramParser;
internal class YandexMarketParser
{
    private ChromeOptions _options;

    public YandexMarketParser()
    {
        _options = new ChromeOptions();
        //_options.AddArguments("--headless"); // Для запуска браузера в фоновом режиме
    }

    public List<MarketItem> ParseByName(string name)
    {
        var paramsUrl = $"&text={name.Replace(" ", "%20")}";

        var html = LoadHtmlFromUrl($"https://market.yandex.ru/catalog--myshi/26913130/list?srnum=2573{paramsUrl}&hid=14333188&allowCollapsing=1");

        return GetMarketItemsFromHtml(html);
    }

    public List<MarketItem> ParseByNameWithCostLimits(string name, int costFrom, int costTo)
    {
        var paramsUrl = $"&text={name.Replace(" ", "%20")}";

        var html = LoadHtmlFromUrl($"https://market.yandex.ru/catalog--myshi/26913130/list?srnum=2573{paramsUrl}&hid=14333188&allowCollapsing=1&pricefrom={costFrom}&priceto={costTo}");

        return GetMarketItemsFromHtml(html);
    }

    private string LoadHtmlFromUrl(string url)
    {
        var driver = new ChromeDriver(_options);

        driver.Navigate().GoToUrl(url);

        var pageSource = driver.PageSource;

        driver.Quit();

        return pageSource;
    }

    private List<MarketItem> GetMarketItemsFromHtml(string html)
    {
        HtmlDocument doc = new HtmlDocument();
        doc.LoadHtml(html);

        var articleNodes = doc.DocumentNode.SelectNodes("//article"); // Выбираем все элементы <article>

        List<MarketItem> marketItems = new();

        if (articleNodes is null)
        {
            return marketItems;
        }

        foreach (var articleNode in articleNodes)
        {
            var imgNode = articleNode.SelectSingleNode(".//img[@itemprop='image' and @data-tid]");
            var priceNode = articleNode.SelectSingleNode(".//div[@data-zone-name='price' and @data-baobab-name='price']//div//div[2]//div//div//h3");

            if (imgNode != null && priceNode != null)
            {
                var img = imgNode.GetAttributeValue("src", "").Substring(2);
                var title = imgNode.GetAttributeValue("title", "");
                var priceText = priceNode.InnerText.Replace("Цена с картой Яндекс Пэй:", "")
                    .Replace(" ₽", "")
                    .Replace(" ", "");
                var price = int.Parse(priceText);

                marketItems.Add(new MarketItem { ImageUrl = img, Title = title, Price = price });
            }
        }

        return marketItems;
    }
}
