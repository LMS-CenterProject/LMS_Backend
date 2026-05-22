using LMS.API.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Reflection;

namespace LMS.Application.UnitTests.Controllers
{
    [TestClass]
    public sealed class EnrollmentsControllerAuthorizationTests
    {
        [TestMethod]
        public void EnrollmentsController_UsesMethodLevelRolesWithoutBlockingAdmins()
        {
            var controllerAuthorize = typeof(EnrollmentsController)
                .GetCustomAttributes<AuthorizeAttribute>(inherit: true)
                .Single();

            Assert.IsTrue(string.IsNullOrWhiteSpace(controllerAuthorize.Roles));

            AssertMethodRole(nameof(EnrollmentsController.Enroll), "Student");
            AssertMethodRole(nameof(EnrollmentsController.GetMine), "Student");
            AssertMethodRole(nameof(EnrollmentsController.GetById), "Student");
            AssertMethodRole(nameof(EnrollmentsController.GetByCourse), "Instructor,Admin,SuperAdmin");
        }

        private static void AssertMethodRole(string methodName, string expectedRoles)
        {
            var method = typeof(EnrollmentsController).GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public);

            Assert.IsNotNull(method, $"Method '{methodName}' was not found.");

            var authorize = method!
                .GetCustomAttributes<AuthorizeAttribute>(inherit: true)
                .Single();

            Assert.AreEqual(expectedRoles, authorize.Roles);
        }
    }
}
