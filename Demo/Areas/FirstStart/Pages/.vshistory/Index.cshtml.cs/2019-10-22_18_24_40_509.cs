using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Demo.Areas.FirstStart.Configurations;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Demo.Areas.FirstStart.Pages
{
    public class IndexModel : PageModel
    {
        public IndexModel(IWebHostEnvironment env, IConfiguration config)
        {
            Env = env ?? throw new System.ArgumentNullException(nameof(env));
            Config = config ?? throw new System.ArgumentNullException(nameof(config));
            //FirstStartConfig = new FirstStartConfiguration();
        }

        public IConfiguration Config { get; }
        public IWebHostEnvironment Env { get; }

        public bool Completed { get; set; } = false;
        public bool CompletedDefault { get; set; } = false;

        [BindProperty] public FirstStartConfiguration FirstStartConfig { get; set; }

        public IActionResult OnGet()
        {
            if (!IsAcessPage()) return NotFound();
            return Page();
        }

        private bool IsAcessPage() => Config.GetValue<bool>(ConstStrings.IsFirstStart);

        public async Task<IActionResult> OnPostDefaultInitializer(CancellationToken cancellationToken = default)
        {
            if (!IsAcessPage()) return NotFound();
            string settingsFileLocation = Path.Combine(Env.ContentRootPath, "appsettings.json");
            string fileContents = await System.IO.File.ReadAllTextAsync(settingsFileLocation, cancellationToken);
            JObject jsonFile = JsonConvert.DeserializeObject<JObject>(fileContents);
            jsonFile[ConstStrings.IsFirstStart] = false;
            await System.IO.File.WriteAllTextAsync(settingsFileLocation, JsonConvert.SerializeObject(jsonFile, Formatting.Indented), cancellationToken);
            CompletedDefault = true;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken = default)
        {
            if (!IsAcessPage()) return NotFound();

            if (!ModelState.IsValid) return Page();

            await WriteConfigFileToDiskAsync(FirstStartConfig, cancellationToken);

            Completed = true;

            return Page();
        }

        private async Task WriteConfigFileToDiskAsync(FirstStartConfiguration firstStartConfiguration, CancellationToken cancellationToken = default)
        {
            string settingsFileLocation = Path.Combine(Env.ContentRootPath, "appsettings.json");

            if (!System.IO.File.Exists(settingsFileLocation))
            {
                FileStream fileStream = System.IO.File.Create(settingsFileLocation);
                byte[] bytes = ASCIIEncoding.ASCII.GetBytes("{}");
                await fileStream.WriteAsync(bytes, 0, bytes.Length, cancellationToken);
                fileStream.Close();
                await fileStream.DisposeAsync();
            }

            string fileContents = await System.IO.File.ReadAllTextAsync(settingsFileLocation, cancellationToken);
            JObject jsonFile = JsonConvert.DeserializeObject<JObject>(fileContents);

            if (string.IsNullOrEmpty(firstStartConfiguration.ConnectionString))
                jsonFile["ConnectionStrings"][$"{firstStartConfiguration.Database}Connection"] = firstStartConfiguration.ConnectionString;

            jsonFile[ConstStrings.IsFirstStart] = false;
            jsonFile[ConstStrings.InitializeFakeData] = firstStartConfiguration.InitializeFakeData;
            jsonFile[ConstStrings.AdminUserName] = firstStartConfiguration.AdminUserName;
            jsonFile[ConstStrings.AdminEmail] = firstStartConfiguration.AdminEmail;
            jsonFile[ConstStrings.AdminPassword] = firstStartConfiguration.AdminPassword;
            jsonFile[ConstStrings.DataProvider] = firstStartConfiguration.Database;

            await System.IO.File.WriteAllTextAsync(settingsFileLocation, JsonConvert.SerializeObject(jsonFile, Formatting.Indented), cancellationToken);
        }
    }
}
