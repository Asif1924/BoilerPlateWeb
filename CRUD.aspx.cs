using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using BoilerPlateWeb.Models;

namespace BoilerPlateWeb
{
    public partial class CRUD : System.Web.UI.Page
    {
        string tableName = "";
        Dictionary<string, int> textFieldLengths = new Dictionary<string, int>();
        List<string> hiddenFields = new List<string>();
        List<string> fieldNames = new List<string>();
        List<Type> fieldTypes = new List<Type>();
        Dictionary<string, Type> sqlCSharpTypes = new Dictionary<string, Type>();
        List<string> lookupFields = new List<string>();
        List<string> lookupTables = new List<string>();
        List<string> lookupReferenceFields = new List<string>();
        Dictionary<string, DataTable> lookupDataTables = new Dictionary<string, DataTable>();
        Dictionary<string, Type> fieldNameTypes = new Dictionary<string, Type>();
        List<DeleteChildInfo> deleteChildInfos = new List<DeleteChildInfo>();
        List<LookupTableField> lookupTableFields = new List<LookupTableField>();

        protected void Page_Load(object sender, EventArgs e)
        {
            DoUniversal();
            if (!IsPostBack)
            {
                DoFreshLoad();
            }
        }

        private void DoUniversal()
        {
            tableName = Request.QueryString["table"];
            GetDeleteInfos();
            GetHiddenFields();
            GetLookupFields();
            GetLookupTables();
            GetLookupTableFields();
            GetSqlCSharpTypes();
            GetFieldNamesAndTypes();

            alertL.Text = "";

            LoadData();
            AddNewControls();
        }

        private void GetLookupTableFields()
        {
            if (Request.QueryString["lookuptable"] != null)
            {
                var lookupTableFields = Request.QueryString["lookuptable"].Split(',');

                foreach (var lookupTableField in lookupTableFields)
                {
                    this.lookupTableFields.Add(new LookupTableField(lookupTableField));
                }
            }
        }

        private void GetDeleteInfos()
        {
            if (Request.QueryString["deletechildinfo"] != null)
            {
                var deleteChildInfosS = Request.QueryString["deletechildinfo"].Split(',');
                foreach (var deleteChildInfoS in deleteChildInfosS)
                {
                    deleteChildInfos.Add(new DeleteChildInfo(deleteChildInfoS));
                }
            }
        }

        private void GetLookupTables()
        {
            for (int i = 0; i < lookupFields.Count; i++)
            {
                var lookupField = lookupFields[i];
                var lookupTable = lookupTables[i];
                var lookupReferenceField = lookupReferenceFields[i];

                var sql = @"select
                                {referencefield}
                            from
                                {lookuptable}
                            order by
                                {referencefield}";
                sql = sql.Replace("{referencefield}", lookupReferenceField);
                sql = sql.Replace("{lookuptable}", lookupTable);

                var ds = SqlDB.GetData(sql);

                DataTable table = new DataTable();

                if (DBHelper.HasData(ds))
                {
                    table = ds.Tables[0];
                }

                lookupDataTables.Add(lookupField, table);
            }
        }

        private void GetSqlCSharpTypes()
        {
            sqlCSharpTypes.Add("varchar", typeof(string));
            sqlCSharpTypes.Add("bit", typeof(bool));
            sqlCSharpTypes.Add("datetime", typeof(DateTime));
            sqlCSharpTypes.Add("int", typeof(int));
            sqlCSharpTypes.Add("decimal", typeof(decimal));
            sqlCSharpTypes.Add("real", typeof(double));
            sqlCSharpTypes.Add("float", typeof(float));
            sqlCSharpTypes.Add("date", typeof(string));
            sqlCSharpTypes.Add("time", typeof(string));
        }

        private void GetFieldNamesAndTypes()
        {
            fieldNames.Add(null);
            fieldTypes.Add(null);

            string sql = @" select
                                column_name,
                                data_type
                            from
                                information_schema.columns
                            where
                                table_name = '{tablename}'
                            order by
                                ORDINAL_POSITION";
            sql = sql.Replace("{tablename}", tableName);

            var ds = SqlDB.GetData(sql);

            if (DBHelper.HasData(ds))
            {
                var table = ds.Tables[0];

                foreach (DataRow row in table.Rows)
                {
                    string fieldName = row[0].ToString();
                    Type dataType = sqlCSharpTypes[row[1].ToString()];

                    fieldNames.Add(fieldName);
                    fieldTypes.Add(dataType);
                    fieldNameTypes.Add(fieldName, dataType);
                }
            }
        }

        private void GetLookupFields()
        {
            if (Request.QueryString["lookup"] != null)
            {
                var lookupFieldInfos = Request.QueryString["lookup"].Split(',');
                foreach (var lookupFieldInfo in lookupFieldInfos)
                {
                    var lookupFieldInfosComponents = lookupFieldInfo.Split('-');

                    lookupFields.Add(lookupFieldInfosComponents[0]);
                    lookupTables.Add(lookupFieldInfosComponents[1]);
                    lookupReferenceFields.Add(lookupFieldInfosComponents[2]);
                }
            }
        }

        private void GetHiddenFields()
        {
            if (Request.QueryString["hidden"] != null)
            {
                var hiddenFieldsS = Request.QueryString["hidden"];
                hiddenFields.AddRange(hiddenFieldsS.Split(','));
            }
        }

        private void AddNewControls()
        {
            TableRow tableRow = new TableRow();
            addNewTbl.Rows.Add(tableRow);

            for (int columnIndex = 2; columnIndex < fieldNames.Count; columnIndex++)
            {
                var columnName = fieldNames[columnIndex];

                if (hiddenFields.FirstOrDefault(m => m.ToLower() == columnName.ToLower()) == null)
                {
                    var type = fieldTypes[columnIndex];

                    TableCell tableCell = new TableCell();
                    tableCell.CssClass = "TableCell";
                    tableRow.Cells.Add(tableCell);

                    Label label = new Label();
                    label.ID = addNewTbl.ID + "|" + columnName + "|L";
                    label.Text = columnName;
                    tableCell.Controls.Add(label);

                    Literal literal = new Literal();
                    literal.ID = addNewTbl.ID + "|" + columnName + "|Ltl";
                    literal.Text = "<br />";
                    tableCell.Controls.Add(literal);

                    var lookupField = lookupFields
                                            .FirstOrDefault(m => m.ToLower() == columnName.ToLower());

                    if (lookupField != null)
                    {
                        DropDownList ddl = new DropDownList();
                        var lookupTable = lookupDataTables[columnName];
                        var lookupReferenceField = lookupTable.Columns[0].ColumnName;
                        ddl.DataSource = lookupTable;
                        ddl.DataTextField = lookupReferenceField;
                        ddl.DataValueField = lookupReferenceField;
                        ddl.DataBind();
                        ddl.ID = addNewTbl.ID + "|" + columnName + "|DDL";

                        var lookupTableField = lookupTableFields
                                            .FirstOrDefault(m => m.lookupTable.ToLower() == columnName.ToLower());

                        if (lookupTableField != null)
                        {
                            ddl.SelectedIndexChanged += NewDDL_SelectedIndexChanged;
                            lookupTableField.triggeringDDL = ddl;
                            ddl.AutoPostBack = true;
                        }

                        tableCell.Controls.Add(ddl);
                    }
                    else
                    {
                        var lookupTableField = lookupTableFields
                                            .FirstOrDefault(m => m.field.ToLower() == columnName.ToLower());

                        if (lookupTableField != null)
                        {
                            var sql = @"select distinct
                                            [{fieldname}]
                                        from
                                            [{tablename}]
                                        order by
                                            1";
                            sql = sql.Replace("{fieldname}", lookupTableField.lookupField);
                            sql = sql.Replace("{tablename}", lookupTableField.triggeringDDL.Text);

                            var ds = SqlDB.GetData(sql);
                            lookupTableField.dropDownList.ID = addNewTbl.ID + "|" + columnName + "|DDL";

                            if (DBHelper.HasData(ds))
                            {
                                var table = ds.Tables[0];

                                lookupTableField.dropDownList.DataSource = table;
                                lookupTableField.dropDownList.DataTextField = lookupTableField.lookupField;
                                lookupTableField.dropDownList.DataTextField = lookupTableField.lookupField;
                                lookupTableField.dropDownList.DataBind();
                            }

                            tableCell.Controls.Add(lookupTableField.dropDownList);
                        }
                        else
                        { 
                            if (type == typeof(string)
                            || type == typeof(float)
                            || type == typeof(int)
                            || type == typeof(long)
                            || type == typeof(decimal)
                            || type == typeof(double)
                            || type == typeof(DateTime))
                            {
                                TextBox textbox = new TextBox();
                                textbox.ID = addNewTbl.ID + "|" + columnName + "|TB";

                                if (textFieldLengths.ContainsKey(columnName))
                                {
                                    if (textFieldLengths[columnName] > 64)
                                    {
                                        textbox.TextMode = TextBoxMode.MultiLine;
                                    }
                                }

                                tableCell.Controls.Add(textbox);
                            }
                            else if (type == typeof(bool))
                            {
                                CheckBox checkbox = new CheckBox();
                                checkbox.ID = addNewTbl.ID + "|" + columnName + "|CB";

                                tableCell.Controls.Add(checkbox);
                            }
                        }
                    }
                }
            }
        }

        private void NewDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            DropDownList triggeringDDL = (DropDownList)sender;
            string triggeringColumnName = "";

            triggeringColumnName = triggeringDDL.ID.Split('|')[1];

            var lookupTableField = lookupTableFields
                                    .FirstOrDefault(m => m.lookupTable.ToLower() == triggeringColumnName.ToLower());

            var sql = @"select distinct
                            [{fieldname}]
                        from
                            [{tablename}]
                        order by
                            1";
            sql = sql.Replace("{fieldname}", lookupTableField.lookupField);
            sql = sql.Replace("{tablename}", triggeringDDL.Text);

            var ds = SqlDB.GetData(sql);
            if (DBHelper.HasData(ds))
            {
                var table = ds.Tables[0];

                lookupTableField.dropDownList.DataSource = table;
                lookupTableField.dropDownList.DataTextField = lookupTableField.lookupField;
                lookupTableField.dropDownList.DataValueField = lookupTableField.lookupField;
                lookupTableField.dropDownList.DataBind();
            }
            else
            {
                lookupTableField.dropDownList.Items.Clear();
            }
        }

        private void EditDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            DropDownList triggeringDDL = (DropDownList)sender;
            string triggeringColumnName = "";

            var triggeringDDLIDComponents = triggeringDDL.ID.Split('|');

            triggeringColumnName = triggeringDDLIDComponents[0];
            var id = triggeringDDLIDComponents[2];

            var lookupTableField = lookupTableFields
                                    .FirstOrDefault(m => m.lookupTable.ToLower() == triggeringColumnName.ToLower());

            var triggeredDDLID = (lookupTableField.field + "|DDL|" + id).ToLower();

            var sql = @"select distinct
                            [{fieldname}]
                        from
                            [{tablename}]
                        order by
                            1";
            sql = sql.Replace("{fieldname}", lookupTableField.lookupField);
            sql = sql.Replace("{tablename}", triggeringDDL.Text);

            var ds = SqlDB.GetData(sql);

            lookupTableField.editIDDropDownLists[triggeredDDLID].DataSource = null;
            lookupTableField.editIDDropDownLists[triggeredDDLID].DataBind();

            if (DBHelper.HasData(ds))
            {
                var table = ds.Tables[0];

                lookupTableField.editIDDropDownLists[triggeredDDLID].DataSource = table;
                lookupTableField.editIDDropDownLists[triggeredDDLID].DataTextField = lookupTableField.lookupField;
                lookupTableField.editIDDropDownLists[triggeredDDLID].DataValueField = lookupTableField.lookupField;
                lookupTableField.editIDDropDownLists[triggeredDDLID].DataBind();
            }
            else
            {
                lookupTableField.editIDDropDownLists[triggeredDDLID].Items.Clear();
            }

            Session.Add("ASL_ENTERPRISE_EDITING_NOTIFICATION_ENTITY_ID", id);
            Edit(id);
        }

        private void DoFreshLoad()
        {
            Session.Remove("ASL_ENTERPRISE_EDITING_NOTIFICATION_ENTITY_ID");
        }

        private void LoadData()
        {
            gridview.DataSource = null;
            gridview.DataBind();
            gridview.Columns.Clear();

            var ds = SqlDB.GetData("select '', * from [" + tableName + "]");

            if (DBHelper.HasData(ds))
            {
                var table = ds.Tables[0];

                gridview.DataSource = table;

                gridview.DataBind();

                gridview.HeaderRow.Cells[0].Text = "";
                gridview.HeaderRow.Cells[1].Visible = false;

                for (int i = 2; i < fieldNames.Count; i++)
                {
                    var hiddenField = hiddenFields
                                        .FirstOrDefault(m => m.ToLower() == fieldNames[i].ToLower());

                    if (hiddenField != null)
                    {
                        gridview.HeaderRow.Cells[i].Visible = false;
                    }
                }


                AddControls();
            }
            if (Session["ASL_ENTERPRISE_EDITING_NOTIFICATION_ENTITY_ID"] != null)
            {
                var idS = (string)Session["ASL_ENTERPRISE_EDITING_NOTIFICATION_ENTITY_ID"];
                Edit(idS);
            }
        }

        private void AddControls()
        {
            var table = (DataTable)gridview.DataSource;

            var ds = SqlDB.GetData("select column_name, CHARACTER_MAXIMUM_LENGTH from information_schema.columns where table_name = '" + tableName + "' and data_type = 'varchar'");
            if (textFieldLengths.Count == 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    textFieldLengths.Add(row[0].ToString(), int.Parse(row[1].ToString()));
                }
            }

            foreach (GridViewRow row in gridview.Rows)
            {
                var id = row.Cells[1].Text;
                row.Cells[1].Visible = false;

                ImageButton deleteIB = new ImageButton();
                deleteIB.ID = "deleteIB" + id;
                deleteIB.CommandArgument = id;
                deleteIB.ImageUrl = "Images/delete.png";
                deleteIB.Click += DeleteIB_Click;

                ImageButton editIB = new ImageButton();
                editIB.ID = "editIB" + id;
                editIB.CommandArgument = id;
                editIB.ImageUrl = "Images/edit.jpg";
                editIB.Click += EditIB_Click;

                ImageButton cancelIB = new ImageButton();
                cancelIB.ID = "cancelIB" + id;
                cancelIB.CommandArgument = id;
                cancelIB.ImageUrl = "Images/cancel.jpg";
                cancelIB.Click += CancelIB_Click;
                cancelIB.Visible = false;

                ImageButton saveIB = new ImageButton();
                saveIB.ID = "saveIB" + id;
                saveIB.CommandArgument = id;
                saveIB.ImageUrl = "Images/save.jpg";
                saveIB.Click += SaveIB_Click;
                saveIB.Visible = false;

                row.Cells[0].Controls.Add(deleteIB);
                row.Cells[0].Controls.Add(editIB);
                row.Cells[0].Controls.Add(cancelIB);
                row.Cells[0].Controls.Add(saveIB);
                row.Cells[0].Wrap = false;

                for (int columnIndex = 2; columnIndex < table.Columns.Count; columnIndex++)
                {
                    row.Cells[columnIndex].CssClass = "TableCell";

                    var columnName = table.Columns[columnIndex].ColumnName;
                    
                    if (hiddenFields.FirstOrDefault(m => m.ToLower() == columnName.ToLower()) != null)
                    {
                        row.Cells[columnIndex].Visible = false;
                    }
                    else
                    {
                        var lookupField = lookupFields
                                            .FirstOrDefault(m => m.ToLower() == columnName.ToLower());

                        if (lookupField != null)
                        {
                            DropDownList ddl = new DropDownList();
                            var lookupTable = lookupDataTables[columnName];
                            var lookupReferenceField = lookupTable.Columns[0].ColumnName;
                            ddl.DataSource = lookupTable;
                            ddl.DataTextField = lookupReferenceField;
                            ddl.DataValueField = lookupReferenceField;
                            ddl.DataBind();
                            ddl.ID = columnName + "|DDL|" + id;
                            ddl.Text = row.Cells[columnIndex].Text;
                            ddl.Enabled = false;

                            var lookupTableField = lookupTableFields
                                            .FirstOrDefault(m => m.lookupTable.ToLower() == columnName.ToLower());

                            if (lookupTableField != null)
                            {
                                ddl.SelectedIndexChanged += EditDDL_SelectedIndexChanged;
                                lookupTableField.triggeringDDL = ddl;
                                ddl.AutoPostBack = true;
                            }

                            row.Cells[columnIndex].Controls.Add(ddl);
                        }
                        else
                        {
                            var lookupTableField = this.lookupTableFields
                                                        .FirstOrDefault(m => m.field.ToLower() == columnName.ToLower());

                            if (lookupTableField != null)
                            {
                                var sql = @"select distinct
                                                [{lookupfield}]
                                            from
                                                [{lookuptable}]
                                            order by
                                                1";
                                sql = sql.Replace("{lookupfield}", lookupTableField.lookupField);
                                sql = sql.Replace("{lookuptable}", lookupTableField.triggeringDDL.Text);
                                ds = SqlDB.GetData(sql);
                                DropDownList ddl = new DropDownList();
                                if (DBHelper.HasData(ds))
                                {
                                    ddl.DataSource = ds.Tables[0];
                                    ddl.DataTextField = lookupTableField.lookupField;
                                    ddl.DataValueField = lookupTableField.lookupField;
                                    ddl.DataBind();
                                    ddl.Text = row.Cells[columnIndex].Text;
                                }
                                ddl.ID = (columnName + "|DDL|" + id).ToLower();
                                ddl.Enabled = false;

                                if (!lookupTableField.editIDDropDownLists.ContainsKey(ddl.ID))
                                {
                                    lookupTableField.editIDDropDownLists.Add(ddl.ID, ddl);
                                }

                                row.Cells[columnIndex].Controls.Add(ddl);
                            }
                            else
                            {

                                if (table.Columns[columnIndex].DataType == typeof(string)
                                || table.Columns[columnIndex].DataType == typeof(int)
                                || table.Columns[columnIndex].DataType == typeof(float)
                                || table.Columns[columnIndex].DataType == typeof(double)
                                || table.Columns[columnIndex].DataType == typeof(long)
                                || table.Columns[columnIndex].DataType == typeof(decimal))
                                {
                                    TextBox textBox = new TextBox();
                                    textBox.ID = columnName + "TB" + id;
                                    textBox.Visible = true;
                                    textBox.Text = row.Cells[columnIndex].Text == "&nbsp;" ? "" : row.Cells[columnIndex].Text;
                                    textBox.BorderStyle = BorderStyle.None;
                                    textBox.Enabled = false;

                                    if (textFieldLengths.ContainsKey(columnName))
                                    {
                                        if (textFieldLengths[columnName] > 64)
                                        {
                                            textBox.TextMode = TextBoxMode.MultiLine;
                                            /*
                                            textBox.Wrap = true;
                                            textBox.Width = 450;
                                            textBox.Height = 50;
                                            textBox.CssClass = "NonStretchableTextBox";*/
                                        }
                                    }

                                    row.Cells[columnIndex].Controls.Add(textBox);
                                }
                                else if (table.Columns[columnIndex].DataType == typeof(bool))
                                {
                                    CheckBox checkbox = (CheckBox)row.Cells[columnIndex].Controls[0];
                                    checkbox.ID = columnName + "CB" + id;
                                    checkbox.Visible = true;
                                    checkbox.Enabled = false;
                                }
                                else if (table.Columns[columnIndex].DataType == typeof(DateTime))
                                {
                                    var dt = table.Rows[row.RowIndex][columnIndex];

                                    var text = dt == null || dt == DBNull.Value ? "" : ((DateTime)dt).ToString("yyyy-MM-dd HH:mm:ss");

                                    TextBox textBox = new TextBox();
                                    textBox.ID = columnName + "TB" + id;
                                    textBox.Visible = true;
                                    textBox.Text = row.Cells[columnIndex].Text == "&nbsp;" ? "" : row.Cells[columnIndex].Text;
                                    textBox.BorderStyle = BorderStyle.None;
                                    textBox.Enabled = false;

                                    row.Cells[columnIndex].Controls.Add(textBox);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void SaveIB_Click(object sender, ImageClickEventArgs e)
        {
            Session.Remove("ASL_ENTERPRISE_EDITING_NOTIFICATION_ENTITY_ID");
            try
            {
                var saveIB = (ImageButton)sender;
                var id = saveIB.CommandArgument;

                int rowIndex = GetRowIndex(id);

                var row = gridview.Rows[rowIndex];

                var table = (DataTable)gridview.DataSource;

                string updates = "";
                DataRow dataRow = table.Rows[rowIndex];
                var values = dataRow.ItemArray;

                for (int cellIndex = 2; cellIndex < table.Columns.Count; cellIndex++)
                {
                    var columnName = table.Columns[cellIndex].ColumnName;
                    var o = values[cellIndex];
                    var type = table.Columns[columnName].DataType;

                    string value = "";

                    var hiddenField = hiddenFields
                                        .FirstOrDefault(m => m.ToLower() == columnName.ToLower());

                    if (hiddenField != null)
                    {
                        continue;
                    }

                    var lookupField = lookupFields
                                        .FirstOrDefault(m => m.ToLower() == columnName.ToLower());

                    var lookupTableField = lookupTableFields
                                        .FirstOrDefault(m => m.field.ToLower() == columnName.ToLower());

                    if (lookupField != null || lookupTableField != null)
                    {
                        DropDownList ddl = (DropDownList)row.Cells[cellIndex].Controls[0];
                        ddl.Enabled = true;

                        if (string.IsNullOrEmpty(ddl.Text) || string.IsNullOrWhiteSpace(ddl.Text))
                        {
                            value = "null";
                        }
                        else
                        {
                            if (fieldTypes[cellIndex] == typeof(string)
                                || fieldTypes[cellIndex] == typeof(DateTime))
                            {
                                value = "'" + ddl.Text + "'";
                            }
                            else
                            {
                                value = ddl.Text;
                            }
                        }
                    }
                    else
                    {
                        if (type == typeof(string))
                        {
                            TextBox textbox = (TextBox)row.Cells[cellIndex].Controls[0];
                            if (textbox.Text == null)
                            {
                                value = "null";
                            }
                            else
                            {
                                value = "'" + textbox.Text + "'";
                            }
                        }
                        else if (type == typeof(float)
                                || type == typeof(int)
                                || type == typeof(long)
                                || type == typeof(decimal)
                                || type == typeof(double))
                        {
                            TextBox textbox = (TextBox)row.Cells[cellIndex].Controls[0];
                            if (string.IsNullOrEmpty(textbox.Text) || string.IsNullOrWhiteSpace(textbox.Text))
                            {
                                value = "null";
                            }
                            else
                            {
                                value = textbox.Text;
                            }
                        }
                        else if (type == typeof(bool))
                        {
                            CheckBox checkBox = (CheckBox)row.Cells[cellIndex].Controls[0];
                            if (checkBox.Checked)
                            {
                                value = "1";
                            }
                            else
                            {
                                value = "0";
                            }
                        }
                        else if (type == typeof(DateTime))
                        {
                            TextBox textBox = (TextBox)row.Cells[cellIndex].Controls[0];
                            if (string.IsNullOrEmpty(textBox.Text) || string.IsNullOrWhiteSpace(textBox.Text))
                            {
                                value = "null";
                            }
                            else
                            {
                                value = "'" + textBox.Text + "'";
                            }
                        }
                    }

                    updates += "[" + columnName + "] = " + value + ", ";
                }

                if (updates.Length > 0)
                {
                    updates = updates.Substring(0, updates.Length - 2);
                }

                string sql = @" update 
                                    [{tablename}]
                                set 
                                    {updates}
                                where 
                                    id = {id};";
                sql = sql.Replace("{updates}", updates);
                sql = sql.Replace("{tablename}", tableName);
                sql = sql.Replace("{id}", id);

                SqlDB.Execute(sql);

                LoadData();
            }
            catch (Exception ex)
            {
                alertL.Text = ex.Message;
            }
        }

        private void CancelIB_Click(object sender, ImageClickEventArgs e)
        {
            Session.Remove("ASL_ENTERPRISE_EDITING_NOTIFICATION_ENTITY_ID");
            LoadData();
        }

        private void EditIB_Click(object sender, ImageClickEventArgs e)
        {
            ImageButton editIB = (ImageButton)sender;
            var id = editIB.CommandArgument;

            Session.Add("ASL_ENTERPRISE_EDITING_NOTIFICATION_ENTITY_ID", id);
            Edit(id);
        }

        private void Edit(string id)
        {
            int rowIndex = GetRowIndex(id);

            var row = gridview.Rows[rowIndex];

            ImageButton cancelIB = (ImageButton)row.Cells[0].Controls[2];
            cancelIB.Visible = true;

            ImageButton saveIB = (ImageButton)row.Cells[0].Controls[3];
            saveIB.Visible = true;

            Dictionary<string, int> textFieldLengths = new Dictionary<string, int>();

            var table = (DataTable)gridview.DataSource;

            DataRow dataRow = table.Rows[rowIndex];

            for (int cellIndex = 2; cellIndex < dataRow.ItemArray.Length; cellIndex++)
            {
                object o = dataRow.ItemArray[cellIndex];
                var columnName = table.Columns[cellIndex].ColumnName;
                var type = table.Columns[cellIndex].DataType;

                if (hiddenFields.FirstOrDefault(m => m.ToLower() == columnName.ToLower()) == null)
                {
                    var lookupField = lookupFields
                                        .FirstOrDefault(m => m.ToLower() == columnName.ToLower());

                    var lookupTableField = lookupTableFields
                                        .FirstOrDefault(m => m.field.ToLower() == columnName.ToLower());

                    if (lookupField != null || lookupTableField != null)
                    {
                        if (row.Cells[cellIndex].Controls.Count != 0)
                        {
                            DropDownList ddl = (DropDownList)row.Cells[cellIndex].Controls[0];
                            ddl.Enabled = true;
                        }
                    }
                    else
                    {
                        if (type == typeof(string)
                            || type == typeof(float)
                            || type == typeof(int)
                            || type == typeof(long)
                            || type == typeof(decimal)
                            || type == typeof(double))
                        {
                            TextBox textbox = (TextBox)row.Cells[cellIndex].Controls[0];
                            textbox.Visible = true;
                            textbox.Text = dataRow[cellIndex].ToString();
                            textbox.BorderStyle = BorderStyle.Solid;
                            textbox.Enabled = true;

                            if (textFieldLengths.ContainsKey(columnName))
                            {
                                if (textFieldLengths[columnName] > 64)
                                {

                                }
                            }
                        }
                        else if (type == typeof(DateTime))
                        {
                            TextBox textbox = (TextBox)row.Cells[cellIndex].Controls[0];
                            textbox.Visible = true;
                            textbox.Text = dataRow[cellIndex] == null || dataRow[cellIndex] == DBNull.Value ? "" : ((DateTime)dataRow[cellIndex]).ToString("yyyy-MM-dd HH:mm:ss");
                            textbox.BorderStyle = BorderStyle.Solid;
                            textbox.Enabled = true;

                            if (textFieldLengths.ContainsKey(columnName))
                            {
                                if (textFieldLengths[columnName] > 64)
                                {

                                }
                            }
                        }
                        else if (type == typeof(bool))
                        {
                            CheckBox checkbox = (CheckBox)row.Cells[cellIndex].Controls[0];
                            checkbox.Enabled = true;

                            if (textFieldLengths.ContainsKey(columnName))
                            {
                                if (textFieldLengths[columnName] > 64)
                                {

                                }
                            }
                        }
                    }
                }
            }

            ImageButton deleteIB = (ImageButton)row.Cells[0].Controls[0];
            deleteIB.Visible = false;

            var editIB = (ImageButton)gridview.Rows[rowIndex].Cells[0].Controls[1];
            editIB.Visible = false;
        }

        private int GetRowIndex(string id)
        {
            foreach (GridViewRow currentRow in gridview.Rows)
            {
                if (currentRow.Cells[1].Text == id)
                {
                    return currentRow.RowIndex;
                }
            }

            return -1;
        }

        private void DeleteIB_Click(object sender, ImageClickEventArgs e)
        {
            ImageButton deleteIB = (ImageButton)sender;
            var id = deleteIB.CommandArgument;

            foreach (var deleteChildInfo in deleteChildInfos)
            {
                var childValue = SqlDB.GetData("select [" + deleteChildInfo.parentField + "] from [" + tableName + "] where id = " + id).Tables[0].Rows[0][0].ToString().Replace("'", "''");

                string condition = deleteChildInfo.condition == null ? "" : deleteChildInfo.condition.Replace("'", "''").Replace("__", "=");
                if (condition != "")
                {
                    var conditionComponents = condition.Split('=');
                    condition = "and [" + conditionComponents[0] + "] = '" + conditionComponents[1] + "'";
                }

                string sql = @" delete
                                    {childtablename}
                                where
                                    {childfield} = '{childvalue}'
                                    {condition}";
                sql = sql.Replace("{childtablename}", deleteChildInfo.childTableName);
                sql = sql.Replace("{childfield}", deleteChildInfo.childField);
                sql = sql.Replace("{childvalue}", childValue);
                sql = sql.Replace("{condition}", condition);

                SqlDB.Execute(sql);
            }

            SqlDB.Execute("delete [" + tableName + "] where id = " + id);

            LoadData();
        }

        protected void addB_Click(object sender, EventArgs e)
        {
            Session.Remove("ASL_ENTERPRISE_EDITING_NOTIFICATION_ENTITY_ID");

            try
            {
                string columns = "";
                for (int i = 2; i < fieldNames.Count; i++)
                {
                    var columnName = fieldNames[i];

                    var hiddenField = hiddenFields
                                        .FirstOrDefault(m => m.ToLower() == columnName.ToLower());

                    if (hiddenField == null)
                    {
                        columns += "[" + columnName + "], ";
                    }
                }
                if (columns.Length > 0)
                {
                    columns = columns.Substring(0, columns.Length - 2);
                }
                columns = "(" + columns + ")";

                string values = "";
                foreach (TableCell tableCell in addNewTbl.Rows[0].Cells)
                {
                    var control = tableCell.Controls[2];
                    var columnName = control.ID.Split('|')[1];

                    if (fieldNameTypes[columnName] == typeof(string)
                        || fieldNameTypes[columnName] == typeof(DateTime))
                    {
                        if (control is DropDownList)
                        {
                            DropDownList ddl = (DropDownList)control;
                            values += "'" + ddl.Text.Replace("'", "''") + "', ";
                        }
                        else
                        {
                            TextBox textBox = (TextBox)control;
                            if (string.IsNullOrEmpty(textBox.Text) || string.IsNullOrWhiteSpace(textBox.Text))
                            {
                                values += "null, ";
                            }
                            else
                            {
                                values += "'" + textBox.Text.Replace("'", "''") + "', ";
                            }
                        }
                    }
                    else if (fieldNameTypes[columnName] == typeof(bool))
                    {
                        CheckBox checkBox = (CheckBox)control;
                        if (checkBox.Checked)
                        {
                            values += "1, ";
                        }
                        else
                        {
                            values += "0, ";
                        }
                    }
                    else
                    {

                        if (control is DropDownList)
                        {
                            DropDownList ddl = (DropDownList)control;
                            values += ddl.Text + ", ";
                        }
                        else
                        {
                            TextBox textBox = (TextBox)control;
                            if (string.IsNullOrEmpty(textBox.Text) || string.IsNullOrWhiteSpace(textBox.Text))
                            {
                                values += "null, ";
                            }
                            else
                            {
                                values += textBox.Text + ", ";
                            }
                        }
                    }
                }
                if (values.Length > 0)
                {
                    values = values.Substring(0, values.Length - 2);
                }
                values = "(" + values + ")";

                string sql = @" insert into
                                    {tablename}
                                    {columns}
                                values
                                    {values}";
                sql = sql.Replace("{tablename}", tableName);
                sql = sql.Replace("{columns}", columns);
                sql = sql.Replace("{values}", values);

                SqlDB.Execute(sql);

                LoadData();

                ClearAddNewControls();
            }
            catch (Exception ex)
            {
                alertL.Text = ex.Message;
            }
        }

        private void ClearAddNewControls()
        {
            foreach (TableRow row in addNewTbl.Rows)
            {
                foreach (TableCell cell in row.Cells)
                {
                    var control = cell.Controls[2];
                    if (control is TextBox)
                    {
                        var textBox = (TextBox)control;

                        textBox.Text = null;
                    }
                }
            }
        }
    }
}