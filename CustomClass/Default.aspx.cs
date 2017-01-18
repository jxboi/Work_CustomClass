using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Text;
using System.IO;

namespace CustomClass
{
    public partial class _Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            attributesList = attributes.Split(',');
            classFullName = classType + "_" + className;
        }

        string className = Properties.Settings.Default.ClassName;
        string classType = Properties.Settings.Default.ClassType;
        string attributes = Properties.Settings.Default.Attributes;
        string primaryKey = Properties.Settings.Default.PrimaryKey;
        string[] libraryList = Properties.Settings.Default.Library.Split(',');
        string[] attributesList;
        string[] nameList;

        string classFullName;

        public string AddLibrary()
        {
            StringBuilder sb = new StringBuilder();
            foreach (string item in libraryList)
            {
                sb.Append("using " + item + ";\r\n");
            }

            return sb.ToString();
        }

        public string AddStructure()
        {
            return "\r\nnamespace Data\r\n{\r\npublic class C" + classFullName + " : IDisposable\r\n{\r\n";
        }

        public string AddEnum()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("private enum E" + classFullName + "\r\n{\r\n");
            int count = 0;

            foreach (string item in attributesList)
            {
                string[] data = item.Split(':');
                sb.Append("" + data[0] + " = " + count.ToString() + ",\r\n");
                count++;
            }
            sb.Append("}\r\n\r\n");

            return sb.ToString();
        }

        public string AddAttributes()
        {
            StringBuilder sb1 = new StringBuilder();
            StringBuilder sb2 = new StringBuilder();

            string name = "";
            nameList = new string[attributesList.Length];

            for (int i =0; i< attributesList.Length; i++)
            {
                string[] data = attributesList[i].Split(':');
                switch (data[1].ToLower().Split('(')[0])
                {
                    case "varchar":
                        name = "m_str" + data[0];
                        nameList[i] = name;
                        sb1.Append("private string " + name + ";\r\n");
                        sb2.Append("\r\npublic string " 
                            + data[0] + "\r\n{\r\nget { return "
                            + name + ";}\r\nset { "
                            + name + " = value;}\r\n}\r\n");
                        break;
                    case "datetime":
                        name = "m_dt" + data[0];
                        nameList[i] = name;
                        sb1.Append("private DateTime " + name + ";\r\n");
                        sb2.Append("\r\npublic DateTime "
                            + data[0] + "\r\n{\r\nget { return "
                            + name + ";}\r\nset { "
                            + name + " = value;}\r\n}\r\n");
                        break;
                    case "int":
                        name = "m_int" + data[0];
                        nameList[i] = name;
                        sb1.Append("private int " + name + ";\r\n");
                        sb2.Append("\r\npublic int "
                            + data[0] + "\r\n{\r\nget { return "
                            + name + ";}\r\nset { "
                            + name + " = value;}\r\n}\r\n");
                        break;
                    case "decimal":
                        name = "m_dec" + data[0];
                        nameList[i] = name;
                        sb1.Append("private decimal " + name + ";\r\n");
                        sb2.Append("\r\npublic decimal "
                            + data[0] + "\r\n{\r\nget { return "
                            + name + ";}\r\nset { "
                            + name + " = value;}\r\n}\r\n");
                        break;
                }
            }

            return sb1.ToString() + sb2.ToString();
        }

        public string AddContructor()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("\r\npublic C" + classFullName + "()\r\n{\r\n");
            string name;

            foreach (string item in attributesList)
            {
                string[] data = item.Split(':');
                switch (data[1].ToLower().Split('(')[0])
                {
                    case "string":
                        name = "m_str" + data[0];
                        sb.Append("" + name + " = string.Empty;\r\n");
                        break;
                    case "datetime":
                        name = "m_dt" + data[0];
                        sb.Append("" + name + " = DateTime.Now;\r\n");
                        break;
                    case "int":
                        name = "m_int" + data[0];
                        sb.Append("" + name + " = 0;\r\n");
                        break;
                    case "decimal":
                        name = "m_dec" + data[0];
                        sb.Append("" + name + " = 0;\r\n");
                        break;
                }
            }
            sb.Append("}");

            return sb.ToString();
        }

        public string AddDispose()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("\r\n\r\npublic void Dispose()\r\n{}");

            return sb.ToString();
        }

        public string AddNew()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("\r\n\r\nprotected bool AddNew(ref CConnection p_oConnect)"
                + "\r\n{"
                + "\r\nCConnection l_oInnerCon = null;"
                + "\r\nSqlCommand l_oCommand = null;"
                + "\r\nSqlParameter[] l_oParam = null;"
                + "\r\n"
                + "\r\nstring l_strSQL = string.Empty;"
                + "\r\nbool l_bAddNew = false;"
                + "\r\n"
                + "\r\ntry"
                + "\r\n{"
                + "\r\nif (p_oConnect == null)"
                + "\r\n{"
                + "\r\nl_oInnerCon = new CConnection();"
                + "\r\nl_oInnerCon.Open();"
                + "\r\n}"
                + "\r\nelse"
                + "\r\nl_oInnerCon = p_oConnect;"
                + "\r\n"
                + "\r\nl_strSQL = \"INSERT INTO " + classFullName + "\"+"
                + "\r\n\"(");

            for (int i = 0; i < attributesList.Length; i++)
            {
                string[] data = attributesList[i].Split(':');
                sb.Append(data[0]);
                if (i != attributesList.Length - 1)
                    sb.Append(",");
            }
            sb.Append(")\"+ \r\n\"VALUES (");
            for (int i = 0; i < attributesList.Length; i++)
            {
                string[] data = attributesList[i].Split(':');
                sb.Append("@" + data[0]);
                if (i != attributesList.Length - 1)
                    sb.Append(",");
            }
            sb.Append(")\";\r\n");

            sb.Append("\r\nl_oParam = new SqlParameter[" + attributesList.Length + "];");

            for (int i = 0; i < attributesList.Length; i++)
            {
                string[] data = attributesList[i].Split(':');
                string length;
                switch (data[1].ToLower().Split('(')[0])
                {
                    case "varchar":
                        length = data[1].ToLower().Split('(')[1];
                        sb.Append("\r\nl_oParam[" + i + "] = new SqlParameter(\"@" + data[0] + @""", SqlDbType.VarChar, " + length + ";");
                        break;
                    case "datetime":
                        sb.Append("\r\nl_oParam[" + i + "] = new SqlParameter(\"@" + data[0] + @""", SqlDbType.DateTime);");
                        break;
                    case "int":
                        sb.Append("\r\nl_oParam[" + i + "] = new SqlParameter(\"@" + data[0] + @""", SqlDbType.Int);");
                        break;
                    case "decimal":
                        sb.Append("\r\nl_oParam[" + i + "] = new SqlParameter(\"@" + data[0] + @""", SqlDbType.Decimal);");
                        break;
                }
            }

            sb.Append("\r\n\r\nl_oCommand = new SqlCommand(l_strSQL, l_oInnerCon.Connection);"
                + "\r\nl_oCommand.Transaction = l_oInnerCon.Transaction;");

            for (int i = 0; i < attributesList.Length; i++)
            {
                sb.Append("\r\nl_oCommand.Parameters.Add(l_oParam[" + i + "]).Value = " + nameList[i] + ";");
            }

            sb.Append("\r\n\r\nif (l_oCommand.ExecuteNonQuery() == 1)"
                    + "\r\nl_bAddNew = true;");

            sb.Append("\r\n\r\nif (p_oConnect == null)"
                + "\r\n{"
                + "\r\nl_oInnerCon.Close();"
                + "\r\nl_oInnerCon.Dispose();"
                + "\r\n}");

            sb.Append("\r\n\r\nif (l_oInnerCon != null) l_oInnerCon = null;"
            + "\r\nif (l_oCommand != null) l_oCommand = null;"
            + "\r\nif (l_oParam != null) l_oParam = null;");

            sb.Append("\r\n\r\nreturn l_bAddNew;");
            sb.Append("\r\n}");

            sb.Append("\r\n\r\ncatch (Exception ex)"
            + "\r\n{"
                + "\r\nif (p_oConnect == null)"
                + "\r\n{"
                + "\r\n    l_oInnerCon.Close();"
                + "\r\n    l_oInnerCon.Dispose();"
                + "\r\n}"
                + "\r\nif (l_oInnerCon != null) l_oInnerCon = null;"
                + "\r\nif (l_oCommand != null) l_oCommand = null;"
                + "\r\nif (l_oParam != null) l_oParam = null;"
                + "\r\n\r\nthrow new Exception(ex.Message.ToString());"
                + "\r\n}"
                + "\r\n}");

            return sb.ToString();
        }

        public string AddLoadInstance()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("\r\n\r\nprotected bool LoadInstance(ref CConnection p_oConnect)"
                + "\r\n{"
                + "\r\nCConnection l_oInnerCon = null;"
                + "\r\nSqlDataAdapter l_oAdapter = null;"
                + "\r\n"
                + "\r\nstring l_strSQL = string.Empty;"
                + "\r\nDataSet l_oDataSet = null;"
                + "\r\nDataRow l_oDataRow = null;"
                + "\r\ntry"
                + "\r\n{"
                + "\r\nif (p_oConnect == null)"
                + "\r\n{"
                + "\r\nl_oInnerCon = new CConnection();"
                + "\r\nl_oInnerCon.Open();"
                + "\r\n}"
                + "\r\nelse"
                + "\r\nl_oInnerCon = p_oConnect;"
                + "\r\n"
                + "\r\nl_oDataSet = new DataSet();"
                + "\r\n"
                + "\r\nl_strSQL = \"SELECT * FROM " + classFullName);
            sb.Append(" WHERE ");
            string[] keyList = primaryKey.Split(',');

            for (int i = 0; i < keyList.Length; i++)
            {
                sb.Append(keyList[i] + " = '\" + " + nameList[i] + " + \"'");
                if (i != keyList.Length - 1)
                    sb.Append(" AND ");
            }
            sb.Append("\";");

            sb.Append("\r\nl_oAdapter = new SqlDataAdapter(l_strSQL, l_oInnerCon.Connection);");
            sb.Append("\r\nl_oAdapter.SelectCommand = new SqlCommand(l_strSQL, l_oInnerCon.Connection, l_oInnerCon.Transaction);");
            sb.Append("\r\nl_oAdapter.Fill(l_oDataSet, \"" + classFullName + "\");");
            sb.Append("\r\n");
            sb.Append("\r\nif (l_oDataSet.Tables[\"" + classFullName + "\"].Rows.Count == 0)");
            sb.Append("\r\n{");
            sb.Append("\r\nif (p_oConnect == null)");
            sb.Append("\r\n{");
            sb.Append("\r\nl_oInnerCon.Close();");
            sb.Append("\r\nl_oInnerCon.Dispose();");
            sb.Append("\r\nl_oInnerCon = null;");
            sb.Append("\r\n}");
            sb.Append("\r\n");
            sb.Append("\r\nif (l_oAdapter != null) l_oAdapter = null;");
            sb.Append("\r\nif (l_oDataSet != null) l_oDataSet = null;");
            sb.Append("\r\nif (l_oDataRow != null) l_oDataRow = null;");
            sb.Append("\r\n");
            sb.Append("\r\nreturn false;");
            sb.Append("\r\n}");
            sb.Append("\r\n");

            sb.Append("\r\nif (p_oConnect == null)"
                + "\r\n{"
                + "\r\nl_oInnerCon.Close();"
                + "\r\nl_oInnerCon.Dispose();"
                + "\r\n}");

            sb.Append("\r\n\r\nif (l_oInnerCon != null) l_oInnerCon = null;"
            + "\r\nif (l_oAdapter != null) l_oAdapter = null;"
            + "\r\nif (l_oDataSet != null) l_oDataSet = null;"
            + "\r\nif (l_oDataRow != null) l_oDataRow = null;"
            + "\r\n"
            + "\r\nreturn true;"
            + "\r\n}");

            sb.Append("\r\n\r\ncatch (Exception ex)"
            + "\r\n{"
                + "\r\nif (p_oConnect == null)"
                + "\r\n{"
                + "\r\n    l_oInnerCon.Close();"
                + "\r\n    l_oInnerCon.Dispose();"
                + "\r\n}"
                + "\r\nif (l_oInnerCon != null) l_oInnerCon = null;"
                + "\r\nif (l_oAdapter != null) l_oAdapter = null;"
                + "\r\nif (l_oDataSet != null) l_oDataSet = null;"
                + "\r\nif (l_oDataRow != null) l_oDataRow = null;"
                + "\r\n\r\nthrow new Exception(ex.Message.ToString());"
                + "\r\n}"
                + "\r\n}");

            return sb.ToString();
        }

        public string AddUpdate()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("\r\n\r\nprotected bool Update(ref CConnection p_oConnect)"
                + "\r\n{"
                + "\r\nCConnection l_oInnerCon = null;"
                + "\r\nSqlCommand l_oCommand = null;"
                + "\r\nSqlParameter[] l_oParam = null;"
                + "\r\n"
                + "\r\nstring l_strSQL = string.Empty;"
                + "\r\nbool l_bAddNew = false;"
                + "\r\n"
                + "\r\ntry"
                + "\r\n{"
                + "\r\nif (p_oConnect == null)"
                + "\r\n{"
                + "\r\nl_oInnerCon = new CConnection();"
                + "\r\nl_oInnerCon.Open();"
                + "\r\n}"
                + "\r\nelse"
                + "\r\nl_oInnerCon = p_oConnect;"
                + "\r\n"
                + "\r\nl_strSQL = \"UPDATE " + classFullName + " SET ");

            for (int i = 0; i < attributesList.Length; i++)
            {
                string[] data = attributesList[i].Split(':');
                sb.Append(data[0] + " = @" + data[0]);
                if (i != attributesList.Length - 1)
                    sb.Append(",");
            }

            sb.Append(" WHERE ");

            string[] keyList = primaryKey.Split(',');

            for (int i = 0; i < keyList.Length; i++)
            {
                sb.Append(keyList[i] + " = '\" + " + nameList[i] + " + \"'");
                if (i != keyList.Length - 1)
                    sb.Append(" AND ");
            }
            sb.Append("\";");

            sb.Append("\r\nl_oParam = new SqlParameter[" + attributesList.Length + "];");

            for (int i = 0; i < attributesList.Length; i++)
            {
                string[] data = attributesList[i].Split(':');
                string length;
                switch (data[1].ToLower().Split('(')[0])
                {
                    case "varchar":
                        length = data[1].ToLower().Split('(')[1];
                        sb.Append("\r\nl_oParam[" + i + "] = new SqlParameter(\"@" + data[0] + @""", SqlDbType.VarChar, " + length + ";");
                        break;
                    case "datetime":
                        sb.Append("\r\nl_oParam[" + i + "] = new SqlParameter(\"@" + data[0] + @""", SqlDbType.DateTime);");
                        break;
                    case "int":
                        sb.Append("\r\nl_oParam[" + i + "] = new SqlParameter(\"@" + data[0] + @""", SqlDbType.Int);");
                        break;
                    case "decimal":
                        sb.Append("\r\nl_oParam[" + i + "] = new SqlParameter(\"@" + data[0] + @""", SqlDbType.Decimal);");
                        break;
                }
            }

            sb.Append("\r\n\r\nl_oCommand = new SqlCommand(l_strSQL, l_oInnerCon.Connection);"
                + "\r\nl_oCommand.Transaction = l_oInnerCon.Transaction;");

            for (int i = 0; i < attributesList.Length; i++)
            {
                sb.Append("\r\nl_oCommand.Parameters.Add(l_oParam[" + i + "]).Value = " + nameList[i] + ";");
            }

            sb.Append("\r\n\r\nif (l_oCommand.ExecuteNonQuery() == 1)"
                    + "\r\nl_bAddNew = true;");

            sb.Append("\r\n\r\nif (p_oConnect == null)"
                + "\r\n{"
                + "\r\nl_oInnerCon.Close();"
                + "\r\nl_oInnerCon.Dispose();"
                + "\r\n}");

            sb.Append("\r\n\r\nif (l_oInnerCon != null) l_oInnerCon = null;"
            + "\r\nif (l_oCommand != null) l_oCommand = null;"
            + "\r\nif (l_oParam != null) l_oParam = null;");

            sb.Append("\r\n\r\nreturn l_bAddNew;");
            sb.Append("\r\n}");

            sb.Append("\r\n\r\ncatch (Exception ex)"
            + "\r\n{"
                + "\r\nif (p_oConnect == null)"
                + "\r\n{"
                + "\r\n    l_oInnerCon.Close();"
                + "\r\n    l_oInnerCon.Dispose();"
                + "\r\n}"
                + "\r\nif (l_oInnerCon != null) l_oInnerCon = null;"
                + "\r\nif (l_oCommand != null) l_oCommand = null;"
                + "\r\nif (l_oParam != null) l_oParam = null;"
                + "\r\n\r\nthrow new Exception(ex.Message.ToString());"
                + "\r\n}"
                + "\r\n}");

            return sb.ToString();
        }

        public string AddDelete()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("\r\n\r\nprotected bool Delete(ref CConnection p_oConnect)"
                + "\r\n{"
                + "\r\nCConnection l_oInnerCon = null;"
                + "\r\nSqlCommand l_oCommand = null;"
                + "\r\nSqlParameter[] l_oParam = null;"
                + "\r\n"
                + "\r\nstring l_strSQL = string.Empty;"
                + "\r\nbool l_bAddNew = false;"
                + "\r\n"
                + "\r\ntry"
                + "\r\n{"
                + "\r\nif (p_oConnect == null)"
                + "\r\n{"
                + "\r\nl_oInnerCon = new CConnection();"
                + "\r\nl_oInnerCon.Open();"
                + "\r\n}"
                + "\r\nelse"
                + "\r\nl_oInnerCon = p_oConnect;"
                + "\r\n"
                + "\r\nl_strSQL = \"DELETE FROM " + classFullName);

            sb.Append(" WHERE ");

            string[] keyList = primaryKey.Split(',');

            for (int i = 0; i < keyList.Length; i++)
            {
                sb.Append(keyList[i] + " = '\" + " + nameList[i] + " + \"'");
                if (i != keyList.Length - 1)
                    sb.Append(" AND ");
            }

            sb.Append("\";");

            sb.Append("\r\n\r\nl_oCommand = new SqlCommand(l_strSQL, l_oInnerCon.Connection);"
                + "\r\nl_oCommand.Transaction = l_oInnerCon.Transaction;");

            for (int i = 0; i < attributesList.Length; i++)
            {
                sb.Append("\r\nl_oCommand.Parameters.Add(l_oParam[" + i + "]).Value = " + nameList[i] + ";");
            }

            sb.Append("\r\n\r\nif (l_oCommand.ExecuteNonQuery() == 1)"
                    + "\r\nl_bAddNew = true;");

            sb.Append("\r\n\r\nif (p_oConnect == null)"
                + "\r\n{"
                + "\r\nl_oInnerCon.Close();"
                + "\r\nl_oInnerCon.Dispose();"
                + "\r\n}");

            sb.Append("\r\n\r\nif (l_oInnerCon != null) l_oInnerCon = null;"
            + "\r\nif (l_oCommand != null) l_oCommand = null;"
            + "\r\nif (l_oParam != null) l_oParam = null;");

            sb.Append("\r\n\r\nreturn l_bAddNew;");
            sb.Append("\r\n}");

            sb.Append("\r\n\r\ncatch (Exception ex)"
            + "\r\n{"
                + "\r\nif (p_oConnect == null)"
                + "\r\n{"
                + "\r\n    l_oInnerCon.Close();"
                + "\r\n    l_oInnerCon.Dispose();"
                + "\r\n}"
                + "\r\nif (l_oInnerCon != null) l_oInnerCon = null;"
                + "\r\nif (l_oCommand != null) l_oCommand = null;"
                + "\r\nif (l_oParam != null) l_oParam = null;"
                + "\r\n\r\nthrow new Exception(ex.Message.ToString());"
                + "\r\n}"
                + "\r\n}");

            return sb.ToString();
        }

        public void SetOutput()
        {
            txtOutput.Text = AddLibrary() + AddStructure() + AddEnum() + AddAttributes()
                + AddContructor() + AddDispose() + AddNew() + AddLoadInstance() + AddUpdate() + AddDelete() + "\r\n}\r\n}";
        }

        protected void btnGenerate_Click(object sender, EventArgs e)
        {
            SetOutput();

            txtOutput.Text += "/r/n Copywrite";
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            string mydocpath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            using (StreamWriter outputFile = new StreamWriter(mydocpath + @"\Test.txt", true))
            {
                outputFile.WriteLine(txtOutput.Text);
            }
        }
    }
}
