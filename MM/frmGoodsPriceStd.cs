using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using cf01.CLS;

namespace cf01.MM
{
    public partial class frmGoodsPriceStd : Form
    {
        private string edit_flag = "0";
        private string userid = DBUtility._user_id;
        private bool append_mode=false;
        private bool edit_mode = false;
        public static string get_id = "";
        public static string get_formula_type = "";
        public frmGoodsPriceStd()
        {
            InitializeComponent();
        }

        private void countUnitPrice()
        {
            //if (edit_mode == false)
            //    return;
            if (!checkNumberValid())
                return;
            double result_a, result_a1, result_b, result_c1, result_c2, result_c3, result_c4;
            double number_a, number_b, number_c, number_d, number_e, number_f, number_g;
            double rate_a, rate_d;
            number_a = (txtNumber_A.Text != "" ? Convert.ToSingle(txtNumber_A.Text) : 0);
            rate_a = (txtRate_A.Text != "" ? Convert.ToSingle(txtRate_A.Text) : 0);
            number_b = (txtNumber_B.Text != "" ? Convert.ToSingle(txtNumber_B.Text) : 0);
            result_a = (rate_a != 0 ? Math.Round((number_a / rate_a) * number_b, 3) : 0);
            txtResult_A.Text = result_a.ToString();
            //txtNumber_C.Text = Math.Round(result_a * number_b, 3).ToString();
            number_c = (txtNumber_C.Text != "" ? Convert.ToSingle(txtNumber_C.Text) : 0);
            result_a1 = Math.Round(result_a * number_c, 3);
            txtResult_A1.Text = result_a1.ToString();
            number_d = (txtNumber_D.Text != "" ? Convert.ToSingle(txtNumber_D.Text) : 0);
            rate_d = (txtRate_D.Text != "" ? Convert.ToSingle(txtRate_D.Text) : 0);
            number_e = (txtNumber_E.Text != "" ? Convert.ToSingle(txtNumber_E.Text) : 0);
            result_b = (rate_d != 0 ? Math.Round((number_d / rate_d) * number_e * number_c, 3) : 0);
            txtResult_B.Text = result_b.ToString();
            number_f = (txtNumber_F.Text != "" ? Convert.ToSingle(txtNumber_F.Text) : 0);
            number_g = (txtNumber_G.Text != "" ? Convert.ToSingle(txtNumber_G.Text) : 0);
            result_c1 = Math.Round((result_a1 + result_b) * number_f * number_g, 3);
            result_c2 = Math.Round(result_c1 * 144, 3);
            result_c3 = Math.Round(result_c1 * 1000, 3);
            result_c4 = Math.Round(result_c1 * 12, 3);
            txtResult_C1.Text = result_c1.ToString();
            txtResult_C2.Text = result_c2.ToString();
            txtResult_C3.Text = result_c3.ToString();
            txtResult_C4.Text = result_c4.ToString();
        }
        private bool checkNumberValid()
        {
            bool result = true;
            //if (txtNumber_A.Text == "")
            //    result = false;
            //if (txtRate_A.Text == "")
            //    result = false;
            //if (txtNumber_B.Text == "")
            //    result = false;
            //if (txtNumber_C.Text == "")
            //    result = false;
            //if (txtNumber_D.Text == "")
            //    result = false;
            //if (txtNumber_E.Text == "")
            //    result = false;
            //if (txtRate_D.Text == "")
            //    result = false;
            //if (txtNumber_E.Text == "")
            //    result = false;
            //if (txtNumber_F.Text == "")
            //    result = false;
            //if (txtNumber_G.Text == "")
            //    result = false;
            return result;

        }


        private void frmUnitPriceFormula_Load(object sender, EventArgs e)
        {
            dgvDetails.AutoGenerateColumns = false;
            setTextBoxEnabled();
            loadData();
        }


        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            addNew();
        }
        private void addNew()
        {
            append_mode = true;
            edit_mode = true;
            cleanTextBox(0);
            setTextBoxEnabled();
            edit_flag = "1";
            txtMat_type.Focus();
            
        }
        private void cleanTextBox(int clean_part)
        {
            if (clean_part == 0)
            {
                txtId.Text = "";
                txtCdesc.Text = "";
                txtMat_type.Text = "";
                txtPrd_Type.Text = "";
                txtArt.Text = "";
                txtSize.Text = "";
                txtFid.Text = "";
            }
            txtRate_A.Text = "";
            txtRate_D.Text = "";
            txtRate_A.Text = "";
            txtRate_D.Text = "";
            txtNumber_A.Text = "";
            txtNumber_B.Text = "";
            txtNumber_C.Text = "";
            txtNumber_D.Text = "";
            txtNumber_E.Text = "";
            txtNumber_E.Text = "";
            txtNumber_F.Text = "";
            txtNumber_G.Text = "";
            txtResult_A.Text = "0";
            txtResult_B.Text = "0";
            txtResult_C1.Text = "0";
            txtResult_C2.Text = "0";
            txtResult_C3.Text = "0";
            txtResult_C4.Text = "0";
        }
        private bool checkValid()
        {

            
            return true;
        }
        private bool checkExistId()
        {
            string id = txtId.Text;
            string strSql = " Select id From mm_goods_price_std Where id='" + id + "'";
            DataTable dtId = clsPublicOfCF01.GetDataTable(strSql);
            if (dtId.Rows.Count > 0)
                return true;
            return false;
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            Save();
        }
        private void Save()
        {
            //if (edit_mode == false)
            //{
            //    MessageBox.Show("不是編輯狀態!");
            //    return;
            //}
            if (!checkValid())
                return;
            string id, cdesc;
            string mat_type, prd_type, art, size, clr, formula_id;
            string strSql;
            string result;
            
            float result_a, result_b, result_c1, result_c2, result_c3, result_c4;
            float number_a, number_b, number_c, number_d, number_e, number_f, number_g;
            float rate_a, rate_d;
            if (txtId.Text == "")
            {
                string dat = System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
                txtId.Text = "P-" + dat.Substring(0, 4) + dat.Substring(5, 2) + dat.Substring(8, 2) + dat.Substring(11, 2) + dat.Substring(14, 2) + dat.Substring(17, 2);
            }
            id = txtId.Text;
            cdesc = txtCdesc.Text;
            mat_type = txtMat_type.Text;
            prd_type = txtPrd_Type.Text;
            art = txtArt.Text;
            size = txtSize.Text;
            clr = "";
            formula_id = txtFid.Text;
            number_a = (txtNumber_A.Text != "" ? Convert.ToSingle(txtNumber_A.Text) : 0);
            rate_a = (txtRate_A.Text != "" ? Convert.ToSingle(txtRate_A.Text) : 0);
            number_b = (txtNumber_B.Text != "" ? Convert.ToSingle(txtNumber_B.Text) : 0);
            result_a = (txtResult_A.Text != "" ? Convert.ToSingle(txtResult_A.Text) : 0);
            number_c = (txtNumber_C.Text != "" ? Convert.ToSingle(txtNumber_C.Text) : 0);
            number_d = (txtNumber_D.Text != "" ? Convert.ToSingle(txtNumber_D.Text) : 0);
            rate_d = (txtRate_D.Text != "" ? Convert.ToSingle(txtRate_D.Text) : 0);
            number_e = (txtNumber_E.Text != "" ? Convert.ToSingle(txtNumber_E.Text) : 0);
            result_b = (txtResult_B.Text != "" ? Convert.ToSingle(txtResult_B.Text) : 0);
            number_f = (txtNumber_F.Text != "" ? Convert.ToSingle(txtNumber_F.Text) : 0);
            number_g = (txtNumber_G.Text != "" ? Convert.ToSingle(txtNumber_G.Text) : 0);
            result_c1 = (txtResult_C1.Text != "" ? Convert.ToSingle(txtResult_C1.Text) : 0);
            result_c2 = (txtResult_C2.Text != "" ? Convert.ToSingle(txtResult_C2.Text) : 0);
            result_c3 = (txtResult_C3.Text != "" ? Convert.ToSingle(txtResult_C3.Text) : 0);
            result_c4 = (txtResult_C4.Text != "" ? Convert.ToSingle(txtResult_C4.Text) : 0);
            if (!checkExistId())//新增
                strSql = string.Format(@"INSERT INTO mm_goods_price_std (id,cdesc,mat_type,prd_type,art,size,clr,formula_id,number_a,rate_a,number_b,result_a,number_c,number_d,rate_d
                    ,number_e,result_b,number_f,number_g,result_c1,result_c2,result_c3,result_c4,crusr,crtim)
                    VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}'
                        ,'{19}','{20}','{21}','{22}','{23}',GETDATE())"
                        , id, cdesc, mat_type, prd_type, art, size, clr, formula_id, number_a, rate_a, number_b, result_a, number_c, number_d, rate_d
                    , number_e, result_b, number_f, number_g, result_c1, result_c2, result_c3, result_c4, userid);
            else
                strSql = string.Format(@"UPDATE mm_goods_price_std SET cdesc='{0}',number_a='{1}',rate_a='{2}',number_b='{3}',result_a='{4}',number_c='{5}'
                    ,number_d='{6}',rate_d='{7}',number_e='{8}',result_b='{9}',number_f='{10}',number_g='{11}',result_c1='{12}',result_c2='{13}',result_c3='{14}'
                    ,result_c4='{15}',amusr='{16}',amtim=GETDATE(),mat_type='{17}',prd_type='{18}',art='{19}',size='{20}',clr='{21}',formula_id='{22}'
                    WHERE id='{23}'"
                    , cdesc, number_a, rate_a, number_b, result_a, number_c, number_d, rate_d, number_e, result_b, number_f, number_g
                    , result_c1, result_c2, result_c3, result_c4, userid, mat_type, prd_type, art, size, clr, formula_id, id);
            result = clsPublicOfCF01.ExecuteSqlUpdate(strSql);
            if (result != "")
                MessageBox.Show("儲存記錄失敗!");
            else
            {
                edit_flag = "0";
                append_mode = false;
                edit_mode = false;
                loadData();
                setTextBoxEnabled();
            }
        }
        private void loadData()
        {

            string strSql = "Select * from mm_goods_price_std Where id>=''";
            if (txtId.Text != "")
                strSql += " And id='" + txtId.Text.Trim() + "'";
            strSql += " order by id";
            DataTable dtPrice = clsPublicOfCF01.GetDataTable(strSql);
            dgvDetails.DataSource = dtPrice;
        }
        private void fillTextBox(int rows)
        {
            if (dgvDetails.Rows.Count == 0)
                return;
            cleanTextBox(0);//全部清空文本框
            DataGridViewRow CurrentRow = dgvDetails.Rows[rows];
            txtId.Text = CurrentRow.Cells["colId"].Value.ToString();
            txtCdesc.Text = CurrentRow.Cells["colCdesc"].Value.ToString();
            txtMat_type.Text = CurrentRow.Cells["colMat_type"].Value.ToString();
            txtPrd_Type.Text = CurrentRow.Cells["colPrd_Type"].Value.ToString();
            txtArt.Text = CurrentRow.Cells["colArt"].Value.ToString();
            txtSize.Text = CurrentRow.Cells["colSize"].Value.ToString();
            txtFid.Text = CurrentRow.Cells["colFid"].Value.ToString();
            txtNumber_A.Text = CurrentRow.Cells["colNumber_A"].Value.ToString();
            txtNumber_B.Text = CurrentRow.Cells["colNumber_B"].Value.ToString();
            txtNumber_C.Text = CurrentRow.Cells["colNumber_C"].Value.ToString();
            txtNumber_D.Text = CurrentRow.Cells["colNumber_D"].Value.ToString();
            txtNumber_E.Text = CurrentRow.Cells["colNumber_E"].Value.ToString();
            txtNumber_F.Text = CurrentRow.Cells["colNumber_F"].Value.ToString();
            txtNumber_G.Text = CurrentRow.Cells["colNumber_G"].Value.ToString();
            txtRate_A.Text = CurrentRow.Cells["colRate_A"].Value.ToString();
            txtRate_D.Text = CurrentRow.Cells["colRate_D"].Value.ToString();
            txtResult_A.Text = CurrentRow.Cells["colResult_A"].Value.ToString();
            txtResult_B.Text = CurrentRow.Cells["colResult_B"].Value.ToString();
            txtResult_C1.Text = CurrentRow.Cells["colResult_C1"].Value.ToString();
            txtResult_C2.Text = CurrentRow.Cells["colResult_C2"].Value.ToString();
            txtResult_C3.Text = CurrentRow.Cells["colResult_C3"].Value.ToString();
            txtResult_C4.Text = CurrentRow.Cells["colResult_C4"].Value.ToString();
        }

        private void dgvDetails_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            edit_flag = "0";
            append_mode = false;
            edit_mode = false;
            setTextBoxEnabled();
            fillTextBox(dgvDetails.CurrentCell.RowIndex);
        }


        private void Edit()
        {
            edit_flag = "2";
            append_mode = false;
            edit_mode = true;
            setTextBoxEnabled();
        }
        private void setTextBoxEnabled()
        {
            //txtId.Properties.ReadOnly = !append_mode;
            //if (append_mode==true && edit_mode == true)
            //    txtId.BackColor = Color.White;
            //else
            //    txtId.BackColor = Color.Silver;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            Delete();
        }
        private void Delete()
        {
            if (checkExistId() == false)
            {
                MessageBox.Show("沒有要刪除的記錄!");
                return;
            }
            string result;
            string strSql;
            strSql = string.Format(@"DELETE mm_goods_price_std WHERE id='{0}'", txtId.Text);
            result = clsPublicOfCF01.ExecuteSqlUpdate(strSql);
            if (result != "")
                MessageBox.Show("刪除記錄失敗!");
            else
            {
                edit_flag = "0";
                append_mode = false;
                edit_mode = false;
                loadData();
                setTextBoxEnabled();
                cleanTextBox(0);//清空全部文本框
            }
        }

        private void txtId_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                SendKeys.Send("{tab}");
            }
        }

        private void btnShowFormula_Click(object sender, EventArgs e)
        {
            get_formula_type = "Y";
            frmGoodsPriceFormula frmFormulaFind = new frmGoodsPriceFormula();
            frmFormulaFind.ShowDialog();
            if (get_formula_type != "")
            {
                txtFid.Text = get_formula_type;
                loadFormula(txtFid.Text);

            }
            get_formula_type = "";
            frmFormulaFind.Dispose();
        }

        private void loadFormula(string fid)
        {
            string strSql;
            strSql = "Select * from mm_goods_price_formula where id>=''";
            if (fid != "")
                strSql += " and  id='" + fid + "'";

            cleanTextBox(2);
            DataTable dtPrice = clsPublicOfCF01.GetDataTable(strSql);
            if (dtPrice.Rows.Count == 0)
            {
                MessageBox.Show("公式不存在!");
                return;
            }
            DataRow dr = dtPrice.Rows[0];
            
            txtFid.Text = dr["id"].ToString();
            txtMat_type.Text = dr["mat_type"].ToString();
            //txtCdesc.Text = dr["cdesc"].ToString();
            txtNumber_A.Text = dr["number_a"].ToString();
            txtNumber_B.Text = dr["number_b"].ToString();
            txtNumber_C.Text = dr["number_c"].ToString();
            txtNumber_D.Text = dr["number_d"].ToString();
            txtNumber_E.Text = dr["number_e"].ToString();
            txtNumber_F.Text = dr["number_f"].ToString();
            txtNumber_G.Text = dr["number_g"].ToString();
            txtRate_A.Text = dr["rate_a"].ToString();
            txtRate_D.Text = dr["rate_d"].ToString();
            txtResult_A.Text = dr["result_a"].ToString();
            txtResult_B.Text = dr["result_b"].ToString();
            txtResult_C1.Text = dr["result_c1"].ToString();
            txtResult_C2.Text = dr["result_c2"].ToString();
            txtResult_C3.Text = dr["result_c3"].ToString();
            txtResult_C4.Text = dr["result_c4"].ToString();

        }

        private void txtNumber_A_Leave(object sender, EventArgs e)
        {
            countUnitPrice();
        }

        private void btnFind_Click(object sender, EventArgs e)
        {
            frmMmCalculatePriceFind frmGoodsPriceStd_Find = new frmMmCalculatePriceFind();
            frmGoodsPriceStd_Find.ShowDialog();
            if (get_id != "")
            {
                txtId.Text = get_id;
                loadData();
                if (dgvDetails.Rows.Count > 0)
                    fillTextBox(dgvDetails.CurrentCell.RowIndex);

            }
            get_id = "";
            frmGoodsPriceStd_Find.Dispose();
        }
    }
}
