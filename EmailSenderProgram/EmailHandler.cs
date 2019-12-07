using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailSenderProgram
{
    class EmailHandler
    {
        private List<Customer> customers;   
        private List<Order> orders;

        /// <summary>
        /// consructor to receive the list of customers and orders
        /// </summary>
        /// <param name="customers"></param>
        /// <param name="orders"></param>
        public EmailHandler(List<Customer> customers, List<Order> orders)
        {
            this.customers = customers;
            this.orders = orders;
        }

       

        /// <summary>
        /// 
        /// </summary>
        public void SendEmails()
        {
            List<Customer> newCustomers;
            List<Customer> noOrderCustomers;
            GetCustomersByType(out newCustomers, out noOrderCustomers);

            //Call the method that do the work for me, I.E. sending the mails
            Console.WriteLine("Send Welcomemail");
            List<Customer> failed = SendEmailToCustomers(newCustomers, EmailType.WelcomeEmail, "");

#if DEBUG
            //Debug mode, always send Comeback mail
            Console.WriteLine("Send Comebackmail");
            failed.AddRange(
                SendEmailToCustomers(noOrderCustomers, EmailType.ComebackEmail, "EOComebackToUs"));
#else
			//Every Sunday run Comeback mail
			if (DateTime.Now.DayOfWeek.Equals(DayOfWeek.Sunday))
			{
				Console.WriteLine("Send Comebackmail");
                failed.AddRange(
                    SendEmailToCustomers(noOrderCustomers, EmailType.ComebackEmail, "EOComebackToUs"));
            }
#endif

            //Check if the sending went OK
            if (failed.Count == 0)
            {
                Console.WriteLine("All emails are sent.");
                return;
            }

            Console.WriteLine("Oops, something went wrong when sending mail to following customers:");
            foreach (Customer customer in failed)
            {
                Console.WriteLine(customer?.Email);
            }
        }
    
        /// <summary>
        /// method to add new customers to newCustomer list and cutomers with no order to noOrdercutomers list 
        /// </summary>
        /// <param name="newCustomers"></param>
        /// <param name="noOrderCustomers"></param>
        private void GetCustomersByType(out List<Customer> newCustomers, out List<Customer> noOrderCustomers)
        {
            newCustomers = new List<Customer>();
            noOrderCustomers = new List<Customer>();

            // loop throw custoemr 
            foreach (Customer customer in customers)
            {
                // Check if customer is new
                if (customer?.CreatedDateTime > DateTime.Now.AddDays(-1))
                {
                    newCustomers.Add(customer);
                    continue;
                }

                bool hasNoOrder = true;
                // Check if customer has placed an order
                foreach (Order order in orders)
                {
                    if (order.CustomerEmail == customer?.Email)
                    {
                        hasNoOrder = false;
                        break;
                    }
                }
                if (hasNoOrder)
                {
                    noOrderCustomers.Add(customer);
                }
            }
        }
              
        private List<Customer> SendEmailToCustomers(List<Customer> customers, EmailType emailType, string voucher)
        {
            List<Customer> failed = new List<Customer>();

            foreach (Customer customer in customers)
            {
                if (!SendlEmailToCustomer(emailType, customer?.Email, voucher))
                    failed.Add(customer);
            }

            return failed;
        }

        /// <summary>
        /// Handles the email for a customer. 
        /// Sends email if needed or return true if no email should be sent.
        /// </summary>
        /// <param name="emailType"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        public bool SendlEmailToCustomer(EmailType emailType, string email, string voucher)
        {
            string subject = "";
            string body = "";
            // If an email should be sent to the customer, get the email subject and body for the email type.
            if (!GetTitleAndBodyForEmailType(emailType, email, voucher, out subject, out body))
            {
                return false;
            }

            try
            {
                //Create a new MailMessage
                System.Net.Mail.MailMessage mailMessage = new System.Net.Mail.MailMessage();
                //Add customer to reciever list
                mailMessage.To.Add(email);
                //Add subject
                mailMessage.Subject = subject;
                //Send mail from info@EO.com
                mailMessage.From = new System.Net.Mail.MailAddress("info@EO.com");
                //Add body to mail
                mailMessage.Body = body;
#if DEBUG
                //Don't send mails in debug mode, just write the emails in console
                Console.WriteLine("Send mail to:" + email);
#else
	//Create a SmtpClient to our smtphost: yoursmtphost
					System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient("yoursmtphost");
					//Send mail
					smtp.Send(mailMessage);
#endif


                //All mails are sent! Success!
                return true;
            }
            catch (Exception exception)
            {
                //Something went wrong :(
                Console.WriteLine(exception.Message);
                return false;
            }

        }
      
        private bool GetTitleAndBodyForEmailType(EmailType emailType, string email, string voucher, out string title, out string body)
        {
            switch (emailType)
            {
                case EmailType.WelcomeEmail:
                    title = "Welcome as a new customer at EO!";
                    body = "Hi " + email +
                         "<br>We would like to welcome you as customer on our site!<br><br>Best Regards,<br>EO Team";
                    return true;
                case EmailType.ComebackEmail:
                    title = "We miss you as a customer";
                    body = "Hi " + email +
                                 "<br>We miss you as a customer. Our shop is filled with nice products. Here is a voucher that gives you 50 kr to shop for." +
                                 "<br>Voucher: " + voucher +
                                 "<br><br>Best Regards,<br>EO Team";
                    return true;
                case EmailType.NoEmail:
                default:
                    title = body = "";
                    return false;
            }
        }
    }
}
