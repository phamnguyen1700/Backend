using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BE_V2.Migrations
{
    /// <inheritdoc />
    public partial class create : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Diamond",
                columns: table => new
                {
                    DiamondID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Shape = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Cut = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Color = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Clarity = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CaratWeight = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    Fluorescence = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    LengthWidthRatio = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    Depth = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    Tables = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    Symmetry = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Girdle = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Measurements = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Certificate = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Diamond__23A8E7BBE47D50DC", x => x.DiamondID);
                });

            migrationBuilder.CreateTable(
                name: "DiamondPriceTable",
                columns: table => new
                {
                    Carat = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Color = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Clarity = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Cut = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiamondPriceTable", x => new { x.Carat, x.Color, x.Clarity, x.Cut });
                });

            migrationBuilder.CreateTable(
                name: "Event",
                columns: table => new
                {
                    EventID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EventName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", maxLength: 2147483647, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Event__1E20497A", x => x.EventID);
                });

            migrationBuilder.CreateTable(
                name: "NecklaceMold",
                columns: table => new
                {
                    NecklaceMoldId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Material = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Size = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CaratWeight = table.Column<decimal>(type: "decimal(3,2)", nullable: false),
                    BasePrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NecklaceMold", x => x.NecklaceMoldId);
                });

            migrationBuilder.CreateTable(
                name: "NecklacePriceTable",
                columns: table => new
                {
                    Material = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Length = table.Column<int>(type: "int", nullable: false),
                    CaratWeight = table.Column<decimal>(type: "decimal(3,2)", nullable: false),
                    BasePrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NecklacePriceTable", x => new { x.Material, x.Length, x.CaratWeight });
                });

            migrationBuilder.CreateTable(
                name: "ProductTypes",
                columns: table => new
                {
                    ProductTypeID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductTypeName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ProductT__A1312F4E6F0C4623", x => x.ProductTypeID);
                });

            migrationBuilder.CreateTable(
                name: "RingMold",
                columns: table => new
                {
                    RingMoldId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Material = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Size = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CaratWeight = table.Column<decimal>(type: "decimal(4,2)", nullable: false),
                    Gender = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    RingType = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    BasePrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RingMold", x => x.RingMoldId);
                });

            migrationBuilder.CreateTable(
                name: "RingPriceTable",
                columns: table => new
                {
                    Material = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Size = table.Column<decimal>(type: "decimal(3,1)", nullable: false),
                    CaratWeight = table.Column<decimal>(type: "decimal(4,2)", nullable: false),
                    BasePrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RingPriceTable", x => new { x.Material, x.Size, x.CaratWeight });
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    RoleID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Roles__8AFACE3A3AEE977B", x => x.RoleID);
                });

            migrationBuilder.CreateTable(
                name: "Product",
                columns: table => new
                {
                    ProductID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ProductType = table.Column<int>(type: "int", nullable: true),
                    Material = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Size = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ProcessingPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ExchangeRate = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Quantity = table.Column<int>(type: "int", nullable: true),
                    MainDiamondId = table.Column<int>(type: "int", nullable: true),
                    Image1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Image2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Image3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecondaryDiamondId = table.Column<int>(type: "int", nullable: true),
                    RingMoldId = table.Column<int>(type: "int", nullable: true),
                    NecklaceMoldId = table.Column<int>(type: "int", nullable: true),
                    SecondaryDiamondCount = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Product", x => x.ProductID);
                    table.ForeignKey(
                        name: "FK_Product_MainDiamond",
                        column: x => x.MainDiamondId,
                        principalTable: "Diamond",
                        principalColumn: "DiamondID");
                    table.ForeignKey(
                        name: "FK_Product_NecklaceMold",
                        column: x => x.NecklaceMoldId,
                        principalTable: "NecklaceMold",
                        principalColumn: "NecklaceMoldId");
                    table.ForeignKey(
                        name: "FK_Product_ProductType",
                        column: x => x.ProductType,
                        principalTable: "ProductTypes",
                        principalColumn: "ProductTypeID");
                    table.ForeignKey(
                        name: "FK_Product_RingMold",
                        column: x => x.RingMoldId,
                        principalTable: "RingMold",
                        principalColumn: "RingMoldId");
                    table.ForeignKey(
                        name: "FK_Product_SecondaryDiamond",
                        column: x => x.SecondaryDiamondId,
                        principalTable: "Diamond",
                        principalColumn: "DiamondID");
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleID = table.Column<int>(type: "int", nullable: true),
                    Username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Sex = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Users__1788CCAC77FF890C", x => x.UserID);
                    table.ForeignKey(
                        name: "FK__Users__RoleID__4BAC3F29",
                        column: x => x.RoleID,
                        principalTable: "Roles",
                        principalColumn: "RoleID");
                });

            migrationBuilder.CreateTable(
                name: "EventItem",
                columns: table => new
                {
                    EventItemID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EventID = table.Column<int>(type: "int", nullable: false),
                    ProductID = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Discount = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__EventItem__1BFD2C07", x => x.EventItemID);
                    table.ForeignKey(
                        name: "FK__EventItem__EventID__02FC7413",
                        column: x => x.EventID,
                        principalTable: "Event",
                        principalColumn: "EventID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK__EventItem__ProductID__02FC7413",
                        column: x => x.ProductID,
                        principalTable: "Product",
                        principalColumn: "ProductID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PriceDetail",
                columns: table => new
                {
                    PriceDetailID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductID = table.Column<int>(type: "int", nullable: false),
                    DiamondPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    JewelryPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ProcessingPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Profit = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__PriceDetail__1A14E395", x => x.PriceDetailID);
                    table.ForeignKey(
                        name: "FK__PriceDeta__ProductID__06CD04F7",
                        column: x => x.ProductID,
                        principalTable: "Product",
                        principalColumn: "ProductID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Cart",
                columns: table => new
                {
                    CartID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Cart__2F36C7C22AAEDC89", x => x.CartID);
                    table.ForeignKey(
                        name: "FK__Cart__UserID__2A4B4B5E",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Customer",
                columns: table => new
                {
                    CustomerID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<int>(type: "int", nullable: true),
                    DateJoined = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Customer__A4AE64B8E2F39143", x => x.CustomerID);
                    table.ForeignKey(
                        name: "FK__Customer__UserID__571DF1D5",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "UserID");
                });

            migrationBuilder.CreateTable(
                name: "CartItem",
                columns: table => new
                {
                    CartItemID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CartID = table.Column<int>(type: "int", nullable: false),
                    ProductID = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Size = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__CartItem__3A4CA8E7AAB53760", x => x.CartItemID);
                    table.ForeignKey(
                        name: "FK__CartItem__CartID__2B3F6F97",
                        column: x => x.CartID,
                        principalTable: "Cart",
                        principalColumn: "CartID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK__CartItem__Product__2C3393D0",
                        column: x => x.ProductID,
                        principalTable: "Product",
                        principalColumn: "ProductID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CustomerPoints",
                columns: table => new
                {
                    CustomerPointID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerID = table.Column<int>(type: "int", nullable: false),
                    Points = table.Column<int>(type: "int", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__CustomerPoints", x => x.CustomerPointID);
                    table.ForeignKey(
                        name: "FK__CustomerPoints__CustomerID",
                        column: x => x.CustomerID,
                        principalTable: "Customer",
                        principalColumn: "CustomerID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Feedback",
                columns: table => new
                {
                    FeedbackID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerID = table.Column<int>(type: "int", nullable: true),
                    FeedbackText = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Rating = table.Column<int>(type: "int", nullable: true),
                    FeedbackDate = table.Column<DateOnly>(type: "date", nullable: true),
                    ProductID = table.Column<int>(type: "int", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Feedback__6A4BEDF6BA84AC5C", x => x.FeedbackID);
                    table.ForeignKey(
                        name: "FK_Feedbacks_Product",
                        column: x => x.ProductID,
                        principalTable: "Product",
                        principalColumn: "ProductID");
                    table.ForeignKey(
                        name: "FK__Feedback__Custom__6477ECF3",
                        column: x => x.CustomerID,
                        principalTable: "Customer",
                        principalColumn: "CustomerID");
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    OrderID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerID = table.Column<int>(type: "int", nullable: true),
                    TotalPrice = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    OrderDate = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Orders__C3905BAF84671313", x => x.OrderID);
                    table.ForeignKey(
                        name: "FK__Orders__Customer__59FA5E80",
                        column: x => x.CustomerID,
                        principalTable: "Customer",
                        principalColumn: "CustomerID");
                });

            migrationBuilder.CreateTable(
                name: "Wishlist",
                columns: table => new
                {
                    WishlistID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerID = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Wishlist__233189CB03175B66", x => x.WishlistID);
                    table.ForeignKey(
                        name: "FK__Wishlist__Custom__02FC7413",
                        column: x => x.CustomerID,
                        principalTable: "Customer",
                        principalColumn: "CustomerID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderDetails",
                columns: table => new
                {
                    OrderDetailID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderID = table.Column<int>(type: "int", nullable: true),
                    ProductID = table.Column<int>(type: "int", nullable: true),
                    ProductName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ProductPrice = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    Quantity = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__OrderDet__D3B9D30CF13D40EE", x => x.OrderDetailID);
                    table.ForeignKey(
                        name: "FK__OrderDeta__Order__5CD6CB2B",
                        column: x => x.OrderID,
                        principalTable: "Orders",
                        principalColumn: "OrderID");
                    table.ForeignKey(
                        name: "FK__OrderDeta__Produ__5DCAEF64",
                        column: x => x.ProductID,
                        principalTable: "Product",
                        principalColumn: "ProductID");
                });

            migrationBuilder.CreateTable(
                name: "OrderLogs",
                columns: table => new
                {
                    LogID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderID = table.Column<int>(type: "int", nullable: false),
                    Phase1 = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    Phase2 = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    Phase3 = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    Phase4 = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    TimePhase1 = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TimePhase2 = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TimePhase3 = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TimePhase4 = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__OrderLog__A5D58A608123E0B0", x => x.LogID);
                    table.ForeignKey(
                        name: "FK__OrderLogs__OrderID__02FC7413",
                        column: x => x.OrderID,
                        principalTable: "Orders",
                        principalColumn: "OrderID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Payment",
                columns: table => new
                {
                    PaymentID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderID = table.Column<int>(type: "int", nullable: true),
                    Deposit = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    AmountPaid = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    Total = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    DatePaid = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Payment__9B556A58C7262DC1", x => x.PaymentID);
                    table.ForeignKey(
                        name: "FK__Payment__OrderID__60A75C0F",
                        column: x => x.OrderID,
                        principalTable: "Orders",
                        principalColumn: "OrderID");
                });

            migrationBuilder.CreateTable(
                name: "WishlistItems",
                columns: table => new
                {
                    WishlistItemID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WishlistID = table.Column<int>(type: "int", nullable: false),
                    ProductID = table.Column<int>(type: "int", nullable: false),
                    AddedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Wishlist__171E21813C0A4C5A", x => x.WishlistItemID);
                    table.ForeignKey(
                        name: "FK__WishlistI__Produ__06CD04F7",
                        column: x => x.ProductID,
                        principalTable: "Product",
                        principalColumn: "ProductID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK__WishlistI__Wishl__05D8E0BE",
                        column: x => x.WishlistID,
                        principalTable: "Wishlist",
                        principalColumn: "WishlistID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cart_UserID",
                table: "Cart",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_CartItem_CartID",
                table: "CartItem",
                column: "CartID");

            migrationBuilder.CreateIndex(
                name: "IX_CartItem_ProductID",
                table: "CartItem",
                column: "ProductID");

            migrationBuilder.CreateIndex(
                name: "UQ__Customer__1788CCAD7E4B64B6",
                table: "Customer",
                column: "UserID",
                unique: true,
                filter: "[UserID] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerPoints_CustomerID",
                table: "CustomerPoints",
                column: "CustomerID");

            migrationBuilder.CreateIndex(
                name: "IX_EventItem_EventID",
                table: "EventItem",
                column: "EventID");

            migrationBuilder.CreateIndex(
                name: "IX_EventItem_ProductID",
                table: "EventItem",
                column: "ProductID");

            migrationBuilder.CreateIndex(
                name: "IX_Feedback_CustomerID",
                table: "Feedback",
                column: "CustomerID");

            migrationBuilder.CreateIndex(
                name: "IX_Feedback_ProductID",
                table: "Feedback",
                column: "ProductID");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_OrderID",
                table: "OrderDetails",
                column: "OrderID");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_ProductID",
                table: "OrderDetails",
                column: "ProductID");

            migrationBuilder.CreateIndex(
                name: "IX_OrderLogs_OrderID",
                table: "OrderLogs",
                column: "OrderID");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CustomerID",
                table: "Orders",
                column: "CustomerID");

            migrationBuilder.CreateIndex(
                name: "IX_Payment_OrderID",
                table: "Payment",
                column: "OrderID");

            migrationBuilder.CreateIndex(
                name: "IX_PriceDetail_ProductID",
                table: "PriceDetail",
                column: "ProductID");

            migrationBuilder.CreateIndex(
                name: "IX_Product_MainDiamondId",
                table: "Product",
                column: "MainDiamondId");

            migrationBuilder.CreateIndex(
                name: "IX_Product_NecklaceMoldId",
                table: "Product",
                column: "NecklaceMoldId");

            migrationBuilder.CreateIndex(
                name: "IX_Product_ProductType",
                table: "Product",
                column: "ProductType");

            migrationBuilder.CreateIndex(
                name: "IX_Product_RingMoldId",
                table: "Product",
                column: "RingMoldId");

            migrationBuilder.CreateIndex(
                name: "IX_Product_SecondaryDiamondId",
                table: "Product",
                column: "SecondaryDiamondId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_RoleID",
                table: "Users",
                column: "RoleID");

            migrationBuilder.CreateIndex(
                name: "IX_Wishlist_CustomerID",
                table: "Wishlist",
                column: "CustomerID");

            migrationBuilder.CreateIndex(
                name: "IX_WishlistItems_ProductID",
                table: "WishlistItems",
                column: "ProductID");

            migrationBuilder.CreateIndex(
                name: "IX_WishlistItems_WishlistID",
                table: "WishlistItems",
                column: "WishlistID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CartItem");

            migrationBuilder.DropTable(
                name: "CustomerPoints");

            migrationBuilder.DropTable(
                name: "DiamondPriceTable");

            migrationBuilder.DropTable(
                name: "EventItem");

            migrationBuilder.DropTable(
                name: "Feedback");

            migrationBuilder.DropTable(
                name: "NecklacePriceTable");

            migrationBuilder.DropTable(
                name: "OrderDetails");

            migrationBuilder.DropTable(
                name: "OrderLogs");

            migrationBuilder.DropTable(
                name: "Payment");

            migrationBuilder.DropTable(
                name: "PriceDetail");

            migrationBuilder.DropTable(
                name: "RingPriceTable");

            migrationBuilder.DropTable(
                name: "WishlistItems");

            migrationBuilder.DropTable(
                name: "Cart");

            migrationBuilder.DropTable(
                name: "Event");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Product");

            migrationBuilder.DropTable(
                name: "Wishlist");

            migrationBuilder.DropTable(
                name: "Diamond");

            migrationBuilder.DropTable(
                name: "NecklaceMold");

            migrationBuilder.DropTable(
                name: "ProductTypes");

            migrationBuilder.DropTable(
                name: "RingMold");

            migrationBuilder.DropTable(
                name: "Customer");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Roles");
        }
    }
}
