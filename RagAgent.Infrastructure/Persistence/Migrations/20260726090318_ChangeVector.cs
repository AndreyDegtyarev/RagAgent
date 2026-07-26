using Microsoft.EntityFrameworkCore.Migrations;
using Pgvector;

#nullable disable

namespace RagAgent.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ChangeVector : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Vector>(
                name: "Embedding",
                table: "ChunkEmbedding",
                type: "vector",
                nullable: false,
                oldClrType: typeof(Vector),
                oldType: "vector(768)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Vector>(
                name: "Embedding",
                table: "ChunkEmbedding",
                type: "vector(768)",
                nullable: false,
                oldClrType: typeof(Vector),
                oldType: "vector");
        }
    }
}
