using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailSenderProgram
{
    /// <summary>
  /// Here is the main class , it will start with sending listcutomers and list orders to the constructer.
  /// the main class will not have a lot of method .
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
