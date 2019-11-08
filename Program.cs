using System;
using System.Threading;
using System.Collections.Generic;
using System.IO;

namespace LabAssignment2Part2
{
    class Program
    {
        static void Main(string[] args)
        {
            
            Stock stock1 = new Stock("Technology", 160, 5, 15);
            Stock stock2 = new Stock("Retail", 30, 2, 6);
            Stock stock3 = new Stock("Banking", 90, 4, 10);
            Stock stock4 = new Stock("Commodity", 500, 20, 50);

            StockBroker b1 = new StockBroker("Broker 1");
            b1.AddStock(stock1);
            b1.AddStock(stock2);

            StockBroker b2 = new StockBroker("Broker 2");
            b2.AddStock(stock1);
            b2.AddStock(stock3);
            b2.AddStock(stock4);

            StockBroker b3 = new StockBroker("Broker 3");
            b3.AddStock(stock1);
            b3.AddStock(stock3);

            StockBroker b4 = new StockBroker("Broker 4");
            b4.AddStock(stock1);
            b4.AddStock(stock2);
            b4.AddStock(stock3);
            b4.AddStock(stock4);

            Console.WriteLine("{0,-15} {1,-15} {2,-15} {3,-15}", "Broker",
                "Stock", "Value", "Changes");
        }
    }

    class Stock
    {
        public string stockName;
        int initVal;
        public int currentVal;
        int maxChange;
        int threshold;
        public int numberOfChanges;

        //public delegate void StockNotification(String stockName, int currentValue, int numberChanges);

        //public event StockNotification stockEvent;

        public event EventHandler<Stock> StockEvent;

        public Stock()
        {

        }

        public Stock(string stockNamein, int initValin, int maxChangein, int thresholdin)
        {
            stockName = stockNamein;
            initVal = initValin;
            maxChange = maxChangein;
            currentVal = initVal;
            
            threshold = thresholdin;
            numberOfChanges = 0;
            int notifyMe = maxChange;

            Thread newStockThread = new Thread(new ThreadStart(Activate));
            newStockThread.Start();
        }

        public void Activate()
        {
            for(int i = 0; i < 20; i++)
            {
                Thread.Sleep(500);
                ChangeStockValue();
            }

        }

        public void ChangeStockValue()
        {
            // Change the stock's value from between 1 and max change
            Random random = new Random();
            currentVal += random.Next(1, maxChange);
            numberOfChanges++;

            // If the change is greater than the threshold, Invoke event
            if (Math.Abs(currentVal - initVal) > threshold)
            {
                stockEventChange(this);
            }
        }

        protected virtual void stockEventChange(Stock stock)
        {
            // Invoke event and notify subscribers
            StockEvent?.Invoke(this, stock);
        }
    }

    class StockBroker
    {
        string brokerName;
        //List<Stock> stockList;
        private static Mutex mutex = new Mutex();

        public StockBroker()
        {

        }


        public StockBroker(string newBrokerName)
        {
            brokerName = newBrokerName;
            //stockList = new List<Stock>();

        }

        public void AddStock(Stock stock)
        {
            // Subscribe to this stock's stock event
            stock.StockEvent += writecons;
            stock.StockEvent += writefiles;
        }

        public void writecons(object sender, Stock stock)
        {
            // Write Stock List to console to test output
            Console.WriteLine("{0,-15} {1,-15} {2,-15} {3,-15}", brokerName, 
               stock.stockName, stock.currentVal, stock.numberOfChanges);
        }

        public void writefiles(object sender, Stock stock)
        {
            // DO MUTEX OR ELSE
            // mutex.WaitOne()
            mutex.WaitOne();
            string docPath = "/Users/christianbaiza/Desktop";
            using (StreamWriter outputFile = new StreamWriter(Path.Combine(docPath, "StockList.txt"), true))
            {
                outputFile.WriteLine("{0,-15} {1,-15} {2,-15} {3,-15}", brokerName,
               stock.stockName, stock.currentVal, stock.numberOfChanges);
            }
            mutex.ReleaseMutex();
            // mutex.ReleaseMutex();
        }
    }
}
