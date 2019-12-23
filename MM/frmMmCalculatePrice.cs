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
using cf01.MDL;
using cf01.Forms;

namespace cf01.MM
{
    public partial class frmMmCalculatePrice : Form
    {
        private int edit_flag = 0;
        private string userid = DBUtility._user_id;
        private bool append_mode=false;
        private bool edit_mode = false;
        private bool allow_edit = false;//控制當點擊表格記錄，賦值給文本框時，不用執行ValueChange的事件
        public static string get_id = "";
        public static int get_ver = 0;
        public static string get_formula_type = "";
        private DataTable dtPriceDetails = new DataTable();
        private int upd_flag = 0;//0--新增或修改；2--新版本；1--刪除後，明細表格中還有記錄；3--刪除後，明細表格中已沒有記錄，用來判別是否刪除主表
        public frmMmCalculatePrice()
        {
            InitializeComponent();
        }



        private void countUnitPrice()
        {
            if (allow_edit == false)//如果是被表格賦值的，就不用計算以下
                return;
            if (!checkNumberValid())
                return;
            double result_a, result_a1, result_b, result_c1, result_c2, result_c3, result_c4;
            double number_a, number_b, number_c, number_d, number_e, number_f, number_g;
            double rate_a, rate_d;
            number_a = (txtNumber_A.Text != "" ? Convert.ToDouble(txtNumber_A.Text) : 0);
            rate_a = (txtRate_A.Text != "" ? Convert.ToDouble(txtRate_A.Text) : 0);
            number_b = (txtNumber_B.Text != "" ? Convert.ToDouble(txtNumber_B.Text) : 0);
            result_a = (rate_a != 0 ? Math.Round((number_a / rate_a) * number_b, 3) : 0);
            txtResult_A.Text = result_a.ToString();
            //txtNumber_C.Text = Math.Round(result_a * number_b, 3).ToString();
            number_c = (txtNumber_C.Text != "" ? Convert.ToDouble(txtNumber_C.Text) : 0);
            result_a1 = Math.Round(result_a * number_c, 3);
            txtResult_A1.Text = result_a1.ToString();
            number_d = (txtNumber_D.Text != "" ? Convert.ToDouble(txtNumber_D.Text) : 0);
            rate_d = (txtRate_D.Text != "" ? Convert.ToDouble(txtRate_D.Text) : 0);
            number_e = (txtNumber_E.Text != "" ? Convert.ToDouble(txtNumber_E.Text) : 0);
            result_b = (rate_d != 0 ? Math.Round((number_d / rate_d) * number_e * number_c, 3) : 0);
            txtResult_B.Text = result_b.ToString();
            number_f = (txtNumber_F.Text != "" ? Convert.ToDouble(txtNumber_F.Text) : 0);
            number_g = (txtNumber_G.Text != "" ? Convert.ToDouble(txtNumber_G.Text) : 0);
            result_c1 = Math.Round((result_a1 + result_b) * number_f * number_g, 3);
            result_c2 = Math.Round(result_c1 * 144, 3);
            result_c3 = Math.Round(result_c1 * 1000, 3);
            result_c4 = Math.Round(result_c1 * 12, 3);
            txtResult_C1.Text = result_c1.ToString();
            txtResult_C2.Text = result_c2.ToString();
            txtResult_C3.Text = result_c3.ToString();
            txtResult_C4.Text = result_c4.ToString();
            
        }
        private void sumResult()
        {
            if (allow_edit == false)
                return;
            double total_a = 0, total_a1 = 0, total_b = 0, total_c1 = 0, total_c2 = 0, total_c3 = 0, total_c4 = 0;
            for (int i = 0; i < dgvDetails.Rows.Count; i++)
            {
                DataGridViewRow CurrentRow = dgvDetails.Rows[i];
                total_a = total_a + (CurrentRow.Cells["colResult_A"].Value.ToString() != "" ? Convert.ToDouble(CurrentRow.Cells["colResult_A"].Value) : 0);
                total_a1 = total_a1 + (CurrentRow.Cells["colResult_A1"].Value.ToString() != "" ? Convert.ToDouble(CurrentRow.Cells["colResult_A1"].Value) : 0);
                total_b = total_b + (CurrentRow.Cells["colResult_B"].Value.ToString() != "" ? Convert.ToDouble(CurrentRow.Cells["colResult_B"].Value) : 0);
                total_c1 = total_c1 + (CurrentRow.Cells["colResult_C1"].Value.ToString() != "" ? Convert.ToDouble(CurrentRow.Cells["colResult_C1"].Value) : 0);
                total_c2 = total_c2 + (CurrentRow.Cells["colResult_C2"].Value.ToString() != "" ? Convert.ToDouble(CurrentRow.Cells["colResult_C2"].Value) : 0);
                total_c3 = total_c3 + (CurrentRow.Cells["colResult_C3"].Value.ToString() != "" ? Convert.ToDouble(CurrentRow.Cells["colResult_C3"].Value) : 0);
                total_c4 = total_c4 + (CurrentRow.Cells["colResult_C4"].Value.ToString() != "" ? Convert.ToDouble(CurrentRow.Cells["colResult_C4"].Value) : 0);
            }
            txtTotal_A.Text = Math.Round(total_a,3).ToString();
            txtTotal_A1.Text = Math.Round(total_a1, 3).ToString();
            txtTotal_B.Text = Math.Round(total_b,3).ToString();
            txtTotal_C1.Text = Math.Round(total_c1,3).ToString();
            txtTotal_C2.Text = Math.Round(total_c2,3).ToString();
            txtTotal_C3.Text = Math.Round(total_c3,3).ToString();
            txtTotal_C4.Text = Math.Round(total_c4,3).ToString();

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
            dgvWipMo.AutoGenerateColumns = false;
            dgvIdAllVer.AutoGenerateColumns = false;
            setTextBoxEnabled();
            loadDetailsById();
            loadHeadByQuoTationId();
        }
        private void loadHeadByQuoTationId()
        {
            if (frmQuotation.sent_quotation != "")
            {
                txtId.Text = clsMmCalculatePrice.getIdByQuotationId(frmQuotation.sent_quotation);
                loadHeadById();
                loadDetailsById();
                showIdAllVer();//查找Id的所有版本記錄
             }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            txtId.Text = "";
            lpeVer.Properties.DataSource = null;
            addNew(0);
            addPriceFromMo();//將選中的制單記錄循環加入到計价表中
        }
        private void btnAddRec_Click(object sender, EventArgs e)
        {
            txtSeq.Focus();
            addNew(1);
            addPriceFromMo();//將選中的制單記錄循環加入到計价表中
            //fillDetailsView();
        }
        private void addNew(int add_type)
        {
            append_mode = true;
            edit_mode = true;
            allow_edit = true;
            upd_flag = 0;
            cleanTextBox(add_type);
            loadHeadById();
            setTextBoxEnabled();
            edit_flag = 1;
            
            if (add_type == 0)
                loadDetailsById();//整套新增，執行這個語句，目的是清空表格內容，
            if (xTabC1.SelectedTabPageIndex == 0)//如果是在第1頁的，就加這條記錄；第2頁的就不用加，因為在循環中會添加的
            {
                addBlankRecGridView();
                if (add_type == 0)
                {
                    txtCdesc.Focus();
                }
                else
                    txtMat_type.Focus();
            }
        }
        //在加入制單中的記錄後，切換顯示到第1個界面
        private void addPriceFromMo()
        {
            if (xTabC1.SelectedTabPageIndex == 1)
            {
                txtMo_id_W.Focus();

                //將選中的制單記錄循環加入到計价表中
                for (int i = 0; i < dgvWipMo.Rows.Count; i++)
                {
                    DataGridViewRow crMo = dgvWipMo.Rows[i];
                    if ((bool)crMo.Cells["colSelect_mo"].Value == true)
                    {
                        addBlankRecGridView();
                        int crrow = dgvDetails.CurrentCell.RowIndex;
                        DataGridViewRow crPrice = dgvDetails.Rows[crrow];
                        if (crMo.Cells["colGoods_id_W"].Value.ToString().Trim().Length >= 18)
                        {
                            crPrice.Cells["colMat_type"].Value = crMo.Cells["colGoods_id_W"].Value.ToString().Substring(2, 2);
                            crPrice.Cells["colPrd_Type"].Value = crMo.Cells["colGoods_id_W"].Value.ToString().Substring(4, 2);
                            crPrice.Cells["colArt"].Value = crMo.Cells["colGoods_id_W"].Value.ToString().Substring(4, 7);
                            crPrice.Cells["colSize"].Value = crMo.Cells["colGoods_id_W"].Value.ToString().Substring(11, 3);
                            crPrice.Cells["colNumber_C"].Value = (crMo.Cells["colNumber_C_W"].Value.ToString()!=""?Convert.ToDecimal(crMo.Cells["colNumber_C_W"].Value.ToString()):0);
                            crPrice.Cells["colFid"].Value = (crMo.Cells["colFid_W"].Value != null ? crMo.Cells["colFid_W"].Value.ToString() : "");
                            crPrice.Cells["colMo_id"].Value = txtMo_id_W.Text;
                            fillTextBox(crrow);//這兩句是將值賦給文本框，以便計算總答案
                            fillFormula(txtFid.Text);
                        }
                    }
                }
                //fillTextBoxValue();//這句是將表格中最後一行的值賦給文本框
                xTabC1.SelectedTabPageIndex = 0;
            }
        }
        private void cleanTextBox(int clean_part)
        {
            if (clean_part == 0)//整套新增時，清空表頭 + 明細
            {
                txtCdesc.Text = "";
                txtTotal_A.Text = "";
                txtTotal_B.Text = "";
                txtTotal_C1.Text = "";
                txtTotal_C2.Text = "";
                txtTotal_C3.Text = "";
                txtTotal_C4.Text = "";
                txtOffer_price.Text = "";
                txtBP.Text = "";
                txtQtNo.Text = "";
                txtVer.Text = "0";
                setVerTextBoxBackColor();//設定版本號顏色
                //txtSeq.Text = "";
            }
            if (clean_part == 1)
            {
                txtSeq.Text = "";
            }
            else
            {
                if (clean_part != 2)
                {
                    txtMo_id.Text = "";
                    txtCdesc_D.Text = "";
                    txtMat_type.Text = "";
                    txtPrd_Type.Text = "";
                    txtArt.Text = "";
                    txtSize.Text = "";
                    txtColor.Text = "";
                    txtSeq.Text = "";
                    txtNumber_C.Text = "";
                    txtFid.Text = "";
                }
                
                txtRate_A.Text = "";
                txtRate_D.Text = "";
                txtRate_A.Text = "";
                txtRate_D.Text = "";
                txtNumber_A.Text = "";
                txtNumber_B.Text = "";
                txtNumber_D.Text = "";
                txtNumber_E.Text = "";
                txtNumber_E.Text = "";
                txtNumber_F.Text = "";
                txtNumber_G.Text = "";

                txtCrusr.Text = "";
                txtCrtim.Text = "";
                txtAmusr.Text = "";
                txtAmtim.Text = "";
            }
            txtResult_A.Text = "";
            txtResult_A1.Text = "";
            txtResult_B.Text = "";
            txtResult_C1.Text = "";
            txtResult_C2.Text = "";
            txtResult_C3.Text = "";
            txtResult_C4.Text = "";
            
        }
        private void addBlankRecGridView()
        {
            dtPriceDetails.Rows.Add();
            setSelectRec(dtPriceDetails.Rows.Count - 1);//定位到新增的記錄
        }
        private void fillDetailsView()
        {
            if (allow_edit == false)
                return;
            if (dgvDetails.Rows.Count == 0)
                return;
            int row = dgvDetails.CurrentCell.RowIndex;//dtPriceDetails.Rows.Count - 1;// 
            DataGridViewRow CurrentRow = dgvDetails.Rows[row];
            CurrentRow.Cells["colId"].Value = txtId.Text;
            CurrentRow.Cells["colSeq"].Value = txtSeq.Text;
            CurrentRow.Cells["colCdesc_D"].Value = txtCdesc_D.Text;
            CurrentRow.Cells["colMat_type"].Value = txtMat_type.Text;
            CurrentRow.Cells["colPrd_Type"].Value = txtPrd_Type.Text;
            CurrentRow.Cells["colArt"].Value = txtArt.Text;
            CurrentRow.Cells["colSize"].Value = txtSize.Text;
            CurrentRow.Cells["colColor"].Value = txtColor.Text;
            CurrentRow.Cells["colFid"].Value = txtFid.Text;
            CurrentRow.Cells["colMo_id"].Value = txtMo_id.Text;
            CurrentRow.Cells["colNumber_A"].Value = (txtNumber_A.Text != "" ? Convert.ToDouble(txtNumber_A.Text) : 0);
            CurrentRow.Cells["colNumber_B"].Value = (txtNumber_B.Text != "" ? Convert.ToDouble(txtNumber_B.Text) : 0);
            CurrentRow.Cells["colNumber_C"].Value = (txtNumber_C.Text != "" ? Convert.ToDouble(txtNumber_C.Text) : 0);
            CurrentRow.Cells["colNumber_D"].Value = (txtNumber_D.Text != "" ? Convert.ToDouble(txtNumber_D.Text) : 0);
            CurrentRow.Cells["colNumber_E"].Value = (txtNumber_E.Text != "" ? Convert.ToDouble(txtNumber_E.Text) : 0);
            CurrentRow.Cells["colNumber_F"].Value = (txtNumber_F.Text != "" ? Convert.ToDouble(txtNumber_F.Text) : 0);
            CurrentRow.Cells["colNumber_G"].Value = (txtNumber_G.Text != "" ? Convert.ToDouble(txtNumber_G.Text) : 0);
            CurrentRow.Cells["colRate_A"].Value = (txtRate_A.Text != "" ? Convert.ToDouble(txtRate_A.Text) : 0);
            CurrentRow.Cells["colRate_D"].Value = (txtRate_D.Text != "" ? Convert.ToDouble(txtRate_D.Text) : 0);
            CurrentRow.Cells["colResult_A"].Value = (txtResult_A.Text != "" ? Convert.ToDouble(txtResult_A.Text) : 0);
            CurrentRow.Cells["colResult_A1"].Value = (txtResult_A1.Text != "" ? Convert.ToDouble(txtResult_A1.Text) : 0);
            CurrentRow.Cells["colResult_B"].Value = (txtResult_B.Text != "" ? Convert.ToDouble(txtResult_B.Text) : 0);
            CurrentRow.Cells["colResult_C1"].Value = (txtResult_C1.Text != "" ? Convert.ToDouble(txtResult_C1.Text) : 0);
            CurrentRow.Cells["colResult_C2"].Value = (txtResult_C2.Text != "" ? Convert.ToDouble(txtResult_C2.Text) : 0);
            CurrentRow.Cells["colResult_C3"].Value = (txtResult_C3.Text != "" ? Convert.ToDouble(txtResult_C3.Text) : 0);
            CurrentRow.Cells["colResult_C4"].Value = (txtResult_C4.Text != "" ? Convert.ToDouble(txtResult_C4.Text) : 0);
        }
        private bool checkValid()
        {
            if (edit_flag == 0)
            {
                DataTable dt = clsMmCalculatePrice.loadHeadById(txtId.Text.Trim() != "" ? txtId.Text.Trim() : "ZZZZZZZZZ", txtVer.Text != "" ? Convert.ToInt32(txtVer.Text) : 0);
                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("沒有儲存的記錄!");
                    return false;
                }
            }
            
            return true;
        }
        private bool checkExistId()
        {
            string id = txtId.Text;
            string strSql = " Select id From mm_calculate_price_details Where id='" + id + "'";
            DataTable dtId = clsPublicOfCF01.GetDataTable(strSql);
            if (dtId.Rows.Count > 0)
                return true;
            return false;
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            txtSeq.Focus();
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
            mdlMmCalculatePriceHead mdlHead = new mdlMmCalculatePriceHead();
            List<mdlMmCalculatePriceDetails> mdlDetails = new List<mdlMmCalculatePriceDetails>();
            

            mdlHead.id = txtId.Text;
            mdlHead.ver = txtVer.Text != "" ? Convert.ToInt32(txtVer.Text) : 0;
            mdlHead.cdesc = txtCdesc.Text;
            mdlHead.qtno = txtQtNo.Text;
            mdlHead.price_bp = (txtBP.Text != "" ? Convert.ToDouble(txtBP.Text) : 0);
            mdlHead.offer_price = (txtOffer_price.Text != "" ? Convert.ToDouble(txtOffer_price.Text) : 0);
            mdlHead.total_a = (txtTotal_A.Text != "" ? Convert.ToDouble(txtTotal_A.Text) : 0);
            mdlHead.total_a1 = (txtTotal_A1.Text != "" ? Convert.ToDouble(txtTotal_A1.Text) : 0);
            mdlHead.total_b = (txtTotal_B.Text != "" ? Convert.ToDouble(txtTotal_B.Text) : 0);
            mdlHead.total_c1 = (txtTotal_C1.Text != "" ? Convert.ToDouble(txtTotal_C1.Text) : 0);
            mdlHead.total_c2 = (txtTotal_C2.Text != "" ? Convert.ToDouble(txtTotal_C2.Text) : 0);
            mdlHead.total_c3 = (txtTotal_C3.Text != "" ? Convert.ToDouble(txtTotal_C3.Text) : 0);
            mdlHead.total_c4 = (txtTotal_C4.Text != "" ? Convert.ToDouble(txtTotal_C4.Text) : 0);
            if (upd_flag == 0 || upd_flag==2)//如果是新增或修改或新版本
            {
                for (int i = 0; i < dgvDetails.Rows.Count; i++)
                {

                    DataGridViewRow cr = dgvDetails.Rows[i];
                    //if (cr.Cells["colStatus"].Value.ToString() == "Y")//將有改變的記錄才更新
                    //{
                    mdlMmCalculatePriceDetails mdlDetail = new mdlMmCalculatePriceDetails();
                    mdlDetail.id = mdlHead.id;
                    mdlDetail.ver = mdlHead.ver;
                    mdlDetail.seq = cr.Cells["colSeq"].Value.ToString();
                    mdlDetail.cdesc = cr.Cells["colCdesc_D"].Value.ToString();
                    mdlDetail.mo_id = cr.Cells["colMo_id"].Value.ToString();
                    mdlDetail.mat_type = cr.Cells["colMat_type"].Value.ToString();
                    mdlDetail.prd_type = cr.Cells["colPrd_Type"].Value.ToString();
                    mdlDetail.art = cr.Cells["colArt"].Value.ToString();
                    mdlDetail.size = cr.Cells["colSize"].Value.ToString();
                    mdlDetail.clr = cr.Cells["colColor"].Value.ToString();
                    mdlDetail.formula_id = cr.Cells["colFid"].Value.ToString();
                    mdlDetail.number_a = (cr.Cells["colNumber_A"].Value.ToString() != "" ? Convert.ToDouble(cr.Cells["colNumber_A"].Value) : 0);
                    mdlDetail.rate_a = (cr.Cells["colRate_A"].Value.ToString() != "" ? Convert.ToDouble(cr.Cells["colRate_A"].Value) : 0);
                    mdlDetail.number_b = (cr.Cells["colNumber_B"].Value.ToString() != "" ? Convert.ToDouble(cr.Cells["colNumber_B"].Value) : 0);
                    mdlDetail.result_a = (cr.Cells["colResult_A"].Value.ToString() != "" ? Convert.ToDouble(cr.Cells["colResult_A"].Value) : 0);
                    mdlDetail.result_a1 = (cr.Cells["colResult_A1"].Value.ToString() != "" ? Convert.ToDouble(cr.Cells["colResult_A1"].Value) : 0);
                    mdlDetail.number_c = (cr.Cells["colNumber_C"].Value.ToString() != "" ? Convert.ToDouble(cr.Cells["colNumber_C"].Value) : 0);
                    mdlDetail.number_d = (cr.Cells["colNumber_D"].Value.ToString() != "" ? Convert.ToDouble(cr.Cells["colNumber_D"].Value) : 0);
                    mdlDetail.rate_d = (cr.Cells["colRate_D"].Value.ToString() != "" ? Convert.ToDouble(cr.Cells["colRate_D"].Value) : 0);
                    mdlDetail.number_e = (cr.Cells["colNumber_E"].Value.ToString() != "" ? Convert.ToDouble(cr.Cells["colNumber_E"].Value) : 0);
                    mdlDetail.result_b = (cr.Cells["colResult_B"].Value.ToString() != "" ? Convert.ToDouble(cr.Cells["colResult_B"].Value) : 0);
                    mdlDetail.number_f = (cr.Cells["colNumber_F"].Value.ToString() != "" ? Convert.ToDouble(cr.Cells["colNumber_F"].Value) : 0);
                    mdlDetail.number_g = (cr.Cells["colNumber_G"].Value.ToString() != "" ? Convert.ToDouble(cr.Cells["colNumber_G"].Value) : 0);
                    mdlDetail.result_c1 = (cr.Cells["colResult_C1"].Value.ToString() != "" ? Convert.ToDouble(cr.Cells["colResult_C1"].Value) : 0);
                    mdlDetail.result_c2 = (cr.Cells["colResult_C2"].Value.ToString() != "" ? Convert.ToDouble(cr.Cells["colResult_C2"].Value) : 0);
                    mdlDetail.result_c3 = (cr.Cells["colResult_C3"].Value.ToString() != "" ? Convert.ToDouble(cr.Cells["colResult_C3"].Value) : 0);
                    mdlDetail.result_c4 = (cr.Cells["colResult_C4"].Value.ToString() != "" ? Convert.ToDouble(cr.Cells["colResult_C4"].Value) : 0);
                    mdlDetails.Add(mdlDetail);
                    //}
                }
            }
            else
            {
                mdlMmCalculatePriceDetails mdlDetail = new mdlMmCalculatePriceDetails();
                mdlDetail.seq = txtSeq.Text;
                mdlDetail.ver = Convert.ToInt32(txtVer.Text);
                mdlDetails.Add(mdlDetail);
            }
            string result = clsMmCalculatePrice.updateMmCalculatePrice(upd_flag, mdlHead, mdlDetails);
            if (result == "")
                MessageBox.Show("儲存記錄失敗!");
            else
            {
                txtId.Text = result;
                if (upd_flag == 2)//如果是新版本的，則在儲存後重新獲取最大的版本號
                    txtVer.Text = clsMmCalculatePrice.getIdVer("Last", txtId.Text).ToString();
                edit_flag = 0;
                upd_flag = 0;
                //int old_row = dgvDetails.CurrentCell.RowIndex;
                append_mode = false;
                edit_mode = false;
                allow_edit = false;
                loadHeadById();
                loadDetailsById();
                loadIdVer();//重新獲取版本號
                setVerTextBoxBackColor();//設定版本號顏色
                setTextBoxEnabled();
                showIdAllVer();//查找Id的所有版本記錄
            }
        }
        private void setSelectRec(int row)
        {
            //移到最後新增的那筆記錄，以便進行編輯
            dgvDetails.SelectionMode = DataGridViewSelectionMode.FullRowSelect;//设置为整行被选中
            //foreach (DataGridViewRow dgr in dgvDetails.Rows)
            //{
            //    //判断单元格值 
            //    if (dgr.Cells[colIndex].Value == value)
            //    {
            //        //设置当前单元格 
            //        dgvDetails.CurrentCell = dgr.Cells[colIndex];
            //        //设置选中状态 
            //        dgr.Cells[colIndex].Selected = true;
            //    }
            //}
            DataGridViewRow CurrentRow = dgvDetails.Rows[row];
            CurrentRow.Cells["colSeq"].Selected = true;
            dgvDetails.CurrentCell = CurrentRow.Cells["colSeq"];//定位到最後的那筆記錄
            CurrentRow.Cells["colVer"].Value = Convert.ToInt32(txtVer.Text);
        }
        //提取主表記錄
        private void loadHeadById()
        {
            string id="";
            id=txtId.Text.Trim()!=""?txtId.Text.Trim():"ZZZZZZZZZ";
            DataTable dt = clsMmCalculatePrice.loadHeadById(id, txtVer.Text != "" ? Convert.ToInt32(txtVer.Text) : 0);
            fillHeadTextBox(dt);
        }
        //提取明細表記錄
        private void loadDetailsById()
        {
            string id = "";
            id = txtId.Text.Trim() != "" ? txtId.Text.Trim() : "ZZZZZZZZZ";
            int old_row = 0;
            if (dgvDetails.Rows.Count > 0)
                old_row = dgvDetails.CurrentCell.RowIndex;//保留原記錄號，以便修改後重新定位回原記錄
            dtPriceDetails = clsMmCalculatePrice.loadDetailsById(id, txtVer.Text != "" ? Convert.ToInt32(txtVer.Text) : 0);
            dgvDetails.DataSource = dtPriceDetails;
            if (dgvDetails.Rows.Count > 0)//保留原記錄號，以便修改後重新定位回原記錄
            {
                setSelectRec(old_row);
                fillTextBox(dgvDetails.CurrentCell.RowIndex);
            }
        }
        private void fillHeadTextBox(DataTable dt)
        {
            cleanTextBox(0);//清空全部文本框
            if (dt.Rows.Count == 0)
                return;
            
            DataRow dr = dt.Rows[0];
            txtVer.Text = dr["ver"].ToString();
            txtCdesc.Text = dr["cdesc"].ToString();
            txtBP.Text = dr["price_bp"].ToString();
            txtOffer_price.Text = dr["offer_price"].ToString();
            txtQtNo.Text = dr["qtno"].ToString();
            txtTotal_A.Text = dr["total_a"].ToString();
            txtTotal_A1.Text = dr["total_a1"].ToString();
            txtTotal_B.Text = dr["total_b"].ToString();
            txtTotal_C1.Text = dr["total_c1"].ToString();
            txtTotal_C2.Text = dr["total_c2"].ToString();
            txtTotal_C3.Text = dr["total_c3"].ToString();
            txtTotal_C4.Text = dr["total_c4"].ToString();
        }
        private void fillTextBox(int rows)
        {
            if (dgvDetails.Rows.Count == 0)
                return;
            allow_edit = false;//將文本框賦值時，設置為不編輯狀態，為了不用重新計算公式
            cleanTextBox(1);//清空配件部分文本框
            DataGridViewRow CurrentRow = dgvDetails.Rows[rows];
            //txtId.Text = CurrentRow.Cells["colId"].Value.ToString();
            txtSeq.Text = CurrentRow.Cells["colSeq"].Value.ToString();
            txtCdesc_D.Text = CurrentRow.Cells["colCdesc_D"].Value.ToString();
            txtMat_type.Text = CurrentRow.Cells["colMat_type"].Value.ToString();
            txtPrd_Type.Text = CurrentRow.Cells["colPrd_Type"].Value.ToString();
            txtArt.Text = CurrentRow.Cells["colArt"].Value.ToString();
            txtSize.Text = CurrentRow.Cells["colSize"].Value.ToString();
            txtColor.Text = CurrentRow.Cells["colColor"].Value.ToString();
            txtMo_id.Text = CurrentRow.Cells["colMo_id"].Value.ToString();
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
            txtResult_A1.Text = CurrentRow.Cells["colResult_A1"].Value.ToString();
            txtResult_B.Text = CurrentRow.Cells["colResult_B"].Value.ToString();
            txtResult_C1.Text = CurrentRow.Cells["colResult_C1"].Value.ToString();
            txtResult_C2.Text = CurrentRow.Cells["colResult_C2"].Value.ToString();
            txtResult_C3.Text = CurrentRow.Cells["colResult_C3"].Value.ToString();
            txtResult_C4.Text = CurrentRow.Cells["colResult_C4"].Value.ToString();
            txtCrusr.Text = CurrentRow.Cells["colCrusr"].Value.ToString();
            txtCrtim.Text = CurrentRow.Cells["colCrtim"].Value.ToString();
            txtAmusr.Text = CurrentRow.Cells["colAmusr"].Value.ToString();
            txtAmtim.Text = CurrentRow.Cells["colAmtim"].Value.ToString();
            allow_edit = true;//賦值後還原為可編輯狀態，可重新計算公式
        }

        private void dgvDetails_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            //edit_flag = 0;
            //append_mode = false;
            //edit_mode = false;
            //allow_edit = false;
            setTextBoxEnabled();
            fillTextBox(dgvDetails.CurrentCell.RowIndex);
        }


        private void Edit()
        {
            edit_flag = 2;
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


        private void txtId_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                SendKeys.Send("{tab}");
            }
        }

        private void btnShowFormula_Click(object sender, EventArgs e)
        {
            getFormulaId(0);
        }
        private void getFormulaId(int type)
        {
            get_formula_type = "Y";
            frmGoodsPriceFormula frmFormulaFind = new frmGoodsPriceFormula();
            frmFormulaFind.ShowDialog();
            if ((get_formula_type!="Y"?get_formula_type:"") != "")
            {
                if (type == 0)//如果是在輸入界面(1)獲取公式
                {
                    txtFid.Text = get_formula_type;
                    fillFormula(txtFid.Text);
                }
                else
                    if (type == 1)//如果是在輸入界面(2)獲取公式
                    {
                        //txtGetFormula.Text = get_formula_type;

                        txtMo_id_W.Focus();

                        //將選中的制單記錄循環加入到計价表中
                        for (int i = 0; i < dgvWipMo.Rows.Count; i++)
                        {
                            DataGridViewRow crMo = dgvWipMo.Rows[i];
                            if ((bool)crMo.Cells["colSelect_mo"].Value == true)
                            {
                                crMo.Cells["colFid_W"].Value = get_formula_type;
                            }
                        }
                    }

                    else
                        dgvWipMo.Rows[dgvWipMo.CurrentCell.RowIndex].Cells["colFid_W"].Value = get_formula_type;//如果是在查詢制單表格中獲取公式
                
            }
            get_formula_type = "";
            frmFormulaFind.Dispose();
        }
        private void fillFormula(string fid)
        {
            if (allow_edit == false)
                return;
            cleanTextBox(2);
            DataTable dtPrice = clsMmCalculatePrice.getGoodsPriceFormula(fid);
            if (dtPrice.Rows.Count == 0)
            {
                MessageBox.Show("公式不存在!");
                return;
            }
            
            
            DataRow dr = dtPrice.Rows[0];
            
            //txtFid.Text = dr["id"].ToString();
            txtMat_type.Text = dr["mat_type"].ToString();
            //txtCdesc.Text = dr["cdesc"].ToString();
            txtNumber_A.Text = dr["number_a"].ToString();
            txtNumber_B.Text = dr["number_b"].ToString();
            //txtNumber_C.Text = dr["number_c"].ToString();
            txtNumber_D.Text = dr["number_d"].ToString();
            txtNumber_E.Text = dr["number_e"].ToString();
            txtNumber_F.Text = dr["number_f"].ToString();
            txtNumber_G.Text = dr["number_g"].ToString();
            txtRate_A.Text = dr["rate_a"].ToString();
            txtRate_D.Text = dr["rate_d"].ToString();
            countUnitPrice();
            //txtResult_A.Text = dr["result_a"].ToString();
            //txtResult_B.Text = dr["result_b"].ToString();
            //txtResult_C1.Text = dr["result_c1"].ToString();
            //txtResult_C2.Text = dr["result_c2"].ToString();
            //txtResult_C3.Text = dr["result_c3"].ToString();
            //txtResult_C4.Text = dr["result_c4"].ToString();
            fillDetailsView();//填入表格
            sumResult();//統計表格中的結果，并填入到表頭
        }

        private void btnFind_Click(object sender, EventArgs e)
        {
            frmMmCalculatePriceFind frmGoodsPriceStd_Find = new frmMmCalculatePriceFind();
            frmGoodsPriceStd_Find.ShowDialog();
            if (get_id != "")
            {
                txtId.Text = get_id;
                txtVer.Text = get_ver.ToString();
                loadHeadById();
                loadDetailsById();
                loadIdVer();
                upd_flag = 0;
                setVerTextBoxBackColor();//設定版本顏色
                showIdAllVer();//查找Id的所有版本記錄

            }
            get_id = "";
            frmGoodsPriceStd_Find.Dispose();
        }

        private void loadIdVer()
        {
            DataTable dtVer = clsMmCalculatePrice.loadIdVer(txtId.Text);
            lpeVer.Properties.ValueMember = "ver";   //相当于editvalue
            lpeVer.Properties.DisplayMember = "ver";    //相当于text
            lpeVer.Properties.DataSource = dtVer;

        }
        //查找Id的所有版本記錄
        private void showIdAllVer()
        {
            DataTable dtId = clsMmCalculatePrice.findMmCalculatePrice(txtId.Text, "", "", "", "", "", "", "", "", "", "", "");
            dgvIdAllVer.DataSource = dtId;
            if (dgvIdAllVer.Rows.Count > 0)
            {
                string id = dgvIdAllVer.Rows[0].Cells["colId_A"].Value.ToString().Trim();
                string ver = dgvIdAllVer.Rows[0].Cells["colVer_A"].Value.ToString().Trim();
                Color cl1 = Color.White;
                Color cl2 = Color.FromArgb(0xCC, 0xFF, 0xFF);
                Color cl3 = Color.White;
                int j = 1;
                int k = 0;
                for (int i = 0; i < dgvIdAllVer.Rows.Count; i++)
                {
                    //dgvDetails.Rows[i].DefaultCellStyle.BackColor = cl3;
                    if (dgvIdAllVer.Rows[i].Cells["colId_A"].Value.ToString().Trim() == id && dgvIdAllVer.Rows[i].Cells["colVer_A"].Value.ToString().Trim() == ver)
                        //dgvDetails.Rows[i].DefaultCellStyle.BackColor = cl3;
                        k = 1;
                    else
                    {
                        //dgvDetails.Rows[i].DefaultCellStyle.BackColor = Color.FromArgb(0xCC, 0xFF, 0xFF);
                        id = dgvIdAllVer.Rows[i].Cells["colId_A"].Value.ToString().Trim();
                        ver = dgvIdAllVer.Rows[i].Cells["colVer_A"].Value.ToString().Trim();
                        j = j + 1;
                        //cl3 = Color.FromArgb(0xCC, 0xFF, 0xFF);
                    }
                    if (j % 2 == 1)
                    {
                        dgvIdAllVer.Rows[i].DefaultCellStyle.BackColor = cl1;
                    }
                    else
                        dgvIdAllVer.Rows[i].DefaultCellStyle.BackColor = cl2;
                }
            }
        }
        private void dgvDetails_Leave(object sender, EventArgs e)
        {
            allow_edit = true;
        }


        private void txtFid_Leave(object sender, EventArgs e)
        {
            fillFormula(txtFid.Text);
            
        }


        private void txtNumber_C_Leave(object sender, EventArgs e)
        {
            countUnitPrice();
            fillDetailsView();//填入表格
            sumResult();//統計表格中的結果，并填入到表頭
        }

        private void txtMat_type_Leave(object sender, EventArgs e)
        {
            fillDetailsView();
        }

        private void setEditStatus()
        {
            if (allow_edit == false)
                return;
            dgvDetails.Rows[dgvDetails.CurrentCell.RowIndex].Cells["colStatus"].Value = "Y";
        }

        private void btnDeleteLine_Click(object sender, EventArgs e)
        {
            if (xTabC1.SelectedTabPageIndex == 1)
            {
                MessageBox.Show("請轉到編輯界面再操作!");
                return;
            }
            if (dgvDetails.Rows.Count == 0)
                return;
            allow_edit = true;
            upd_flag = 1;
            int i = dgvDetails.CurrentCell.RowIndex;
            string id = dgvDetails.Rows[i].Cells["colId"].Value.ToString();
            int ver = dgvDetails.Rows[i].Cells["colVer"].Value != null ? Convert.ToInt32(dgvDetails.Rows[i].Cells["colVer"].Value.ToString()) : 999;
            string seq = dgvDetails.Rows[i].Cells["colSeq"].Value.ToString();
            DataGridViewRow row = dgvDetails.Rows[i];
            dgvDetails.Rows.Remove(row);
            //if (dgvDetails.SelectedRows.Count > 0)
            //{
                //DataRowView drv = dgvDetails.SelectedRows[dgvDetails.CurrentCell.RowIndex].DataBoundItem as DataRowView;
                //drv.Delete();
            //} 
            
            sumResult();//統計表格中的結果，并填入到表頭
            if (clsMmCalculatePrice.checkExistIdSeq(id,ver, seq) == true)
            {
                if (dgvDetails.Rows.Count == 0)//如果明細表已沒有記錄，則標識為3，用來刪除表頭的記錄
                    upd_flag = 3;
                Save();
            }
        }

        private void btnFindMo_Click(object sender, EventArgs e)
        {
            findWipMo();
        }
        private void findWipMo()
        {
            DataTable dtWipMo = clsMmCalculatePrice.findWipMo(txtMo_id_W.Text);
            dgvWipMo.DataSource = dtWipMo;
        }

        private void xTabC1_Click(object sender, EventArgs e)
        {
            fillTextBoxValue();
        }
        private void fillTextBoxValue()
        {
            if (xTabC1.SelectedTabPageIndex == 0)
            {
                if (dgvDetails.Rows.Count > 0)
                {
                    dgvDetails.SelectionMode = DataGridViewSelectionMode.FullRowSelect;//设置为整行被选中
                    int row = dtPriceDetails.Rows.Count - 1;//dgvDetails.CurrentCell.RowIndex;// 
                    DataGridViewRow CurrentRow = dgvDetails.Rows[row];
                    CurrentRow.Cells["colSeq"].Selected = true;
                    dgvDetails.CurrentCell = CurrentRow.Cells["colSeq"];//定位到最後的那筆記錄
                    fillTextBox(row);
                }
            }
            else
                txtMo_id_W.Focus();
        }

        private void dgvWipMo_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewColumn column = dgvWipMo.Columns[e.ColumnIndex];
                if (column is DataGridViewButtonColumn)
                {
                    //这里可以编写你需要的任意关于按钮事件的操作~
                    //MessageBox.Show(e.RowIndex.ToString());

                    getFormulaId(2);



                }
        　　　　　//DGV下拉框的取值
                //MessageBox.Show(dgvWipMo.Rows[e.RowIndex].Cells[e.ColumnIndex - 1].Value.ToString());
            }

        }

        private void btnGetFormula_Click(object sender, EventArgs e)
        {
            getFormulaId(1);
        }

        private void btnNewVer_Click(object sender, EventArgs e)
        {
            if (!clsMmCalculatePrice.checkExistId(txtId.Text, txtVer.Text!=""?Convert.ToInt32(txtVer.Text):0))
            {
                MessageBox.Show("不存在記錄，不能新增新版本!");
                return;
            }
            upd_flag = 2;
            allow_edit = true;
            txtVer.Text = clsMmCalculatePrice.getIdVer("Max",txtId.Text).ToString();
            setVerTextBoxBackColor();
        }
        private void setVerTextBoxBackColor()
        {
            if (upd_flag == 2)
                txtVer.BackColor = Color.Red;
            else
                txtVer.BackColor = SystemColors.Control;
        }
        private void lpeVer_EditValueChanged(object sender, EventArgs e)
        {
            txtVer.Text = lpeVer.EditValue.ToString();
            edit_flag = 0;
            upd_flag = 0;
            allow_edit = true;
            loadHeadById();
            loadDetailsById();
        }

        private void txtQtNo_Leave(object sender, EventArgs e)
        {
            loadQtNo();
            fillDetailsView();
        }
        private void loadQtNo()
        {
            if (txtQtNo.Text.Trim() == "")
                return;
             DataTable dtQtNo = clsMmCalculatePrice.loadQtNo(txtQtNo.Text.Trim());
             if (dtQtNo.Rows.Count > 0)
             {
                 DataRow dr = dtQtNo.Rows[0];
                 txtMat_type.Text = dr["material"].ToString();
                 txtArt.Text = dr["cf_code"].ToString();
                 txtSize.Text = dr["size"].ToString();
                 txtColor.Text = dr["cf_color"].ToString();
                 txtMo_id.Text = dr["mo_id"].ToString();
                 txtBP.Text = dr["number_enter"].ToString();
             }
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            txtId.Text = "";
            txtVer.Text = "0";
            lpeVer.Properties.DataSource = null;
            append_mode = true;
            edit_mode = true;
            allow_edit = true;
            upd_flag = 0;
            setTextBoxEnabled();
            edit_flag = 1;
        }

        private void dgvIdAllVer_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            txtId.Text = dgvIdAllVer.Rows[dgvIdAllVer.CurrentCell.RowIndex].Cells["colId_A"].Value.ToString().Trim();
            txtVer.Text = dgvIdAllVer.Rows[dgvIdAllVer.CurrentCell.RowIndex].Cells["colVer_A"].Value.ToString().Trim();
            loadHeadById();
            loadDetailsById();
        }

    }
}
