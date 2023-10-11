using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketTelegramParser;
public class MarketItem
{
    public required string Title { get; set; }
    public int Price { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
}
