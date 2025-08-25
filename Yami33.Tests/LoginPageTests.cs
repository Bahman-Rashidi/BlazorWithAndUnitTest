using AngleSharp.Dom;
using Bunit;
using Bunit;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;

using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit;
using Yami33.Components.Account.Pages;
using Yami33.Services;
using Yami33.Services;
using Yami33.Utility;
// 👉 Namespaces پروژه خودت را تنظیم کن:
using Yami33.Utility;               // IAuthService


namespace Yami33.Tests
{
    //public class LoginPageTests : TestContext
    //{
    //    [Fact]
    //    public void Login_Success_Shows_Welcome()
    //    {
    //        // ماک سرویس
    //        Services.AddScoped<IAuthService>(_ => new MockAuthService());

    //        var cut = RenderComponent<Login>(); // نام صفحه لاگینت
    //        cut.Find("input[name=username]").Change("admin");
    //        cut.Find("input[name=password]").Change("123");
    //        cut.Find("button[type=submit]").Click();

    //     //   cut.MarkupMatches(markup => Assert.Contains("خوش آمدید", markup));
    //    }
    //}


    //public class LoginPageTests : TestContext
    //{
    //    // کمک‌کننده برای انتخاب دکمه‌ها (اولی: Login، دومی: test Message Box)
    //    private static IElement FindLoginButton(IRenderedFragment cut) => cut.FindAll("button")[0];
    //    private static IElement FindTestMsgButton(IRenderedFragment cut) => cut.FindAll("button")[1];

    //    [Fact(DisplayName = "ورود موفق: پیام موفقیت و مخفی شدن لودینگ")]
    //    public void Login_WithValidCreds_ShowsSuccess_And_HidesLoading()
    //    {
    //        // Arrange
    //        var authMock = new Mock<IAuthService>();
    //        authMock.Setup(s => s.LoginAsync("admin", "123"))
    //                .ReturnsAsync("FAKE.JWT");

    //        Services.AddScoped<IAuthService>(_ => authMock.Object);

    //        // ⚠️ اگر CustomAuthStateProvider سازنده بدون پارامتر ندارد،
    //        // این خط را با نسخه‌ی سازگار با پروژه‌ات تنظیم کن:
    //        Services.AddScoped<CustomAuthStateProvider>(_ => new CustomAuthStateProvider());

    //        var cut = RenderComponent<Login>();

    //        // Act
    //        FindLoginButton(cut).Click();

    //        // Assert
    //        cut.WaitForAssertion(() =>
    //        {
    //            // پیام متن لاگین
    //            cut.Markup.Should().Contain("Logged in!");
    //            // پیام موفقیت
    //            cut.Markup.Should().Contain("login  has been shown successfully!");
    //            // لودینگ نباید باشد
    //            cut.Markup.Should().NotContain("/images/loading.gif");
    //        });

    //        authMock.Verify(s => s.LoginAsync("admin", "123"), Times.Once);
    //    }

    //    [Fact(DisplayName = "ورود ناموفق: پیام شکست و باقی ماندن لودینگ")]
    //    public void Login_WithInvalidCreds_ShowsFailure_And_KeepsLoading()
    //    {
    //        // Arrange
    //        var authMock = new Mock<IAuthService>();
    //        authMock.Setup(s => s.LoginAsync(It.IsAny<string>(), It.IsAny<string>()))
    //                .ReturnsAsync((string?)null);

    //        Services.AddScoped<IAuthService>(_ => authMock.Object);
    //        Services.AddScoped<CustomAuthStateProvider>(_ => new CustomAuthStateProvider());

    //        var cut = RenderComponent<Login>();

    //        // تغییر ورودی‌ها (برعکس حالت پیش‌فرض admin/123)
    //        cut.Find("input[placeholder='Username']").Change("user");
    //        cut.Find("input[placeholder='Password']").Change("123");

    //        // Act
    //        FindLoginButton(cut).Click();

    //        // Assert
    //        cut.WaitForAssertion(() =>
    //        {
    //            cut.Markup.Should().Contain("Login failed.");
    //            // در حالت ناموفق IsProcessing=false می‌ماند، پس لودینگ باید باشد
    //            cut.Markup.Should().Contain("/images/loading.gif");
    //            // پیام موفقیت نباید باشد
    //            cut.Markup.Should().NotContain("alert alert-success");
    //        });

    //        authMock.Verify(s => s.LoginAsync("user", "123"), Times.Once);
    //    }

    //    [Fact(DisplayName = "JSInterop: کلیک روی test Message Box، فراخوانی showAlert با HI")]
    //    public void Clicking_TestMessageBox_Invokes_JS_ShowAlert()
    //    {
    //        // Arrange
    //        // مقدار بازگشتی مهم نیست؛ فقط می‌خواهیم invoke شدن چک شود
    //        JSInterop.Setup<bool>("showAlert", "HI").SetResult(false);

    //        // سرویس‌ها برای اینکه رندر خطا ندهد (در این تست Login زده نمی‌شود)
    //        Services.AddScoped<IAuthService>(_ => Mock.Of<IAuthService>());
    //        Services.AddScoped<CustomAuthStateProvider>(_ => new CustomAuthStateProvider());

    //        var cut = RenderComponent<Login>();

    //        // Act
    //        FindTestMsgButton(cut).Click();

    //        // Assert
    //        var invoke = JSInterop.VerifyInvoke("showAlert");
    //        invoke.Arguments[0]!.ToString().Should().Be("HI");
    //    }
    //}

    public class LoginPageTests : TestContext
    {
        private static IElement LoginBtn(IRenderedFragment cut) => cut.FindAll("button")[0];
        private static IElement JsBtn(IRenderedFragment cut) => cut.FindAll("button")[1];


        //dotnet test --filter "FullyQualifiedName~LoginPageTests.Login_Success_ShowsSuccess_And_HidesLoading"
        [Fact(DisplayName = "ورود موفق: پیام موفقیت و حذف لودینگ")]
        public void Login_Success_ShowsSuccess_And_HidesLoading()
        {
            // Arrange
            Services.AddScoped<IAuthService, MockAuthService>();
            // اگر CustomAuthStateProvider وابستگی خاصی ندارد:
            //  Services.AddScoped<CustomAuthStateProvider>(_ => new CustomAuthStateProvider());




            Services.AddScoped<CustomAuthStateProvider>(sp =>
    new CustomAuthStateProvider(
        sp.GetRequiredService<IJSRuntime>()));  // 👈 اگر ctor چیزهای دیگری می‌خواهد، همینجا بده
                                                // ... سایر دیپندنسی‌ها





            var cut = RenderComponent<Login>(); // نام کامپوننتت

            // Act (ورودی‌های پیش‌فرض صفحه admin/123 هستند)
            LoginBtn(cut).Click();

            // Assert
            cut.WaitForAssertion(() =>
            {
                cut.Markup.Should().Contain("Logged in!");
                cut.Markup.Should().Contain("login  has been shown successfully!");
                cut.Markup.Should().NotContain("/images/loading.gif"); // لودینگ نباید باشد
            });
        }



        //dotnet test --filter "FullyQualifiedName~LoginPageTests.Login_Failure_ShowsFailure_And_KeepsLoading"

        [Theory(DisplayName = "ورود ناموفق: پیام شکست و باقی ماندن لودینگ")]
        [InlineData("user", "123")]
        [InlineData("foo", "bar")]
        [InlineData("admin", "wrong")]
        public void Login_Failure_ShowsFailure_And_KeepsLoading(string u, string p)
        {
            // Arrange
            Services.AddScoped<IAuthService, MockAuthService>();
            Services.AddScoped<CustomAuthStateProvider>(sp =>
new CustomAuthStateProvider(
sp.GetRequiredService<IJSRuntime>()));
            var cut = RenderComponent<Login>();

            // Override ورودی‌ها
            cut.Find("input[placeholder='Username']").Change(u);
            cut.Find("input[placeholder='Password']").Change(p);

            // Act
            LoginBtn(cut).Click();

            // Assert
            cut.WaitForAssertion(() =>
            {
                cut.Markup.Should().Contain("Login failed.");
                cut.Markup.Should().Contain("/images/loading.gif"); // IsProcessing=false ⇒ لودینگ هست
                cut.Markup.Should().NotContain("alert alert-success");
            });
        }


        //dotnet test --filter "FullyQualifiedName~LoginPageTests.JsInterop_ShowAlert_Is_Called_With_HI"

        [Fact(DisplayName = "JSInterop: کلیک روی test Message Box ⇒ showAlert('HI')")]
        public void JsInterop_ShowAlert_Is_Called_With_HI()
        {
            // Arrange: مقدار بازگشتی مهم نیست
            JSInterop.Setup<bool>("showAlert", "HI").SetResult(false);

            Services.AddScoped<IAuthService, MockAuthService>();
            Services.AddScoped<CustomAuthStateProvider>(sp =>
new CustomAuthStateProvider(
sp.GetRequiredService<IJSRuntime>()));

            var cut = RenderComponent<Login>();

            // Act
            JsBtn(cut).Click();

            // Assert
            var invoke = JSInterop.VerifyInvoke("showAlert");
            invoke.Arguments[0]!.ToString().Should().Be("HI");
        }
    }
}
