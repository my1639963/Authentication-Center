CREATE TABLE `sys_user` (
  `id` bigint PRIMARY KEY NOT NULL COMMENT '用户ID，应用层生成分布式ID',
  `login_name` varchar(100) NOT NULL COMMENT '登录账号，全局唯一',
  `password_hash` varchar(512) NOT NULL COMMENT '密码哈希，不保存明文密码',
  `real_name` varchar(100) NOT NULL COMMENT '真实姓名',
  `mobile` varchar(32) COMMENT '手机号码',
  `email` varchar(200) COMMENT '电子邮箱',
  `id_card_ciphertext` varchar(1024) COMMENT '身份证号密文',
  `id_card_hash` varchar(128) COMMENT '身份证号HMAC-SHA256，用于精确查询',
  `status` integer NOT NULL DEFAULT 0 COMMENT '用户状态：0正常，1禁用，2锁定',
  `login_fail_count` integer NOT NULL DEFAULT 0 COMMENT '连续登录失败次数',
  `lock_until` timestamp COMMENT '登录锁定截止时间',
  `must_modify_pwd` integer NOT NULL DEFAULT 0 COMMENT '是否必须修改密码：0否，1是',
  `password_expire_time` timestamp NULL COMMENT '密码过期时间',
  `security_stamp` varchar(128) COMMENT '安全戳，密码/安全状态变化时更新',
  `token_version` bigint NOT NULL DEFAULT 1 COMMENT '用户Token版本，安全事件时递增',
  `last_login_time` timestamp NULL COMMENT '最后登录时间',
  `last_login_ip` varchar(64) COMMENT '最后登录IP',
  `create_time` timestamp NOT NULL COMMENT '创建时间' DEFAULT now(),
  `update_time` timestamp NOT NULL COMMENT '更新时间' DEFAULT now(),
  `is_deleted` integer NOT NULL DEFAULT 0 COMMENT '逻辑删除：0否，1是'
);

CREATE TABLE `sys_region` (
  `id` bigint PRIMARY KEY NOT NULL COMMENT '行政区划ID',
  `parent_id` bigint COMMENT '父级行政区划ID，顶级节点为空',
  `region_code` varchar(32) NOT NULL COMMENT '行政区划编码，全局唯一',
  `region_name` varchar(100) NOT NULL COMMENT '行政区划名称',
  `region_level` integer NOT NULL COMMENT '行政区划级别',
  `status` integer NOT NULL DEFAULT 0 COMMENT '状态：0正常，1禁用',
  `sort` integer NOT NULL DEFAULT 0 COMMENT '排序号',
  `create_time` timestamp NOT NULL DEFAULT now(),
  `update_time` timestamp NOT NULL DEFAULT now(),
  `is_deleted` integer NOT NULL DEFAULT 0 COMMENT '逻辑删除'
);

CREATE TABLE `sys_unit` (
  `id` bigint PRIMARY KEY NOT NULL COMMENT '单位ID',
  `parent_id` bigint COMMENT '上级单位ID',
  `unit_code` varchar(64) NOT NULL COMMENT '单位编码，全局唯一',
  `unit_name` varchar(200) NOT NULL COMMENT '单位名称',
  `unit_type` varchar(32) COMMENT '单位类型',
  `unit_level` integer COMMENT '单位层级',
  `region_id` bigint COMMENT '所属行政区划',
  `status` integer NOT NULL DEFAULT 0 COMMENT '状态：0正常，1禁用',
  `sort` integer NOT NULL DEFAULT 0,
  `description` varchar(1000) COMMENT '单位描述',
  `create_time` timestamp NOT NULL DEFAULT now(),
  `update_time` timestamp NOT NULL DEFAULT now(),
  `is_deleted` integer NOT NULL DEFAULT 0
);

CREATE TABLE `sys_department` (
  `id` bigint PRIMARY KEY NOT NULL COMMENT '部门ID',
  `unit_id` bigint NOT NULL COMMENT '所属单位',
  `parent_id` bigint COMMENT '父级部门',
  `dept_code` varchar(64) NOT NULL COMMENT '部门编码，在单位内唯一',
  `dept_name` varchar(200) NOT NULL COMMENT '部门名称',
  `leader_user_id` bigint COMMENT '部门负责人用户ID',
  `status` integer NOT NULL DEFAULT 0 COMMENT '状态：0正常，1禁用',
  `sort` integer NOT NULL DEFAULT 0,
  `description` varchar(1000),
  `create_time` timestamp NOT NULL DEFAULT now(),
  `update_time` timestamp NOT NULL DEFAULT now(),
  `is_deleted` integer NOT NULL DEFAULT 0
);

CREATE TABLE `sys_position` (
  `id` bigint PRIMARY KEY NOT NULL COMMENT '职务ID',
  `position_code` varchar(64) NOT NULL COMMENT '职务编码，全局唯一',
  `position_name` varchar(100) NOT NULL COMMENT '职务名称',
  `position_type` varchar(32) COMMENT '职务类型',
  `position_level` integer COMMENT '职务层级',
  `status` integer NOT NULL DEFAULT 0 COMMENT '状态：0正常，1禁用',
  `sort` integer NOT NULL DEFAULT 0,
  `description` varchar(1000),
  `create_time` timestamp NOT NULL DEFAULT now(),
  `update_time` timestamp NOT NULL DEFAULT now(),
  `is_deleted` integer NOT NULL DEFAULT 0
);

CREATE TABLE `sys_user_organization` (
  `id` bigint PRIMARY KEY NOT NULL COMMENT '任职关系ID',
  `user_id` bigint NOT NULL COMMENT '用户ID',
  `unit_id` bigint NOT NULL COMMENT '单位ID',
  `department_id` bigint COMMENT '部门ID',
  `position_id` bigint COMMENT '职务ID',
  `region_id` bigint COMMENT '行政区划ID',
  `start_time` timestamp COMMENT '任职开始时间' DEFAULT now(),
  `end_time` timestamp NULL COMMENT '任职结束时间',
  `status` integer NOT NULL DEFAULT 0 COMMENT '状态：0正常，1停用',
  `create_time` timestamp NOT NULL DEFAULT now(),
  `update_time` timestamp NOT NULL DEFAULT now(),
  `is_deleted` integer NOT NULL DEFAULT 0
);

CREATE TABLE `sys_user_main_organization` (
  `id` bigint PRIMARY KEY NOT NULL COMMENT '主任职记录ID',
  `user_id` bigint NOT NULL COMMENT '用户ID，每个用户只能存在一条主任职记录',
  `user_organization_id` bigint NOT NULL COMMENT '对应用户任职关系ID',
  `create_time` timestamp NOT NULL DEFAULT now(),
  `update_time` timestamp NOT NULL DEFAULT now()
);

CREATE TABLE `sys_user_password_history` (
  `id` bigint PRIMARY KEY NOT NULL COMMENT '密码历史ID',
  `user_id` bigint NOT NULL COMMENT '用户ID',
  `password_hash` varchar(512) NOT NULL COMMENT '历史密码哈希',
  `create_time` timestamp NOT NULL COMMENT '密码设置时间'
);

CREATE TABLE `sys_admin_role` (
  `id` bigint PRIMARY KEY NOT NULL COMMENT '管理角色ID',
  `role_code` varchar(64) NOT NULL COMMENT '角色编码',
  `role_name` varchar(100) NOT NULL COMMENT '角色名称',
  `description` varchar(1000),
  `status` integer NOT NULL DEFAULT 0 COMMENT '状态：0正常，1禁用',
  `create_time` timestamp NOT NULL DEFAULT now(),
  `update_time` timestamp NOT NULL DEFAULT now(),
  `is_deleted` integer NOT NULL DEFAULT 0
);

CREATE TABLE `sys_admin_permission` (
  `id` bigint PRIMARY KEY NOT NULL COMMENT '权限ID',
  `permission_code` varchar(128) NOT NULL COMMENT '权限编码',
  `permission_name` varchar(100) NOT NULL COMMENT '权限名称',
  `description` varchar(1000),
  `create_time` timestamp NOT NULL DEFAULT now(),
  `update_time` timestamp NOT NULL DEFAULT now()
);

CREATE TABLE `sys_admin_user_role` (
  `id` bigint PRIMARY KEY NOT NULL,
  `user_id` bigint NOT NULL COMMENT '用户ID',
  `role_id` bigint NOT NULL COMMENT '管理角色ID',
  `create_time` timestamp NOT NULL DEFAULT now()
);

CREATE TABLE `sys_admin_role_permission` (
  `id` bigint PRIMARY KEY NOT NULL,
  `role_id` bigint NOT NULL COMMENT '管理角色ID',
  `permission_id` bigint NOT NULL COMMENT '管理权限ID',
  `create_time` timestamp NOT NULL DEFAULT now()
);

CREATE TABLE `sys_login_log` (
  `id` bigint PRIMARY KEY NOT NULL COMMENT '登录日志ID',
  `user_id` bigint COMMENT '用户ID',
  `login_name` varchar(100) COMMENT '登录账号',
  `client_id` varchar(200) COMMENT 'OAuth Client ID',
  `login_type` varchar(32) NOT NULL COMMENT '登录类型：PASSWORD / REFRESH_TOKEN / CLIENT_CREDENTIALS / SSO',
  `login_result` integer NOT NULL COMMENT '登录结果：0成功，1失败',
  `fail_reason` varchar(1000) COMMENT '失败原因',
  `ip_address` varchar(64) COMMENT '客户端IP',
  `user_agent` varchar(1000) COMMENT 'User-Agent',
  `request_id` varchar(128) COMMENT '请求追踪ID',
  `create_time` timestamp NOT NULL COMMENT '登录时间' DEFAULT now()
);

CREATE TABLE `sys_audit_log` (
  `id` bigint PRIMARY KEY NOT NULL COMMENT '审计日志ID',
  `event_type` varchar(64) NOT NULL COMMENT '事件类型',
  `operation_type` varchar(64) NOT NULL COMMENT '操作类型',
  `operate_user_id` bigint COMMENT '操作人用户ID',
  `target_user_id` bigint COMMENT '目标用户ID',
  `client_id` varchar(200) COMMENT 'OAuth Client ID',
  `ip_address` varchar(64),
  `user_agent` varchar(1000),
  `event_result` integer NOT NULL COMMENT '事件结果：0成功，1失败',
  `fail_reason` varchar(1000) COMMENT '失败原因',
  `content` text COMMENT '审计详细内容，JSON字符串',
  `request_id` varchar(128) COMMENT '请求追踪ID',
  `create_time` timestamp NOT NULL DEFAULT now(),
  `previous_hash` varchar(128) COMMENT '上一条审计日志Hash',
  `log_hash` varchar(128) NOT NULL COMMENT '当前审计日志Hash'
);

CREATE TABLE `openiddict_applications` (
  `id` varchar(450) PRIMARY KEY NOT NULL COMMENT 'OpenIddict Application ID',
  `application_type` varchar(100),
  `client_id` varchar(200) COMMENT 'OAuth Client ID',
  `client_secret` varchar(1000) COMMENT 'OAuth Client Secret',
  `consent_type` varchar(100),
  `display_name` varchar(500),
  `display_names` text,
  `permissions` text,
  `requirements` text,
  `redirect_uris` text,
  `post_logout_redirect_uris` text,
  `client_type` varchar(100),
  `application_type_detail` varchar(100),
  `properties` text,
  `settings` text
);

CREATE TABLE `openiddict_authorizations` (
  `id` varchar(450) PRIMARY KEY NOT NULL COMMENT 'Authorization ID',
  `application_id` varchar(450) COMMENT 'OAuth Application ID',
  `subject` varchar(400) COMMENT '用户主体标识',
  `status` varchar(100) COMMENT 'Authorization状态',
  `type` varchar(100) COMMENT 'Authorization类型',
  `scopes` text COMMENT '授权Scope',
  `creation_date` timestamp,
  `properties` text
);

CREATE TABLE `openiddict_scopes` (
  `id` varchar(450) PRIMARY KEY NOT NULL,
  `name` varchar(200) COMMENT 'Scope名称',
  `display_name` varchar(500) COMMENT 'Scope显示名称',
  `description` varchar(1000),
  `resources` text COMMENT 'Scope关联的Resource/Audience',
  `properties` text
);

CREATE TABLE `openiddict_tokens` (
  `id` varchar(450) PRIMARY KEY NOT NULL,
  `application_id` varchar(450) COMMENT 'OAuth Application ID',
  `authorization_id` varchar(450) COMMENT 'Authorization ID',
  `subject` varchar(400) COMMENT 'Token Subject',
  `type` varchar(100) COMMENT 'Token类型',
  `status` varchar(100) COMMENT 'Token状态',
  `reference_id` varchar(450) COMMENT 'Reference Token标识',
  `payload` text COMMENT 'Token Payload',
  `creation_date` timestamp DEFAULT now(),
  `expiration_date` timestamp DEFAULT now(),
  `redemption_date` timestamp DEFAULT now(),
  `properties` text
);

CREATE UNIQUE INDEX `uk_sys_user_login_name` ON `sys_user` (`login_name`);

CREATE INDEX `idx_sys_user_real_name` ON `sys_user` (`real_name`);

CREATE INDEX `idx_sys_user_mobile` ON `sys_user` (`mobile`);

CREATE INDEX `idx_sys_user_id_card_hash` ON `sys_user` (`id_card_hash`);

CREATE INDEX `idx_sys_user_status` ON `sys_user` (`status`);

CREATE INDEX `idx_sys_user_create_time` ON `sys_user` (`create_time`);

CREATE UNIQUE INDEX `uk_sys_region_code` ON `sys_region` (`region_code`);

CREATE INDEX `idx_sys_region_parent` ON `sys_region` (`parent_id`);

CREATE INDEX `idx_sys_region_level` ON `sys_region` (`region_level`);

CREATE INDEX `idx_sys_region_status` ON `sys_region` (`status`);

CREATE UNIQUE INDEX `uk_sys_unit_code` ON `sys_unit` (`unit_code`);

CREATE INDEX `idx_sys_unit_parent` ON `sys_unit` (`parent_id`);

CREATE INDEX `idx_sys_unit_region` ON `sys_unit` (`region_id`);

CREATE INDEX `idx_sys_unit_status` ON `sys_unit` (`status`);

CREATE INDEX `idx_sys_unit_name` ON `sys_unit` (`unit_name`);

CREATE UNIQUE INDEX `uk_sys_department_unit_code` ON `sys_department` (`unit_id`, `dept_code`);

CREATE INDEX `idx_sys_department_unit` ON `sys_department` (`unit_id`);

CREATE INDEX `idx_sys_department_parent` ON `sys_department` (`parent_id`);

CREATE INDEX `idx_sys_department_leader` ON `sys_department` (`leader_user_id`);

CREATE INDEX `idx_sys_department_status` ON `sys_department` (`status`);

CREATE UNIQUE INDEX `uk_sys_position_code` ON `sys_position` (`position_code`);

CREATE INDEX `idx_sys_position_status` ON `sys_position` (`status`);

CREATE INDEX `idx_sys_position_type` ON `sys_position` (`position_type`);

CREATE INDEX `idx_sys_user_org_user` ON `sys_user_organization` (`user_id`);

CREATE INDEX `idx_sys_user_org_unit` ON `sys_user_organization` (`unit_id`);

CREATE INDEX `idx_sys_user_org_department` ON `sys_user_organization` (`department_id`);

CREATE INDEX `idx_sys_user_org_position` ON `sys_user_organization` (`position_id`);

CREATE INDEX `idx_sys_user_org_region` ON `sys_user_organization` (`region_id`);

CREATE INDEX `idx_sys_user_org_status` ON `sys_user_organization` (`status`);

CREATE UNIQUE INDEX `uk_sys_user_main_org_user` ON `sys_user_main_organization` (`user_id`);

CREATE UNIQUE INDEX `uk_sys_user_main_org_relation` ON `sys_user_main_organization` (`user_organization_id`);

CREATE INDEX `idx_sys_pwd_history_user_time` ON `sys_user_password_history` (`user_id`, `create_time`);

CREATE UNIQUE INDEX `uk_sys_admin_role_code` ON `sys_admin_role` (`role_code`);

CREATE INDEX `idx_sys_admin_role_status` ON `sys_admin_role` (`status`);

CREATE UNIQUE INDEX `uk_sys_admin_permission_code` ON `sys_admin_permission` (`permission_code`);

CREATE UNIQUE INDEX `uk_sys_admin_user_role` ON `sys_admin_user_role` (`user_id`, `role_id`);

CREATE INDEX `idx_sys_admin_user_role_user` ON `sys_admin_user_role` (`user_id`);

CREATE INDEX `idx_sys_admin_user_role_role` ON `sys_admin_user_role` (`role_id`);

CREATE UNIQUE INDEX `uk_sys_admin_role_permission` ON `sys_admin_role_permission` (`role_id`, `permission_id`);

CREATE INDEX `idx_sys_admin_role_permission_role` ON `sys_admin_role_permission` (`role_id`);

CREATE INDEX `idx_sys_admin_role_permission_permission` ON `sys_admin_role_permission` (`permission_id`);

CREATE INDEX `idx_sys_login_log_user` ON `sys_login_log` (`user_id`);

CREATE INDEX `idx_sys_login_log_login_name` ON `sys_login_log` (`login_name`);

CREATE INDEX `idx_sys_login_log_client` ON `sys_login_log` (`client_id`);

CREATE INDEX `idx_sys_login_log_type` ON `sys_login_log` (`login_type`);

CREATE INDEX `idx_sys_login_log_result` ON `sys_login_log` (`login_result`);

CREATE INDEX `idx_sys_login_log_time` ON `sys_login_log` (`create_time`);

CREATE INDEX `idx_sys_login_log_request` ON `sys_login_log` (`request_id`);

CREATE INDEX `idx_sys_login_log_user_time` ON `sys_login_log` (`user_id`, `create_time`);

CREATE INDEX `idx_sys_login_log_client_time` ON `sys_login_log` (`client_id`, `create_time`);

CREATE INDEX `idx_sys_audit_operate_user` ON `sys_audit_log` (`operate_user_id`);

CREATE INDEX `idx_sys_audit_target_user` ON `sys_audit_log` (`target_user_id`);

CREATE INDEX `idx_sys_audit_client` ON `sys_audit_log` (`client_id`);

CREATE INDEX `idx_sys_audit_event` ON `sys_audit_log` (`event_type`);

CREATE INDEX `idx_sys_audit_operation` ON `sys_audit_log` (`operation_type`);

CREATE INDEX `idx_sys_audit_result` ON `sys_audit_log` (`event_result`);

CREATE INDEX `idx_sys_audit_time` ON `sys_audit_log` (`create_time`);

CREATE INDEX `idx_sys_audit_request` ON `sys_audit_log` (`request_id`);

CREATE INDEX `idx_sys_audit_event_time` ON `sys_audit_log` (`event_type`, `create_time`);

CREATE INDEX `idx_sys_audit_operate_user_time` ON `sys_audit_log` (`operate_user_id`, `create_time`);

CREATE INDEX `idx_sys_audit_target_user_time` ON `sys_audit_log` (`target_user_id`, `create_time`);

CREATE UNIQUE INDEX `uk_openiddict_app_client_id` ON `openiddict_applications` (`client_id`);

CREATE INDEX `idx_openiddict_authorization_application` ON `openiddict_authorizations` (`application_id`);

CREATE INDEX `idx_openiddict_authorization_subject` ON `openiddict_authorizations` (`subject`);

CREATE INDEX `idx_openiddict_authorization_status` ON `openiddict_authorizations` (`status`);

CREATE INDEX `idx_openiddict_authorization_type` ON `openiddict_authorizations` (`type`);

CREATE UNIQUE INDEX `uk_openiddict_scope_name` ON `openiddict_scopes` (`name`);

CREATE INDEX `idx_openiddict_token_application` ON `openiddict_tokens` (`application_id`);

CREATE INDEX `idx_openiddict_token_authorization` ON `openiddict_tokens` (`authorization_id`);

CREATE INDEX `idx_openiddict_token_subject` ON `openiddict_tokens` (`subject`);

CREATE INDEX `idx_openiddict_token_type` ON `openiddict_tokens` (`type`);

CREATE INDEX `idx_openiddict_token_status` ON `openiddict_tokens` (`status`);

CREATE UNIQUE INDEX `uk_openiddict_token_reference` ON `openiddict_tokens` (`reference_id`);

CREATE INDEX `idx_openiddict_token_expiration` ON `openiddict_tokens` (`expiration_date`);
