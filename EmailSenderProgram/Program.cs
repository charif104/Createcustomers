using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailSenderProgram
{
    /// <summary>
  /// here is the main class , it will start with sending listcutomers and list orders to the constructer
  /// 
  /// </summary>
    internal class Program
    {
        /// <summary>
        /// This application is run everyday
        /// </summary>
        /// <param name="args"></param>
        private static void Main(string[] args)
        {
            EmailHandler emaiHandler = new EmailHandler(DataLayer.ListCustomers(), DataLayer.ListOrders());

            emaiHandler.SendEmails();

            Console.ReadKey();
        }

    }
}
