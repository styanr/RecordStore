using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace RecordStore.Api.Entities.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:moddatetime", ",,");

            migrationBuilder.CreateTable(
                name: "app_user",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    password = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    first_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    last_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("user_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "artist",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    image_url = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("artist_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "format",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    format_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("format_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "genre",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("genre_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "order_status",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    name = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("order_status_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "record",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    title = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    image_url = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    release_date = table.Column<DateOnly>(type: "date", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("record_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "region",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    region_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("region_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "track",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    title = table.Column<string>(type: "character varying", nullable: false),
                    duration_seconds = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("track_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "shopping_cart",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    user_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("shopping_cart_pkey", x => x.id);
                    table.ForeignKey(
                        name: "shopping_cart_user_id_fkey",
                        column: x => x.user_id,
                        principalTable: "app_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "genre_artist",
                columns: table => new
                {
                    genre_id = table.Column<int>(type: "integer", nullable: false),
                    artist_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("genre_artist_pkey", x => new { x.genre_id, x.artist_id });
                    table.ForeignKey(
                        name: "genre_artist_artist_id_fkey",
                        column: x => x.artist_id,
                        principalTable: "artist",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "genre_artist_genre_id_fkey",
                        column: x => x.genre_id,
                        principalTable: "genre",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "shop_order",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    total = table.Column<decimal>(type: "money", nullable: false),
                    city = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    street = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    building = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    apartment = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    status_id = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("shop_order_pkey", x => x.id);
                    table.ForeignKey(
                        name: "shop_order_status_id_fkey",
                        column: x => x.status_id,
                        principalTable: "order_status",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "shop_order_user_id_fkey",
                        column: x => x.user_id,
                        principalTable: "app_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "artist_record",
                columns: table => new
                {
                    artist_id = table.Column<int>(type: "integer", nullable: false),
                    record_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("artist_record_pkey", x => new { x.artist_id, x.record_id });
                    table.ForeignKey(
                        name: "artist_record_artist_id_fkey",
                        column: x => x.artist_id,
                        principalTable: "artist",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "artist_record_record_id_fkey",
                        column: x => x.record_id,
                        principalTable: "record",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "genre_record",
                columns: table => new
                {
                    genre_id = table.Column<int>(type: "integer", nullable: false),
                    record_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("genre_record_pkey", x => new { x.genre_id, x.record_id });
                    table.ForeignKey(
                        name: "genre_record_genre_id_fkey",
                        column: x => x.genre_id,
                        principalTable: "genre",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "genre_record_record_id_fkey",
                        column: x => x.record_id,
                        principalTable: "record",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "product",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    record_id = table.Column<int>(type: "integer", nullable: false),
                    format_id = table.Column<int>(type: "integer", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    quantity = table.Column<int>(type: "integer", nullable: false),
                    price = table.Column<decimal>(type: "money", nullable: false),
                    inactive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("product_pkey", x => x.id);
                    table.ForeignKey(
                        name: "product_format_id_fkey",
                        column: x => x.format_id,
                        principalTable: "format",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "product_record_id_fkey",
                        column: x => x.record_id,
                        principalTable: "record",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "address",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    city = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    street = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    building = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    apartment = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    region_id = table.Column<int>(type: "integer", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("address_pkey", x => x.id);
                    table.ForeignKey(
                        name: "address_region_id_fkey",
                        column: x => x.region_id,
                        principalTable: "region",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "discount",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    product_id = table.Column<int>(type: "integer", nullable: false),
                    start_date = table.Column<DateOnly>(type: "date", nullable: false),
                    end_date = table.Column<DateOnly>(type: "date", nullable: false),
                    discount_percent = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("discount_pkey", x => x.id);
                    table.ForeignKey(
                        name: "discount_product_id_fkey",
                        column: x => x.product_id,
                        principalTable: "product",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "order_line",
                columns: table => new
                {
                    shop_order_id = table.Column<int>(type: "integer", nullable: false),
                    product_id = table.Column<int>(type: "integer", nullable: false),
                    quantity = table.Column<int>(type: "integer", nullable: false),
                    price = table.Column<decimal>(type: "money", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("order_line_pkey", x => new { x.shop_order_id, x.product_id });
                    table.ForeignKey(
                        name: "order_line_product_id_fkey",
                        column: x => x.product_id,
                        principalTable: "product",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "order_line_shop_order_id_fkey",
                        column: x => x.shop_order_id,
                        principalTable: "shop_order",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "review",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    product_id = table.Column<int>(type: "integer", nullable: false),
                    rating = table.Column<int>(type: "integer", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("review_pkey", x => x.id);
                    table.ForeignKey(
                        name: "review_product_id_fkey",
                        column: x => x.product_id,
                        principalTable: "product",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "review_user_id_fkey",
                        column: x => x.user_id,
                        principalTable: "app_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "shopping_cart_product",
                columns: table => new
                {
                    shopping_cart_id = table.Column<int>(type: "integer", nullable: false),
                    product_id = table.Column<int>(type: "integer", nullable: false),
                    quantity = table.Column<int>(type: "integer", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("shopping_cart_product_pkey", x => new { x.shopping_cart_id, x.product_id });
                    table.ForeignKey(
                        name: "shopping_cart_product_product_id_fkey",
                        column: x => x.product_id,
                        principalTable: "product",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "shopping_cart_product_shopping_cart_id_fkey",
                        column: x => x.shopping_cart_id,
                        principalTable: "shopping_cart",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "track_product",
                columns: table => new
                {
                    track_id = table.Column<int>(type: "integer", nullable: false),
                    product_id = table.Column<int>(type: "integer", nullable: false),
                    track_order = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("track_product_pkey", x => new { x.track_id, x.product_id });
                    table.ForeignKey(
                        name: "track_product_product_id_fkey",
                        column: x => x.product_id,
                        principalTable: "product",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "track_product_track_id_fkey",
                        column: x => x.track_id,
                        principalTable: "track",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_address",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    address_id = table.Column<int>(type: "integer", nullable: false),
                    is_default = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("user_address_pkey", x => new { x.user_id, x.address_id });
                    table.ForeignKey(
                        name: "user_address_address_id_fkey",
                        column: x => x.address_id,
                        principalTable: "address",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "user_address_user_id_fkey",
                        column: x => x.user_id,
                        principalTable: "app_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_address_region_id",
                table: "address",
                column: "region_id");

            migrationBuilder.CreateIndex(
                name: "app_user_email_key",
                table: "app_user",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_artist_record_record_id",
                table: "artist_record",
                column: "record_id");

            migrationBuilder.CreateIndex(
                name: "IX_discount_product_id",
                table: "discount",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "format_format_name_key",
                table: "format",
                column: "format_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "genre_name_key",
                table: "genre",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_genre_artist_artist_id",
                table: "genre_artist",
                column: "artist_id");

            migrationBuilder.CreateIndex(
                name: "IX_genre_record_record_id",
                table: "genre_record",
                column: "record_id");

            migrationBuilder.CreateIndex(
                name: "IX_order_line_product_id",
                table: "order_line",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "IX_product_format_id",
                table: "product",
                column: "format_id");

            migrationBuilder.CreateIndex(
                name: "IX_product_record_id",
                table: "product",
                column: "record_id");

            migrationBuilder.CreateIndex(
                name: "region_region_name_key",
                table: "region",
                column: "region_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_review_product_id",
                table: "review",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "IX_review_user_id",
                table: "review",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_shop_order_status_id",
                table: "shop_order",
                column: "status_id");

            migrationBuilder.CreateIndex(
                name: "IX_shop_order_user_id",
                table: "shop_order",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_shopping_cart_user_id",
                table: "shopping_cart",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_shopping_cart_product_product_id",
                table: "shopping_cart_product",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "IX_track_product_product_id",
                table: "track_product",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_address_address_id",
                table: "user_address",
                column: "address_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "artist_record");

            migrationBuilder.DropTable(
                name: "discount");

            migrationBuilder.DropTable(
                name: "genre_artist");

            migrationBuilder.DropTable(
                name: "genre_record");

            migrationBuilder.DropTable(
                name: "order_line");

            migrationBuilder.DropTable(
                name: "review");

            migrationBuilder.DropTable(
                name: "shopping_cart_product");

            migrationBuilder.DropTable(
                name: "track_product");

            migrationBuilder.DropTable(
                name: "user_address");

            migrationBuilder.DropTable(
                name: "artist");

            migrationBuilder.DropTable(
                name: "genre");

            migrationBuilder.DropTable(
                name: "shop_order");

            migrationBuilder.DropTable(
                name: "shopping_cart");

            migrationBuilder.DropTable(
                name: "product");

            migrationBuilder.DropTable(
                name: "track");

            migrationBuilder.DropTable(
                name: "address");

            migrationBuilder.DropTable(
                name: "order_status");

            migrationBuilder.DropTable(
                name: "app_user");

            migrationBuilder.DropTable(
                name: "format");

            migrationBuilder.DropTable(
                name: "record");

            migrationBuilder.DropTable(
                name: "region");
        }
    }
}
