using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using SuperPanel.App.Data;
using FluentAssertions;
using FluentValidation;
using Microsoft.AspNetCore.Hosting;
using SuperPanel.App;


namespace SuperPanelTestsNew
{
    public class UsersRepositoryTests
    {
        [Test]
        public void TestGetUserList()
        {

            using var scope = TestInit._scopeFactory.CreateScope();

            var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();

            var user_page_1 = userRepository.Query(string.Empty, "FirstName", false, 1, 20);

            user_page_1.Items.Should().NotBeNullOrEmpty();
            user_page_1.PageNumber.Should().Be(1);
        }

        [Test]
        public void TestDeleteGDPR()
        {
            using var scope = TestInit._scopeFactory.CreateScope();

            var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();

            var user_delete_result = userRepository.GDPRDeletion(1);

            user_delete_result.Result.Should().BeEmpty();
        }
    }
}