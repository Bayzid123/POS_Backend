
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using POS.Core.Models;
using SQLitePCL;

namespace POS.Core
{
    public interface IDataSource : IDisposable
    {
        EntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;
        DbSet<TblUser> Users
        {
            get; set;
        }

        DbSet<TblSettings> Settings
        {
            get; set;
        }
        DbSet<CounterSession> CounterSessions
        {
            get; set;
        }
        DbSet<SQLCounterSession> CounterSessionsSql
        {
            get; set;
        }
        DbSet<CounterSessionDetails> CounterSessionDetails
        {
            get; set;

        }
        DbSet<SqlCounterSessionDetails> SqlCounterSessionDetails
        {
            get; set;

        }
        DbSet<POSCounter> Counters
        {
            get; set;
        }
        DbSet<Item> Items
        {
            get; set;
        }
        DbSet<SQLItem> SQLItems
        {
            get; set;
        }
        
        DbSet<ItemSellingPriceHeader> SellingPriceHeaders
        {
            get; set;
        }
        DbSet<ItemSellingPriceRow> SellingPriceRows
        {
            get; set;
        }
        public DbSet<SQLServerItemSellingPriceRow> SQLServerSellingPriceRows
        {
            get; set;
        }

        public DbSet<ItemSellingPriceHeader> SQLItemSellingPriceHeaders
        {
            get; set;
        }


        DbSet<POSSalesDeliveryHeader> DeliveryHeaders
        {
            get; set;
        }
        DbSet<POSSalesDeliveryLine> SalesDeliveryLines
        {
            get; set;
        }
        DbSet<POSSalesPayment> SalesPayments
        {
            get; set;
        }
        DbSet<SQLPosSalesPayment> SqlSalesPayments
        {
            get; set;
        }
        DbSet<SalesWallet> SalesWallets
        {
            get; set;
        }
        DbSet<Partner> Partners
        {
            get; set;
        }

        //DbSet<SQLPartner> SQLPartners
        //{
        //    get; set;
        //}
        DbSet<UnitOfMeasure> UOMs
        {
            get; set;
        }
        DbSet<Warehouse> Warehouses
        {
            get; set;
        }
        DbSet<ItemWarehouseBalance> ItemBalance
        {
            get; set;
        }
        public DbSet<PartnerForSqlBD> PartnersSql
        {
            get; set;
        }

        DbSet<SQLServertblPromotionRow> SQLServerPromotionRows
        {
            get; set;
        }

        DbSet<tblPromotionRow> PromotionRows
        {
            get; set;
        }
        DbSet<SQLServertblPromotionHeader> SQLServerPromotionHeader
        {
            get; set;
        }

        DbSet<POSSpecialDiscount> SpecialDiscounts
        {
            get; set;
        }
        DbSet<SQLServerPOSSpecialDiscount> SQLServerSpecialDiscounts
        {
            get; set;
        }
        DbSet<SQLServerPOSSalesDeliveryLine> SQLServerSalesDeliveryLines
        {
            get; set;
        }
        DbSet<SQLServerPOSSalesDeliveryHeader> SQLServerDeliveryHeaders
        {
            get; set;
        }
        DbSet<tblDataLog> DataLogs
        {
            get; set;
        }
        DbSet<SQLtblDataLog> SQLDataLogs
        {
            get; set;
        }
        DbSet<User> SQLUser
        {
            get; set;
        }


        DbSet<tblPointOfferRow> PointOffer
        {
            get; set;
        }

        DbSet<SQLServerTblPointOfferRow> SqlServerPointOffer
        {
            get; set;
        }

        int SaveChanges();
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));
    }


}
