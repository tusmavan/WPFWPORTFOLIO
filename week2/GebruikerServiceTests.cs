using Microsoft.VisualStudio.TestTools.UnitTesting;
using Opdracht;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;

namespace Opdracht.Tests
{
    [TestClass()]
    public class GebruikerServiceTests
    {

        [TestMethod()]
        public void GebruikerServiceTest()
        {

        }

        [TestMethod()]
        public void RegistreerTest()
        {
            var gebruikerService = new Mock<GebruikerServiceMock>();
            var mail = "mail";
            var ww = "wachtwoord";
            Gebruiker geb = gebruikerService.Object.Registreer(mail, ww);
            Gebruiker geb2 = gebruikerService.Object.context.GetGebruikerByMail(mail);
            Assert.AreEqual(mail, geb.Email);
            Assert.AreEqual(ww, geb.Wachtwoord);
            Assert.AreEqual(mail, geb2.Email);
            Assert.AreEqual(ww, geb2.Wachtwoord);
        }

        [TestMethod()]
        public void LoginTest()
        {
            var gebruikerService = new Mock<GebruikerServiceMock>();
            Gebruiker geb = gebruikerService.Object.Registreer("emailadres", "Wachtwoooord");
            gebruikerService.Object.Verifieer("emailadres", "token");
            Assert.IsTrue(gebruikerService.Object.Login("emailadres", "Wachtwoooord"));

        }

        [TestMethod()]
        public void VerifieerTest()
        {
            var gebruikerService = new Mock<GebruikerServiceMock>();
            gebruikerService.Object.Registreer("mijnmail", "mijnww");
            Assert.IsFalse(gebruikerService.Object.Login("mijnmail", "mijnww"));
            gebruikerService.Object.Verifieer("mijnmail", "token");
            Assert.IsTrue(gebruikerService.Object.Login("mijnmail", "mijnww"));
        }

        [TestMethod()]
        public void VerifieerTestVerificatieVerlopen()
        {
            var gebruikerService = new Mock<GebruikerServiceMock>();
            gebruikerService.Object.Registreer("mijnmail2", "mijnww2");
            Assert.IsFalse(gebruikerService.Object.Login("mijnmail2", "mijnww2"));
            gebruikerService.Object.context.GetGebruikerByMail("mijnmail2").verificatieToken.verloopDatum = DateTime.Now.AddDays(-1);
            gebruikerService.Object.Verifieer("mijnmail2", "token");
            Assert.IsFalse(gebruikerService.Object.Login("mijnmail2", "mijnww2"));
        }
    }
}