﻿namespace JobOffersWebsite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddApplyForJobClass : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ApplyForJobs",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Message = c.String(),
                        ApplyDate = c.DateTime(nullable: false),
                        JobId = c.Int(nullable: false),
                        UserId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Jobs", t => t.JobId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.JobId)
                .Index(t => t.UserId);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ApplyForJobs", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.ApplyForJobs", "JobId", "dbo.Jobs");
            DropIndex("dbo.ApplyForJobs", new[] { "UserId" });
            DropIndex("dbo.ApplyForJobs", new[] { "JobId" });
            DropTable("dbo.ApplyForJobs");
        }
    }
}
