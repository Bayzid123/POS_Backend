using Microsoft.EntityFrameworkCore;
using POS.Core.Models;

namespace POS.Core;

public class SQLServerDb : DbContext, IDataSource
{
    private string _connectionString = null;

    public SQLServerDb(string connectionString)
    {
        _connectionString = connectionString;
    }

    public DbSet<TblUser> Users
    {
        get; set;
    }
    public DbSet<TblSettings> Settings
    {
        get; set;
    }
    public DbSet<CounterSession> CounterSessions
    {
        get; set;
    }
    public DbSet<SQLCounterSession> CounterSessionsSql
    {
        get; set;
    }
    public DbSet<CounterSessionDetails> CounterSessionDetails
    {
        get; set;

    }
    public DbSet<SqlCounterSessionDetails> SqlCounterSessionDetails
    {
        get; set;
    }
    public DbSet<POSCounter> Counters
    {
        get; set;
    }
    public DbSet<Item> Items
    {
        get; set;
    }

    public DbSet<SQLItem> SQLItems
    {
        get; set;
    }

    
    public DbSet<ItemSellingPriceHeader> SellingPriceHeaders
    {
        get; set;
    }
    public DbSet<ItemSellingPriceRow> SellingPriceRows
    {
        get; set;
    }
    public DbSet<SQLServerItemSellingPriceRow> SQLServerSellingPriceRows
    {
        get; set;
    }
    public DbSet<POSSalesDeliveryHeader> DeliveryHeaders
    {
        get; set;
    }
    public DbSet<POSSalesDeliveryLine> SalesDeliveryLines
    {
        get; set;
    }
    public DbSet<POSSalesPayment> SalesPayments
    {
        get; set;
    }
    public DbSet<SalesWallet> SalesWallets
    {
        get; set;
    }
    public DbSet<Partner> Partners
    {
        get; set;
    }
    //public DbSet<SQLPartner> SQLPartners
    //{
    //    get; set;
    //}
    public DbSet<PartnerForSqlBD> PartnersSql
    {
        get; set;
    }
    public DbSet<UnitOfMeasure> UOMs
    {
        get; set;
    }
    public DbSet<Warehouse> Warehouses
    {
        get; set;
    }
    public DbSet<ItemWarehouseBalance> ItemBalance
    {
        get; set;
    }

    public DbSet<SQLServertblPromotionRow> SQLServerPromotionRows
    {
        get; set;
    }

    public DbSet<tblPromotionRow> PromotionRows
    {
        get; set;
    }

    public DbSet<SQLServertblPromotionHeader> SQLServerPromotionHeader
    {
        get; set;
    }

    public DbSet<POSSpecialDiscount> SpecialDiscounts
    {
        get; set;
    }
    public DbSet<SQLServerPOSSpecialDiscount> SQLServerSpecialDiscounts
    {
        get; set;
    }

    public DbSet<SQLServerPOSSalesDeliveryLine> SQLServerSalesDeliveryLines
    {
        get; set;
    }
    public DbSet<SQLServerPOSSalesDeliveryHeader> SQLServerDeliveryHeaders
    {
        get; set;
    }
    public DbSet<SQLPosSalesPayment> SqlSalesPayments
    {
        get;
        set;
    }
    public DbSet<ItemSellingPriceHeader> SQLItemSellingPriceHeaders
    {
        get;
        set;
    }
    public DbSet<tblDataLog> DataLogs
    {
        get;
        set;
    }
    public DbSet<SQLtblDataLog> SQLDataLogs
    {
        get;
        set;
    }
    public DbSet<User> SQLUser
    {
        get;
        set;
    }

    public DbSet<tblPointOfferRow> PointOffer
    {
        get; set;
    }

    public DbSet<SQLServerTblPointOfferRow> SqlServerPointOffer
    {
        get; set;
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(_connectionString, opts => opts.CommandTimeout((int)TimeSpan.FromMinutes(10).TotalSeconds));

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //modelBuilder.Entity<OrderItem>().HasKey(e => new { e.OrderID, e.OrderLine });
    }


}
