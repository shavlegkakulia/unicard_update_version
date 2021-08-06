namespace Kunicardus.Domain.Migrations
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Data.Entity.Spatial;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    internal sealed class Configuration : DbMigrationsConfiguration<Kunicardus.Domain.Db.KunicardusDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(Kunicardus.Domain.Db.KunicardusDbContext context)
        {
            if (!context.Set<Advertisement>().Any())
            {
                List<Advertisement> advertisements = new List<Advertisement>();
                for (int i = 0; i < 10; i++)
                {
                    advertisements.Add(new Advertisement
                    {
                        Image = GetResourceImage("k9eNEQE.png"),
                        Status = AdvertisementStatus.Active,
                        ExternalUrl = "http://unimania.ge/ka/video/94",
                        Points = new Random().Next(10, 80),
                        Timeout = 15
                    });
                }

                List<Billboard> billboards = new List<Billboard>{
                new Billboard{
                    Advertisement = advertisements.ElementAt(0),
                    Location = DbGeography.FromText("POINT(44.86116886 41.68582898)"),
                    AlertDistance = 200,
                    MerchantLogo = GetResourceImage("socar.png"),
                    MerchantName = "სოკარი",
                    Points = 10,
                    Code = "Billbord_1"
                },
                new Billboard{
                    Advertisement = advertisements.ElementAt(1),
                    Location = DbGeography.FromText("POINT(44.86769199 41.68459908)"),
                    AlertDistance = 200,
                    MerchantLogo = GetResourceImage("gpc.png"),
                    MerchantName = "GPC",
                    Points = 10,
                    Code = "Billbord_2"
                },
                new Billboard{
                    Advertisement = advertisements.ElementAt(2),
                    Location = DbGeography.FromText("POINT(44.87672299 41.68397811)"),
                    AlertDistance = 200,
                    MerchantLogo = GetResourceImage("socar.png"),
                    MerchantName = "სოკარი",
                    Points = 10,
                    Code = "Billbord_3"
                },
                new Billboard{
                    Advertisement = advertisements.ElementAt(3),
                    BillboardId = 4,
                    Location = DbGeography.FromText("POINT(44.88282233 41.68407626)"),
                    AlertDistance = 200,
                    MerchantLogo = GetResourceImage("biblusi.png"),
                    MerchantName = "ბიბლუსი",
                    Points = 10,
                    Code = "Billbord_4"
                },
                new Billboard{
                    Advertisement = advertisements.ElementAt(4),
                    BillboardId = 5,
                    Location = DbGeography.FromText("POINT(44.88765299 41.6844829)"),
                    AlertDistance = 200,
                    MerchantLogo = GetResourceImage("socar.png"),
                    MerchantName = "სოკარი",
                    Points = 10,
                    Code = "Billbord_5"
                },
                new Billboard{
                    Advertisement = advertisements.ElementAt(5),
                    BillboardId = 6,
                    Location = DbGeography.FromText("POINT(44.89025474 41.68528614)"),
                    AlertDistance = 200,
                    MerchantLogo = GetResourceImage("biblusi.png"),
                    MerchantName = "ბიბლუსი",
                    Points = 10,
                    Code = "Billbord_6"
                },
                new Billboard{
                    Advertisement = advertisements.ElementAt(6),
                    BillboardId = 7,
                    Location = DbGeography.FromText("POINT(44.89936888 41.68773589)"),
                    AlertDistance = 200,
                    MerchantLogo = GetResourceImage("gpc.png"),
                    MerchantName = "GPC",
                    Points = 10,
                    Code = "Billbord_7"
                },
                new Billboard{
                    Advertisement = advertisements.ElementAt(7),
                    BillboardId = 8,
                    Location = DbGeography.FromText("POINT(44.91835624 41.68836684)"),
                    AlertDistance = 200,
                    MerchantLogo = GetResourceImage("socar.png"),
                    MerchantName = "სოკარი",
                    Points = 10,
                    Code = "Billbord_8"
                },
                new Billboard{
                    Advertisement = advertisements.ElementAt(8),
                    BillboardId = 9,
                    Location = DbGeography.FromText("POINT(44.92688835 41.68842492)"),
                    AlertDistance = 200,
                    MerchantLogo = GetResourceImage("socar.png"),
                    MerchantName = "სოკარი",
                    Points = 10,
                    Code = "Billbord_9"
                },
                new Billboard{
                    Advertisement = advertisements.ElementAt(9),
                    BillboardId = 10,
                    Location = DbGeography.FromText("POINT(44.93070781 41.6890819)"),
                    AlertDistance = 200,
                    MerchantLogo = GetResourceImage("socar.png"),
                    MerchantName = "სოკარი",
                    Points = 10,
                    Code = "Billbord_10"
                }
            };

                var userManager = new UserManager<User>(new UserStore<User>(context));
                userManager.Create(new User
                {
                    UserName = "misha"
                }, "123456");

                context.Set<Advertisement>().AddRange(advertisements);
                context.Set<Billboard>().AddRange(billboards);
            }
        }

        private UploadedFile GetResourceImage(string name)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "Kunicardus.Domain.Db.Migrations.Files." + name;
            var names = assembly.GetManifestResourceNames();
            if (!names.Contains(resourceName))
            {
                return null;
            }

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                var buffer = new byte[stream.Length];
                stream.Read(buffer, 0, (int)stream.Length);

                var result = new UploadedFile();
                result.Content = buffer;
                result.ContentType = "image\\png";
                result.FileName = Guid.NewGuid().ToString();
                result.Title = "Image";
                result.UploadDate = DateTime.Now;
                return result;
            }
        }
    }
}
