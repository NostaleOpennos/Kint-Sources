namespace OpenNos.DAL.EF.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class Aphrodite71 : DbMigration
    {
        #region Methods

        public override void Down() => DropColumn("dbo.RollGeneratedItem", "ItemGeneratedUpgrade");

        public override void Up()
        {
            AddColumn("dbo.RollGeneratedItem", "ItemGeneratedUpgrade", c => c.Byte(nullable: false));
        }

        #endregion
    }
}