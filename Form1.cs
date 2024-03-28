using Dapper;
using Npgsql;
using System.Diagnostics;
using System.Text;
using System.Xml.Linq;

namespace CodeGenerator
{
    public partial class Form1 : Form
    {
        private string[] IgnoreColums = { "Id", "CreatorId", "Creator", "CreationTime", "UpdaterId", "Updater", "UpdateTime", "SoftDeleted" };
        private string output_dir;

        public Form1()
        {
            InitializeComponent();

            this.Load += Form1_Load;
        }

        private void Form1_Load(object? sender, EventArgs e)
        {
            var ini_file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ini.txt");
            var lines = File.ReadAllLines(ini_file);
            if(lines.Length < 5)
            {
                MessageBox.Show("初始化文件配置错误");
                return;
            }
            
            txtConnectionString.Text = lines[0];
            txtNamespace1.Text = lines[1];
            txtNamespace2.Text = lines[2];
            txtTableName.Text = lines[3];
            txtEntityName.Text = lines[4];
            output_dir = lines[5];

            if (!Directory.Exists(output_dir))
            {
                Directory.CreateDirectory(output_dir);
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            #region 获取基本信息并初始化

            var connectString = txtConnectionString.Text;
            var tableName = txtTableName.Text;
            var entityName = txtEntityName.Text;
            var entityNameCN = txtEntityNameCN.Text;
            var namespace1 = txtNamespace1.Text;
            var namespace2 = txtNamespace2.Text;

            if (string.IsNullOrWhiteSpace(connectString))
            {
                MessageBox.Show("请输入数据库连接串");
                return;
            }

            if (string.IsNullOrWhiteSpace(tableName))
            {
                MessageBox.Show("请输入表名");
                return;
            }

            if (string.IsNullOrWhiteSpace(entityName))
            {
                MessageBox.Show("请输入实体名");
                return;
            }

            if (string.IsNullOrWhiteSpace(namespace1))
            {
                MessageBox.Show("请输入命名空间1");
                return;
            }

            if (string.IsNullOrWhiteSpace(namespace2))
            {
                MessageBox.Show("请输入命名空间2");
                return;
            }

            var application_dir = Path.Combine(output_dir, namespace2 + ".Application", entityName);
            if (!Directory.Exists(application_dir))
            {
                Directory.CreateDirectory(application_dir);
            }

            var domain_dir = Path.Combine(output_dir, namespace2 + ".Domain", entityName);
            if (!Directory.Exists(domain_dir))
            {
                Directory.CreateDirectory(domain_dir);
            }

            var infra_dir = Path.Combine(output_dir, namespace2 + ".Infrastructure");
            if (!Directory.Exists(infra_dir))
            {
                Directory.CreateDirectory(infra_dir);
            }

            var webapi_dir = Path.Combine(output_dir, namespace2 + ".WebApi");
            if (!Directory.Exists(webapi_dir))
            {
                Directory.CreateDirectory(webapi_dir);
            }

            //拷贝模板
            var templateDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates");

            CopyDirectory(Path.Combine(templateDir, "Application"), application_dir);
            CopyDirectory(Path.Combine(templateDir, "Domain"), domain_dir);
            CopyDirectory(Path.Combine(templateDir, "Infrastructure"), infra_dir);
            CopyDirectory(Path.Combine(templateDir, "WebApi"), webapi_dir);

            #endregion

            #region 获取字段信息

            var columns = new List<Column>();

            var sql = $"SELECT col.column_name, col.is_nullable, col.data_type, col.character_maximum_length,des.description FROM information_schema.columns col LEFT JOIN pg_class cls ON col.table_name = cls.relname LEFT JOIN pg_description des ON des.objoid = cls.oid AND col.ordinal_position = des.objsubid WHERE col.table_schema = 'public' and col.table_name = '{tableName}' ORDER BY col.ordinal_position;";
            using (var connection = new NpgsqlConnection(connectString))
            {
                connection.Open();

                var reader = connection.ExecuteReader(sql);
                while (reader.Read())
                {
                    var col = new Column();
                    if (!reader.IsDBNull(0))
                    {
                        col.Name = reader.GetString(0);
                    }

                    if (!reader.IsDBNull(1))
                    {
                        col.Nullable = reader.GetString(1) == "YES";
                    }

                    if (!reader.IsDBNull(2))
                    {
                        col.DataType = reader.GetString(2);
                    }

                    if (!reader.IsDBNull(3))
                    {
                        col.Length = reader.GetInt32(3);
                    }

                    if (!reader.IsDBNull(4))
                    {
                        col.Description = reader.GetString(4);
                    }

                    columns.Add(col);
                }
            }

            #endregion

            #region 替换 Infrastructure

            //Mappings\TemplateMap.cs
            var map_file_path = Path.Combine(infra_dir, "Mappings", "TemplateMap.cs");
            var map_text = File.ReadAllText(map_file_path);
            var new_map_text = map_text.Replace("{{Template}}", entityName)
                                        .Replace("{{TableName}}", tableName)
                                        .Replace("{{Namespace1}}", namespace1)
                                        .Replace("{{Namespace2}}", namespace2);

            var new_map_file_path = Path.Combine(infra_dir, "Mappings", $"{entityName}Map.cs");
            File.WriteAllText(new_map_file_path, new_map_text);
            File.Delete(map_file_path);

            #endregion

            #region 替换 Domain

            #region 替换 Entities\Template.cs

            var entity_file_path = Path.Combine(domain_dir, "Entities", "Template.cs");
            var entity_text = File.ReadAllText(entity_file_path);
            var new_entity_text = entity_text.Replace("{{Template}}", entityName)
                                        .Replace("{{TableName}}", tableName)
                                        .Replace("{{Namespace1}}", namespace1)
                                        .Replace("{{Namespace2}}", namespace2)
                                        .Replace("{{ENTITY_NAME_CN}}", entityNameCN);

            var builder_entity = new StringBuilder(new_entity_text);

            foreach (var col in columns)
            {
                if (IgnoreColums.Contains(col.Name))
                {
                    continue;
                }

                builder_entity.AppendLine($"        /// <summary>");
                builder_entity.AppendLine($"        /// {col.Description}");
                builder_entity.AppendLine($"        /// </summary>");

                if (!col.Nullable)
                {
                    builder_entity.AppendLine($"        [Required(ErrorMessage = \"必填项\")]");
                }

                if (col.DataType == "character varying" && col.Length.HasValue)
                {
                    builder_entity.AppendLine($"        [MaxLength({col.Length}, ErrorMessage = \"{col.Description}最长{col.Length}个字符\")]");
                }

                if (col.DataType == "ARRAY")
                {
                    builder_entity.AppendLine($"        [Column(\"{col.Name}\", TypeName = \"varchar[]\")]");
                }

                var typeName = GetTypeName(col.DataType, col.Nullable);

                builder_entity.AppendLine($"        public {typeName} {col.Name} {{ get; set; }}");
                builder_entity.AppendLine();
            }

            builder_entity.AppendLine("    }");
            builder_entity.AppendLine("}");

            var new_entity_file_path = Path.Combine(domain_dir, "Entities", $"{entityName}.cs");
            File.WriteAllText(new_entity_file_path, builder_entity.ToString());
            File.Delete(entity_file_path);

            #endregion

            #region 替换 CommandHandlers\TemplateCommandHandler.cs
            
            var cmdhdl_file_path = Path.Combine(domain_dir, "CommandHandlers", "TemplateCommandHandler.cs");
            var cmdhdl_text = File.ReadAllText(cmdhdl_file_path);
            var new_cmdhdl_text = cmdhdl_text.Replace("{{Template}}", entityName)
                                        .Replace("{{TableName}}", tableName)
                                        .Replace("{{Namespace1}}", namespace1)
                                        .Replace("{{Namespace2}}", namespace2)
                                        .Replace("{{template}}", ToCamel(entityName))
                                        .Replace("{{ENTITY_NAME_CN}}", entityNameCN);
                                      
            var builder_add_1 = new StringBuilder();
            foreach (var col in columns)
            {
                if (IgnoreColums.Contains(col.Name))
                {
                    continue;
                }

                builder_add_1.AppendLine($"                {col.Name} = cmd.{col.Name},");
            }

            new_cmdhdl_text = new_cmdhdl_text.Replace("{{ENTITY_ADD_1}}", builder_add_1.ToString());


            var builder_update_1 = new StringBuilder();
            foreach (var col in columns)
            {
                if (IgnoreColums.Contains(col.Name))
                {
                    continue;
                }

                if (col.DataType == "character varying")
                {
                    builder_update_1.AppendLine($"            if (!string.IsNullOrWhiteSpace(cmd.{col.Name}) && cmd.{col.Name} != entity.{col.Name})");
                }
                else if (col.DataType == "ARRAY")
                {
                    builder_update_1.AppendLine($"            if (cmd.{col.Name} != null)");
                }
                else
                {
                    builder_update_1.AppendLine($"            if (cmd.{col.Name}.HasValue && cmd.{col.Name} != entity.{col.Name})");
                }

                builder_update_1.AppendLine("            {");

                if (col.DataType == "character varying" || col.DataType == "ARRAY")
                {
                    builder_update_1.AppendLine($"                entity.{col.Name} = cmd.{col.Name};");
                }
                else
                {
                    builder_update_1.AppendLine($"                entity.{col.Name} = cmd.{col.Name}.Value;");
                }

                builder_update_1.AppendLine("            }");
                builder_update_1.AppendLine();
            }

            new_cmdhdl_text = new_cmdhdl_text.Replace("{{ENTITY_UPDATE_1}}", builder_update_1.ToString());

            var new_cmdhdl_file_path = Path.Combine(domain_dir, "CommandHandlers", $"{entityName}CommandHandler.cs");
            File.WriteAllText(new_cmdhdl_file_path, new_cmdhdl_text.ToString());
            File.Delete(cmdhdl_file_path);

            #endregion

            #region 替换 Commands

            var addcmd_file_path = Path.Combine(domain_dir, "Commands", "AddTemplateCommand.cs");
            var addcmd_text = File.ReadAllText(addcmd_file_path);
            var new_addcmd_text = addcmd_text.Replace("{{Template}}", entityName)
                                        .Replace("{{TableName}}", tableName)
                                        .Replace("{{Namespace1}}", namespace1)
                                        .Replace("{{Namespace2}}", namespace2);

            var builder_add_cmd = new StringBuilder();

            foreach (var col in columns)
            {
                if (IgnoreColums.Contains(col.Name))
                {
                    continue;
                }

                builder_add_cmd.AppendLine($"        /// <summary>");
                builder_add_cmd.AppendLine($"        /// {col.Description}");
                builder_add_cmd.AppendLine($"        /// </summary>");

                var typeName = GetTypeName(col.DataType, col.Nullable);

                builder_add_cmd.AppendLine($"        public {typeName} {col.Name} {{ get; set; }}");
                builder_add_cmd.AppendLine();
            }

            new_addcmd_text = new_addcmd_text.Replace("{{PROPERTIES}}", builder_add_cmd.ToString());

            var new_addcmd_file_path = Path.Combine(domain_dir, "Commands", $"Add{entityName}Command.cs");
            File.WriteAllText(new_addcmd_file_path, new_addcmd_text);
            File.Delete(addcmd_file_path);



            var updcmd_file_path = Path.Combine(domain_dir, "Commands", "UpdateTemplateCommand.cs");
            var updcmd_text = File.ReadAllText(updcmd_file_path);
            var new_updcmd_text = updcmd_text.Replace("{{Template}}", entityName)
                                        .Replace("{{TableName}}", tableName)
                                        .Replace("{{Namespace1}}", namespace1)
                                        .Replace("{{Namespace2}}", namespace2);

            var builder_upd_cmd = new StringBuilder();

            foreach (var col in columns)
            {
                if (IgnoreColums.Contains(col.Name))
                {
                    continue;
                }

                builder_upd_cmd.AppendLine($"        /// <summary>");
                builder_upd_cmd.AppendLine($"        /// {col.Description}");
                builder_upd_cmd.AppendLine($"        /// </summary>");

                var typeName = GetTypeName(col.DataType, true);

                builder_upd_cmd.AppendLine($"        public {typeName} {col.Name} {{ get; set; }}");
                builder_upd_cmd.AppendLine();
            }

            new_updcmd_text = new_updcmd_text.Replace("{{PROPERTIES}}", builder_upd_cmd.ToString());

            var new_updcmd_file_path = Path.Combine(domain_dir, "Commands", $"Update{entityName}Command.cs");
            File.WriteAllText(new_updcmd_file_path, new_updcmd_text);
            File.Delete(updcmd_file_path);



            var rmvcmd_file_path = Path.Combine(domain_dir, "Commands", "RemoveTemplateCommand.cs");
            var rmvcmd_text = File.ReadAllText(rmvcmd_file_path);
            var new_rmvcmd_text = rmvcmd_text.Replace("{{Template}}", entityName)
                                        .Replace("{{TableName}}", tableName)
                                        .Replace("{{Namespace1}}", namespace1)
                                        .Replace("{{Namespace2}}", namespace2);

            var new_rmvcmd_file_path = Path.Combine(domain_dir, "Commands", $"Remove{entityName}Command.cs");
            File.WriteAllText(new_rmvcmd_file_path, new_rmvcmd_text);
            File.Delete(rmvcmd_file_path);

            #endregion

            #region 替换 EventHandlers\TemplateEventHandler.cs

            var evthdl_file_path = Path.Combine(domain_dir, "EventHandlers", "TemplateEventHandler.cs");
            var evthdl_text = File.ReadAllText(evthdl_file_path);
            var new_evthdl_text = evthdl_text.Replace("{{Template}}", entityName)
                                        .Replace("{{TableName}}", tableName)
                                        .Replace("{{Namespace1}}", namespace1)
                                        .Replace("{{Namespace2}}", namespace2);

            var new_evthdl_file_path = Path.Combine(domain_dir, "EventHandlers", $"{entityName}EventHandler.cs");
            File.WriteAllText(new_evthdl_file_path, new_evthdl_text);
            File.Delete(evthdl_file_path);

            #endregion

            #region 替换 Events

            var evtadd_file_path = Path.Combine(domain_dir, "Events", "TemplateAddedEvent.cs");
            var evtadd_text = File.ReadAllText(evtadd_file_path);
            var new_evtadd_text = evtadd_text.Replace("{{Template}}", entityName)
                                        .Replace("{{TableName}}", tableName)
                                        .Replace("{{Namespace1}}", namespace1)
                                        .Replace("{{Namespace2}}", namespace2);

            var builder_add_evt = new StringBuilder(new_evtadd_text);

            foreach (var col in columns)
            {
                if (IgnoreColums.Contains(col.Name))
                {
                    continue;
                }

                builder_add_evt.AppendLine($"        /// <summary>");
                builder_add_evt.AppendLine($"        /// {col.Description}");
                builder_add_evt.AppendLine($"        /// </summary>");

                var typeName = GetTypeName(col.DataType, col.Nullable);

                builder_add_evt.AppendLine($"        public {typeName} {col.Name} {{ get; set; }}");
                builder_add_evt.AppendLine();
            }

            builder_add_evt.AppendLine("    }");
            builder_add_evt.AppendLine("}");

            var new_evtadd_file_path = Path.Combine(domain_dir, "Events", $"{entityName}AddedEvent.cs");
            File.WriteAllText(new_evtadd_file_path, builder_add_evt.ToString());
            File.Delete(evtadd_file_path);



            var evtupd_file_path = Path.Combine(domain_dir, "Events", "TemplateUpdatedEvent.cs");
            var evtupd_text = File.ReadAllText(evtupd_file_path);
            var new_evtupd_text = evtupd_text.Replace("{{Template}}", entityName)
                                        .Replace("{{TableName}}", tableName)
                                        .Replace("{{Namespace1}}", namespace1)
                                        .Replace("{{Namespace2}}", namespace2);

            var builder_upd_evt = new StringBuilder(new_evtupd_text);

            foreach (var col in columns)
            {
                if (IgnoreColums.Contains(col.Name))
                {
                    continue;
                }

                builder_upd_evt.AppendLine($"        /// <summary>");
                builder_upd_evt.AppendLine($"        /// {col.Description}");
                builder_upd_evt.AppendLine($"        /// </summary>");

                var typeName = GetTypeName(col.DataType, true);

                builder_upd_evt.AppendLine($"        public {typeName} {col.Name} {{ get; set; }}");
                builder_upd_evt.AppendLine();
            }

            builder_upd_evt.AppendLine("    }");
            builder_upd_evt.AppendLine("}");

            var new_evtupd_file_path = Path.Combine(domain_dir, "Events", $"{entityName}UpdatedEvent.cs");
            File.WriteAllText(new_evtupd_file_path, builder_upd_evt.ToString());
            File.Delete(evtupd_file_path);



            var rmvevt_file_path = Path.Combine(domain_dir, "Events", "TemplateRemovedEvent.cs");
            var rmvevt_text = File.ReadAllText(rmvevt_file_path);
            var new_rmvevt_text = rmvevt_text.Replace("{{Template}}", entityName)
                                        .Replace("{{TableName}}", tableName)
                                        .Replace("{{Namespace1}}", namespace1)
                                        .Replace("{{Namespace2}}", namespace2);

            var new_rmvevt_file_path = Path.Combine(domain_dir, "Events", $"{entityName}RemovedEvent.cs");
            File.WriteAllText(new_rmvevt_file_path, new_rmvevt_text);
            File.Delete(rmvevt_file_path);

            #endregion

            #region 替换 Validations

            var addvld_file_path = Path.Combine(domain_dir, "Validations", "AddTemplateCommandValidation.cs");
            var addvld_text = File.ReadAllText(addvld_file_path);
            var new_addvld_text = addvld_text.Replace("{{Template}}", entityName)
                                        .Replace("{{TableName}}", tableName)
                                        .Replace("{{Namespace1}}", namespace1)
                                        .Replace("{{Namespace2}}", namespace2);

            var new_addvld_file_path = Path.Combine(domain_dir, "Validations", $"Add{entityName}CommandValidation.cs");
            File.WriteAllText(new_addvld_file_path, new_addvld_text);
            File.Delete(addvld_file_path);

            var updvld_file_path = Path.Combine(domain_dir, "Validations", "UpdateTemplateCommandValidation.cs");
            var updvld_text = File.ReadAllText(updvld_file_path);
            var new_updvld_text = updvld_text.Replace("{{Template}}", entityName)
                                        .Replace("{{TableName}}", tableName)
                                        .Replace("{{Namespace1}}", namespace1)
                                        .Replace("{{Namespace2}}", namespace2);

            var new_updvld_file_path = Path.Combine(domain_dir, "Validations", $"Update{entityName}CommandValidation.cs");
            File.WriteAllText(new_updvld_file_path, new_updvld_text);
            File.Delete(updvld_file_path);

            #endregion

            #endregion

            #region 替换 Applications

            #region 替换 Dtos

            var dto_file_path = Path.Combine(application_dir, "Dtos", "TemplateDto.cs");
            var dto_text = File.ReadAllText(dto_file_path);
            var new_dto_text = dto_text.Replace("{{Template}}", entityName)
                                        .Replace("{{TableName}}", tableName)
                                        .Replace("{{Namespace1}}", namespace1)
                                        .Replace("{{Namespace2}}", namespace2)
                                        .Replace("{{ENTITY_NAME_CN}}", entityNameCN);

            var builder_dto = new StringBuilder(new_dto_text);

            foreach (var col in columns)
            {
                if (IgnoreColums.Contains(col.Name))
                {
                    continue;
                }

                builder_dto.AppendLine($"        /// <summary>");
                builder_dto.AppendLine($"        /// {col.Description}");
                builder_dto.AppendLine($"        /// </summary>");

                if (!col.Nullable)
                {
                    builder_dto.AppendLine($"        [Required(ErrorMessage = \"必填项\")]");
                }

                if (col.DataType == "character varying" && col.Length.HasValue)
                {
                    builder_dto.AppendLine($"        [MaxLength({col.Length}, ErrorMessage = \"{col.Description}最长{col.Length}个字符\")]");
                }

                var typeName = GetTypeName(col.DataType, col.Nullable);

                builder_dto.AppendLine($"        public {typeName} {col.Name} {{ get; set; }}");
                builder_dto.AppendLine();
            }

            builder_dto.AppendLine("    }");
            builder_dto.AppendLine("}");

            var new_dto_file_path = Path.Combine(application_dir, "Dtos", $"{entityName}Dto.cs");
            File.WriteAllText(new_dto_file_path, builder_dto.ToString());
            File.Delete(dto_file_path);


            #endregion

            #region 替换 Inputs

            var add_file_path = Path.Combine(application_dir, "Dtos", "TemplateAddInput.cs");
            var add_text = File.ReadAllText(add_file_path);
            var new_add_text = add_text.Replace("{{Template}}", entityName)
                                        .Replace("{{TableName}}", tableName)
                                        .Replace("{{Namespace1}}", namespace1)
                                        .Replace("{{Namespace2}}", namespace2)
                                        .Replace("{{ENTITY_NAME_CN}}", entityNameCN);

            var builder_add = new StringBuilder(new_add_text);

            foreach (var col in columns)
            {
                if (IgnoreColums.Contains(col.Name))
                {
                    continue;
                }

                builder_add.AppendLine($"        /// <summary>");
                builder_add.AppendLine($"        /// {col.Description}");
                builder_add.AppendLine($"        /// </summary>");

                if (!col.Nullable)
                {
                    builder_add.AppendLine($"        [Required(ErrorMessage = \"必填项\")]");
                }

                if (col.DataType == "character varying" && col.Length.HasValue)
                {
                    builder_add.AppendLine($"        [MaxLength({col.Length}, ErrorMessage = \"{col.Description}最长{col.Length}个字符\")]");
                }

                var typeName = GetTypeName(col.DataType, col.Nullable);
                if (!col.Nullable && !typeName.EndsWith("?"))
                {
                    typeName += "?";
                }

                builder_add.AppendLine($"        public {typeName} {col.Name} {{ get; set; }}");
                builder_add.AppendLine();
            }

            builder_add.AppendLine("        /// <summary>");
            builder_add.AppendLine("        /// 映射为Command");
            builder_add.AppendLine("        /// </summary>");
            builder_add.AppendLine("        /// <param name=\"mapper\"></param>");
            builder_add.AppendLine("        /// <returns></returns>");
            builder_add.AppendLine("        public Command Map(IMapper mapper)");
            builder_add.AppendLine("        {");
            builder_add.AppendLine($"            return mapper.Map<Add{entityName}Command>(this);");
            builder_add.AppendLine("        }");
            builder_add.AppendLine("    }");
            builder_add.AppendLine("}");

            var new_add_file_path = Path.Combine(application_dir, "Dtos", $"{entityName}AddInput.cs");
            File.WriteAllText(new_add_file_path, builder_add.ToString());
            File.Delete(add_file_path);



            var upd_file_path = Path.Combine(application_dir, "Dtos", "TemplateUpdateInput.cs");
            var upd_text = File.ReadAllText(upd_file_path);
            var new_upd_text = upd_text.Replace("{{Template}}", entityName)
                                        .Replace("{{TableName}}", tableName)
                                        .Replace("{{Namespace1}}", namespace1)
                                        .Replace("{{Namespace2}}", namespace2)
                                        .Replace("{{ENTITY_NAME_CN}}", entityNameCN);

            var builder_upd = new StringBuilder(new_upd_text);

            foreach (var col in columns)
            {
                if (IgnoreColums.Contains(col.Name))
                {
                    continue;
                }

                builder_upd.AppendLine($"        /// <summary>");
                builder_upd.AppendLine($"        /// {col.Description}");
                builder_upd.AppendLine($"        /// </summary>");

                if (col.DataType == "character varying" && col.Length.HasValue)
                {
                    builder_upd.AppendLine($"        [MaxLength({col.Length}, ErrorMessage = \"{col.Description}最长{col.Length}个字符\")]");
                }

                var typeName = GetTypeName(col.DataType, true);

                builder_upd.AppendLine($"        public {typeName} {col.Name} {{ get; set; }}");
                builder_upd.AppendLine();
            }

            builder_upd.AppendLine("        /// <summary>");
            builder_upd.AppendLine("        /// 映射为Command");
            builder_upd.AppendLine("        /// </summary>");
            builder_upd.AppendLine("        /// <param name=\"mapper\"></param>");
            builder_upd.AppendLine("        /// <returns></returns>");
            builder_upd.AppendLine("        public Command Map(IMapper mapper)");
            builder_upd.AppendLine("        {");
            builder_upd.AppendLine($"            return mapper.Map<Update{entityName}Command>(this);");
            builder_upd.AppendLine("        }");
            builder_upd.AppendLine("    }");
            builder_upd.AppendLine("}");

            var new_upd_file_path = Path.Combine(application_dir, "Dtos", $"{entityName}UpdateInput.cs");
            File.WriteAllText(new_upd_file_path, builder_upd.ToString());
            File.Delete(upd_file_path);



            var rmv_file_path = Path.Combine(application_dir, "Dtos", "TemplateRemoveInput.cs");
            var rmv_text = File.ReadAllText(rmv_file_path);
            var new_rmv_text = rmv_text.Replace("{{Template}}", entityName)
                                        .Replace("{{TableName}}", tableName)
                                        .Replace("{{Namespace1}}", namespace1)
                                        .Replace("{{Namespace2}}", namespace2)
                                        .Replace("{{ENTITY_NAME_CN}}", entityNameCN);

            var new_rmv_file_path = Path.Combine(application_dir, "Dtos", $"{entityName}RemoveInput.cs");
            File.WriteAllText(new_rmv_file_path, new_rmv_text);
            File.Delete(rmv_file_path);

            #endregion

            #region 替换 Services

            var svc_file_path = Path.Combine(application_dir, "Services", "TemplateAppService.cs");
            var svc_text = File.ReadAllText(svc_file_path);
            var new_svc_text = svc_text.Replace("{{Template}}", entityName)
                                        .Replace("{{TableName}}", tableName)
                                        .Replace("{{Namespace1}}", namespace1)
                                        .Replace("{{Namespace2}}", namespace2)
                                        .Replace("{{ENTITY_NAME_CN}}", entityNameCN);

            var new_svc_file_path = Path.Combine(application_dir, "Services", $"{entityName}AppService.cs");
            File.WriteAllText(new_svc_file_path, new_svc_text);
            File.Delete(svc_file_path);

            var isvc_file_path = Path.Combine(application_dir, "Services", "ITemplateAppService.cs");
            var isvc_text = File.ReadAllText(isvc_file_path);
            var new_isvc_text = isvc_text.Replace("{{Template}}", entityName)
                                        .Replace("{{TableName}}", tableName)
                                        .Replace("{{Namespace1}}", namespace1)
                                        .Replace("{{Namespace2}}", namespace2)
                                        .Replace("{{ENTITY_NAME_CN}}", entityNameCN);

            var new_isvc_file_path = Path.Combine(application_dir, "Services", $"I{entityName}AppService.cs");
            File.WriteAllText(new_isvc_file_path, new_isvc_text);
            File.Delete(isvc_file_path);

            #endregion

            #region 替换 Filters

            var filter_file_path = Path.Combine(application_dir, "Filters", "TemplateQueryFilter.cs");
            var filter_text = File.ReadAllText(filter_file_path);
            var new_filter_text = filter_text.Replace("{{Template}}", entityName)
                                        .Replace("{{TableName}}", tableName)
                                        .Replace("{{Namespace1}}", namespace1)
                                        .Replace("{{Namespace2}}", namespace2)
                                        .Replace("{{ENTITY_NAME_CN}}", entityNameCN);

            var builder_filter_p = new StringBuilder();
            var builder_filter_e = new StringBuilder();

            foreach (var col in columns)
            {
                if (IgnoreColums.Contains(col.Name))
                {
                    continue;
                }

                if (col.DataType == "ARRAY" || col.DataType == "text" || col.DataType == "bytea")
                {
                    continue;
                }

                builder_filter_p.AppendLine($"        /// <summary>");
                builder_filter_p.AppendLine($"        /// {col.Description}");
                builder_filter_p.AppendLine($"        /// </summary>");

                var typeName = GetTypeName(col.DataType, true);

                builder_filter_p.AppendLine($"        public {typeName} {col.Name} {{ get; set; }}");
                builder_filter_p.AppendLine();

                if (col.DataType == "character varying")
                {
                    builder_filter_e.AppendLine($"            if (!string.IsNullOrWhiteSpace({col.Name}))");
                }
                else
                {
                    builder_filter_e.AppendLine($"            if ({col.Name}.HasValue)");
                }

                builder_filter_e.AppendLine("            {");
                builder_filter_e.AppendLine($"                expression = expression.And(s => {col.Name}.Equals(s.{col.Name}));");
                builder_filter_e.AppendLine("            }");
                builder_filter_e.AppendLine();
            }

            new_filter_text = new_filter_text.Replace("{{PROPERTIES}}", builder_filter_p.ToString());
            new_filter_text = new_filter_text.Replace("{{EXPRESSION}}", builder_filter_e.ToString());

            var new_filter_file_path = Path.Combine(application_dir, "Filters", $"{entityName}QueryFilter.cs");
            File.WriteAllText(new_filter_file_path, new_filter_text);
            File.Delete(filter_file_path);


            #endregion

            #region 替换 Controllers

            var ctl_file_path = Path.Combine(webapi_dir, "Controllers", "TemplateController.cs");
            var ctl_text = File.ReadAllText(ctl_file_path);
            var new_ctl_text = ctl_text.Replace("{{Template}}", entityName)
                                        .Replace("{{TableName}}", tableName)
                                        .Replace("{{Namespace1}}", namespace1)
                                        .Replace("{{Namespace2}}", namespace2)
                                        .Replace("{{template}}", ToCamel(entityName))
                                        .Replace("{{ENTITY_NAME_CN}}", entityNameCN);


            var new_ctl_file_path = Path.Combine(webapi_dir, "Controllers", $"{entityName}Controller.cs");
            File.WriteAllText(new_ctl_file_path, new_ctl_text.ToString());
            File.Delete(ctl_file_path);

            #endregion

            #region 其他

            rbRemark.AppendText($"CreateMap<{entityName}, {entityName}Dto>();" + Environment.NewLine);
            rbRemark.AppendText($"CreateMap<{entityName}Dto, {entityName}>();" + Environment.NewLine);
            
            rbRemark.AppendText(Environment.NewLine);

            rbRemark.AppendText($"CreateMap<{entityName}AddInput, Add{entityName}Command>();" + Environment.NewLine);
            rbRemark.AppendText($"CreateMap<{entityName}RemoveInput, Remove{entityName}Command>();" + Environment.NewLine);
            rbRemark.AppendText($"CreateMap<{entityName}UpdateInput, Update{entityName}Command>();" + Environment.NewLine);

            rbRemark.AppendText(Environment.NewLine);

            rbRemark.AppendText($"services.AddScoped<I{entityName}AppService, {entityName}AppService>();" + Environment.NewLine);

            rbRemark.AppendText(Environment.NewLine);

            rbRemark.AppendText($"services.AddScoped<IRequestHandler<Add{entityName}Command, ValidationResult>, {entityName}CommandHandler>();" + Environment.NewLine);
            rbRemark.AppendText($"services.AddScoped<IRequestHandler<Update{entityName}Command, ValidationResult>, {entityName}CommandHandler>();" + Environment.NewLine);
            rbRemark.AppendText($"services.AddScoped<IRequestHandler<Remove{entityName}Command, ValidationResult>, {entityName}CommandHandler>();" + Environment.NewLine);

            rbRemark.AppendText(Environment.NewLine);

            rbRemark.AppendText($"services.AddScoped<INotificationHandler<{entityName}AddedEvent>, {entityName}EventHandler>();" + Environment.NewLine);
            rbRemark.AppendText($"services.AddScoped<INotificationHandler<{entityName}UpdatedEvent>, {entityName}EventHandler>();" + Environment.NewLine);
            rbRemark.AppendText($"services.AddScoped<INotificationHandler<{entityName}RemovedEvent>, {entityName}EventHandler>();" + Environment.NewLine);

            rbRemark.AppendText(Environment.NewLine);

            rbRemark.AppendText($"new OpenApiTag {{ Name = \"{entityName}\", Description = \"{entityNameCN}接口\" }},");

            #endregion


            #endregion

            MessageBox.Show("成功生成代码");
        }

        private string GetTypeName(string dataType, bool nullable)
        {
            var typeName = "string";
            switch (dataType)
            {
                case "uuid":
                    typeName = "Guid";
                    break;
                case "character varying":
                    typeName = "string";
                    break;
                case "integer":
                    typeName = "int";
                    break;
                case "bigint":
                    typeName = "long";
                    break;
                case "date":
                    typeName = "DateOnly";
                    break;
                case "timestamp with time zone":
                    typeName = "DateTime";
                    break;
                case "boolean":
                    typeName = "bool";
                    break;
                case "text":
                    typeName = "string";
                    break;
                case "ARRAY":
                    typeName = "string[]";
                    break;
                case "numeric":
                    typeName = "decimal";
                    break;
                case "bytea":
                    typeName = "byte[]";
                    break;
                default:
                    typeName = "string";
                    break;
            }

            if (nullable && dataType != "bytea" && dataType != "ARRAY")
            {
                typeName += "?";
            }

            return typeName;
        }

        private void CopyDirectory(string sourceDir, string targetDir)
        {
            DirectoryInfo dir = new DirectoryInfo(sourceDir);
            DirectoryInfo[] dirs = dir.GetDirectories();

            // If the source directory does not exist, throw an exception.
            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException($"Source directory does not exist or could not be found: {sourceDir}");
            }

            // If the destination directory does not exist, create it.
            if (!Directory.Exists(targetDir))
            {
                Directory.CreateDirectory(targetDir);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string tempPath = Path.Combine(targetDir, file.Name);
                file.CopyTo(tempPath, true);
            }

            // If copying subdirectories, copy them and their contents to the new location.
            foreach (DirectoryInfo subdir in dirs)
            {
                string tempPath = Path.Combine(targetDir, subdir.Name);
                CopyDirectory(subdir.FullName, tempPath);
            }
        }

        private string ToCamel(string name)
        {
            var header = name.Substring(0, 1).ToLower();

            return header + name.Substring(1);
        }
    }

    internal class Column
    {
        public string Name { get; set; }
        public bool Nullable { get; set; }
        public string DataType { get; set; }
        public int? Length { get; set; }
        public string Description { get; set; }
    }
}
