规则名称
修改实体需同步更新 D:\CMS\database_schema.sql 数据库修改语句
1. 规则目的
保证实体定义与数据库结构一致：避免代码中实体字段 / 类型修改后，数据库脚本未同步导致的运行时错误；
统一数据库变更管理：所有实体相关的数据库修改（新增 / 删除 / 修改字段、索引等）均集中维护在 database_schema.sql，便于追溯、回滚和环境同步；
降低协作风险：避免多人开发时因数据库脚本不一致导致的联调、测试环境异常。
2. 适用范围
本规则适用于 Trae AI IDE 项目中 CMS 模块 的所有实体修改场景，包括但不限于：
新增实体类 / 实体字段（如给 User 实体新增 phone 字段）；
修改实体字段属性（如字段类型、长度、默认值、非空约束等）；
删除实体 / 实体字段；
调整实体关联关系（如新增 / 删除外键、一对一 / 一对多关联）；
新增 / 修改 / 删除实体对应的数据库索引、唯一约束。
3. 核心要求
3.1 同步时机
实体代码修改完成后，提交代码前 必须完成 database_schema.sql 的更新；
若实体修改涉及数据库变更（如字段类型调整），需在 SQL 文件中添加 可执行的修改语句；若仅为代码层逻辑修改（如新增实体方法），无需修改 SQL 文件，但需在提交备注中注明 “无数据库变更”。
3.2 SQL 文件规范
文件路径：严格指向 D:\CMS\database_schema.sql（若为多人协作 / 跨平台场景，建议补充相对路径说明：项目根目录下 CMS/database_schema.sql）；
SQL 格式：
每条数据库修改语句需添加 注释，注明 “修改原因、关联实体、修改人、修改时间”；
新增字段 / 表需明确字段类型、长度、默认值、非空约束，示例：ALTER TABLE user ADD COLUMN phone VARCHAR(20) DEFAULT '' COMMENT '用户手机号' NOT NULL;；
修改字段需注明 “原属性→新属性”，示例：ALTER TABLE user MODIFY COLUMN age INT(3) DEFAULT 0 COMMENT '用户年龄（原类型为TINYINT）';；
删除操作需谨慎，必须添加 “确认删除” 注释，示例：-- 确认删除：用户表冗余字段 address，2026-03-19，张三 ALTER TABLE user DROP COLUMN address;；
版本兼容：SQL 语句需兼容项目使用的数据库版本（如 MySQL 8.0+），避免语法错误；
可执行性：新增的 SQL 语句需保证 “可重复执行”（如添加 IF NOT EXISTS），示例：ALTER TABLE user ADD COLUMN IF NOT EXISTS phone VARCHAR(20) DEFAULT '';。