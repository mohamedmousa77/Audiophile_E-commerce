using Audiophile.Domain.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Mail;


namespace Audiophile.Application.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        private async Task SendEmailAsync(string to, string subject, string htmlBody)
        {
            var smtpServer = _configuration["Email:SmtpServer"];
            var smtpPort = int.Parse(_configuration["Email:SmtpPort"] ?? "587");
            var from = _configuration["Email:From"];
            var username = _configuration["Email:Username"];
            var password = _configuration["Email:Password"];

            try
            {
                using var client = new SmtpClient(smtpServer, smtpPort)
                {
                    EnableSsl = true,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(username, password)
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(from ?? "noreply@audiophile.com", "Audiophile E-Commerce"),
                    Subject = subject,
                    Body = htmlBody,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(to);

                await client.SendMailAsync(mailMessage);
                _logger.LogInformation("Email sent successfully to {Email}", to);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {Email}", to);
            }
        }

        public async Task SendEmailConfirmationAsync(User user, string token)
        {
            var confirmationLink = $"https://audiophile.com/confirm-email?token={token}&email={user.Email}";

            var htmlBody = $@"
                <html>
                <body style='font-family: Arial, sans-serif;'>
                    <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                        <h2 style='color: #D87D4A;'>Benvenuto su Audiophile!</h2>
                        <p>Ciao {user.FullName},</p>
                        <p>Grazie per esserti registrato. Per completare la registrazione, conferma il tuo indirizzo email cliccando sul link qui sotto:</p>
                        <p style='text-align: center; margin: 30px 0;'>
                            <a href='{confirmationLink}' style='background-color: #D87D4A; color: white; padding: 15px 30px; text-decoration: none; border-radius: 5px;'>
                                Conferma Email
                            </a>
                        </p>
                        <p>Oppure copia e incolla questo link nel tuo browser:</p>
                        <p style='word-break: break-all; color: #666;'>{confirmationLink}</p>
                        <p>Se non hai richiesto questa registrazione, ignora questa email.</p>
                        <hr style='margin: 30px 0; border: none; border-top: 1px solid #eee;'>
                        <p style='color: #999; font-size: 12px;'>Audiophile - Premium Audio Equipment</p>
                    </div>
                </body>
                </html>
            ";

            await SendEmailAsync(user.Email, "Conferma il tuo account Audiophile", htmlBody);
        }

        public async Task SendPasswordResetAsync(User user, string token)
        {
            var resetLink = $"https://audiophile.com/reset-password?token={token}&email={user.Email}";

            var htmlBody = $@"
                <html>
                <body style='font-family: Arial, sans-serif;'>
                    <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                        <h2 style='color: #D87D4A;'>Reset Password</h2>
                        <p>Ciao {user.FullName},</p>
                        <p>Hai richiesto il reset della tua password. Clicca sul link qui sotto per reimpostare la password:</p>
                        <p style='text-align: center; margin: 30px 0;'>
                            <a href='{resetLink}' style='background-color: #D87D4A; color: white; padding: 15px 30px; text-decoration: none; border-radius: 5px;'>
                                Reset Password
                            </a>
                        </p>
                        <p style='color: #d9534f;'><strong>Questo link scadrà tra 1 ora.</strong></p>
                        <p>Se non hai richiesto il reset della password, ignora questa email. Il tuo account è sicuro.</p>
                        <hr style='margin: 30px 0; border: none; border-top: 1px solid #eee;'>
                        <p style='color: #999; font-size: 12px;'>Audiophile - Premium Audio Equipment</p>
                    </div>
                </body>
                </html>
            ";

            await SendEmailAsync(user.Email, "Reset Password - Audiophile", htmlBody);
        }

        public async Task SendOrderConfirmationAsync(Order order)
        {
            var itemsHtml = string.Join("", order.Items.Select(item => $@"
                <tr>
                    <td style='padding: 10px; border-bottom: 1px solid #eee;'>{item.Product?.Name ?? "Prodotto"}</td>
                    <td style='padding: 10px; border-bottom: 1px solid #eee; text-align: center;'>{item.Quantity}</td>
                    <td style='padding: 10px; border-bottom: 1px solid #eee; text-align: right;'>{item.UnitPrice:C}</td>
                    <td style='padding: 10px; border-bottom: 1px solid #eee; text-align: right;'>{(item.UnitPrice * item.Quantity):C}</td>
                </tr>
            "));

            var htmlBody = $@"
                <html>
                <body style='font-family: Arial, sans-serif;'>
                    <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                        <h2 style='color: #D87D4A;'>Ordine Confermato!</h2>
                        <p>Ciao {order.CustomerInfo.FullName},</p>
                        <p>Il tuo ordine <strong>#{order.Id}</strong> è stato confermato e verrà processato a breve.</p>
                        
                        <div style='background-color: #f8f9fa; padding: 20px; border-radius: 5px; margin: 20px 0;'>
                            <h3 style='margin-top: 0;'>Riepilogo Ordine</h3>
                            <table style='width: 100%; border-collapse: collapse;'>
                                <thead>
                                    <tr style='background-color: #e9ecef;'>
                                        <th style='padding: 10px; text-align: left;'>Prodotto</th>
                                        <th style='padding: 10px; text-align: center;'>Qtà</th>
                                        <th style='padding: 10px; text-align: right;'>Prezzo</th>
                                        <th style='padding: 10px; text-align: right;'>Totale</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    {itemsHtml}
                                </tbody>
                                <tfoot>
                                    <tr>
                                        <td colspan='3' style='padding: 10px; text-align: right;'><strong>Subtotale:</strong></td>
                                        <td style='padding: 10px; text-align: right;'>{order.Subtotal:C}</td>
                                    </tr>
                                    <tr>
                                        <td colspan='3' style='padding: 10px; text-align: right;'><strong>Spedizione:</strong></td>
                                        <td style='padding: 10px; text-align: right;'>{order.Shipping:C}</td>
                                    </tr>
                                    <tr>
                                        <td colspan='3' style='padding: 10px; text-align: right;'><strong>IVA (20%):</strong></td>
                                        <td style='padding: 10px; text-align: right;'>{order.VAT:C}</td>
                                    </tr>
                                    <tr style='background-color: #e9ecef;'>
                                        <td colspan='3' style='padding: 15px; text-align: right;'><strong>TOTALE:</strong></td>
                                        <td style='padding: 15px; text-align: right; font-size: 18px; color: #D87D4A;'><strong>{order.Total:C}</strong></td>
                                    </tr>
                                </tfoot>
                            </table>
                        </div>

                        <div style='background-color: #f8f9fa; padding: 15px; border-radius: 5px; margin: 20px 0;'>
                            <h4 style='margin-top: 0;'>Indirizzo di Spedizione</h4>
                            <p style='margin: 5px 0;'>{order.CustomerInfo.FullName}</p>
                            <p style='margin: 5px 0;'>{order.CustomerInfo.Address}</p>
                            <p style='margin: 5px 0;'>{order.CustomerInfo.ZipCode} {order.CustomerInfo.City}</p>
                            <p style='margin: 5px 0;'>{order.CustomerInfo.Country}</p>
                            <p style='margin: 5px 0;'>Tel: {order.CustomerInfo.Phone}</p>
                        </div>

                        <p>Riceverai un'email di conferma quando il tuo ordine verrà spedito.</p>
                        <p>Grazie per aver scelto Audiophile!</p>
                        
                        <hr style='margin: 30px 0; border: none; border-top: 1px solid #eee;'>
                        <p style='color: #999; font-size: 12px;'>Audiophile - Premium Audio Equipment</p>
                    </div>
                </body>
                </html>
            ";

            await SendEmailAsync(order.CustomerInfo.Email, $"Ordine #{order.Id} Confermato", htmlBody);
        }

        public async Task SendOrderCancellationAsync(Order order)
        {
            var htmlBody = $@"
                <html>
                <body style='font-family: Arial, sans-serif;'>
                    <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                        <h2 style='color: #d9534f;'>Ordine Cancellato</h2>
                        <p>Ciao {order.CustomerInfo.FullName},</p>
                        <p>Il tuo ordine <strong>#{order.Id}</strong> è stato cancellato come richiesto.</p>
                        
                        <div style='background-color: #f8f9fa; padding: 15px; border-radius: 5px; margin: 20px 0;'>
                            <p><strong>Numero Ordine:</strong> #{order.Id}</p>
                            <p><strong>Totale Ordine:</strong> {order.Total:C}</p>
                            <p><strong>Motivo Cancellazione:</strong> {order.CancellationReason ?? "Non specificato"}</p>
                        </div>

                        <p>Se hai pagato con carta di credito, il rimborso verrà processato entro 5-7 giorni lavorativi.</p>
                        <p>Per qualsiasi domanda, contattaci a support@audiophile.com</p>
                        
                        <hr style='margin: 30px 0; border: none; border-top: 1px solid #eee;'>
                        <p style='color: #999; font-size: 12px;'>Audiophile - Premium Audio Equipment</p>
                    </div>
                </body>
                </html>
            ";

            await SendEmailAsync(order.CustomerInfo.Email, $"Ordine #{order.Id} Cancellato", htmlBody);
        }

        public async Task SendWelcomeEmailAsync(User user)
        {
            var htmlBody = $@"
                <html>
                <body style='font-family: Arial, sans-serif;'>
                    <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                        <div style='text-align: center; margin-bottom: 30px;'>
                            <h1 style='color: #D87D4A;'>Benvenuto su Audiophile!</h1>
                        </div>
                        
                        <p>Ciao {user.FullName},</p>
                        <p>Siamo entusiasti di averti con noi! 🎉</p>
                        
                        <p>Audiophile è il tuo punto di riferimento per l'audio di alta qualità. Scopri la nostra collezione di:</p>
                        <ul style='line-height: 1.8;'>
                            <li>🎧 Cuffie premium</li>
                            <li>🔊 Speaker wireless</li>
                            <li>🎵 Auricolari di alta fedeltà</li>
                        </ul>

                        <div style='text-align: center; margin: 30px 0;'>
                            <a href='https://audiophile.com/products' style='background-color: #D87D4A; color: white; padding: 15px 40px; text-decoration: none; border-radius: 5px; display: inline-block;'>
                                Scopri i Prodotti
                            </a>
                        </div>

                        <p>Goditi la spedizione gratuita per ordini superiori a 100€!</p>
                        
                        <hr style='margin: 30px 0; border: none; border-top: 1px solid #eee;'>
                        <p style='color: #999; font-size: 12px;'>Audiophile - Premium Audio Equipment</p>
                    </div>
                </body>
                </html>
            ";

            await SendEmailAsync(user.Email, "Benvenuto su Audiophile!", htmlBody);
        }
    }
}