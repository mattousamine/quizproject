using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace quiz.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "category",
                columns: table => new
                {
                    category_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    category_name = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    category_is_active = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_category", x => x.category_id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "option",
                columns: table => new
                {
                    option_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    option_text = table.Column<string>(type: "text", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    option_is_correct = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_option", x => x.option_id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "question",
                columns: table => new
                {
                    question_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    question_text = table.Column<string>(type: "text", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    question_feedback = table.Column<string>(type: "text", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    question_level = table.Column<short>(type: "smallint", nullable: false),
                    question_is_active = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_question", x => x.question_id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "quiz",
                columns: table => new
                {
                    quiz_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    quiz_name = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    quiz_is_active = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_quiz", x => x.quiz_id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "quiz_issue",
                columns: table => new
                {
                    quiz_issue_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    quiz_issue_description = table.Column<string>(type: "text", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    quiz_issue_date_reported = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    quiz_issue_is_fixed = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_quiz_issue", x => x.quiz_issue_id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "quiz_session",
                columns: table => new
                {
                    quiz_session_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    quiz_session_score = table.Column<int>(type: "int", nullable: false),
                    quiz_session_time = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_quiz_session", x => x.quiz_session_id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "user_quiz",
                columns: table => new
                {
                    user_quiz_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    user_quiz_username = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    user_quiz_password = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    user_quiz_email = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    user_quiz_is_admin = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    user_quiz_is_active = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_quiz", x => x.user_quiz_id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "question_option",
                columns: table => new
                {
                    question_option_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    question_id = table.Column<int>(type: "int", nullable: false),
                    option_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_question_option", x => x.question_option_id);
                    table.ForeignKey(
                        name: "FK_question_option_option_option_id",
                        column: x => x.option_id,
                        principalTable: "option",
                        principalColumn: "option_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_question_option_question_question_id",
                        column: x => x.question_id,
                        principalTable: "question",
                        principalColumn: "question_id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "category_quiz",
                columns: table => new
                {
                    category_quiz_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    category_id = table.Column<int>(type: "int", nullable: false),
                    quiz_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_category_quiz", x => x.category_quiz_id);
                    table.ForeignKey(
                        name: "FK_category_quiz_category_category_id",
                        column: x => x.category_id,
                        principalTable: "category",
                        principalColumn: "category_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_category_quiz_quiz_quiz_id",
                        column: x => x.quiz_id,
                        principalTable: "quiz",
                        principalColumn: "quiz_id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "QuizQuestions",
                columns: table => new
                {
                    quiz_question_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    quiz_id = table.Column<int>(type: "int", nullable: false),
                    question_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuizQuestions", x => x.quiz_question_id);
                    table.ForeignKey(
                        name: "FK_QuizQuestions_question_question_id",
                        column: x => x.question_id,
                        principalTable: "question",
                        principalColumn: "question_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuizQuestions_quiz_quiz_id",
                        column: x => x.quiz_id,
                        principalTable: "quiz",
                        principalColumn: "quiz_id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "quiz_tracker",
                columns: table => new
                {
                    quiz_tracker_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    user_quiz_id = table.Column<int>(type: "int", nullable: false),
                    quiz_session_id = table.Column<int>(type: "int", nullable: false),
                    quiz_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_quiz_tracker", x => x.quiz_tracker_id);
                    table.ForeignKey(
                        name: "FK_quiz_tracker_quiz_quiz_id",
                        column: x => x.quiz_id,
                        principalTable: "quiz",
                        principalColumn: "quiz_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_quiz_tracker_quiz_session_quiz_session_id",
                        column: x => x.quiz_session_id,
                        principalTable: "quiz_session",
                        principalColumn: "quiz_session_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_quiz_tracker_user_quiz_user_quiz_id",
                        column: x => x.user_quiz_id,
                        principalTable: "user_quiz",
                        principalColumn: "user_quiz_id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "user_issue",
                columns: table => new
                {
                    user_issue_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    quiz_issue_id = table.Column<int>(type: "int", nullable: false),
                    user_quiz_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_issue", x => x.user_issue_id);
                    table.ForeignKey(
                        name: "FK_user_issue_quiz_issue_quiz_issue_id",
                        column: x => x.quiz_issue_id,
                        principalTable: "quiz_issue",
                        principalColumn: "quiz_issue_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_user_issue_user_quiz_user_quiz_id",
                        column: x => x.user_quiz_id,
                        principalTable: "user_quiz",
                        principalColumn: "user_quiz_id",
                        onDelete: ReferentialAction.SetNull);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_category_quiz_category_id",
                table: "category_quiz",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "IX_category_quiz_quiz_id",
                table: "category_quiz",
                column: "quiz_id");

            migrationBuilder.CreateIndex(
                name: "IX_question_option_option_id",
                table: "question_option",
                column: "option_id");

            migrationBuilder.CreateIndex(
                name: "IX_question_option_question_id",
                table: "question_option",
                column: "question_id");

            migrationBuilder.CreateIndex(
                name: "IX_quiz_tracker_quiz_id",
                table: "quiz_tracker",
                column: "quiz_id");

            migrationBuilder.CreateIndex(
                name: "IX_quiz_tracker_quiz_session_id",
                table: "quiz_tracker",
                column: "quiz_session_id");

            migrationBuilder.CreateIndex(
                name: "IX_quiz_tracker_user_quiz_id",
                table: "quiz_tracker",
                column: "user_quiz_id");

            migrationBuilder.CreateIndex(
                name: "IX_QuizQuestions_question_id",
                table: "QuizQuestions",
                column: "question_id");

            migrationBuilder.CreateIndex(
                name: "IX_QuizQuestions_quiz_id",
                table: "QuizQuestions",
                column: "quiz_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_issue_quiz_issue_id",
                table: "user_issue",
                column: "quiz_issue_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_issue_user_quiz_id",
                table: "user_issue",
                column: "user_quiz_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "category_quiz");

            migrationBuilder.DropTable(
                name: "question_option");

            migrationBuilder.DropTable(
                name: "quiz_tracker");

            migrationBuilder.DropTable(
                name: "QuizQuestions");

            migrationBuilder.DropTable(
                name: "user_issue");

            migrationBuilder.DropTable(
                name: "category");

            migrationBuilder.DropTable(
                name: "option");

            migrationBuilder.DropTable(
                name: "quiz_session");

            migrationBuilder.DropTable(
                name: "question");

            migrationBuilder.DropTable(
                name: "quiz");

            migrationBuilder.DropTable(
                name: "quiz_issue");

            migrationBuilder.DropTable(
                name: "user_quiz");
        }
    }
}
