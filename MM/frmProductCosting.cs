using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using cf01.MDL;
using cf01.CLS;
using cf01.Forms;
using cf01.Reports;
using DevExpress.XtraReports.UI;

namespace cf01.MM
{
    public partial class frmProductCosting : Form
    {
        public static string searchProductId = "";
        public static string searchProductMo = "";
        public static string searchProductName = "";
        public static string searchDepId = "";
        public static decimal searchPrice = 0;
        public static mdlDepPrice sentDepPrice = new mdlDepPrice();
        private bool firstLevel;
        private bool firstCount;
        private DataTable dtBomDetails = new DataTable();
        public frmProductCosting()
        {
            InitializeComponent();
        }
        private void frmProductCosting_Load(object sender, EventArgs e)
        {
            dgvBomDetails.AutoGenerateColumns = false;
            initBomDataTable();
        }
        private void btnExit_Click(object sender, EventArgs e)
        {
            searchProductId = "";
            searchProductMo = "";
            searchProductName = "";
            searchDepId = "";
            searchPrice = 0;
            this.Close();
        }

        private void btnFind_Click(object sender, EventArgs e)
        {
            searchProductId = txtProductId.Text;
            frmProductCostingFind frmProductCostingFind = new frmProductCostingFind();
            frmProductCostingFind.ShowDialog();
            if (searchProductId != "")
            {
                txtProductId.Text = searchProductId;
                txtProductName.Text = searchProductName;
                txtProductMo.Text = searchProductMo;
                firstCount = true;
                showBomTree(searchProductMo,searchProductId,searchProductName);
                firstCount = false;
            }
            frmProductCostingFind.Dispose();
            chkSelectAll.Checked = false;
        }
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            if (dgvBomDetails.Rows.Count == 0)
                return;
            DataGridViewRow dgr = dgvBomDetails.Rows[0];
            txtProductId.Text = dgr.Cells["colProductId"].Value.ToString();
            txtProductName.Text = dgr.Cells["colProductName"].Value.ToString();
            //txtProductMo.Text = dgr.Cells["colProductMo"].Value.ToString();
            showBomTree(txtProductMo.Text, txtProductId.Text, txtProductName.Text);
            chkSelectAll.Checked = false;
        }
        private void showBomTree(string productMo,string productId,string productName)
        {
            if (txtProductId.Text.Trim() == "")
            {
                MessageBox.Show("沒有產品記錄!");
                return;
            }
            frmProgress wForm = new frmProgress();
            new Thread((ThreadStart)delegate
            {
                wForm.TopMost = true;
                wForm.ShowDialog();
            }).Start();

            //**********************
            initTreeView(productMo,productId,productName); //数据处理

            //genBomTree(pid);
            //**********************
            wForm.Invoke((EventHandler)delegate { wForm.Close(); });
        }
        protected void initTreeView(string productMo, string productId, string productName)
        {
            initBomDataTable();
            tvBom.Nodes.Clear();

            DataTable dtWipData = clsProductCosting.getWipData(productMo);
            dgvWipData.DataSource = dtWipData;
            //添加主菜单
            TreeNode TopNode;
            //獲取首層F0
            //DataTable dtBom_h = clsProductCosting.getBomPid(pid);
            //if (dtBom_h.Rows.Count > 0)
            //{
                //string id = dtBom_h.Rows[0]["id"].ToString();
                //string goods_id = dtBom_h.Rows[0]["goods_id"].ToString();
                //string goods_name = dtBom_h.Rows[0]["goods_name"].ToString();
                TopNode = new TreeNode();
                TopNode.Text = productId + "[" + productName + "]";
                TopNode.Tag = productId;//goods_id ;//保存表單名
                TopNode.ImageIndex = 2;
                tvBom.Nodes.Add(TopNode);
                addChildNodeBom(TopNode, productId);//递归调用
                tvBom.ExpandAll();//展開
                doBomTree();
            //}

        }

        /// <summary>
        /// 递归调用方法，添加菜单的子菜单
        /// </summary>
        /// <param name="tsi"></param>
        public void addChildNodeBom(TreeNode subNode, string pid)
        {
            TreeNode subNode1;
            DataTable dtBom_d = clsProductCosting.getBomCid(pid);
            if (dtBom_d.Rows.Count == 0)
            {
                subNode.ImageIndex = 3;
            }
            else
            {
                for (int i = 0; i < dtBom_d.Rows.Count; i++)
                {
                    string ppid = dtBom_d.Rows[i]["d_goods_id"].ToString();
                    string goods_name = dtBom_d.Rows[i]["d_goods_name"].ToString();
                    subNode1 = new TreeNode();//实例化一个菜单项
                    subNode1.Text = ppid + "[" + goods_name + "]";//实例化一个菜单项
                    subNode1.Tag = ppid;//保存表單名
                    subNode1.ImageIndex = 2;
                    subNode.Nodes.Add(subNode1);
                    addChildNodeBom(subNode1, ppid);//递归调用的方法                     
                    //InitMenu(ppid);
                    //}
                }
            }
        }
        protected void genBomTree(string pid)
        {
            #region 從生產計劃中生成BOM,暫不用
            initBomDataTable();
            tvBom.Nodes.Clear();
            string productMo = txtProductMo.Text.Trim();
            DataTable dtWipData=clsProductCosting.getWipData(productMo);
            dgvWipData.DataSource = dtWipData;
            //添加主菜单
            TreeNode TopNode;
            //獲取首層F0
            //DataTable dtBom_h = clsProductCosting.getBomFromOc(productMo, pid);
            //if (dtBom_h.Rows.Count > 0)
            //{
                //string goods_id = dtBom_h.Rows[0]["goods_id"].ToString();
                //string goods_name = dtBom_h.Rows[0]["goods_name"].ToString();
                TopNode = new TreeNode();
                TopNode.Text = searchProductId + "[" + searchProductName + "]";
                TopNode.Tag = searchProductId;//id;//保存表單名
                TopNode.ImageIndex = 2;
                tvBom.Nodes.Add(TopNode);
                addChildNode(TopNode, productMo, searchProductId);//递归调用
                tvBom.ExpandAll();//展開
                doBomTree();
                //TopNode.ImageIndex = TopNode.SelectedImageIndex = 2;                        
            //}
            #endregion
        }
        /// <summary>
        /// 递归调用方法，添加菜单的子菜单
        /// </summary>
        /// <param name="tsi"></param>
        public void addChildNode(TreeNode subNode, string mo_id, string pid)
        {
            #region 從生產計劃中生成BOM,暫不用
            TreeNode subNode1;
            DataTable dtBom_d = clsProductCosting.getBomCidFromWip(mo_id, pid);
            if (dtBom_d.Rows.Count == 0)
            {
                subNode.ImageIndex = 3;
            }
            else
            {
                for (int i = 0; i < dtBom_d.Rows.Count; i++)
                {
                    string ppid = dtBom_d.Rows[i]["d_goods_id"].ToString();
                    string goods_name = dtBom_d.Rows[i]["d_goods_name"].ToString();
                    //string dept_id = "";
                    ////獲取配件的生產部門
                    //DataTable dtDep = clsProductCosting.getBomItemDepFromWip(mo_id, ppid);
                    //if (dtDep.Rows.Count > 0)
                    //    dept_id = dtDep.Rows[0]["dept_id"].ToString();
                    subNode1 = new TreeNode();//实例化一个菜单项
                    subNode1.Text = ppid + "[" + goods_name + "]";//实例化一个菜单项
                    subNode1.Tag = ppid;// d_id;//保存表單名
                    subNode1.ImageIndex = 2;
                    subNode.Nodes.Add(subNode1);
                    addChildNode(subNode1, mo_id, ppid);//递归调用的方法
                    //InitMenu(ppid);
                    //}
                }
            }
            #endregion
        }

        //Bom在Tree中顯示後，再遞歸Tree控件，將所有子件加入到Table中，以表格的形式顯示
        private void doBomTree()
        {
            #region 递归
            //1.获取TreeView的所有根节点
            foreach (TreeNode tn in tvBom.Nodes)
            {
                expandBomTree(tn);
            }
            dgvBomDetails.DataSource = dtBomDetails;
            if (dtBomDetails.Rows.Count > 0)
            {
                
                for (int i = dtBomDetails.Rows.Count - 1; i >= 0; i--)
                {
                    //重新查找數據後，若該主件是未設定成本的，則自動計算每一件的成本：從最後的記錄開始，倒序重新計算每件的子件累計成本及產品成本，直到頂層
                    //if ((bool)dgvBomDetails.Rows[i].Cells["colIsSetFlag"].Value == false)
                        //countAllItemCost(dgvBomDetails.Rows.Count - 1);
                        countProductCostingRoll(i);
                }
                        //并初始化將文本框記錄顯示定位到頂層主件的記錄
                fillControlsValue(0);
            }
            #endregion
        }

        private void expandBomTree(TreeNode tn)
        {
            //1.将当前节点显示到lable上
            string bomLevel = "";
            string parentLevel = "";
            string productName = "";
            string tnText = "";
            if (tn.Parent != null)
            {
                parentLevel = tn.Parent.Level.ToString();
            }
            else
            {
                parentLevel = "--";

            }
            bomLevel = tn.Level.ToString();
            tnText = tn.Text.Trim();
            productName = tnText.Substring(tnText.IndexOf("[") + 1, (tnText.Length - (tnText.IndexOf("[") + 1) - 1));
            addBomToTable(parentLevel, bomLevel, tn.Tag.ToString(), productName);
            foreach (TreeNode tnSub in tn.Nodes)
            {
                expandBomTree(tnSub);
            }
        }
        //將每個子件插入到表格，同時查找若存在單價設定的就顯示，若沒有的，就從生產流程中獲取初始值
        private void addBomToTable(string parentLevel, string bomLevel, string productId, string productName)
        {
            string productMo = txtProductMo.Text.Trim();
            DataTable dtCost = new DataTable();
            dtCost = clsProductCosting.getProductCosting(productId);
            DataRow dr2 = dtBomDetails.NewRow();
            dr2["ParentLevel"] = parentLevel;
            dr2["BomLevel"] = bomLevel;
            dr2["ProductId"] = productId;
            dr2["ProductName"] = productName;
            dr2["IsSelect"] = false;
            if (dtCost.Rows.Count > 0)
            {
                DataRow dr = dtCost.Rows[0];
                dr2["IsSetFlag"] = true;
                dr2["ProductMo"] = dr["ProductMo"].ToString();
                dr2["ProductWeight"] = dr["ProductWeight"].ToString() != "" ? dr["ProductWeight"].ToString() : "0";
                dr2["OriginWeight"] = dr["OriginWeight"].ToString() != "" ? dr["OriginWeight"].ToString() : "0";
                dr2["WasteRate"] = dr["WasteRate"].ToString();
                dr2["MaterialRequest"] = dr["MaterialRequest"].ToString();
                dr2["OriginalPrice"] = dr["OriginalPrice"].ToString();
                dr2["MaterialPrice"] = dr["MaterialPrice"].ToString();
                dr2["MaterialCost"] = dr["MaterialCost"].ToString();
                dr2["RollUpCost"] = dr["RollUpCost"].ToString();
                dr2["DepId"] = dr["DepId"].ToString();
                dr2["DepCdesc"] = dr["DepCdesc"].ToString();
                dr2["DepPrice"] = dr["DepPrice"].ToString();
                dr2["DepStdPrice"] = dr["DepStdPrice"].ToString() != "" ? Convert.ToDecimal(dr["DepStdPrice"]) : 0;
                dr2["DepStdQty"] = dr["DepStdQty"].ToString() != "" ? Convert.ToDecimal(dr["DepStdQty"]) : 0;
                dr2["DepCost"] = dr["DepCost"].ToString();
                dr2["OtherCost1"] = dr["OtherCost1"].ToString() != "" ? dr["OtherCost1"].ToString() : "0";
                dr2["OtherCost2"] = dr["OtherCost2"].ToString() != "" ? dr["OtherCost2"].ToString() : "0";
                dr2["OtherCost3"] = dr["OtherCost3"].ToString() != "" ? dr["OtherCost3"].ToString() : "0";
                dr2["DepTotalCost"] = dr["DepTotalCost"].ToString() != "" ? dr["DepTotalCost"].ToString() : "0";
                dr2["ProductCost"] = dr["ProductCost"].ToString() != "" ? dr["ProductCost"].ToString() : "0";
                dr2["ProductCostGrs"] = dr["ProductCostGrs"].ToString() != "" ? dr["ProductCostGrs"].ToString() : "0";
                dr2["ProductCostK"] = dr["ProductCostK"].ToString() != "" ? dr["ProductCostK"].ToString() : "0";
                dr2["ProductCostDzs"] = dr["ProductCostDzs"].ToString() != "" ? dr["ProductCostDzs"].ToString() : "0";
                dr2["DoColor"] = dr["DoColor"].ToString();
            }
            else
            {
                dr2["IsSetFlag"] = false;
                decimal materialPrice = 0;
                decimal wasteRate = 1;
                decimal depPrice = 0;
                string depId = "", depCdesc = "", doColor = "";
                dr2["OriginWeight"] = 0;
                dr2["ProductWeight"] = 0;
                dr2["MaterialPrice"] = 0;
                dr2["WasteRate"] = 1;
                dr2["DepStdPrice"] = 0;
                dr2["DepStdQty"] = 0;
                DataTable dtDep = clsProductCosting.getProductDepFromBom(productId);
                if (dtDep.Rows.Count > 0)
                {
                    depId = dtDep.Rows[0]["dept_id"].ToString();
                    depCdesc = dtDep.Rows[0]["DepCdesc"].ToString();
                    doColor = dtDep.Rows[0]["DoColor"].ToString();
                    dr2["DepId"] = depId;
                    dr2["DepCdesc"] = depCdesc;
                    dr2["DoColor"] = doColor;
                    wasteRate = clsProductCosting.getDepWasteRate(depId);//部門損耗率
                }
                //如果是原料或採購料，則從採購單中提取原料單價
                if (productId.Substring(0, 2) == "ML" || productId.Substring(0, 2) == "PL")
                {
                    if (productId.Substring(0, 2) == "ML")
                    {
                        depId = "802";
                        depCdesc = "原料倉";
                    }
                    wasteRate = clsProductCosting.getDepWasteRate(depId);//部門損耗率
                    DataTable dt = clsProductCosting.findMaterialPrice(productId, "");//從採購單中提取原料單價
                    if (dt.Rows.Count > 0)
                    {
                        materialPrice = dt.Rows[0]["price_g"].ToString() != "" ? Convert.ToDecimal(dt.Rows[0]["price_g"].ToString()) : 0;
                        dr2["OriginalPrice"] = dt.Rows[0]["PriceHkd"].ToString() != "" ? Convert.ToDecimal(dt.Rows[0]["PriceHkd"].ToString()) : 0;
                    }
                    if (productId.Substring(0, 2) == "PL")
                    {
                        dr2["ProductWeight"] = clsProductCosting.getProductWeight("PL", productMo, productId);
                    }
                    dr2["OriginWeight"] = dr2["ProductWeight"];
                    dr2["DepId"] = depId;
                    dr2["DepCdesc"] = depCdesc;
                    dr2["MaterialPrice"] = materialPrice;
                    dr2["WasteRate"] = wasteRate;
                    ////原料/膠料/噴油/挂電都是按粒計算成本的，所以不用乘以重量 (1-2019/12/14日取消，不用計算，在countProductCostingRoll中計算)
                    //dr2["MaterialCost"] = Math.Round(materialPrice * wasteRate, 4);
                }
                else
                {
                    //從計劃單中查找匹配的物料編號，并計算物料成本
                    for (int i = 0; i < dgvWipData.Rows.Count; i++)
                    {
                        if (productId == dgvWipData.Rows[i].Cells["colWipGoodsId"].Value.ToString())
                        {
                            DataGridViewRow dr = dgvWipData.Rows[i];
                            if (depId == "")
                            {
                                depId = dr.Cells["colWipWpId"].Value.ToString();
                                depCdesc = dr.Cells["colWipDepCdesc"].Value.ToString();
                                dr2["DepId"] = depId;
                                dr2["DepCdesc"] = depCdesc;
                                dr2["DoColor"] = dr.Cells["colWipDoColor"].Value.ToString();
                                wasteRate = clsProductCosting.getDepWasteRate(depId);
                            }
                            dr2["ProductMo"] = dr.Cells["colWipProductMo"].Value.ToString();
                            dr2["ProductWeight"] = dr.Cells["colWipPcsWeg"].Value;
                            if ((dr2["ProductWeight"].ToString() != "" ? Convert.ToDecimal(dr2["ProductWeight"]) : 0) == 0)
                                dr2["ProductWeight"] = clsProductCosting.getProductWeight("", productMo, productId);
                            dr2["OriginWeight"] = dr2["ProductWeight"];
                            if (depId == "501" || depId == "510")
                            {
                                //默認從之前的外發加工單中提取單價
                                DataTable dt = clsProductCosting.findPlatePrice(depId, productId, "");
                                if (dt.Rows.Count > 0)
                                {
                                    dr2["OriginalPrice"] = dt.Rows[0]["price_kg"].ToString() != "" ? Convert.ToDecimal(dt.Rows[0]["price_kg"].ToString()) : 0;
                                    materialPrice = dt.Rows[0]["price_g"].ToString() != "" ? Convert.ToDecimal(dt.Rows[0]["price_g"].ToString()) : 0;
                                }
                                dr2["MaterialPrice"] = materialPrice;
                                dr2["WasteRate"] = wasteRate;
                                ////原料/膠料/噴油/挂電都是按粒計算成本的，所以不用乘以重量(2-2019/12/14日取消，不用計算)
                                //if (depId == "510" || (depId == "501" && dr2["DoColor"].ToString().IndexOf("挂電") > 0))
                                //    dr2["MaterialCost"] = Math.Round(wasteRate * materialPrice, 4);
                                //else
                                //    dr2["MaterialCost"] = Math.Round((dr2["ProductWeight"].ToString() != "" ? Convert.ToDecimal(dr2["ProductWeight"]) : 0) * wasteRate * materialPrice, 4);
                            }
                            else
                            {
                                dr2["WasteRate"] = wasteRate;
                                dr2["MaterialRequest"] = Math.Round((dr2["ProductWeight"].ToString() != "" ? Convert.ToDecimal(dr2["ProductWeight"].ToString()) : 0)
                                    * wasteRate, 4);
                                //if (productId.Substring(0, 2) == "PL")//如果是膠料，成本就是每PCS的價錢了，所以不用再乘以重量的(3-2019/12/14日取消，不用計算)
                                //    dr2["MaterialCost"] = materialPrice;
                                //else
                                //    dr2["MaterialCost"] = Math.Round((dr2["MaterialRequest"].ToString() != "" ? Convert.ToDecimal(dr2["MaterialRequest"].ToString()) : 0)
                                //* materialPrice, 4);
                            }
                            break;
                        }
                    }
                }
                //獲取部門的加工單價
                DataTable dtDepPrice=clsProductCosting.getDepPrice(dr2["DepId"].ToString(), productId);// getDepPrice(dr2["DepId"].ToString(), productId);
                if (dtDepPrice.Rows.Count > 0)
                {
                    depPrice = Math.Round((dtDepPrice.Rows[0]["cost_price"].ToString() != "" ? Convert.ToDecimal(dtDepPrice.Rows[0]["cost_price"]) : 0)
                        / (dtDepPrice.Rows[0]["product_qty"].ToString() != "" ? Convert.ToDecimal(dtDepPrice.Rows[0]["product_qty"]) : 1)
                        , 4);
                    dr2["DepStdPrice"] = dtDepPrice.Rows[0]["cost_price"].ToString();
                    dr2["DepStdQty"] = dtDepPrice.Rows[0]["product_qty"].ToString();
                }
                dr2["DepPrice"] = depPrice;
                dr2["DepCost"] = dr2["DepPrice"];
                //部門總成本(4-2019/12/14日取消，不用計算)
                //dr2["DepTotalCost"] = Math.Round((dr2["DepCost"].ToString() != "" ? Convert.ToDecimal(dr2["DepCost"].ToString()) : 0)
                //    + (dr2["MaterialCost"].ToString() != "" ? Convert.ToDecimal(dr2["MaterialCost"].ToString()) : 0)
                //    + (dr2["OtherCost1"].ToString() != "" ? Convert.ToDecimal(dr2["OtherCost1"].ToString()) : 0)
                //    + (dr2["OtherCost2"].ToString() != "" ? Convert.ToDecimal(dr2["OtherCost2"].ToString()) : 0)
                //    + (dr2["OtherCost3"].ToString() != "" ? Convert.ToDecimal(dr2["OtherCost3"].ToString()) : 0)
                //    , 4);
            }
            dtBomDetails.Rows.Add(dr2);
        }


        private void initBomDataTable()
        {
            dtBomDetails = clsProductCosting.getProductCosting("");
            //將BOM填入到表中
            dtBomDetails.Columns.Add("ParentLevel", typeof(string)); //数据类型为 文本
            dtBomDetails.Columns.Add("BomLevel", typeof(string));
            dtBomDetails.Columns.Add("IsSetFlag", typeof(bool));
            dtBomDetails.Columns.Add("IsSelect", typeof(bool));
            dgvBomDetails.DataSource = dtBomDetails;
        }


        private void fillControlsValue(int row)
        {
            DataGridViewRow dgr = dgvBomDetails.Rows[row];
            string currentLevel = dgr.Cells["colBomLevel"].Value.ToString();
            txtProductId.Text = dgr.Cells["colProductId"].Value.ToString();
            txtProductName.Text = dgr.Cells["colProductName"].Value.ToString();
            txtDoColor.Text = dgr.Cells["colDoColor"].Value.ToString();
            txtProductWeight.Text = dgr.Cells["colProductWeight"].Value.ToString();
            txtOriginWeight.Text = dgr.Cells["colOriginWeight"].Value.ToString();
            txtWasteRate.Text = dgr.Cells["colWasteRate"].Value.ToString();
            txtMaterialRequest.Text = dgr.Cells["colMaterialRequest"].Value.ToString();
            txtOriginalPrice.Text = dgr.Cells["colOriginalPrice"].Value.ToString();
            txtMaterialPrice.Text = dgr.Cells["colMaterialPrice"].Value.ToString();
            txtMaterialCost.Text = dgr.Cells["colMaterialCost"].Value.ToString();
            txtDepId.Text = dgr.Cells["colDepId"].Value.ToString();
            txtDepCdesc.Text = dgr.Cells["colDepCdesc"].Value.ToString();
            txtDepPrice.Text = dgr.Cells["colDepPrice"].Value.ToString();
            txtDepCost.Text = dgr.Cells["colDepCost"].Value.ToString();
            txtOtherCost1.Text = dgr.Cells["colOtherCost1"].Value.ToString();
            txtOtherCost2.Text = dgr.Cells["colOtherCost2"].Value.ToString();
            txtOtherCost3.Text = dgr.Cells["colOtherCost3"].Value.ToString();
            txtProductCost.Text = dgr.Cells["colProductCost"].Value.ToString();
            txtDepTotalCost.Text = dgr.Cells["colDepTotalCost"].Value.ToString();
            txtProductCostGrs.Text = dgr.Cells["colProductCostGrs"].Value.ToString();
            txtProductCostK.Text = dgr.Cells["colProductCostK"].Value.ToString();
            txtProductCostDzs.Text = dgr.Cells["colProductCostDzs"].Value.ToString();
            txtRollUpCost.Text = dgr.Cells["colRollUpCost"].Value.ToString();
            //countRollUpCost(row, currentLevel);
            //判斷下一層是否原料層
            firstLevel = getFirstLevel(currentLevel, row);
            
            setControlsDesc(txtDepId.Text.Trim(),firstLevel);
            selectdgvWipDataRow(txtProductId.Text);
        }
        //檢查當層是否最開始的層
        private bool getFirstLevel(string currentLevel,int row)
        {
            bool firstLevelFlag = false;
            int nextRow = row + 1;
            if (nextRow < dgvBomDetails.Rows.Count)
            {
                if (currentLevel == dgvBomDetails.Rows[nextRow].Cells["colParentLevel"].Value.ToString() && dgvBomDetails.Rows[nextRow].Cells["colProductId"].Value.ToString().Substring(0, 2) == "ML")
                    firstLevelFlag = true;
                else
                    firstLevelFlag = false;
            }
            return firstLevelFlag;
        }
        ////計算子件成本
        //private void countRollUpCost(int row,string currentLevel)
        //{
        //    decimal rollUpCost = 0;
        //    for (int i = row+1; i < dgvBomDetails.Rows.Count; i++)
        //    {
        //        DataGridViewRow dgr1 = dgvBomDetails.Rows[i];
        //        string nowLevel = dgr1.Cells["colBomLevel"].Value.ToString();
        //        string parentLevel = dgr1.Cells["colParentLevel"].Value.ToString();
        //        if (currentLevel == nowLevel)
        //            break;
        //        else
        //        {
        //            if (currentLevel == parentLevel)
        //                rollUpCost += (dgr1.Cells["colProductCost"].Value.ToString() != "" ? Convert.ToDecimal(dgr1.Cells["colProductCost"].Value) : 0);
        //        }
        //    }
        //    txtRollUpCost.Text = rollUpCost.ToString();
        //    DataGridViewRow dgr = dgvBomDetails.Rows[dgvBomDetails.CurrentRow.Index];
        //    dgr.Cells["colRollUpCost"].Value = txtRollUpCost.Text != "" ? Convert.ToDecimal(txtRollUpCost.Text) : 0;
        //}
        private void setControlsDesc(string depId,bool firstLevel)
        {
            bool vFlag = true;
            bool tEnabled = firstLevel;
            if (depId == "501")
            {
                vFlag = false;
                tEnabled = true;
                lblOriginalPrice.Text = "電鍍單價:";
                lblWasteRate.Text = "電鍍損耗:";
                lblMaterialPrice.Text = "電鍍單價(G):";
                lblMaterialCost.Text = "電鍍成本(G):";
            }
            else if (depId == "510")
            {
                vFlag = false;
                tEnabled = true;
                lblOriginalPrice.Text = "噴油單價:";
                lblWasteRate.Text = "噴油損耗:";
                lblMaterialPrice.Text = "噴油單價(G):";
                lblMaterialCost.Text = "噴油成本(G):";
            }
            else
            {
                vFlag = true;
                lblOriginalPrice.Text = "原始單價:";
                lblWasteRate.Text = "原料損耗:";
                lblMaterialPrice.Text = "原料單價(G):";
                lblMaterialCost.Text = "原料成本(G):";
                if (firstLevel == true)
                    tEnabled = true;
                else
                    tEnabled = false;
            }
            lblMaterialRequest.Visible = vFlag;
            txtMaterialRequest.Visible = vFlag;
            //txtOriginalPrice.Enabled = tEnabled;
            //txtMaterialPrice.Enabled = tEnabled;
            //txtMaterialCost.Enabled = tEnabled;
        }
        private void tvBom_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            string productId = e.Node.Tag.ToString();
            for (int i = 0; i < dgvBomDetails.Rows.Count; i++)
            {
                if (productId == dgvBomDetails.Rows[i].Cells["colProductId"].Value.ToString())
                {
                    selectDgvBomDetailsRow(i);
                    fillControlsValue(i);
                    break;
                }
            }

        }
        private void selectDgvBomDetailsRow(int row)
        {
            //移到最後新增的那筆記錄，以便進行編輯
            dgvBomDetails.SelectionMode = DataGridViewSelectionMode.FullRowSelect;//设置为整行被选中
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
            DataGridViewRow CurrentRow = dgvBomDetails.Rows[row];
            CurrentRow.Cells["colProductId"].Selected = true;
            dgvBomDetails.CurrentCell = CurrentRow.Cells["colProductId"];//定位到最後的那筆記錄

        }
        private void selectdgvWipDataRow(string productId)
        {
            for (int i = 0; i < dgvWipData.Rows.Count; i++)
            {
                if (productId == dgvWipData.Rows[i].Cells["colWipGoodsId"].Value.ToString())
                {
                    //移到最後新增的那筆記錄，以便進行編輯
                    dgvWipData.SelectionMode = DataGridViewSelectionMode.FullRowSelect;//设置为整行被选中
                    DataGridViewRow CurrentRow = dgvWipData.Rows[i];
                    CurrentRow.Cells["colWipSeq"].Selected = true;
                    dgvWipData.CurrentCell = CurrentRow.Cells["colWipSeq"];//定位到最後的那筆記錄
                }
            }

        }
        private void countProductCosting()
        {
            decimal rollUpCost = txtRollUpCost.Text != "" ? Convert.ToDecimal(txtRollUpCost.Text) : 0;
            decimal depCost = txtDepCost.Text != "" ? Convert.ToDecimal(txtDepCost.Text) : 0;
            decimal otherCost1 = txtOtherCost1.Text != "" ? Convert.ToDecimal(txtOtherCost1.Text) : 0;
            decimal otherCost2 = txtOtherCost2.Text != "" ? Convert.ToDecimal(txtOtherCost2.Text) : 0;
            decimal otherCost3 = txtOtherCost3.Text != "" ? Convert.ToDecimal(txtOtherCost3.Text) : 0;
            decimal materialCost = txtMaterialCost.Text != "" ? Convert.ToDecimal(txtMaterialCost.Text) : 0;
            decimal productCost = 0;
            productCost = Math.Round(rollUpCost + materialCost + depCost + otherCost1 + otherCost2 + otherCost3, 4);
            txtDepTotalCost.Text = Math.Round(materialCost + depCost + otherCost1 + otherCost2 + otherCost3, 4).ToString();
            txtProductCost.Text = productCost.ToString();
            txtProductCostGrs.Text = Math.Round(productCost * 144, 4).ToString();
            txtProductCostK.Text = Math.Round(productCost * 1000, 4).ToString();
            txtProductCostDzs.Text = Math.Round(productCost * 12, 4).ToString();
        }

        private void txtProductWeight_Leave(object sender, EventArgs e)
        {
            countMaterialCost();
            fillDgvBomDetails();
        }

        private void txtOriginalPrice_Leave(object sender, EventArgs e)
        {
            txtMaterialPrice.Text = (txtOriginalPrice.Text != "" ? Math.Round(Convert.ToDecimal(txtOriginalPrice.Text) / 1000, 4) : 0).ToString();
            countMaterialCost();
            fillDgvBomDetails();
        }

        private void txtDepPrice_Leave(object sender, EventArgs e)
        {
            txtDepCost.Text = (txtDepPrice.Text != "" ? Convert.ToDecimal(txtDepPrice.Text) : 0).ToString();
            countProductCosting();
            fillDgvBomDetails();
        }

        private void txtRollUpCost_Leave(object sender, EventArgs e)
        {
            countProductCosting();
            fillDgvBomDetails();
        }

        private void txtMaterialPrice_Leave(object sender, EventArgs e)
        {
            countMaterialCost();
        }
        private void countMaterialCost()
        {
            decimal wasteRate = txtWasteRate.Text != "" ? Convert.ToDecimal(txtWasteRate.Text) : 0;
            wasteRate = wasteRate == 0 ? 1 : wasteRate;
            decimal materialPrice = txtMaterialPrice.Text != "" ? Convert.ToDecimal(txtMaterialPrice.Text) : 0;
            decimal productWeight = txtProductWeight.Text != "" ? Convert.ToDecimal(txtProductWeight.Text) : 0;
            if (firstLevel == true)
            {
                decimal materialRequest = Math.Round(productWeight * wasteRate, 4);
                txtMaterialRequest.Text = materialRequest.ToString();
                txtMaterialCost.Text = Math.Round(materialRequest * materialPrice, 4).ToString();
            }
            else
            {
                //膠料、噴油、挂電的成本是按粒計算的，所以不用乘以重量
                if (txtProductId.Text.Substring(0, 2) == "PL"||txtDepId.Text.Trim() == "510"||(txtDepId.Text.Trim() == "501"&&txtDoColor.Text.IndexOf("挂電")>0))
                    txtMaterialCost.Text = Math.Round(wasteRate * materialPrice, 4).ToString();
                else
                    txtMaterialCost.Text = Math.Round(productWeight * wasteRate * materialPrice, 4).ToString();
            }
            countProductCosting();
        }
        //當文本框的值改變時，同時更新表格的對應值，并自動更新每一件的成本：從當前記錄開始，倒序重新計算每件的子件累計成本及產品成本，直到頂層
        private void fillDgvBomDetails()
        {
            int row=dgvBomDetails.CurrentRow.Index;
            DataGridViewRow dgr = dgvBomDetails.Rows[row];
            dgr.Cells["colRollUpCost"].Value = txtRollUpCost.Text != "" ? Convert.ToDecimal(txtRollUpCost.Text) : 0;
            dgr.Cells["colProductWeight"].Value = txtProductWeight.Text != "" ? Convert.ToDecimal(txtProductWeight.Text) : 0;
            dgr.Cells["colWasteRate"].Value = txtWasteRate.Text != "" ? Convert.ToDecimal(txtWasteRate.Text) : 0;
            dgr.Cells["colMaterialRequest"].Value = txtMaterialRequest.Text != "" ? Convert.ToDecimal(txtMaterialRequest.Text) : 0;
            dgr.Cells["colOriginalPrice"].Value = txtOriginalPrice.Text != "" ? Convert.ToDecimal(txtOriginalPrice.Text) : 0;
            dgr.Cells["colMaterialPrice"].Value = txtMaterialPrice.Text != "" ? Convert.ToDecimal(txtMaterialPrice.Text) : 0;
            dgr.Cells["colMaterialCost"].Value = txtMaterialCost.Text != "" ? Convert.ToDecimal(txtMaterialCost.Text) : 0;
            dgr.Cells["colDepPrice"].Value = txtDepPrice.Text != "" ? Convert.ToDecimal(txtDepPrice.Text) : 0;
            dgr.Cells["colDepCost"].Value = txtDepCost.Text != "" ? Convert.ToDecimal(txtDepCost.Text) : 0;
            dgr.Cells["colOtherCost1"].Value = txtOtherCost1.Text != "" ? Convert.ToDecimal(txtOtherCost1.Text) : 0;
            dgr.Cells["colOtherCost2"].Value = txtOtherCost2.Text != "" ? Convert.ToDecimal(txtOtherCost2.Text) : 0;
            dgr.Cells["colOtherCost3"].Value = txtOtherCost3.Text != "" ? Convert.ToDecimal(txtOtherCost3.Text) : 0;
            dgr.Cells["colProductCost"].Value = txtProductCost.Text != "" ? Convert.ToDecimal(txtProductCost.Text) : 0;
            dgr.Cells["colDepTotalCost"].Value = txtDepTotalCost.Text != "" ? Convert.ToDecimal(txtDepTotalCost.Text) : 0;
            dgr.Cells["colProductCostGrs"].Value = txtProductCostGrs.Text != "" ? Convert.ToDecimal(txtProductCostGrs.Text) : 0;
            dgr.Cells["colProductCostK"].Value = txtProductCostK.Text != "" ? Convert.ToDecimal(txtProductCostK.Text) : 0;
            dgr.Cells["colProductCostDzs"].Value = txtProductCostDzs.Text != "" ? Convert.ToDecimal(txtProductCostDzs.Text) : 0;
            countProductCostInGrid(row);
            //可以使用，暫時取消
            //countAllItemCost(row);//自動更新每一件的成本：從當前記錄開始，倒序重新計算每件的子件累計成本及產品成本，直到頂層


            countProductCostingRoll(row);
        }

        private void txtProductCost_Leave(object sender, EventArgs e)
        {
            decimal productCost = txtProductCost.Text != "" ? Convert.ToDecimal(txtProductCost.Text) : 0;
            txtProductCostGrs.Text = Math.Round(productCost * 144, 4).ToString();
            txtProductCostK.Text = Math.Round(productCost * 1000, 4).ToString();
            txtProductCostDzs.Text = Math.Round(productCost * 12, 4).ToString();
            fillDgvBomDetails();
        }

        private void txtProductCostGrs_Leave(object sender, EventArgs e)
        {
            fillDgvBomDetails();
        }

        private void txtProductCostK_Leave(object sender, EventArgs e)
        {
            fillDgvBomDetails();
        }
        private void txtProductCostDzs_Leave(object sender, EventArgs e)
        {
            fillDgvBomDetails();
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            txtDepId.Focus();
            if (!validData())
                return;
            save();
        }
        private bool validData()
        {
            bool selectFlag = false;
            for (int i = 0; i < dgvBomDetails.Rows.Count; i++)
            {
                DataGridViewRow dgr = dgvBomDetails.Rows[i];
                if ((bool)dgr.Cells["colIsSelect"].Value == true)
                {
                    selectFlag = true;
                    break;
                }
            }
            if (selectFlag == false)
                MessageBox.Show("沒有選定需儲存的記錄!");
            return selectFlag;
        }
        private void save()
        {
            string result = "";
            List<mdlProductCosting> lsModel = new List<mdlProductCosting>();
            for (int i = 0; i < dgvBomDetails.Rows.Count; i++)
            {
                DataGridViewRow dgr = dgvBomDetails.Rows[i];
                if ((bool)dgr.Cells["colIsSelect"].Value == true)
                {
                    bool isExist = false;
                    string productId = dgr.Cells["colProductId"].Value.ToString();
                    //如果物料編號已在列表中，則不再加入，以免重複加入
                    for (int j = 0; j < lsModel.Count; j++)
                    {
                        if (productId == lsModel[j].productId)
                        {
                            isExist = true;
                            break;
                        }
                    }
                    if (!isExist)
                    {
                        mdlProductCosting objModel = new mdlProductCosting();
                        objModel.productId = productId;
                        objModel.productMo = txtProductMo.Text.Trim();
                        objModel.rollUpCost = dgr.Cells["colRollUpCost"].Value.ToString() != "" ? Convert.ToDecimal(dgr.Cells["colRollUpCost"].Value) : 0;
                        objModel.productWeight = dgr.Cells["colProductWeight"].Value.ToString() != "" ? Convert.ToDecimal(dgr.Cells["colProductWeight"].Value) : 0;
                        objModel.originWeight = dgr.Cells["colOriginWeight"].Value.ToString() != "" ? Convert.ToDecimal(dgr.Cells["colOriginWeight"].Value) : 0;
                        objModel.wasteRate = dgr.Cells["colWasteRate"].Value.ToString() != "" ? Convert.ToDecimal(dgr.Cells["colWasteRate"].Value) : 0;
                        objModel.materialRequest = dgr.Cells["colMaterialRequest"].Value.ToString() != "" ? Convert.ToDecimal(dgr.Cells["colMaterialRequest"].Value) : 0;
                        objModel.originalPrice = dgr.Cells["colOriginalPrice"].Value.ToString() != "" ? Convert.ToDecimal(dgr.Cells["colOriginalPrice"].Value) : 0;
                        objModel.materialPrice = dgr.Cells["colMaterialPrice"].Value.ToString() != "" ? Convert.ToDecimal(dgr.Cells["colMaterialPrice"].Value) : 0;
                        objModel.materialCost = dgr.Cells["colMaterialCost"].Value.ToString() != "" ? Convert.ToDecimal(dgr.Cells["colMaterialCost"].Value) : 0;
                        objModel.depId = dgr.Cells["colDepId"].Value.ToString();
                        objModel.depPrice = dgr.Cells["colDepPrice"].Value.ToString() != "" ? Convert.ToDecimal(dgr.Cells["colDepPrice"].Value) : 0;
                        objModel.depCost = dgr.Cells["colDepCost"].Value.ToString() != "" ? Convert.ToDecimal(dgr.Cells["colDepCost"].Value) : 0;
                        objModel.depTotalCost = dgr.Cells["colDepTotalCost"].Value.ToString() != "" ? Convert.ToDecimal(dgr.Cells["colDepTotalCost"].Value) : 0;
                        objModel.depStdPrice = dgr.Cells["colDepStdPrice"].Value.ToString() != "" ? Convert.ToDecimal(dgr.Cells["colDepStdPrice"].Value) : 0;
                        objModel.depStdQty = dgr.Cells["colDepStdQty"].Value.ToString() != "" ? Convert.ToDecimal(dgr.Cells["colDepStdQty"].Value) : 0;
                        objModel.otherCost1 = dgr.Cells["colOtherCost1"].Value.ToString() != "" ? Convert.ToDecimal(dgr.Cells["colOtherCost1"].Value) : 0;
                        objModel.otherCost2 = dgr.Cells["colOtherCost2"].Value.ToString() != "" ? Convert.ToDecimal(dgr.Cells["colOtherCost2"].Value) : 0;
                        objModel.otherCost3 = dgr.Cells["colOtherCost3"].Value.ToString() != "" ? Convert.ToDecimal(dgr.Cells["colOtherCost3"].Value) : 0;
                        objModel.productCost = dgr.Cells["colProductCost"].Value.ToString() != "" ? Convert.ToDecimal(dgr.Cells["colProductCost"].Value) : 0;
                        objModel.productCostGrs = dgr.Cells["colProductCostGrs"].Value.ToString() != "" ? Convert.ToDecimal(dgr.Cells["colProductCostGrs"].Value) : 0;
                        objModel.productCostK = dgr.Cells["colProductCostK"].Value.ToString() != "" ? Convert.ToDecimal(dgr.Cells["colProductCostK"].Value) : 0;
                        objModel.productCostDzs = dgr.Cells["colProductCostDzs"].Value.ToString() != "" ? Convert.ToDecimal(dgr.Cells["colProductCostDzs"].Value) : 0;
                        objModel.createUser = DBUtility._user_id.ToUpper();
                        objModel.createTime = System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
                        lsModel.Add(objModel);
                    }
                }
            }
            result = clsProductCosting.updateProductCosting(lsModel);
            if (result == "")
            {
                MessageBox.Show("更新產品成本成功!");
                chkSelectAll.Checked = false;
            }
            else
                MessageBox.Show("更新產品成本失敗!");
        }

        //當本層的值改變后，判斷上層是否是原料、膠料、電鍍等，將本層單價帶到上層，并重新計算上層的成本
        //并重新從上層開始，倒序計算所有件的子件成本
        //如果是已設定的，就不再計算
        private void countProductCostingRoll(int currentRow)
        {
            if (currentRow == 0)
                return;
            int parentRow = currentRow - 1;
            //當本層是原料，要將單價帶到上層，并重新計算上層的原料需求及成本
            DataGridViewRow dgrParent = dgvBomDetails.Rows[parentRow];
            string currentLevel = dgrParent.Cells["colBomLevel"].Value.ToString();
            DataGridViewRow dgrCurrent = dgvBomDetails.Rows[currentRow];
            string parentLevel = dgrCurrent.Cells["colParentLevel"].Value.ToString();
            if (dgrCurrent.Cells["colProductId"].Value.ToString().Substring(0, 2) == "ML")
            {
                if (parentLevel == currentLevel)
                {
                    if ((bool)dgrParent.Cells["colIsSetFlag"].Value == false)//如果是已設定的，就不再計算
                    {
                        //上層的單價由本層得來
                        dgrParent.Cells["colOriginalPrice"].Value = dgrCurrent.Cells["colOriginalPrice"].Value;
                        dgrParent.Cells["colMaterialPrice"].Value = dgrCurrent.Cells["colMaterialPrice"].Value;
                        //dgrParent.Cells["colWasteRate"].Value = clsProductCosting.getDepWasteRate(dgrParent.Cells["colDepId"].Value.ToString());
                        dgrParent.Cells["colMaterialRequest"].Value = Math.Round((dgrParent.Cells["colProductWeight"].Value.ToString() != "" ? Convert.ToDecimal(dgrParent.Cells["colProductWeight"].Value) : 0)
                             * Convert.ToDecimal(dgrParent.Cells["colWasteRate"].Value)
                            , 4);
                        dgrParent.Cells["colMaterialCost"].Value = Math.Round(
                            (dgrParent.Cells["colMaterialRequest"].Value.ToString() != "" ? Convert.ToDecimal(dgrParent.Cells["colMaterialRequest"].Value.ToString()) : 0)
                            * (dgrParent.Cells["colMaterialPrice"].Value.ToString() != "" ? Convert.ToDecimal(dgrParent.Cells["colMaterialPrice"].Value.ToString()) : 0)
                            , 4);
                        countProductCostInGrid(parentRow);
                    }
                }
            }
            else if (dgrCurrent.Cells["colProductId"].Value.ToString().Substring(0, 2) == "PL")
            {
                if ((bool)dgrCurrent.Cells["colIsSetFlag"].Value == false)//如果是已設定的，就不再計算
                {
                    decimal wasteRate = 0;
                    wasteRate = dgrCurrent.Cells["colWasteRate"].Value.ToString() != "" ? Convert.ToDecimal(dgrCurrent.Cells["colWasteRate"].Value.ToString()) : 0;
                    wasteRate = wasteRate != 0 ? wasteRate : 1;
                    dgrCurrent.Cells["colMaterialCost"].Value = Math.Round(wasteRate
                        * (dgrCurrent.Cells["colMaterialPrice"].Value.ToString() != "" ? Convert.ToDecimal(dgrCurrent.Cells["colMaterialPrice"].Value.ToString()) : 0)
                        , 4);
                    countProductCostInGrid(currentRow);
                }
            }
            else if (dgrParent.Cells["colDepId"].Value.ToString() == "501" || dgrParent.Cells["colDepId"].Value.ToString() == "510")//
            {
                if (parentLevel == currentLevel)
                {
                    if ((bool)dgrParent.Cells["colIsSetFlag"].Value == false)//如果是已設定的，就不再計算
                    {
                        decimal wasteRate = 0;
                        dgrParent.Cells["colProductWeight"].Value = dgrCurrent.Cells["colProductWeight"].Value;//因為外發的金額是按照NEP計算的，所以要將上層的重量帶入到本層,作為本層的重量
                        dgrParent.Cells["colOriginWeight"].Value = dgrParent.Cells["colProductWeight"].Value;
                        wasteRate = dgrParent.Cells["colWasteRate"].Value.ToString() != "" ? Convert.ToDecimal(dgrParent.Cells["colWasteRate"].Value.ToString()) : 0;
                        //if (wasteRate == 0)
                        //    wasteRate = (decimal)1.1;
                        //噴油、挂電是按粒計算成本的，所以不用乘以重量
                        if (dgrParent.Cells["colDepId"].Value.ToString() == "510" || (dgrParent.Cells["colDepId"].Value.ToString() == "501" && dgrParent.Cells["colDoColor"].Value.ToString().IndexOf("挂電") > 0))
                            dgrParent.Cells["colMaterialCost"].Value = Math.Round(wasteRate
                            * (dgrParent.Cells["colMaterialPrice"].Value.ToString() != "" ? Convert.ToDecimal(dgrParent.Cells["colMaterialPrice"].Value.ToString()) : 0)
                            , 4);
                        else
                            dgrParent.Cells["colMaterialCost"].Value = Math.Round(
                        (dgrParent.Cells["colProductWeight"].Value.ToString() != "" ? Convert.ToDecimal(dgrParent.Cells["colProductWeight"].Value.ToString()) : 0)
                        * wasteRate
                        * (dgrParent.Cells["colMaterialPrice"].Value.ToString() != "" ? Convert.ToDecimal(dgrParent.Cells["colMaterialPrice"].Value.ToString()) : 0)
                        , 4);
                        countProductCostInGrid(parentRow);
                    }
                }
            }
            //從本層的上層開始，重新計算子件累計成本
            for (int i = parentRow; i >= 0; i--)
            {
                DataGridViewRow dgr = dgvBomDetails.Rows[i];
                if ((bool)dgr.Cells["colIsSetFlag"].Value == false)//如果是已設定的，就不再計算
                {
                    int nextRow = i + 1;
                    decimal rollUpCost = 0;
                    string upLevel = dgr.Cells["colBomLevel"].Value.ToString();
                    for (int j = nextRow; j < dgvBomDetails.Rows.Count; j++)
                    {
                        DataGridViewRow dgrRoll = dgvBomDetails.Rows[j];
                        if (upLevel == dgrRoll.Cells["colBomLevel"].Value.ToString())
                            break;
                        if (upLevel == dgrRoll.Cells["colParentLevel"].Value.ToString())
                            if (dgrRoll.Cells["colProductId"].Value.ToString().Substring(0, 2) != "ML")
                                rollUpCost += (dgrRoll.Cells["colProductCost"].Value.ToString() != "" ? Convert.ToDecimal(dgrRoll.Cells["colProductCost"].Value) : 0);
                    }

                    dgr.Cells["colRollUpCost"].Value = rollUpCost.ToString();
                    countProductCostInGrid(i);
                }
            }
        }

        //產品成本=各費用相加
        private void countProductCostInGrid(int row)
        {
            decimal productCost = 0;
            decimal depTotalCost = 0;
            DataGridViewRow dgr = dgvBomDetails.Rows[row];
            depTotalCost = (dgr.Cells["colMaterialCost"].Value.ToString() != "" ? Convert.ToDecimal(dgr.Cells["colMaterialCost"].Value) : 0)
                + (dgr.Cells["colDepCost"].Value.ToString() != "" ? Convert.ToDecimal(dgr.Cells["colDepCost"].Value) : 0)
                + (dgr.Cells["colOtherCost1"].Value.ToString() != "" ? Convert.ToDecimal(dgr.Cells["colOtherCost1"].Value) : 0)
                + (dgr.Cells["colOtherCost2"].Value.ToString() != "" ? Convert.ToDecimal(dgr.Cells["colOtherCost2"].Value) : 0)
                + (dgr.Cells["colOtherCost3"].Value.ToString() != "" ? Convert.ToDecimal(dgr.Cells["colOtherCost3"].Value) : 0);
            productCost = Math.Round(
                (dgr.Cells["colRollUpCost"].Value.ToString() != "" ? Convert.ToDecimal(dgr.Cells["colRollUpCost"].Value) : 0)
                + depTotalCost
                , 4);
            dgr.Cells["colDepTotalCost"].Value = Math.Round(depTotalCost, 4).ToString();
            dgr.Cells["colProductCost"].Value = productCost.ToString();
            dgr.Cells["colProductCostGrs"].Value = Math.Round(productCost * 144, 4).ToString();
            dgr.Cells["colProductCostK"].Value = Math.Round(productCost * 1000, 4).ToString();
            dgr.Cells["colProductCostDzs"].Value = Math.Round(productCost * 12, 4).ToString();
        }

        #region 此段為累計成本,暫不用
        //當值改變時，自動重新計算每件的子件累計成本及產品成本，一直計算到頂層F0，不用人手計算每一層
        //從表格的當前記錄的前一筆記錄開始，倒序計算每件的：子件累計成本=該件對應的下一層的產品成本之和
        //重新累加該件的產品成本
        private void countAllItemCost(int startRow)
        {
            //DataGridViewRow dgrCurrent = dgvBomDetails.Rows[row];
            //string parentSeq = dgrCurrent.Cells["colParentLevel"].Value.ToString();
            for (int i = startRow - 1; i >= 0; i--)
            {
                decimal rollUpCost = 0;
                DataGridViewRow dgr = dgvBomDetails.Rows[i];
                string currentLevel = dgr.Cells["colBomLevel"].Value.ToString();
                rollUpCost = countChildCost(startRow,i, currentLevel);
                dgr.Cells["colRollUpCost"].Value = rollUpCost.ToString();
                countProductCostInGrid(i);
             }
            
        }
        //計算子件累計成本=該件對應的下一層的產品成本之和
        private decimal countChildCost(int startRow,int row, string currentLevel)
        {
            decimal rollUpCost = 0;
            for (int i = row + 1; i < dgvBomDetails.Rows.Count; i++)
            {
                DataGridViewRow dgr1 = dgvBomDetails.Rows[i];
                string nowLevel = dgr1.Cells["colBomLevel"].Value.ToString();
                string parentLevel = dgr1.Cells["colParentLevel"].Value.ToString();
                if (currentLevel == nowLevel)
                    break;
                else
                {
                    if (currentLevel == parentLevel)
                    {
                        decimal costNoMaterial = 0;
                        //因為上層的產品開料是有損耗的，所以不能直接將本層的原料成本帶給上層，只需將本層的原料單價帶給上層，由上層的原料需求乘以單價，即為上層的原料成本
                        //如果本層是原料，則只將除了“原料單價”的產品成本帶到上層，而單價就單獨帶到上層，乘以上層的原料需求，得出上層的原料成本
                        //并將上一層的單價改為本層的單價，計算出上一層的原料成本
                        costNoMaterial = Math.Round(
                                (dgr1.Cells["colRollUpCost"].Value.ToString() != "" ? Convert.ToDecimal(dgr1.Cells["colRollUpCost"].Value) : 0)
                                + (dgr1.Cells["colDepCost"].Value.ToString() != "" ? Convert.ToDecimal(dgr1.Cells["colDepCost"].Value) : 0)
                                + (dgr1.Cells["colOtherCost1"].Value.ToString() != "" ? Convert.ToDecimal(dgr1.Cells["colOtherCost1"].Value) : 0)
                                + (dgr1.Cells["colOtherCost2"].Value.ToString() != "" ? Convert.ToDecimal(dgr1.Cells["colOtherCost2"].Value) : 0)
                                + (dgr1.Cells["colOtherCost3"].Value.ToString() != "" ? Convert.ToDecimal(dgr1.Cells["colOtherCost3"].Value) : 0)
                                , 4);
                        //當本層是原料，要將單價帶到上層，并重新計算上層的原料需求及成本
                        if (dgr1.Cells["colProductId"].Value.ToString().Substring(0, 2) == "ML")
                        {
                            if (i == startRow || firstCount == true)
                            {
                                rollUpCost += costNoMaterial;
                                //上層的單價由本層得來
                                dgvBomDetails.Rows[row].Cells["colOriginalPrice"].Value = dgr1.Cells["colOriginalPrice"].Value;
                                dgvBomDetails.Rows[row].Cells["colMaterialPrice"].Value = dgr1.Cells["colMaterialPrice"].Value;
                                //dgvBomDetails.Rows[row].Cells["colWasteRate"].Value = 1.4;
                                dgvBomDetails.Rows[row].Cells["colMaterialRequest"].Value = Math.Round((dgvBomDetails.Rows[row].Cells["colProductWeight"].Value.ToString() != "" ? Convert.ToDecimal(dgvBomDetails.Rows[row].Cells["colProductWeight"].Value) : 0)
                                     * Convert.ToDecimal(dgvBomDetails.Rows[row].Cells["colWasteRate"].Value)
                                    , 4);
                                dgvBomDetails.Rows[row].Cells["colMaterialCost"].Value = Math.Round(
                                    (dgvBomDetails.Rows[row].Cells["colMaterialRequest"].Value.ToString() != "" ? Convert.ToDecimal(dgvBomDetails.Rows[row].Cells["colMaterialRequest"].Value.ToString()) : 0)
                                    * (dgvBomDetails.Rows[row].Cells["colMaterialPrice"].Value.ToString() != "" ? Convert.ToDecimal(dgvBomDetails.Rows[row].Cells["colMaterialPrice"].Value.ToString()) : 0)
                                    , 4);
                            }
                        }
                        else
                        {
                            if (dgr1.Cells["colProductId"].Value.ToString().Substring(0, 2) == "PL")
                            {
                                if (i == startRow || firstCount == true)
                                {
                                    decimal materialCost = 0;
                                    decimal wasteRate = 0;
                                    wasteRate = dgr1.Cells["colWasteRate"].Value.ToString() != "" ? Convert.ToDecimal(dgr1.Cells["colWasteRate"].Value.ToString()) : 0;
                                    wasteRate = wasteRate != 0 ? wasteRate : 1;
                                    materialCost = Math.Round(wasteRate
                                        * (dgr1.Cells["colMaterialPrice"].Value.ToString() != "" ? Convert.ToDecimal(dgr1.Cells["colMaterialPrice"].Value.ToString()) : 0)
                                        , 4);
                                    dgr1.Cells["colMaterialCost"].Value = materialCost;
                                    dgr1.Cells["colProductCost"].Value = costNoMaterial + materialCost;
                                    rollUpCost = rollUpCost + costNoMaterial + materialCost;
                                }
                            }
                            else
                            {
                                if (dgvBomDetails.Rows[row].Cells["colDepId"].Value.ToString() == "501" || dgvBomDetails.Rows[row].Cells["colDepId"].Value.ToString() == "510")//
                                {
                                    if (i == startRow || firstCount == true)
                                    {
                                        decimal materialCost = 0;
                                        decimal wasteRate = 0;
                                        dgvBomDetails.Rows[row].Cells["colProductWeight"].Value = dgr1.Cells["colProductWeight"].Value;
                                        wasteRate = dgvBomDetails.Rows[row].Cells["colWasteRate"].Value.ToString() != "" ? Convert.ToDecimal(dgvBomDetails.Rows[row].Cells["colWasteRate"].Value.ToString()) : 0;
                                        //if (wasteRate == 0)
                                        //    wasteRate = (decimal)1.1;
                                        materialCost = Math.Round(
                                        (dgvBomDetails.Rows[row].Cells["colProductWeight"].Value.ToString() != "" ? Convert.ToDecimal(dgvBomDetails.Rows[row].Cells["colProductWeight"].Value.ToString()) : 0)
                                        * wasteRate
                                        * (dgvBomDetails.Rows[row].Cells["colMaterialPrice"].Value.ToString() != "" ? Convert.ToDecimal(dgvBomDetails.Rows[row].Cells["colMaterialPrice"].Value.ToString()) : 0)
                                        , 4);
                                        dgvBomDetails.Rows[row].Cells["colMaterialCost"].Value = materialCost;
                                        dgvBomDetails.Rows[row].Cells["colProductCost"].Value = costNoMaterial + materialCost;
                                        rollUpCost = rollUpCost + costNoMaterial + materialCost;
                                    }
                                }
                                else
                                    //decimal materialCost=0;
                                    rollUpCost += (dgr1.Cells["colProductCost"].Value.ToString() != "" ? Convert.ToDecimal(dgr1.Cells["colProductCost"].Value) : 0);
                                //bool isFirstLevel = getFirstLevel(currentLevel, row);//判斷當層是否最開始
                                //if (isFirstLevel)
                                //    materialCost = Math.Round(
                                //    (dgr1.Cells["colMaterialRequest"].Value.ToString() != "" ? Convert.ToDecimal(dgr1.Cells["colMaterialRequest"].Value.ToString()) : 0)
                                //    * (dgr1.Cells["colMaterialPrice"].Value.ToString() != "" ? Convert.ToDecimal(dgr1.Cells["colMaterialPrice"].Value.ToString()) : 0)
                                //    , 4);
                                //else
                                //{
                                //    decimal wasteRate = 0;
                                //    wasteRate = dgr1.Cells["colWasteRate"].Value.ToString() != "" ? Convert.ToDecimal(dgr1.Cells["colWasteRate"].Value.ToString()) : 0;
                                //    wasteRate = wasteRate != 0 ? wasteRate : 1;
                                //    materialCost = Math.Round(wasteRate
                                //        * (dgr1.Cells["colMaterialPrice"].Value.ToString() != "" ? Convert.ToDecimal(dgr1.Cells["colMaterialPrice"].Value.ToString()) : 0)
                                //        , 4);
                                //}
                                //rollUpCost = rollUpCost + costNoMaterial + materialCost;
                            }
                        }
                    }
                }
            }
            return rollUpCost;
        }

        #endregion


        

        private void btnFindPrdPrice_Click(object sender, EventArgs e)
        {
            searchProductId = txtProductId.Text;
            searchProductName = txtProductName.Text;
            searchDepId = txtDepId.Text;
            searchPrice = 0;
            frmProductCostingFindPrice frmProductCostingFindPrice = new frmProductCostingFindPrice();
            frmProductCostingFindPrice.ShowDialog();
            if (searchPrice != 0)
            {
                txtOriginalPrice.Text = searchPrice.ToString();
                txtOriginalPrice_Leave(sender, e);
            }
            frmProductCostingFindPrice.Dispose();
        }

        private void btnFindDepCost_Click(object sender, EventArgs e)
        {
            searchProductId = txtProductId.Text;
            searchProductName = txtProductName.Text;
            searchDepId = txtDepId.Text;
            searchPrice = 0;
            frmProductProcessCost frmProductProcessCost = new frmProductProcessCost();
            frmProductProcessCost.ShowDialog();
            if (searchPrice != 0)
            {
                //txtDepPrice.Text = searchPrice.ToString();
                int row=dgvBomDetails.CurrentRow.Index;
                dgvBomDetails.Rows[row].Cells["colDepStdPrice"].Value = sentDepPrice.depStdPrice;
                dgvBomDetails.Rows[row].Cells["colDepStdQty"].Value = sentDepPrice.depStdQty;
                txtDepPrice.Text = sentDepPrice.depPrice.ToString();
                txtDepPrice_Leave(sender, e);
            }
            frmProductProcessCost.Dispose();
        }

        private void dgvBomDetails_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            //btnFindDepCost_Click(sender, e);
        }

        private void chkSelectAll_CheckedChanged(object sender, EventArgs e)
        {
            bool chkFlag = chkSelectAll.Checked;
            for (int i = 0; i < dgvBomDetails.Rows.Count; i++)
            {
                DataGridViewRow dgr = dgvBomDetails.Rows[i];
                dgr.Cells["colIsSelect"].Value = chkFlag;
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            xrProductCosting oRepot = new xrProductCosting() { DataSource = dtBomDetails };
            oRepot.CreateDocument();
            oRepot.PrintingSystem.ShowMarginsWarning = false;
            oRepot.ShowPreview();
        }

        private void dgvBomDetails_SelectionChanged(object sender, EventArgs e)
        {
            fillControlsValue(dgvBomDetails.CurrentRow.Index);
        }

        private void btnDepWasteRate_Click(object sender, EventArgs e)
        {
            frmDepWasteRate frmDepWasteRate = new frmDepWasteRate();
            frmDepWasteRate.ShowDialog();
            frmDepWasteRate.Dispose();
        }

        private void btnShowRemark_Click(object sender, EventArgs e)
        {
            if (btnShowRemark.Text == "顯示說明(&I)")
            {
                btnShowRemark.Text = "隱藏說明(&I)";
                panelControl4.Visible = true;
            }
            else
            {
                btnShowRemark.Text = "顯示說明(&I)";
                panelControl4.Visible = false;
            }
        }

    }
}
