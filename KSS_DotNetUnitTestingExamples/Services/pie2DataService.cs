using Services.Dto;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace Services
{
    public interface IPie2DataService
    {
        Task<Pie> BakePie(string flavour, int pastry, int filling, DateTime now);

        Task<List<Pie>> GetPieAudit();
    }

    public class Pie2DataService : IPie2DataService
    {
        public const string PieAudit = "pieAudit.json";

        public async Task<Pie> BakePie(string flavour, int pastry, int filling, DateTime now)
        {
            Pie pie = await CookPie(flavour, pastry, filling, now);

            await UpdatePieAudit(flavour, now);
            return pie;
        }

        public async Task<List<Pie>> GetPieAudit()
        {
            using var reader = File.OpenText(GetFilePath(PieAudit));

            var file = await reader.ReadToEndAsync();
            var r = JsonSerializer.Deserialize<List<Pie>>(file);

            return await Task.FromResult(JsonSerializer.Deserialize<List<Pie>>(file));
        }

        public async Task UpdatePieAudit(string flavour, DateTime now)
        {
            List<Pie> pies = new List<Pie> { new Pie { Flavour = flavour, LastMadeOn = now.ToString("T") } };
            var filePath = GetFilePath(PieAudit);
            FileInfo fileInfo = new FileInfo(filePath);
            if (fileInfo.Exists)
            {
                pies.AddRange(await GetPieAudit());
            }
            var text = JsonSerializer.Serialize(pies);
            using (var tw = new StreamWriter(filePath, false))
            {
                tw.Write(text);
            }
        }

        private string GetFilePath(string fileName)
        {
            var di = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).Parent;
            if (di != null && !di.Exists)
            {
                di.Create();
            }
            if (di != null && di.Exists)
            {
                return Path.Combine(di.FullName, fileName);
            }
            return null;
        }

        private async Task<Pie> CookPie(string flavour, int pastry, int filling, DateTime now)
        {
            Pie pie = new Pie
            {
                Flavour = flavour,
                LastMadeOn = now.ToString("T")
            };
            await WaitForPieToCook();
            return pie;
        }

        private async Task WaitForPieToCook()
        {
            await Task.Delay(TimeSpan.FromSeconds(1));
        }
    }

}
