using SaRLAB.Models.Entity;

namespace SaRLAB.DataAccess.Service.BannerService
{
    public class BannerService : IBannerService
    {
        private readonly ApplicationDbContext _context;

        public BannerService(ApplicationDbContext context)
        {
            _context = context;
        }

        public void DeleteById(int id)
        {
            var bannerToDelete = _context.Banners.Find(id);
            if (bannerToDelete != null)
            {
                _context.Banners.Remove(bannerToDelete);
                _context.SaveChanges();
            }
        }

        public void DeleteByIds(string bannerIds)
        {
            // Split the string into individual IDs
            var ids = bannerIds.Split(',');

            // Convert string IDs to integers
            var bannerIdIntegers = ids.Select(id => int.Parse(id)).ToList();

            // Find and delete users with the specified IDs
            var bannersToDelete = _context.Banners.Where(u => bannerIdIntegers.Contains(u.ID)).ToList();

            if (bannersToDelete.Any())
            {
                _context.Banners.RemoveRange(bannersToDelete);
                _context.SaveChanges();
            }
        }

        public List<Banner> GetAll()
        {
            var banner = _context.Banners.Select(value => new Banner
            {
                ID = value.ID,
                PathImage = value.PathImage,
                CreateBy = value.CreateBy,
                CreateTime = value.CreateTime,
                UpdateBy = value.UpdateBy,
                UpdateTime = value.UpdateTime,
            });
            return banner.ToList();
        }

        public Banner GetById(int id)
        {
            var banner = _context.Banners.SingleOrDefault(item => item.ID == id);

            if (banner != null)
            {
                Banner _banner = new Banner
                {
                    ID = banner.ID,
                    PathImage = banner.PathImage,
                    CreateBy = banner.CreateBy,
                    CreateTime = banner.CreateTime,
                    UpdateBy = banner.UpdateBy,
                    UpdateTime = banner.UpdateTime,
                };

                return _banner;
            }
            else
            {
                return null;
            }
        }

        public Banner Insert(Banner banner)
        {
            var _bannner = _context.Banners.SingleOrDefault(item => (item.ID == banner.ID));

            if (_bannner != null)
            {
                return null;
            }

            var newbanner = new Banner
            {
                PathImage = banner.PathImage,
                CreateBy = banner.CreateBy,
                CreateTime = banner.CreateTime,
                UpdateBy = banner.UpdateBy,
                UpdateTime = banner.UpdateTime,
            };

            _context.Banners.Add(newbanner);
            _context.SaveChanges();

            return newbanner;
        }

        public Banner Update(Banner banner)
        {
            var _bannner = _context.Banners.SingleOrDefault(item => (item.ID == banner.ID));


            if (_bannner != null)
            {
                _bannner.PathImage = banner.PathImage;
                _bannner.CreateTime = banner.CreateTime;
                _bannner.UpdateTime = banner.UpdateTime;
                _bannner.UpdateBy = banner.UpdateBy;
                _context.SaveChanges();
            }

            return _bannner;
        }
    }
}
