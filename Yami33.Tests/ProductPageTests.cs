using Bunit.TestDoubles;
using Microsoft.Extensions.DependencyInjection;
using Yami33.Models;             // Product
using Yami33.Utility;            // IProductRepository
using Yami33.Services;
using ProductPage = Yami33.Components.Pages.Product_Pages.ProductList;

namespace Yami33.Tests
{
    public class ProductPageTests : TestContext
    {
        private IRenderedComponent<ProductPage> RenderProductsPage()
            => RenderComponent<ProductPage>();

        private static int CountRows(IRenderedFragment cut) => cut.FindAll("tbody tr").Count;

        // Loading assertion removed from test:
        // cut.Markup.Should().Contain("/images/loading.gif");
        // Run: dotnet test --filter "FullyQualifiedName~ProductPageTests.FirstRender_RendersList_From_MockRepo"
        [Fact(DisplayName = "First render: renders two products from MockProductRepository")]
        public void FirstRender_RendersList_From_MockRepo()
        {
            Services.AddAuthorizationCore();
            var auth = this.AddTestAuthorization();
            auth.SetAuthorized("admin");
            auth.SetRoles("Admin");

            Services.AddScoped<IProductRepository, MockProductRepository>();
            JSInterop.SetupVoid("ShowConfirmationModal");
            JSInterop.SetupVoid("HideConfirmationModal");

            var cut = RenderComponent<ProductPage>(); // use alias or fully qualified component type

            // Remove this line; loading might disappear immediately after render:
            // cut.Markup.Should().Contain("/images/loading.gif");

            // Final assertions:
            cut.WaitForAssertion(() =>
            {
                cut.Markup.Should().Contain("Product List");
                cut.FindAll("table").Should().HaveCount(1);
                cut.FindAll("tbody tr").Count.Should().Be(2);
                cut.Markup.Should().Contain("$950.00");
                cut.Markup.Should().Contain("$499.00");
                cut.Markup.Should().NotContain("/images/loading.gif");
            });
        }

        // --------------------------------
        //  Delete ⇒ ShowConfirmationModal
        // --------------------------------
        [Fact(DisplayName = "Click Delete → shows confirmation modal")]
        public void Clicking_Delete_Shows_ConfirmationModal()
        {
            // Arrange
            Services.AddAuthorizationCore();
            var auth = this.AddTestAuthorization();
            auth.SetAuthorized("admin");
            auth.SetRoles("Admin");

            Services.AddScoped<IProductRepository, MockProductRepository>();
            JSInterop.SetupVoid("ShowConfirmationModal");
            JSInterop.SetupVoid("HideConfirmationModal");

            var cut = RenderProductsPage(); // same helper as before

            // Ensure the table has loaded
            cut.WaitForAssertion(() => CountRows(cut).Should().Be(2));

            // Act: click the Delete button on the first table row (not the modal's button)
            var rowDeleteBtn = cut.Find("table tbody tr:nth-child(1) button.btn-danger");
            rowDeleteBtn.Click();

            // Assert: JS function to show the modal should be invoked
            cut.WaitForAssertion(() => JSInterop.VerifyInvoke("ShowConfirmationModal"));
        }

        private sealed class EmptyRepo : IProductRepository
        {
            public Task<Product> CreateAsync(Product obj) => Task.FromResult(obj);
            public Task<Product> UpdateAsync(Product obj) => Task.FromResult(obj);
            public Task<bool> DeleteAsync(int id) => Task.FromResult(false);
            public Task<Product> GetAsync(int id) => Task.FromResult<Product>(null);
            public Task<IEnumerable<Product>> GetAllAsync() => Task.FromResult(Enumerable.Empty<Product>());
        }
    }
}