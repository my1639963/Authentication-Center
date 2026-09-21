-- ============================================================
-- 用户数据
-- 密码为: Admin@123
-- PBKDF2 hash (salt.hash 格式)
-- ============================================================
INSERT INTO sys_user (id, login_name, password_hash, real_name, mobile, email, status, login_fail_count, must_modify_pwd, token_version, is_deleted, create_time, update_time) VALUES
(1001, 'admin','GiTqWE91tQ2FIgScQRyU0Q==.TqKxy/37Niax5p5M34slQA/XQVxZAWG67+FTAfWESHw=', '系统管理员', '13800000001', 'admin@authcenter.com',0, 0, 0, 1, 0, NOW(), NOW()),

-- ============================================================
-- 管理角色
-- ============================================================
INSERT INTO sys_admin_role (id, role_code, role_name, description, status, is_deleted, create_time, update_time) VALUES
(600, 'SUPER_ADMIN',  '超级管理员', '拥有系统全部权限',     0, 0, NOW(), NOW()),
(601, 'ADMIN',        '管理员',     '系统管理角色',         0, 0, NOW(), NOW()),
(602, 'USER_ADMIN',   '用户管理员', '负责用户账号管理',     0, 0, NOW(), NOW()),
(603, 'AUDITOR',      '审计员',     '负责日志审计查看',     0, 0, NOW(), NOW()),
(604, 'VIEWER',       '只读用户',   '仅有查看权限',         0, 0, NOW(), NOW());

-- ============================================================
-- 管理权限
-- ============================================================
INSERT INTO sys_admin_permission (id, permission_code, permission_name, description, create_time, update_time) VALUES
(700, 'user:read',       '查看用户',       '查看用户列表和详情',     NOW(), NOW()),
(701, 'user:write',      '编辑用户',       '创建和修改用户信息',     NOW(), NOW()),
(702, 'user:delete',     '删除用户',       '删除用户账号',           NOW(), NOW()),
(703, 'user:reset_pwd',  '重置密码',       '重置用户密码',           NOW(), NOW()),
(704, 'user:status',     '修改状态',       '启用或禁用用户',         NOW(), NOW()),
(710, 'role:read',       '查看角色',       '查看角色列表',           NOW(), NOW()),
(711, 'role:write',      '编辑角色',       '创建和修改角色',         NOW(), NOW()),
(712, 'role:assign',     '分配权限',       '为角色分配权限',         NOW(), NOW()),
(720, 'org:read',        '查看组织',       '查看组织架构信息',       NOW(), NOW()),
(721, 'org:write',       '编辑组织',       '管理区域/单位/部门/岗位', NOW(), NOW()),
(730, 'oauth:read',      '查看客户端',     '查看OAuth客户端列表',    NOW(), NOW()),
(731, 'oauth:write',     '编辑客户端',     '管理OAuth客户端',        NOW(), NOW()),
(740, 'log:login',       '查看登录日志',   '查看登录日志',           NOW(), NOW()),
(741, 'log:audit',       '查看审计日志',   '查看审计日志',           NOW(), NOW());

-- ============================================================
-- 角色权限分配
-- ============================================================
-- 超级管理员: 所有权限
INSERT INTO sys_admin_role_permission (id, role_id, permission_id, create_time) VALUES
(800, 600, 700, NOW()), (801, 600, 701, NOW()), (802, 600, 702, NOW()), (803, 600, 703, NOW()), (804, 600, 704, NOW()),
(805, 600, 710, NOW()), (806, 600, 711, NOW()), (807, 600, 712, NOW()),
(808, 600, 720, NOW()), (809, 600, 721, NOW()),
(810, 600, 730, NOW()), (811, 600, 731, NOW()),
(812, 600, 740, NOW()), (813, 600, 741, NOW());

-- 管理员: 用户管理 + 组织管理 + 日志
INSERT INTO sys_admin_role_permission (id, role_id, permission_id, create_time) VALUES
(820, 601, 700, NOW()), (821, 601, 701, NOW()), (822, 601, 703, NOW()), (823, 601, 704, NOW()),
(824, 601, 720, NOW()), (825, 601, 721, NOW()),
(826, 601, 740, NOW()), (827, 601, 741, NOW());

-- 用户管理员: 用户管理
INSERT INTO sys_admin_role_permission (id, role_id, permission_id, create_time) VALUES
(830, 602, 700, NOW()), (831, 602, 701, NOW()), (832, 602, 703, NOW()), (833, 602, 704, NOW());

-- 审计员: 日志
INSERT INTO sys_admin_role_permission (id, role_id, permission_id, create_time) VALUES
(840, 603, 740, NOW()), (841, 603, 741, NOW());

-- 只读用户: 查看权限
INSERT INTO sys_admin_role_permission (id, role_id, permission_id, create_time) VALUES
(850, 604, 700, NOW()), (851, 604, 710, NOW()), (852, 604, 720, NOW()), (853, 604, 730, NOW()), (854, 604, 740, NOW()), (855, 604, 741, NOW());

-- ============================================================
-- 用户角色分配
-- ============================================================
INSERT INTO sys_admin_user_role (id, user_id, role_id, create_time) VALUES
(900, 1001, 600, NOW()),  -- admin -> 超级管理员