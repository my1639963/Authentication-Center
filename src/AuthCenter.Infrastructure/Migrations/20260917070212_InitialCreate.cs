using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace AuthCenter.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "oauth_application",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(255)", nullable: false),
                    ApplicationType = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    ClientId = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    ClientSecret = table.Column<string>(type: "longtext", nullable: true),
                    ClientType = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    ConcurrencyToken = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    ConsentType = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    DisplayName = table.Column<string>(type: "longtext", nullable: true),
                    DisplayNames = table.Column<string>(type: "longtext", nullable: true),
                    JsonWebKeySet = table.Column<string>(type: "longtext", nullable: true),
                    Permissions = table.Column<string>(type: "longtext", nullable: true),
                    PostLogoutRedirectUris = table.Column<string>(type: "longtext", nullable: true),
                    Properties = table.Column<string>(type: "longtext", nullable: true),
                    RedirectUris = table.Column<string>(type: "longtext", nullable: true),
                    Requirements = table.Column<string>(type: "longtext", nullable: true),
                    Settings = table.Column<string>(type: "longtext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_oauth_application", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "oauth_scope",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(255)", nullable: false),
                    ConcurrencyToken = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    Description = table.Column<string>(type: "longtext", nullable: true),
                    Descriptions = table.Column<string>(type: "longtext", nullable: true),
                    DisplayName = table.Column<string>(type: "longtext", nullable: true),
                    DisplayNames = table.Column<string>(type: "longtext", nullable: true),
                    Name = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true),
                    Properties = table.Column<string>(type: "longtext", nullable: true),
                    Resources = table.Column<string>(type: "longtext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_oauth_scope", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "sys_admin_permission",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    permission_code = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: false),
                    permission_name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true),
                    create_time = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    update_time = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sys_admin_permission", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "sys_admin_role",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    role_code = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false),
                    role_name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true),
                    status = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    create_time = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    update_time = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    is_deleted = table.Column<int>(type: "int", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sys_admin_role", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "sys_admin_role_permission",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    role_id = table.Column<long>(type: "bigint", nullable: false),
                    permission_id = table.Column<long>(type: "bigint", nullable: false),
                    create_time = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sys_admin_role_permission", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "sys_admin_user_role",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    role_id = table.Column<long>(type: "bigint", nullable: false),
                    create_time = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sys_admin_user_role", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "sys_audit_log",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    event_type = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false),
                    operation_type = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false),
                    operate_user_id = table.Column<long>(type: "bigint", nullable: true),
                    target_user_id = table.Column<long>(type: "bigint", nullable: true),
                    client_id = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true),
                    ip_address = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: true),
                    user_agent = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true),
                    event_result = table.Column<int>(type: "int", nullable: false),
                    fail_reason = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true),
                    content = table.Column<string>(type: "text", nullable: true),
                    request_id = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: true),
                    previous_hash = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: true),
                    log_hash = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: false),
                    create_time = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sys_audit_log", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "sys_department",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    unit_id = table.Column<long>(type: "bigint", nullable: false),
                    parent_id = table.Column<long>(type: "bigint", nullable: true),
                    dept_code = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false),
                    dept_name = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    leader_user_id = table.Column<long>(type: "bigint", nullable: true),
                    status = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    sort = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    description = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true),
                    create_time = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    update_time = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    is_deleted = table.Column<int>(type: "int", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sys_department", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "sys_login_log",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    user_id = table.Column<long>(type: "bigint", nullable: true),
                    login_name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    client_id = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true),
                    login_type = table.Column<string>(type: "varchar(32)", maxLength: 32, nullable: false),
                    login_result = table.Column<int>(type: "int", nullable: false),
                    fail_reason = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true),
                    ip_address = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: true),
                    user_agent = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true),
                    request_id = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: true),
                    create_time = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sys_login_log", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "sys_position",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    position_code = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false),
                    position_name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    position_type = table.Column<string>(type: "varchar(32)", maxLength: 32, nullable: true),
                    position_level = table.Column<int>(type: "int", nullable: true),
                    status = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    sort = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    description = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true),
                    create_time = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    update_time = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    is_deleted = table.Column<int>(type: "int", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sys_position", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "sys_region",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    parent_id = table.Column<long>(type: "bigint", nullable: true),
                    region_code = table.Column<string>(type: "varchar(32)", maxLength: 32, nullable: false),
                    region_name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    region_level = table.Column<int>(type: "int", nullable: false),
                    status = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    sort = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    create_time = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    update_time = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    is_deleted = table.Column<int>(type: "int", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sys_region", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "sys_unit",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    parent_id = table.Column<long>(type: "bigint", nullable: true),
                    unit_code = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false),
                    unit_name = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    unit_type = table.Column<string>(type: "varchar(32)", maxLength: 32, nullable: true),
                    unit_level = table.Column<int>(type: "int", nullable: true),
                    region_id = table.Column<long>(type: "bigint", nullable: true),
                    status = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    sort = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    description = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true),
                    create_time = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    update_time = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    is_deleted = table.Column<int>(type: "int", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sys_unit", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "sys_user",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    login_name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    password_hash = table.Column<string>(type: "varchar(512)", maxLength: 512, nullable: false),
                    real_name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    mobile = table.Column<string>(type: "varchar(32)", maxLength: 32, nullable: true),
                    email = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true),
                    id_card_ciphertext = table.Column<string>(type: "varchar(1024)", maxLength: 1024, nullable: true),
                    id_card_hash = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: true),
                    status = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    login_fail_count = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    lock_until = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    must_modify_pwd = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    password_expire_time = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    security_stamp = table.Column<string>(type: "longtext", nullable: true),
                    token_version = table.Column<long>(type: "bigint", nullable: false, defaultValue: 1L),
                    last_login_time = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    last_login_ip = table.Column<string>(type: "longtext", nullable: true),
                    create_time = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    update_time = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    is_deleted = table.Column<int>(type: "int", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sys_user", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "sys_user_main_organization",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    user_organization_id = table.Column<long>(type: "bigint", nullable: false),
                    create_time = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    update_time = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sys_user_main_organization", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "sys_user_organization",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    unit_id = table.Column<long>(type: "bigint", nullable: false),
                    department_id = table.Column<long>(type: "bigint", nullable: true),
                    position_id = table.Column<long>(type: "bigint", nullable: true),
                    region_id = table.Column<long>(type: "bigint", nullable: true),
                    start_time = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    end_time = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    status = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    create_time = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    update_time = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    is_deleted = table.Column<int>(type: "int", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sys_user_organization", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "sys_user_password_history",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    password_hash = table.Column<string>(type: "varchar(512)", maxLength: 512, nullable: false),
                    create_time = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sys_user_password_history", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "oauth_authorization",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(255)", nullable: false),
                    ApplicationId = table.Column<string>(type: "varchar(255)", nullable: true),
                    ConcurrencyToken = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    CreationDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    Properties = table.Column<string>(type: "longtext", nullable: true),
                    Scopes = table.Column<string>(type: "longtext", nullable: true),
                    Status = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    Subject = table.Column<string>(type: "varchar(400)", maxLength: 400, nullable: true),
                    Type = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_oauth_authorization", x => x.Id);
                    table.ForeignKey(
                        name: "FK_oauth_authorization_oauth_application_ApplicationId",
                        column: x => x.ApplicationId,
                        principalTable: "oauth_application",
                        principalColumn: "Id");
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "oauth_token",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(255)", nullable: false),
                    ApplicationId = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: true),
                    AuthorizationId = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: true),
                    ConcurrencyToken = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    CreationDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ExpirationDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    Payload = table.Column<string>(type: "longtext", nullable: true),
                    Properties = table.Column<string>(type: "longtext", nullable: true),
                    RedemptionDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ReferenceId = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    Status = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: true),
                    Subject = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: true),
                    Type = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_oauth_token", x => x.Id);
                    table.ForeignKey(
                        name: "FK_oauth_token_oauth_application_ApplicationId",
                        column: x => x.ApplicationId,
                        principalTable: "oauth_application",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_oauth_token_oauth_authorization_AuthorizationId",
                        column: x => x.AuthorizationId,
                        principalTable: "oauth_authorization",
                        principalColumn: "Id");
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_oauth_application_ClientId",
                table: "oauth_application",
                column: "ClientId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_oauth_authorization_ApplicationId_Status_Subject_Type",
                table: "oauth_authorization",
                columns: new[] { "ApplicationId", "Status", "Subject", "Type" });

            migrationBuilder.CreateIndex(
                name: "IX_oauth_scope_Name",
                table: "oauth_scope",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_oauth_token_ApplicationId_Status_Subject_Type",
                table: "oauth_token",
                columns: new[] { "ApplicationId", "Status", "Subject", "Type" });

            migrationBuilder.CreateIndex(
                name: "IX_oauth_token_AuthorizationId",
                table: "oauth_token",
                column: "AuthorizationId");

            migrationBuilder.CreateIndex(
                name: "IX_oauth_token_ReferenceId",
                table: "oauth_token",
                column: "ReferenceId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "uk_sys_admin_permission_code",
                table: "sys_admin_permission",
                column: "permission_code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_sys_admin_role_status",
                table: "sys_admin_role",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "uk_sys_admin_role_code",
                table: "sys_admin_role",
                column: "role_code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_sys_admin_role_permission_permission",
                table: "sys_admin_role_permission",
                column: "permission_id");

            migrationBuilder.CreateIndex(
                name: "idx_sys_admin_role_permission_role",
                table: "sys_admin_role_permission",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "uk_sys_admin_role_permission",
                table: "sys_admin_role_permission",
                columns: new[] { "role_id", "permission_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_sys_admin_user_role_role",
                table: "sys_admin_user_role",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "idx_sys_admin_user_role_user",
                table: "sys_admin_user_role",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "uk_sys_admin_user_role",
                table: "sys_admin_user_role",
                columns: new[] { "user_id", "role_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_sys_audit_client",
                table: "sys_audit_log",
                column: "client_id");

            migrationBuilder.CreateIndex(
                name: "idx_sys_audit_event",
                table: "sys_audit_log",
                column: "event_type");

            migrationBuilder.CreateIndex(
                name: "idx_sys_audit_event_time",
                table: "sys_audit_log",
                columns: new[] { "event_type", "create_time" });

            migrationBuilder.CreateIndex(
                name: "idx_sys_audit_operate_user",
                table: "sys_audit_log",
                column: "operate_user_id");

            migrationBuilder.CreateIndex(
                name: "idx_sys_audit_operate_user_time",
                table: "sys_audit_log",
                columns: new[] { "operate_user_id", "create_time" });

            migrationBuilder.CreateIndex(
                name: "idx_sys_audit_operation",
                table: "sys_audit_log",
                column: "operation_type");

            migrationBuilder.CreateIndex(
                name: "idx_sys_audit_request",
                table: "sys_audit_log",
                column: "request_id");

            migrationBuilder.CreateIndex(
                name: "idx_sys_audit_result",
                table: "sys_audit_log",
                column: "event_result");

            migrationBuilder.CreateIndex(
                name: "idx_sys_audit_target_user",
                table: "sys_audit_log",
                column: "target_user_id");

            migrationBuilder.CreateIndex(
                name: "idx_sys_audit_target_user_time",
                table: "sys_audit_log",
                columns: new[] { "target_user_id", "create_time" });

            migrationBuilder.CreateIndex(
                name: "idx_sys_audit_time",
                table: "sys_audit_log",
                column: "create_time");

            migrationBuilder.CreateIndex(
                name: "idx_sys_department_leader",
                table: "sys_department",
                column: "leader_user_id");

            migrationBuilder.CreateIndex(
                name: "idx_sys_department_parent",
                table: "sys_department",
                column: "parent_id");

            migrationBuilder.CreateIndex(
                name: "idx_sys_department_status",
                table: "sys_department",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "idx_sys_department_unit",
                table: "sys_department",
                column: "unit_id");

            migrationBuilder.CreateIndex(
                name: "uk_sys_department_unit_code",
                table: "sys_department",
                columns: new[] { "unit_id", "dept_code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_sys_login_log_client",
                table: "sys_login_log",
                column: "client_id");

            migrationBuilder.CreateIndex(
                name: "idx_sys_login_log_client_time",
                table: "sys_login_log",
                columns: new[] { "client_id", "create_time" });

            migrationBuilder.CreateIndex(
                name: "idx_sys_login_log_login_name",
                table: "sys_login_log",
                column: "login_name");

            migrationBuilder.CreateIndex(
                name: "idx_sys_login_log_request",
                table: "sys_login_log",
                column: "request_id");

            migrationBuilder.CreateIndex(
                name: "idx_sys_login_log_result",
                table: "sys_login_log",
                column: "login_result");

            migrationBuilder.CreateIndex(
                name: "idx_sys_login_log_time",
                table: "sys_login_log",
                column: "create_time");

            migrationBuilder.CreateIndex(
                name: "idx_sys_login_log_type",
                table: "sys_login_log",
                column: "login_type");

            migrationBuilder.CreateIndex(
                name: "idx_sys_login_log_user",
                table: "sys_login_log",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "idx_sys_login_log_user_time",
                table: "sys_login_log",
                columns: new[] { "user_id", "create_time" });

            migrationBuilder.CreateIndex(
                name: "idx_sys_position_status",
                table: "sys_position",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "idx_sys_position_type",
                table: "sys_position",
                column: "position_type");

            migrationBuilder.CreateIndex(
                name: "uk_sys_position_code",
                table: "sys_position",
                column: "position_code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_sys_region_level",
                table: "sys_region",
                column: "region_level");

            migrationBuilder.CreateIndex(
                name: "idx_sys_region_parent",
                table: "sys_region",
                column: "parent_id");

            migrationBuilder.CreateIndex(
                name: "idx_sys_region_status",
                table: "sys_region",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "uk_sys_region_code",
                table: "sys_region",
                column: "region_code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_sys_unit_name",
                table: "sys_unit",
                column: "unit_name");

            migrationBuilder.CreateIndex(
                name: "idx_sys_unit_parent",
                table: "sys_unit",
                column: "parent_id");

            migrationBuilder.CreateIndex(
                name: "idx_sys_unit_region",
                table: "sys_unit",
                column: "region_id");

            migrationBuilder.CreateIndex(
                name: "idx_sys_unit_status",
                table: "sys_unit",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "uk_sys_unit_code",
                table: "sys_unit",
                column: "unit_code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_sys_user_create_time",
                table: "sys_user",
                column: "create_time");

            migrationBuilder.CreateIndex(
                name: "idx_sys_user_id_card_hash",
                table: "sys_user",
                column: "id_card_hash");

            migrationBuilder.CreateIndex(
                name: "idx_sys_user_mobile",
                table: "sys_user",
                column: "mobile");

            migrationBuilder.CreateIndex(
                name: "idx_sys_user_real_name",
                table: "sys_user",
                column: "real_name");

            migrationBuilder.CreateIndex(
                name: "idx_sys_user_status",
                table: "sys_user",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "uk_sys_user_login_name",
                table: "sys_user",
                column: "login_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "uk_sys_user_main_org_relation",
                table: "sys_user_main_organization",
                column: "user_organization_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "uk_sys_user_main_org_user",
                table: "sys_user_main_organization",
                column: "user_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_sys_user_org_department",
                table: "sys_user_organization",
                column: "department_id");

            migrationBuilder.CreateIndex(
                name: "idx_sys_user_org_position",
                table: "sys_user_organization",
                column: "position_id");

            migrationBuilder.CreateIndex(
                name: "idx_sys_user_org_region",
                table: "sys_user_organization",
                column: "region_id");

            migrationBuilder.CreateIndex(
                name: "idx_sys_user_org_status",
                table: "sys_user_organization",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "idx_sys_user_org_unit",
                table: "sys_user_organization",
                column: "unit_id");

            migrationBuilder.CreateIndex(
                name: "idx_sys_user_org_user",
                table: "sys_user_organization",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "idx_sys_pwd_history_user_time",
                table: "sys_user_password_history",
                columns: new[] { "user_id", "create_time" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "oauth_scope");

            migrationBuilder.DropTable(
                name: "oauth_token");

            migrationBuilder.DropTable(
                name: "sys_admin_permission");

            migrationBuilder.DropTable(
                name: "sys_admin_role");

            migrationBuilder.DropTable(
                name: "sys_admin_role_permission");

            migrationBuilder.DropTable(
                name: "sys_admin_user_role");

            migrationBuilder.DropTable(
                name: "sys_audit_log");

            migrationBuilder.DropTable(
                name: "sys_department");

            migrationBuilder.DropTable(
                name: "sys_login_log");

            migrationBuilder.DropTable(
                name: "sys_position");

            migrationBuilder.DropTable(
                name: "sys_region");

            migrationBuilder.DropTable(
                name: "sys_unit");

            migrationBuilder.DropTable(
                name: "sys_user");

            migrationBuilder.DropTable(
                name: "sys_user_main_organization");

            migrationBuilder.DropTable(
                name: "sys_user_organization");

            migrationBuilder.DropTable(
                name: "sys_user_password_history");

            migrationBuilder.DropTable(
                name: "oauth_authorization");

            migrationBuilder.DropTable(
                name: "oauth_application");
        }
    }
}
