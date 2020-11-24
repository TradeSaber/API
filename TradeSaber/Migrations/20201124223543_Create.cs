using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using NodaTime;
using TradeSaber.Models;
using TradeSaber.Models.Discord;

namespace TradeSaber.Migrations
{
    public partial class Create : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "mutations",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    active = table.Column<bool>(type: "boolean", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    global_xp_boost = table.Column<float>(type: "real", nullable: true),
                    global_tir_boost = table.Column<float>(type: "real", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_mutations", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "packs",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    cover_url = table.Column<string>(type: "text", nullable: false),
                    theme = table.Column<string>(type: "text", nullable: false),
                    value = table.Column<float>(type: "real", nullable: true),
                    count = table.Column<int>(type: "integer", nullable: false),
                    rarities = table.Column<IList<Rarity>>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_packs", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "series",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    main_color = table.Column<string>(type: "text", nullable: false),
                    sub_color = table.Column<string>(type: "text", nullable: false),
                    icon_url = table.Column<string>(type: "text", nullable: false),
                    banner_url = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_series", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    profile = table.Column<DiscordUser>(type: "jsonb", nullable: false),
                    state = table.Column<int>(type: "integer", nullable: false),
                    role = table.Column<int>(type: "integer", nullable: false),
                    level = table.Column<int>(type: "integer", nullable: false),
                    tir_coin = table.Column<float>(type: "real", nullable: false),
                    experience = table.Column<long>(type: "bigint", nullable: false),
                    created = table.Column<Instant>(type: "timestamp", nullable: false),
                    last_logged_in = table.Column<Instant>(type: "timestamp", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "cards",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    series_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    probability = table.Column<float>(type: "real", nullable: false),
                    rarity = table.Column<int>(type: "integer", nullable: false),
                    root = table.Column<string>(type: "text", nullable: true),
                    maximum = table.Column<int>(type: "integer", nullable: true),
                    locked = table.Column<bool>(type: "boolean", nullable: false),
                    cover_url = table.Column<string>(type: "text", nullable: false),
                    base_url = table.Column<string>(type: "text", nullable: false),
                    value = table.Column<float>(type: "real", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_cards", x => x.id);
                    table.ForeignKey(
                        name: "fk_cards_series_series_id",
                        column: x => x.series_id,
                        principalTable: "series",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "series_reference",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    series_id = table.Column<Guid>(type: "uuid", nullable: true),
                    boost = table.Column<float>(type: "real", nullable: true),
                    mutation_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_series_reference", x => x.id);
                    table.ForeignKey(
                        name: "fk_reference_mutations_mutation_id",
                        column: x => x.mutation_id,
                        principalTable: "mutations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_reference_series_series_id",
                        column: x => x.series_id,
                        principalTable: "series",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "pack_user",
                columns: table => new
                {
                    owned_by_id = table.Column<Guid>(type: "uuid", nullable: false),
                    packs_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_pack_user", x => new { x.owned_by_id, x.packs_id });
                    table.ForeignKey(
                        name: "fk_pack_user_packs_packs_id",
                        column: x => x.packs_id,
                        principalTable: "packs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_pack_user_users_owned_by_id",
                        column: x => x.owned_by_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "card_pack",
                columns: table => new
                {
                    cards_id = table.Column<Guid>(type: "uuid", nullable: false),
                    packs_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_card_pack", x => new { x.cards_id, x.packs_id });
                    table.ForeignKey(
                        name: "fk_card_pack_cards_cards_id",
                        column: x => x.cards_id,
                        principalTable: "cards",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_card_pack_packs_packs_id",
                        column: x => x.packs_id,
                        principalTable: "packs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "card_reference",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    card_id = table.Column<Guid>(type: "uuid", nullable: true),
                    boost = table.Column<float>(type: "real", nullable: true),
                    mutation_id = table.Column<Guid>(type: "uuid", nullable: true),
                    pack_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_card_reference", x => x.id);
                    table.ForeignKey(
                        name: "fk_reference_cards_card_id",
                        column: x => x.card_id,
                        principalTable: "cards",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_reference_mutations_mutation_id",
                        column: x => x.mutation_id,
                        principalTable: "mutations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_reference_packs_pack_id",
                        column: x => x.pack_id,
                        principalTable: "packs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "card_user",
                columns: table => new
                {
                    cards_id = table.Column<Guid>(type: "uuid", nullable: false),
                    owned_by_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_card_user", x => new { x.cards_id, x.owned_by_id });
                    table.ForeignKey(
                        name: "fk_card_user_cards_cards_id",
                        column: x => x.cards_id,
                        principalTable: "cards",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_card_user_users_owned_by_id",
                        column: x => x.owned_by_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_card_pack_packs_id",
                table: "card_pack",
                column: "packs_id");

            migrationBuilder.CreateIndex(
                name: "ix_reference_card_id",
                table: "card_reference",
                column: "card_id");

            migrationBuilder.CreateIndex(
                name: "ix_reference_mutation_id",
                table: "card_reference",
                column: "mutation_id");

            migrationBuilder.CreateIndex(
                name: "ix_reference_pack_id",
                table: "card_reference",
                column: "pack_id");

            migrationBuilder.CreateIndex(
                name: "ix_card_user_owned_by_id",
                table: "card_user",
                column: "owned_by_id");

            migrationBuilder.CreateIndex(
                name: "ix_cards_series_id",
                table: "cards",
                column: "series_id");

            migrationBuilder.CreateIndex(
                name: "ix_pack_user_packs_id",
                table: "pack_user",
                column: "packs_id");

            migrationBuilder.CreateIndex(
                name: "ix_reference_mutation_id1",
                table: "series_reference",
                column: "mutation_id");

            migrationBuilder.CreateIndex(
                name: "ix_reference_series_id",
                table: "series_reference",
                column: "series_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "card_pack");

            migrationBuilder.DropTable(
                name: "card_reference");

            migrationBuilder.DropTable(
                name: "card_user");

            migrationBuilder.DropTable(
                name: "pack_user");

            migrationBuilder.DropTable(
                name: "series_reference");

            migrationBuilder.DropTable(
                name: "cards");

            migrationBuilder.DropTable(
                name: "packs");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "mutations");

            migrationBuilder.DropTable(
                name: "series");
        }
    }
}
