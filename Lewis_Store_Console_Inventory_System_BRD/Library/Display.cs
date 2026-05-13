using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lewis_Store_Console_Inventory_System_BRD.Library
{
    public class Display
    {
        public DatabaseManager display = new DatabaseManager();

        public Table StockTable;

        public Rows StockWarningRow;
        public Panel StockWarning;
        public List<string> ProductList;

        public Display()
        {
            StockTable = new Table()
                        .RoundedBorder()
                        .BorderColor(Color.Grey)
                        .ShowRowSeparators()
                        .Expand();
            StockTable.AddColumn("[yellow]Item[/]");
            StockTable.AddColumn("[yellow]Description[/]");
            StockTable.AddColumn("[yellow]Qty[/]");
            StockTable.AddColumn("[yellow]Price.Excl VAT[/]");

            var (productlist, stockwarn) = display.DisplayStock(StockTable);

            ProductList = productlist;

            StockWarningRow = new Rows(stockwarn);

            StockWarning = new Panel(StockWarningRow) { Width = 50 }
                        .Header("[red bold]Stock Warnings[/]", Justify.Center)
                        .RoundedBorder()
                        .BorderColor(Color.Red)
                        .Padding(1, 1);
        }

        public virtual void ViewStock()
        {

            var DisplayPanel = new Panel(StockTable)
                .Header("[lightgreen bold]View Product Stock[/]", Justify.Center)
                .RoundedBorder()
                .BorderColor(Color.Blue)
                .Padding(2, 1);

            var DisplayLayout = new Columns(new Spectre.Console.Rendering.IRenderable[]
                {
                DisplayPanel,
                StockWarning
                });

            AnsiConsole.Write(DisplayLayout);
        }

        public void UpdateStock()
        {
            var DisplayPanel = new Panel(StockTable)
                .Header("[lightgreen bold]Update Product Stock[/]", Justify.Center)
                .RoundedBorder()
                .BorderColor(Color.Blue)
                .Padding(2, 1);

            var DisplayLayout = new Columns(new Spectre.Console.Rendering.IRenderable[]
                {
                DisplayPanel,
                StockWarning
                });

            AnsiConsole.Write(DisplayLayout);

        }

        public void DeleteStock()
        {
            var DisplayPanel = new Panel(StockTable)
                .Header("[lightgreen bold]Delete Product[/]", Justify.Center)
                .RoundedBorder()
                .BorderColor(Color.Blue)
                .Padding(2, 1)
                .Expand();

            var DisplayLayout = new Columns(new Spectre.Console.Rendering.IRenderable[]
                {
                DisplayPanel
                });

            AnsiConsole.Write(DisplayLayout);
        }

        public void SellItemMenu(List<Product> CartList)
        {
            Panel SellItems = new Panel(StockTable)
                .Header("[lightgreen bold]Purchase[/]", Justify.Center)
                .RoundedBorder()
                .BorderColor(Color.Blue)
                .Padding(2, 1);

            Table Cart = new Table()
                .RoundedBorder()
                .BorderColor(Color.Grey)
                .Expand();

            Cart.AddColumn("[yellow]Items[/]", col => col.Centered());

            foreach (var item in CartList)
            {
                Cart.AddRow($"{item.Name} x{item.Quantity} - R{item.PriceExclVAT * item.Quantity}");
            }

            Panel CartPanel = new Panel(Cart)
                .Header("[lightgreen bold]Cart[/]", Justify.Center)
                .RoundedBorder()
                .BorderColor(Color.Blue)
                .Padding(2, 1);

            Columns SellLayout = new Columns(new Spectre.Console.Rendering.IRenderable[]
                {
                SellItems,
                CartPanel});

            AnsiConsole.Write(new Rule("[lightgreen bold] SELL STOCK[/]"));
            AnsiConsole.Write(SellLayout);

        }

        public void SalesHistory()
        {
            List<Table> SaleItemHist = new List<Table>();

            Table SalesTable = new Table()
                .RoundedBorder()
                .BorderColor(Color.Grey)
                .ShowRowSeparators();

            SalesTable.AddColumns(new string[] { "[yellow]Sale Number[/]", "[yellow]Sale ID[/]", "[yellow]Subtotal[/]", "[yellow]VAT Amount[/]", "[yellow]Total Amount[/]", "[yellow]Sale Date[/]" });

            DatabaseManager SaleHist = new DatabaseManager();
            SaleHist.SaleHistory(SalesTable, SaleItemHist);

            Panel SalesPanel = new Panel(SalesTable) { Width = 70 }
                .Header("[lightgreen bold]Sales History[/]", Justify.Center)
                .RoundedBorder()
                .BorderColor(Color.Blue)
                .Padding(2, 1)
                .Expand();

            if (SaleItemHist.Count == 0)
            {
                AnsiConsole.MarkupLine("[yellow]No sales history found yet.[/]");
                Console.ReadKey();
                return;
            }

            Panel ItemPanel = new Panel(SaleItemHist[0]) { Width = 45 }
                .Header("[lightgreen bold]Items From Sale[/]", Justify.Center)
                .RoundedBorder()
                .BorderColor(Color.Blue)
                .Padding(2, 1)
                .Expand();

            while (true)
            {
                AnsiConsole.Clear();

                Columns SalesHistMenu = new Columns(new Spectre.Console.Rendering.IRenderable[]{
              SalesPanel,
                ItemPanel
            });

                AnsiConsole.Write(new Rule("[lightgreen bold] The Lewis Store Inventory Management System[/]"));
                AnsiConsole.MarkupLine("\n[cyan]ℹ Scroll with Arrow Keys, Press Enter to Select[/]");
                AnsiConsole.Write(SalesHistMenu);

                var Choice = AnsiConsole.Prompt(new SelectionPrompt<string>()
                    .Title("\nSelect A Sale To View Items")
                    .PageSize(10)
                    .EnableSearch()
                    .SearchPlaceholderText("Type to search Sales...")
                    .AddChoices("[red]Exit[/]")
                    .AddChoices(SaleItemHist.Select((table, index) => $"Sale {index + 1}").ToList())
                    .WrapAround());

                if (Choice == "[red]Exit[/]")
                {
                    Console.Clear();
                    break;
                }

                int selectedIndex = int.Parse(Choice.Split(' ')[1]) - 1;
                ItemPanel = new Panel(SaleItemHist[selectedIndex]) { Width = 45 }
                .Header("[lightgreen bold]Items From Sale[/]", Justify.Center)
                .RoundedBorder()
                .BorderColor(Color.Blue)
                .Padding(2, 1)
                .Expand();

                Console.Clear();
            }
        }
    }

}
